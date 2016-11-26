using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Compression;

namespace UltimaXNA.Ultima.World.Data {
    public class CustomHousePlane {
        public readonly int Index;
        public readonly bool IsFloor;
        public readonly byte[] ItemData;

        public CustomHousePlane(PacketReader reader) {
            byte[] data = reader.ReadBytes(4);
            Index = data[0];
            int uncompressedsize = data[1] + ((data[3] & 0xF0) << 4);
            int compressedLength = data[2] + ((data[3] & 0xF) << 8);
            ItemData = new byte[uncompressedsize];
            ZlibCompression.Unpack(ItemData, ref uncompressedsize, reader.ReadBytes(compressedLength), compressedLength);
            IsFloor = ((Index & 0x20) == 0x20);
            Index &= 0x1F;
        }
    }
}
