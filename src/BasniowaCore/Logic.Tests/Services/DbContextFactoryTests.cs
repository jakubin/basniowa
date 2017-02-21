using System;
using DataAccess.Database;
using FluentAssertions;
using Logic.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Logic.Tests.Services
{
    public class DbContextFactoryTests
    {
        [Fact(DisplayName = nameof(DbContextFactory) + ": Create() should correctly resolve DbContext.")]
        public void CreateSuccessful()
        {
            // arrange
            var options = new DbContextOptions<TheaterDb>();
            var db = new TheaterDb(options);
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(TheaterDb)))
                .Returns(db)
                .Verifiable();
            var factory = new DbContextFactory(serviceProviderMock.Object);

            // act
            var actual = factory.Create();

            // assert
            actual.Should().Be(db);
            serviceProviderMock.Verify();
        }
    }
}
