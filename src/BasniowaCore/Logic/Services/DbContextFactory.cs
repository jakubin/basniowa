using System;
using Common;
using DataAccess.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logic.Services
{
    /// <summary>
    /// Implementation of <see cref="IDbContextFactory"/> using 
    /// <see cref="IServiceProvider"/>.
    /// </summary>
    public class DbContextFactory : IDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

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
        public TheaterDb Create(bool trackEntities = true)
        {
            var context = _serviceProvider.GetService<TheaterDb>();
            context.ChangeTracker.QueryTrackingBehavior =
                trackEntities ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
            return context;
        }
    }
}
