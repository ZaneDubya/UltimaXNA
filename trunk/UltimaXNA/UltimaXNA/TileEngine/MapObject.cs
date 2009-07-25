using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Entities;

namespace UltimaXNA.TileEngine
{
    public class MapObject : IMapObject
    {
        public int ItemID { get; internal set; }
        public int Tiebreaker { get; internal set; }
        public int SortZ { get; internal set; }
        public int Z { get; internal set; }
        public Vector2 Position { get; internal set; }
        public Entity OwnerEntity { get; internal set; }
        public int Threshold { get; internal set; }

        public Serial OwnerSerial
        {
            get { return (OwnerEntity == null) ? (Serial)unchecked((int)0) : OwnerEntity.Serial; }
        }

        public MapObject(Vector2 position)
        {
            ItemID = 0;
            Tiebreaker = 0;
            SortZ = 0;
            Position = position;
            Z = 0;
            Threshold = 0;
        }
    }
}
