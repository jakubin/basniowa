using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Website.Api.Users;
using Website.Infrastructure.ErrorHandling;
using Website.Infrastructure.Jwt;
using Xunit;

namespace Tests.Api.Shows
{
    public class UsersControllerTests
    {
        public IJwtBearerTokenProvider TokenProvider { get; set; }

        public UsersControllerTests()
        {
            TokenProvider = Mock.Of<IJwtBearerTokenProvider>(); 
        }

        public UsersController Create()
        {
            return new UsersController
            {
                TokenProvider = TokenProvider
            };
        }

        [Fact(DisplayName = nameof(UsersController) + ": Authenticate rejects invalid model with status 400.")]
        public async Task AuthenticateInvalidModel()
        {
            var model = new UserLogonModel();
            var controller = Create();
            controller.ModelState.AddModelError("Password", "The password is required.");

            var error = await Assert.ThrowsAsync<HttpErrorException>(() => controller.Authenticate(model));

            error.ActionResult.Should().BeOfType<BadRequestObjectResult>();
            var errors = error.ActionResult.As<BadRequestObjectResult>().Value.As<IDictionary<string, object>>();
            errors.Keys.Should().Contain("password");
            errors["password"].As<IEnumerable<string>>().Should().BeEquivalentTo("The password is required.");
        }
    }
}
