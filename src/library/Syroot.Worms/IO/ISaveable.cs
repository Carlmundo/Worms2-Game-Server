using System.IO;

namespace Syroot.Worms.IO
{
    /// <summary>
    /// Represents data which can be saved by providing a <see cref="Stream "/>to write to.
    /// </summary>
    public interface ISaveable
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Saves the data into the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the data to.</param>
        void Save(Stream stream);
    }
}
