namespace Marcum.CCH.Axcess.API.Statics;

public static class Constants
{

    public static class HttpRequestHeader
    {
        public const string IntegratorKeyHeaderName = "IntegratorKey";
        public const string AuthorizationHeaderName = "Authorization";
        public const string TokenPlaceholder = "{tokenValue}";
        public const string AuthorizationHeaderValue = "Bearer " + TokenPlaceholder;        
    }

    public static class ErrorMessages
    {
        public const string AuthNotInitialized = "Auth not initialized";
        public const string UnauthorizedRefreshToken = "Unauthorized. Please make sure the Authorization token / ClientSecret is refreshed in CCHAxcess dashboard, then process GetLoginUrl.";
    }

    public static class ReturnTypes
    {
        public const string FedState = "FedState";
        public const string FedOnly = "FedOnly";
    }

    public static class Formats
    {
        public const string HttpResponse = "{0}\n{1}";

    }
}
