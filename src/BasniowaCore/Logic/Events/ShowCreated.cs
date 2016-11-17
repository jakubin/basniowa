using Common.Cqrs;

namespace Logic.Events
{
    public class ShowCreated: IEvent
    {
        public long Id { get; set; }
    }
}
