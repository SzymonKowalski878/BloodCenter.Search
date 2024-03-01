using System.Net;

namespace BloodCenter.Search.Client
{
    public static class StatusCodeExtensions
    {
        public static bool IsSuccessfulStatusCode(this HttpStatusCode statusCode) =>
            (int)statusCode >= 200 && (int)statusCode < 300;
    }
}
