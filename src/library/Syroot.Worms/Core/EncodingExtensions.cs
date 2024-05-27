using System;
using System.Text;

namespace Syroot.Worms.Core
{
    /// <summary>
    /// Represents extension methods for <see cref="Encoding"/> instances.
    /// </summary>
    public static class EncodingExtensions
    {
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        public static string GetZeroTerminatedString(this Encoding encoding, ReadOnlySpan<byte> bytes)
            => encoding.GetString(bytes.Slice(0, Math.Max(0, bytes.IndexOf((byte)0))));

        public static int GetZeroTerminatedBytes(this Encoding encoding, ReadOnlySpan<char> chars, Span<byte> bytes)
        {
            int length = encoding.GetBytes(chars, bytes);
            bytes[length] = 0;
            return ++length;
        }
    }
}
