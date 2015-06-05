using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;

namespace UltimaXNA.Ultima.World.Gumps
{
    class WorldViewGump : Gump
    {
        private WorldModel m_Model;

        private ChatControl m_ChatWindow;
        private const int BorderWidth = 5, BorderHeight = 7;
        private int m_WorldWidth, m_WorldHeight;

        public WorldViewGump()
            : base(0, 0)
        {
            HandlesMouseInput = false;
            IsUncloseableWithRMB = true;
            IsUncloseableWithEsc = true;
            IsMovable = true;
            UserInterface.SetControlLayer(this, UILayer.Under);

            m_Model = ServiceRegistry.GetService<WorldModel>();

            m_WorldWidth = Settings.World.GumpResolution.Width;
            m_WorldHeight = Settings.World.GumpResolution.Height;

            Position = new Point(32, 32);
            Size = new Point(m_WorldWidth + BorderWidth * 2, m_WorldHeight + BorderHeight * 2);

            OnResize();
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("worldview");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
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
            // border for dragging
            AddControl(new ResizePic(this, 0, 0, 0, 0xa3c, Width, Height));
            // world control!
            AddControl(new WorldControl(this, BorderWidth, BorderHeight, m_WorldWidth, m_WorldHeight));
            // chat!
            AddControl(m_ChatWindow = new ChatControl(this, BorderWidth, BorderHeight, 400, m_WorldHeight));
            ServiceRegistry.Register<ChatControl>(m_ChatWindow);
        }
    }
}
