/***************************************************************************
 *   PacketReader.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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

namespace UltimaXNA.Network
{
    public class PacketReader : IDisposable
    {
        //static ILoggable log = new Logger("Static PacketReader");

        private static Stack<PacketReader> _pool = new Stack<PacketReader>();

        public static PacketReader CreateInstance(byte[] buffer, int length, bool fixedSize)
        {
            PacketReader reader = null;

            lock (_pool)
            {
                if (_pool.Count > 0)
                {
                    reader = _pool.Pop();

                    if (reader != null)
                    {
                        reader._buffer = buffer;
                        reader._length = length;
                        reader._index = fixedSize ? 1 : 3;
                    }
                }
            }

            if (reader == null)
                reader = new PacketReader(buffer, length, fixedSize);

            return reader;
        }

        public static void ReleaseInstance(PacketReader reader)
        {
            lock (_pool)
            {
                if (!_pool.Contains(reader))
                {
                    _pool.Push(reader);
                }
                else
                {
                    ////log.Warn("Instance pool already contains reader");
                }
            }
        }
        private byte[] _buffer;
        private int _length;
        private int _index;

        public int Index
        {
            get { return _index; }
        }

        public PacketReader(byte[] data, int size, bool fixedSize)
        {
            _buffer = data;
            _length = size;
            _index = fixedSize ? 1 : 3;
        }

        public byte[] Buffer
        {
            get
            {
                return _buffer;
            }
        }

        public int Size
        {
            get
            {
                return _length;
            }
        }

        public int Seek(int offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin: _index = offset; break;
                case SeekOrigin.Current: _index += offset; break;
                case SeekOrigin.End: _index = _length - offset; break;
            }

            return _index;
        }

        public int ReadInt32()
        {
            if ((_index + 4) > _length)
                return 0;

            return (_buffer[_index++] << 24)
                 | (_buffer[_index++] << 16)
                 | (_buffer[_index++] << 8)
                 | _buffer[_index++];
        }

        public short ReadInt16()
        {
            if ((_index + 2) > _length)
                return 0;

            return (short)((_buffer[_index++] << 8) | _buffer[_index++]);
        }

        public byte ReadByte()
        {
            if ((_index + 1) > _length)
                return 0;

            return _buffer[_index++];
        }

        public byte[] ReadBytes(int length)
        {
            if ((_index + length) > _length)
                return new byte[0];

            byte[] b = new byte[length];

            Array.Copy(_buffer, _index, b, 0, length);
            _index += length;
            return b;
        }

        public uint ReadUInt32()
        {
            if ((_index + 4) > _length)
                return 0;

            return (uint)((_buffer[_index++] << 24) | (_buffer[_index++] << 16) | (_buffer[_index++] << 8) | _buffer[_index++]);
        }

        public ushort ReadUInt16()
        {
            if ((_index + 2) > _length)
                return 0;

            return (ushort)((_buffer[_index++] << 8) | _buffer[_index++]);
        }

        public sbyte ReadSByte()
        {
            if ((_index + 1) > _length)
                return 0;

            return (sbyte)_buffer[_index++];
        }

        public bool ReadBoolean()
        {
            if ((_index + 1) > _length)
                return false;

            return (_buffer[_index++] != 0);
        }

        public string ReadUnicodeStringLE()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < _length && (c = (_buffer[_index++] | (_buffer[_index++] << 8))) != 0)
                sb.Append((char)c);

            return sb.ToString();
        }

        public string ReadUnicodeStringLESafe(int fixedLength)
        {
            int bound = _index + (fixedLength << 1);
            int end = bound;

            if (bound > _length)
                bound = _length;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < bound && (c = (_buffer[_index++] | (_buffer[_index++] << 8))) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            _index = end;

            return sb.ToString();
        }

        public string ReadUnicodeStringLESafe()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < _length && (c = (_buffer[_index++] | (_buffer[_index++] << 8))) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringSafe()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < _length && (c = ((_buffer[_index++] << 8) | _buffer[_index++])) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeString()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < _length && (c = ((_buffer[_index++] << 8) | _buffer[_index++])) != 0)
                sb.Append((char)c);

            return sb.ToString();
        }

        public bool IsSafeChar(int c)
        {
            return ((c >= 0x20 && c < 0xFFFE) || (c == 0x09));
        }

        public string ReadUTF8StringSafe(int fixedLength)
        {
            if (_index >= _length)
            {
                _index += fixedLength;
                return String.Empty;
            }

            int bound = _index + fixedLength;
            //int end   = bound;

            if (bound > _length)
                bound = _length;

            int count = 0;
            int index = _index;
            int start = _index;

            while (index < bound && _buffer[index++] != 0)
                ++count;

            index = 0;

            byte[] buffer = new byte[count];
            int value = 0;

            while (_index < bound && (value = _buffer[_index++]) != 0)
                buffer[index++] = (byte)value;

            string s = Utility.UTF8.GetString(buffer);

            bool isSafe = true;

            for (int i = 0; isSafe && i < s.Length; ++i)
                isSafe = IsSafeChar((int)s[i]);

            _index = start + fixedLength;

            if (isSafe)
                return s;

            StringBuilder sb = new StringBuilder(s.Length);

            for (int i = 0; i < s.Length; ++i)
                if (IsSafeChar((int)s[i]))
                    sb.Append(s[i]);

            return sb.ToString();
        }

        public string ReadUTF8StringSafe()
        {
            if (_index >= _length)
                return String.Empty;

            int count = 0;
            int index = _index;

            while (index < _length && _buffer[index++] != 0)
                ++count;

            index = 0;

            byte[] buffer = new byte[count];
            int value = 0;

            while (_index < _length && (value = _buffer[_index++]) != 0)
                buffer[index++] = (byte)value;

            string s = Utility.UTF8.GetString(buffer);

            bool isSafe = true;

            for (int i = 0; isSafe && i < s.Length; ++i)
                isSafe = IsSafeChar((int)s[i]);

            if (isSafe)
                return s;

            StringBuilder sb = new StringBuilder(s.Length);

            for (int i = 0; i < s.Length; ++i)
            {
                if (IsSafeChar((int)s[i]))
                    sb.Append(s[i]);
            }

            return sb.ToString();
        }

        public string ReadUTF8String()
        {
            if (_index >= _length)
                return String.Empty;

            int count = 0;
            int index = _index;

            while (index < _length && _buffer[index++] != 0)
                ++count;

            index = 0;

            byte[] buffer = new byte[count];
            int value = 0;

            while (_index < _length && (value = _buffer[_index++]) != 0)
                buffer[index++] = (byte)value;

            return Utility.UTF8.GetString(buffer);
        }

        public string ReadString()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while (_index < _length && (c = _buffer[_index++]) != 0)
                sb.Append((char)c);

            return sb.ToString();
        }

        public string ReadStringSafe()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while (_index < _length && (c = _buffer[_index++]) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringSafe(int fixedLength)
        {
            int bound = _index + (fixedLength << 1);
            int end = bound;

            if (bound > _length)
                bound = _length;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < bound && (c = ((_buffer[_index++] << 8) | _buffer[_index++])) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            _index = end;

            return sb.ToString();
        }

        public string ReadUnicodeStringSafeReverse()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < _length && (c = ((_buffer[_index++]) | _buffer[_index++] << 8)) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringReverse(int fixedLength)
        {
            int bound = _index + (fixedLength << 1);
            int end = bound;

            if (bound > _length)
                bound = _length;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < bound && (c = ((_buffer[_index++]) | _buffer[_index++] << 8)) != 0)
                sb.Append((char)c);

            _index = end;

            return sb.ToString();
        }

        public string ReadUnicodeString(int fixedLength)
        {
            int bound = _index + (fixedLength << 1);
            int end = bound;

            if (bound > _length)
                bound = _length;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((_index + 1) < bound && (c = ((_buffer[_index++] << 8) | _buffer[_index++])) != 0)
                sb.Append((char)c);

            _index = end;

            return sb.ToString();
        }

        public string ReadStringSafe(int fixedLength)
        {
            int bound = _index + fixedLength;
            int end = bound;

            if (bound > _length)
                bound = _length;

            StringBuilder sb = new StringBuilder();

            int c;

            while (_index < bound && (c = _buffer[_index++]) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            _index = end;

            return sb.ToString();
        }

        public string ReadString(int fixedLength)
        {
            int bound = _index + fixedLength;
            int end = bound;

            if (bound > _length)
                bound = _length;

            StringBuilder sb = new StringBuilder();

            int c;

            while (_index < bound && (c = _buffer[_index++]) != 0)
                sb.Append((char)c);

            _index = end;

            return sb.ToString();
        }

        public void Dispose()
        {
            _buffer = null;
            _length = 0;
            _index = 0;

            ReleaseInstance(this);
        }
    }
}
