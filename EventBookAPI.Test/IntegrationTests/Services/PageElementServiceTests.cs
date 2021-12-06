using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using EventBookAPI.Contracts.v1;
using EventBookAPI.Contracts.v1.Requests;
using EventBookAPI.Contracts.v1.Responses;
using EventBookAPI.Data;
using EventBookAPI.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventBookAPI.Test.IntegrationTests.Services
{
    [Collection("Integration Test Collection")]
    public class PageElementServiceTests : IntegrationTestBase
    {
        [Fact]
        public async Task Create_returns_response_with_uri_and_pageElement()
        {
            var request = new CreatePageElementRequest
            {
                Content = "TestContent42",
                Classname = "TestClassname42"
            };
            await AuthenticateAsync();
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.PageElements.Create, request);
            var result = await response.Content.ReadAsAsync<PageElementResponse>();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            result.Should().BeEquivalentTo(request);
        }

        [Fact]
        public async Task Create_stores_pageElement_in_db()
        {
            var request = new CreatePageElementRequest
            {
                Content = "TestContent43",
                Classname = "TestClassname43"
            };
            await AuthenticateAsync();
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.PageElements.Create, request);
            var newElement = await response.Content.ReadAsAsync<PageElementResponse>();
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();

            var result = await context.PageElements.FirstOrDefaultAsync(x => x.Id == newElement.Id);

            result.Should().BeEquivalentTo(request);
        }

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
        public async Task GetAll_returns_seeded_pageElement()
        {
            var request = new CreatePageElementRequest
            {
                Content = "TestContent44",
                Classname = "TestClassname44"
            };
            await AuthenticateAsync();
            await TestClient.PostAsJsonAsync(ApiRoutes.PageElements.Create, request);
        
            var response = await TestClient.GetAsync(ApiRoutes.PageElements.GetAll);
            var result = await response.Content.ReadAsAsync<List<PageElement>>();
        
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result[0].Should().BeEquivalentTo(request);;
        }

        [Fact]
        public async Task Get_returns_element_with_proper_response()
        {
            var request = new CreatePageElementRequest
            {
                Content = "TestContent45",
                Classname = "TestClassname45"
            };
            await AuthenticateAsync();
            var postResponse = await TestClient.PostAsJsonAsync(ApiRoutes.PageElements.Create, request);

            var response = await TestClient.GetAsync(postResponse.Headers.Location);
            var result = await response.Content.ReadAsAsync<PageElement>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().BeEquivalentTo(request);
        }

        [Fact]
        public async Task Update_correctly_updates_pageElement_in_db()
        {
            var request = new CreatePageElementRequest
            {
                Content = "TestContent46",
                Classname = "TestClassname46"
            };
            await AuthenticateAsync();
            var postResponse = await TestClient.PostAsJsonAsync(ApiRoutes.PageElements.Create, request);
            var updateRequest = new UpdatePageElementRequest {Content = "NewContent", Classname = "NewName"};

            TestClient.DefaultRequestHeaders.Authorization = new("bearer", _registrationResponse.Token);
            await TestClient.PutAsJsonAsync(postResponse.Headers.Location, updateRequest);
            var response = await TestClient.GetAsync(postResponse.Headers.Location);
            var result = await response.Content.ReadAsAsync<PageElement>();
            
            result.Should().BeEquivalentTo(updateRequest);
        }

        [Fact]
        public async Task Delete_gives_correct_response()
        {
            var request = new CreatePageElementRequest
            {
                Content = "TestContent46",
                Classname = "TestClassname46"
            };
            await AuthenticateAsync();
            var postResponse = await TestClient.PostAsJsonAsync(ApiRoutes.PageElements.Create, request);
            
            var result = await TestClient.DeleteAsync(postResponse.Headers.Location);

            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_removes_pageElement_from_db()
        {
            var request = new CreatePageElementRequest
            {
                Content = "TestContent46",
                Classname = "TestClassname46"
            };
            await AuthenticateAsync();
            var postResponse = await TestClient.PostAsJsonAsync(ApiRoutes.PageElements.Create, request);
            
            await TestClient.DeleteAsync(postResponse.Headers.Location);
            
            var result = await TestClient.GetAsync(postResponse.Headers.Location);

            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }


    }
}