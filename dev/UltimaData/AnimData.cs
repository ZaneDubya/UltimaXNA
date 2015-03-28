using System.IO;

namespace UltimaXNA.UltimaData
{
    /// <summary>
    /// This file contains information about animated statics.
    /// </summary>
    public class AnimData
    {
        const int Count = 0x0800;
        static AnimDataEntry[][] m_AnimData;
        public static AnimDataEntry[] GetAnimData(int index)
        {
            return m_AnimData[index & 0x07FF];
        }

        public static void Initialize()
        {
            // From http://wpdev.sourceforge.net/docs/guide/node167.html:
            // There are 2048 blocks, 8 entries per block, 68 bytes per entry.
            // Thanks to Krrios for figuring out the blocksizes.
            // Each block has an 4 byte header which is currently unknown. The
            // entries correspond with the Static ID. You can lookup an entry
            // for a given static with this formula:
            // Offset = (id>>3)*548+(id&15)*68+4;
            // Here is the record format for each entry:
            // byte[64] Frames
            // byte     Unknown
            // byte     Number of Frames Used
            // byte     Frame Interval
            // byte     Start Interval

            m_AnimData = new AnimDataEntry[Count][];

            FileStream stream = FileManager.GetFile("animdata.mul");
            BinaryReader reader = new BinaryReader(stream);

            for (int i = 0; i < Count; i++)
            {
                AnimDataEntry[] data = new AnimDataEntry[8];
                int header = reader.ReadInt32(); // unknown value.
                for (int j = 0; j < 8; j++)
                {
                    data[j] = new AnimDataEntry(reader);
                }
                m_AnimData[i] = data;
            }
        }

        public struct AnimDataEntry
        {
            public byte[] Frames;
            private byte m_Unknown;
            public byte FrameCount;
            public byte FrameInterval;
            public byte StartInterval;

            public AnimDataEntry(BinaryReader reader)
            {
                Frames = reader.ReadBytes(0x40);
                m_Unknown = reader.ReadByte();
                FrameCount = reader.ReadByte();
                FrameInterval = reader.ReadByte();
                StartInterval = reader.ReadByte();
            }
        }
    }
}
