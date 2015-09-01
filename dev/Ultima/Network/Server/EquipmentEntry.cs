/***************************************************************************
 *   EquipmentEntry.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings

#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public struct EquipmentEntry
    {
        public Serial Serial;
        public ushort GumpId;
        public byte Layer;
        public ushort Hue;

        public EquipmentEntry(Serial serial, ushort gumpId, byte layer, ushort hue)
        {
            Serial = serial;
            GumpId = gumpId;
            Layer = layer;
            Hue = hue;
        }
    }
}
