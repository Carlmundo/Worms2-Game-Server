using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Worms.IO;

namespace Syroot.Worms.Worms2
{
    /// <summary>
    /// Represents map configuration stored by the land generator in LAND.DAT files.
    /// Used by W2 and OW. S. https://worms2d.info/Land_Data_file.
    /// </summary>
    public class LandData : ILoadableFile, ISaveableFile
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _signature = 0x1A444E4C; // "LND\x1A"

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="LandData"/> class.
        /// </summary>
        public LandData() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LandData"/> class, loading the data from the given
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        public LandData(Stream stream) => Load(stream);

        /// <summary>
        /// Initializes a new instance of the <see cref="LandData"/> class, loading the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public LandData(string fileName) => Load(fileName);

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the size of the landscape in pixels.
        /// </summary>
        public Size Size { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether an indestructible top border will be enabled.
        /// </summary>
        public bool TopBorder { get; set; }

        /// <summary>
        /// Gets or sets an array of coordinates at which objects can be placed.
        /// </summary>
        public IList<Point> ObjectLocations { get; set; } = new List<Point>();

        /// <summary>
        /// Gets or sets an unknown value, seeming to be 0 most of the time.
        /// </summary>
        public int Unknown { get; set; }

        /// <summary>
        /// Gets or sets the visual foreground image.
        /// </summary>
        public Img Foreground { get; set; } = new Img();

        /// <summary>
        /// Gets or sets the collision mask of the landscape.
        /// </summary>
        public Img CollisionMask { get; set; } = new Img();

        /// <summary>
        /// Gets or sets the visual background image.
        /// </summary>
        public Img Background { get; set; } = new Img();

        /// <summary>
        /// Gets or sets an image of unknown use.
        /// </summary>
        public Img UnknownImage { get; set; } = new Img();

        /// <summary>
        /// Gets or sets the path to the land image file.
        /// </summary>
        public string LandTexturePath { get; set; } = String.Empty;

        /// <summary>
        /// Gets or sets the path to the Water.dir file.
        /// </summary>
        public string WaterDirPath { get; set; } = String.Empty;

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public void Load(Stream stream)
        {
            using BinaryStream reader = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);

            // Read the header.
            if (reader.ReadInt32() != _signature)
                throw new InvalidDataException("Invalid LND file signature.");
            int fileSize = reader.ReadInt32();

            // Read the data.
            Size = reader.ReadStruct<Size>();
            TopBorder = reader.ReadBoolean(BooleanCoding.Dword);

            // Read the possible object coordinate array.
            ObjectLocations = new List<Point>();
            int locationCount = reader.ReadInt32();
            for (int i = 0; i < locationCount; i++)
                ObjectLocations.Add(reader.ReadStruct<Point>());
            Unknown = reader.ReadInt32();

            // Read the image data.
            Foreground = reader.Load<Img>();
            CollisionMask = reader.Load<Img>();
            Background = reader.Load<Img>();
            UnknownImage = reader.Load<Img>();

            // Read the file paths.
            LandTexturePath = reader.ReadString(StringCoding.ByteCharCount);
            WaterDirPath = reader.ReadString(StringCoding.ByteCharCount);
        }

        /// <inheritdoc/>
        public void Load(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Load(stream);
        }

        /// <inheritdoc/>
        public void Save(Stream stream)
        {
            using BinaryStream writer = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);

            // Write the header.
            writer.Write(_signature);
            uint fileSizeOffset = writer.ReserveOffset();

            // Write the data.
            writer.WriteStruct(Size);
            writer.Write(TopBorder, BooleanCoding.Dword);

            // Write the possible object coordinate array.
            writer.Write(ObjectLocations.Count);
            for (int i = 0; i < ObjectLocations.Count; i++)
                writer.WriteStruct(ObjectLocations[i]);
            writer.Write(Unknown);

            // Write the image data.
            Foreground.Save(writer.BaseStream);
            CollisionMask.Save(writer.BaseStream);
            Background.Save(writer.BaseStream);
            UnknownImage.Save(writer.BaseStream);

            // Write the file paths.
            writer.Write(LandTexturePath, StringCoding.ByteCharCount);
            writer.Write(WaterDirPath, StringCoding.ByteCharCount);

            writer.SatisfyOffset(fileSizeOffset, (uint)writer.Position);
        }

        /// <inheritdoc/>
        public void Save(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Save(stream);
        }
    }
}
