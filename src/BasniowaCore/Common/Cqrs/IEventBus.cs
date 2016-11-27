namespace Common.Cqrs
{
    /// <summary>
    /// Service allowing to publish events, which are processed by an extendable chain of processors.
    /// </summary>
    /// <seealso cref="Common.Cqrs.IEventPublisher" />
    public interface IEventBus : IEventPublisher
    {
        /// <summary>
        /// Adds the event publication processor to event publication chain.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <remarks>
        /// This chain is executed when an event is raised, but not dispatched yet to handlers.
        /// This step is meant for activities associated with publication of event rather than handling it, e.g.
        /// validation, authorization, etc.
        /// All exceptions raised by processors at this point will surface the callers. 
        /// </remarks>
        void AddEventPublicationProcessor(IMessageProcessor processor);

        /// <summary>
        /// Adds the event handler processor.
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <remarks>
        /// This chain is executed when an event is handled by a specific handler - 
        /// there is a separate invocation for each handler.
        /// This step is meant for activities associated with handling of event, e.g. logging,
        /// All exceptions raised by processors at this point will not surface the callers. 
        /// </remarks>
        void AddEventHandlerProcessor(IMessageProcessor processor);
    }
}