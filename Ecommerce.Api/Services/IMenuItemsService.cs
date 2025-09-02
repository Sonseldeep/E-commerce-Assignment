using Ecommerce.Api.Models;

namespace Ecommerce.Api.Services;

public interface IMenuItemsService
{
    Task<IEnumerable<MenuItem>> GetAllAsync();
    Task<MenuItem?> GetByIdAsync(int id);
}