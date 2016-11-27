using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Cqrs;
using FluentAssertions;
using Moq;
using Xunit;

namespace Common.Tests.Cqrs
{
    public class SynchronousMessageBusCommandProcessorsTests
    {
        private Mock<IHandlerResolver> HandlerResolverMock { get; set; }

        public TestHandler<TestCommand> CommandHandler { get; set; }

        private SynchronousMessageBus CreateBus()
        {
            return new SynchronousMessageBus(HandlerResolverMock?.Object);
        }

        public SynchronousMessageBusCommandProcessorsTests()
        {
            CommandHandler = new TestHandler<TestCommand>();
            HandlerResolverMock = new Mock<IHandlerResolver>();
            HandlerResolverMock.Setup(x => x.Resolve<TestCommand>()).Returns(new[] { CommandHandler });
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Processing chain should be executed while sending command.")]
        public async Task CommandProcessingChain()
        {
            // arrange
            var command = new TestCommand();
            var bus = CreateBus();
            var processor = TestProcessor.PassThru();
            bus.AddCommandHandlerProcessor(processor);

            // act
            await bus.Send(command);

            // assert
            processor.CallCount.Should().Be(1);
            processor.ReceivedContext.Message.Should().BeSameAs(command);
            processor.ReceivedContext.Handler.Should().BeSameAs(CommandHandler);
        }
    }
}
