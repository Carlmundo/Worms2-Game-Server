#if NETSTANDARD2_0
namespace System.IO
{
    /// <summary>
    /// Represents extension methods for <see cref="Stream"/> instances.
    /// </summary>
    public static class StreamShims
    {
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the
        /// position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> instance to write with.</param>
        /// <param name="buffer">A region of memory. When this method returns, the contents of this region are replaced
        /// by the bytes read from the current source.</param>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes allocated
        /// in the buffer if that many bytes are not currently available, or zero (0) if the end of the stream has been
        /// reached.</returns>
        /// <remarks>This .NET Standard 2.0 backport requires a temporary copy.</remarks>
        public static int Read(this Stream stream, Span<byte> buffer)
        {
            byte[] bytes = new byte[buffer.Length];
            int bytesRead = stream.Read(bytes, 0, bytes.Length);
            bytes.AsSpan(0, bytesRead).CopyTo(buffer);
            return bytesRead;
        }

        /// <summary>
        /// When overridden in a derived class, writes a sequence of bytes to the current stream and advances the
        /// current position within this stream by the number of bytes written.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> instance.</param>
        /// <param name="value">A region of memory. This method copies the contents of this region to the current
        /// stream.</param>
        /// <remarks>This .NET Standard 2.0 backport requires a temporary copy.</remarks>
        public static void Write(this Stream stream, ReadOnlySpan<byte> value) => stream.Write(value.ToArray());
    }
}
#endif
