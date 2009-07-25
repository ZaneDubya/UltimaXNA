/***************************************************************************
 *   MobileTile.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.TileEngine
{
    public class MapObjectMobile : MapObject
    {
        public int BodyID { get; internal set; }
        public int Action { get; internal set; }
        public int Facing { get; internal set; }
        public int Hue { get; internal set; }
        private float _frame;
        public Vector3 Offset { get; internal set; }

        public MapObjectMobile(int bodyID, Vector3 position, Vector3 positionOffset, int facing, int action, float frame, Entities.Entity ownerEntity, int layer, int hue)
            : base(new Vector2(position.X, position.Y))
        {
            BodyID = bodyID;
            Facing = facing;
            Action = action;
            _frame = frame;
            Hue = hue;
            Tiebreaker = layer;
            Offset = positionOffset;
            Z = (int)position.Z;
            OwnerEntity = ownerEntity;
        }

        public int Frame(int nMaxFrames)
        {
            return (int)(_frame * (float)nMaxFrames);
        }

        public new int SortZ
        {
            get { return (int)Z; }
        }
    }
}
