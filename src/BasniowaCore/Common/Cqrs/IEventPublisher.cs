using System.Threading.Tasks;

namespace Common.Cqrs
{
    /// <summary>
    /// Publishes events.
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes the specified event, so that registered handlers can process it.
        /// </summary>
        /// <typeparam name="T">Type of event.</typeparam>
        /// <param name="event">The event to publish.</param>
        /// <returns>Task representing the operation of event registration; not necessarily event handling.</returns>
        /// <remarks>
        /// You should not assume that the event will be executed immediately.
        /// Any exceptions raised during event processing will not surface this API.
        /// Events should be published only within command and event handlers.
        /// </remarks>
        Task PublishEvent<T>(T @event) where T : IEvent;
    }
}
