using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using Microsoft.Xna.Framework;
using System.IO;

namespace UltimaXNA.Graphics.UI
{
    public class Gump : UINode, IMouseAttachable, IGump, IUIContainer
    {
        private Serial _serial;
        private UIContainer _children;
        //private string _script; Doesn't need this as scripting is not yet enabled. -- ZANE
        private bool _moveable;
        private bool _closable;

        public bool Closable
        {
            get { return _closable; }
            set { _closable = value; }
        }

        public bool Moveable
        {
            get { return _moveable; }
            set { _moveable = value; }
        }

        public Serial Serial
        {
            get { return _serial; }
        }

        public UIContainer Children
        {
            get { return _children; }
        }

        public Gump(Game game, Serial serial)
            : base(game, null)
        {
            _serial = serial;
            _children = new UIContainer(this);
        }

        internal override void Initialize(XmlElement element)
        {
            ValidateScript(element);

            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                XmlElement child = element.ChildNodes[i] as XmlElement;

                if (child != null)
                {
                    ValidateChild(child);
                }
            }

            base.Initialize(element);
        }

        public UINode FindFirstChildAt(float x, float y)
        {
            UINode firstChild = null;

            for (int i = _children.Count - 1; i >= 0 && firstChild == null; i--)
            {
                UINode node = _children[i];

                if (node.HitTest(x, y))
                {
                    firstChild = node;
                }
            }

            return firstChild;
        }

        public UINode FindFirstChildAt(Vector2 v)
        {
            return FindFirstChildAt(v.X, v.Y);
        }

        internal override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Draw(gameTime);
            }
        }

        public override bool HitTest(float x, float y)
        {
            bool hit = false;

            for (int i = 0; i < _children.Count && !hit; i++)
            {
                hit = _children[i].HitTest(x, y);
            }

            return hit;
        }

        private void ValidateScript(XmlElement element)
        {
#if LUA_UI
            XmlAttribute scriptAttr = element.Attributes["Script"];

            if (scriptAttr == null)
            {
                scriptAttr = element.Attributes["script"];

                if (scriptAttr == null)
                {
                    scriptAttr = element.Attributes["SCRIPT"];
                }

            }

            if (scriptAttr != null)
            {
                string script = scriptAttr.Value;

                if (File.Exists(script))
                {
                    _manager.Lua.DoFile(script);
                }
                else
                {
                    _log.Error("Could not find script file '{1}'", script);
                }
            }
#endif
        }

        private void ValidateChild(XmlElement element)
        {
            UINode node = null;

            if (!string.IsNullOrEmpty(element.Name))
            {
                string typeName = element.Name;

                Type type = Utility.GetTypeFromAppDomain(typeName);

                if (type == null)
                {
                    _log.Error("Unable to find class type of '{0}'", typeName);
                }
                else if (!type.IsSubclassOf(typeof(UINode)))
                {
                    _log.Error("The class must inheret UINode");
                }
                else
                {
                    ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(Game), typeof(UINode) });

                    if (ctor == null)
                    {
                        _log.Error("{0} must contain a public constructor containing 1 argument of type Microsoft.Xna.Framework.Game", typeName);
                    }
                    else
                    {
                        node = (UINode)Activator.CreateInstance(type, _game, this);
                    }
                }
            }
            else
            {
                _log.Warn("UINode doesnt define a type attribute, or the type defined by the type attribute is unknown. Unable to load UINode");
            }

            if (node != null)
            {
                node.Initialize(element);
                _children.Add(node);
            }
        }
    }
}
