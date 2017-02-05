using System.Threading.Tasks;
using Common.Cqrs;
using Logic.Common;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Logic.Shows
{
    /// <summary>
    /// Handler for <see cref="DeleteShowCommand"/>.
    /// </summary>
    public class DeleteShowHandler : IHandler<DeleteShowCommand>
    {
        /// <summary>
        /// Gets or sets the database context factory.
        /// </summary>
        public IDbContextFactory DbContextFactory { get; set; }

        /// <summary>
        /// Gets or sets the date time service.
        /// </summary>
        public IDateTimeService DateTimeService { get; set; }

        /// <summary>
        /// Gets or sets the event publisher.
        /// </summary>
        public IEventPublisher EventPublisher { get; set; }

        /// <inheritdoc/>
        public async Task Handle(DeleteShowCommand message)
        {
            using (var db = DbContextFactory.Create())
            {
                var show = await db.Shows.FirstOrDefaultAsync(x => x.Id == message.ShowId);
                show.ThrowIfNull(message.ShowId.ToString());

                if (show.IsDeleted)
                {
                    return;
                }

                show.IsDeleted = true;
                show.ModifiedBy = message.UserName;
                show.ModifiedUtc = DateTimeService.UtcNow;

                await db.SaveChangesAsync();
            }

            await PublishShowDeleted(message.ShowId);
        }

        private async Task PublishShowDeleted(long showId)
        {
            var @event = new ShowDeleted {ShowId = showId};
            await EventPublisher.Publish(@event);
        }
    }
}