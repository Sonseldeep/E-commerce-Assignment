using Ecommerce.Api.Data;
using Ecommerce.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Ecommerce.Api.Controllers;

[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly ApiResponse _response;

    public PaymentController(IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
        _response = new ApiResponse();
    }

    [HttpPost("api/payment")]
    public async Task<ActionResult<ApiResponse>> MakePayment(string userId)
    {
        var shoppingCart = await _context.ShoppingCarts
            .Include(x => x.CartItems)
            .ThenInclude(x => x.MenuItem)
            .SingleOrDefaultAsync(x => x.UserId == userId);

        if (shoppingCart is null || shoppingCart.CartItems.Count == 0)
        {
            _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Your cart is empty.");
            return BadRequest(_response);
        }

        #region Create Payment Intent
        try
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"]; shoppingCart.CartTotal = shoppingCart.CartItems
                .Sum(x => 
                x.Quantity * x.MenuItem.Price);
            
            var options = new PaymentIntentCreateOptions
            {
                Amount = (int)(shoppingCart.CartTotal * 100), // amount in cents
                Currency = "usd",
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            shoppingCart.StripePaymentIntentId = paymentIntent.Id;
            shoppingCart.ClientSecret = paymentIntent.ClientSecret;
            await _context.SaveChangesAsync();
        }
        
        catch (Exception ex)
        {
            _response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add($"Payment failed: {ex.Message}");
            return StatusCode(500, _response);
        }
        
        #endregion

        _response.IsSuccess = true;
        _response.Result = shoppingCart;
        _response.StatusCode = System.Net.HttpStatusCode.OK;
        return Ok(_response);
    }
}