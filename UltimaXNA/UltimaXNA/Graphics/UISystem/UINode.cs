using System;
using System.Reflection;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Diagnostics;
using UltimaXNA.EventSystem;
using UltimaXNA.Extensions;
using UltimaXNA.Input;

namespace UltimaXNA.Graphics.UI
{
    public delegate void ClickHandler(UINode self, MouseButtons button, bool down);
    public delegate void KeyCharHandler(UINode self, char ch);
    public delegate void MouseEnterHandler(UINode self);
    public delegate void MouseLeaveHandler(UINode self);
    
    public abstract class UINode : IUINode
    {
        protected Game _game;
        protected GraphicsDevice _graphicsDevice;
        protected ExtendedSpriteBatch _spriteBatch;

        protected ILoggingService _log;
        protected IEventService _eventEngine;

        protected UINode _parent;
        protected UIManager _manager;

        protected Vector2 _position;
        protected Vector2 _size;

        protected string _name;
        protected int _index;
        protected bool _useInternalEventHandlers = true;

        public bool UseInternalEventHandlers
        {
            get { return _useInternalEventHandlers; }
            set { _useInternalEventHandlers = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public virtual Vector2 Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public RectangleF Bounds
        {
            get
            {
                Vector2 size = Size;
                Vector2 position = Position;

                return new RectangleF(position, size);
            }
        }

        public UINode Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }

        public event EventEngineHandler Event;
        public event MouseEnterHandler MouseEnter;
        public event MouseLeaveHandler MouseLeave;
        public event ClickHandler Click;
        public event KeyCharHandler KeyChar;

        public UINode(Game game, UINode parent)
        {
            _game = game;
            _graphicsDevice = _game.GraphicsDevice;
            _parent = parent;

            _manager = (UIManager)_game.Services.GetService<IUIService>(true);
            _eventEngine = _game.Services.GetService<IEventService>(true);
            _log = _game.Services.GetService<ILoggingService>(true);

            _spriteBatch = _manager.SpriteBatch;
        }

        internal virtual void Initialize(XmlElement element)
        {
            XmlNodeList nodes = element.ChildNodes;

            for (int i = 0; i < element.Attributes.Count; i++)
            {
                XmlAttribute attr = element.Attributes[i];

                if (attr != null)
                {
                    ValidateAttribute(attr);
                }
            }
        }

        internal virtual void HandleClick(MouseButtons button, bool down)
        {
            if (Click != null)
            {
                Click(this, button, down);
            }
        }

        internal virtual void HandleKeyChar(char ch)
        {
            if (KeyChar != null)
            {
                KeyChar(this, ch);
            }
        }

        public virtual bool HitTest(Vector2 position)
        {
            return HitTest(position.X, position.Y);
        }
        
        public virtual bool HitTest(float x, float y)
        {
            bool hit;
            HitTest(ref x, ref y, out hit);
            return hit;
        }

        public virtual void HitTest(ref float x, ref float y, out bool value)
        {
            RectangleF renderBounds;
            CalculateRenderBounds(out renderBounds);
            value = renderBounds.Contains(x, y);
        }

        internal virtual void Draw(GameTime gameTime)
        {

        }

        public bool IsRegisteredEvent(short eventId)
        {
            return _eventEngine.IsRegistered(this, eventId);
        }

        public void RegisterEvent(short eventId)
        {
            _eventEngine.Register(this, eventId);
        }

        public void RegisterAllEvents()
        {
            _eventEngine.RegisterAll(this);
        }

        public void UnregisterEvent(short eventId)
        {
            _eventEngine.Unregister(this, eventId);
        }

        public void UnregisterAllEvents()
        {
            _eventEngine.UnregisterAll(this);
        }

        public void Dispose()
        {

        }

        protected virtual void Dispose(bool disposing)
        {

        }

        public void SendEvent(object sender, int eventId, params object[] args)
        {
            if (Event != null)
            {
                Event(this, args);
            }
        }

        internal void CalculateRenderBounds(out RectangleF renderBounds)
        {
            RectangleF parentBounds = renderBounds = RectangleF.Empty;
            renderBounds = Bounds;

            if (_parent != null)
            {
                _parent.CalculateRenderBounds(out parentBounds);
            }

            renderBounds.X += parentBounds.X;
            renderBounds.Y += parentBounds.Y;
        }

        private void ValidateAttribute(XmlAttribute attr)
        {
            string name = attr.Name;
            string value = attr.Value;

            if (name.ToLower() == "script")
                return;

            PropertyInfo nameProp = GetType().GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);

            if (nameProp == null)
            {
                EventInfo eventInfo = GetType().GetEvent(name);

                if (eventInfo == null)
                {
                    _log.Warn("Attribute {0} is not a valid property or event name.", name);
                }
                else
                {                    
#if LUA_UI
                    _useInternalEventHandlers = false;
                    _manager.Lua.DoString("local controlTemp = {{}}");
                    _manager.Lua["controlTemp"] = this;
                    _manager.Lua.DoString(String.Format("controlTemp.{0}:Add({1})", eventInfo.Name, value));
#endif
                }
            }
            else
            {
                try
                {
                    object to;

                    if (Utility.TryConvert(typeof(string), (object)value, nameProp.PropertyType, out to))
                    {
                        nameProp.SetValue(this, to, null);
                    }
                }
                catch (Exception e)
                {
                    _log.Error("Unable to set property {0} to value {1}.  See error log for more details", name, value);
                    _log.Fatal(e);
                }
            }
        }

        internal virtual void HandleMouseEnter()
        {
            if (MouseEnter != null)
                MouseEnter(this);
        }

        internal virtual void HandleMouseLeave()
        {
            if (MouseLeave != null)
                MouseLeave(this);
        }

        internal virtual void HandleKeyUp(KeyboardEventArgs e)
        {

        }

        internal virtual void HandleKeyDown(KeyboardEventArgs e)
        {

        }
    }
}
