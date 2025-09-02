using Ecommerce.Api.Data;
using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Repositories;

public class MenuItemRepository : IMenueItemsRepository
{
    private readonly ApplicationDbContext _context;

    public MenuItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MenuItem>> GetAllAsync()
    {
       var menuItems = await   _context.MenuItems.AsNoTracking().ToListAsync();
       return menuItems;
    }

    public async Task<MenuItem?> GetByIdAsync(int id)
    {
        var existingMenuItem = await _context.MenuItems.SingleOrDefaultAsync(m => m.Id == id);
        return existingMenuItem;
    }
}