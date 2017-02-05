using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Common.Cqrs;
using DataAccess.Shows;
using FluentAssertions;
using Logic.Common;
using Logic.Shows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Website.Api.Shows;
using Website.Infrastructure.ErrorHandling;
using Xunit;

namespace Tests.Api.Shows
{
    public class ShowsCommandControllerTests
    {
        #region Helpers and setup

        public string UserName { get; set; }

        public IMapper Mapper { get; set; }

        public ICommandSender CommandSender { get; set; }

        public ShowsCommandControllerTests()
        {
            UserName = "user";

            var config = new MapperConfiguration(ShowsCommandController.ConfigureMapper);
            Mapper = new Mapper(config);

            CommandSender = Mock.Of<ICommandSender>();
        }

        public ShowsCommandController Create()
        {
            var controller = new ShowsCommandController
            {
                Mapper = Mapper,
                CommandSender = CommandSender,
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext
                    {
                        User = new GenericPrincipal(new GenericIdentity(UserName), new string[0])
                    }
                }
            };

            return controller;
        }

        #endregion

        #region Delete tests

        [Fact]
        public async Task Delete_Successful()
        {
            var model = new DeleteShowModel {ShowId = 10};
            var controller = Create();
            DeleteShowCommand command = null;
            Mock.Get(CommandSender)
                .Setup(x => x.Send(It.IsAny<DeleteShowCommand>()))
                .Callback<DeleteShowCommand>(c => { command = c; })
                .Returns(Task.CompletedTask)
                .Verifiable();

            await controller.Delete(model);

            Mock.Get(CommandSender).Verify();
            command.Should().NotBeNull();
            command.ShowId.Should().Be(10L);
            command.UserName.Should().Be(UserName);
        }

        [Fact]
        public async Task Delete_BadModel()
        {
            var model = new DeleteShowModel();
            var controller = Create();
            controller.ModelState.AddModelError("key", "error");

            var exception = await Assert.ThrowsAsync<HttpErrorException>(
                () => controller.Delete(model));

            exception.ActionResult.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_NotFound()
        {
            var model = new DeleteShowModel { ShowId = 10 };
            var controller = Create();
            var exception = new EntityNotFoundException<Show>("10");
            Mock.Get(CommandSender)
                .Setup(x => x.Send(It.IsAny<DeleteShowCommand>()))
                .Returns(Task.FromException(exception));

            var actualException =
                await Assert.ThrowsAsync<HttpErrorException>(
                    () => controller.Delete(model));

            actualException.ActionResult.Should().BeOfType<NotFoundResult>();
        }


        #endregion
    }
}