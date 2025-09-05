namespace Ecommerce.Api.Endpoints;

public static  class ApiEndpoints
{
    private const string ApiBase = "api";
    public static class MenuItems
    {
        private const string Base = $"{ApiBase}/menuitems";
        public const string Create = Base;
        public const string Get = $"{Base}/{{id:int}}";
        public const string GetAll = Base;
        
        public const string Update = $"{Base}/{{id:int}}";
        public const string Delete = $"{Base}/{{id:int}}";
    }
    
    public static class Auth
    {
        private const string Base = $"{ApiBase}/auth";
        public const string Register = $"{Base}/register";
        public const string Login = $"{Base}/login";
       
    }
   
}