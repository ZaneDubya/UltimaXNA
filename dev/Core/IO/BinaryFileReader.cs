using System;
using System.IO;
using System.Net;
using System.Text;

namespace UltimaXNA.IO
{
    public sealed class BinaryFileReader : GenericReader
    {
        private readonly BinaryReader m_File;

        public BinaryFileReader(MemoryStream stream)
        {
            m_File = new BinaryReader(stream);
        }

        public BinaryFileReader(BinaryReader br)
        {
            m_File = br;
        }

        public long Position
        {
            get { return m_File.BaseStream.Position; }
            set { m_File.BaseStream.Position = value; }
        }

        public Stream Stream
        {
            get { return m_File.BaseStream; }
        }

        public void Close()
        {
            m_File.Close();
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return m_File.BaseStream.Seek(offset, origin);
        }

        public string ReadLine()
        {
            var sb = new StringBuilder();
            var reading = true;
            while(reading && !End())
            {
                var c = this.ReadChar();
                if(c == '\n')
                {
                    reading = false;
                }
                else if(c == '\r')
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
            var ticks = m_File.ReadInt64();
            var now = DateTime.Now.Ticks;

            if(ticks > 0 && (ticks + now) < 0)
            {
                return DateTime.MaxValue;
            }
            if(ticks < 0 && (ticks + now) < 0)
            {
                return DateTime.MinValue;
            }

            try
            {
                return new DateTime(now + ticks);
            }
            catch
            {
                if(ticks > 0)
                {
                    return DateTime.MaxValue;
                }
                return DateTime.MinValue;
            }
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
            } while(b >= 0x80);

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
            var data = ReadBytes(count * 2);
            var data_out = new ushort[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 2);
            return data_out;
        }

        public int[] ReadInts(int count)
        {
            var data = ReadBytes(count * 4);
            var data_out = new int[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 4);
            return data_out;
        }

        public uint[] ReadUInts(int count)
        {
            var data = ReadBytes(count * 4);
            var data_out = new uint[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 4);
            return data_out;
        }

        public int Read7BitEncodedInt()
        {
            var value = 0;
            while(true)
            {
                var temp = ReadByte();
                value += temp & 0x7F;
                if((temp & 0x80) == 0x80)
                {
                    value = (value << 7);
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// WARNING: INCOMPLETE, ONLY READS 2-byte UTF8 chars.
        /// </summary>
        /// <returns></returns>
        public char ReadCharUTF8()
        {
            var value = 0;
            var b0 = ReadByte();
            if((b0 & 0x80) == 0x00)
            {
                value = (b0 & 0x7F);
            }
            else
            {
                value = (b0 & 0x3F);
                var b1 = ReadByte();
                if((b1 & 0xE0) == 0xC0)
                {
                    value += (b1 & 0x1F) << 6;
                }
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
}