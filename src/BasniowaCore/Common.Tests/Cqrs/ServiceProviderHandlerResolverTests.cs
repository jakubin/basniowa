using Common.Cqrs;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Common.Tests.Cqrs
{
    public class ServiceProviderHandlerResolverTests
    {
        [Fact(DisplayName = nameof(ServiceProviderHandlerResolver) + ": Resolve() should correctly return when no dependency is registered.")]
        public void ResolveNone()
        {
            var handlers = new IHandler<TestCommand>[0];
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<IHandler<TestCommand>>)))
                .Returns(handlers);
            var resolver = new ServiceProviderHandlerResolver(serviceProviderMock.Object);

            var actualHandlers = resolver.Resolve<TestCommand>();
            actualHandlers.Should().BeEmpty();
        }

        [Fact(DisplayName = nameof(ServiceProviderHandlerResolver) + ": Resolve() should correctly return when single dependency is registered.")]
        public void ResolveSingle()
        {
            var handlers = new IHandler<TestCommand>[]
            {
                new Mock<IHandler<TestCommand>>().Object
            };
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<IHandler<TestCommand>>)))
                .Returns(handlers);
            var resolver = new ServiceProviderHandlerResolver(serviceProviderMock.Object);

            var actualHandlers = resolver.Resolve<TestCommand>();
            actualHandlers.Should().BeEquivalentTo(handlers);
        }

        [Fact(DisplayName = nameof(ServiceProviderHandlerResolver) + ": Resolve() should correctly return when multiple dependency is registered.")]
        public void ResolveMultiple()
        {
            var handlers = new IHandler<TestEvent>[]
            {
                new Mock<IHandler<TestEvent>>().Object,
                new Mock<IHandler<TestEvent>>().Object
            };
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(x => x.GetService(typeof(IEnumerable<IHandler<TestEvent>>)))
                .Returns(handlers);
            var resolver = new ServiceProviderHandlerResolver(serviceProviderMock.Object);

            var actualHandlers = resolver.Resolve<TestEvent>();
            actualHandlers.Should().BeEquivalentTo(handlers);
        }
    }
}
