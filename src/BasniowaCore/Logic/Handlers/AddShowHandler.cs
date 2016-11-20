using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Cqrs;
using DataAccess.Shows;
using Logic.Commands;
using Logic.Events;
using Logic.Services;

namespace Logic.Handlers
{
    /// <summary>
    /// Handles <see cref="AddShowCommand"/>.
    /// </summary>
    public class AddShowHandler : IHandler<AddShowCommand>
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
        public async Task Handle(AddShowCommand message)
        {
            var utcNow = DateTimeService.UtcNow;
            var show = new Show()
            {
                Id = message.Id,
                Title = message.Title,
                Subtitle = message.Subtitle,
                Description = message.Description,
                CreatedUtc = utcNow,
                CreatedBy = message.UserName,
                ModifiedUtc = utcNow,
                ModifiedBy = message.UserName,
                IsDeleted = false
            };

            var propertyIds = new Queue<long>(await IdService.GenerateIds(message.Properties.Count));
            var properties = message.Properties
                .Select(x => new ShowProperty
                {
                    Id = propertyIds.Dequeue(),
                    Name = x.Key,
                    Value = x.Value,
                    Show = show
                })
                .ToList();

            using (var db = DbFactory.Create())
            {
                db.Add(show);
                db.AddRange(properties);

                await db.SaveChangesAsync();
            }

            await EventPublisher.PublishEvent(new ShowCreated { Id = show.Id });
        }
    }
}
