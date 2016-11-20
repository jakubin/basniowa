using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Helpers
{
    /// <summary>
    /// Helper for creating an in-memory database.
    /// </summary>
    public static class InMemoryDbHelper
    {
        /// <summary>
        /// Creates <see cref="DbContextOptions{TContext}"/> object for in-memory database.
        /// </summary>
        /// <typeparam name="TContext">Type of DB context.</typeparam>
        /// <returns>DB context options.</returns>
        public static DbContextOptions<TContext> CreateOptions<TContext>() where TContext : DbContext
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider);

            return optionsBuilder.Options;
        }
    }
}
