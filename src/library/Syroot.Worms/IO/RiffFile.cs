using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Syroot.BinaryData;
using Syroot.Worms.IO;

namespace Syroot.Worms.Core.Riff
{
    /// <summary>
    /// Represents the format of RIFF files, which manage their data in chunks.
    /// </summary>
    public abstract class RiffFile
    {
        // ---- CONSTANTS ----------------------------------------------------------------------------------------------

        private const string _signature = "RIFF";

        // ---- FIELDS -------------------------------------------------------------------------------------------------

        private static readonly Dictionary<Type, TypeData> _typeDataCache = new Dictionary<Type, TypeData>();

        private readonly TypeData _typeData;

        // ---- CONSTRUCTORS & DESTRUCTOR ------------------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance of the <see cref="RiffFile"/> class.
        /// </summary>
        protected RiffFile() => _typeData = GetTypeData();

        // ---- METHODS (PROTECTED) ------------------------------------------------------------------------------------

        /// <summary>
        /// Loads the RIFF data from the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to load the RIFF data from.</param>
        protected void LoadRiff(Stream stream)
        {
            using BinaryStream reader = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);

            // Read the file header.
            if (reader.ReadString(_signature.Length) != _signature)
                throw new InvalidDataException("Invalid RIFF file signature.");
            int fileSize = reader.ReadInt32();
            string fileIdentifier = reader.ReadString(4);
            if (fileIdentifier != _typeData.FileIdentifier)
                throw new InvalidDataException("Invalid RIFF file identifier.");

            // Read the chunks.
            while (!reader.EndOfStream)
            {
                string chunkIdentifier = reader.ReadString(4);
                int chunkLength = reader.ReadInt32();
                // Invoke a loader method if matching the identifier or skip the chunk.
                if (_typeData.ChunkLoaders.TryGetValue(chunkIdentifier, out MethodInfo loader))
                    loader.Invoke(this, new object[] { reader, chunkLength });
                else
                    reader.Seek(chunkLength);
            }
        }

        /// <summary>
        /// Saves the RIFF data in the given <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to save the RIFF data in.</param>
        protected void SaveRiff(Stream stream)
        {
            using BinaryStream writer = new BinaryStream(stream, encoding: Encoding.ASCII, leaveOpen: true);

            // Write the header.
            writer.Write(_signature, StringCoding.Raw);
            uint fileSizeOffset = writer.ReserveOffset();
            writer.Write(_typeData.FileIdentifier, StringCoding.Raw);

            // Write the chunks.
            foreach (KeyValuePair<string, MethodInfo> chunkSaver in _typeData.ChunkSavers)
            {
                writer.Write(chunkSaver.Key, StringCoding.Raw);
                uint chunkSizeOffset = writer.ReserveOffset();

                chunkSaver.Value.Invoke(this, new object[] { writer });

                writer.SatisfyOffset(chunkSizeOffset, (uint)(writer.Position - chunkSizeOffset - 4));
            }

            writer.SatisfyOffset(fileSizeOffset, (uint)(writer.Position - 8));
        }

        // ---- METHODS (PRIVATE) --------------------------------------------------------------------------------------

        private TypeData GetTypeData()
        {
            Type type = GetType();
            if (!_typeDataCache.TryGetValue(type, out TypeData typeData))
            {
                typeData = new TypeData();
                TypeInfo typeInfo = type.GetTypeInfo();

                // Get the file identifier.
                typeData.FileIdentifier = typeInfo.GetCustomAttribute<RiffFileAttribute>().Identifier;

                // Get the chunk loading and saving handlers.
                foreach (MethodInfo method in typeInfo.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    RiffChunkLoadAttribute loadAttribute = method.GetCustomAttribute<RiffChunkLoadAttribute>();
                    if (loadAttribute != null)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length == 2
                            && parameters[0].ParameterType == typeof(BinaryStream)
                            && parameters[1].ParameterType == typeof(int))
                        {
                            typeData.ChunkLoaders.Add(loadAttribute.Identifier, method);
                        }
                        continue;
                    }
                    RiffChunkSaveAttribute saveAttribute = method.GetCustomAttribute<RiffChunkSaveAttribute>();
                    if (saveAttribute != null)
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(BinaryStream))
                            typeData.ChunkSavers.Add(saveAttribute.Identifier, method);
                        continue;
                    }
                }

                _typeDataCache.Add(type, typeData);
            }
            return typeData;
        }

        // ---- CLASSES ------------------------------------------------------------------------------------------------

        private class TypeData
        {
            internal string FileIdentifier = String.Empty;
            internal IDictionary<string, MethodInfo> ChunkLoaders = new Dictionary<string, MethodInfo>();
            internal IDictionary<string, MethodInfo> ChunkSavers = new Dictionary<string, MethodInfo>();
        }
    }
}
