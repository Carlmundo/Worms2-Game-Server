using System;
using System.IO;

namespace Syroot.Worms.Graphics
{
    /// <summary>
    /// Represents methods to decompress data which is compressed using Team17's internal compression algorithm for
    /// graphic file formats.
    /// S. http://worms2d.info/Team17_compression.
    /// </summary>
    internal static class Team17Compression
    {
        // ---- METHODS (INTERNAL) -------------------------------------------------------------------------------------

        /// <summary>
        /// Returns the data available in <paramref name="bytes"/> in compressed format.
        /// </summary>
        /// <param name="bytes">The data to compress.</param>
        /// <returns>The compressed data.</returns>
        internal static byte[] Compress(ReadOnlySpan<byte> bytes)
            => throw new NotImplementedException("Compressing data has not been implemented yet.");

        /// <summary>
        /// Decompresses the data available in the given <paramref name="stream"/> into the provided
        /// <paramref name="buffer"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read the data from.</param>
        /// <param name="buffer">The byte buffer to write the decompressed data to.</param>
        internal static void Decompress(Stream stream, Span<byte> buffer)
        {
            int output = 0; // Offset of next write.
            int cmd;
            while ((cmd = stream.ReadByte()) != -1)
            {
                // Read a byte.
                if ((cmd & 0x80) == 0)
                {
                    // Command: 1 byte (color)
                    buffer[output++] = (byte)cmd;
                }
                else
                {
                    int arg1 = cmd >> 3 & 0b1111; // bits 2-5
                    int arg2 = stream.ReadByte();
                    if (arg2 == -1)
                        return;
                    // Arg2 = bits 6-16
                    arg2 = ((cmd << 8) | arg2) & 0x7FF;
                    if (arg1 == 0)
                    {
                        // Command: 0x80 0x00
                        if (arg2 == 0)
                            return;
                        int arg3 = stream.ReadByte();
                        if (arg3 == -1)
                            return;
                        // Command: 3 bytes
                        output = CopyData(output, arg2, arg3 + 18, buffer);
                    }
                    else
                    {
                        // Command: 2 bytes
                        output = CopyData(output, arg2 + 1, arg1 + 2, buffer);
                    }
                }
            }
        }

        // Own Span reimplementation, has the same issues with some out-of-bounds-access maps. Supports color remap.
        //internal static int Decompress(ReadOnlySpan<byte> src, Span<byte> dst, ReadOnlySpan<byte> colorMap)
        //{
        //    int srcPos = 0;
        //    int dstPos = 0;
        //    int count, seek;
        //
        //    while (true)
        //    {
        //        while (true)
        //        {
        //            while ((src[srcPos] & 0b1000_0000) == 0)
        //                dst[dstPos++] = colorMap[src[srcPos++]];
        //
        //            if ((src[srcPos] & 0b0111_1000) == 0)
        //                break;
        //
        //            count = (src[srcPos] >> 3 & 0b0000_1111) + 2;
        //            seek = (src[srcPos + 1] | ((src[srcPos] & 0b0000_0111) << 8)) + 1;
        //            while (count-- > 0)
        //                dst[dstPos] = dst[dstPos++ - seek];
        //            srcPos += 2;
        //        }
        //
        //        seek = src[srcPos + 1] | (src[srcPos] & 0b0000_0111) << 8;
        //        if (seek == 0)
        //            break;
        //        count = src[srcPos + 2] + 18;
        //        while (count-- > 0)
        //            dst[dstPos] = dst[dstPos++ - seek];
        //
        //        srcPos += 3;
        //    }
        //
        //    return dstPos;
        //}

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private static int CopyData(int offset, int compressedOffset, int count, Span<byte> buffer)
        {
            for (; count > 0; count--)
                buffer[offset] = buffer[offset++ - compressedOffset];
            return offset;
        }
    }
}
