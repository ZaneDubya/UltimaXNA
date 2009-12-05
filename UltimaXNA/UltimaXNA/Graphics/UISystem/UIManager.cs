using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Diagnostics;
using UltimaXNA.Extensions;
using UltimaXNA.Input;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.EventSystem;
 
namespace UltimaXNA.Graphics.UI
{
    public interface IMouseAttachable
    {
        bool Moveable { get; }
    }

    public class UIManager : DrawableGameComponent, IUIService
    {
        static ILoggingService _log = new Logger("UIManager");
        static string _rootDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Content\\UI");
        static Dictionary<string, Type> _predefinedComponents = new Dictionary<string, Type>();
        static Dictionary<string, string> _predefinedXmlComponents = new Dictionary<string, string>();

        public static string RootDirectory
        {
          get { return _rootDirectory; }
          set { _rootDirectory = value; }
        }

        public static void Initialize(Game game)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();

                for (int j = 0; j < types.Length; j++)
                {
                    if (types[j].IsSubclassOf(typeof(UINode)))
                    {
                        string name = types[j].Name;

                        if (name.IndexOf('.') != -1 && name.IndexOf('.') + 1 != name.Length)
                            name = name.Substring(name.LastIndexOf('.') + 1);

                        Register(name, types[j]);
                    }
                }
            }

            DirectoryInfo dir = new DirectoryInfo(RootDirectory);

            if (dir.Exists)
            {
                FileInfo[] xmlFiles = dir.GetFiles("*.xml", SearchOption.AllDirectories);

                for (int i = 0; i < xmlFiles.Length; i++)
                {
                    FileInfo info = xmlFiles[i];

                    string name = Path.GetFileNameWithoutExtension(info.Name);

                    Register(name, info.FullName, true);
                }
            }
        }

        public static void Register(string name, Type type)
        {
            if (!type.IsSubclassOf(typeof(UINode)))
            {
                _log.Error("UIManager cannot register {0} as {1} since {1} does not implement IUINode", name, type);
                return;
            }

            _log.Info("UIManager Registering {0} as {1}", name, type);

            if (_predefinedComponents.ContainsKey(name.ToLower()))
            {
                _predefinedComponents[name.ToLower()] = type;
            }
            else
            {
                _predefinedComponents.Add(name.ToLower(), type);
            }
        }

        public static void Register<T>(string name) where T : UINode
        {
            _log.Info("UIManager Registering {0} as {1}", name, typeof(T));

            if (_predefinedComponents.ContainsKey(name.ToLower()))
            {
                _predefinedComponents[name.ToLower()] = typeof(T);
            }
            else
            {
                _predefinedComponents.Add(name.ToLower(), typeof(T));
            }
        }

        public static void Register(string name, string pathOrXml, bool file)
        {
            _log.Info("UIManager Registering {0} as {1}", name, pathOrXml);

            if (file && !File.Exists(pathOrXml))
            {
                throw new FileNotFoundException(pathOrXml);
            }

            pathOrXml = File.ReadAllText(pathOrXml);

            if (_predefinedXmlComponents.ContainsKey(name.ToLower()))
            {
                _predefinedXmlComponents[name.ToLower()] = pathOrXml;
            }
            else
            {
                _predefinedXmlComponents.Add(name.ToLower(), pathOrXml);
            }
        }

        IInputService _input;
        IEventService _eventEngine;

        UIContainer _children;
        UINode _attachedNode;
        UINode _focusedNode;
        UINode _mouseDownNode;
        UINode _trackingNode;

        ExtendedSpriteBatch _spriteBatch;

#if LUA_UI
        Lua _lua;

        internal Lua Lua
        {
            get { return _lua; }
        }
#endif

        internal UIContainer Children
        {
            get { return _children; }
        }

        internal ExtendedSpriteBatch SpriteBatch
        {
            get { return _spriteBatch; }
        }

        internal UIManager(Game game)
            : base(game)
        {
            _spriteBatch = new ExtendedSpriteBatch(game.GraphicsDevice);

            //Retrieve the needed services.
            _input = game.Services.GetService<IInputService>(true);
            _eventEngine = game.Services.GetService<IEventService>(true);
            
#if LUA_UI
            //Create the Lua VM
            _lua = new Lua();
            
            //Load the init script from the internal resource.
            _lua.DoString(LuaScripts.gui_init);

            //Set UI to this object
            _lua["UI"] = this;

            //Set World to a new instance of World.  
            //We have to set it to a new world so it can.
            //register the static functions.
            _lua["World"] = new World();

            //Set Input to the InputService
            _lua["Input"] = _input;

            //Get all the event's from the Event class.
            FieldInfo[] events = typeof(Events).GetFields();

            //Register each as a global variable... this saves us 
            //from having to remember to add a new event to Lua 
            //and C# for each additional event added.
            for (int i = 0; i < events.Length; i++)
            {
                if(events[i].GetCustomAttributes(typeof(LuaExposedEvent), false).Length > 0)
                    _lua.DoString(String.Format("{0} = {1}", events[i].Name, events[i].GetValue(null)));
            }

            //Register the overrides, this allows us to override 
            //lots of Lua functions to make them ours... 
            _lua.DoString(LuaScripts.lua_overrides);

            //Register all global helper functions and varaibles
            _lua.DoString(LuaScripts.lua_globals);
#endif
            _children = new UIContainer(null, true);

            _spriteBatch.Effect = game.Content.Load<Effect>("Shaders\\Gumps");

            _input.MouseDown += new EventHandler<MouseEventArgs>(OnMouseDown);
            _input.MouseUp += new EventHandler<MouseEventArgs>(OnMouseUp);
            _input.MouseMove += new EventHandler<MouseEventArgs>(OnMouseMove);
            _input.KeyUp += new EventHandler<KeyboardEventArgs>(OnKeyUp);
            _input.KeyDown += new EventHandler<KeyboardEventArgs>(OnKeyDown);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.ContainsButtonDown(MouseButtons.LeftButton))
            {
                UINode node = FindFirstChildAt(_input.CurrentMousePosition);

                while (node != null)
                {
                    SetFocus(node);

                    IUIContainer container = node as IUIContainer;

                    if (container != null)
                    {
                        node = container.FindFirstChildAt(_input.CurrentMousePosition);
                    }
                }

                if (_focusedNode != null && _focusedNode.HitTest(_input.CurrentMousePosition))
                {
                    _focusedNode.HandleClick(MouseButtons.LeftButton, true);
                    _mouseDownNode = _focusedNode;
                }
            }
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.ContainsButtonUp(MouseButtons.LeftButton))
            {
                UINode node = FindFirstChildAt(_input.CurrentMousePosition);

                while (node != null)
                {
                    SetFocus(node);

                    IUIContainer container = node as IUIContainer;

                    if (container != null)
                    {
                        node = container.FindFirstChildAt(_input.CurrentMousePosition);
                    }
                }

                if (_focusedNode != null)
                {
                    _focusedNode.HandleClick(MouseButtons.LeftButton, false);
                }
            }

            if (e.ContainsButtonReleased(MouseButtons.RightButton))
            {
                UINode node = FindFirstChildAt(_input.CurrentMousePosition);

                while (node != null)
                {
                    IUIContainer container = node as IUIContainer;

                    if (container != null)
                    {
                        node = container.FindFirstChildAt(_input.CurrentMousePosition);
                    }
                }

                if (node != null)
                {
                    node.HandleClick(MouseButtons.RightButton, false);
                }
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            UINode node = FindFirstChildAt(_input.CurrentMousePosition);

            if (node != null)
            {
                while (true)
                {
                    UINode n = null;
                    IUIContainer container = node as IUIContainer;

                    if (container != null)
                    {
                        n = container.FindFirstChildAt(_input.CurrentMousePosition);
                    }

                    if (n == null)
                        break;

                    node = n;
                }

                if (_trackingNode != node)
                {
                    if (_trackingNode != null)
                    {
                        _trackingNode.HandleMouseLeave();
                    }

                    _trackingNode = node;

                    if (_trackingNode != null)
                    {
                        _trackingNode.HandleMouseEnter();
                    }
                }
            }

            if (_attachedNode != null &&
                _attachedNode.Parent != null &&
                _attachedNode.Parent is Gump)
            {
                _attachedNode.Parent.Position += e.MovementDelta;
            }
        }

        private void OnKeyUp(object sender, KeyboardEventArgs e)
        {
            if (_focusedNode != null)
            {
                _focusedNode.HandleKeyUp(e);
            }
        }

        private void OnKeyDown(object sender, KeyboardEventArgs e)
        {
            if (_focusedNode != null)
            {
                _focusedNode.HandleKeyDown(e);
            }
        }

        public UINode FindFirstChildAt(Vector2 position)
        {
            UINode node = null;

            for (int i = _children.Count - 1; i >= 0 && node == null; i--)
            {
                if (_children[i].HitTest(position.X, position.Y))
                {
                    node = _children[i];
                }
            }

            return node;
        }

        public void SetFocus(UINode node)
        {
            if (node != null)
            {
                if (node.Parent != null && node.Parent is Gump && _children.Contains(node.Parent))
                {
                    _children.BringToFront(node.Parent);
                }
            }

            _focusedNode = node;
        }

        public void Remove(UINode node)
        {

        }

        public T CreateInstance<T>(string name) where T : UINode
        {
            return CreateInstance<T>(name, null);
        }

        public T CreateInstance<T>(string name, Serial? serial) where T : UINode
        {
            Type type = null;
            T node = default(T);

            if (_predefinedComponents.TryGetValue(name.ToLower(), out type))
            {
                node = (T)Activator.CreateInstance(type, Game, null);
            }
            else
            {
                string xml = string.Empty;

                if (_predefinedXmlComponents.TryGetValue(name.ToLower(), out xml))
                {
                    try
                    {
                        _log.Info("Loading UI from xml", xml);
                        node = CreateXml<T>(xml, serial);
                    }
                    catch (Exception e)
                    {
                        _log.Error("An error occurred while loading a ui element from xml, please see the error log for more details.");
                        _log.Fatal(e);
                    }
                }
            }

            if (node != null)
            {
                _children.Add(node);
            }

            return node;
        }

        public T CreateXml<T>(string xml, Serial? serial) where T : UINode
        {
            T node = null;

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlElement element = doc.FirstChild as XmlElement;

                if (!string.IsNullOrEmpty(element.Name) && element.Name == "Gump")
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
                        ConstructorInfo ctor = type.GetConstructor(new Type[] { typeof(Game), typeof(Serial) });

                        if (ctor == null)
                        {
                            _log.Error("{0} must contain a public constructor containing 2 argument of type Microsoft.Xna.Framework.Game and UltimaXNA.UINode", typeName);
                        }
                        else if(!serial.HasValue)
                        {                            
                            _log.Error("Cannot load Xml UI without a valid Serial");
                        }
                        else
                        {
                            //ctor(Game game, Serial serial)
                            node = (T)Activator.CreateInstance(type, Game, serial.Value);
                        }
                    }
                }
                else
                {
                    _log.Warn("The first element in the UI Xml must be type 'Gump'");
                }

                if (node != null)
                {
                    node.Initialize(element);
                    _children.Add(node);
                }
            }
            catch (Exception e)
            {
                _log.Error("An error occured while loading UI Element from xml.  Error: {0}", e.Message);
            }

            return node;
        }

        public void AttachMouse(UINode node)
        {
            if (node is IMouseAttachable)
            {
                _attachedNode = node;
            }
        }

        public void DettachMouse()
        {
            _attachedNode = null;
        }

        protected override void  Dispose(bool disposing)
        {
 	        base.Dispose(disposing);
            _input.MouseDown -= new EventHandler<MouseEventArgs>(OnMouseDown);
            _input.MouseMove -= new EventHandler<MouseEventArgs>(OnMouseMove);
            _input.MouseUp -= new EventHandler<MouseEventArgs>(OnMouseUp);
            _input.KeyUp -= new EventHandler<KeyboardEventArgs>(OnKeyUp);
            _input.KeyDown -= new EventHandler<KeyboardEventArgs>(OnKeyDown);
#if LUA_UI
            _lua.Dispose();
#endif
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            _spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None);

            for (int i = 0; i < _children.Count; i++)
            {
                _children[i].Draw(gameTime);
            }

            _spriteBatch.End();
        }
        
#if LUA_UI
        public void PrintDebug(string message)
        {
            _log.Debug(message);
        }

        public void PrintInfo(string message)
        {
            _log.Info(message);
        }

        public void PrintWarn(string message)
        {
            _log.Warn(message);
        }

        public void PrintError(string message)
        {
            _log.Error(message);
        }

        public void PrintFatal(string message)
        {
            _log.Fatal(message);
        }
#endif
    }
}
