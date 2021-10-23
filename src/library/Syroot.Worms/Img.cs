using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Worms.Graphics;
using Syroot.Worms.IO;

namespace Syroot.Worms
{
    /// <summary>
    /// Represents a (palletized) graphical image stored in an IMG file, possibly compressed.
    /// Used by W2, WA and WWP. S. https://worms2d.info/Image_file.
    /// </summary>
    public class Img : RawBitmap, ILoadableFile, ISaveableFile
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        /// <summary>Magic value identifying the start of IMG data.</summary>
        public const uint Signature = 0x1A474D49; // "IMG\x1A"

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Img"/> class.
        /// </summary>
        public Img() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Img"/> class, loading the data from the given
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        public Img(Stream stream) => Load(stream);

        /// <summary>
        /// Initializes a new instance of the <see cref="Img"/> class, loading the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public Img(string fileName) => Load(fileName);

        /// <summary>
        /// Initializes a new instance of the <see cref="Img"/> class, loading the data from the given
        /// <see cref="Stream"/>. The data block can be aligned to a 4-bte boundary.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        /// <param name="alignData"><see langword="true"/> to align the data array by 4 bytes.</param>
        public Img(Stream stream, bool alignData) => Load(stream, alignData);

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets a value indicating data was compressed or should be compressed when saving.
        /// </summary>
        public bool Compressed { get; private set; }

        /// <summary>
        /// Gets an optional description of the image contents.
        /// </summary>
        public string? Description { get; private set; }

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Loads the data from the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        /// <exception cref="IndexOutOfRangeException">Compressed images required more bytes than usually to decompress.
        /// This error can be ignored, the image has been completely read, and is only caused by a few files.</exception>
        public void Load(Stream stream) => Load(stream, false);

        /// <summary>
        /// Loads the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        /// <exception cref="IndexOutOfRangeException">Compressed images required more bytes than usually to decompress.
        /// This error can be ignored, the image has been completely read, and is only caused by a few files.</exception>
        public void Load(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Load(stream);
        }

        /// <summary>
        /// Loads the data from the given <see cref="Stream"/>. The data block can be aligned to a 4-bte boundary.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        /// <param name="alignData"><see langword="true"/> to align the data array by 4 bytes.</param>
        /// <exception cref="IndexOutOfRangeException">Compressed images required more bytes than usually to decompress.
        /// This error can be ignored, the image has been completely read, and is only caused by a few files.</exception>
        public void Load(Stream stream, bool alignData)
        {
            using BinaryStream reader = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);

            // Read the header.
            if (reader.ReadUInt32() != Signature)
                throw new InvalidDataException("Invalid IMG file signature.");
            int fileSize = reader.ReadInt32();

            // Read an optional string describing the image contents and the bits per pixel.
            BitsPerPixel = reader.Read1Byte();
            if (BitsPerPixel == 0)
            {
                Description = String.Empty;
                BitsPerPixel = reader.Read1Byte();
            }
            else if (BitsPerPixel > 32)
            {
                Description = (char)BitsPerPixel + reader.ReadString(StringCoding.ZeroTerminated);
                BitsPerPixel = reader.Read1Byte();
            }
            else
            {
                Description = null;
            }

            // Read image flags describing the format and availability of the following contents.
            Flags flags = (Flags)reader.ReadByte();

            // Read the image palette if available. The first color of the palette is implicitly black.
            if (flags.HasFlag(Flags.Palettized))
            {
                int colorCount = reader.ReadInt16();
                Palette = new List<Color>(colorCount + 1);
                Palette.Add(Color.Black);
                while (colorCount-- > 0)
                    Palette.Add(Color.FromArgb(reader.Read1Byte(), reader.Read1Byte(), reader.Read1Byte()));
            }
            else
            {
                Palette = null;
            }

            // Read the image size.
            Size = new Size(reader.ReadInt16(), reader.ReadInt16());

            // Read the data byte array, which might be compressed or aligned.
            if (alignData)
                reader.Align(4);
            int dataSize = (int)(BitsPerPixel / 8f * Size.Width * Size.Height);
            if (flags.HasFlag(Flags.Compressed))
            {
                // Some shipped images require up to 3 bytes to not fail when decompressing with out-of-bounds access.
                Data = new byte[dataSize];
                Team17Compression.Decompress(reader.BaseStream, Data);
            }
            else
            {
                Data = reader.ReadBytes(dataSize);
            }
        }

        /// <summary>
        /// Loads the data from the given file. The data block can be aligned to a 4-bte boundary.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        /// <param name="alignData"><see langword="true"/> to align the data array by 4 bytes.</param>
        /// <exception cref="IndexOutOfRangeException">Compressed images required more bytes than usually to decompress.
        /// This error can be ignored, the image has been completely read, and is only caused by a few files.</exception>
        public void Load(string fileName, bool alignData)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Load(stream, alignData);
        }

        /// <summary>
        /// Saves the data into the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the data to.</param>
        public void Save(Stream stream) => Save(stream, false);

        /// <summary>
        /// Saves the data in the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to save the data in.</param>
        public void Save(string fileName) => Save(fileName, false);

        /// <summary>
        /// Saves the data into the given <paramref name="stream"/>. The data block can be aligned to a 4-byte boundary.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the data to.</param>
        /// <param name="alignData"><see langword="true"/> to align the data array by 4 bytes.</param>
        public void Save(Stream stream, bool alignData)
        {
            using BinaryStream writer = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);

            // Write the header.
            writer.Write(Signature);
            uint fileSizeOffset = writer.ReserveOffset();

            // Write an optional string describing the image contents and the bits per pixel.
            if (Description != null)
                writer.Write(Description, StringCoding.ZeroTerminated);
            writer.Write(BitsPerPixel);

            // Write image flags describing the format and availability of the following contents.
            Flags flags = Flags.None;
            if (Palette != null)
                flags |= Flags.Palettized;
            if (Compressed)
                flags |= Flags.Compressed;
            writer.WriteEnum(flags, true);

            // Write the image palette if available. The first color of the palette is implicitly black.
            if (Palette != null)
            {
                writer.Write((short)(Palette.Count - 1));
                for (int i = 1; i < Palette.Count; i++)
                {
                    Color color = Palette[i];
                    writer.Write(color.R);
                    writer.Write(color.G);
                    writer.Write(color.B);
                }
            }

            // Write the image size.
            writer.Write((short)Size.Width);
            writer.Write((short)Size.Height);

            // Write the data byte array, which might be compressed or aligned.
            if (alignData)
                writer.Align(4);
            byte[] data = Data;
            if (Compressed)
                data = Team17Compression.Compress(data);
            writer.Write(data);

            writer.SatisfyOffset(fileSizeOffset, (uint)writer.Position);
        }

        /// <summary>
        /// Saves the data in the given file. The data block can be aligned to a 4-byte boundary.
        /// </summary>
        /// <param name="fileName">The name of the file to save the data in.</param>
        /// <param name="alignData"><see langword="true"/> to align the data array by 4 bytes.</param>
        public void Save(string fileName, bool alignData)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Save(stream, alignData);
        }

        // ---- ENUMERATIONS -------------------------------------------------------------------------------------------

        [Flags]
        private enum Flags : byte
        {
            None,
            Compressed = 1 << 6,
            Palettized = 1 << 7
        }
    }
}
