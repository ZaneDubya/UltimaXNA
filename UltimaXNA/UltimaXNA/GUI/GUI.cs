/***************************************************************************
 *   GUI.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;
using xWinFormsLib;
#endregion

namespace UltimaXNA.GUI
{
    public interface IGUI
    {
        bool IsMouseOverGUI(Vector2 nPosition);
        void Container_Open(Entity nContainerObject, int nGump);
        void Merchant_Open(Entity nContainerObject, int nGump);
        void PaperDoll_Open(Entity nMobileObject);
        void ErrorPopup_Modal(string nText);
        Window Window(string nWindowName);
        Window AddWindow(string windowName, Window window);
        void CloseWindow(string nWindowName);
        bool TargettingCursor { get; set; }
        void Reset();
    }

    public class EngineGUI : DrawableGameComponent, IGUI
    {
        private SpriteFont _FontArial14;
        private SpriteBatch _SpriteBatch;
        private FormCollection _FormCollection;
        private bool _DrawForms = false;
        public string DebugMessage;

        private Dictionary<string, Window> _GUIWindows;
        IEntitiesService _GameObjectsService;
        Client.IUltimaClient _GameClientService;

        private int _MouseCursorIndex = int.MinValue; // set so it is always properly initialized to zero in Initialize();
        private const int _MouseCursorHolding = int.MaxValue;
        private const int _MouseCursorTargetting = int.MaxValue - 1;
        public int MouseCursor
        {
            get { return _MouseCursorIndex; }
            set
            {
                // Only change the mouse cursor when you need to.
                if (_MouseCursorIndex != value)
                {
                    _MouseCursorIndex = value;
                    GraphicsDeviceManager graphics = Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
                    switch (_MouseCursorIndex)
                    {
                        case 0:
                            // movement
                            FormCollection.Cursor.Center = new Vector2(0, 0);
                            FormCollection.Cursor.Texture = Data.Art.GetStaticTexture(8307, graphics.GraphicsDevice);
                            FormCollection.Cursor.SourceRect = new Rectangle(1, 1, 31, 26);
                            break;
                        case _MouseCursorTargetting:
                            // target
                            FormCollection.Cursor.Center = new Vector2(13, 13);
                            FormCollection.Cursor.Texture = Data.Art.GetStaticTexture(8310, graphics.GraphicsDevice);
                            FormCollection.Cursor.SourceRect = new Rectangle(1, 1, 46, 34);
                            break;
                        case _MouseCursorHolding:
                            // holding something.
                            FormCollection.Cursor.Center = new Vector2(0, 0);
                            FormCollection.Cursor.Texture = GUIHelper.ItemIcon(((Item)GUIHelper.MouseHoldingItem).ItemID);
                            FormCollection.Cursor.SourceRect = new Rectangle(0, 0, 64, 64);
                            break;
                        default:
                            // unknown cursor type. Raise an exception!
                            FormCollection.Cursor.Texture = Data.Art.GetStaticTexture(1, graphics.GraphicsDevice);
                            FormCollection.Cursor.SourceRect = new Rectangle(0, 0, 44, 44);
                            break;
                    }
                }
            }
        }
        private bool _TargettingCursor = false;
        public bool TargettingCursor
        {
            get
            {
                return _TargettingCursor;
            }
            set
            {
                // Only change it if we have to...
                if (_TargettingCursor != value)
                {
                    _TargettingCursor = value;
                    if (_TargettingCursor == true)
                    {
                        // If we're carrying something in the mouse cursor...
                        if (MouseCursor == _MouseCursorHolding)
                        {
                            // drop it!
                            GUIHelper.MouseHoldingItem = null;
                        }
                        MouseCursor = _MouseCursorTargetting;
                    }
                    else
                    {
                        // Reset the mouse cursor to default.
                        MouseCursor = 0;
                    }
                }
            }
        }

        public EngineGUI(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGUI), this);
        }

        public override void Initialize()
        {
            GraphicsDeviceManager graphics = Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
            _FontArial14 = Game.Content.Load<SpriteFont>(@"fonts\ArialNarrow10");
            _FormCollection = new FormCollection(Game.Window, Game.Services, ref graphics, @"..\..\res\");
            _SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
            _GameObjectsService = (IEntitiesService)Game.Services.GetService(typeof(IEntitiesService));
            _GameClientService = (Client.IUltimaClient)Game.Services.GetService(typeof(Client.IUltimaClient));
            Events.Initialize(Game.Services);
            GUIHelper.SetObjects(graphics.GraphicsDevice, _FormCollection, Game.Services);
            _GUIWindows = new Dictionary<string, Window>();
            _DrawForms = true;
            MouseCursor = 0;
            base.Initialize();
        }

        protected override void UnloadContent()
        {
            if (_DrawForms)
            {
                //Dispose of the form collection
                _FormCollection.Dispose();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            GUIHelper.Update();

            // Fix for issue 1. http://code.google.com/p/ultimaxna/issues/detail?id=1 --ZDW 6/17/09
            if ((MouseCursor == _MouseCursorHolding) && (GUIHelper.MouseHoldingItem == null))
            {
                MouseCursor = 0;
            }
            else if (GUIHelper.MouseHoldingItem != null)
            {
                MouseCursor = _MouseCursorHolding;
            }

            // First update our collection of windows.
            mUpdateWindows();

            if (_DrawForms)
            {
                lock (_FormCollection)
                {
                    //Update the form collection
                    _FormCollection.Update(gameTime);
                    if (_FormCollection["msgbox"] != null)
                        _FormCollection["msgbox"].Focus();
                    //Render the form collection (required before drawing)
                    _FormCollection.Render();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw the form collection
            if (_DrawForms)
            {
                lock (_FormCollection)
                {
                    _FormCollection.Draw();
                }
            }

            // Draw debug message
            _SpriteBatch.Begin();
            if (DebugMessage != null)
                _SpriteBatch.DrawString(_FontArial14, DebugMessage,
                    new Vector2(5, 5), Color.White, 0f, Vector2.Zero, 1,
                    SpriteEffects.None, 1f);
            // version message
            Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime d = new DateTime(v.Build * TimeSpan.TicksPerDay).AddYears(1999).AddDays(-1);
            string versionString = "UltimaXNA PreAlpha v" + v.Major + "." + v.Minor + Environment.NewLine +
                "Compiled: " + String.Format("{0:MMMM d, yyyy}", d);
            _SpriteBatch.DrawString(_FontArial14, versionString,
                    new Vector2(670, 565), Color.White, 0f, Vector2.Zero, 1,
                    SpriteEffects.None, 1f);
            // tooltip message
            _SpriteBatch.DrawString(_FontArial14, GUIHelper.TooltipMsg,
                    new Vector2(GUIHelper.TooltipX, GUIHelper.TooltipY), Color.White, 0f, Vector2.Zero, 1,
                    SpriteEffects.None, 1f);
            _SpriteBatch.End();

            base.Draw(gameTime);
        }

        public bool IsMouseOverGUI(Vector2 nPosition)
        {
            foreach (Form f in FormCollection.Forms)
            {
                if (!f.IsDisposed)
                    if (f.IsMouseOver())
                        return true;
            }
            return false;
        }

        public Window Window(string nWindowName)
        {
            try { return _GUIWindows[nWindowName]; }
            catch
            {
                // This window is not open.
                return null;
            }
        }

        public void CloseWindow(string nWindowName)
        {
            Window w = _GUIWindows[nWindowName];
            if (w != null)
                w.Close();
        }

        public Window AddWindow(string windowName, Window window)
        {
            if (_GUIWindows.ContainsKey(windowName))
            {
                if (_GUIWindows[windowName].IsClosed)
                {
                    _GUIWindows.Remove(windowName);
                }
                else
                {
                    _GUIWindows[windowName].Close();
                    _GUIWindows.Remove(windowName);
                }
            }
            _GUIWindows.Add(windowName, window);
            return _GUIWindows[windowName];
        }

        public void Reset()
        {
            lock (_FormCollection)
            {
                foreach (KeyValuePair<string, Window> kvp in _GUIWindows)
                {
                    if (!kvp.Key.Contains("Error"))
                        CloseWindow(kvp.Key);
                }
            }
        }

        public void PaperDoll_Open(Entity nMobileObject)
        {
            lock (_FormCollection)
            {
                string iContainerKey = "PaperDoll:" + nMobileObject.Serial;
                if (_GUIWindows.ContainsKey(iContainerKey))
                {
                    // focus the window
                }
                else
                {
                    _GUIWindows.Add(iContainerKey, new Window_PaperDoll(nMobileObject, _FormCollection));
                }
            }
        }

        public void Container_Open(Entity nContainerObject, int nGump)
        {
            lock (_FormCollection)
            {
                string iContainerKey = "Container:" + nContainerObject.Serial;
                if (_GUIWindows.ContainsKey(iContainerKey))
                {
                    // focus the window
                }
                else
                {
                    _GUIWindows.Add(iContainerKey, new Window_Container(nContainerObject, _FormCollection));
                }
            }
        }

        public void Merchant_Open(Entity nContainerObject, int nGump)
        {
            lock (_FormCollection)
            {
                string iContainerKey = "Merchant:" + nContainerObject.Serial;
                if (_GUIWindows.ContainsKey(iContainerKey))
                {
                    // focus the window
                }
                else
                {
                    _GUIWindows.Add(iContainerKey, new Window_Merchant(nContainerObject, _FormCollection));
                }
            }
        }

        public void ErrorPopup_Modal(string nText)
        {
            lock (_FormCollection)
            {
                if (_GUIWindows.ContainsKey("ErrorModal"))
                    _GUIWindows.Remove("ErrorModal");
                _GUIWindows.Add("ErrorModal", new ErrorModal(_FormCollection, nText));
            }
            }


        private void mUpdateWindows()
        {
            lock (_FormCollection)
            {
                bool iMustUpdateWindowList = false;

                foreach (KeyValuePair<string, Window> w in _GUIWindows)
                {
                    if (!(w.Value.IsClosed))
                        w.Value.Update();
                    else
                        iMustUpdateWindowList = true;
                }

                if (iMustUpdateWindowList)
                {
                    Dictionary<string, Window> iGUIWindows = new Dictionary<string, Window>();
                    foreach (KeyValuePair<string, Window> w in _GUIWindows)
                    {
                        if (!w.Value.IsClosed)
                        {
                            iGUIWindows.Add(w.Key, w.Value);
                        }
                    }
                    _GUIWindows = iGUIWindows;
                }
            }
        }
    }
}