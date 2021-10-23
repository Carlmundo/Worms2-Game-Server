namespace Syroot.Worms.IO
{
    /// <summary>
    /// Represents a file which can be saved by providing a file name.
    /// </summary>
    public interface ISaveableFile : ISaveable
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Saves the data in the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to save the data in.</param>
        void Save(string fileName);
    }
}
