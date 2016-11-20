using System.Collections.Generic;
using FluentAssertions;
using Logic;
using Logic.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Website.Api.Shows;
using Xunit;

namespace Tests.Controllers
{
    public class ShowsController_GetById_Tests
    {
        public ShowsController Controller { get; set; }

        public Mock<IShowsProvider> ShowsProviderMock { get; set; }

        public ShowsController_GetById_Tests()
        {
            ShowsProviderMock = new Mock<IShowsProvider>(MockBehavior.Strict);
            Controller = new ShowsController();
            Controller.ShowsProvider = ShowsProviderMock.Object;
        }

        [Fact]
        public void When_ShowExists_ReturnsItCorrectly()
        {
            long showId = 10;
            var show = new ShowWithDetails
            {
                Id = showId,
                Title = "A",
                Subtitle = "B",
                Description = "C",
                Properties = new Dictionary<string, string>
                {
                    { "Prop1", "Val1" },
                    { "Prop2", "Val2" }
                }
            };

            ShowsProviderMock.Setup(x => x.GetShowById(showId)).Returns(show);

            var result = Controller.GetById(showId);

            result.Should().BeOfType<ObjectResult>()
                .Subject.Value.Should().BeOfType<ShowWithDetails>();
            var value = result.As<ObjectResult>().Value.As<ShowWithDetails>();

            var actual = new { value.Id, value.Title, value.Subtitle, value.Description };
            var expected = new { show.Id, show.Title, show.Subtitle, show.Description };
            actual.ShouldBeEquivalentTo(expected, "show value should match");
            value.Properties.ShouldBeEquivalentTo(show.Properties, "show.Properties should match");
        }

        [Fact]
        public void When_ShowNotExists_ReturnsNotFound()
        {
            long showId = 10;

            ShowsProviderMock.Setup(x => x.GetShowById(showId)).Throws(new EntityNotFoundException<ShowWithDetails>($"Id={showId}"));

            var result = Controller.GetById(showId);

            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
