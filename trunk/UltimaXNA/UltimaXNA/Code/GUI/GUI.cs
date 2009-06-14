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
        void AddChatText(string nText);
        void LoadInWorldGUI();
        void LoadLoginGUI();
        void Container_Open(GameObjects.BaseObject nContainerObject, int nGump);
        void ErrorPopup_Modal(string nText);
    }

    public class EngineGUI : DrawableGameComponent, IGUI
    {
        SpriteFont fontArial14;
        SpriteBatch spriteBatch;
        private FormCollection formCollection;
        private bool mDrawForms = false;
        public string DebugMessage;
        private Texture2D surfaceCursors;

        private Dictionary<string, Window> m_GUIWindows;

        public EngineGUI(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGUI), this);
        }

        public override void Initialize()
        {
            base.Initialize();

            m_GUIWindows = new Dictionary<string, Window>();
            
            Events.Initialize(Game.Services);
            GraphicsDeviceManager graphics = Game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
            formCollection = new FormCollection(Game.Window, Game.Services, ref graphics, @"..\..\res\");
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            mDrawForms = true;
            surfaceCursors = DataLocal.Art.GetStaticTexture(8307, graphics.GraphicsDevice);
                // Texture2D.FromFile(graphics.GraphicsDevice, @"..\..\res\GUI\cursors.png");

            fontArial14 = Game.Content.Load<SpriteFont>(@"fonts\ArialNarrow10");
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

            FormCollection.Cursor.Texture = surfaceCursors;
            FormCollection.Cursor.SourceRect = new Rectangle(1, 1, 31, 26);
            FormCollection.Cursor.HasShadow = true;

            // First update our collection of windows.
            mUpdateWindows();

            if (mDrawForms)
            {
                //Update the form collection
                formCollection.Update(gameTime);
                //Render the form collection (required before drawing)
                formCollection.Render();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw the form collection
            if (mDrawForms)
                formCollection.Draw();

            // Draw debug message
            spriteBatch.Begin();
            if (DebugMessage != null)
                spriteBatch.DrawString(fontArial14, DebugMessage,
                    new Vector2(10, 10), Color.White, 0f, Vector2.Zero, 1,
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

        public void AddChatText(string nText)
        {
            ((Window_Chat)m_GUIWindows["ChatFrame"]).AddText(nText);
        }

        public void LoadInWorldGUI()
        {
            m_GUIWindows["LoginBG"].Close();
            m_GUIWindows["LoginWindow"].Close();
            m_GUIWindows.Add("ChatFrame", new Window_Chat(formCollection));
        }

        public void LoadLoginGUI()
        {
            if (!m_GUIWindows.ContainsKey("LoginWindow"))
            {
                m_GUIWindows.Clear();
                m_GUIWindows.Add("LoginBG", new Window_LoginBG(formCollection));
                m_GUIWindows.Add("LoginWindow", new Window_Login(formCollection));
            }
        }

        public void Container_Open(GameObjects.BaseObject nContainerObject, int nGump)
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

        public void ErrorPopup_Modal(string nText)
        {
            m_GUIWindows.Add("ErrorModal", new ErrorModal(formCollection, nText));
        }

        private void mUpdateWindows()
        {
            bool iMustUpdateWindowList = false;

            foreach (KeyValuePair<string, Window> w in m_GUIWindows)
            {
                w.Value.Update();
                if (w.Value.IsClosed)
                {
                    iMustUpdateWindowList = true;
                }
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