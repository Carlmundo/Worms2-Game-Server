using System;
using System.Text;

namespace Syroot.Worms.IO
{
    /// <summary>
    /// Represents extension methods for <see cref="Encoding"/> instances.
    /// </summary>
    public static class EncodingExtensions
    {
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

#if NETSTANDARD2_0
        // ---- Backports ----

        public unsafe static int GetBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            fixed (byte* pBytes = bytes)
            fixed (char* pChars = chars)
                return encoding.GetBytes(pChars, chars.Length, pBytes, bytes.Length);
        }

        public unsafe static string GetString(this Encoding encoding, ReadOnlySpan<byte> bytes)
        {
            fixed (byte* pBytes = bytes)
                return encoding.GetString(pBytes, bytes.Length);
        }
#endif
    }
}
