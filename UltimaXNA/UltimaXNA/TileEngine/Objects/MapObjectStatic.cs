/***************************************************************************
 *   MapObjectStatic.cs
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
    public class MapObjectStatic : MapObject
    {
        bool _noDraw = false;
        public bool NoDraw
        {  get { return (_noDraw); } }
        public Data.ItemData ItemData;

        public MapObjectStatic(int staticTileID, int sortInfluence, Position3D position)
            : base(position)
        {
            ItemID = staticTileID;
            ItemData = Data.TileData.ItemData[ItemID & 0x3FFF];

            // get no draw flag
            if (ItemData.Name == "nodraw" || ItemID <= 0)
                _noDraw = true;

            // Set sorting threshholds.
            int background = (ItemData.Background) ? 0 : 1;
            if (!ItemData.Background)
                SortThreshold++;
            if (!(ItemData.Height == 0))
                SortThreshold++;
            if (ItemData.Surface)
                SortThreshold--;
            SortTiebreaker = sortInfluence;

            // set up draw variables
            _draw_texture = Data.Art.GetStaticTexture(ItemID);
            _draw_width = _draw_texture.Width;
            _draw_height = _draw_texture.Height;
            _draw_X = (_draw_width >> 1) - 22;
            _draw_Y = (Z << 2) + _draw_height - 44;
            _draw_hue = Vector2.Zero;
            _pickType = PickTypes.PickStatics;
            _draw_flip = false;
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            if (_noDraw)
                return false;
            return base.Draw(sb, drawPosition, molist, pickType, maxAlt);
        }

        public override string ToString()
        {
            return string.Format("Static@:{0},{1},{2}, ItemID:{3}", Z, Position.X, Position.Y, ItemID);
        }
    }
}