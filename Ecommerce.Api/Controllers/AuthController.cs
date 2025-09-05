using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Ecommerce.Api.Data;
using Ecommerce.Api.Dtos.Login;
using Ecommerce.Api.Dtos.Register;
using Ecommerce.Api.Models;
using Ecommerce.Api.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Api.Controllers;

[ApiController]

public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private ApiResponse _response;
    private string _secretKey;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    

    public AuthController(ApplicationDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _secretKey = configuration.GetValue<string>("ApiSettings:Secret")!;
        _response = new ApiResponse();
    }

    [HttpPost(Endpoints.ApiEndpoints.Auth.Login)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
    {
        // check if user exists
        var userFromDb = await _context.ApplicationUsers.SingleOrDefaultAsync(x => x.UserName !=null && x.UserName.ToLower() == model.UserName.ToLower());
        
        //is exists, check if password is correct
        var isValid = await _userManager.CheckPasswordAsync(userFromDb, model.Password);
        if (!isValid)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username or password is incorrect.");
            return BadRequest(_response);
        }
        // we have to generate JWT token
        var roles = await _userManager.GetRolesAsync(userFromDb);
        JwtSecurityTokenHandler tokenHandler = new();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity([
                new Claim("fullname", userFromDb.Name),
                new Claim("id", userFromDb.Id.ToString()),
                new Claim(ClaimTypes.Email, userFromDb.UserName?.ToString()!),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault()!)

            ]),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        
        
        LoginResponseDto loginResponse = new()
        {
            Email = model.UserName,
            Token = tokenHandler.WriteToken(token),
        };
        
        if (string.IsNullOrEmpty(loginResponse.Token))
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Username or password is incorrect.");
            return BadRequest(_response);
        }
        
        _response.StatusCode = HttpStatusCode.OK;
        _response.IsSuccess = true;
        _response.Result = loginResponse;
        return Ok(_response);

    } 

  [HttpPost(Endpoints.ApiEndpoints.Auth.Register)]
public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
{
    var userFromDb = await _context.ApplicationUsers
        .SingleOrDefaultAsync(x => 
            x.UserName != null && x.UserName.ToLower() == model.UserName.ToLower());
    
    if (userFromDb is not null)
    {
        _response.StatusCode = HttpStatusCode.BadRequest;
        _response.IsSuccess = false;
        _response.ErrorMessages.Add("Username already exists.");
        return BadRequest(_response);
    }

    ApplicationUser newUser = new()
    {
        UserName = model.UserName,
        Email = model.UserName, 
        NormalizedUserName = model.UserName.ToUpper(),
        Name = model.Name,
    };

    try
    {
        var result = await _userManager.CreateAsync(newUser, model.Password);
        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync(Sd.RoleAdmin))
            {
                // create roles in db
                await _roleManager.CreateAsync(new IdentityRole(Sd.RoleAdmin));
                await _roleManager.CreateAsync(new IdentityRole(Sd.RoleCustomer));
            }

            if (model.Role.Equals(Sd.RoleCustomer, StringComparison.OrdinalIgnoreCase))
            {
                await _userManager.AddToRoleAsync(newUser, Sd.RoleCustomer);
            }
            else
            {
                await _userManager.AddToRoleAsync(newUser, Sd.RoleAdmin);
            }
        
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }

        _response.StatusCode = HttpStatusCode.BadRequest;
        _response.IsSuccess = false;
        _response.ErrorMessages.Add("Error while registering.");
        return BadRequest(_response);
    }
    catch (Exception)
    {
        _response.StatusCode = HttpStatusCode.BadRequest;
        _response.IsSuccess = false;
        _response.ErrorMessages.Add("Error while registering.");
        return BadRequest(_response);
    }
}

}