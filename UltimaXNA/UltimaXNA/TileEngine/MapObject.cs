using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Entities;

namespace UltimaXNA.TileEngine
{
    public class MapObject
    {
        public Vector3 Position;
        public int ItemID { get; set; }
        public int Tiebreaker { get; set; }
        public Entity OwnerEntity { get; set; }
        public virtual int Threshold { get; set; }
        public int SortZ { get; set; }
        public int Z { get { return (int)Position.Z; } }

        public Serial OwnerSerial
        {
            get { return (OwnerEntity == null) ? (Serial)unchecked((int)0) : OwnerEntity.Serial; }
        }

        public MapObject(Vector3 position)
        {
            ItemID = 0;
            Tiebreaker = 0;
            Position = position;
            SortZ = (int)Position.Z;
            Threshold = 0;
        }
    }
}
