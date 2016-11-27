using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Cqrs;
using FluentAssertions;
using Moq;
using Xunit;

namespace Common.Tests.Cqrs
{
    public class SynchronousMessageBusTests
    {
        private Mock<IHandlerResolver> HandlerResolverMock { get; set; }

        private SynchronousMessageBus CreateBus()
        {
            return new SynchronousMessageBus(
                HandlerResolverMock?.Object);
        }

        public SynchronousMessageBusTests()
        {
            HandlerResolverMock = new Mock<IHandlerResolver>(MockBehavior.Strict);
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Ctor should not allow null handler resolver.")]
        public void Ctor_Null_Handler_Resolver()
        {
            // arrange
            HandlerResolverMock = null;
            
            // act & assert
            Assert.Throws<ArgumentNullException>("handlerResolver", () => CreateBus());
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Send should fail when no command handlers are available.")]
        public async Task Publish_Command_No_Handlers()
        {
            // arrange
            HandlerResolverMock.Setup(x => x.Resolve<TestCommand>()).Returns(() => new List<IHandler<TestCommand>>());
            var command = new TestCommand();
            var bus = CreateBus();

            // act & assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => { await bus.Send(command); });
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Send should fail when multiple command handlers are available.")]
        public async Task Publish_Command_Multiple_Handlers()
        {
            // arrange
            var handler1 = new Mock<IHandler<TestCommand>>().Object;
            var handler2 = new Mock<IHandler<TestCommand>>().Object;
            HandlerResolverMock.Setup(x => x.Resolve<TestCommand>()).Returns(() => new IHandler<TestCommand>[] { handler1, handler2 });
            var command = new TestCommand();
            var bus = CreateBus();

            // act & assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => { await bus.Send(command); });
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Send should call single handler's Handle method.")]
        public async Task Publish_Command_Single_Handler()
        {
            // arrange
            var command = new TestCommand();
            var handlerMock = new Mock<IHandler<TestCommand>>();
            handlerMock
                .Setup(x => x.Handle(command))
                .Returns(Task.CompletedTask)
                .Verifiable("Handler's Handle() method has not been called.");
            HandlerResolverMock
                .Setup(x => x.Resolve<TestCommand>())
                .Returns(() => new IHandler<TestCommand>[] { handlerMock.Object });
            var bus = CreateBus();

            // act
            await bus.Send(command);

            // assert
            handlerMock.Verify();
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Handling commands should be async.")]
        public async Task Publish_Command_Async()
        {
            // arrange
            var command = new TestCommand();
            var tcs = new TaskCompletionSource<object>();
            var handlerMock = new Mock<IHandler<TestCommand>>();
            handlerMock
                .Setup(x => x.Handle(command))
                .Returns(tcs.Task)
                .Verifiable("Handler's Handle() method has not been called.");
            HandlerResolverMock
                .Setup(x => x.Resolve<TestCommand>())
                .Returns(() => new IHandler<TestCommand>[] { handlerMock.Object });
            var bus = CreateBus();

            // act 1
            var publishTask = bus.Send(command);
            // assert 1
            handlerMock.Verify();
            publishTask.IsCompleted.Should().Be(false);

            // act 2
            tcs.SetResult(null);
            // assert 2
            await publishTask;
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Publish should silently continue when no event handlers are available.")]
        public async Task Publish_Event_No_Handlers()
        {
            // arrange
            HandlerResolverMock.Setup(x => x.Resolve<TestEvent>()).Returns(() => new List<IHandler<TestEvent>>());
            var @event = new TestEvent();
            var bus = CreateBus();

            // act & assert
            await bus.Publish(@event);
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Publish should call all event handlers.")]
        public async Task Publish_Event_Multiple_Handlers()
        {
            // arrange
            var @event = new TestEvent();
            var handler1 = new Mock<IHandler<TestEvent>>();
            handler1.Setup(x => x.Handle(@event))
                .Returns(Task.CompletedTask)
                .Verifiable("Handler 1 should be called.");
            var handler2 = new Mock<IHandler<TestEvent>>();
            handler2.Setup(x => x.Handle(@event))
                .Returns(Task.CompletedTask)
                .Verifiable("Handler 2 should be called.");
            HandlerResolverMock.Setup(x => x.Resolve<TestEvent>())
                .Returns(() => new IHandler<TestEvent>[] { handler1.Object, handler2.Object });
            var bus = CreateBus();

            // act
            await bus.Publish(@event);

            // assert
            handler1.Verify();
            handler2.Verify();
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Exceptions raised while handling events should be ignored.")]
        public async Task Publish_Event_With_Exception()
        {
            // arrange
            var @event = new TestEvent();
            var handler = new Mock<IHandler<TestEvent>>();
            handler.Setup(x => x.Handle(@event))
                .Returns(Task.FromException(new InvalidOperationException()))
                .Verifiable("Handler 1 should be called.");
            HandlerResolverMock.Setup(x => x.Resolve<TestEvent>())
                .Returns(() => new IHandler<TestEvent>[] { handler.Object });
            var bus = CreateBus();

            // act
            await bus.Publish(@event); // should not throw

            // assert
            handler.Verify();
        }

        [Fact(DisplayName = nameof(SynchronousMessageBus) + ": Handling events should be async.")]
        public async Task Publish_Event_Async()
        {
            // arrange
            var @event = new TestEvent();
            var tcs = new TaskCompletionSource<object>();
            var handlerMock = new Mock<IHandler<TestEvent>>();
            handlerMock
                .Setup(x => x.Handle(@event))
                .Returns(tcs.Task)
                .Verifiable("Handler's Handle() method has not been called.");
            HandlerResolverMock
                .Setup(x => x.Resolve<TestEvent>())
                .Returns(() => new IHandler<TestEvent>[] { handlerMock.Object });
            var bus = CreateBus();

            // act 1
            var publishTask = bus.Publish(@event);
            // assert 1
            handlerMock.Verify();
            publishTask.IsCompleted.Should().Be(false);

            // act 2
            tcs.SetResult(null);
            // assert 2
            await publishTask;
        }
    }
}
