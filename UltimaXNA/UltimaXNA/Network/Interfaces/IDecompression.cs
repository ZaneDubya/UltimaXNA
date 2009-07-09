using System;
namespace UltimaXNA.Network
{
    public interface IDecompression
    {
        void DecompressAll(ref byte[] src, int src_size, ref byte[] dest, ref int dest_size);
        bool DecompressOnePacket(ref byte[] src, int src_size, ref byte[] dest, ref int dest_size);
    }
}
