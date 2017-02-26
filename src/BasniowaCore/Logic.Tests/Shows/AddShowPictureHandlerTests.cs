using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.FileContainers;
using DataAccess.Database.Shows;
using FluentAssertions;
using Logic.Common;
using Logic.Common.BusinessRules;
using Logic.Services;
using Logic.Shows;
using Logic.Tests.Helpers;
using Testing.FileContainers;
using Testing.Images;
using Xunit;

namespace Logic.Tests.Shows
{
    public class AddShowPictureHandlerTests : IDisposable
    {
        #region Setup

        private IUniqueIdService IdService { get; set; }

        private TestEventPublisher EventPublisher { get; set; }

        private IDateTimeService DateTimeService { get; set; }

        private IDbContextFactory DbContextFactory { get; set; }

        private InMemoryDb InMemoryDb { get; set; }

        private TestFileContainer FileContainer { get; set; }

        public AddShowPictureHandlerTests()
        {
            IdService = new TestUniqueIdService();

            EventPublisher = new TestEventPublisher();

            DateTimeService = new TestDateTimeService();

            InMemoryDb = new InMemoryDb();
            DbContextFactory = InMemoryDb;

            FileContainer = new TestFileContainer();
        }

        public void Dispose()
        {
            InMemoryDb.Dispose();
        }

        public AddShowPictureHandler CreateHandler()
        {
            return new AddShowPictureHandler()
            {
                IdService = IdService,
                EventPublisher = EventPublisher,
                DateTimeService = DateTimeService,
                DbFactory = DbContextFactory,
                ShowPictures = new ShowPicturesFileContainer(FileContainer)
            };
        }

        #endregion

        #region Tests

        [Fact(DisplayName = "Image is saved and show picture is added when command is valid.")]
        public async Task ImageAndShowPictureSaved()
        {
            // arrange
            var showId = await PrepareShow();
            long showPictureId = await IdService.GenerateId();
            var fileBytes = TestData.GetResourceFileBytes("Images.400x300.jpg");
            var command = new AddShowPictureCommand
            {
                ShowId = showId,
                ShowPictureId = showPictureId,
                FileName = "picture.jpg",
                FileStream = new MemoryStream(fileBytes),
                Title = "Picture",
                UserName = "test"
            };

            // act
            var handler = CreateHandler();
            await handler.Handle(command);

            // assert
            var actualPictures = GetAllShowPictures(showId);
            actualPictures.Should().HaveCount(1);

            var actualPicture = actualPictures[0];
            actualPicture.Id.Should().Be(showPictureId);
            actualPicture.ShowId.Should().Be(showId);
            actualPicture.Title.Should().Be(command.Title);
            actualPicture.ImagePath.Should().EndWith(command.FileName);
            actualPicture.ThumbPath.Should().EndWith(command.FileName);
            actualPicture.CreatedBy.Should().Be(command.UserName);
            actualPicture.CreatedUtc.Should().Be(DateTimeService.UtcNow);
            actualPicture.ModifiedBy.Should().Be(command.UserName);
            actualPicture.ModifiedUtc.Should().Be(DateTimeService.UtcNow);
            actualPicture.IsDeleted.Should().Be(false);

            var actualFiles = await FileContainer.GetAllFiles();
            actualFiles.Should().BeEquivalentTo(actualPicture.ImagePath, actualPicture.ThumbPath);

            EventPublisher.PublishedEvents.Should().HaveCount(1);
            var actualEvent = EventPublisher.PublishedEvents.First().Should().BeOfType<ShowPictureAddedEvent>().Subject;
            actualEvent.ShowId.Should().Be(showId);
            actualEvent.ShowPictureId.Should().Be(showPictureId);
        }

        [Theory(DisplayName = "Compressed image is saved correctly and a valid thumbnail is generated.")]
        [InlineData("Images.100x50.jpg", 200, 100)]
        [InlineData("Images.200x200.jpg", 200, 200)]
        [InlineData("Images.200x200.png", 200, 200)]
        [InlineData("Images.300x400.jpg", 150, 200)]
        [InlineData("Images.400x300.jpg", 200, 150)]
        public async Task CompressedImageCorrectlySavedAndResized(string imagePath, int expectedThumbWidth, int expectedThumbHeight)
        {
            // arrange
            var showId = await PrepareShow();
            long showPictureId = await IdService.GenerateId();
            var fileBytes = TestData.GetResourceFileBytes(imagePath);
            var command = new AddShowPictureCommand
            {
                ShowId = showId,
                ShowPictureId = showPictureId,
                FileName = "picture.jpg",
                FileStream = new MemoryStream(fileBytes),
                Title = "Picture",
                UserName = "test"
            };

            // act
            var handler = CreateHandler();
            await handler.Handle(command);

            // assert
            var actualPicture = GetAllShowPictures(showId).Single();
            var actualImageBytes = await FileContainer.ReadAllBytes(actualPicture.ImagePath);
            ImageAssert.AreAlmostIdentical(fileBytes, actualImageBytes);

            var actualThumbsBytes = await FileContainer.ReadAllBytes(actualPicture.ThumbPath);
            ImageAssert.HasSize(actualThumbsBytes, expectedThumbWidth, expectedThumbHeight);
        }

        [Theory(DisplayName = "Uncompressed image is saved correctly (as JPG) and a valid thumbnail is generated.")]
        [InlineData("Images.200x200.bmp", 200, 200)]
        [InlineData("Images.200x200.tif", 200, 200)]
        public async Task UncompressedImageIsCompressed(string imagePath, int expectedThumbWidth, int expectedThumbHeight)
        {
            // arrange
            var showId = await PrepareShow();
            long showPictureId = await IdService.GenerateId();
            var fileBytes = TestData.GetResourceFileBytes(imagePath);
            var command = new AddShowPictureCommand
            {
                ShowId = showId,
                ShowPictureId = showPictureId,
                FileName = "image" + Path.GetExtension(imagePath),
                FileStream = new MemoryStream(fileBytes),
                Title = "Picture",
                UserName = "test"
            };

            // act
            var handler = CreateHandler();
            await handler.Handle(command);

            // assert
            var actualPicture = GetAllShowPictures(showId).Single();
            actualPicture.ImagePath.Should().EndWith("image.jpg");
            actualPicture.ThumbPath.Should().EndWith("image.jpg");

            var actualImageBytes = await FileContainer.ReadAllBytes(actualPicture.ImagePath);
            ImageAssert.AreAlmostIdentical(fileBytes, actualImageBytes);
            ImageAssert.IsJpeg(actualImageBytes);

            var actualThumbsBytes = await FileContainer.ReadAllBytes(actualPicture.ThumbPath);
            ImageAssert.HasSize(actualThumbsBytes, expectedThumbWidth, expectedThumbHeight);
            ImageAssert.IsJpeg(actualThumbsBytes);
        }

        [Fact(DisplayName = "Business rule is violated when show picture has not valid extension.")]
        public async Task InvalidExtension()
        {
            // arrange
            var showId = await PrepareShow();
            long showPictureId = await IdService.GenerateId();
            var fileBytes = TestData.GetResourceFileBytes("Other.Sample.docx");
            var command = new AddShowPictureCommand
            {
                ShowId = showId,
                ShowPictureId = showPictureId,
                FileName = "Sample.docx",
                FileStream = new MemoryStream(fileBytes),
                Title = "Picture",
                UserName = "test"
            };

            // act
            var handler = CreateHandler();
            var exception = await Assert.ThrowsAsync<BusinessRuleException<FileMustHaveImageExtensionRule>>(
                async () => await handler.Handle(command));

            // assert
            exception.BusinessRule.FileExtension.Should().Be(".docx");
            GetAllShowPictures(showId).Should().BeEmpty();
            FileContainer.GetAllFiles().Result.Should().BeEmpty();
        }

        [Fact(DisplayName = "Business rule is violated when picture has valid extension, but is not a valid image.")]
        public async Task FileNotImage()
        {
            // arrange
            var showId = await PrepareShow();
            long showPictureId = await IdService.GenerateId();
            var fileBytes = TestData.GetResourceFileBytes("Other.Sample.docx");
            var command = new AddShowPictureCommand
            {
                ShowId = showId,
                ShowPictureId = showPictureId,
                FileName = "Sample.jpg",
                FileStream = new MemoryStream(fileBytes),
                Title = "Picture",
                UserName = "test"
            };

            // act
            var handler = CreateHandler();
            await Assert.ThrowsAsync<BusinessRuleException<FileMustBeImageRule>>(
                async () => await handler.Handle(command));

            // assert
            GetAllShowPictures(showId).Should().BeEmpty();
            FileContainer.GetAllFiles().Result.Should().BeEmpty();
        }

        [Fact(DisplayName = "EntityNotFoundException is thrown when show doesn't exist.")]
        public async Task ShowDoesNotExist()
        {
            // arrange
            long showId = -1;
            long showPictureId = await IdService.GenerateId();
            var fileBytes = TestData.GetResourceFileBytes("Images.200x200.jpg");
            var command = new AddShowPictureCommand
            {
                ShowId = showId,
                ShowPictureId = showPictureId,
                FileName = "Sample.jpg",
                FileStream = new MemoryStream(fileBytes),
                Title = "Picture",
                UserName = "test"
            };

            // act
            var handler = CreateHandler();
            await Assert.ThrowsAsync<EntityNotFoundException<Show>>(
                async () => await handler.Handle(command));

            // assert
            GetAllShowPictures(showId).Should().BeEmpty();
            FileContainer.GetAllFiles().Result.Should().BeEmpty();
        }

        [Fact(DisplayName = "EntityNotFoundException is thrown when show was deleted.")]
        public async Task ShowDeleted()
        {
            // arrange
            var showId = await PrepareShow(x => x.IsDeleted = true);
            long showPictureId = await IdService.GenerateId();
            var fileBytes = TestData.GetResourceFileBytes("Images.200x200.jpg");
            var command = new AddShowPictureCommand
            {
                ShowId = showId,
                ShowPictureId = showPictureId,
                FileName = "Sample.jpg",
                FileStream = new MemoryStream(fileBytes),
                Title = "Picture",
                UserName = "test"
            };

            // act
            var handler = CreateHandler();
            await Assert.ThrowsAsync<EntityNotFoundException<Show>>(
                async () => await handler.Handle(command));

            // assert
            GetAllShowPictures(showId).Should().BeEmpty();
            FileContainer.GetAllFiles().Result.Should().BeEmpty();
        }

        #endregion

        #region Helpers

        private async Task<long> PrepareShow(Action<Show> customActions = null)
        {
            long showId = await IdService.GenerateId();

            var db = InMemoryDb.Create();
            var show = TestData.CreateShow(id: showId);
            customActions?.Invoke(show);

            db.Shows.Add(show);

            db.SaveChanges();

            return showId;
        }

        private ShowPicture[] GetAllShowPictures(long showId)
        {
            var db = InMemoryDb.Create(false);
            return db.ShowPictures.Where(x => x.ShowId == showId).ToArray();
        }

        #endregion
    }
}