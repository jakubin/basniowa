using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Cqrs;
using DataAccess.Shows;
using Logic.Common;
using Logic.Common.Helpers;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Logic.Shows
{
    /// <summary>
    /// Handles <see cref="UpdateShowCommand"/>.
    /// </summary>
    public class UpdateShowHandler : IHandler<UpdateShowCommand>
    {
        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        public IDbContextFactory DbFactory { get; set; }

        /// <summary>
        /// Gets or sets the event publisher.
        /// </summary>
        public IEventPublisher EventPublisher { get; set; }

        /// <summary>
        /// Gets or sets the date time service.
        /// </summary>
        public IDateTimeService DateTimeService { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier service.
        /// </summary>
        public IUniqueIdService IdService { get; set; }

        /// <inheritdoc/>
        public async Task Handle(UpdateShowCommand message)
        {
            using (var db = DbFactory.Create())
            {
                var utcNow = DateTimeService.UtcNow;

                var show = await db.Shows
                    .Include(x => x.ShowProperties)
                    .Where(x => !x.IsDeleted)
                    .FirstOrDefaultAsync(x => x.Id == message.ShowId);

                show.ThrowIfNull(message.ShowId.ToString());

                if (show.IsDeleted)
                {
                    throw new EntityNotFoundException<Show>(message.ShowId.ToString());
                }

                show.Title = message.Title;
                show.Subtitle = message.Subtitle;
                show.Description = message.Description;
                show.ModifiedUtc = utcNow;
                show.ModifiedBy = message.UserName;

                await UpdateProperties(message, show);

                await db.SaveChangesAsync();
            }

            await EventPublisher.Publish(new ShowUpdated {ShowId = message.ShowId});
        }

        private async Task UpdateProperties(UpdateShowCommand message, Show show)
        {
            var targetProperties = message.Properties.Select(x => new ShowProperty {Name = x.Key, Value = x.Value});
            var operations = show.ShowProperties.GetRequiredOperations(targetProperties, x => x.Name);

            foreach (var property in operations.ToUpdate)
            {
                property.Source.Value = property.Target.Value;
                property.Source.IsDeleted = false;
            }

            foreach (var property in operations.ToRemove)
            {
                property.IsDeleted = true;
            }

            var ids = new Queue<long>(await IdService.GenerateIds(operations.ToAdd.Count));
            foreach (var property in operations.ToAdd)
            {
                property.Id = ids.Dequeue();
                property.IsDeleted = false;
                show.ShowProperties.Add(property);
            }
        }
    }
}
