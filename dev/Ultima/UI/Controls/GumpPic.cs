/***************************************************************************
 *   GumpPic.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources;

namespace UltimaXNA.Ultima.UI.Controls
{
    class GumpPic : AGumpPic
    {
        public GumpPic(AControl parent, string[] arguements)
            : base(parent)
        {
            int x, y, gumpID, hue = 0;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            gumpID = Int32.Parse(arguements[3]);
            if (arguements.Length > 4)
            {
                // has a HUE=XXX arguement (and potentially a CLASS=XXX argument).
                string hueArgument = arguements[4].Substring(arguements[4].IndexOf('=') + 1);
                hue = Int32.Parse(hueArgument);
            }
            buildGumpling(x, y, gumpID, hue);
        }

        public GumpPic(AControl parent, int x, int y, int gumpID, int hue)
            : base(parent)
        {
            buildGumpling(x, y, gumpID, hue);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            Vector3 hueVector = Utility.GetHueVector(Hue);
            spriteBatch.Draw2D(m_Texture, new Vector3(position.X, position.Y, 0), hueVector);
            base.Draw(spriteBatch, position);
        }

        
    }
}
