using System;

namespace Syroot.Worms.Core.Riff
{
    /// <summary>
    /// Represents the attribute to decorate methods in <see cref="RiffFile"/> inheriting classes to provide methods
    /// saving specific chunks with their identifier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class RiffChunkSaveAttribute : Attribute
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="RiffFileAttribute"/> class with the given
        /// <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The chunk identifier saved in the file.</param>
        internal RiffChunkSaveAttribute(string identifier) => Identifier = identifier;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the chunk identifier saved in the file.
        /// </summary>
        internal string Identifier { get; }
    }
}
