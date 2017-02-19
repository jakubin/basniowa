using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Cqrs;
using Logic.Common.BusinessRules;
using Logic.Common.Images;
using Logic.Services;

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

        #endregion

        /// <inheritdoc/>
        public async Task Handle(AddShowPictureCommand command)
        {
            var extension = Path.GetExtension(command.FileName);
            if (!ImageFileTypes.ImageFileExtensions.Contains(extension))
            {
                throw new BusinessRuleException<FileMustBeImageRule>(
                    new FileMustBeImageRule(extension, ImageFileTypes.ImageFileExtensions.ToArray()));
            }

            throw new NotImplementedException();
        }
    }
}