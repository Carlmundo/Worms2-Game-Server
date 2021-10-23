using System.Collections.Generic;
using System.Drawing;

namespace Syroot.Worms.Graphics
{
    /// <summary>
    /// Represents an interface for any class storing indexed image palette colors.
    /// </summary>
    public interface IPalette
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the <see cref="Color"/> values stored by this palette.
        /// </summary>
        IList<Color> Colors { get; set; }
    }
}
