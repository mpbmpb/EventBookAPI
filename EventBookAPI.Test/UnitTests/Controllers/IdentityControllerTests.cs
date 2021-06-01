using System.Threading.Tasks;
using EventBookAPI.Contracts.v1.Requests;
using EventBookAPI.Contracts.v1.Responses;
using EventBookAPI.Controllers.v1;
using EventBookAPI.Domain;
using EventBookAPI.Services;
using EventBookAPI.Test.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace EventBookAPI.Test.UnitTests.Controllers
{
    public class IdentityControllerTests : UnitTestBase
    {
        private readonly IdentityController _sut;
        private readonly IIdentityService _identityService = Substitute.For<IIdentityService>();

        public IdentityControllerTests()
        {
            _identityService.ReturnsForAll(Task.Factory.StartNew(TestHelper.GetPositiveAuthenticationResult));
            
            _sut = new IdentityController(_identityService);
        }

        [Fact]
        public async Task Register_returns_Ok_with_AuthSuccessResponse_when_authorization_succeeds()
        {
            var request = new UserRegistrationRequest();

            var response = await _sut.Register(request);

            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeAssignableTo<AuthSuccessResponse>();
        }

        [Fact]
        public async Task Register_returns_BadRequest_with_AuthFailedResponse_when_authorization_fails()
        {
            var request = new UserRegistrationRequest();
            _identityService.ReturnsForAll(Task.Factory.StartNew(TestHelper.GetFailedAuthenticationResult));

            var response = await _sut.Register(request);

            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.Should().BeAssignableTo<AuthFailedResponse>();
        }

        [Fact]
        public async Task Login_returns_Ok_with_AuthSuccessResponse_when_authorization_succeeds()
        {
            var request = new UserLoginRequest();

            var response = await _sut.Login(request);

            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeAssignableTo<AuthSuccessResponse>();
        }
        
        [Fact]
        public async Task Login_returns_BadRequest_with_AuthFailedResponse_when_authorization_fails()
        {
            var request = new UserLoginRequest();
            _identityService.ReturnsForAll(Task.Factory.StartNew(TestHelper.GetFailedAuthenticationResult));

            var response = await _sut.Login(request);

            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.Should().BeAssignableTo<AuthFailedResponse>();
        }
        
        [Fact]
        public async Task Refresh_returns_Ok_with_AuthSuccessResponse_when_authorization_succeeds()
        {
            var request = new RefreshTokenRequest();

            var response = await _sut.Refresh(request);

            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeAssignableTo<AuthSuccessResponse>();
        }
        
        [Fact]
        public async Task Refresh_returns_BadRequest_with_AuthFailedResponse_when_authorization_fails()
        {
            var request = new RefreshTokenRequest();
            _identityService.ReturnsForAll(Task.Factory.StartNew(TestHelper.GetFailedAuthenticationResult));

            var response = await _sut.Refresh(request);

            response.Should().BeOfType<BadRequestObjectResult>();
            response.As<BadRequestObjectResult>().Value.Should().BeAssignableTo<AuthFailedResponse>();
        }
    }
}