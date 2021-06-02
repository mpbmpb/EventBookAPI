using System;
using System.Threading.Tasks;
using EventBookAPI.Domain;
using EventBookAPI.Options;
using EventBookAPI.Services;
using EventBookAPI.Test.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Xunit;

namespace EventBookAPI.Test.IntegrationTests.Services
{
    public class IdentityServiceTests : IntegrationTestBase
    {
        private IIdentityService _sut;

        public IdentityServiceTests()
        {
        }

        [Fact]
        public async Task RegisterAsync_returns_authenticationResult_with_tokens_when_given_unique_user()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            
            var result = await _sut.RegisterAsync($"{Guid.NewGuid().ToString()}@integration.com", "Password42!");
            
            result.Should().BeOfType<AuthenticationResult>();
            result.As<AuthenticationResult>().Token.Should().NotBeNullOrEmpty();
            result.As<AuthenticationResult>().RefreshToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task RegisterAsync_returns_failed_authenticationResult_when_password_violates_requirements()
        {
            using var scope = _factory.Services.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            
            var result = await _sut.RegisterAsync("test@email.nl", "Password42");
            
            result.Should().BeOfType<AuthenticationResult>();
            result.As<AuthenticationResult>().Success.Should().BeFalse();
            result.As<AuthenticationResult>().Errors.Should().NotBeEmpty();    
        }


    }
}