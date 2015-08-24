/***************************************************************************
 *   StaticTile.cs
 *   Based on code from UltimaSDK: http://ultimasdk.codeplex.com/
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace UltimaXNA.Ultima.Resources
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct StaticTile : IComparable<StaticTile>
    {
        public short ID;
        public byte X;
        public byte Y;
        public sbyte Z;
        public short Hue;

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("ID: " + ID.ToString());
            stringBuilder.AppendLine("X: " + X.ToString());
            stringBuilder.AppendLine("Y: " + Y.ToString());
            stringBuilder.AppendLine("Z: " + Z.ToString());
            stringBuilder.AppendLine("Hue: " + Hue.ToString());

            return stringBuilder.ToString();
        }

        public int CompareTo(StaticTile t)
        {
            return (Z - t.Z);
        }
    }
}
