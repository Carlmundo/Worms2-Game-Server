﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Syroot.Worms.Core;

namespace Syroot.Worms.IO
{
    /// <summary>
    /// Represents extension methods for <see cref="Stream"/> instances.
    /// </summary>
    public static class StreamExtensions
    {
        // ---- METHODS (PUBLIC) ---------------------------------------------------------------------------------------

        /// <summary>
        /// Reads a 0-terminated string which is stored in a fixed-size block of <paramref name="length"/> bytes.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> instance to read with.</param>
        /// <param name="length">The number of bytes the fixed-size blocks takes.</param>
        /// <param name="encoding">The 1-byte <see cref="Encoding"/> to use or <see langword="null"/> to use
        /// <see cref="Encoding.ASCII"/>.</param>
        /// <returns>The read string.</returns>
        public static string ReadFixedString(this Stream stream, int length, Encoding? encoding = null)
        {
            // Ensure to not try to decode any bytes after the 0 termination.
            Span<byte> bytes = stackalloc byte[length];
            stream.Read(bytes);
            return (encoding ?? Encoding.ASCII).GetZeroTerminatedString(bytes);
        }

        /// <summary>
        /// Reads the unmanaged representation of a struct of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the struct to read.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance to read with.</param>
        /// <returns>The read value.</returns>
        public static T ReadStruct<T>(this Stream stream) where T : unmanaged
        {
            Span<T> span = stackalloc T[1];
            stream.Read(MemoryMarshal.Cast<T, byte>(span));
            return span[0];
        }

        /// <summary>
        /// Reads the unmanaged representations of structs of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the structs to read.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance to read with.</param>
        /// <param name="span">A span receiving the read instances.</param>
        public static void ReadStructs<T>(this Stream stream, Span<T> span) where T : unmanaged
            => stream.Read(MemoryMarshal.Cast<T, byte>(span));

        /// <summary>
        /// Writes the given string into a fixed-size block of <paramref name="length"/> bytes and 0-terminates it.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> instance to write with.</param>
        /// <param name="value">The string to write.</param>
        /// <param name="length">The number of bytes the fixed-size block takes.</param>
        /// <param name="encoding">The 1-byte <see cref="Encoding"/> to use or <see langword="null"/> to use
        /// <see cref="Encoding.ASCII"/>.</param>
        public static void WriteFixedString(this Stream stream, string value, int length, Encoding? encoding = null)
        {
            Span<byte> bytes = stackalloc byte[length];
            (encoding ?? Encoding.ASCII).GetBytes(value.AsSpan(), bytes);
            stream.Write(bytes);
        }

        /// <summary>
        /// Writes the unmanaged representation of a struct of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the struct to write.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance to write with.</param>
        /// <param name="value">The value to write.</param>
        public static unsafe void WriteStruct<T>(this Stream stream, T value) where T : unmanaged
            => stream.Write(new ReadOnlySpan<byte>(Unsafe.AsPointer(ref value), Unsafe.SizeOf<T>()));

        /// <summary>
        /// Writes the unmanaged representation of a struct of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the struct to write.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance to write with.</param>
        /// <param name="value">The value to write.</param>
        public static unsafe void WriteStruct<T>(this Stream stream, ref T value) where T : unmanaged
            => stream.Write(new ReadOnlySpan<byte>(Unsafe.AsPointer(ref value), Unsafe.SizeOf<T>()));

        /// <summary>
        /// Writes the unmanaged representations of structs of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the struct to write.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance to write with.</param>
        /// <param name="span">The values to write.</param>
        public static unsafe void WriteStructs<T>(this Stream stream, ReadOnlySpan<T> span) where T : unmanaged
            => stream.Write(MemoryMarshal.Cast<T, byte>(span));

        // ---- ILoadable / ISaveable ----

        /// <summary>
        /// Reads an <see cref="ILoadable"/> instance from the current stream.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ILoadable"/> class to instantiate.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance to read with.</param>
        /// <returns>The <see cref="ILoadable"/> instance.</returns>
        public static T Load<T>(this Stream stream) where T : ILoadable, new()
        {
            T instance = new T();
            instance.Load(stream);
            return instance;
        }

        /// <summary>
        /// Writes the given <see cref="ISaveable"/> instance into the current stream.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="ISaveable"/> class to write.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> instance to write with.</param>
        /// <param name="value">The instance to write into the current stream.</param>
        public static void Save<T>(this Stream stream, T value) where T : ISaveable => value.Save(stream);
    }
}
