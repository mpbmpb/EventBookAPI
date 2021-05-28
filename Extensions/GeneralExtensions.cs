using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace EventBookAPI.Extensions
{
    public static class GeneralExtensions
    {
        public static string GetUserId(this HttpContext httpContext)
        {
            if (httpContext is null)
                return string.Empty;

            return httpContext.User.Claims.Single(x => x.Type == "id").Value;
        }
    }
}