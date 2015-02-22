/***************************************************************************
 *                             Serialization.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: Serialization.cs 644 2010-12-23 09:18:45Z asayre $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace InterXLib
{
    public static class Serialize
    {
        public static BinaryFileReader OpenReader(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            BinaryFileReader reader = new BinaryFileReader(br);
            return reader;
        }

        public static BinaryFileWriter OpenWriter(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Create);
            BinaryFileWriter writer = new BinaryFileWriter(fs, false);
            return writer;
        }
    }

    public abstract class GenericReader
    {
        protected GenericReader() { }

        public abstract string ReadString();
        public abstract DateTime ReadDateTime();
        public abstract TimeSpan ReadTimeSpan();
        public abstract DateTime ReadDeltaTime();
        public abstract decimal ReadDecimal();
        public abstract long ReadLong();
        public abstract ulong ReadULong();
        public abstract int ReadInt();
        public abstract uint ReadUInt();
        public abstract short ReadShort();
        public abstract ushort ReadUShort();
        public abstract double ReadDouble();
        public abstract float ReadFloat();
        public abstract char ReadChar();
        public abstract byte ReadByte();
        public abstract sbyte ReadSByte();
        public abstract bool ReadBool();
        public abstract int ReadEncodedInt();
        public abstract IPAddress ReadIPAddress();

        public abstract bool End();
    }

    public abstract class GenericWriter
    {
        protected GenericWriter() { }

        public abstract void Close();

        public abstract long Position { get; }

        public abstract void Write(string value);
        public abstract void Write(DateTime value);
        public abstract void Write(TimeSpan value);
        public abstract void Write(decimal value);
        public abstract void Write(long value);
        public abstract void Write(ulong value);
        public abstract void Write(int value);
        public abstract void Write(uint value);
        public abstract void Write(short value);
        public abstract void Write(ushort value);
        public abstract void Write(double value);
        public abstract void Write(float value);
        public abstract void Write(char value);
        public abstract void Write(byte value);
        public abstract void Write(byte[] value);
        public abstract void Write(sbyte value);
        public abstract void Write(bool value);
        public abstract void WriteEncodedInt(int value);
        public abstract void Write(IPAddress value);

        public abstract void WriteDeltaTime(DateTime value);

        //Stupid compiler won't notice there 'where' to differentiate the generic methods.
    }

    public class BinaryFileWriter : GenericWriter
    {
        private bool PrefixStrings;
        private Stream m_File;

        protected virtual int BufferSize
        {
            get
            {
                return 64 * 1024;
            }
        }

        private byte[] m_Buffer;

        private int m_Index;

        private Encoding m_Encoding;

        public BinaryFileWriter(Stream strm, bool prefixStr)
        {
            PrefixStrings = prefixStr;
            m_Encoding = Library.UTF8;
            m_Buffer = new byte[BufferSize];
            m_File = strm;
        }

        public BinaryFileWriter(string filename, bool prefixStr)
        {
            PrefixStrings = prefixStr;
            m_Buffer = new byte[BufferSize];
            m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            m_Encoding = Library.UTF8WithEncoding;
        }

        public void Flush()
        {
            if (m_Index > 0)
            {
                m_Position += m_Index;

                m_File.Write(m_Buffer, 0, m_Index);
                m_Index = 0;
            }
        }

        private long m_Position;

        public override long Position
        {
            get
            {
                return m_Position + m_Index;
            }
        }

        public Stream UnderlyingStream
        {
            get
            {
                if (m_Index > 0)
                    Flush();

                return m_File;
            }
        }

        public override void Close()
        {
            if (m_Index > 0)
                Flush();

            m_File.Close();
        }

        public override void WriteEncodedInt(int value)
        {
            uint v = (uint)value;

            while (v >= 0x80)
            {
                if ((m_Index + 1) > m_Buffer.Length)
                    Flush();

                m_Buffer[m_Index++] = (byte)(v | 0x80);
                v >>= 7;
            }

            if ((m_Index + 1) > m_Buffer.Length)
                Flush();

            m_Buffer[m_Index++] = (byte)v;
        }

        private byte[] m_CharacterBuffer;
        private int m_MaxBufferChars;
        private const int LargeByteBufferSize = 256;

        internal void InternalWriteString(string value)
        {
            if (value == null)
                value = string.Empty;
            int length = m_Encoding.GetByteCount(value);

            WriteEncodedInt(length);

            if (m_CharacterBuffer == null)
            {
                m_CharacterBuffer = new byte[LargeByteBufferSize];
                m_MaxBufferChars = LargeByteBufferSize / m_Encoding.GetMaxByteCount(1);
            }

            if (length > LargeByteBufferSize)
            {
                int current = 0;
                int charsLeft = value.Length;

                while (charsLeft > 0)
                {
                    int charCount = (charsLeft > m_MaxBufferChars) ? m_MaxBufferChars : charsLeft;
                    int byteLength = m_Encoding.GetBytes(value, current, charCount, m_CharacterBuffer, 0);

                    if ((m_Index + byteLength) > m_Buffer.Length)
                        Flush();

                    Buffer.BlockCopy(m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength);
                    m_Index += byteLength;

                    current += charCount;
                    charsLeft -= charCount;
                }
            }
            else
            {
                int byteLength = m_Encoding.GetBytes(value, 0, value.Length, m_CharacterBuffer, 0);

                if ((m_Index + byteLength) > m_Buffer.Length)
                    Flush();

                Buffer.BlockCopy(m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength);
                m_Index += byteLength;
            }
        }

        public override void Write(string value)
        {
            if (PrefixStrings)
            {
                if (value == null)
                {
                    if ((m_Index + 1) > m_Buffer.Length)
                        Flush();

                    m_Buffer[m_Index++] = 0;
                }
                else
                {
                    if ((m_Index + 1) > m_Buffer.Length)
                        Flush();

                    m_Buffer[m_Index++] = 1;

                    InternalWriteString(value);
                }
            }
            else
            {
                InternalWriteString(value);
            }
        }

        public override void Write(DateTime value)
        {
            Write(value.Ticks);
        }

        public override void WriteDeltaTime(DateTime value)
        {
            long ticks = value.Ticks;
            long now = DateTime.Now.Ticks;

            TimeSpan d;

            try { d = new TimeSpan(ticks - now); }
            catch { if (ticks < now) d = TimeSpan.MaxValue; else d = TimeSpan.MaxValue; }

            Write(d);
        }

        public override void Write(IPAddress value)
        {
            Write(Library.GetLongAddressValue(value));
        }

        public override void Write(TimeSpan value)
        {
            Write(value.Ticks);
        }

        public override void Write(decimal value)
        {
            int[] bits = Decimal.GetBits(value);

            for (int i = 0; i < bits.Length; ++i)
                Write(bits[i]);
        }

        public override void Write(long value)
        {
            if ((m_Index + 8) > m_Buffer.Length)
                Flush();

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Buffer[m_Index + 2] = (byte)(value >> 16);
            m_Buffer[m_Index + 3] = (byte)(value >> 24);
            m_Buffer[m_Index + 4] = (byte)(value >> 32);
            m_Buffer[m_Index + 5] = (byte)(value >> 40);
            m_Buffer[m_Index + 6] = (byte)(value >> 48);
            m_Buffer[m_Index + 7] = (byte)(value >> 56);
            m_Index += 8;
        }

        public override void Write(ulong value)
        {
            if ((m_Index + 8) > m_Buffer.Length)
                Flush();

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Buffer[m_Index + 2] = (byte)(value >> 16);
            m_Buffer[m_Index + 3] = (byte)(value >> 24);
            m_Buffer[m_Index + 4] = (byte)(value >> 32);
            m_Buffer[m_Index + 5] = (byte)(value >> 40);
            m_Buffer[m_Index + 6] = (byte)(value >> 48);
            m_Buffer[m_Index + 7] = (byte)(value >> 56);
            m_Index += 8;
        }

        public override void Write(int value)
        {
            if ((m_Index + 4) > m_Buffer.Length)
                Flush();

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Buffer[m_Index + 2] = (byte)(value >> 16);
            m_Buffer[m_Index + 3] = (byte)(value >> 24);
            m_Index += 4;
        }

        public override void Write(uint value)
        {
            if ((m_Index + 4) > m_Buffer.Length)
                Flush();

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Buffer[m_Index + 2] = (byte)(value >> 16);
            m_Buffer[m_Index + 3] = (byte)(value >> 24);
            m_Index += 4;
        }

        public override void Write(short value)
        {
            if ((m_Index + 2) > m_Buffer.Length)
                Flush();

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Index += 2;
        }

        public override void Write(ushort value)
        {
            if ((m_Index + 2) > m_Buffer.Length)
                Flush();

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Index += 2;
        }

        public unsafe override void Write(double value)
        {
            if ((m_Index + 8) > m_Buffer.Length)
                Flush();

            fixed (byte* pBuffer = m_Buffer)
                *((double*)(pBuffer + m_Index)) = value;

            m_Index += 8;
        }

        public unsafe override void Write(float value)
        {
            if ((m_Index + 4) > m_Buffer.Length)
                Flush();

            fixed (byte* pBuffer = m_Buffer)
                *((float*)(pBuffer + m_Index)) = value;

            m_Index += 4;
        }

        private char[] m_SingleCharBuffer = new char[1];

        public override void Write(char value)
        {
            if ((m_Index + 8) > m_Buffer.Length)
                Flush();

            m_SingleCharBuffer[0] = value;

            int byteCount = m_Encoding.GetBytes(m_SingleCharBuffer, 0, 1, m_Buffer, m_Index);
            m_Index += byteCount;
        }

        public override void Write(byte value)
        {
            if ((m_Index + 1) > m_Buffer.Length)
                Flush();

            m_Buffer[m_Index++] = value;
        }

        public override void Write(byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
                Write(value[i]);
        }

        public override void Write(sbyte value)
        {
            if ((m_Index + 1) > m_Buffer.Length)
                Flush();

            m_Buffer[m_Index++] = (byte)value;
        }

        public override void Write(bool value)
        {
            if ((m_Index + 1) > m_Buffer.Length)
                Flush();

            m_Buffer[m_Index++] = (byte)(value ? 1 : 0);
        }
    }

    public sealed class BinaryFileReader : GenericReader
    {
        private BinaryReader m_File;

        public BinaryFileReader(MemoryStream stream)
        {
            m_File = new BinaryReader(stream);
        }

        public BinaryFileReader(BinaryReader br)
        {
            m_File = br;
        }

        public void Close()
        {
            m_File.Close();
        }

        public long Position
        {
            get
            {
                return m_File.BaseStream.Position;
            }
            set
            {
                m_File.BaseStream.Position = value;
            }
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return m_File.BaseStream.Seek(offset, origin);
        }

        public Stream Stream
        {
            get
            {
                return m_File.BaseStream;
            }
        }

        public string ReadLine()
        {
            StringBuilder sb = new StringBuilder();
            bool reading = true;
            while (reading && !End())
            {
                Char c = this.ReadChar();
                if (c == '\n')
                {
                    reading = false;
                }
                else if (c == '\r')
                {
                    // discard
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public override string ReadString()
        {
            return m_File.ReadString();
        }

        public override DateTime ReadDeltaTime()
        {
            long ticks = m_File.ReadInt64();
            long now = DateTime.Now.Ticks;

            if (ticks > 0 && (ticks + now) < 0)
                return DateTime.MaxValue;
            else if (ticks < 0 && (ticks + now) < 0)
                return DateTime.MinValue;

            try { return new DateTime(now + ticks); }
            catch { if (ticks > 0) return DateTime.MaxValue; else return DateTime.MinValue; }
        }

        public override IPAddress ReadIPAddress()
        {
            return new IPAddress(m_File.ReadInt64());
        }

        public override int ReadEncodedInt()
        {
            int v = 0, shift = 0;
            byte b;

            do
            {
                b = m_File.ReadByte();
                v |= (b & 0x7F) << shift;
                shift += 7;
            } while (b >= 0x80);

            return v;
        }

        public override DateTime ReadDateTime()
        {
            return new DateTime(m_File.ReadInt64());
        }

        public override TimeSpan ReadTimeSpan()
        {
            return new TimeSpan(m_File.ReadInt64());
        }

        public override decimal ReadDecimal()
        {
            return m_File.ReadDecimal();
        }

        public override long ReadLong()
        {
            return m_File.ReadInt64();
        }

        public override ulong ReadULong()
        {
            return m_File.ReadUInt64();
        }

        public override int ReadInt()
        {
            return m_File.ReadInt32();
        }

        public override uint ReadUInt()
        {
            return m_File.ReadUInt32();
        }

        public override short ReadShort()
        {
            return m_File.ReadInt16();
        }

        public override ushort ReadUShort()
        {
            return m_File.ReadUInt16();
        }

        public override double ReadDouble()
        {
            return m_File.ReadDouble();
        }

        public override float ReadFloat()
        {
            return m_File.ReadSingle();
        }

        public override char ReadChar()
        {
            return m_File.ReadChar();
        }

        public override byte ReadByte()
        {
            return m_File.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return m_File.ReadBytes(count);
        }

        public ushort[] ReadUShorts(int count)
        {
            byte[] data = ReadBytes(count * 2);
            ushort[] data_out = new ushort[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 2);
            return data_out;
        }

        public uint[] ReadUInts(int count)
        {
            byte[] data = ReadBytes(count * 4);
            uint[] data_out = new uint[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 4);
            return data_out;
        }

        public int Read7BitEncodedInt()
        {
            int value = 0;
            while (true)
            {
                byte temp = ReadByte();
                value += temp & 0x7F;
                if ((temp & 0x80) == 0x80)
                    value = (value << 7);
                else
                    return value;
            }
        }

        /// <summary>
        /// WARNING: INCOMPLETE, ONLY READS 2-byte UTF8 chars.
        /// </summary>
        /// <returns></returns>
        public char ReadCharUTF8()
        {
            int value = 0;
            byte b0 = ReadByte();
            if ((b0 & 0x80) == 0x00)
                value = (b0 & 0x7F);
            else
            {
                value = (b0 & 0x3F);
                byte b1 = ReadByte();
                if ((b1 & 0xE0) == 0xC0)
                    value += (b1 & 0x1F) << 6;
            }
            return (char)value;
        }

        public override sbyte ReadSByte()
        {
            return m_File.ReadSByte();
        }

        public override bool ReadBool()
        {
            return m_File.ReadBoolean();
        }

        public override bool End()
        {
            return m_File.PeekChar() == -1;
        }
    }

    public sealed class AsyncWriter : GenericWriter
    {
        private static int m_ThreadCount = 0;
        public static int ThreadCount { get { return m_ThreadCount; } }


        private int BufferSize;

        private long m_LastPos, m_CurPos;
        private bool m_Closed;
        private bool PrefixStrings;

        private MemoryStream m_Mem;
        private BinaryWriter m_Bin;
        private FileStream m_File;

        private Queue m_WriteQueue;
        private Thread m_WorkerThread;

        public AsyncWriter(string filename, bool prefix)
            : this(filename, 1048576, prefix)//1 mb buffer
        {
        }

        public AsyncWriter(string filename, int buffSize, bool prefix)
        {
            PrefixStrings = prefix;
            m_Closed = false;
            m_WriteQueue = Queue.Synchronized(new Queue());
            BufferSize = buffSize;

            m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            m_Mem = new MemoryStream(BufferSize + 1024);
            m_Bin = new BinaryWriter(m_Mem, Library.UTF8WithEncoding);
        }

        private void Enqueue(MemoryStream mem)
        {
            m_WriteQueue.Enqueue(mem);

            if (m_WorkerThread == null || !m_WorkerThread.IsAlive)
            {
                m_WorkerThread = new Thread(new ThreadStart(new WorkerThread(this).Worker));
                m_WorkerThread.Priority = ThreadPriority.BelowNormal;
                m_WorkerThread.Start();
            }
        }

        private class WorkerThread
        {
            private AsyncWriter m_Owner;

            public WorkerThread(AsyncWriter owner)
            {
                m_Owner = owner;
            }

            public void Worker()
            {
                AsyncWriter.m_ThreadCount++;
                while (m_Owner.m_WriteQueue.Count > 0)
                {
                    MemoryStream mem = (MemoryStream)m_Owner.m_WriteQueue.Dequeue();

                    if (mem != null && mem.Length > 0)
                        mem.WriteTo(m_Owner.m_File);
                }

                if (m_Owner.m_Closed)
                    m_Owner.m_File.Close();

                AsyncWriter.m_ThreadCount--;

                if (AsyncWriter.m_ThreadCount <= 0)
                {
                    // Program.NotifyDiskWriteComplete();
                }
            }
        }

        private void OnWrite()
        {
            long curlen = m_Mem.Length;
            m_CurPos += curlen - m_LastPos;
            m_LastPos = curlen;
            if (curlen >= BufferSize)
            {
                Enqueue(m_Mem);
                m_Mem = new MemoryStream(BufferSize + 1024);
                m_Bin = new BinaryWriter(m_Mem, Library.UTF8WithEncoding);
                m_LastPos = 0;
            }
        }

        public MemoryStream MemStream
        {
            get
            {
                return m_Mem;
            }
            set
            {
                if (m_Mem.Length > 0)
                    Enqueue(m_Mem);

                m_Mem = value;
                m_Bin = new BinaryWriter(m_Mem, Library.UTF8WithEncoding);
                m_LastPos = 0;
                m_CurPos = m_Mem.Length;
                m_Mem.Seek(0, SeekOrigin.End);
            }
        }

        public override void Close()
        {
            Enqueue(m_Mem);
            m_Closed = true;
        }

        public override long Position
        {
            get
            {
                return m_CurPos;
            }
        }

        public override void Write(IPAddress value)
        {
            m_Bin.Write(Library.GetLongAddressValue(value));
            OnWrite();
        }

        public override void Write(string value)
        {
            if (PrefixStrings)
            {
                if (value == null)
                {
                    m_Bin.Write((byte)0);
                }
                else
                {
                    m_Bin.Write((byte)1);
                    m_Bin.Write(value);
                }
            }
            else
            {
                m_Bin.Write(value);
            }
            OnWrite();
        }

        public override void WriteDeltaTime(DateTime value)
        {
            long ticks = value.Ticks;
            long now = DateTime.Now.Ticks;

            TimeSpan d;

            try { d = new TimeSpan(ticks - now); }
            catch { if (ticks < now) d = TimeSpan.MaxValue; else d = TimeSpan.MaxValue; }

            Write(d);
        }

        public override void Write(DateTime value)
        {
            m_Bin.Write(value.Ticks);
            OnWrite();
        }

        public override void Write(TimeSpan value)
        {
            m_Bin.Write(value.Ticks);
            OnWrite();
        }

        public override void Write(decimal value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(long value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(ulong value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void WriteEncodedInt(int value)
        {
            uint v = (uint)value;

            while (v >= 0x80)
            {
                m_Bin.Write((byte)(v | 0x80));
                v >>= 7;
            }

            m_Bin.Write((byte)v);
            OnWrite();
        }

        public override void Write(int value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(uint value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(short value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(ushort value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(double value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(float value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(char value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(byte value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
                m_Bin.Write(value[i]);
            OnWrite();
        }

        public override void Write(sbyte value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(bool value)
        {
            m_Bin.Write(value);
            OnWrite();
        }
    }

    public interface ISerializable
    {
        int TypeReference { get; }
        int SerialIdentity { get; }
        void Serialize(GenericWriter writer);
    }
}