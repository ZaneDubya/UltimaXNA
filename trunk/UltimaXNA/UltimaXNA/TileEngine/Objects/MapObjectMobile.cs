/***************************************************************************
 *   MapObjectMobile.cs
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
        public int BodyID { get; set; }
        public int Action { get; set; }
        public int Facing { get; set; }
        public int Hue { get; set; }
        private float _frame;

        public MapObjectMobile(int bodyID, Position3D position, int facing, int action, float frame, Entities.Entity ownerEntity, int layer, int hue)
            : base(position)
        {
            BodyID = bodyID;
            Facing = facing;
            Action = action;
            if (_frame >= 1.0f)
                return;
            _frame = frame;
            Hue = hue;
            Tiebreaker = layer;
            OwnerEntity = ownerEntity;
        }

        public int Frame(int nMaxFrames)
        {
            return (int)(_frame * (float)nMaxFrames);
        }
    }
}