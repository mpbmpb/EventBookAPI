using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EventBookAPI.Contracts.v1;
using EventBookAPI.Data;
using EventBookAPI.Domain;
using EventBookAPI.Test.Infrastructure;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EventBookAPI.Test.IntegrationTests
{
    public class PageElementTests : IntegrationTestBase
    {
        [Fact]
        public async Task GetAll_without_any_pageElements_returns_empty_response()
        {
            await AuthenticateAsync();

            var response = await TestClient.GetAsync(ApiRoutes.PageElements.GetAll);
            var result = await response.Content.ReadAsAsync<List<PageElement>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_returns_seeded_pageElements()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            await TestHelper.SeedDbAsync(context);
            await AuthenticateAsync();
            var expectedResult = TestHelper.GetMockPageElements();

            var response = await TestClient.GetAsync(ApiRoutes.PageElements.GetAll);
            var result = await response.Content.ReadAsAsync<List<PageElement>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}