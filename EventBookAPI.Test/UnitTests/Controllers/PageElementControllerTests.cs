using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBookAPI.Contracts.v1.Requests;
using EventBookAPI.Controllers.v1;
using EventBookAPI.Domain;
using EventBookAPI.Services;
using EventBookAPI.Test.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace EventBookAPI.Test.UnitTests.Controllers
{
    public class PageElementControllerTests : UnitTestBase
    {
        private PageElementController _controller => new PageElementController(new PageElementService(_context));

        [Fact]
        public async Task GetAll_returns_seeded_pageElements()
        {
            await TestDbHelper.InitializeAsync(_seedContext);

            var response = await _controller.GetAll();
            var result = response.As<OkObjectResult>().Value;

            result.Should().BeAssignableTo<IEnumerable<PageElement>>();
            result.As<IEnumerable<PageElement>>().Count().Should().Be(4);
        }
}

}