using Microsoft.Xna.Framework;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;

namespace UltimaXNA.Ultima.World.Gumps
{
    class WorldViewGump : Gump
    {
        private WorldModel m_Model;

        private ChatWindow m_ChatWindow;

        private const int BorderWidth = 5, BorderHeight = 7;
        private int m_WorldWidth, m_WorldHeight;

        public WorldViewGump()
            : base(0, 0)
        {
            HandlesMouseInput = false;
            IsUncloseableWithRMB = true;
            IsUncloseableWithEsc = true;
            IsMovable = true;
            Layer = GumpLayer.Under;
        }

        protected override void OnInitialize()
        {
            m_Model = UltimaServices.GetService<WorldModel>();

            m_WorldWidth = Settings.Game.WorldGumpResolution.Width;
            m_WorldHeight = Settings.Game.WorldGumpResolution.Height;

            Position = new Point(32, 32);
            Size = new Point(m_WorldWidth + BorderWidth * 2, m_WorldHeight + BorderHeight * 2);

            OnResize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            CheckPosition(spriteBatch);
            base.Draw(spriteBatch);
        }

        private void CheckPosition(SpriteBatchUI spriteBatch)
        {
            if (X < -BorderWidth)
                X = -BorderWidth;
            if (Y < -BorderHeight)
                Y = -BorderHeight;
            if (X + Width - BorderWidth > spriteBatch.GraphicsDevice.Viewport.Width)
                X = spriteBatch.GraphicsDevice.Viewport.Width - (Width - BorderWidth);
            if (Y + Height - BorderHeight > spriteBatch.GraphicsDevice.Viewport.Height)
                Y = spriteBatch.GraphicsDevice.Viewport.Height - (Height - BorderHeight);
        }

        private void OnResize()
        {
            if (UltimaServices.GetService<ChatWindow>() != null)
                UltimaServices.Unregister<ChatWindow>(m_ChatWindow);

            ClearControls();
            // border for dragging
            AddControl(new ResizePic(this, 0, 0, 0, 0xa3c, Width, Height));
            // world control!
            AddControl(new WorldControl(this, BorderWidth, BorderHeight, m_WorldWidth, m_WorldHeight));
            // chat!
            AddControl(m_ChatWindow = new ChatWindow(this, BorderWidth, BorderHeight, 400, m_WorldHeight));
            UltimaServices.Register<ChatWindow>(m_ChatWindow);
        }
    }
}
