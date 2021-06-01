using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EventBookAPI.Data;
using EventBookAPI.Domain;

namespace EventBookAPI.Test.Infrastructure
{
    public static class TestDbHelper
    {
        public static async Task SeedAsync(DataContext context)
        {
            if (context.PageElements.Any())
                return;
            
            var pageElements = new List<PageElement>();

            for (var i = 1; i < 4; i++)
                pageElements.Add(new()
                {
                    Id = GuidIndex(i), Content = $"SeedContent{i}", Classname = $"SeedClassname{i}"
                });

            context.AddRange(pageElements);
            await context.SaveChangesAsync();
        }

        public static Guid GuidIndex(int number)
        {
            return Guid.Parse($"fcebe99a-d958-4b38-8ab6-568d{number.ToString("X8")}");
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
    }
}