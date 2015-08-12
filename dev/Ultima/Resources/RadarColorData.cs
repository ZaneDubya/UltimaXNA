/***************************************************************************
 *   Radarcol.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.IO;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Resources
{
    class RadarColorData
    {
        public static uint[] Colors = new uint[0x10000];

        const int multiplier = 0xFF / 0x1F;

        static RadarColorData()
        {
            using (FileStream index = new FileStream(FileManager.GetFilePath("Radarcol.mul"), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader bin = new BinaryReader(index);

                // Prior to 7.0.7.1, all clients have 0x10000 colors. Newer clients have fewer colors.
                int colorCount = (int)index.Length / 2;

                for (int i = 0; i < colorCount; i++)
                {
                    uint c = bin.ReadUInt16();
                    Colors[i] = 0xFF000000 | (
                            ((((c >> 10) & 0x1F) * multiplier)) |
                            ((((c >> 5) & 0x1F) * multiplier) << 8) |
                            (((c & 0x1F) * multiplier) << 16)
                            );
                }
                // fill the remainder of the color table with non-transparent magenta.
                for (int i = colorCount; i < Colors.Length; i++)
                {
                    Colors[i] = 0xFFFF00FF;
                }

                Metrics.ReportDataRead((int)bin.BaseStream.Position);
            }
        }
    }
}