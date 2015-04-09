using System;
using System.IO;
using System.Net;
using System.Text;

namespace UltimaXNA.IO
{
    public class BinaryFileWriter : GenericWriter
    {
        private const int LargeByteBufferSize = 256;
        private readonly bool PrefixStrings;

        private readonly byte[] m_Buffer;

        private readonly Encoding m_Encoding;
        private readonly Stream m_File;
        private readonly char[] m_SingleCharBuffer = new char[1];
        private byte[] m_CharacterBuffer;
        private int m_Index;
        private int m_MaxBufferChars;
        private long m_Position;

        public BinaryFileWriter(Stream strm, bool prefixStr)
        {
            PrefixStrings = prefixStr;
            m_Encoding = Utility.UTF8;
            m_Buffer = new byte[BufferSize];
            m_File = strm;
        }

        public BinaryFileWriter(string filename, bool prefixStr)
        {
            PrefixStrings = prefixStr;
            m_Buffer = new byte[BufferSize];
            m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            m_Encoding = Utility.UTF8WithEncoding;
        }

        protected virtual int BufferSize
        {
            get { return 64 * 1024; }
        }

        public override long Position
        {
            get { return m_Position + m_Index; }
        }

        public Stream UnderlyingStream
        {
            get
            {
                if(m_Index > 0)
                {
                    Flush();
                }

                return m_File;
            }
        }

        public void Flush()
        {
            if(m_Index > 0)
            {
                m_Position += m_Index;

                m_File.Write(m_Buffer, 0, m_Index);
                m_Index = 0;
            }
        }

        public override void Close()
        {
            if(m_Index > 0)
            {
                Flush();
            }

            m_File.Close();
        }

        public override void WriteEncodedInt(int value)
        {
            var v = (uint)value;

            while(v >= 0x80)
            {
                if((m_Index + 1) > m_Buffer.Length)
                {
                    Flush();
                }

                m_Buffer[m_Index++] = (byte)(v | 0x80);
                v >>= 7;
            }

            if((m_Index + 1) > m_Buffer.Length)
            {
                Flush();
            }

            m_Buffer[m_Index++] = (byte)v;
        }

        internal void InternalWriteString(string value)
        {
            if(value == null)
            {
                value = string.Empty;
            }
            var length = m_Encoding.GetByteCount(value);

            WriteEncodedInt(length);

            if(m_CharacterBuffer == null)
            {
                m_CharacterBuffer = new byte[LargeByteBufferSize];
                m_MaxBufferChars = LargeByteBufferSize / m_Encoding.GetMaxByteCount(1);
            }

            if(length > LargeByteBufferSize)
            {
                var current = 0;
                var charsLeft = value.Length;

                while(charsLeft > 0)
                {
                    var charCount = (charsLeft > m_MaxBufferChars) ? m_MaxBufferChars : charsLeft;
                    var byteLength = m_Encoding.GetBytes(value, current, charCount, m_CharacterBuffer, 0);

                    if((m_Index + byteLength) > m_Buffer.Length)
                    {
                        Flush();
                    }

                    Buffer.BlockCopy(m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength);
                    m_Index += byteLength;

                    current += charCount;
                    charsLeft -= charCount;
                }
            }
            else
            {
                var byteLength = m_Encoding.GetBytes(value, 0, value.Length, m_CharacterBuffer, 0);

                if((m_Index + byteLength) > m_Buffer.Length)
                {
                    Flush();
                }

                Buffer.BlockCopy(m_CharacterBuffer, 0, m_Buffer, m_Index, byteLength);
                m_Index += byteLength;
            }
        }

        public override void Write(string value)
        {
            if(PrefixStrings)
            {
                if(value == null)
                {
                    if((m_Index + 1) > m_Buffer.Length)
                    {
                        Flush();
                    }

                    m_Buffer[m_Index++] = 0;
                }
                else
                {
                    if((m_Index + 1) > m_Buffer.Length)
                    {
                        Flush();
                    }

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
            var ticks = value.Ticks;
            var now = DateTime.Now.Ticks;

            TimeSpan d;

            try
            {
                d = new TimeSpan(ticks - now);
            }
            catch
            {
                if(ticks < now)
                {
                    d = TimeSpan.MaxValue;
                }
                else
                {
                    d = TimeSpan.MaxValue;
                }
            }

            Write(d);
        }

        public override void Write(IPAddress value)
        {
            Write(Utility.GetLongAddressValue(value));
        }

        public override void Write(TimeSpan value)
        {
            Write(value.Ticks);
        }

        public override void Write(decimal value)
        {
            var bits = Decimal.GetBits(value);

            for(var i = 0; i < bits.Length; ++i)
            {
                Write(bits[i]);
            }
        }

        public override void Write(long value)
        {
            if((m_Index + 8) > m_Buffer.Length)
            {
                Flush();
            }

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
            if((m_Index + 8) > m_Buffer.Length)
            {
                Flush();
            }

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
            if((m_Index + 4) > m_Buffer.Length)
            {
                Flush();
            }

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Buffer[m_Index + 2] = (byte)(value >> 16);
            m_Buffer[m_Index + 3] = (byte)(value >> 24);
            m_Index += 4;
        }

        public override void Write(uint value)
        {
            if((m_Index + 4) > m_Buffer.Length)
            {
                Flush();
            }

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Buffer[m_Index + 2] = (byte)(value >> 16);
            m_Buffer[m_Index + 3] = (byte)(value >> 24);
            m_Index += 4;
        }

        public override void Write(short value)
        {
            if((m_Index + 2) > m_Buffer.Length)
            {
                Flush();
            }

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Index += 2;
        }

        public override void Write(ushort value)
        {
            if((m_Index + 2) > m_Buffer.Length)
            {
                Flush();
            }

            m_Buffer[m_Index] = (byte)value;
            m_Buffer[m_Index + 1] = (byte)(value >> 8);
            m_Index += 2;
        }

        public override unsafe void Write(double value)
        {
            if((m_Index + 8) > m_Buffer.Length)
            {
                Flush();
            }

            fixed(byte* pBuffer = m_Buffer)
            {
                *((double*)(pBuffer + m_Index)) = value;
            }

            m_Index += 8;
        }

        public override unsafe void Write(float value)
        {
            if((m_Index + 4) > m_Buffer.Length)
            {
                Flush();
            }

            fixed(byte* pBuffer = m_Buffer)
            {
                *((float*)(pBuffer + m_Index)) = value;
            }

            m_Index += 4;
        }

        public override void Write(char value)
        {
            if((m_Index + 8) > m_Buffer.Length)
            {
                Flush();
            }

            m_SingleCharBuffer[0] = value;

            var byteCount = m_Encoding.GetBytes(m_SingleCharBuffer, 0, 1, m_Buffer, m_Index);
            m_Index += byteCount;
        }

        public override void Write(byte value)
        {
            if((m_Index + 1) > m_Buffer.Length)
            {
                Flush();
            }

            m_Buffer[m_Index++] = value;
        }

        public override void Write(byte[] value)
        {
            for(var i = 0; i < value.Length; i++)
            {
                Write(value[i]);
            }
        }

        public override void Write(sbyte value)
        {
            if((m_Index + 1) > m_Buffer.Length)
            {
                Flush();
            }

            m_Buffer[m_Index++] = (byte)value;
        }

        public override void Write(bool value)
        {
            if((m_Index + 1) > m_Buffer.Length)
            {
                Flush();
            }

            m_Buffer[m_Index++] = (byte)(value ? 1 : 0);
        }
    }
}