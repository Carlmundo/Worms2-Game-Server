using System;

namespace Syroot.Worms.Core.Riff
{
    /// <summary>
    /// Represents the attribute to decorate methods in <see cref="RiffFile"/> inheriting classes to provide methods
    /// loading specific chunks according to their identifier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class RiffChunkLoadAttribute : Attribute
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="RiffFileAttribute"/> class with the given
        /// <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The chunk identifier required to invoke this method for loading it.</param>
        internal RiffChunkLoadAttribute(string identifier) => Identifier = identifier;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the chunk identifier required to invoke this method for loading it.
        /// </summary>
        internal string Identifier { get; }
    }
}
