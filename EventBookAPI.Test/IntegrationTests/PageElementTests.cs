using System.Net;
using System.Threading.Tasks;
using EventBookAPI.Contracts.v1;
using EventBookAPI.Test.Infrastructure;
using FluentAssertions;
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

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
        }

    }
}