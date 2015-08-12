using System;
using System.IO;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.Resources
{
    /// <summary>
    /// This file contains information about animated effects.
    /// </summary>
    public class EffectDataResource : IResource<EffectData>
    {
        const int Count = 0x0800;
        private EffectData[][] m_AnimData;

        public EffectDataResource()
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

            m_AnimData = new EffectData[Count][];

            FileStream stream = FileManager.GetFile("animdata.mul");
            BinaryReader reader = new BinaryReader(stream);

            for (int i = 0; i < Count; i++)
            {
                EffectData[] data = new EffectData[8];
                int header = reader.ReadInt32(); // unknown value.
                for (int j = 0; j < 8; j++)
                {
                    data[j] = new EffectData(reader);
                }
                m_AnimData[i] = data;
            }
        }

        public EffectData GetResource(int itemID)
        {
            itemID &= 0x3fff;
            return m_AnimData[(itemID >> 3)][itemID & 0x07];
        }
    }
}
