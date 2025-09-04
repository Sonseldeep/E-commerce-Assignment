using System.Net;

namespace Ecommerce.Api.Models;

public class ApiResponse
{
    public ApiResponse()
    {
        ErrorMessages = [];
    }
    public HttpStatusCode StatusCode { get; set; }
    public bool IsSuccess { get; set; } = true;
    public List<string> ErrorMessages { get; set; }
    public object Result { get; set; }

   
}