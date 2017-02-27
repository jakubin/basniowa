using Logic.Common.BusinessRules;

namespace Logic.Shows
{
    /// <summary>
    /// Business rule making sure that main show picture belongs to the show.
    /// </summary>
    public sealed class ShowMainPictureMustBelongToShowRule : IBusinessRule
    {
        /// <inheritdoc/>
        public string RuleId => "Shows.ShowMainPictureMustBelongToShow";

        /// <summary>
        /// Gets the show picture identifier.
        /// </summary>
        public long ShowPictureId { get; }

        /// <summary>
        /// Gets or sets the specified show identifier to set the picture for.
        /// </summary>
        public long ShowId { get; set; }

        /// <summary>
        /// Gets the actual show identifier that the picture belongs to.
        /// </summary>
        public long ActualShowId { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowMainPictureMustBelongToShowRule"/> class.
        /// </summary>
        /// <param name="showPictureId">The show picture identifier.</param>
        /// <param name="showId">The show identifier.</param>
        /// <param name="actualShowId">The actual show identifier.</param>
        public ShowMainPictureMustBelongToShowRule(long showPictureId, long showId, long actualShowId)
        {
            ShowPictureId = showPictureId;
            ShowId = showId;
            ActualShowId = actualShowId;
        }

        /// <inheritdoc/>
        public string GetUserMessage()
        {
            return
                $"Specified show picture ID={ShowPictureId} cannot be the main show picture for show ID={ShowId} as it belongs to show ID={ActualShowId}.";
        }
    }
}