using Common.FileContainers;

namespace Logic.Shows
{
    /// <summary>
    /// Wrapper for <see cref="IFileContainer"/>, which accesses show pictures.
    /// </summary>
    public class ShowPicturesFileContainer
    {
        /// <summary>
        /// Gets the file container.
        /// </summary>
        public IFileContainer FileContainer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowPicturesFileContainer"/> class.
        /// </summary>
        /// <param name="fileContainer">The file container.</param>
        public ShowPicturesFileContainer(IFileContainer fileContainer)
        {
            FileContainer = fileContainer;
        }
    }
}