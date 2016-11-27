using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Cqrs;
using FluentAssertions;
using Moq;
using Xunit;

namespace Common.Tests.Cqrs
{
    public class SynchronousMessageBusEventProcessorsTests
    {
        private Mock<IHandlerResolver> HandlerResolverMock { get; set; }

        private List<IHandler<TestEvent>> EventHandlers { get; set; }

        private SynchronousMessageBus CreateBus()
        {
            return new SynchronousMessageBus(HandlerResolverMock?.Object);
        }

        public SynchronousMessageBusEventProcessorsTests()
        {
            EventHandlers = new List<IHandler<TestEvent>>();
            HandlerResolverMock = new Mock<IHandlerResolver>();
            HandlerResolverMock.Setup(x => x.Resolve<TestEvent>()).Returns(EventHandlers);
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Event publication chain should be executed while publishing event.")]
        public async Task EventPublicationChain()
        {
            // arrange
            var @event = new TestEvent();
            var bus = CreateBus();
            EventHandlers.Add(new TestHandler<TestEvent>());
            EventHandlers.Add(new TestHandler<TestEvent>());
            var processor = TestProcessor.PassThru();
            bus.AddEventPublicationProcessor(processor);

            // act
            await bus.Publish(@event);

            // assert
            processor.CallCount.Should().Be(1);
            processor.ReceivedContext.Message.Should().BeSameAs(@event);
            processor.ReceivedContext.Handler.Should().BeNull();
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Event handling chain should be executed while publishing event.")]
        public async Task EventHandlingChain()
        {
            // arrange
            var @event = new TestEvent();
            var bus = CreateBus();
            EventHandlers.Add(new TestHandler<TestEvent>());
            EventHandlers.Add(new TestHandler<TestEvent>());
            var processor = TestProcessor.PassThru();
            var processorInvocations = new List<MessageProcessingContext>();
            processor.OnCall = () => processorInvocations.Add(processor.ReceivedContext);
            bus.AddEventHandlerProcessor(processor);

            // act
            await bus.Publish(@event);

            // assert
            processor.CallCount.Should().Be(2);
            for (int i = 0; i < EventHandlers.Count; i++)
            {
                var handler = EventHandlers[i];
                var context = processorInvocations[i];
                context.Message.Should().BeSameAs(@event);
                context.Handler.Should().BeSameAs(handler);
            }
        }
    }
}
