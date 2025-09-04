using System.Net;
using Ecommerce.Api.Data;
using Ecommerce.Api.Dtos;
using Ecommerce.Api.Models;
using Ecommerce.Api.Services;
using Ecommerce.Api.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Controllers;

[ApiController]
public class MenuItemController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IBlobService _blobService;
    private readonly ApiResponse _response;

    public MenuItemController(ApplicationDbContext context, IBlobService blobService)
    {
        _context = context;
        _blobService = blobService;
        _response = new ApiResponse();
    }

    private BadRequestObjectResult SetBadRequestResponse(string message = "Invalid request.")
    {
        _response.StatusCode = HttpStatusCode.BadRequest;
        _response.IsSuccess = false;
        _response.ErrorMessages = [message];
        return BadRequest(_response);
    }

    [HttpGet(Endpoints.ApiEndpoints.MenuItems.GetAll)]
    public async Task<IActionResult> GetMenuItems()
    {
        _response.Result = await _context.MenuItems.AsNoTracking().ToListAsync();
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
    }

    [HttpGet(Endpoints.ApiEndpoints.MenuItems.Get)]
    public async Task<IActionResult> GetMenuItem([FromRoute] int id)
    {
        if (id <= 0)
        {
            return SetBadRequestResponse("Menu item ID must be greater than zero.");
        }

        var menuItem = await _context.MenuItems.SingleOrDefaultAsync(x => x.Id == id);

        if (menuItem is null)
        {
            _response.StatusCode = HttpStatusCode.NotFound;
            _response.IsSuccess = false;
            _response.ErrorMessages = ["Menu item not found."];
            return NotFound(_response);
        }

        _response.Result = menuItem;
        _response.StatusCode = HttpStatusCode.OK;
        return Ok(_response);
    }

    [HttpPost(Endpoints.ApiEndpoints.MenuItems.Create)]
    public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm] MenuItemCreateDto menuItemCreateDto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (menuItemCreateDto.File.Length == 0)
                {
                    return SetBadRequestResponse("Image file is required.");
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemCreateDto.File.FileName)}";
                MenuItem menuItemToCreate = new()
                {
                    Name = menuItemCreateDto.Name,
                    Price = menuItemCreateDto.Price,
                    Category = menuItemCreateDto.Category,
                    SpecialTag = menuItemCreateDto.SpecialTag,
                    Description = menuItemCreateDto.Description,
                    Image = await _blobService.UploadBlob(fileName, Sd.SdStorageContainer, menuItemCreateDto.File)
                };
                
                await _context.MenuItems.AddAsync(menuItemToCreate);
                await _context.SaveChangesAsync();
                
                _response.Result = menuItemToCreate;
                _response.StatusCode = HttpStatusCode.Created;
                return CreatedAtAction(nameof(GetMenuItem), new { id = menuItemToCreate.Id }, _response);
            }
            return SetBadRequestResponse();
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [e.ToString()];
        }
        return _response;
    }

    [HttpPut(Endpoints.ApiEndpoints.MenuItems.Update)]
    public async Task<ActionResult<ApiResponse>> UpdateMenuItem([FromRoute] int id, MenuItemUpdateDto menuItemUpdateDto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (id != menuItemUpdateDto.Id)
                {
                    return SetBadRequestResponse("Route ID and DTO ID do not match.");
                }

                var menuItemFromDb = await _context.MenuItems.FindAsync(id);
                if (menuItemFromDb is null)
                {
                    return SetBadRequestResponse("Menu item not found.");
                }

                menuItemFromDb.Name = menuItemUpdateDto.Name;
                menuItemFromDb.Price = menuItemUpdateDto.Price;
                menuItemFromDb.Category = menuItemUpdateDto.Category;
                menuItemFromDb.SpecialTag = menuItemUpdateDto.SpecialTag;
                menuItemFromDb.Description = menuItemUpdateDto.Description;

                if (menuItemUpdateDto.File.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(menuItemUpdateDto.File.FileName)}";
                    await _blobService.DeleteBlob(menuItemFromDb.Image.Split('/').Last(), Sd.SdStorageContainer);
                    menuItemFromDb.Image = await _blobService.UploadBlob(fileName, Sd.SdStorageContainer, menuItemUpdateDto.File);
                }

                _context.MenuItems.Update(menuItemFromDb);
                await _context.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            return SetBadRequestResponse();
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [e.ToString()];
        }
        return _response;
    }

    [HttpDelete(Endpoints.ApiEndpoints.MenuItems.Delete)]
    public async Task<ActionResult<ApiResponse>> DeleteMenuItem([FromRoute] int id)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    return SetBadRequestResponse("Menu item ID must be greater than zero.");
                }

                var menuItemFromDb = await _context.MenuItems.FindAsync(id);
                if (menuItemFromDb is null)
                {
                    return SetBadRequestResponse("Menu item not found.");
                }

                await _blobService.DeleteBlob(menuItemFromDb.Image.Split('/').Last(), Sd.SdStorageContainer);

                _context.MenuItems.Remove(menuItemFromDb);
                await _context.SaveChangesAsync();
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            return SetBadRequestResponse();
        }
        catch (Exception e)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [e.ToString()];
        }
        return _response;
    }
}



