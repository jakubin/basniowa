using System.Threading.Tasks;
using Common.Cqrs;
using Logic.Common;
using Logic.Common.BusinessRules;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Logic.Shows
{
    /// <summary>
    /// Handles <see cref="SetShowMainPictureCommand"/>.
    /// </summary>
    public class SetShowMainPictureHandler : IHandler<SetShowMainPictureCommand>
    {
        #region Services

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

        #endregion

        /// <inheritdoc/>
        public async Task Handle(SetShowMainPictureCommand command)
        {
            using (var db = DbFactory.Create())
            {
                var show = await db.Shows.FirstOrDefaultAsync(x => x.Id == command.ShowId && !x.IsDeleted);
                show.ThrowIfNull(command.ShowId.ToString());

                if (show.MainShowPictureId == command.ShowPictureId)
                {
                    return;
                }

                if (command.ShowPictureId != null)
                {
                    var showPicture = await db.ShowPictures
                        .FirstOrDefaultAsync(x => x.Id == command.ShowPictureId && !x.IsDeleted);
                    showPicture.ThrowIfNull(command.ShowPictureId.ToString());

                    if (showPicture.ShowId != show.Id)
                    {
                        var rule = new ShowMainPictureMustBelongToShowRule(showPicture.Id, show.Id, showPicture.ShowId);
                        throw new BusinessRuleException<ShowMainPictureMustBelongToShowRule>(rule);
                    }
                }

                show.MainShowPictureId = command.ShowPictureId;
                show.ModifiedBy = command.UserName;
                show.ModifiedUtc = DateTimeService.UtcNow;
                await db.SaveChangesAsync();
            }

            var @event = new ShowMainPictureSetEvent
            {
                ShowId = command.ShowId,
                ShowPictureId = command.ShowPictureId
            };
            await EventPublisher.Publish(@event);
        }
    }
}