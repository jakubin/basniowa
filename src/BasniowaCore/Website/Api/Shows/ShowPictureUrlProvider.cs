using Website.Infrastructure.FileContainers;

namespace Website.Api.Shows
{
    /// <summary>
    /// The wrapper for <see cref="IFileContainerUrlProvider"/> for show pictures.
    /// </summary>
    public class ShowPictureUrlProvider
    {
        /// <summary>
        /// Gets the URL provider.
        /// </summary>
        public IFileContainerUrlProvider UrlProvider { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowPictureUrlProvider"/> class.
        /// </summary>
        /// <param name="urlProvider">The URL provider.</param>
        public ShowPictureUrlProvider(IFileContainerUrlProvider urlProvider)
        {
            UrlProvider = urlProvider;
        }
    }
}