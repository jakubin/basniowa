using System;
using System.Threading.Tasks;
using Common.Cqrs;
using FluentAssertions;
using Logic.Common.Logging;
using Microsoft.Extensions.Logging;
using Testing.Logging;
using Xunit;

namespace Logic.Tests.Common.Logging
{
    public class EventHandlingLoggerTests
    {
        public TestLogger<EventHandlingLogger> Logger { get; set; }

        public EventHandlingLoggerTests()
        {
            Logger = new TestLogger<EventHandlingLogger>();
        }

        public EventHandlingLogger Create()
        {
            return new EventHandlingLogger(Logger);
        }

        [Fact(DisplayName = nameof(EventHandlingLogger) + ": Log message is written before and after successful execution.")]
        public async Task Successful()
        {
            // arrange
            var processor = Create();
            var context = MessageProcessingContext.Create(new TestEvent(), new TestEventHandler());
            MessageHandlerDelegate next = ctx =>
            {
                Logger.LogTrace("Inside next handler"); // separates logs
                return Task.CompletedTask;
            };

            // act
            await processor.Process(context, next);

            // assert
            var actualLogs = Logger.GetEntries();
            actualLogs.Should().HaveCount(3);

            var logBefore = actualLogs[0];
            logBefore.LogLevel.Should().Be(LogLevel.Information);
            logBefore.Message.Should().Be($"Event {nameof(TestEvent)} will be handled by {nameof(TestEventHandler)}.");
            logBefore.GetArgument<string>("MessageType").Should().Be(nameof(TestEvent));
            logBefore.GetArgument<string>("HandlerType").Should().Be(nameof(TestEventHandler));
            logBefore.EventId.Should().Be(LoggingEvents.EventHandling);

            var logAfter = actualLogs[2];
            logAfter.LogLevel.Should().Be(LogLevel.Information);
            logAfter.Message.Should().Be($"Event {nameof(TestEvent)} was successfully handled by {nameof(TestEventHandler)}.");
            logAfter.GetArgument<string>("MessageType").Should().Be(nameof(TestEvent));
            logAfter.GetArgument<string>("HandlerType").Should().Be(nameof(TestEventHandler));
            logAfter.EventId.Should().Be(LoggingEvents.EventHandled);
        }

        [Fact(DisplayName = nameof(EventHandlingLogger) + ": Log message is written before and after execution with exception.")]
        public async Task Exception()
        {
            // arrange
            var processor = Create();
            var context = MessageProcessingContext.Create(new TestEvent(), new TestEventHandler());
            var error = new InvalidOperationException();
            MessageHandlerDelegate next = ctx =>
            {
                Logger.LogTrace("Inside next handler"); // separates logs
                return Task.FromException(error);
            };

            // act
            var actualException = 
                await Assert.ThrowsAsync<InvalidOperationException>(async () => await processor.Process(context, next));

            // assert
            actualException.Should().BeSameAs(error);
            var actualLogs = Logger.GetEntries();
            actualLogs.Should().HaveCount(3);

            var logBefore = actualLogs[0];
            logBefore.LogLevel.Should().Be(LogLevel.Information);
            logBefore.EventId.Should().Be(LoggingEvents.EventHandling);

            var logAfter = actualLogs[2];
            logAfter.LogLevel.Should().Be(LogLevel.Error);
            logAfter.Message.Should().Be($"Processing of event {nameof(TestEvent)} by handler {nameof(TestEventHandler)} resulted with an exception.");
            logAfter.Exception.Should().BeSameAs(error);
            logAfter.GetArgument<string>("MessageType").Should().Be(nameof(TestEvent));
            logAfter.GetArgument<string>("HandlerType").Should().Be(nameof(TestEventHandler));
            logAfter.EventId.Should().Be(LoggingEvents.EventHandlingException);
        }
    }
}
