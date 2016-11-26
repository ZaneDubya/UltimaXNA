using System;

namespace UltimaXNA.Core.Network
{

    class PacketChunk
    {
        readonly byte[] m_Buffer;
        int m_Length;

        public PacketChunk(byte[] buffer)
        {
            m_Buffer = buffer;
        }

        public int Length
        {
            get { return m_Length; }
        }

        public void Write(byte[] source, int offset, int length)
        {
            Buffer.BlockCopy(source, offset, m_Buffer, m_Length, length);

            m_Length += length;
        }

        public void Prepend(byte[] dest, int length)
        {
            // Offset the intial buffer by the amount we need to prepend
            if (length > 0)
            {
                Buffer.BlockCopy(dest, 0, dest, m_Length, length);
            }

            // Prepend the buffer to the destination buffer
            Buffer.BlockCopy(m_Buffer, 0, dest, 0, m_Length);
        }

        public void Clear()
        {
            m_Length = 0;
        }
    }
}
