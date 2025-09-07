using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ecommerce.Api.Models;

namespace Ecommerce.Api.Dtos.Order;

public class OrderHeaderUpdateDto
{
    
    public int OrderHeaderId { get; set; }
    public string PickupName { get; set; }
    public string PickupPhoneNumber { get; set; }
    public string PickupEmail { get; set; }
    
    public string StripePaymentIntentID { get; set; }
    public string Status { get; set; }
    

}