/***************************************************************************
 *   WorldViewGump.cs
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
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    /// <summary>
    /// A bordered container that displays the world control.
    /// </summary>
    class WorldViewGump : Gump
    {
        private WorldModel m_Model;

        private WorldViewport m_Viewport;
        private ResizePic m_Border;
        private ChatControl m_ChatWindow;

        private const int BorderWidth = 5, BorderHeight = 7;
        private int m_WorldWidth, m_WorldHeight;

        public WorldViewGump()
            : base(0, 0)
        {
            HandlesMouseInput = false;
            IsUncloseableWithRMB = true;
            IsUncloseableWithEsc = true;
            IsMoveable = true;
            MetaData.Layer = UILayer.Under;

            m_Model = ServiceRegistry.GetService<WorldModel>();

            m_WorldWidth = Settings.World.PlayWindowGumpResolution.Width;
            m_WorldHeight = Settings.World.PlayWindowGumpResolution.Height;

            Position = new Point(32, 32);
            OnResize();
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("worldview");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_WorldWidth != Settings.World.PlayWindowGumpResolution.Width || m_WorldHeight != Settings.World.PlayWindowGumpResolution.Height)
            {
                m_WorldWidth = Settings.World.PlayWindowGumpResolution.Width;
                m_WorldHeight = Settings.World.PlayWindowGumpResolution.Height;
                OnResize();
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);
        }

        protected override void OnMove()
        {
            // base.OnMove() would make sure that the gump remained at least half on screen, but we want more fine-grained control over movement.
            SpriteBatchUI sb = ServiceRegistry.GetService<SpriteBatchUI>();
            Point position = Position;

            if (position.X < -BorderWidth)
                position.X = -BorderWidth;
            if (position.Y < -BorderHeight)
                position.Y = -BorderHeight;
            if (position.X + Width - BorderWidth > sb.GraphicsDevice.Viewport.Width)
                position.X = sb.GraphicsDevice.Viewport.Width - (Width - BorderWidth);
            if (position.Y + Height - BorderHeight > sb.GraphicsDevice.Viewport.Height)
                position.Y = sb.GraphicsDevice.Viewport.Height - (Height - BorderHeight);

            Position = position;
        }

        private void OnResize()
        {
            if (ServiceRegistry.ServiceExists<ChatControl>())
                ServiceRegistry.Unregister<ChatControl>();

            ClearControls();

            Size = new Point(m_WorldWidth + BorderWidth * 2, m_WorldHeight + BorderHeight * 2);
            AddControl(m_Border = new ResizePic(this, 0, 0, 0xa3c, Width, Height));
            AddControl(m_Viewport = new WorldViewport(this, BorderWidth, BorderHeight, m_WorldWidth, m_WorldHeight));
            AddControl(m_ChatWindow = new ChatControl(this, BorderWidth, BorderHeight, 400, m_WorldHeight));
            ServiceRegistry.Register<ChatControl>(m_ChatWindow);
        }
    }
}
