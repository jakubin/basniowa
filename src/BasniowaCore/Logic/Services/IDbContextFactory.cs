using DataAccess;

namespace Logic.Services
{
    /// <summary>
    /// Database context factory.
    /// </summary>
    public interface IDbContextFactory
    {
        /// <summary>
        /// Creates a new database context.
        /// It is caller responsibility to dispose the object.
        /// </summary>
        /// <param name="trackEntities">If set to <c>true</c> entities are tracked by the context.</param>
        /// <returns>
        /// New database context
        /// </returns>
        TheaterDb Create(bool trackEntities = true);
    }
}