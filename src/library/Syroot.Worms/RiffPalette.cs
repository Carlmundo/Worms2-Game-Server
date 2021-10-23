using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Syroot.BinaryData;
using Syroot.Worms.Core.Riff;
using Syroot.Worms.Graphics;
using Syroot.Worms.IO;

namespace Syroot.Worms
{
    /// <summary>
    /// Represents a color palette stored in PAL files, following the RIFF format. It is used to index colors in images.
    /// Used by WA and WWP. S. http://worms2d.info/Palette_file.
    /// </summary>
    [RiffFile("PAL ")]
    public class RiffPalette : RiffFile, ILoadableFile, ISaveableFile, IPalette
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const short _version = 0x0300;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="RiffPalette"/> class.
        /// </summary>
        public RiffPalette() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RiffPalette"/> class, loading the data from the given
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        public RiffPalette(Stream stream) => Load(stream);

        /// <summary>
        /// Initializes a new instance of the <see cref="RiffPalette"/> class, loading the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public RiffPalette(string fileName) => Load(fileName);

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the version of the palette data.
        /// </summary>
        public int Version { get; set; } = _version;

        /// <summary>
        /// Gets or sets the <see cref="Color"/> instances stored in this palette.
        /// </summary>
        public IList<Color> Colors { get; set; } = new List<Color>();

        /// <summary>
        /// Gets the unknown data in the offl chunk.
        /// </summary>
        public byte[] OfflData { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Gets the unknown data in the tran chunk.
        /// </summary>
        public byte[] TranData { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Gets the unknown data in the unde chunk.
        /// </summary>
        public byte[] UndeData { get; set; } = Array.Empty<byte>();

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Loads the data from the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        public void Load(Stream stream) => LoadRiff(stream);

        /// <summary>
        /// Loads the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public void Load(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Load(stream);
        }

        /// <summary>
        /// Saves the data into the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the data to.</param>
        public void Save(Stream stream) => SaveRiff(stream);

        /// <summary>
        /// Saves the data in the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to save the data in.</param>
        public void Save(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Save(stream);
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

#pragma warning disable IDE0051 // Remove unused private members
        [RiffChunkLoad("data")]
        private void LoadDataChunk(BinaryStream reader, int _)
        {
            // Read the PAL version.
            Version = reader.ReadInt16();
            if (Version != _version)
                throw new InvalidDataException("Unknown PAL version.");

            // Read the colors.
            short colorCount = reader.ReadInt16();
            Colors = new List<Color>();
            while (colorCount-- > 0)
            {
                Colors.Add(Color.FromArgb(reader.Read1Byte(), reader.Read1Byte(), reader.Read1Byte()));
                _ = reader.ReadByte(); // Dismiss alpha, as it is not used in WA.
            }
        }

        [RiffChunkLoad("offl")]
        private void LoadOfflChunk(BinaryStream reader, int length) => OfflData = reader.ReadBytes(length);

        [RiffChunkLoad("tran")]
        private void LoadTranChunk(BinaryStream reader, int length) => TranData = reader.ReadBytes(length);

        [RiffChunkLoad("unde")]
        private void LoadUndeChunk(BinaryStream reader, int length) => UndeData = reader.ReadBytes(length);

        [RiffChunkSave("data")]
        private void SaveDataChunk(BinaryStream writer)
        {
            // Write the PAL version.
            writer.Write(_version);

            // Write the colors.
            writer.Write((short)Colors.Count);
            foreach (Color color in Colors)
            {
                writer.Write(color.R);
                writer.Write(color.G);
                writer.Write(color.B);
                writer.Write((byte)0); // Dismiss alpha, as it is not used in WA.
            }
        }

        [RiffChunkSave("offl")]
        private void SaveOfflChunk(BinaryStream writer) => writer.Write(OfflData);

        [RiffChunkSave("tran")]
        private void SaveTranChunk(BinaryStream writer) => writer.Write(TranData);

        [RiffChunkSave("unde")]
        private void SaveUndeChunk(BinaryStream writer) => writer.Write(UndeData);
#pragma warning restore IDE0051
    }
}
