using System.Net;
using Ecommerce.Api.Data;
using Ecommerce.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Controllers;

[ApiController]
public class ShoppingCartController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    protected ApiResponse _response;

    public ShoppingCartController(ApplicationDbContext context)
    {
        _response = new ApiResponse();
        _context = context;
    }

    [HttpGet("api/shoppingcart")]
    public async Task<ActionResult<ApiResponse>> GetShoppingCart(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            var shoppingCart = await  _context.ShoppingCarts.Include(x => x.CartItems).ThenInclude(x => x.MenuItem)
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (shoppingCart?.CartItems is not null && shoppingCart.CartItems.Count > 0)
            {
                shoppingCart.CartTotal = shoppingCart.CartItems.Sum(x => x.Quantity * x.MenuItem.Price);
            }
            
            _response.Result = shoppingCart;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);

        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.ErrorMessages = [ex.ToString()];
            _response.StatusCode = HttpStatusCode.BadRequest;
        }
        return _response;
    }

    [HttpPost("api/shoppingcart")]
    public async Task<ActionResult<ApiResponse>> AddOrUpdateItemInCart(string userId, int menuItemId, int updateQuantityBy)
    {
        var shoppingCart = await _context.ShoppingCarts
            .Include(x => x.CartItems)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        var menuItem = await _context.MenuItems.FirstOrDefaultAsync(s => s.Id == menuItemId);

        if (menuItem is null)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages.Add("Invalid MenuItemId");
            return BadRequest(_response);
        }

        if (shoppingCart is null && updateQuantityBy > 0)
        {
            // create a new shopping cart
            ShoppingCart newCart = new() { UserId = userId };
            _context.ShoppingCarts.Add(newCart);
            await _context.SaveChangesAsync();

            CartItem newCartItem = new()
            {
                MenuItemId = menuItemId,
                Quantity = updateQuantityBy,
                ShoppingCartId = newCart.Id
            };

            _context.CartItems.Add(newCartItem);
            await _context.SaveChangesAsync();

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = newCart;
            return Ok(_response);
        }

        if (shoppingCart is not null)
        {
            var cartItemInCart = shoppingCart.CartItems.FirstOrDefault(x => x.MenuItemId == menuItemId);
            if (cartItemInCart is null && updateQuantityBy > 0)
            {
                CartItem newCartItem = new()
                {
                    MenuItemId = menuItemId,
                    Quantity = updateQuantityBy,
                    ShoppingCartId = shoppingCart.Id
                };
                _context.CartItems.Add(newCartItem);
                await _context.SaveChangesAsync();
            }
            else if (cartItemInCart is not null)
            {
                var newQuantity = cartItemInCart.Quantity + updateQuantityBy;
                if (updateQuantityBy == 0 || newQuantity <= 0)
                {
                    _context.CartItems.Remove(cartItemInCart);

                    if (shoppingCart.CartItems.Count == 1) // removing last item → remove cart too
                    {
                        _context.ShoppingCarts.Remove(shoppingCart);
                    }

                    await _context.SaveChangesAsync();
                }
                else
                {
                    cartItemInCart.Quantity = newQuantity;
                    await _context.SaveChangesAsync();
                }
            }

            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = shoppingCart;
            return Ok(_response);
        }

        _response.StatusCode = HttpStatusCode.BadRequest;
        _response.IsSuccess = false;
        return BadRequest(_response);
    }
}
