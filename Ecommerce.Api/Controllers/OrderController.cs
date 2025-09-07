using System.Net;
using Ecommerce.Api.Data;
using Ecommerce.Api.Models;
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
            var orderHeaders = _context.OrderHeaders.Include(x => x.OrderDetails).ThenInclude(x => x.MenuItem)
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
    
    
    
    [HttpGet("api/orders/{userId:int}")]
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
}