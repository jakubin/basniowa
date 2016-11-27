using System;
using System.Threading.Tasks;
using Common.Cqrs;

namespace Common.Tests.Cqrs
{
    public class TestProcessor : IMessageProcessor
    {
        private bool _execute;
        private bool _hideException;

        public int CallCount { get; private set; }

        public MessageProcessingContext ReceivedContext { get; private set; }

        public Exception ReceivedException { get; set; }

        public Action OnCall { get; set; }

        public Exception ExceptionToThrow { get; private set; }

        public async Task Process(MessageProcessingContext context, MessageHandlerDelegate next)
        {
            CallCount++;
            ReceivedContext = context;

            OnCall?.Invoke();

            if (ExceptionToThrow != null)
            {
                throw ExceptionToThrow;
            }

            if (_execute)
            {
                try
                {
                    await next(context);
                }
                catch (Exception ex)
                {
                    ReceivedException = ex;
                    if (!_hideException)
                    {
                        throw;
                    }
                }
            }
        }

        private TestProcessor(bool execute, bool hideException, Exception exceptionToThrow)
        {
            _execute = execute;
            _hideException = hideException;
            ExceptionToThrow = exceptionToThrow;
        }

        public static TestProcessor PassThru()
        {
            return new TestProcessor(execute: true, hideException: false, exceptionToThrow: null);
        }

        public static TestProcessor HidingException()
        {
            return new TestProcessor(execute: true, hideException: true, exceptionToThrow: null);
        }

        public static TestProcessor Throwing(Exception exception)
        {
            return new TestProcessor(execute: true, hideException: false, exceptionToThrow: exception);
        }

        public static TestProcessor Cancelling()
        {
            return new TestProcessor(execute: false, hideException: false, exceptionToThrow: null);
        }
    }
}
