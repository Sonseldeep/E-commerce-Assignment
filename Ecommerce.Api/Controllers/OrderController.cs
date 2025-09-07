using System.Net;
using Ecommerce.Api.Data;
using Ecommerce.Api.Dtos.Order;
using Ecommerce.Api.Models;
using Ecommerce.Api.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Controllers;

[ApiController]
public class OrderController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ApiResponse _response;

    public OrderController(ApplicationDbContext context)
    {
        _context = context;
        _response = new ApiResponse();
    }

    [HttpGet("api/orders")]
    public async Task<ActionResult<ApiResponse>> GetOrders(string? userId)
    {
        try
        {
            var orderHeaders =  _context.OrderHeaders.Include(x => x.OrderDetails).ThenInclude(x => x.MenuItem)
                .OrderByDescending(x => x.OrderHeaderId);
            if (!string.IsNullOrEmpty(userId))
            {
                _response.Result = orderHeaders.Where(x => x.ApplicationUserId == userId);
            }
            else
            {
                _response.Result = orderHeaders;
            }
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages = [ex.ToString()];
            
        }
        return _response;
    }
    
    
    
    [HttpGet("api/orders/{id:int}")]
    public async Task<ActionResult<ApiResponse>> GetOrderById(int id)
    {
        try
        {
            if (id <= 0)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
            var orderHeaders = _context.OrderHeaders.Include(x => x.OrderDetails).ThenInclude(x => x.MenuItem)
                .Where(x => x.OrderHeaderId == id);

            _response.Result = orderHeaders;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
        catch (Exception ex)
        {
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.IsSuccess = false;
            _response.ErrorMessages = [ex.ToString()];
            
        }
        return _response;
    }

   [HttpPost("api/orders")]
public async Task<ActionResult<ApiResponse>> CreateOrder([FromBody] OrderHeaderCreateDto orderHeaderDto)
{
    try
    {
        if (!ModelState.IsValid)
        {
            _response.IsSuccess = false;
            _response.StatusCode = HttpStatusCode.BadRequest;
            _response.ErrorMessages = ["Invalid model state"];
            return BadRequest(_response);
        }

        // Create OrderHeader
        OrderHeader order = new()
        {
            ApplicationUserId = orderHeaderDto.ApplicationUserId,
            PickupEmail = orderHeaderDto.PickupEmail,
            PickupName = orderHeaderDto.PickupName,
            PickupPhoneNumber = orderHeaderDto.PickupPhoneNumber,
            OrderTotal = orderHeaderDto.OrderTotal,
            OrderDate = DateTime.Now,
            StripePaymentIntentID = orderHeaderDto.StripePaymentIntentID,
            TotalItems = orderHeaderDto.TotalItems,
            Status = string.IsNullOrEmpty(orderHeaderDto.Status) ? Sd.statusPending : orderHeaderDto.Status
        };

        _context.OrderHeaders.Add(order);
        await _context.SaveChangesAsync();

        // Validate and insert OrderDetails
        if (orderHeaderDto.OrderDetailsDto.Any())
        {
            foreach (var detailDto in orderHeaderDto.OrderDetailsDto)
            {
                // ✅ Validate MenuItemId exists
                var exists = await _context.MenuItems.AnyAsync(m => m.Id == detailDto.MenuItemId);
                if (!exists)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = [$"MenuItemId {detailDto.MenuItemId} does not exist."];
                    return BadRequest(_response);
                }

                OrderDetails detail = new()
                {
                    OrderHeaderId = order.OrderHeaderId,
                    ItemName = detailDto.ItemName,
                    MenuItemId = detailDto.MenuItemId,
                    Price = detailDto.Price,
                    Quantity = detailDto.Quantity
                };
                _context.OrderDetails.Add(detail);
            }

            await _context.SaveChangesAsync();
        }

        // Return response
        order.OrderDetails = null!; // prevent circular reference in JSON
        _response.Result = order;
        _response.StatusCode = HttpStatusCode.Created;
        return Ok(_response);
    }
    catch (Exception ex)
    {
        _response.IsSuccess = false;
        _response.StatusCode = HttpStatusCode.BadRequest;
        _response.ErrorMessages = [ex.Message];
        return BadRequest(_response);
    }
}

}