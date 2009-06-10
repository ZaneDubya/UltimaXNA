#region File Description & Usings
//-----------------------------------------------------------------------------
// Packet.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.IO;
#endregion

namespace UltimaXNA.Network
{
    public class Packet
    {
        private MemoryStream m_MemoryStream;
        private MiscUtil.IO.EndianBinaryReader m_Reader;
        private MiscUtil.IO.EndianBinaryWriter m_Writer;
        public OpCodes OpCode;

        public MemoryStream Stream
        {
            get { return m_MemoryStream; }
        }

        public Packet(OpCodes nOpCode)
        {
            m_MemoryStream = new MemoryStream();
            m_Reader = new MiscUtil.IO.EndianBinaryReader(MiscUtil.Conversion.EndianBitConverter.Big, m_MemoryStream);
            m_Writer = new MiscUtil.IO.EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Big, m_MemoryStream);
            if (nOpCode != OpCodes.CMSG_SEED)
                this.Write((byte)nOpCode);
            this.OpCode = nOpCode;
        }

        public Packet(byte[] nData)
        {
            // When we receive data from the server.
            m_MemoryStream = new MemoryStream();
            m_Reader = new MiscUtil.IO.EndianBinaryReader(MiscUtil.Conversion.EndianBitConverter.Big, m_MemoryStream);
            m_Writer = new MiscUtil.IO.EndianBinaryWriter(MiscUtil.Conversion.EndianBitConverter.Big, m_MemoryStream);
            m_MemoryStream.Position = 0;
            m_MemoryStream.Write(nData, 0, nData.Length);
            m_MemoryStream.Position = 0;
            this.OpCode = (OpCodes)m_Reader.ReadByte();
        }

        public int Position
        {
            set { m_MemoryStream.Position = value; }
            get { return (int)m_MemoryStream.Position; }
        }

        // Implement the Reader properties ...
        public bool ReadBool() { return m_Reader.ReadBoolean(); }
        public byte ReadByte() { return m_Reader.ReadByte(); }
        public byte[] ReadBytes(int nCount) { return m_Reader.ReadBytes(nCount); }
        public char ReadChar() { return m_Reader.ReadChar(); }
        public char[] ReadChars(int nCount) { return m_Reader.ReadChars(nCount); }
        public decimal ReadDecimal() { return m_Reader.ReadDecimal(); }
        public double ReadDouble() { return m_Reader.ReadDouble(); }
        public float ReadSingle() { return m_Reader.ReadSingle(); }
        public int ReadInt() { return m_Reader.ReadInt32(); }
        public long ReadLong() { return m_Reader.ReadInt64(); }
        public sbyte ReadSByte() { return m_Reader.ReadSByte(); }
        public short ReadShort() { return m_Reader.ReadInt16(); }
        public string ReadString() { return m_Reader.ReadString(); }
        public uint ReadUInt() { return m_Reader.ReadUInt32(); }
        public ulong ReadULong() { return m_Reader.ReadUInt64(); }
        public ushort ReadUShort() { return m_Reader.ReadUInt16(); }

        private static void m_SwapEndian()
        {
            long[] array = new long[] { 0x0102030405060708, 0x1122334455667788 };

            foreach (long n in array)
            {
                Console.WriteLine(n.ToString("x"));
            }
        }

        // Implement the Writer properties ...
        public void Write(bool value)                           { m_Writer.Write(value); }
        public void Write(byte value)                           { m_Writer.Write(value); }
        public void Write(byte[] value)                         { m_Writer.Write(value); }
        public void Write(char value)                           { m_Writer.Write(value); }
        public void Write(char[] value)                         { m_Writer.Write(value); }
        public void Write(decimal value)                        { m_Writer.Write(value); }
        public void Write(double value)                         { m_Writer.Write(value); }
        public void Write(float value)                          { m_Writer.Write(value); }
        public void Write(int value)                            { m_Writer.Write(value); }
        public void Write(long value)                           { m_Writer.Write(value); }
        public void Write(sbyte value)                          { m_Writer.Write(value); }
        public void Write(short value)                          { m_Writer.Write(value); }
        public void Write(string value)                         { m_Writer.Write(value); }
        public void Write(uint value)                           { m_Writer.Write(value); }
        public void Write(ulong value)                          { m_Writer.Write(value); }
        public void Write(ushort value)                         { m_Writer.Write(value); }
        public void Write(byte[] buffer, int index, int count)  { m_Writer.Write(buffer, index, count); }
        // public void Write(char[] buffer, int index, int count)  { m_Writer.Write(buffer, index, count); }
    }
}
