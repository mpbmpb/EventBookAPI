using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
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
using Microsoft.EntityFrameworkCore;
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
                controller.ControllerContext = _mockControllerContext;
                return controller;
            }
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
        public async Task GetAll_returns_correct_response()
        {
            var response = await _sut.GetAll();

            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeAssignableTo<IEnumerable<PageElement>>();
        }

        [Fact]
        public async Task GetAll_returns_seeded_pageElements()
        {
            await TestHelper.SeedDbAsync(_seedContext);

            var response = await _sut.GetAll();
            var result = response.As<OkObjectResult>().Value.As<IEnumerable<PageElement>>();

            result.Count().Should().Be(3);
            result.FirstOrDefault()?.Content.Should().Be("SeedContent1");
            result.FirstOrDefault()?.Classname.Should().Be("SeedClassname1");
        }

        [Fact]
        public async Task Get_returns_correct_response()
        {
            await TestHelper.SeedDbAsync(_seedContext);

            var response = await _sut.Get(TestHelper.GuidIndex(1));

            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeAssignableTo<PageElement>();
        }

        [Fact]
        public async Task Get_returns_correct_pageElement()
        {
            await TestHelper.SeedDbAsync(_seedContext);
            var expectedResult = await _seedContext.PageElements.FirstOrDefaultAsync();

            var response = await _sut.Get(TestHelper.GuidIndex(1));
            var result = response.As<OkObjectResult>().Value.As<PageElement>();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task Get_returns_NotFound_when_id_doesnt_exist()
        {
            var result = await _sut.Get(TestHelper.GuidIndex(1));

            result.Should().BeAssignableTo<NotFoundResult>();
        }


        [Fact]
        public async Task Update_returns_correct_response()
        {
            await TestHelper.SeedDbAsync(_seedContext);
            var request = new UpdatePageElementRequest {Content = "Changed", Classname = "ChangedClass"};

            var response = await _sut.Update(TestHelper.GuidIndex(1), request);

            response.Should().BeAssignableTo<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeAssignableTo<PageElement>();
        }

        [Fact]
        public async Task Update_returns_NotFound_when_id_doesnt_exist()
        {
            var request = new UpdatePageElementRequest {Content = "Changed", Classname = "ChangedClass"};
            
            var result = await _sut.Update(TestHelper.GuidIndex(1), request);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Update_returns_NotFound_when_user_doesnt_own_pageElement()
        {
            await TestHelper.SeedDbAsync(_seedContext);
            var request = new UpdatePageElementRequest {Content = "Changed", Classname = "ChangedClass"};
            _sut.ControllerContext.HttpContext.User = new(
                new ClaimsIdentity(new[]
                {
                    new Claim("id", TestHelper.GuidIdString(2))
                }));
            
            var result = await _sut.Update(TestHelper.GuidIndex(1), request);

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Update_correctly_updates_pageElement()
        {
            await TestHelper.SeedDbAsync(_seedContext);
            var request = new UpdatePageElementRequest {Content = "Changed", Classname = "ChangedClass"};

            var response = await _sut.Update(TestHelper.GuidIndex(1), request);
            var result = response.As<OkObjectResult>().Value.As<PageElement>();

            result.Content.Should().Match(request.Content);
            result.Classname.Should().Match(request.Classname);
        }

        [Fact]
        public async Task Delete_returns_correct_response()
        {
            await TestHelper.SeedDbAsync(_seedContext);

            var response = await _sut.Delete(TestHelper.GuidIndex(1));

            response.Should().BeAssignableTo<NoContentResult>();
        }

        [Fact]
        public async Task Delete_returns_NotFound_when_id_doesnt_exist()
        {
            var result = await _sut.Delete(TestHelper.GuidIndex(1));

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_returns_NotFound_when_user_doesnt_own_pageElement()
        {
            await TestHelper.SeedDbAsync(_seedContext);
            var request = new UpdatePageElementRequest {Content = "Changed", Classname = "ChangedClass"};
            _sut.ControllerContext.HttpContext.User = new(
                new ClaimsIdentity(new[]
                {
                    new Claim("id", TestHelper.GuidIdString(2))
                }));
            
            var result = await _sut.Delete(TestHelper.GuidIndex(1));

            result.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_correctly_removes_pageElement_from_db()
        {
            await TestHelper.SeedDbAsync(_seedContext);

            await _sut.Delete(TestHelper.GuidIndex(1));

            var result = _resultContext.PageElements
                .FirstOrDefault(x => x.Id == TestHelper.GuidIndex(1) );

            _resultContext.PageElements.Count().Should().Be(2);
            result.Should().BeNull();
        }
    }
}