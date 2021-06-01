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
        public async Task Register_returns_AuthSuccessResponse_when_authorization_succeeds()
        {
            var request = new UserRegistrationRequest()
            {
                Email = "test@somewhere.com",
                Password = "password1234!"
            };

            var response = await _sut.Register(request);

            response.Should().BeOfType<OkObjectResult>();
            response.As<OkObjectResult>().Value.Should().BeAssignableTo<AuthSuccessResponse>();
        }

        [Fact]
        public async Task Register_returns_BadRequest_when_authorization_fails()
        {
            var request = new UserRegistrationRequest()
            {
                Email = "test",
                Password = "password1234!"
            };
            _identityService.ReturnsForAll(Task.Factory.StartNew(TestHelper.GetFailedAuthenticationResult));

            var response = await _sut.Register(request);

            response.Should().BeOfType<BadRequestObjectResult>();
        }



    }
}