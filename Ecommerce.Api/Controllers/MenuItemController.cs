using Ecommerce.Api.Data;
using Ecommerce.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Controllers;

[ApiController]
public class MenuItemController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public MenuItemController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("api/menuitems")]
    public async Task<IActionResult> GetMenuItems()
    {
        var menuItems = await _context.MenuItems.ToListAsync();
        return Ok(menuItems);
    }
    
    [HttpGet("api/menuitems/{id:int}")]
    public async Task<IActionResult> GetMenuItemById([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }
        var menuItems = await _context.MenuItems.SingleOrDefaultAsync(m => m.Id == id);
        
        return menuItems is null ? NotFound() : Ok(menuItems);
    }
}