using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Common.Cqrs;
using DataAccess.Database.Shows;
using FluentAssertions;
using Logic.Common;
using Logic.Shows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Website.Api.Shows;
using Website.Infrastructure.ErrorHandling;
using Website.Tests.Helpers;
using Xunit;

namespace Website.Tests.Api.Shows
{
    public class ShowsCommandControllerTests
    {
        #region Helpers and setup

        public string UserName { get; set; }

        public IMapper Mapper { get; set; }

        public ICommandSender CommandSender { get; set; }

        public TestUniqueIdService UniqueIdService { get; set; }

        public ShowsCommandControllerTests()
        {
            UserName = "user";

            var config = new MapperConfiguration(ShowsCommandController.ConfigureMapper);
            Mapper = new Mapper(config);

            CommandSender = Mock.Of<ICommandSender>();

            UniqueIdService = new TestUniqueIdService();
        }

        public ShowsCommandController Create()
        {
            var controller = new ShowsCommandController
            {
                Mapper = Mapper,
                CommandSender = CommandSender,
                IdService = UniqueIdService,
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

        #region Add tests

        [Fact]
        public async Task Add_Successful()
        {
            var model = new AddShowModel
            {
                Title = "1",
                Subtitle = "2",
                Description = "3",
                Properties = new Dictionary<string, string>
                {
                    ["P1"] = "V1"
                }
            };
            UniqueIdService.NextId = 10;
            var controller = Create();
            AddShowCommand command = null;
            Mock.Get(CommandSender)
                .Setup(x => x.Send(It.IsAny<AddShowCommand>()))
                .Callback<AddShowCommand>(c => { command = c; })
                .Returns(Task.CompletedTask)
                .Verifiable();

            var actualResult = await controller.Add(model);

            actualResult.ShowId.Should().Be(10L);
            Mock.Get(CommandSender).Verify();
            command.Should().NotBeNull();
            command.ShowId.Should().Be(10L);
            command.UserName.Should().Be(UserName);
            command.Title.Should().Be(model.Title);
            command.Subtitle.Should().Be(model.Subtitle);
            command.Description.Should().Be(model.Description);
            command.Properties.ShouldBeEquivalentTo(model.Properties);
        }

        [Fact]
        public async Task Add_BadModel()
        {
            var model = new AddShowModel();
            var controller = Create();
            controller.ModelState.AddModelError("key", "error");

            var exception = await Assert.ThrowsAsync<HttpErrorException>(
                () => controller.Add(model));

            exception.ActionResult.Should().BeOfType<BadRequestObjectResult>();
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

        #region Update tests

        [Fact]
        public async Task Update_Successful()
        {
            var model = new UpdateShowModel
            {
                ShowId = 10,
                Title = "1",
                Subtitle = "2",
                Description = "3",
                Properties = new Dictionary<string, string>
                {
                    ["P1"] = "V1"
                }
            };
            var controller = Create();
            UpdateShowCommand command = null;
            Mock.Get(CommandSender)
                .Setup(x => x.Send(It.IsAny<UpdateShowCommand>()))
                .Callback<UpdateShowCommand>(c => { command = c; })
                .Returns(Task.CompletedTask)
                .Verifiable();

            await controller.Update(model);

            Mock.Get(CommandSender).Verify();
            command.Should().NotBeNull();
            command.ShowId.Should().Be(10L);
            command.UserName.Should().Be(UserName);
            command.Title.Should().Be(model.Title);
            command.Subtitle.Should().Be(model.Subtitle);
            command.Description.Should().Be(model.Description);
            command.Properties.ShouldBeEquivalentTo(model.Properties);
        }

        [Fact]
        public async Task Update_BadModel()
        {
            var model = new UpdateShowModel();
            var controller = Create();
            controller.ModelState.AddModelError("key", "error");

            var exception = await Assert.ThrowsAsync<HttpErrorException>(
                () => controller.Update(model));

            exception.ActionResult.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_NotFound()
        {
            var model = new UpdateShowModel
            {
                ShowId = 10,
                Title = "1",
                Subtitle = "2",
                Description = "3",
                Properties = new Dictionary<string, string>
                {
                    ["P1"] = "V1"
                }
            };
            var controller = Create();
            var exception = new EntityNotFoundException<Show>("10");
            Mock.Get(CommandSender)
                .Setup(x => x.Send(It.IsAny<UpdateShowCommand>()))
                .Returns(Task.FromException(exception));

            var actualException =
                await Assert.ThrowsAsync<HttpErrorException>(
                    () => controller.Update(model));

            actualException.ActionResult.Should().BeOfType<NotFoundResult>();
        }

        #endregion
    }
}