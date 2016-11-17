using System.Collections.Generic;

namespace Common.Cqrs
{
    /// <summary>
    /// Represents the context of execution of current command and events published as a result of it.
    /// </summary>
    public interface ICommandContext : IReadOnlyDictionary<string, string>
    {
    }
}
