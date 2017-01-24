using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Cqrs;

namespace Logic.Tests.Helpers
{
    public class TestEventPublisher : IEventPublisher
    {
        public List<IEvent> PublishedEvents { get; } = new List<IEvent>();

        public Task Publish<T>(T @event) where T : IEvent
        {
            PublishedEvents.Add(@event);
            return Task.CompletedTask;
        }
    }
}