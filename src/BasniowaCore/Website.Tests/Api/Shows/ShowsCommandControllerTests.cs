using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;
using AutoMapper;
using Common.Cqrs;
using FluentAssertions;
using Logic.Shows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
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

            await AssertBadRequest(() => controller.Add(model));
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

            await AssertBadRequest(() => controller.Delete(model));
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

            await AssertBadRequest(() => controller.Update(model));
        }

        #endregion

        #region AddPicture

        [Fact]
        public async Task AddPicture_Success()
        {
            var model = new AddShowPictureModel()
            {
                ShowId = 10,
                Title = "Some image",
                Picture = new FormFile(new MemoryStream(new byte[] {1,2,3}), 0, 3, "Picture", "aaa.jpg")
            };
            var controller = Create();
            UniqueIdService.NextId = 11;

            AddShowPictureCommand command = null;
            var actualFile = new MemoryStream();
            Mock.Get(CommandSender)
                .Setup(x => x.Send(It.IsAny<AddShowPictureCommand>()))
                .Callback<AddShowPictureCommand>(c =>
                {
                    command = c;
                    c.FileStream.CopyTo(actualFile);
                })
                .Returns(Task.CompletedTask)
                .Verifiable();

            await controller.AddPicture(model);

            Mock.Get(CommandSender).Verify();
            command.ShowId.Should().Be(10);
            command.ShowPictureId.Should().Be(11);
            command.FileName.Should().Be("aaa.jpg");
            command.UserName.Should().Be(UserName);
            actualFile.ToArray().Should().Equal(1, 2, 3);
        }

        [Fact]
        public async Task AddPicture_BadModel()
        {
            var model = new AddShowPictureModel() { ShowId = null, Title = null, Picture = null};
            var controller = Create();
            controller.ModelState.AddModelError("key", "error");

            await AssertBadRequest(() => controller.AddPicture(model));
        }

        #endregion

        #region SetMainPicture

        [Fact]
        public async Task SetMainPicture_Success()
        {
            var model = new SetShowMainPictureModel {ShowId = 10, ShowPictureId = 11};
            var controller = Create();

            SetShowMainPictureCommand command = null;
            Mock.Get(CommandSender)
                .Setup(x => x.Send(It.IsAny<SetShowMainPictureCommand>()))
                .Callback<SetShowMainPictureCommand>(c => { command = c; })
                .Returns(Task.CompletedTask)
                .Verifiable();

            await controller.SetMainPicture(model);

            Mock.Get(CommandSender).Verify();
            command.ShowId.Should().Be(10);
            command.ShowPictureId.Should().Be(11);
            command.UserName.Should().Be(UserName);
        }

        [Fact]
        public async Task SetMainPicture_BadModel()
        {
            var model = new SetShowMainPictureModel { ShowId = null, ShowPictureId = 11 };
            var controller = Create();
            controller.ModelState.AddModelError("key", "error");

            await AssertBadRequest(() => controller.SetMainPicture(model));
        }

        #endregion

        #region DeleteShowPicture

        [Fact]
        public async Task DeleteShowPicture_Success()
        {
            var model = new DeleteShowPictureModel { ShowPictureId = 11 };
            var controller = Create();

            DeleteShowPictureCommand command = null;
            Mock.Get(CommandSender)
                .Setup(x => x.Send(It.IsAny<DeleteShowPictureCommand>()))
                .Callback<DeleteShowPictureCommand>(c => { command = c; })
                .Returns(Task.CompletedTask)
                .Verifiable();

            await controller.DeletePicture(model);

            Mock.Get(CommandSender).Verify();
            command.ShowPictureId.Should().Be(11);
            command.UserName.Should().Be(UserName);
        }

        [Fact]
        public async Task DeleteShowPicture_BadModel()
        {
            var model = new DeleteShowPictureModel { ShowPictureId = null };
            var controller = Create();
            controller.ModelState.AddModelError("key", "error");

            await AssertBadRequest(() => controller.DeletePicture(model));
        }

        #endregion

        #region Helpers

        private async Task AssertBadRequest(Func<Task> controllerActionCall)
        {
            var exception = await Assert.ThrowsAsync<HttpErrorException>(controllerActionCall);
            exception.ActionResult.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion
    }
}