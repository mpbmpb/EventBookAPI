using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using EventBookAPI.Contracts.v1.Responses;
using EventBookAPI.Data;
using EventBookAPI.Domain;
using EventBookAPI.Options;
using EventBookAPI.Services;
using EventBookAPI.Test.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using Xunit;

namespace EventBookAPI.Test.IntegrationTests.Services
{
    [Collection("Integration Test Collection")]
    public class IdentityServiceTests : IntegrationTestBase
    {

        private IIdentityService _sut;

        [Fact]
        public async Task RegisterAsync_returns_authenticationResult_with_tokens_when_given_new_user()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            
            var result = await _sut.RegisterAsync("test@email.nl", "Password42!");
            
            result.Token.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task RegisterAsync_returns_valid_token_when_successful()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var expectedErrors = new[] {"This token hasn't expired yet"};
            
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            var result = await _sut.RefreshTokenAsync(authResult.Token, authResult.RefreshToken);

            result.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task RegisterAsync_returns_valid_refresh_token_when_successful()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
            var shortLifeTime = TimeSpan.FromMilliseconds(10);
            jwtSettings.TokenLifetime = shortLifeTime;
            
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            await Task.Delay(shortLifeTime);
            var result = await _sut.RefreshTokenAsync(authResult.Token, authResult.RefreshToken);
            
            result.Token.Should().NotBeNullOrEmpty();
            result.Token.Should().NotMatch(authResult.Token);
        }



        [Fact]
        public async Task RegisterAsync_returns_failed_result_when_given_existing_user()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await _sut.RegisterAsync("test@email.nl", "Password42!");
            var expectedErrors = new[] {"User with this email address already exists"};
            
            var result = await _sut.RegisterAsync("test@email.nl", "Password42!");

            result.Success.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(expectedErrors);
        }
        
        [Fact]
        public async Task RegisterAsync_returns_failed_result_with_errors_when_password_violates_requirements()
        {
            using var scope = _factory.Services.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            
            var result = await _sut.RegisterAsync("test@email.nl", "Password42");
            
            result.Success.Should().BeFalse();
            result.Errors.Count().Should().Be(1);
            result.Errors.Should().NotBeEmpty();
        }     
        
        [Fact]
        public async Task RegisterAsync_doesnt_return_tokens_when_password_violates_requirements()
        {
            using var scope = _factory.Services.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            
            var result = await _sut.RegisterAsync("test@email.nl", "Password42");
            
            result.Token.Should().BeNull();
            result.RefreshToken.Should().BeEmpty();    
        }

        [Theory]
        [InlineData("Pw42!", new [] {"Passwords must be at least 6 characters."})]
        [InlineData("Password42", new [] {"Passwords must have at least one non alphanumeric character."})]
        [InlineData("Password!", new [] {"Passwords must have at least one digit ('0'-'9')."})]
        [InlineData("password42!", new [] {"Passwords must have at least one uppercase ('A'-'Z')."})]
        [InlineData("PASSWORD42!", new [] {"Passwords must have at least one lowercase ('a'-'z')."})]
        [InlineData("", new [] {"Passwords must be at least 6 characters.", 
            "Passwords must have at least one non alphanumeric character.", 
            "Passwords must have at least one digit ('0'-'9').", 
            "Passwords must have at least one lowercase ('a'-'z').", 
            "Passwords must have at least one uppercase ('A'-'Z').", 
            "Passwords must use at least 1 different characters."})]
        public async Task RegisterAsync_returns_correct_error_messages_when_password_violates_requirements(string password, string [] expectedErrors)
        {
            using var scope = _factory.Services.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            
            var result = await _sut.RegisterAsync("test@email.nl", password);

            result.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task Login_returns_authenticationResult_with_tokens_when_successful()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await _sut.RegisterAsync("test@email.nl", "Password42!");
            
            var result = await _sut.LoginAsync("test@email.nl", "Password42!");
            
            result.Token.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task LoginAsync_returns_valid_token_when_successful()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await _sut.RegisterAsync("test@email.nl", "Password42!");
            var expectedErrors = new[] {"This token hasn't expired yet"};
            
            var authResult = await _sut.LoginAsync("test@email.nl", "Password42!");
            var result = await _sut.RefreshTokenAsync(authResult.Token, authResult.RefreshToken);

            result.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task LoginAsync_returns_valid_refresh_token_when_successful()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
            var shortLifeTime = TimeSpan.FromMilliseconds(10);
            jwtSettings.TokenLifetime = shortLifeTime;
            
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            await Task.Delay(shortLifeTime);
            var result = await _sut.RefreshTokenAsync(authResult.Token, authResult.RefreshToken);
            
            result.Token.Should().NotBeNullOrEmpty();
            result.Token.Should().NotMatch(authResult.Token);
        }

        [Fact]
        public async Task LoginAsync_returns_error_when_user_doesnt_exist()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await _sut.RegisterAsync("test@email.nl", "Password42!");
            var expectedErrors = new[] {"User does not exist"};
            
            var result = await _sut.LoginAsync("newPerson@email.nl", "Password42!");

            result.Success.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task LoginAsync_doesnt_return_tokens_when_user_doesnt_exist()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await _sut.RegisterAsync("test@email.nl", "Password42!");
            
            var result = await _sut.LoginAsync("newPerson@email.nl", "Password42!");

            result.Success.Should().BeFalse();
            result.Token.Should().BeNull();
            result.RefreshToken.Should().BeEmpty();
        }

        [Fact]
        public async Task LoginAsync_returns_error_when_password_is_incorrect()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await _sut.RegisterAsync("test@email.nl", "Password42!");
            var expectedErrors = new[] {"User/password combination is not correct"};
            
            var result = await _sut.LoginAsync("test@email.nl", "WrongPassword43!");

            result.Success.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(expectedErrors);
        }
        
        [Fact]
        public async Task LoginAsync_doesnt_return_tokens_when_password_is_incorrect()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            await _sut.RegisterAsync("test@email.nl", "Password42!");
            
            var result = await _sut.LoginAsync("test@email.nl", "WrongPassword43!");

            result.Success.Should().BeFalse();
            result.Token.Should().BeNull();
            result.RefreshToken.Should().BeEmpty();
        }

        [Fact]
        public async Task RefreshAsync_refreshes_tokens_when_valid_and_expired()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
            var shortLifeTime = TimeSpan.FromMilliseconds(10);
            jwtSettings.TokenLifetime = shortLifeTime;
            
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            await Task.Delay(shortLifeTime);
            var result = await _sut.RefreshTokenAsync(authResult.Token, authResult.RefreshToken);
            
            result.Token.Should().NotBeNullOrEmpty();
            result.Token.Should().NotMatch(authResult.Token);
        }

        [Fact]
        public async Task RefreshAsync_returns_error_when_token_is_invalid()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            var token = authResult.Token.Substring(1);
            var expectedErrors = new[] {"Invalid Token"};
            
            var result = await _sut.RefreshTokenAsync(token, authResult.RefreshToken);

            result.Success.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(expectedErrors);
        }
[Fact]
        public async Task RefreshAsync_doesnt_return_tokens_when_token_is_invalid()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            var token = authResult.Token.Substring(1);
            
            var result = await _sut.RefreshTokenAsync(token, authResult.RefreshToken);

            result.Token.Should().BeNull();
            result.RefreshToken.Should().BeEmpty();
        }

        [Fact]
        public async Task RefreshAsync_returns_error_when_token_is_not_expired()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var expectedErrors = new[] {"This token hasn't expired yet"};
            
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            var result = await _sut.RefreshTokenAsync(authResult.Token, authResult.RefreshToken);

            result.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task RefreshAsync_returns_failed_result_when_refreshToken_doesnt_exist()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
            var shortLifeTime = TimeSpan.FromMilliseconds(10);
            jwtSettings.TokenLifetime = shortLifeTime;
            var expectedErrors = new[] {"This refresh token does not exist"};
            
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            await Task.Delay(shortLifeTime);
            var result = await _sut.RefreshTokenAsync(authResult.Token, Guid.NewGuid());

            result.Success.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(expectedErrors);
            result.Token.Should().BeNull();
            result.RefreshToken.Should().BeEmpty();
        }

        [Fact]
        public async Task RefreshAsync_returns_failed_result_when_refreshToken_has_expired()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
            var shortLifeTime = TimeSpan.FromMilliseconds(10);
            jwtSettings.TokenLifetime = shortLifeTime;
            jwtSettings.RefreshTokenLifetime = shortLifeTime;
            var expectedErrors = new[] {"This refresh token has expired"};
            
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            await Task.Delay(shortLifeTime);
            var result = await _sut.RefreshTokenAsync(authResult.Token, authResult.RefreshToken);

            result.Success.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(expectedErrors);
            result.Token.Should().BeNull();
            result.RefreshToken.Should().BeEmpty();
        }

        [Fact]
        public async Task RefreshAsync_returns_failed_result_when_refreshToken_is_invalidated()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
            var shortLifeTime = TimeSpan.FromMilliseconds(10);
            jwtSettings.TokenLifetime = shortLifeTime;
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var expectedErrors = new[] {"This refresh token has been invalidated"};
            
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == authResult.RefreshToken);
            refreshToken.Invalidated = true;
            await context.SaveChangesAsync();
            
            await Task.Delay(shortLifeTime);
            var result = await _sut.RefreshTokenAsync(authResult.Token, authResult.RefreshToken);

            result.Success.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(expectedErrors);
            result.Token.Should().BeNull();
            result.RefreshToken.Should().BeEmpty();
        }

        [Fact]
        public async Task RefreshAsync_returns_failed_result_when_refreshToken_has_been_used()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
            var shortLifeTime = TimeSpan.FromMilliseconds(10);
            jwtSettings.TokenLifetime = shortLifeTime;
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var expectedErrors = new[] {"This refresh token has been used"};
            
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == authResult.RefreshToken);
            refreshToken.Used = true;
            await context.SaveChangesAsync();
            
            await Task.Delay(shortLifeTime);
            var result = await _sut.RefreshTokenAsync(authResult.Token, authResult.RefreshToken);

            result.Success.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(expectedErrors);
            result.Token.Should().BeNull();
            result.RefreshToken.Should().BeEmpty();
        }

        [Fact]
        public async Task RefreshAsync_returns_failed_result_when_refreshToken_doesnt_belong_to_token()
        {
            using var scope = _serviceProvider.CreateScope();
            _sut = scope.ServiceProvider.GetRequiredService<IIdentityService>();
            var jwtSettings = scope.ServiceProvider.GetRequiredService<JwtSettings>();
            var shortLifeTime = TimeSpan.FromMilliseconds(10);
            jwtSettings.TokenLifetime = shortLifeTime;
            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var expectedErrors = new[] {"This refresh token does not match this JWT"};
            
            var authResult = await _sut.RegisterAsync("test@email.nl", "Password42!");
            var refreshToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == authResult.RefreshToken);
            refreshToken.JwtId = "wrongId";
            await context.SaveChangesAsync();
            
            await Task.Delay(shortLifeTime);
            var result = await _sut.RefreshTokenAsync(authResult.Token, authResult.RefreshToken);

            result.Success.Should().BeFalse();
            result.Errors.Should().BeEquivalentTo(expectedErrors);
            result.Token.Should().BeNull();
            result.RefreshToken.Should().BeEmpty();
        }


    }
}