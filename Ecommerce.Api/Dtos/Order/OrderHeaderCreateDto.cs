using System.ComponentModel.DataAnnotations;
using Ecommerce.Api.Models;

namespace Ecommerce.Api.Dtos.Order;

public class OrderHeaderCreateDto
{
    [Required]
    public string PickupName { get; set; }
    [Required]
    public string PickupPhoneNumber { get; set; }
    [Required]
    public string PickupEmail { get; set; }
   
    public string ApplicationUserId { get; set; }
    public double OrderTotal { get; set; }
    
    public string StripePaymentIntentID { get; set; }
    public string Status { get; set; }
    public int TotalItems { get; set; }

    public IEnumerable<OrderDetailsCreateDto> OrderDetailsDto { get; set; }
}