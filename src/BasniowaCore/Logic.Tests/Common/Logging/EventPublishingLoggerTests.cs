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
    public class EventPublishingLoggerTests
    {
        public TestLogger<EventPublishingLogger> Logger { get; set; }

        public EventPublishingLoggerTests()
        {
            Logger = new TestLogger<EventPublishingLogger>();
        }

        public EventPublishingLogger Create()
        {
            return new EventPublishingLogger(Logger);
        }

        [Fact(DisplayName = nameof(EventPublishingLogger) + ": Log message is written before and after successful execution.")]
        public async Task Successful()
        {
            // arrange
            var processor = Create();
            var context = MessageProcessingContext.Create(new TestEvent(), () => Task.CompletedTask);
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
            logBefore.Message.Should().Be($"Event {nameof(TestEvent)} is being published.");
            logBefore.GetArgument<string>("MessageType").Should().Be(nameof(TestEvent));
            logBefore.EventId.Should().Be(LoggingEvents.EventPublishing);

            var logAfter = actualLogs[2];
            logAfter.LogLevel.Should().Be(LogLevel.Debug);
            logAfter.Message.Should().Be($"Event {nameof(TestEvent)} was published successfully.");
            logAfter.GetArgument<string>("MessageType").Should().Be(nameof(TestEvent));
            logAfter.EventId.Should().Be(LoggingEvents.EventPublished);
        }

        [Fact(DisplayName = nameof(EventPublishingLogger) + ": Log message is written before and after execution with exception.")]
        public async Task Exception()
        {
            // arrange
            var processor = Create();
            var context = MessageProcessingContext.Create(new TestEvent(), () => Task.CompletedTask);
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
            logBefore.EventId.Should().Be(LoggingEvents.EventPublishing);

            var logAfter = actualLogs[2];
            logAfter.LogLevel.Should().Be(LogLevel.Error);
            logAfter.Message.Should().Be($"Publication of event {nameof(TestEvent)} resulted with an exception.");
            logAfter.Exception.Should().BeSameAs(error);
            logAfter.GetArgument<string>("MessageType").Should().Be(nameof(TestEvent));
            logAfter.EventId.Should().Be(LoggingEvents.EventPublishingException);
        }
    }
}
