using System;
using System.Data;
using System.Threading.Tasks;
using Common.Cqrs;
using Common.FileContainers;
using DataAccess.Database.Shows;
using Logic.Common;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Logic.Shows
{
    /// <summary>
    /// Handles <see cref="DeleteShowPictureCommand"/>.
    /// </summary>
    public class DeleteShowPictureHandler : IHandler<DeleteShowPictureCommand>
    {
        #region Injected dependencies

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
        /// Gets or sets the file container for show pictures.
        /// </summary>
        public ShowPicturesFileContainer ShowPictures { get; set; }

        #endregion

        /// <inheritdoc/>
        public async Task Handle(DeleteShowPictureCommand command)
        {
            ShowPicture showPicture;
            ShowMainPictureSetEvent showMainPictureSetEvent = null;
            using (var db = DbFactory.Create())
            using (var transaction = db.Database.BeginTransaction(IsolationLevel.RepeatableRead))
            {
                showPicture = await db.ShowPictures
                    .Include(x => x.Show)
                    .FirstOrDefaultAsync(x => x.Id == command.ShowPictureId);
                showPicture.ThrowIfNull(command.ShowPictureId.ToString());

                if (showPicture.IsDeleted)
                {
                    return;
                }

                var now = DateTimeService.UtcNow;

                if (showPicture.Show.MainShowPictureId == showPicture.Id)
                {
                    showPicture.Show.MainShowPictureId = null;
                    showPicture.Show.ModifiedUtc = now;
                    showPicture.Show.ModifiedBy = command.UserName;
                    showMainPictureSetEvent = new ShowMainPictureSetEvent
                    {
                        ShowId = showPicture.ShowId,
                        ShowPictureId = null
                    };
                }

                showPicture.IsDeleted = true;
                showPicture.ModifiedUtc = DateTimeService.UtcNow;
                showPicture.ModifiedBy = command.UserName;
                await db.SaveChangesAsync();
                transaction.Commit();
            }

            await DeletePicture(showPicture.ImagePath);
            await DeletePicture(showPicture.ThumbPath);

            if (showMainPictureSetEvent != null)
            {
                await EventPublisher.Publish(showMainPictureSetEvent);
            }

            var @event = new ShowPictureDeletedEvent
            {
                ShowId = showPicture.ShowId,
                ShowPictureId = showPicture.Id
            };
            await EventPublisher.Publish(@event);
        }

        private async Task DeletePicture(string path)
        {
            if (path == null)
            {
                return;
            }

            try
            {
                await ShowPictures.FileContainer.RemoveFile(path);
            }
            catch (FileNotFoundInContainerException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                // TODO: log
            }
        }
    }
}