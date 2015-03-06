/***************************************************************************
 *   Radarcol.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
#endregion

namespace UltimaXNA.UltimaData
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
                for (int i = 0; i < Colors.Length; i++)
                {
                    uint c = bin.ReadUInt16();
                    Colors[i] = 0xff000000 | (
                            ((((c >> 10) & 0x1F) * multiplier)) |
                            ((((c >> 5) & 0x1F) * multiplier) << 8) |
                            (((c & 0x1F) * multiplier) << 16)
                            );
                }
                Diagnostics.Metrics.ReportDataRead((int)bin.BaseStream.Position);
            }
        }
    }
}