/***************************************************************************
 *   MapObjectText.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.TileEngine
{
    public class MapObjectText : MapObject
    {
        public Texture2D Texture { get; internal set; }
        public int Hue;
        public int FontID;

        public MapObjectText(Position3D position, Entities.Entity ownerEntity, string text, int hue, int fontID)
            : base(position)
        {
            
            OwnerEntity = ownerEntity;
            Hue = hue;
            FontID = fontID;

            // set up draw data
            _draw_texture = Texture = Data.ASCIIText.GetTextTexture(text, 1);
            _draw_width = _draw_texture.Width;
            _draw_height = _draw_texture.Height;
            _draw_X = (_draw_width >> 1) - 22 - (int)((Position.Draw_Xoffset - Position.Draw_Yoffset) * 22);
            _draw_Y = ((int)Position.Draw_Zoffset << 2) + _draw_height - 44 - (int)((Position.Draw_Xoffset + Position.Draw_Yoffset) * 22);
            _draw_hue = IsometricRenderer.GetHueVector(Hue);
            _pickType = PickTypes.PickObjects;
            _draw_flip = false;
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            return base.Draw(sb, drawPosition, molist, pickType, maxAlt);
        }
    }
}
