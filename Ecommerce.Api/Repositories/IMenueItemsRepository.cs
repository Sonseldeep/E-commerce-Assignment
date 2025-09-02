using Ecommerce.Api.Models;

namespace Ecommerce.Api.Repositories;

public interface IMenueItemsRepository
{
    Task<IEnumerable<MenuItem>> GetAllAsync();
    Task<MenuItem?> GetByIdAsync(int id);
}