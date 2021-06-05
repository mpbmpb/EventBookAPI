using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventBookAPI.Data;
using EventBookAPI.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EventBookAPI.Test.Infrastructure
{
    public static class TestHelper
    {
        public static async Task SeedDbAsync(DataContext context)
        {
            if (context.PageElements.Any())
                return;

            var pageElements = GetMockPageElements();
            
            context.AddRange(pageElements);
            await context.SaveChangesAsync();
        }
        public static Guid GuidIndex(int number)
        {
            return Guid.Parse($"fcebe99a-d958-4b38-8ab6-568d{number.ToString("X8")}");
        }

        public static List<PageElement> GetMockPageElements(int numberOfElements = 3 )
        {
            var pageElements = new List<PageElement>();

            for (var i = 1; i <= numberOfElements; i++)
                pageElements.Add(new()
                {
                    Id = GuidIndex(i), 
                    Content = $"SeedContent{i}", 
                    Classname = $"SeedClassname{i}", 
                    UserId = GuidIdString(1)
                });
            return pageElements;
        }

        public static int GuidIndex(string guid)
        {
            var last8Digits = guid.Substring(guid.Length - 9);
            int index;
            int.TryParse(last8Digits, NumberStyles.HexNumber, null, out index);
            return index;
        }

        public static Guid GuidId(int number)
        {
            return Guid.Parse($"0153a680-c026-4472-a200-de23{number.ToString("X8")}");
        }

        public static string GuidIdString(int number)
        {
            return $"0153a680-c026-4472-a200-de23{number.ToString("X8")}";
        }

        public static ControllerContext GetMockControllerContext()
        {
            var mockControllerContext = new ControllerContext();
            mockControllerContext.HttpContext = new DefaultHttpContext();
            mockControllerContext.HttpContext.Request.Scheme = "https";
            mockControllerContext.HttpContext.Request.Host = new("localhost", 5001);
            mockControllerContext.HttpContext.User = new(
                new ClaimsIdentity(new[]
                {
                    new Claim("id", TestHelper.GuidIdString(1))
                }));

            return mockControllerContext;
        }

        public static AuthenticationResult GetPositiveAuthenticationResult()
        {
            return new AuthenticationResult
            {
                Success = true,
                Token = TestHelper.GuidIdString(42),
                RefreshToken = GuidIndex(101)
            };
        }

        public static AuthenticationResult GetFailedAuthenticationResult() => new();
    }
}