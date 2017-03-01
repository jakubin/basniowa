using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Logic.Shows;
using Moq;
using Website.Api.Shows;
using Website.Infrastructure.FileContainers;
using Xunit;

namespace Website.Tests.Api.Shows
{
    public class ShowsQueryControllerTests
    {
        public IShowsReader ShowsReader { get; set; }

        public IMapper Mapper { get; set; }

        public IFileContainerUrlProvider UrlProvider { get; set; }

        public ShowsQueryControllerTests()
        {
            var config = new MapperConfiguration(ShowsQueryController.ConfigureMapper);
            Mapper = new Mapper(config);

            ShowsReader = Mock.Of<IShowsReader>();
            UrlProvider = Mock.Of<IFileContainerUrlProvider>();
            Mock.Get(UrlProvider)
                .Setup(x => x.GetDownloadUrl(It.IsAny<string>()))
                .Returns<string>(x => $"pictures/{x}");
        }

        public ShowsQueryController Create()
        {
            return new ShowsQueryController
            {
                ShowsReader = ShowsReader,
                Mapper = Mapper,
                ShowPictures = new ShowPictureUrlProvider(UrlProvider)
            };
        }

        [Fact(DisplayName = nameof(ShowsCommandControllerTests) + ": GetAllShows should return correct list of shows.")]
        public void GetAllShowsSuccessful()
        {
            // arrange
            var shows = new List<ShowHeader>
            {
                new ShowHeader { ShowId = 1, Title = "T1", Subtitle = "S1" },
                new ShowHeader { ShowId = 2, Title = "T2", Subtitle = "S2" },
            };
            Mock.Get(ShowsReader).Setup(x => x.GetAllShows()).Returns(shows);
            var controller = Create();

            // act
            var actual = controller.GetAll();

            actual.Select(x => new { Id = x.ShowId, x.Title, x.Subtitle })
                .Should().BeEquivalentTo(
                shows.Select(x => new { Id = x.ShowId, x.Title, x.Subtitle }));
        }

        [Fact(DisplayName = nameof(ShowsCommandControllerTests) + ": GetById should return an existing show.")]
        public void GetByIdExisting()
        {
            // arrange
            long id = 123;
            var show = new ShowWithDetails
            {
                ShowId = id,
                Title = "T",
                Subtitle = "S",
                Description = "D",
                Properties = new Dictionary<string, string>
                {
                    ["P1"] = "V1",
                    ["P2"] = "V2",
                }
            };
            Mock.Get(ShowsReader).Setup(x => x.GetShowById(id)).Returns(show);
            var controller = Create();

            // act
            var actual = controller.GetById(id);

            actual.ShowId.Should().Be(show.ShowId);
            actual.Title.Should().Be(show.Title);
            actual.Subtitle.Should().Be(show.Subtitle);
            actual.Description.Should().Be(show.Description);
            actual.Properties.ShouldBeEquivalentTo(show.Properties);
        }

        [Fact]
        public async Task GetShowPictures_ReturnsListFromReader()
        {
            // arrange
            var pictures = new List<ShowPictureDetails>
            {
                new ShowPictureDetails
                {
                    ShowPictureId = 1,
                    Title = "abc",
                    ImagePath = "i1.jpg",
                    ThumbPath = "t1.jpg",
                    IsMainShowPicture = true
                },
                new ShowPictureDetails
                {
                    ShowPictureId = 2,
                    Title = "def",
                    ImagePath = "i2.jpg",
                    ThumbPath = "t2.jpg",
                    IsMainShowPicture = false
                },
            };
            Mock.Get(ShowsReader).Setup(x => x.GetShowPictures(1))
                .Returns(Task.FromResult<IList<ShowPictureDetails>>(pictures));
            var controller = Create();

            // act
            var actual = await controller.GetShowPictures(1);

            actual
                .Select(x => new {x.ShowPictureId, x.Title, x.ImageUrl, x.ThumbUrl, x.IsMainShowPicture})
                .Should().Equal(
                    new {ShowPictureId = 1L, Title = "abc", ImageUrl = "pictures/i1.jpg", ThumbUrl = "pictures/t1.jpg", IsMainShowPicture = true}, 
                    new {ShowPictureId = 2L, Title = "def", ImageUrl = "pictures/i2.jpg", ThumbUrl = "pictures/t2.jpg", IsMainShowPicture = false});
        }
    }
}
