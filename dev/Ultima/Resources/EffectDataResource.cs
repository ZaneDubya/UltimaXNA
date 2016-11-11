/***************************************************************************
 *   EffectDataResource.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.IO;
using UltimaXNA.Core.Resources;
using UltimaXNA.Ultima.IO;
#endregion

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
            itemID &= FileManager.ItemIDMask;
            return m_AnimData[(itemID >> 3)][itemID & 0x07];
        }
    }
}
