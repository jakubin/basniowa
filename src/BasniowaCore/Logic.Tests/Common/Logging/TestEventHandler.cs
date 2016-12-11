using System.Threading.Tasks;
using Common.Cqrs;

namespace Logic.Tests.Common.Logging
{
    public class TestEventHandler : IHandler<TestEvent>
    {
        public Task Handle(TestEvent @event)
        {
            return Task.CompletedTask;
        }
    }
}