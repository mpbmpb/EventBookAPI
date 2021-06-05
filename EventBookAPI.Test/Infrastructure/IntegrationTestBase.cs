using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using EventBookAPI.Contracts.v1;
using EventBookAPI.Contracts.v1.Requests;
using EventBookAPI.Contracts.v1.Responses;
using EventBookAPI.Data;
using EventBookAPI.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace EventBookAPI.Test.Infrastructure
{
    public class IntegrationTestBase : IDisposable
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly HttpClient TestClient;
        protected WebApplicationFactory<Startup> _factory;
        protected AuthSuccessResponse _registrationResponse;
        protected string _userName;

        public IntegrationTestBase()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll(typeof(DataContext));
                        services.AddDbContext<DataContext>(options => 
                            { options.UseInMemoryDatabase("TestDb"); });

                        var serviceProvider = services.BuildServiceProvider();
                        using (var scope = serviceProvider.CreateScope())
                        {
                            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                            dbContext.Database.EnsureCreated();
                        }
                    });
                });
            TestClient = appFactory.CreateClient();
            _serviceProvider = appFactory.Services;
            _factory = appFactory;
            _userName = Guid.NewGuid().ToString();
        }

        protected async Task AuthenticateAsync()
        {
            TestClient.DefaultRequestHeaders.Authorization = new("bearer", await GetJwtAsync());
        }

        private async Task<string> GetJwtAsync()
        {
            var response = await TestClient.PostAsJsonAsync(ApiRoutes.Identity.Register, new UserRegistrationRequest
            {
                Email = $"{_userName}@integration.com",
                Password = "SomePass1234!"
            });

            var registrationResponse = await response.Content.ReadAsAsync<AuthSuccessResponse>();
            _registrationResponse = registrationResponse;
            return registrationResponse.Token;
        }

        public void Dispose()
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var context = serviceScope.ServiceProvider.GetService<DataContext>();
            context?.Database.EnsureDeleted();
            context?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}