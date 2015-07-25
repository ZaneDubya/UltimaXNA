/***************************************************************************
 *   PacketReader.cs
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
using System.IO;
using System.Text;
#endregion

namespace UltimaXNA.Core.Network
{
    public class PacketReader : IDisposable
    {
        private static Stack<PacketReader> m_pool = new Stack<PacketReader>();

        public static PacketReader CreateInstance(byte[] buffer, int length, bool fixedSize)
        {
            PacketReader reader = null;

            lock (m_pool)
            {
                if (m_pool.Count > 0)
                {
                    reader = m_pool.Pop();

                    if (reader != null)
                    {
                        reader.m_buffer = buffer;
                        reader.m_length = length;
                        reader.m_index = fixedSize ? 1 : 3;
                    }
                }
            }

            if (reader == null)
                reader = new PacketReader(buffer, length, fixedSize);

            return reader;
        }

        public static void ReleaseInstance(PacketReader reader)
        {
            lock (m_pool)
            {
                if (!m_pool.Contains(reader))
                {
                    m_pool.Push(reader);
                }
                else
                {
                    ////log.Warn("Instance pool already contains reader");
                }
            }
        }
        private byte[] m_buffer;
        private int m_length;
        private int m_index;

        public int Index
        {
            get { return m_index; }
        }

        public PacketReader(byte[] data, int size, bool fixedSize)
        {
            m_buffer = data;
            m_length = size;
            m_index = fixedSize ? 1 : 3;
        }

        public byte[] Buffer
        {
            get
            {
                return m_buffer;
            }
        }

        public int Size
        {
            get
            {
                return m_length;
            }
        }

        public int Seek(int offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin: m_index = offset; break;
                case SeekOrigin.Current: m_index += offset; break;
                case SeekOrigin.End: m_index = m_length - offset; break;
            }

            return m_index;
        }

        public int ReadInt32()
        {
            if ((m_index + 4) > m_length)
                return 0;

            return (m_buffer[m_index++] << 24)
                 | (m_buffer[m_index++] << 16)
                 | (m_buffer[m_index++] << 8)
                 | m_buffer[m_index++];
        }

        public short ReadInt16()
        {
            if ((m_index + 2) > m_length)
                return 0;

            return (short)((m_buffer[m_index++] << 8) | m_buffer[m_index++]);
        }

        public byte ReadByte()
        {
            if ((m_index + 1) > m_length)
                return 0;

            return m_buffer[m_index++];
        }

        public byte[] ReadBytes(int length)
        {
            if ((m_index + length) > m_length)
                return new byte[0];

            byte[] b = new byte[length];

            Array.Copy(m_buffer, m_index, b, 0, length);
            m_index += length;
            return b;
        }

        public ulong ReadUInt64()
        {
            if ((m_index + 8) > m_length)
                return 0;

            return (ulong)(
                ((ulong)m_buffer[m_index++] << 56) | ((ulong)m_buffer[m_index++] << 48) | ((ulong)m_buffer[m_index++] << 40) | ((ulong)m_buffer[m_index++] << 32) |
                ((ulong)m_buffer[m_index++] << 24) | ((ulong)m_buffer[m_index++] << 16) | ((ulong)m_buffer[m_index++] << 8) | (ulong)m_buffer[m_index++]);
        }

        public uint ReadUInt32()
        {
            if ((m_index + 4) > m_length)
                return 0;

            return (uint)((m_buffer[m_index++] << 24) | (m_buffer[m_index++] << 16) | (m_buffer[m_index++] << 8) | m_buffer[m_index++]);
        }

        public ushort ReadUInt16()
        {
            if ((m_index + 2) > m_length)
                return 0;

            return (ushort)((m_buffer[m_index++] << 8) | m_buffer[m_index++]);
        }

        public sbyte ReadSByte()
        {
            if ((m_index + 1) > m_length)
                return 0;

            return (sbyte)m_buffer[m_index++];
        }

        public bool ReadBoolean()
        {
            if ((m_index + 1) > m_length)
                return false;

            return (m_buffer[m_index++] != 0);
        }

        public string ReadUnicodeStringLE()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((m_index + 1) < m_length && (c = (m_buffer[m_index++] | (m_buffer[m_index++] << 8))) != 0)
                sb.Append((char)c);

            return sb.ToString();
        }

        public string ReadUnicodeStringLESafe(int fixedLength)
        {
            int bound = m_index + (fixedLength << 1);
            int end = bound;

            if (bound > m_length)
                bound = m_length;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((m_index + 1) < bound && (c = (m_buffer[m_index++] | (m_buffer[m_index++] << 8))) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            m_index = end;

            return sb.ToString();
        }

        public string ReadUnicodeStringLESafe()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((m_index + 1) < m_length && (c = (m_buffer[m_index++] | (m_buffer[m_index++] << 8))) != 0)
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

            while ((m_index + 1) < m_length && (c = ((m_buffer[m_index++] << 8) | m_buffer[m_index++])) != 0)
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

            while ((m_index + 1) < m_length && (c = ((m_buffer[m_index++] << 8) | m_buffer[m_index++])) != 0)
                sb.Append((char)c);

            return sb.ToString();
        }

        public bool IsSafeChar(int c)
        {
            return ((c >= 0x20 && c < 0xFFFE) || (c == 0x09));
        }

        public string ReadUTF8StringSafe(int fixedLength)
        {
            if (m_index >= m_length)
            {
                m_index += fixedLength;
                return String.Empty;
            }

            int bound = m_index + fixedLength;
            //int end   = bound;

            if (bound > m_length)
                bound = m_length;

            int count = 0;
            int index = m_index;
            int start = m_index;

            while (index < bound && m_buffer[index++] != 0)
                ++count;

            index = 0;

            byte[] buffer = new byte[count];
            int value = 0;

            while (m_index < bound && (value = m_buffer[m_index++]) != 0)
                buffer[index++] = (byte)value;

            string s = Utility.UTF8.GetString(buffer);

            bool isSafe = true;

            for (int i = 0; isSafe && i < s.Length; ++i)
                isSafe = IsSafeChar((int)s[i]);

            m_index = start + fixedLength;

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
            if (m_index >= m_length)
                return String.Empty;

            int count = 0;
            int index = m_index;

            while (index < m_length && m_buffer[index++] != 0)
                ++count;

            index = 0;

            byte[] buffer = new byte[count];
            int value = 0;

            while (m_index < m_length && (value = m_buffer[m_index++]) != 0)
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
            if (m_index >= m_length)
                return String.Empty;

            int count = 0;
            int index = m_index;

            while (index < m_length && m_buffer[index++] != 0)
                ++count;

            index = 0;

            byte[] buffer = new byte[count];
            int value = 0;

            while (m_index < m_length && (value = m_buffer[m_index++]) != 0)
                buffer[index++] = (byte)value;

            return Utility.UTF8.GetString(buffer);
        }

        public string ReadString()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while (m_index < m_length && (c = m_buffer[m_index++]) != 0)
                sb.Append((char)c);

            return sb.ToString();
        }

        public string ReadStringSafe()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while (m_index < m_length && (c = m_buffer[m_index++]) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringSafe(int fixedLength)
        {
            int bound = m_index + (fixedLength << 1);
            int end = bound;

            if (bound > m_length)
                bound = m_length;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((m_index + 1) < bound && (c = ((m_buffer[m_index++] << 8) | m_buffer[m_index++])) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            m_index = end;

            return sb.ToString();
        }

        public string ReadUnicodeStringSafeReverse()
        {
            StringBuilder sb = new StringBuilder();

            int c;

            while ((m_index + 1) < m_length && (c = ((m_buffer[m_index++]) | m_buffer[m_index++] << 8)) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            return sb.ToString();
        }

        public string ReadUnicodeStringReverse(int fixedLength)
        {
            int bound = m_index + (fixedLength << 1);
            int end = bound;

            if (bound > m_length)
                bound = m_length;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((m_index + 1) < bound && (c = ((m_buffer[m_index++]) | m_buffer[m_index++] << 8)) != 0)
                sb.Append((char)c);

            m_index = end;

            return sb.ToString();
        }

        public string ReadUnicodeString(int fixedLength)
        {
            int bound = m_index + (fixedLength << 1);
            int end = bound;

            if (bound > m_length)
                bound = m_length;

            StringBuilder sb = new StringBuilder();

            int c;

            while ((m_index + 1) < bound && (c = ((m_buffer[m_index++] << 8) | m_buffer[m_index++])) != 0)
                sb.Append((char)c);

            m_index = end;

            return sb.ToString();
        }

        public string ReadStringSafe(int fixedLength)
        {
            int bound = m_index + fixedLength;
            int end = bound;

            if (bound > m_length)
                bound = m_length;

            StringBuilder sb = new StringBuilder();

            int c;

            while (m_index < bound && (c = m_buffer[m_index++]) != 0)
            {
                if (IsSafeChar(c))
                    sb.Append((char)c);
            }

            m_index = end;

            return sb.ToString();
        }

        public string ReadString(int fixedLength)
        {
            int bound = m_index + fixedLength;
            int end = bound;

            if (bound > m_length)
                bound = m_length;

            StringBuilder sb = new StringBuilder();

            int c;

            while (m_index < bound && (c = m_buffer[m_index++]) != 0)
                sb.Append((char)c);

            m_index = end;

            return sb.ToString();
        }

        public void Dispose()
        {
            m_buffer = null;
            m_length = 0;
            m_index = 0;

            ReleaseInstance(this);
        }
    }
}
