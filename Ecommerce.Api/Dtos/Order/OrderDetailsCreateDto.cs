using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Api.Models;

namespace Ecommerce.Api.Dtos.Order;

public class OrderDetailsCreateDto
{
    public int MenuItemId { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public string ItemName { get; set; }
    [Required]
    public double  Price { get; set; }
}