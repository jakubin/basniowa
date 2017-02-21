using System;
using DataAccess.Database;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Logic.Tests.Helpers
{
    /// <summary>
    /// In-memory DB helper.
    /// </summary>
    public sealed class InMemoryDb : IDbContextFactory, IDisposable
    {
        private volatile bool _wasCreated = false;

        /// <summary>
        /// Gets the name of the unique name for this in-memory database.
        /// </summary>
        public string DbName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDb"/> class.
        /// </summary>
        public InMemoryDb()
            : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDb"/> class.
        /// </summary>
        /// <param name="dbName">Name of the database.</param>
        public InMemoryDb(string dbName)
        {
            DbName = dbName;
        }

        /// <inheritdoc/>
        public TheaterDb Create(bool trackEntities = true)
        {
            _wasCreated = true;

            var builder = new DbContextOptionsBuilder<TheaterDb>()
                .UseInMemoryDatabase(DbName);

            var context = new TheaterDb(builder.Options);
            context.ChangeTracker.QueryTrackingBehavior =
                trackEntities ? QueryTrackingBehavior.TrackAll : QueryTrackingBehavior.NoTracking;
            return context;
        }

        /// <summary>
        /// Deletes in-memory DB if it was created.
        /// </summary>
        public void Dispose()
        {
            if (!_wasCreated)
            {
                return;
            }

            var db = Create();
            db.Database.EnsureDeleted();
            _wasCreated = false;
        }
    }
}