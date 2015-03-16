using System;
using System.Text;

namespace InterXLib.FileSystem
{
    public static class CRC32
    {
        private static uint[] s_Table;

        public static uint ComputeChecksum(byte[] bytes)
        {
            uint crc = 0xffffffff;
            for (int i = 0; i < bytes.Length; i++)
            {
                byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
                crc = (uint)((crc >> 8) ^ s_Table[index]);
            }
            return ~crc;
        }

        public static uint ComputeChecksumReverse(byte[] bytes)
        {
            uint crc = 0xffffffff;
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                byte index = (byte)(((crc) & 0xff) ^ bytes[i]);
                crc = (uint)((crc >> 8) ^ s_Table[index]);
            }
            return ~crc;
        }

        public static uint ComputeChecksum(string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            return ComputeChecksum(bytes);
        }

        public static uint ComputeChecksumReverse(string str)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(str);
            return ComputeChecksumReverse(bytes);
        }

        static CRC32()
        {
            uint poly = 0xedb88320;
            s_Table = new uint[256];
            uint temp = 0;
            for (uint i = 0; i < s_Table.Length; ++i)
            {
                temp = i;
                for (int j = 8; j > 0; --j)
                {
                    if ((temp & 1) == 1)
                    {
                        temp = (uint)((temp >> 1) ^ poly);
                    }
                    else
                    {
                        temp >>= 1;
                    }
                }
                s_Table[i] = temp;
            }
        }
    }
}
