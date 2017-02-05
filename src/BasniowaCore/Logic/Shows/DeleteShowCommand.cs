using Common.Cqrs;

namespace Logic.Shows
{
    /// <summary>
    /// Command to mark show as deleted.
    /// </summary>
    public sealed class DeleteShowCommand : ICommand
    {
        /// <summary>
        /// Gets the show identifier.
        /// </summary>
        public long ShowId { get; set; }

        /// <summary>
        /// Gets or sets the name of the user executing this command.
        /// </summary>
        public string UserName { get; set; }
    }
}