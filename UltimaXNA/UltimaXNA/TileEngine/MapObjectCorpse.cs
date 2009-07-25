/***************************************************************************
 *   MapObjectCorpse.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   begin                : July 24, 2009
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
    public class MapObjectCorpse : MapObjectItem
    {
        public int BodyID { get; set; }
        public int FrameIndex { get; set; }

        public MapObjectCorpse(Vector3 position, int direction, Entity ownerEntity, int nHue, int bodyID, float frame)
            : base(0x2006, position, direction, ownerEntity, nHue)
        {
            BodyID = bodyID;
            FrameIndex = (int)(frame * Data.BodyConverter.DeathAnimationFrameCount(bodyID));
        }

        public new int SortZ
        {
            get { return Z; }
        }

        public int Layer
        {
            get { return Tiebreaker; }
            set { Tiebreaker = value; }
        }
    }
}
