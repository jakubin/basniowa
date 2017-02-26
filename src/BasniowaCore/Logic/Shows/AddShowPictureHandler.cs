using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Cqrs;
using Common.FileContainers;
using DataAccess.Database.Shows;
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
        /// Gets or sets the unique identifier service.
        /// </summary>
        public IUniqueIdService IdService { get; set; }

        /// <summary>
        /// Gets or sets the file container for show pictures.
        /// </summary>
        public ShowPicturesFileContainer ShowPictures { get; set; }

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

        private string _fullImagePath;

        private string _thumbImagePath;

        /// <inheritdoc/>
        public async Task Handle(AddShowPictureCommand command)
        {
            var extension = Path.GetExtension(command.FileName);
            if (!ImageFileTypes.ImageFileExtensions.Contains(extension))
            {
                throw new BusinessRuleException<FileMustHaveImageExtensionRule>(
                    new FileMustHaveImageExtensionRule(extension, ImageFileTypes.ImageFileExtensions.ToArray()));
            }

            try
            {
                await ProcessAndSaveImage(command);

                await AddShowPictureToDatabase(command);
            }
            catch
            {
                await RemoveImages();
                throw;
            }

            var @event = new ShowPictureAddedEvent {ShowId = command.ShowId, ShowPictureId = command.ShowPictureId};
            await EventPublisher.Publish(@event);
        }

        private async Task ProcessAndSaveImage(AddShowPictureCommand command)
        {
            var fileName = command.FileName;
            var extension = Path.GetExtension(fileName);
            bool compressed = ImageFileTypes.CompressedImageFileExtensions.Contains(extension);

            if (!compressed)
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + ImageFileTypes.Jpg;
            }

            _fullImagePath = GenerateContainerPath(command.ShowId, command.ShowPictureId, fileName, false);
            _thumbImagePath = GenerateContainerPath(command.ShowId, command.ShowPictureId, fileName, true);

            try
            {
                using (var image = new MagickImage(command.FileStream))
                {
                    image.Strip();

                    // process full-size image
                    using (var memoryStream = new MemoryStream())
                    {
                        image.Write(memoryStream, compressed ? image.Format : MagickFormat.Jpg);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await ShowPictures.FileContainer.AddFile(_fullImagePath, memoryStream);
                    }

                    // process thumbnail
                    var resizeFactor = (double)ThumbSize / Math.Max(image.Width, image.Height);
                    image.Resize(new Percentage(resizeFactor * 100));
                    using (var memoryStream = new MemoryStream())
                    {
                        image.Write(memoryStream, compressed ? image.Format : MagickFormat.Jpg);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await ShowPictures.FileContainer.AddFile(_thumbImagePath, memoryStream);
                    }
                }
            }
            catch (MagickException ex)
            {
                var businessRule = new FileMustBeImageRule();
                throw new BusinessRuleException<FileMustBeImageRule>(businessRule, ex);
            }
        }

        private async Task AddShowPictureToDatabase(AddShowPictureCommand command)
        {
            using (var db = DbFactory.Create())
            using (var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.RepeatableRead))
            {
                var show = await db.Shows.FirstOrDefaultAsync(x => x.Id == command.ShowId);
                if (show == null || show.IsDeleted)
                {
                    throw new EntityNotFoundException<Show>(command.ShowId.ToString());
                }

                var now = DateTimeService.UtcNow;

                var showPicture = new ShowPicture
                {
                    Id = command.ShowPictureId,
                    ShowId = command.ShowId,
                    ImagePath = _fullImagePath,
                    ThumbPath = _thumbImagePath,
                    Title = command.Title,
                    CreatedBy = command.UserName,
                    CreatedUtc = now,
                    ModifiedBy = command.UserName,
                    ModifiedUtc = now,
                    IsDeleted = false
                };
                db.ShowPictures.Add(showPicture);
                await db.SaveChangesAsync();

                transaction.Commit();
            }
        }

        private async Task RemoveImages()
        {
            var files = new[] {_fullImagePath, _thumbImagePath};

            foreach (var filePath in files.Where(x => x != null))
            {
                try
                {
                    await ShowPictures.FileContainer.RemoveFile(filePath);
                }
                catch
                {
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