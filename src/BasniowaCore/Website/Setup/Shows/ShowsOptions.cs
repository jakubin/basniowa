namespace Website.Setup.Shows
{
    /// <summary>
    /// Options for shows module.
    /// </summary>
    public class ShowsOptions
    {
        /// <summary>
        /// Gets or sets the show picture container root path.
        /// This path can be either absolute (rooted) or relative to content root path.
        /// </summary>
        public string ShowPictureContainerRoot { get; set; }
    }
}