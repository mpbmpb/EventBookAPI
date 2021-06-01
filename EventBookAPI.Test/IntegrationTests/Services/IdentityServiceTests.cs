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
        private readonly IdentityService _sut;

        public IdentityServiceTests()
        {
            _sut = _serviceProvider.GetService<IdentityService>();
        }

        [Fact]
        public async Task RegisterAsync_returns_authenticationResult_with_tokens_when_given_unique_user()
        {
            var result = await _sut.RegisterAsync("test@mailbox.com", "Password42!");

            result.Should().BeOfType<AuthenticationResult>();
        }

    }
}