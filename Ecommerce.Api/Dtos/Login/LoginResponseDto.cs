namespace Ecommerce.Api.Dtos.Login;

public class LoginResponseDto
{
    public required string Email { get; set; }
    public required string Token { get; set; }
}