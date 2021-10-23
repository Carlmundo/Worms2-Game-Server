using System.IO;

namespace Syroot.Worms.IO
{
    /// <summary>
    /// Represents data which can be loaded by providing a <see cref="Stream"/> to read from.
    /// </summary>
    public interface ILoadable
    {
        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <summary>
        /// Loads the data from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        void Load(Stream stream);
    }
}
