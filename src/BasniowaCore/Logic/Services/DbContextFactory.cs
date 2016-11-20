using System;
using Common;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Services
{
    /// <summary>
    /// Implementation of <see cref="IDbContextFactory"/> using 
    /// <see cref="IServiceProvider"/>.
    /// </summary>
    public class DbContextFactory : IDbContextFactory
    {
        private IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextFactory" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public DbContextFactory(IServiceProvider serviceProvider)
        {
            Guard.NotNull(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public TheaterDb Create()
        {
            return _serviceProvider.GetService<TheaterDb>();
        }
    }
}
