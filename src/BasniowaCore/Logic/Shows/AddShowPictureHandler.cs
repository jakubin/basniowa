using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Cqrs;
using Common.FileContainers;
using ImageMagick;
using Logic.Common;
using Logic.Common.BusinessRules;
using Logic.Common.Images;
using Logic.Services;
using Microsoft.EntityFrameworkCore;

namespace Logic.Shows
{
    /// <summary>
    /// Handles <see cref="AddShowPictureCommand"/>.
    /// </summary>
    public class AddShowPictureHandler : IHandler<AddShowPictureCommand>
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

        /// <summary>
        /// Gets or sets the unique identifier service.
        /// </summary>
        public IUniqueIdService IdService { get; set; }

        /// <summary>
        /// Gets or sets the file container for show pictures.
        /// </summary>
        public IFileContainer FileContainer { get; set; }

        #endregion

        /// <summary>
        /// The size (width or height) of thumb image.
        /// </summary>
        public const int ThumbSize = 200;

        /// <summary>
        /// The directory name containing thumbnail.
        /// </summary>
        public static readonly string ThumbDirectory = "thumb-" + ThumbSize;

        /// <summary>
        /// The directory name containing full image.
        /// </summary>
        public static readonly string FullDirectory = "full";

        /// <inheritdoc/>
        public async Task Handle(AddShowPictureCommand command)
        {
            var extension = Path.GetExtension(command.FileName);
            if (!ImageFileTypes.ImageFileExtensions.Contains(extension))
            {
                throw new BusinessRuleException<FileMustHaveImageExtensionRule>(
                    new FileMustHaveImageExtensionRule(extension, ImageFileTypes.ImageFileExtensions.ToArray()));
            }

            var fullImagePath = GenerateContainerPath(command.ShowId, command.ShowPictureId, command.FileName, false);
            var thumbImagePath = GenerateContainerPath(command.ShowId, command.ShowPictureId, command.FileName, true);
            await ProcessImage(command, fullImagePath, thumbImagePath);

            using (var db = DbFactory.Create())
            using (var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead))
            {
                var show = await db.Shows.FirstOrDefaultAsync(x => x.Id == command.ShowId);
                show.ThrowIfNull(command.ShowId.ToString());

                var showPicture = new DataAccess.Database.Shows.ShowPicture
                {
                    Id = command.ShowPictureId,
                    ShowId = command.ShowId,
                    ImagePath = fullImagePath,
                    ThumbPath = thumbImagePath,
                    Title = command.Title,
                    CreatedBy = command.UserName,
                    CreatedUtc = DateTimeService.UtcNow,
                    IsDeleted = false
                };
                db.ShowPictures.Add(showPicture);

                transaction.Commit();
            }

            var @event = new ShowPictureAdded {ShowId = command.ShowId, ShowPictureId = command.ShowPictureId};
            await EventPublisher.Publish(@event);
        }

        private async Task ProcessImage(AddShowPictureCommand command, string fullImagePath, string thumbImagePath)
        {
            var extension = Path.GetExtension(command.FileName);
            bool compressed = ImageFileTypes.CompressedImageFileExtensions.Contains(extension);

            using (var image = new MagickImage(command.FileStream))
            {
                image.Strip();

                // process full-size image
                using (var memoryStream = new MemoryStream())
                {
                    image.Write(memoryStream, compressed ? image.Format : MagickFormat.Jpg);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await FileContainer.AddFile(fullImagePath, memoryStream);
                }

                // process thumbnail
                var resizeFactor = (double) ThumbSize / Math.Max(image.Width, image.Height);
                image.Resize(new Percentage(resizeFactor * 100));
                using (var memoryStream = new MemoryStream())
                {
                    image.Write(memoryStream, compressed ? image.Format : MagickFormat.Jpg);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    await FileContainer.AddFile(thumbImagePath, memoryStream);
                }
            }
        }

        private static string GenerateContainerPath(long showId, long showPictureId, string fileName, bool isThumbnail)
        {
            fileName = FileContainerPath.Normalize(fileName);
            return FileContainerPath.Combine(
                showId.ToString(),
                showPictureId.ToString(),
                isThumbnail ? ThumbDirectory : FullDirectory,
                fileName);
        }
    }
}