using System;
using System.Net;

namespace UltimaXNA.Core.IO
{
    public abstract class GenericWriter
    {
        public abstract long Position
        {
            get;
        }

        public abstract void Close();

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
}