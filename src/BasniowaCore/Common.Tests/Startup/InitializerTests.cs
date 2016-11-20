using System;
using Common.Startup;
using FluentAssertions;
using Xunit;

namespace Common.Tests.Startup
{
    public class InitializerTests
    {
        [Fact(DisplayName = nameof(Initializer) + ": Init should call argumentless methods.")]
        public void Init_Should_Call_Argumentless_Methods()
        {
            SimpleStartup.Reset();

            Initializer.Init<SimpleStartupAttribute>();

            SimpleStartup.StartupMethodCalls.Should().Be(1);
            SimpleStartup.OtherMethodCalls.Should().Be(0);
        }

        [Fact(DisplayName = nameof(Initializer) + ": Init should call methods with arguments.")]
        public void Init_Should_Call_Methods_With_Arguments()
        {
            ComplexStartup.Reset();

            var userType = new ComplexUserType();
            Initializer.Init<ComplexStartupAttribute>("name", 15, userType);

            ComplexStartup.StartupMethodCalls.Should()
                .HaveCount(1)
                .And.Equal(new Tuple<string, int, IComplexUserType>("name", 15, userType));
            ComplexStartup.OtherMethodCalls.Should().Be(0);
        }

        [Fact(DisplayName = nameof(Initializer) + ": Init should call multiple startup methods.")]
        public void Init_Should_Call_Multiple_Startup_Methods()
        {
            MultipleStartup1.Reset();
            MultipleStartup2.Reset();

            Initializer.Init<MultipleStartupAttribute>();

            MultipleStartup1.StartupMethodCalls.Should().Be(1);
            MultipleStartup2.StartupMethodCalls.Should().Be(1);
        }
    }
}
