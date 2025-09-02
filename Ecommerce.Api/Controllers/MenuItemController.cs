using Ecommerce.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
public class MenuItemController : ControllerBase
{
    private readonly IMenueItemsRepository _services;

    public MenuItemController(IMenueItemsRepository services)
    {
        _services = services;
    }


    [HttpGet(Endpoints.ApiEndpoints.MenuItems.GetAll)]
    public async Task<IActionResult> GetMenuItems()
    {
        var menuItems = await _services.GetAllAsync();
        return Ok(menuItems);
    }
    
    [HttpGet(Endpoints.ApiEndpoints.MenuItems.Get)]
    public async Task<IActionResult> GetMenuItemById([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Invalid Id");
        }

        var menuItems = await _services.GetByIdAsync(id);
        
        return menuItems is null ? NotFound() : Ok(menuItems);
    }
}