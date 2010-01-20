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
        public Vector3 Offset { get; internal set; }

        public MapObjectText(Vector3 position, Vector3 offset, Entities.Entity ownerEntity, string text, int hue, int fontID)
            : base(position)
        {
            Texture = Data.ASCIIText.GetTextTexture(text, 1);
            OwnerEntity = ownerEntity;
            Hue = hue;
            FontID = fontID;
            Offset = offset;
        }
    }
}
