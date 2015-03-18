﻿/***************************************************************************
 *   Shared.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.Core.Network;

namespace UltimaXNA.UltimaPackets
{
    public enum Sex
    {
        Male = 0,
        Female = 1
    }

    public enum Race
    {
        Human = 1,
        Elf = 2
    }

    public class HouseRevisionState
    {
        public Serial Serial;
        public int Hash;

        public HouseRevisionState(Serial serial, int revisionHash)
        {
            Serial = serial;
            Hash = revisionHash;
        }
    }

    public class ContentItem
    {
        public readonly Serial Serial;
        public readonly int ItemID;
        public readonly int Amount;
        public readonly int X;
        public readonly int Y;
        public readonly int GridLocation;
        public readonly Serial ContainerSerial;
        public readonly int Hue;

        public ContentItem(Serial serial, int itemId, int amount, int x, int y, int gridLocation, int containerSerial, int hue)
        {
            this.Serial = serial;
            this.ItemID = itemId;
            this.Amount = amount;
            this.X = x;
            this.Y = y;
            this.GridLocation = gridLocation;
            this.ContainerSerial = containerSerial;
            this.Hue = hue;
        }
    }

    public class StatLocks
    {
        public int Strength;
        public int Dexterity;
        public int Intelligence;

        public StatLocks(int s, int d, int i)
        {
            Strength = s;
            Dexterity = d;
            Intelligence = i;
        }
    }
}
