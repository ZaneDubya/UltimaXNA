using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network
{
    public class ContentItem
    {
        public readonly Serial serial;
        public readonly int ItemID;
        public readonly int Amount;
        public readonly int X;
        public readonly int Y;
        public readonly int GridLocation;
        public readonly int ContainerGUID;
        public readonly int Hue;

        public ContentItem(Serial serial, int itemId, int amount, int x, int y, int gridLocation, int containerGUID, int hue)
        {
            this.serial = serial;
            this.ItemID = itemId;
            this.Amount = amount;
            this.X = x;
            this.Y = y;
            this.GridLocation = gridLocation;
            this.ContainerGUID = containerGUID;
            this.Hue = hue;
        }
    }
}
