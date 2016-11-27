using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Cqrs;
using FluentAssertions;
using Xunit;

namespace Common.Tests.Cqrs
{
    public class MessageProcessingChainTests
    {
        public TestCommand Message { get; set; }

        public TestHandler<TestCommand> Handler { get; set; }

        public List<IMessageProcessor> Processors { get; set; }

        public MessageProcessingChainTests()
        {
            Message = new TestCommand();
            Handler = new TestHandler<TestCommand>();
            Processors = new List<IMessageProcessor>();
        }

        public MessageProcessingContext CreateContext()
        {
            return MessageProcessingContext.Create(Message, Handler);
        }

        public MessageProcessingChain Create()
        {
            var chain = new MessageProcessingChain();
            foreach (var processor in Processors)
            {
                chain.AddProcessor(processor);
            }

            return chain;
        }

        [Fact(DisplayName = nameof(MessageProcessingChain) + ": Chain from empty list of processors should just execute the handler.")]
        public async Task Empty()
        {
            // arrange
            var context = CreateContext();
            var chain = Create();

            // act
            await chain.Process(context);

            // assert
            Handler.CallCount.Should().Be(1);
        }

        [Fact(DisplayName = nameof(MessageProcessingChain) + ": Chain from empty list of processors should throw exception from handler.")]
        public async Task EmptyWithException()
        {
            // arrange
            var exception = new InvalidOperationException();
            Handler.ExceptionToThrow = exception;
            var context = CreateContext();
            var chain = Create();

            // act & assert
            var actualException =
                await Assert.ThrowsAsync<InvalidOperationException>(async () => { await chain.Process(context); });
            actualException.Should().BeSameAs(exception);
        }

        [Fact(DisplayName = nameof(MessageProcessingChain) + ": Chain from single processor should execute correctly.")]
        public async Task SingleProcessor()
        {
            // arrange
            var context = CreateContext();
            var processor = TestProcessor.PassThru();
            Processors.Add(processor);
            var chain = Create();

            // act
            await chain.Process(context);

            // assert
            processor.ReceivedContext.Should().BeSameAs(context);
            processor.CallCount.Should().Be(1);
            Handler.CallCount.Should().Be(1);
        }

        [Fact(DisplayName = nameof(MessageProcessingChain) + ": Chain from single processor should correctly pass exception from handler.")]
        public async Task SingleProcessorWithException()
        {
            // arrange
            Handler.ExceptionToThrow = new InvalidOperationException();
            var context = CreateContext();
            var processor = TestProcessor.PassThru();
            Processors.Add(processor);
            var chain = Create();

            // act
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(async () => await chain.Process(context));

            // assert
            processor.ReceivedException.Should().BeSameAs(Handler.ExceptionToThrow);
            actualException.Should().BeSameAs(Handler.ExceptionToThrow);
            processor.ReceivedException.Should().BeSameAs(Handler.ExceptionToThrow);
            processor.CallCount.Should().Be(1);
            Handler.CallCount.Should().Be(1);
        }

        [Fact(DisplayName = nameof(MessageProcessingChain) + ": Chain from 2 processors should execute correctly.")]
        public async Task TwoProcessors()
        {
            // arrange
            var context = CreateContext();
            var callOrder = new List<int>();
            var processor1 = TestProcessor.PassThru();
            processor1.OnCall = () => callOrder.Add(1);
            Processors.Add(processor1);
            var processor2 = TestProcessor.PassThru();
            processor2.OnCall = () => callOrder.Add(2);
            Processors.Add(processor2);
            var chain = Create();

            // act
            await chain.Process(context);

            // assert
            processor1.ReceivedContext.Should().BeSameAs(context);
            processor1.CallCount.Should().Be(1);
            processor2.ReceivedContext.Should().BeSameAs(context);
            processor2.CallCount.Should().Be(1);
            callOrder.Should().BeEquivalentTo(new[] { 1, 2 });
            Handler.CallCount.Should().Be(1);
        }

        [Fact(DisplayName = nameof(MessageProcessingChain) + ": Processor should be able to hide exception.")]
        public async Task HideException()
        {
            // arrange
            Handler.ExceptionToThrow = new InvalidOperationException();
            var context = CreateContext();
            var processor1 = TestProcessor.PassThru();
            Processors.Add(processor1);
            var processor2 = TestProcessor.HidingException();
            Processors.Add(processor2);
            var chain = Create();

            // act
            await chain.Process(context);

            // assert
            processor1.CallCount.Should().Be(1);
            processor1.ReceivedException.Should().BeNull();
            processor2.CallCount.Should().Be(1);
            processor2.ReceivedException.Should().BeSameAs(Handler.ExceptionToThrow);
            Handler.CallCount.Should().Be(1);
        }

        [Fact(DisplayName = nameof(MessageProcessingChain) + ": Processor should be able to cancel execution.")]
        public async Task CancelExecution()
        {
            // arrange
            Handler.ExceptionToThrow = new InvalidOperationException();
            var context = CreateContext();
            var processor1 = TestProcessor.Cancelling();
            Processors.Add(processor1);
            var processor2 = TestProcessor.PassThru();
            Processors.Add(processor2);
            var chain = Create();

            // act
            await chain.Process(context);

            // assert
            processor1.CallCount.Should().Be(1);
            processor2.CallCount.Should().Be(0);
            Handler.CallCount.Should().Be(0);
        }

        [Fact(DisplayName = nameof(MessageProcessingChain) + ": Processor should be able to throw own exceptions.")]
        public async Task ProcessorThrowing()
        {
            // arrange
            Handler.ExceptionToThrow = new InvalidOperationException();
            var context = CreateContext();
            var processor1 = TestProcessor.PassThru();
            Processors.Add(processor1);
            var processor2 = TestProcessor.Throwing(new InvalidOperationException());
            Processors.Add(processor2);
            var chain = Create();

            // act
            var actualException = await Assert.ThrowsAsync<InvalidOperationException>(async () => await chain.Process(context));

            // assert
            processor1.CallCount.Should().Be(1);
            processor1.ReceivedException.Should().Be(processor2.ExceptionToThrow);
            processor2.CallCount.Should().Be(1);
            Handler.CallCount.Should().Be(0);

            actualException.Should().BeSameAs(processor2.ExceptionToThrow);
        }

        [Fact(DisplayName = nameof(MessageProcessingChain) + ": Chain should allow adding new processors while in operation.")]
        public async Task AddingProcessors()
        {
            // arrange
            var context = CreateContext();
            var processor = TestProcessor.PassThru();
            Processors.Add(processor);
            var chain = Create();
            await chain.Process(context);
            var newProcessor = TestProcessor.PassThru();
            var newContext = CreateContext();

            // act
            chain.AddProcessor(newProcessor);
            await chain.Process(newContext);

            // assert
            processor.CallCount.Should().Be(2);
            newProcessor.CallCount.Should().Be(1);
            Handler.CallCount.Should().Be(2);

            newProcessor.ReceivedContext.Should().BeSameAs(newContext);
        }
    }
}
