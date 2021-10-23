using System.Text;

namespace Syroot.Worms.Worms2.GameServer
{
    /// <summary>
    /// Represents additional code pages.
    /// </summary>
    internal static class Encodings
    {
        // ---- FIELDS -------------------------------------------------------------------------------------------------

        /// <summary>Windows-1252 encoding (codepage 1252).</summary>
        internal static readonly Encoding Windows1252;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes static members of the <see cref="Encodings"/> class.
        /// </summary>
        static Encodings()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Windows1252 = Encoding.GetEncoding(1252);
        }
    }
}
