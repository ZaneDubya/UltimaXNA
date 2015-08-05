/***************************************************************************
 *   DebugGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    public class DebugGump : Gump
    {
        private WorldModel m_World;

        public DebugGump()
            : base(0, 0)
        {
            m_World = ServiceRegistry.GetService<WorldModel>();

            IsMoveable = true;

            AddControl(new ResizePic(this, 0, 0, 0x2436, 256 + 16, 256 + 16));
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);

            spriteBatch.Draw2D(((WorldView)m_World.GetView()).MiniMap.Texture, new Vector3(position.X + 8, position.Y + 8, 0), Vector3.Zero);
        }
    }
}