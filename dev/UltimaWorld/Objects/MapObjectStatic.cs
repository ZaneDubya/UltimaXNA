/***************************************************************************
 *   MapObjectStatic.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
#endregion

namespace UltimaXNA.UltimaWorld
{
    public class MapObjectStatic : AMapObject
    {
        bool _noDraw = false;
        public bool NoDraw
        {
            get { return (_noDraw); }
        }

        UltimaData.ItemData _itemData;
        public UltimaData.ItemData ItemData
        {
            get { return _itemData; }
        }

        public MapObjectStatic(int staticTileID, int sortInfluence, Position3D position)
            : base(position)
        {
            ItemID = staticTileID;
            SortTiebreaker = sortInfluence;

            _itemData = UltimaData.TileData.ItemData[ItemID & 0x3FFF];

            // Set threshold.
            int background = (_itemData.Background) ? 0 : 1;
            if (!_itemData.Background)
                SortThreshold++;
            if (!(_itemData.Height == 0))
                SortThreshold++;
            if (_itemData.Surface)
                SortThreshold--;

            // get no draw flag
            if (_itemData.Name == "nodraw" || ItemID <= 0)
                _noDraw = true;

            // set up draw variables
            _draw_texture = UltimaData.ArtData.GetStaticTexture(ItemID);
            _draw_width = _draw_texture.Width;
            _draw_height = _draw_texture.Height;
            _draw_X = (_draw_width >> 1) - 22;
            _draw_Y = (int)(Z * 4) + _draw_height - 44;
            _draw_hue = Vector2.Zero;
            _pickType = PickTypes.PickStatics;
            _draw_flip = false;
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            _draw_Y = (int)(Z * 4) + _draw_height - 44;
            if (_noDraw)
                return false;
            return base.Draw(sb, drawPosition, molist, pickType, maxAlt);
        }

        public override string ToString()
        {
            return string.Format("MapObjectStatic\n   ItemID:{1}\n{0}", base.ToString(), ItemID);
        }
    }
}