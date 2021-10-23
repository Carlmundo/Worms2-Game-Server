using System;
using System.IO;
using System.Linq;
using System.Text;
using Syroot.BinaryData;
using Syroot.Worms.IO;

namespace Syroot.Worms.Worms2
{
    /// <summary>
    /// Represents scheme weapons stored in an WEP file which contains armory configuration.
    /// Used by W2. S. https://worms2d.info/Weapons_file.
    /// </summary>
    public class SchemeWeapons : ILoadableFile, ISaveableFile, IEquatable<SchemeWeapons?>
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _trashLength = 16;
        private const string _signature = "WEPFILE"; // Zero-terminated.
        private const int _weaponCount = 38;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeWeapons"/> class.
        /// </summary>
        public SchemeWeapons() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeWeapons"/> class, loading the data from the given
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        public SchemeWeapons(Stream stream) => Load(stream);

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeWeapons"/> class, loading the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public SchemeWeapons(string fileName) => Load(fileName);

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the array of <see cref="SchemeWeapon"/> instances, each mapping to one weapon at the index of
        /// the <see cref="Weapon"/> enumeration.
        /// </summary>
        public SchemeWeapon[] Weapons { get; set; } = new SchemeWeapon[_weaponCount];

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public override bool Equals(object? obj) => Equals(obj as SchemeWeapons);

        /// <inheritdoc/>
        public bool Equals(SchemeWeapons? other)
            => other != null
            && Weapons.SequenceEqual(other.Weapons);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Weapons);

        /// <inheritdoc/>
        public void Load(Stream stream)
        {
            using BinaryStream reader = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);

            // Read the header.
            reader.Seek(_trashLength);
            if (reader.ReadString(StringCoding.ZeroTerminated) != _signature)
                throw new InvalidDataException("Invalid WEP file signature.");

            // Read the weapon settings.
            Weapons = new SchemeWeapon[_weaponCount];
            for (int i = 0; i < _weaponCount; i++)
                Weapons[i] = reader.ReadStruct<SchemeWeapon>();
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
            writer.Write(new byte[_trashLength]);
            writer.Write(_signature, StringCoding.ZeroTerminated);

            // Write the weapon settings.
            foreach (SchemeWeapon weapon in Weapons)
                writer.WriteStruct(weapon);
        }

        /// <inheritdoc/>
        public void Save(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Save(stream);
        }
    }
}
