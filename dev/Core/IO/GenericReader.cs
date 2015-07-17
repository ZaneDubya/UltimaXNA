#region usings
using System;
using System.Net;
#endregion

namespace UltimaXNA.Core.IO
{
    public abstract class GenericReader
    {
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
}