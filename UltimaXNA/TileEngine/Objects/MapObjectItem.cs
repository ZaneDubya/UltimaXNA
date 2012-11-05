/***************************************************************************
 *   MapObjectItem.cs
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
using UltimaXNA.Graphics;
#endregion

namespace UltimaXNA.TileEngine
{
    public class MapObjectItem : MapObject
    {
        public int Hue { get; set; }
        public int Facing { get; set; }

        public MapObjectItem(int itemID, Position3D position, int direction, BaseEntity ownerEntity, int hue)
            : base(position)
        {
            ItemID = itemID;
            OwnerEntity = ownerEntity;
            Facing = direction;
            Hue = hue;

            // set up draw data
            _draw_texture = UltimaData.Art.GetStaticTexture(ItemID);
            _draw_width = _draw_texture.Width;
            _draw_height = _draw_texture.Height;
            _draw_X = (_draw_width >> 1) - 22;
            _draw_Y = (int)(Z * 4) + _draw_height - 44;
            _draw_hue = Utility.GetHueVector(Hue);
            _pickType = PickTypes.PickObjects;
            _draw_flip = false;
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            return base.Draw(sb, drawPosition, molist, pickType, maxAlt);
        }

        public override string ToString()
        {
            return string.Format("MapObjectItem\n   ItemID:{3} Serial{4}\n   Hue:{1} Facing:{2}\n{0}", base.ToString(), Hue, Facing, ItemID, OwnerSerial);
        }
    }
}
