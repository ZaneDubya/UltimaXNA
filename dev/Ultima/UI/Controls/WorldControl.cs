using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World;

namespace UltimaXNA.Ultima.UI.Controls
{
    class WorldControl : AControl
    {
        private WorldModel m_Model;

        public Point MousePosition;

        private Vector2 m_InputMultiplier = Vector2.One;

        public WorldControl(AControl owner, int x, int y, int width, int height)
            : base(owner, 0)
        {
            Position = new Point(x, y);
            Size = new Point(width, height);

            HandlesMouseInput = true;
        }

        protected override void OnInitialize()
        {
            m_Model = UltimaServices.GetService<WorldModel>();
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            Texture2D worldTexture = (m_Model.GetView() as WorldView).Isometric.Texture;
            m_InputMultiplier = new Vector2((float)worldTexture.Width / Width, (float)worldTexture.Height / Height);

            spriteBatch.Draw2D(worldTexture, new Rectangle(X, Y, Width, Height), Vector3.Zero);
            base.Draw(spriteBatch);
        }

        protected override void OnMouseOver(int x, int y)
        {
            MousePosition = new Point((int)(x * m_InputMultiplier.X), (int)(y * m_InputMultiplier.Y));
        }
    }
}
