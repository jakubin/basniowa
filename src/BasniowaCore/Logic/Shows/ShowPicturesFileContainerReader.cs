using Common.FileContainers;

namespace Logic.Shows
{
    /// <summary>
    /// Wrapper for <see cref="IFileContainerReader"/>, which accesses show pictures.
    /// </summary>
    public class ShowPicturesFileContainerReader
    {
        /// <summary>
        /// Gets the file container.
        /// </summary>
        public IFileContainerReader FileContainer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowPicturesFileContainerReader"/> class.
        /// </summary>
        /// <param name="fileContainer">The file container.</param>
        public ShowPicturesFileContainerReader(IFileContainerReader fileContainer)
        {
            FileContainer = fileContainer;
        }
    }
}