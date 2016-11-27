using System;
using System.Threading.Tasks;
using Common.Cqrs;

namespace Common.Tests.Cqrs
{
    public class TestHandler<T> : IHandler<T>
        where T : IMessage
    {
        public Exception ExceptionToThrow { get; set; }

        public int CallCount { get; private set; }

        public T Message { get; private set; }

        public Task Handle(T message)
        {
            CallCount++;
            Message = message;

            if (ExceptionToThrow != null)
            {
                throw ExceptionToThrow;
            }

            return Task.CompletedTask;
        }
    }
}
