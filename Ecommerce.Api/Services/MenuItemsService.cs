using Ecommerce.Api.Models;
using Ecommerce.Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Ecommerce.Api.Services;

public class MenuItemsService : IMenuItemsService
{
    private readonly IMenueItemsRepository _repository;

    public MenuItemsService(IMenueItemsRepository repository)
    {
        _repository = repository;
    }
    public async Task<IEnumerable<MenuItem>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
        
    }

    public async Task<MenuItem?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }
}