using System;
using System.Linq;
using EventBookAPI.Data;
using EventBookAPI.Domain;

namespace EventBookAPI.Test.Infrastructure
{
    public class TestDbInitializer
    {
        public static void Initialize(DataContext context)
        {
            if (context.PageElements.Any())
                return;

            Seed(context);
        }

        private static void Seed(DataContext context)
        {
            var pageElements = new[]
            {
                new PageElement
                {
                    Id = Guid.Parse("fcebe99a-d958-4b38-8ab6-568d00142251"), Content = "Content1",
                    Classname = "Classname1"
                }
            };
        }
    }
}