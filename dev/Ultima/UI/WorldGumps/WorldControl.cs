/***************************************************************************
 *   WorldControl.cs
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
using System.Collections.Generic;
using System.Text;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Core.Input;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World;
using UltimaXNA.Core.UI;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    class WorldControl : AControl
    {
        private WorldModel m_Model;

        public Point MousePosition;

        private Vector2 m_InputMultiplier = Vector2.One;

        public WorldControl(AControl owner, int x, int y, int width, int height)
            : base(owner)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);

            HandlesMouseInput = true;
            ServiceRegistry.Register<WorldControl>(this);
        }

        protected override void OnInitialize()
        {
            m_Model = ServiceRegistry.GetService<WorldModel>();
        }

        public override void Dispose()
        {
            ServiceRegistry.Unregister<WorldControl>();
            base.Dispose();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            Texture2D worldTexture = (m_Model.GetView() as WorldView).Isometric.Texture;
            m_InputMultiplier = new Vector2((float)worldTexture.Width / Width, (float)worldTexture.Height / Height);

            spriteBatch.Draw2D(worldTexture, new Rectangle(position.X, position.Y, Width, Height), Vector3.Zero);
            base.Draw(spriteBatch, position);
        }

        protected override void OnMouseOver(int x, int y)
        {
            MousePosition = new Point((int)(x * m_InputMultiplier.X), (int)(y * m_InputMultiplier.Y));
        }
    }
}
