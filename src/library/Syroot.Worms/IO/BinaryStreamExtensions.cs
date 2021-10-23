using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Syroot.BinaryData;

namespace Syroot.Worms.IO
{
    /// <summary>
    /// Represents extension methods for <see cref="BinaryStream"/> instances.
    /// </summary>
    public static partial class BinaryStreamExtensions
    {
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        // ---- Reading ----

        /// <summary>
        /// Reads an RGBA 32-bit color.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryStream"/> instance.</param>
        /// <returns>The read color.</returns>
        public static Color ReadColor(this BinaryStream self) => Color.FromArgb(self.ReadInt32());

        /// <summary>
        /// Reads <paramref name="count"/> RGBA 32-bit colors.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryStream"/> instance.</param>
        /// <param name="count">The number of values to read.</param>
        /// <returns>The read colors.</returns>
        public static Color[] ReadColors(this BinaryStream self, int count)
        {
            Color[] values = new Color[count];
            for (int i = 0; i < count; i++)
                values[i] = Color.FromArgb(self.ReadInt32());
            return values;
        }

        /// <summary>
        /// Returns the current position of the stream at which a 4-byte placeholder has been written which can be
        /// filled later with <see cref="SatisfyOffset"/>.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryStream"/> instance.</param>
        /// <returns>The position at which a 4-byte placeholder has been written to.</returns>
        public static uint ReserveOffset(this BinaryStream self)
        {
            uint offset = (uint)self.Position;
            self.WriteUInt32(0);
            return offset;
        }

        // ---- Writing ----

        /// <summary>
        /// Writes the given <paramref name="value"/> to the given <paramref name="offset"/>. This is meant to be used
        /// in combination with <see cref="ReserveOffset"/>.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryStream"/> instance.</param>
        /// <param name="offset">The position at which to write the value.</param>
        /// <param name="value">The value to write.</param>
        public static void SatisfyOffset(this BinaryStream self, uint offset, uint value)
        {
            using var _ = self.TemporarySeek(offset, SeekOrigin.Begin);
            self.WriteUInt32(value);
        }

        /// <summary>
        /// Writes the given color.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryStream"/> instance.</param>
        /// <param name="color">The color to write.</param>
        public static void WriteColor(this BinaryStream self, Color color)
            => self.Write(color.A << 24 | color.R << 16 | color.G << 8 | color.B);

        /// <summary>
        /// Writes the given colors.
        /// </summary>
        /// <param name="self">The extended <see cref="BinaryStream"/> instance.</param>
        /// <param name="colors">The colors to write.</param>
        public static void WriteColors(this BinaryStream self, IEnumerable<Color> colors)
        {
            foreach (Color color in colors)
                WriteColor(self, color);
        }
    }
}

