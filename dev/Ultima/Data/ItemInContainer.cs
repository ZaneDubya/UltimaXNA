/***************************************************************************
 *   ContentItem.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Ultima.Data
{
    public class ItemInContainer
    {
        public readonly Serial Serial;
        public readonly int ItemID;
        public readonly int Amount;
        public readonly int X;
        public readonly int Y;
        public readonly int GridLocation;
        public readonly Serial ContainerSerial;
        public readonly int Hue;

        public ItemInContainer(Serial serial, int itemId, int amount, int x, int y, int gridLocation, int containerSerial, int hue)
        {
            Serial = serial;
            ItemID = itemId;
            Amount = amount;
            X = x;
            Y = y;
            GridLocation = gridLocation;
            ContainerSerial = containerSerial;
            Hue = hue;
        }
    }
}
