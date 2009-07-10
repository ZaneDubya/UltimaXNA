#region File Description & Usings
//-----------------------------------------------------------------------------
// GUI.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;
#endregion

namespace UltimaXNA.GUI
{
    public interface IGUI
    {
        bool IsMouseOverGUI(Vector2 nPosition);
        void LoadInWorldGUI();
        void LoadLoginGUI();
        void Container_Open(GameObjects.BaseObject nContainerObject, int nGump);
        void Merchant_Open(GameObjects.BaseObject nContainerObject, int nGump);
        void PaperDoll_Open(GameObjects.BaseObject nMobileObject);
        void ErrorPopup_Modal(string nText);
        Window Window(string nWindowName);
        void CloseWindow(string nWindowName);
        bool TargettingCursor { get; set; }
        void Reset();
    }

    public class EngineGUI : DrawableGameComponent, IGUI
    {
        SpriteFont fontArial14;
        SpriteBatch spriteBatch;
        private FormCollection formCollection;
        private bool mDrawForms = false;
        public string DebugMessage;

        private Dictionary<string, Window> m_GUIWindows;
        GameObjects.IGameObjects m_GameObjectsService;
        Client.IUltimaClient m_GameClientService;

        private int m_MouseCursorIndex = int.MinValue; // set so it is always properly initialized to zero in Initialize();
        private const int m_MouseCursorHolding = int.MaxValue;
        private const int m_MouseCursorTargetting = int.MaxValue - 1;
        public int MouseCursor
        {
            get { return m_MouseCursorIndex; }
            set
            {
                // Only change the mouse cursor when you need to.
                if (m_MouseCursorIndex != value)
                {
                    m_MouseCursorIndex = value;
                    GraphicsDeviceManager graphics = Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
                    switch (m_MouseCursorIndex)
                    {
                        case 0:
                            // movement
                            FormCollection.Cursor.Center = new Vector2(0, 0);
                            FormCollection.Cursor.Texture = Data.Art.GetStaticTexture(8307, graphics.GraphicsDevice);
                            FormCollection.Cursor.SourceRect = new Rectangle(1, 1, 31, 26);
                            break;
                        case m_MouseCursorTargetting:
                            // target
                            FormCollection.Cursor.Center = new Vector2(13, 13);
                            FormCollection.Cursor.Texture = Data.Art.GetStaticTexture(8310, graphics.GraphicsDevice);
                            FormCollection.Cursor.SourceRect = new Rectangle(1, 1, 46, 34);
                            break;
                        case m_MouseCursorHolding:
                            // holding something.
                            FormCollection.Cursor.Center = new Vector2(0, 0);
                            FormCollection.Cursor.Texture = GUIHelper.GetItemIcon(((GameObjects.GameObject)GUIHelper.MouseHoldingItem).ObjectTypeID);
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
        private bool m_TargettingCursor = false;
        public bool TargettingCursor
        {
            get
            {
                return m_TargettingCursor;
            }
            set
            {
                // Only change it if we have to...
                if (m_TargettingCursor != value)
                {
                    m_TargettingCursor = value;
                    if (m_TargettingCursor == true)
                    {
                        // If we're carrying something in the mouse cursor...
                        if (MouseCursor == m_MouseCursorHolding)
                        {
                            // drop it!
                            GUIHelper.MouseHoldingItem = null;
                        }
                        MouseCursor = m_MouseCursorTargetting;
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
            fontArial14 = Game.Content.Load<SpriteFont>(@"fonts\ArialNarrow10");
            formCollection = new FormCollection(Game.Window, Game.Services, ref graphics, @"..\..\res\");
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            m_GameObjectsService = (GameObjects.IGameObjects)Game.Services.GetService(typeof(GameObjects.IGameObjects));
            m_GameClientService = (Client.IUltimaClient)Game.Services.GetService(typeof(Client.IUltimaClient));
            Events.Initialize(Game.Services);
            GUIHelper.SetObjects(graphics.GraphicsDevice, this, m_GameClientService, m_GameObjectsService);
            m_GUIWindows = new Dictionary<string, Window>();
            mDrawForms = true;
            MouseCursor = 0;
            base.Initialize();
        }

        protected override void UnloadContent()
        {
            if (mDrawForms)
            {
                //Dispose of the form collection
                formCollection.Dispose();
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Fix for issue 1. http://code.google.com/p/ultimaxna/issues/detail?id=1 --ZDW 6/17/09
            if ((MouseCursor == m_MouseCursorHolding) && (GUIHelper.MouseHoldingItem == null))
            {
                MouseCursor = 0;
            }
            else if (GUIHelper.MouseHoldingItem != null)
            {
                MouseCursor = m_MouseCursorHolding;
            }

            // First update our collection of windows.
            mUpdateWindows();

            if (mDrawForms)
            {
                lock (formCollection)
                {
                    //Update the form collection
                    formCollection.Update(gameTime);
                    if (formCollection["msgbox"] != null)
                        formCollection["msgbox"].Focus();
                    //Render the form collection (required before drawing)
                    formCollection.Render();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw the form collection
            if (mDrawForms)
            {
                lock (formCollection)
                {
                    formCollection.Draw();
                }
            }

            // Draw debug message
            spriteBatch.Begin();
            if (DebugMessage != null)
                spriteBatch.DrawString(fontArial14, DebugMessage,
                    new Vector2(5, 58), Color.White, 0f, Vector2.Zero, 1,
                    SpriteEffects.None, 1f);
            spriteBatch.DrawString(fontArial14, GUIHelper.TooltipMsg,
                    new Vector2(GUIHelper.TooltipX, GUIHelper.TooltipY), Color.White, 0f, Vector2.Zero, 1,
                    SpriteEffects.None, 1f);
            spriteBatch.End();

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
            try { return m_GUIWindows[nWindowName]; }
            catch
            {
                // This window is not open.
                return null;
            }
        }

        public void CloseWindow(string nWindowName)
        {
            Window w = m_GUIWindows[nWindowName];
            if (w != null)
                w.Close();
        }

        public void Reset()
        {
            lock (formCollection)
            {
                foreach (KeyValuePair<string, Window> kvp in m_GUIWindows)
                {
                    if (!kvp.Key.Contains("Error"))
                        CloseWindow(kvp.Key);
                }
            }
        }

        public void LoadInWorldGUI()
        {
            lock (formCollection)
            {
                m_GUIWindows["LoginBG"].Close();
                m_GUIWindows["LoginWindow"].Close();
                m_GUIWindows.Add("ChatFrame", new Window_Chat(formCollection));
                m_GUIWindows.Add("ChatInput", new Window_ChatInput(formCollection));
                m_GUIWindows.Add("StatusFrame", new Window_StatusFrame(formCollection));
            }
        }

        public void LoadLoginGUI()
        {
            lock (formCollection)
            {
                if (!m_GUIWindows.ContainsKey("LoginWindow"))
                {
                    this.Reset();
                    m_GUIWindows.Add("LoginBG", new Window_LoginBG(formCollection));
                    m_GUIWindows.Add("LoginWindow", new Window_Login(formCollection));
                }
            }
        }

        public void PaperDoll_Open(GameObjects.BaseObject nMobileObject)
        {
            lock (formCollection)
            {
                string iContainerKey = "PaperDoll:" + nMobileObject.GUID;
                if (m_GUIWindows.ContainsKey(iContainerKey))
                {
                    // focus the window
                }
                else
                {
                    m_GUIWindows.Add(iContainerKey, new Window_PaperDoll(nMobileObject, formCollection));
                }
            }
        }

        public void Container_Open(GameObjects.BaseObject nContainerObject, int nGump)
        {
            lock (formCollection)
            {
                string iContainerKey = "Container:" + nContainerObject.GUID;
                if (m_GUIWindows.ContainsKey(iContainerKey))
                {
                    // focus the window
                }
                else
                {
                    m_GUIWindows.Add(iContainerKey, new Window_Container(nContainerObject, formCollection));
                }
            }
        }

        public void Merchant_Open(GameObjects.BaseObject nContainerObject, int nGump)
        {
            lock (formCollection)
            {
                string iContainerKey = "Merchant:" + nContainerObject.GUID;
                if (m_GUIWindows.ContainsKey(iContainerKey))
                {
                    // focus the window
                }
                else
                {
                    m_GUIWindows.Add(iContainerKey, new Window_Merchant(nContainerObject, formCollection));
                }
            }
        }

        public void ErrorPopup_Modal(string nText)
        {
            lock (formCollection)
            {
                if (m_GUIWindows.ContainsKey("ErrorModal"))
                    m_GUIWindows.Remove("ErrorModal");
                m_GUIWindows.Add("ErrorModal", new ErrorModal(formCollection, nText));
            }
            }


        private void mUpdateWindows()
        {
            lock (formCollection)
            {
                bool iMustUpdateWindowList = false;

                foreach (KeyValuePair<string, Window> w in m_GUIWindows)
                {
                    if (!(w.Value.IsClosed))
                        w.Value.Update();
                    else
                        iMustUpdateWindowList = true;
                }

                if (iMustUpdateWindowList)
                {
                    Dictionary<string, Window> iGUIWindows = new Dictionary<string, Window>();
                    foreach (KeyValuePair<string, Window> w in m_GUIWindows)
                    {
                        if (!w.Value.IsClosed)
                        {
                            iGUIWindows.Add(w.Key, w.Value);
                        }
                    }
                    m_GUIWindows = iGUIWindows;
                }
            }
        }
    }
}