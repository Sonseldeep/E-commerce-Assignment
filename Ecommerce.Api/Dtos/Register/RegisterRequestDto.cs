namespace Ecommerce.Api.Dtos.Register;

public class RegisterRequestDto
{
    public required string UserName { get; set; }
    public required string Name { get; set; }
    public required string Password { get; set; }
   
}