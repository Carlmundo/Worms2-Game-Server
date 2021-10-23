using System;

namespace Syroot.Worms.Core.Riff
{
    /// <summary>
    /// Represents the attribute to decorate <see cref="RiffFile"/> inheriting classes to provide their file identifier.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    internal class RiffFileAttribute : Attribute
    {
        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="RiffFileAttribute"/> class with the given
        /// <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The file identifier in the RIFF file header which will be validated.</param>
        internal RiffFileAttribute(string identifier) => Identifier = identifier;

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the file identifier in the RIFF file header which will be validated.
        /// </summary>
        internal string Identifier { get; }
    }
}
