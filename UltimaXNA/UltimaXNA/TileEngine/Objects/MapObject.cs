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
        private Vector3 _position;
        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                Z = (int)_position.Z;
                SortZ = (int)Position.Z;
            }
        }
        public int ItemID = 0;
        public int Tiebreaker = 0;
        public Entity OwnerEntity = null;
        public int Threshold = 0;
        public int SortZ = 0;
        public int Z = 0;

        public Serial OwnerSerial
        {
            get { return (OwnerEntity == null) ? (Serial)unchecked((int)0) : OwnerEntity.Serial; }
        }

        public MapObject(Vector3 position)
        {
            Position = position;
        }
    }
}
