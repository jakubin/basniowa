using Common.Cqrs;

namespace Common.Tests.Cqrs
{
    public class TestEvent : IEvent
    {
        public string Text { get; }

        public TestEvent(string text = null)
        {
            Text = text;
        }
    }
}
