using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EventBookAPI.Contracts.v1;
using EventBookAPI.Contracts.v1.Requests;
using EventBookAPI.Contracts.v1.Responses;
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
        private PageElementController _sut
        {
            get
            {
                var controller = new PageElementController(new PageElementService(_context));
                controller.ControllerContext = _controllerContext;
                return controller;
            }
        }

        [Fact]
        public async Task GetAll_returns_seeded_pageElements()
        {
            await TestDbHelper.SeedAsync(_seedContext);

            var response = await _sut.GetAll();
            var result = response.As<OkObjectResult>().Value;

            result.Should().BeAssignableTo<IEnumerable<PageElement>>();
            result.As<IEnumerable<PageElement>>().Count().Should().Be(3);
            result.As<IEnumerable<PageElement>>().FirstOrDefault()?.Content.Should().Be("SeedContent1");
            result.As<IEnumerable<PageElement>>().FirstOrDefault()?.Classname.Should().Be("SeedClassname1");
        }

        [Fact]
        public async Task Create_stores_pageElement()
        {
            var request = new CreatePageElementRequest
            {
                Content = "TestContent42",
                Classname = "TestClassname42"
            };

            await _sut.Create(request);
            var result = _resultContext.PageElements.FirstOrDefault(x => x.Content == "TestContent42");

            result.Should().BeEquivalentTo(request, options =>
                options.ComparingByMembers<string>());
        }

        [Fact]
        public async Task Create_returns_correct_response()
        {
            var request = new CreatePageElementRequest
            {
                Content = "TestContent42",
                Classname = "TestClassname42"
            };

            var result = await _sut.Create(request);
            var expectedResponse = new PageElementResponse
            {
                Id = _context.PageElements.FirstOrDefault().Id,
                Content = request.Content,
                Classname = request.Classname
            };
            var expectedLocation = "https://localhost:5001/" +
                    $"{ApiRoutes .PageElements.Get.Replace("{pageElementId}", expectedResponse.Id.ToString())}";

            result.Should().BeOfType<CreatedResult>();
            result.As<CreatedResult>().Location.Should().Match(expectedLocation);
            result.As<CreatedResult>().Value.Should().BeEquivalentTo(expectedResponse);
        }

    }
}