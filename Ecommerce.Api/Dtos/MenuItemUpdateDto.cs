using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Api.Dtos;

public class MenuItemUpdateDto
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? SpecialTag { get; set; }
    public required string Category { get; set; }
    
    [Range(1,int.MaxValue)]
    public required double Price { get; set; }
    public required IFormFile File { get; set; }
}