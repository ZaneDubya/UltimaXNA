/***************************************************************************
 *   DebugGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Configuration;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Core.Graphics;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    public class DebugGump : Gump
    {
        private WorldModel m_World;

        public DebugGump()
            : base(0, 0)
        {
            m_World = UltimaServices.GetService<WorldModel>();

            IsMovable = true;

            AddControl(new ResizePic(this, 0, 0, 0, 0x2436, 256 + 16, 256 + 16));
            LastControl.MakeCloseTarget(this);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            base.Draw(spriteBatch);

            spriteBatch.Draw2D(((WorldView)m_World.GetView()).MiniMap.Texture, new Vector3(X + 8, Y + 8, 0), Vector3.Zero);
        }
    }
}