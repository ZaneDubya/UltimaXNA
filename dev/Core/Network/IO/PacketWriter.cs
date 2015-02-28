/***************************************************************************
 *   PacketWriter.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
#endregion

namespace UltimaXNA.Core.Network
{
    public class PacketWriter : IDisposable
    {
        private static Stack<PacketWriter> _pool = new Stack<PacketWriter>();

        public static PacketWriter CreateInstance()
        {
            return CreateInstance(32);
        }

        public static PacketWriter CreateInstance(int capacity)
        {
            PacketWriter writer = null;

            lock (_pool)
            {
                if (_pool.Count > 0)
                {
                    writer = _pool.Pop();

                    if (writer != null)
                    {
                        writer._capacity = capacity;
                        writer._stream = new MemoryStream(capacity);
                        writer._stream.Seek(0, SeekOrigin.Begin);
                    }
                }
            }

            if (writer == null)
                writer = new PacketWriter(capacity);

            return writer;
        }

        public static void ReleaseInstance(PacketWriter writer)
        {
            lock (_pool)
            {
                if (!_pool.Contains(writer))
                {
                    _pool.Push(writer);
                }
                else
                {
                    ////log.Warn("Instance pool already contains writer");
                }
            }
        }

        private static byte[] _buffer = new byte[4];
        private MemoryStream _stream;
        private int _capacity;
        
        /// <summary>
        /// Gets the total stream length.
        /// </summary>
        public long Length
        {
            get { return _stream.Length; }
        }

        /// <summary>
        /// Gets or sets the current stream position.
        /// </summary>
        public long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        /// <summary>
        /// The internal stream used by this PacketWriter instance.
        /// </summary>
        public MemoryStream BaseStream
        {
            get { return _stream; }
        }

        /// <summary>
        /// Instantiates a new PacketWriter instance with a 32 byte capacity.
        /// </summary>
        public PacketWriter()
            : this(32)
        {

        }

        /// <summary>
        /// Instantiates a new PacketWriter instance with a given capacity.
        /// </summary>
        /// <param name="capacity">Initial capacity for the internal stream.</param>
        public PacketWriter(int capacity)
        {
            this._stream = new MemoryStream(capacity);
            this._capacity = capacity;
        }

        /// <summary>
        /// Writes a 1-byte boolean value to the underlying stream. False is represented by 0, true by 1.
        /// </summary>
        public void Write(bool value)
        {
            _stream.WriteByte((byte)(value ? 1 : 0));
        }

        /// <summary>
        /// Writes a 1-byte unsigned integer value to the underlying stream.
        /// </summary>
        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }

        /// <summary>
        /// Writes a 1-byte signed integer value to the underlying stream.
        /// </summary>
        public void Write(sbyte value)
        {
            _stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes a 2-byte signed integer value to the underlying stream.
        /// </summary>
        public void Write(short value)
        {
            _buffer[0] = (byte)(value >> 8);
            _buffer[1] = (byte)value;

            _stream.Write(_buffer, 0, 2);
        }

        /// <summary>
        /// Writes a 2-byte unsigned integer value to the underlying stream.
        /// </summary>
        public void Write(ushort value)
        {
            _buffer[0] = (byte)(value >> 8);
            _buffer[1] = (byte)value;

            _stream.Write(_buffer, 0, 2);
        }

        /// <summary>
        /// Writes a 4-byte signed integer value to the underlying stream.
        /// </summary>
        public void Write(int value)
        {
            _buffer[0] = (byte)(value >> 24);
            _buffer[1] = (byte)(value >> 16);
            _buffer[2] = (byte)(value >> 8);
            _buffer[3] = (byte)value;

            _stream.Write(_buffer, 0, 4);
        }

        /// <summary>
        /// Writes a 4-byte unsigned integer value to the underlying stream.
        /// </summary>
        public void Write(uint value)
        {
            _buffer[0] = (byte)(value >> 24);
            _buffer[1] = (byte)(value >> 16);
            _buffer[2] = (byte)(value >> 8);
            _buffer[3] = (byte)value;

            _stream.Write(_buffer, 0, 4);
        }

        /// <summary>
        /// Writes a sequence of bytes to the underlying stream
        /// </summary>
        public void Write(byte[] buffer, int offset, int size)
        {
            _stream.Write(buffer, offset, size);
        }

        /// <summary>
        /// Writes a fixed-length ASCII-encoded string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
        /// </summary>
        public void WriteAsciiFixed(string value, int size)
        {
            if (value == null)
            {
                ////log.Warn("Attempted to WriteAsciiFixed() with null value");
                value = String.Empty;
            }

            int length = value.Length;

            _stream.SetLength(_stream.Length + size);

            if (length >= size)
                _stream.Position += Encoding.ASCII.GetBytes(value, 0, size, _stream.GetBuffer(), (int)_stream.Position);
            else
            {
                Encoding.ASCII.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
                _stream.Position += size;
            }
        }

        /// <summary>
        /// Writes a dynamic-length ASCII-encoded string value to the underlying stream, followed by a 1-byte null character.
        /// </summary>
        public void WriteAsciiNull(string value)
        {
            if (value == null)
            {
                ////log.Warn("Attempted to WriteAsciiNull() with null value");
                value = String.Empty;
            }

            int length = value.Length;

            _stream.SetLength(_stream.Length + length + 1);

            Encoding.ASCII.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
            _stream.Position += length + 1;
        }

        /// <summary>
        /// Writes a dynamic-length little-endian unicode string value to the underlying stream, followed by a 2-byte null character.
        /// </summary>
        public void WriteLittleUniNull(string value)
        {
            if (value == null)
            {
                //log.Warn("Attempted to WriteLittleUniNull() with null value");
                value = String.Empty;
            }

            int length = value.Length;

            _stream.SetLength(_stream.Length + ((length + 1) * 2));

            _stream.Position += Encoding.Unicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
            _stream.Position += 2;
        }

        /// <summary>
        /// Writes a fixed-length little-endian unicode string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
        /// </summary>
        public void WriteLittleUniFixed(string value, int size)
        {
            if (value == null)
            {
                //log.Warn("Attempted to WriteLittleUniFixed() with null value");
                value = String.Empty;
            }

            size *= 2;

            int length = value.Length;

            _stream.SetLength(_stream.Length + size);

            if ((length * 2) >= size)
                _stream.Position += Encoding.Unicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
            else
            {
                Encoding.Unicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
                _stream.Position += size;
            }
        }

        /// <summary>
        /// Writes a dynamic-length big-endian unicode string value to the underlying stream, followed by a 2-byte null character.
        /// </summary>
        public void WriteBigUniNull(string value)
        {
            if (value == null)
            {
                //log.Warn("Attempted to WriteBigUniNull() with null value");
                value = String.Empty;
            }

            int length = value.Length;

            _stream.SetLength(_stream.Length + ((length + 1) * 2));

            _stream.Position += Encoding.BigEndianUnicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
            _stream.Position += 2;
        }

        /// <summary>
        /// Writes a fixed-length big-endian unicode string value to the underlying stream. To fit (size), the string content is either truncated or padded with null characters.
        /// </summary>
        public void WriteBigUniFixed(string value, int size)
        {
            if (value == null)
            {
                //log.Warn("Attempted to WriteBigUniFixed() with null value");
                value = String.Empty;
            }

            size *= 2;

            int length = value.Length;

            _stream.SetLength(_stream.Length + size);

            if ((length * 2) >= size)
                _stream.Position += Encoding.BigEndianUnicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
            else
            {
                Encoding.BigEndianUnicode.GetBytes(value, 0, length, _stream.GetBuffer(), (int)_stream.Position);
                _stream.Position += size;
            }
        }

        /// <summary>
        /// Fills the stream from the current position up to (capacity) with 0x00's
        /// </summary>
        public void Fill()
        {
            Fill((int)(_capacity - _stream.Length));
        }

        /// <summary>
        /// Writes a number of 0x00 byte values to the underlying stream.
        /// </summary>
        public void Fill(int length)
        {
            if (_stream.Position == _stream.Length)
            {
                _stream.SetLength(_stream.Length + length);
                _stream.Seek(0, SeekOrigin.End);
            }
            else
            {
                _stream.Write(new byte[length], 0, length);
            }
        }

        /// <summary>
        /// Offsets the current position from an origin.
        /// </summary>
        public long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        /// <summary>
        /// Gets the entire stream content as a byte array.
        /// </summary>
        public byte[] ToArray()
        {
            return _stream.ToArray();
        }

        #region IDisposable Members

        public void Dispose()
        {
            _capacity = 0;

            if (_stream != null)
            {
                _stream.Close();
                _stream.Dispose();
            }

            ReleaseInstance(this);
        }

        #endregion

        public void Flush()
        {
            BaseStream.Flush();
        }

        public byte[] Compile()
        {
            Flush();
            return _stream.ToArray();
        }
    }
}
