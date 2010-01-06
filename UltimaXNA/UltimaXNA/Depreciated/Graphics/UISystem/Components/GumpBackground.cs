using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Graphics.UI
{
    public class GumpBackground : UINode, IMouseAttachable, IGumpBackground
    {
        private int _gumpId;

        private Texture2D _topLeftTexture;
        private Texture2D _topTexture;
        private Texture2D _topRightTexture;
        private Texture2D _middleLeftTexture;
        private Texture2D _middleTexture;
        private Texture2D _middleRightTexture;
        private Texture2D _bottomLeftTexture;
        private Texture2D _bottomTexture;
        private Texture2D _bottomRightTexture;

        public bool Moveable
        {
            get
            {
                bool moveable = false;

                if (_parent != null && _parent is IMouseAttachable)
                {
                    moveable = ((IMouseAttachable)_parent).Moveable;
                }

                return moveable;
            }
        }

        public Texture2D TopLeftTexture
        {
            get { return _topLeftTexture; }
        }

        public Texture2D TopTexture
        {
            get { return _topTexture; }
        }

        public Texture2D TopRightTexture
        {
            get { return _topRightTexture; }
        }

        public Texture2D MiddleLeftTexture
        {
            get { return _middleLeftTexture; }
        }

        public Texture2D MiddleTexture
        {
            get { return _middleTexture; }
        }

        public Texture2D MiddleRightTexture
        {
            get { return _middleRightTexture; }
        }

        public Texture2D BottomLeftTexture
        {
            get { return _bottomLeftTexture; }
        }

        public Texture2D BottomTexture
        {
            get { return _bottomTexture; }
        }

        public Texture2D BottomRightTexture
        {
            get { return _bottomRightTexture; }
        }

        public int GumpId
        {
            get { return _gumpId; }
            set
            {
                _gumpId = value;
                InitializeTextures();
            }
        }

        public GumpBackground(Game game, UINode parent)
            : base(game, parent)
        {

        }

        internal override void Initialize(System.Xml.XmlElement element)
        {
            base.Initialize(element);
        }

        internal override void HandleClick(UltimaXNA.Input.MouseButtons button, bool down)
        {
            if (_useInternalEventHandlers)
            {
                if (down)
                {
                    _manager.AttachMouse(this);
                }
                else
                {
                    _manager.DettachMouse();
                }
            }
            else
            {
                base.HandleClick(button, down);
            }
        }

        private void InitializeTextures()
        {
            _topLeftTexture = Data.Gumps.GetGumpXNA(_gumpId);
            _topTexture = Data.Gumps.GetGumpXNA(_gumpId + 1);
            _topRightTexture = Data.Gumps.GetGumpXNA(_gumpId + 2);
            _middleLeftTexture = Data.Gumps.GetGumpXNA(_gumpId + 3);
            _middleTexture = Data.Gumps.GetGumpXNA(_gumpId + 4);
            _middleRightTexture = Data.Gumps.GetGumpXNA(_gumpId + 5);
            _bottomLeftTexture = Data.Gumps.GetGumpXNA(_gumpId + 6);
            _bottomTexture = Data.Gumps.GetGumpXNA(_gumpId + 7);
            _bottomRightTexture = Data.Gumps.GetGumpXNA(_gumpId + 8);
        }

        internal override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            RectangleF renderBounds;

            CalculateRenderBounds(out renderBounds);

            PresentationParameters pp = _graphicsDevice.PresentationParameters;

            //_graphicsDevice.ScissorRectangle = renderBounds;
            //_graphicsDevice.RenderState.ScissorTestEnable = true;

            Vector2 position;
            Vector2 size;

            renderBounds.GetPosition(out position);
            renderBounds.GetSize(out size);

            int x = (int)position.X;
            int y = (int)position.Y;
            int leftWidth = _middleLeftTexture.Width;
            int rightWidth = _middleRightTexture.Width;
            int topHeight = _topTexture.Height;
            int bottomHeight = _bottomTexture.Height;
            int width = (int)size.X;
            int height = (int)size.Y;

            Rectangle[] sourceRectangles = new Rectangle[] {
                new Rectangle(0, 0, leftWidth - 2, topHeight),//topleft
                new Rectangle(0, 0, width - (rightWidth + leftWidth) + 4, topHeight),//topmiddle
                new Rectangle(0, 0, rightWidth - 1, topHeight),//topright
                new Rectangle(0, 0, leftWidth, height - (topHeight + bottomHeight)),//midleft
                new Rectangle(0, 0, width - (leftWidth + rightWidth) + 4, height - (topHeight + bottomHeight)),//midmid
                new Rectangle(0, 0, rightWidth - 1, height - (topHeight + bottomHeight)),//midright
                new Rectangle(0, 0, leftWidth - 2, bottomHeight),//botleft
                new Rectangle(0, 0, width - (leftWidth + rightWidth) + 4, bottomHeight),//botmiddle
                new Rectangle(0, 0, rightWidth - 1, bottomHeight)//botright
            };

            //Topleft
            _spriteBatch.Draw(_topLeftTexture, position, sourceRectangles[0], Color.White);
            //Top
            _spriteBatch.Draw(_topTexture, position + new Vector2(leftWidth - 2, 0), sourceRectangles[1], Color.White);
            //Topright
            _spriteBatch.Draw(_topRightTexture, position + new Vector2(width - (rightWidth - 2), 0), sourceRectangles[2], Color.White);
            //Left
            _spriteBatch.Draw(_middleLeftTexture, position + new Vector2(0, topHeight), sourceRectangles[3], Color.White);
            //Middle
            _spriteBatch.Draw(_middleTexture, position + new Vector2(leftWidth, topHeight), sourceRectangles[4], Color.White);
            //Right
            _spriteBatch.Draw(_middleRightTexture, position + new Vector2(width - rightWidth + 2, topHeight), sourceRectangles[5], Color.White);
            //Bottomleft
            _spriteBatch.Draw(_bottomLeftTexture, position + new Vector2(0, height - bottomHeight), sourceRectangles[6], Color.White);
            //Bottom
            _spriteBatch.Draw(_bottomTexture, position + new Vector2(leftWidth - 2, height - bottomHeight), sourceRectangles[7], Color.White);
            //Bottomright
            _spriteBatch.Draw(_bottomRightTexture, position + new Vector2(width - (rightWidth - 2), height - bottomHeight), sourceRectangles[8], Color.White);

            //_graphicsDevice.ScissorRectangle = new Rectangle(0, 0, pp.BackBufferWidth, pp.BackBufferHeight);
            //_graphicsDevice.RenderState.ScissorTestEnable = false;
        }
    }
}
