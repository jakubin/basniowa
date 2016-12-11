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
    public class CommandHandlingLoggerTests
    {
        public TestLogger<CommandHandlingLogger> Logger { get; set; }

        public CommandHandlingLoggerTests()
        {
            Logger = new TestLogger<CommandHandlingLogger>();
        }

        public CommandHandlingLogger Create()
        {
            return new CommandHandlingLogger(Logger);
        }

        [Fact(DisplayName = nameof(CommandHandlingLogger) + ": Log message is written before and after successful execution.")]
        public async Task Successful()
        {
            // arrange
            var processor = Create();
            var context = MessageProcessingContext.Create(new TestCommand(), new TestCommandHandler());
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
            logBefore.Message.Should().Be($"Command {nameof(TestCommand)} will be handled by {nameof(TestCommandHandler)}.");
            logBefore.GetArgument<string>("MessageType").Should().Be(nameof(TestCommand));
            logBefore.GetArgument<string>("HandlerType").Should().Be(nameof(TestCommandHandler));
            logBefore.EventId.Should().Be(LoggingEvents.CommandHandling);

            var logAfter = actualLogs[2];
            logAfter.LogLevel.Should().Be(LogLevel.Information);
            logAfter.Message.Should().Be($"Command {nameof(TestCommand)} was successfully handled by {nameof(TestCommandHandler)}.");
            logAfter.GetArgument<string>("MessageType").Should().Be(nameof(TestCommand));
            logAfter.GetArgument<string>("HandlerType").Should().Be(nameof(TestCommandHandler));
            logAfter.EventId.Should().Be(LoggingEvents.CommandHandled);
        }

        [Fact(DisplayName = nameof(CommandHandlingLogger) + ": Log message is written before and after execution with exception.")]
        public async Task Exception()
        {
            // arrange
            var processor = Create();
            var context = MessageProcessingContext.Create(new TestCommand(), new TestCommandHandler());
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
            logBefore.EventId.Should().Be(LoggingEvents.CommandHandling);

            var logAfter = actualLogs[2];
            logAfter.LogLevel.Should().Be(LogLevel.Information);
            logAfter.Message.Should().Be($"Processing of command {nameof(TestCommand)} by handler {nameof(TestCommandHandler)} resulted with an exception.");
            logAfter.Exception.Should().BeSameAs(error);
            logAfter.GetArgument<string>("MessageType").Should().Be(nameof(TestCommand));
            logAfter.GetArgument<string>("HandlerType").Should().Be(nameof(TestCommandHandler));
            logAfter.EventId.Should().Be(LoggingEvents.CommandHandlingException);
        }
    }
}
