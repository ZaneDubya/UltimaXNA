/***************************************************************************
 *   
 *   MapObjectText.cs
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
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
using UltimaXNA.UltimaData.Fonts;
#endregion

namespace UltimaXNA.UltimaWorld
{
    public class MapObjectText : AMapObject
    {
        public Texture2D Texture { get; internal set; }
        public int Hue;
        public int FontID;

        public MapObjectText(Position3D position, BaseEntity ownerEntity, string text, int hue, int fontID)
            : base(position)
        {
            
            OwnerEntity = ownerEntity;
            Hue = hue;
            FontID = fontID;

            // set up draw data
            _draw_texture = Texture = ASCIIText.GetTextTexture(text, 1);
            _draw_width = _draw_texture.Width;
            _draw_height = _draw_texture.Height;
            _draw_X = (_draw_width >> 1) - 22 - (int)((Position.X_offset - Position.Y_offset) * 22);
            _draw_Y = ((int)Position.Z_offset * 4) + _draw_height - 44 - (int)((Position.X_offset + Position.Y_offset) * 22);
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
            return string.Format("MapObjectText\n   Hue:{1} Font:{2}\n{0}", base.ToString(), Hue, FontID);
        }
    }
}
