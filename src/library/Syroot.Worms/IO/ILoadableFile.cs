namespace Syroot.Worms.IO
{
    /// <summary>
    /// Represents a file which can be loaded by providing a file name.
    /// </summary>
    public interface ILoadableFile : ILoadable
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Loads the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        void Load(string fileName);
    }
}
