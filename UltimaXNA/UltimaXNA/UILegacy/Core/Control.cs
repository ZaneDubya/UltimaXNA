using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Graphics;

namespace UltimaXNA.UILegacy
{
    public class Control : iControl
    {
        Serial _serial = Serial.Null;
        bool _enabled = false;
        bool _visible = false;
        bool _isInitialized = false;
        bool _isDisposed = false;
        public Serial Serial { get { return _serial; } set { _serial = value; } }
        public bool Enabled { get { return _enabled; } set { _enabled = value; } }
        public bool Visible { get { return _visible; } set { _visible = value; } }
        public bool IsInitialized { get { return _isInitialized; } set { _isInitialized = value; } }
        public bool IsDisposed { get { return _isDisposed; } set { _isDisposed = value; } }
        public bool HandlesInput = false;

        Rectangle _area = Rectangle.Empty;
        Vector2 _position = Vector2.Zero;
        public int X { get { return (int)(_position.X); } set { _position.X = (int)value; } }
        public int Y { get { return (int)(_position.Y); } set { _position.Y = (int)value; } }
        public int Width
        {
            get { return _area.Width; }
            set
            {
                _area.Width = (int)value;
            }
        }
        public int Height
        {
            get { return _area.Height; }
            set
            {
                _area.Height = (int)value;
            }
        }
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }
        public Vector2 Size
        {
            get { return new Vector2(_area.Width, _area.Height); }
            set
            {
                _area.Width = (int)value.X;
                _area.Height = (int)value.Y;
            }
        }
        public Rectangle Area
        {
            get { return _area; }
        }

        protected Control _owner = null;
        protected UIManager _manager = null;
        protected List<Control> _controls = null;

#if DEBUG
        protected Texture2D _debugTexture;
#endif

        public Control(Serial serial, Control owner)
        {
            _serial = serial;
            _owner = owner;
        }

        virtual public void Initialize(UIManager manager)
        {
            _manager = manager;
            _isInitialized = true;
            _isDisposed = false;
        }

        virtual public void Dispose()
        {
            if (_controls != null)
            {
                foreach (Control c in _controls)
                {
                    c.Dispose();
                }
            }
            _isDisposed = true;
        }

        virtual public Control[] HitTest(Vector2 position)
        {
            List<Control> focusedControls = new List<Control>();

            Rectangle hitArea = Area;
            // If we're owned by something, make sure we increment our hitArea to show this.
            if (_owner != null)
            {
                hitArea.X += _owner.X;
                hitArea.Y += _owner.Y;
            }

            bool inBounds = hitArea.Contains((int)position.X, (int)position.Y);
            if (inBounds)
            {
                // !!! This will double include nested controls that can handle input... :(
                if (this.HandlesInput)
                    focusedControls.Insert(0, this);
                if (_controls != null)
                {
                    foreach (Control c in _controls)
                    {
                        Control[] c1 = c.HitTest(position);
                        if (c1 != null)
                        {
                            for (int i = c1.Length - 1; i >= 0; i--)
                            {
                                focusedControls.Insert(0, c1[i]);
                            }
                        }
                    }
                }
            }

            if (focusedControls.Count == 0)
                return null;
            else
                return focusedControls.ToArray();
        }

        virtual public void Update(GameTime gameTime)
        {
            if (!_isInitialized)
                return;

            // update our area X and Y to reflect any movement.
            _area.X = X;
            _area.Y = Y;

            if (_controls != null)
            {
                foreach (Control c in _controls)
                {
                    if (!c.IsInitialized)
                        c.Initialize(_manager);
                    c.Update(gameTime);
                }
            }
        }

        virtual public void Draw(ExtendedSpriteBatch spriteBatch)
        {
            if (!_isInitialized)
                return;
            /*
#if DEBUG
            if (_debugTexture == null)
            {
                Color[] data = new Color[] { Color.White };

                _debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _debugTexture.SetData<Color>(data);
            }

            Color color = Color.Red;
            if (_manager.MouseOverControl == this)
                color = Color.Blue;

            Rectangle drawArea = _area;
            if (_owner == null)
            {
                _area.X -= X;
                _area.Y -= Y;
            }
            spriteBatch.Draw(_debugTexture, new Rectangle(_area.X, _area.Y, _area.Width, 1), color);
            spriteBatch.Draw(_debugTexture, new Rectangle(_area.X, _area.Y + _area.Height - 1, _area.Width, 1), color);
            spriteBatch.Draw(_debugTexture, new Rectangle(_area.X, _area.Y, 1, _area.Height), color);
            spriteBatch.Draw(_debugTexture, new Rectangle(_area.X + _area.Width - 1, _area.Y, 1, _area.Height), color);
#endif
            */
            if (_controls != null)
            {
                foreach (Control c in _controls)
                {
                    c.Draw(spriteBatch);
                }
            }
        }

        public virtual void Activate(Control c)
        {
            if (_owner != null)
                _owner.Activate(c);
        }

        public void MouseDown(Vector2 position, int button)
        {
            int x = (int)position.X - X - ((_owner != null) ? _owner.X : 0);
            int y = (int)position.Y - Y - ((_owner != null) ? _owner.Y : 0);
            _mouseDown(x, y, button);
        }

        public void MouseUp(Vector2 position, int button)
        {
            int x = (int)position.X - X - ((_owner != null) ? _owner.X : 0);
            int y = (int)position.Y - Y - ((_owner != null) ? _owner.Y : 0);
            _mouseUp(x, y, button);
        }

        public void MouseClick(Vector2 position, int button)
        {
            int x = (int)position.X - X - ((_owner != null) ? _owner.X : 0);
            int y = (int)position.Y - Y - ((_owner != null) ? _owner.Y : 0);
            _mouseClick(x, y, button);
        }

        public virtual void _mouseDown(int x, int y, int button)
        {

        }

        public virtual void _mouseUp(int x, int y, int button)
        {

        }

        public virtual void _mouseClick(int x, int y, int button)
        {

        }
    }
}
