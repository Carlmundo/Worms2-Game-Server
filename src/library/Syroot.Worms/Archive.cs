using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Syroot.BinaryData;
using Syroot.Worms.IO;

namespace Syroot.Worms
{
    /// <summary>
    /// Represents a directory of files stored in a DIR file, mostly used to store graphics files. Due to the nowadays
    /// small size of typical directories, the entries and data are loaded directly into a dictionary to profit from
    /// optimal performance when accessing and manipulating the directory.
    /// Used by W2, WA and WWP. S. https://worms2d.info/Graphics_directory.
    /// </summary>
    public class Archive : IDictionary<string, byte[]>, ILoadableFile, ISaveableFile
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const int _signature = 0x1A524944; // "DIR", 0x1A
        private const int _tocSignature = 0x0000000A;

        private const int _hashBits = 10;
        private const int _hashSize = 1 << _hashBits;

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private readonly IDictionary<string, byte[]> _entries = new Dictionary<string, byte[]>();

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="Archive"/> class.
        /// </summary>
        public Archive() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Archive"/> class, loading the data from the given
        /// <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        public Archive(Stream stream) => Load(stream);

        /// <summary>
        /// Initializes a new instance of the <see cref="Archive"/> class, loading the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public Archive(string fileName) => Load(fileName);

        // ---- OPERATORS ----------------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public byte[] this[string key]
        {
            get => _entries[key];
            set => _entries[key] = value;
        }

        // ---- PROPERTIES ---------------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public ICollection<string> Keys => _entries.Keys;

        /// <inheritdoc/>
        public ICollection<byte[]> Values => _entries.Values;

        /// <inheritdoc/>
        public int Count => _entries.Count;

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, byte[]>>.IsReadOnly => false;

        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <inheritdoc/>
        public void Add(string key, byte[] value) => _entries.Add(key, value);

        /// <inheritdoc/>
        public void Clear() => _entries.Clear();
        /// <inheritdoc/>
        public bool ContainsKey(string key) => _entries.ContainsKey(key);

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, byte[]>> GetEnumerator() => _entries.GetEnumerator();

        /// <summary>
        /// Loads the data from the given <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the data from.</param>
        public void Load(Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("Stream requires to be seekable.", nameof(stream));

            _entries.Clear();
            using BinaryStream reader = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);

            // Read the header.
            if (reader.ReadInt32() != _signature)
                throw new InvalidDataException("Invalid DIR file signature.");
            int fileSize = reader.ReadInt32();
            int tocOffset = reader.ReadInt32();

            // Read the table of contents.
            reader.Position = tocOffset;
            int tocSignature = reader.ReadInt32();
            if (tocSignature != _tocSignature)
                throw new InvalidDataException("Invalid DIR table of contents signature.");
            // Generate a data dictionary out of the hash table and file entries.
            int[] hashTable = reader.ReadInt32s(_hashSize);
            foreach (int entryOffset in hashTable)
            {
                // If the hash is not 0, it points to a list of files which have a hash being the hash table index.
                if (entryOffset > 0)
                {
                    int nextEntryOffset = entryOffset;
                    do
                    {
                        reader.Position = tocOffset + nextEntryOffset;
                        nextEntryOffset = reader.ReadInt32();
                        int offset = reader.ReadInt32();
                        int length = reader.ReadInt32();
                        string name = reader.ReadString(StringCoding.ZeroTerminated);
                        using (reader.TemporarySeek(offset, SeekOrigin.Begin))
                            _entries.Add(name, reader.ReadBytes(length));
                    } while (nextEntryOffset != 0);
                }
            }
        }

        /// <summary>
        /// Loads the data from the given file.
        /// </summary>
        /// <param name="fileName">The name of the file to load the data from.</param>
        public void Load(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            Load(stream);
        }

        /// <inheritdoc/>
        public bool Remove(string key) => _entries.Remove(key);

        /// <summary>
        /// Saves the data into the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the data in.</param>
        public void Save(Stream stream)
        {
            using BinaryStream writer = new BinaryStream(stream, leaveOpen: true);

            // Write the header.
            writer.Write(_signature);
            uint fileSizeOffset = writer.ReserveOffset();
            uint tocOffset = writer.ReserveOffset();

            // Write the data and build the hash table and file entries.
            List<HashTableEntry>[] hashTable = new List<HashTableEntry>[_hashSize];
            foreach (KeyValuePair<string, byte[]> item in _entries)
            {
                HashTableEntry entry = new HashTableEntry()
                {
                    Name = item.Key,
                    Offset = (int)writer.Position,
                    Length = item.Value.Length
                };
                writer.Write(item.Value);

                int hash = CalculateHash(item.Key);
                if (hashTable[hash] == null)
                    hashTable[hash] = new List<HashTableEntry>();
                hashTable[hash].Add(entry);
            }

            // Write the hash table and file entries.
            uint tocStart = (uint)writer.Position;
            uint fileEntryOffset = sizeof(int) + _hashSize * sizeof(int);
            writer.SatisfyOffset(tocOffset, tocStart);
            writer.Write(_tocSignature);
            for (int i = 0; i < _hashSize; i++)
            {
                List<HashTableEntry> entries = hashTable[i];
                if (entries == null)
                {
                    writer.Write(0);
                }
                else
                {
                    // Write the entries resolving to the current hash.
                    writer.Write(fileEntryOffset);
                    using (writer.TemporarySeek(tocStart + fileEntryOffset, SeekOrigin.Begin))
                    {
                        for (int j = 0; j < entries.Count; j++)
                        {
                            HashTableEntry entry = entries[j];
                            uint nextEntryOffset = writer.ReserveOffset();
                            writer.Write(entry.Offset);
                            writer.Write(entry.Length);
                            writer.Write(entry.Name, StringCoding.ZeroTerminated);
                            writer.Align(4);
                            if (j < entries.Count - 1)
                                writer.SatisfyOffset(nextEntryOffset, (uint)writer.Position - tocStart);
                        }
                        fileEntryOffset = (uint)writer.Position - tocStart;
                    }
                }
            }

            writer.SatisfyOffset(fileSizeOffset, tocStart + fileEntryOffset - 1);
        }

        /// <summary>
        /// Saves the data in the file with the given <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The name of the file to save the data in.</param>
        public void Save(string fileName)
        {
            using FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            Save(stream);
        }

        /// <inheritdoc/>
        public bool TryGetValue(string key, out byte[] value) => _entries.TryGetValue(key, out value);

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private int CalculateHash(string name)
        {
            int hash = 0;
            for (int i = 0; i < name.Length; i++)
            {
                hash = ((hash << 1) % _hashSize) | (hash >> (_hashBits - 1) & 1);
                hash += name[i];
                hash %= _hashSize;
            }
            return hash;
        }

        // ---- METHODS ------------------------------------------------------------------------------------------------

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, byte[]>>.Add(KeyValuePair<string, byte[]> item) => _entries.Add(item);

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, byte[]>>.Contains(KeyValuePair<string, byte[]> item) => _entries.Contains(item);

        /// <inheritdoc/>
        void ICollection<KeyValuePair<string, byte[]>>.CopyTo(KeyValuePair<string, byte[]>[] array, int arrayIndex) => _entries.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _entries.GetEnumerator();

        /// <inheritdoc/>
        bool ICollection<KeyValuePair<string, byte[]>>.Remove(KeyValuePair<string, byte[]> item) => _entries.Remove(item);

        // ---- CLASSES, STRUCTS & ENUMS -------------------------------------------------------------------------------

        private struct HashTableEntry
        {
            internal string Name;
            internal int Offset;
            internal int Length;
        }
    }
}
