using System.Threading.Tasks;

namespace Common.Cqrs
{
    /// <summary>
    /// Delegate executing next processor in message processor chain 
    /// or, finally, the actual message handler.
    /// </summary>
    /// <param name="context">The context of current execution.</param>
    /// <returns>Task representing the async operation.</returns>
    public delegate Task MessageHandlerDelegate(MessageProcessingContext context);
}
