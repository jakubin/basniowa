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
        /// <returns>New database context</returns>
        TheaterDb Create();
    }
}