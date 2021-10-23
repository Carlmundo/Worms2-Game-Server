using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Syroot.Worms
{
    /// <summary>
    /// Represents a pixel-based 2D image in different color formats.
    /// </summary>
    public class RawBitmap
    {
        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the number of bits required to describe a color per pixel.
        /// </summary>
        public byte BitsPerPixel { get; set; }

        /// <summary>
        /// Gets or sets the colors in the palette of the bitmap, if it has one.
        /// </summary>
        public IList<Color>? Palette { get; set; }

        /// <summary>
        /// Gets or sets the size of the image in pixels.
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Gets or sets the data of the image pixels.
        /// </summary>
        public byte[] Data { get; set; } = Array.Empty<byte>();

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Creates a <see cref="Bitmap"/> from the raw data.
        /// </summary>
        /// <returns>The <see cref="Bitmap"/> created from the raw data.</returns>
        public unsafe Bitmap ToBitmap()
        {
            // Create bitmap with appropriate pixel format.
            PixelFormat pixelFormat = BitsPerPixel switch
            {
                1 => PixelFormat.Format1bppIndexed,
                8 => PixelFormat.Format8bppIndexed,
                32 => PixelFormat.Format32bppRgb,
                _ => throw new NotSupportedException($"Cannot convert to {BitsPerPixel}bpp bitmap.")
            };
            Bitmap bitmap = new Bitmap(Size.Width, Size.Height, pixelFormat);

            // Transfer the pixel data, respecting power-of-2 strides.
            BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, Size.Width, Size.Height),
                ImageLockMode.WriteOnly, pixelFormat);
            float bytesPerPixel = BitsPerPixel / 8f;
            Span<byte> bitmapSpan = new Span<byte>(bitmapData.Scan0.ToPointer(), bitmapData.Stride * bitmapData.Height);
            for (int y = 0; y < Size.Height; y++)
            {
                Data.AsSpan((int)(y * Size.Width * bytesPerPixel), (int)(Size.Width * bytesPerPixel))
                    .CopyTo(bitmapSpan.Slice((int)(y * bitmapData.Stride * bytesPerPixel)));
            }
            bitmap.UnlockBits(bitmapData);

            // Transfer any palette.
            switch (pixelFormat)
            {
                case PixelFormat.Format1bppIndexed:
                    ColorPalette palette1bpp = bitmap.Palette;
                    palette1bpp.Entries[0] = Color.Black;
                    palette1bpp.Entries[1] = Color.White;
                    bitmap.Palette = palette1bpp;
                    break;
                case PixelFormat.Format8bppIndexed:
                    ColorPalette palette8bpp = bitmap.Palette;
                    for (int i = 0; i < Palette!.Count; i++)
                        palette8bpp.Entries[i] = Palette[i];
                    bitmap.Palette = palette8bpp;
                    break;
            }

            return bitmap;
        }
    }
}
