using Microsoft.AspNetCore.Identity;

namespace Ecommerce.Api.Models;

public class ApplicationUser : IdentityUser
{
    public required string Name { get; set; } = string.Empty;
}