using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EventBookAPI.Data;
using EventBookAPI.Domain;

namespace EventBookAPI.Test.Infrastructure
{
    public class TestDbHelper
    {
        public static async Task InitializeAsync(DataContext context)
        {
            if (context.PageElements.Any())
                return;

            await SeedAsync(context);
        }

        private static async Task SeedAsync(DataContext context)
        {
            var pageElements = new List<PageElement>();

            for (int i = 1; i < 5; i++)
            {
                pageElements.Add(new PageElement
                {
                    Id = GuidIndex(i), Content = $"Content{i}", Classname = $"Classname{i}"
                });
            }
            
            context.AddRange(pageElements);
            await context.SaveChangesAsync();
        }

        public static Guid GuidIndex(int number) => 
            Guid.Parse($"fcebe99a-d958-4b38-8ab6-568d{number.ToString("X8")}");

        public static int GuidIndex(string guid)
        {
            var last8Digits = guid.Substring(guid.Length - 9);
            int index;
            Int32.TryParse(last8Digits, NumberStyles.HexNumber, null, out index);
            return index;
        }
    }
}