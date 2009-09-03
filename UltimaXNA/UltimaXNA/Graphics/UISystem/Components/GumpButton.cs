using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Graphics.UI
{
    public class GumpButton : UINode, IGumpButton
    {
        private int _normalId;
        private int _pressedId;
        private int _buttonId;
        private int _param;

        private bool _pressed;

        private Texture2D _normalTexture;
        private Texture2D _pressedTexture;

        public bool Pressed
        {
            get { return _pressed; }
            set { _pressed = value; }
        }

        public Texture2D NormalTexture
        {
            get { return _normalTexture; }
        }

        public Texture2D PressedTexture
        {
            get { return _pressedTexture; }
        }

        public int NormalId
        {
            get { return _normalId; }
            set { _normalId = value; }
        }

        public int PressedId
        {
            get { return _pressedId; }
            set { _pressedId = value; }
        }

        public int ButtonId
        {
            get { return _buttonId; }
            set { _buttonId = value; }
        }

        public int Param
        {
            get { return _param; }
            set { _param = value; }
        }

        public override Vector2 Size
        {
            get
            {
                if (_pressed)
                {
                    if (_pressedTexture != null)
                    {
                        return new Vector2(_pressedTexture.Width, _pressedTexture.Height);
                    }
                    else
                    {
                        return base.Size;
                    }
                }
                else
                {
                    if (_normalTexture != null)
                    {
                        return new Vector2(_normalTexture.Width, _normalTexture.Height);
                    }
                    else
                    {
                        return base.Size;
                    }
                }
            }
            set
            {
                base.Size = value;
            }
        }

        public GumpButton(Game game, UINode parent)
            : base(game, parent)
        {

        }

        internal override void Initialize(System.Xml.XmlElement element)
        {
            base.Initialize(element);

            _normalTexture = Data.Gumps.GetGumpXNA(_normalId);
            _pressedTexture = Data.Gumps.GetGumpXNA(_pressedId);
        }

        internal override void HandleClick(UltimaXNA.Input.MouseButtons button, bool down)
        {
            if (_useInternalEventHandlers)
            {
                _pressed = down;

                if (!down)
                {
                    base.HandleClick(button, down);
                }
            }
            else
            {
                base.HandleClick(button, down);
            }
        }

        internal override void HandleMouseLeave()
        {
            if (_useInternalEventHandlers)
            {
                _pressed = false;
            }
            else
            {
                base.HandleMouseLeave();
            }
        }

        internal override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            RectangleF renderBounds;
            PresentationParameters pp = _graphicsDevice.PresentationParameters;

            CalculateRenderBounds(out renderBounds);

            //_graphicsDevice.ScissorRectangle = renderBounds;
            //_graphicsDevice.RenderState.ScissorTestEnable = true;

            Texture2D texture = _normalTexture;

            if (_pressed)
                texture = _pressedTexture;

            _spriteBatch.Draw(texture, renderBounds, Color.White);

            //_graphicsDevice.ScissorRectangle = new Rectangle(0, 0, pp.BackBufferWidth, pp.BackBufferHeight);
            //_graphicsDevice.RenderState.ScissorTestEnable = false;
        }
    }
}
