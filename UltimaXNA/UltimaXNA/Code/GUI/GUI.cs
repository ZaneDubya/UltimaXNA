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
        void ErrorPopup_Modal(string nText);
        Window Window(string nWindowName);
        void CloseWindow(string nWindowName);
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
        GameObjects.IGameObjects m_GameObjectsService;
        Network.IGameClient m_GameClientService;

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

            fontArial14 = Game.Content.Load<SpriteFont>(@"fonts\ArialNarrow10");

            m_GameObjectsService = (GameObjects.IGameObjects)Game.Services.GetService(typeof(GameObjects.IGameObjects));
            m_GameClientService = (Network.IGameClient)Game.Services.GetService(typeof(Network.IGameClient));

            GUIHelper.SetObjects(graphics.GraphicsDevice, this, m_GameClientService);
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
            if (GUIHelper.MouseHoldingItem == null)
            {
                FormCollection.Cursor.Texture = surfaceCursors;
                FormCollection.Cursor.SourceRect = new Rectangle(1, 1, 31, 26);
                FormCollection.Cursor.HasShadow = true;
            }
            else
            {
                FormCollection.Cursor.Texture = GUIHelper.GetItemIcon(((GameObjects.GameObject)GUIHelper.MouseHoldingItem).ObjectTypeID);
                FormCollection.Cursor.SourceRect = new Rectangle(0, 0, 64, 64);
                FormCollection.Cursor.HasShadow = true;
            }

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

        public void LoadInWorldGUI()
        {
            m_GUIWindows["LoginBG"].Close();
            m_GUIWindows["LoginWindow"].Close();
            m_GUIWindows.Add("ChatFrame", new Window_Chat(formCollection));
            m_GUIWindows.Add("ChatInput", new Window_ChatInput(formCollection));
            m_GUIWindows.Add("StatusFrame", new Window_StatusFrame(formCollection));
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

        public void Merchant_Open(GameObjects.BaseObject nContainerObject, int nGump)
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

    delegate void GUIEVENT_DropItemIntoSlot(GameObjects.BaseObject nHeldObject, GameObjects.BaseObject nDestContainer, int nDestSlot);
    delegate void GUIEVENT_BuyItemFromVendor(int nVendorGUID, int nItemGUID, int nAmount);

    static class GUIHelper
    {
        private static bool m_IsPrepared = false;
        private static EngineGUI m_GUI = null;
        private static Network.IGameClient m_GameClientService = null;
        private static Texture2D m_TextureBorder = null;
        private static Texture2D m_TextureBG = null;
        private static Texture2D m_TextureEmpty = null;
        private static RenderTarget2D m_RenderTarget;
        public static Texture2D[] m_IconCache;
        private static GraphicsDevice m_GraphicsDevice = null;

        public static string TooltipMsg = "";
        public static int TooltipX = 0;
        public static int TooltipY = 0;
        
        public static GameObjects.BaseObject MouseHoldingItem;
        private static GameObjects.GameObject mToolTipItem;

        public static void Network_SendChat(string nChatText)
        {
            m_GameClientService.Send_ChatMsg(nChatText);
        }

        public static void Chat_AddLine(string nChatText)
        {
            // add text.
            ((Window_Chat)m_GUI.Window("ChatFrame")).AddText(nChatText);
        }

        public static void PickUpItem(GameObjects.GameObject nObject)
        {
            MouseHoldingItem = nObject;
        }

        public static void DropItemIntoSlot(GameObjects.BaseObject nDestContainer, int nDestSlot)
        {
            GameObjects.GameObject iHeldObject = (GameObjects.GameObject)MouseHoldingItem;
            GameObjects.GameObject iDestContainer = (GameObjects.GameObject)nDestContainer;

            if (iHeldObject.Item_ContainedWithinGUID == iDestContainer.GUID)
            {
                iDestContainer.ContainerObject.Event_MoveItemToSlot(iHeldObject, nDestSlot);
            }
            else
            {
                m_GameClientService.Send_DropItem(iHeldObject.GUID, 0, 0, 0, iDestContainer.GUID);
                // throw (new Exception("No support for moving items between containers."));
            }
        }

        public static void BuyItemFromVendor(int nVendorGUID, int nItemGUID, int nAmount)
        {
            // Due to the way that the RunUO server ( and all UO servers ) are set up,
            // if we want to buy one item at a item, we need to:
            // 1. Buy the item.
            // 2. Re-request the context menu.
            // 3. Send the context menu response for buying. (handled automatically)
            m_GameClientService.Send_BuyItemFromVendor(nVendorGUID, nItemGUID, nAmount);
            m_GameClientService.Send_RequestContextMenu(nVendorGUID);
        }

        public static GameObjects.GameObject ToolTipItem
        {
            set
            {
                if (value == null)
                {
                    mToolTipItem = null;
                    TooltipMsg = String.Empty;
                }
                else
                {
                    mToolTipItem = value;
                    DataLocal.ItemData iData = mToolTipItem.ItemData;
                    TooltipMsg = iData.Name +
                        Environment.NewLine + "GUID:" + mToolTipItem.GUID.ToString();
                }
            }
            get
            {
                return mToolTipItem;
            }
        }

        public static void SetObjects(GraphicsDevice nDevice, EngineGUI nGUI, Network.IGameClient nGameClientService)
        {
            m_GraphicsDevice = nDevice;
            m_GUI = nGUI;
            m_GameClientService = nGameClientService;
        }

        private static void m_PrepareHelper()
        {
            m_TextureBorder = Texture2D.FromFile(m_GraphicsDevice,
               FormCollection.ContentManager.RootDirectory + @"GUI\COMMON\UI-Slot-Border.png");
            m_TextureBG = Texture2D.FromFile(m_GraphicsDevice,
                FormCollection.ContentManager.RootDirectory + @"GUI\COMMON\UI-Slot-BG.png");
            m_TextureEmpty = Texture2D.FromFile(m_GraphicsDevice,
                FormCollection.ContentManager.RootDirectory + @"GUI\COMMON\UI-Slot-Empty.png");
            m_RenderTarget = new RenderTarget2D(m_GraphicsDevice, 64, 64, 0, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            m_IconCache = new Texture2D[0x4000];
            m_IsPrepared = true;
        }

        public static Texture2D GetItemIcon(int nItemID)
        {
            if (!m_IsPrepared)
                m_PrepareHelper();

            if (m_IconCache[nItemID] == null)
            {
                // Retrieve the TextureArt from DataLocal for this ItemID. ItemID = 0 is empty.
                Texture2D iTextureArt = null;
                if (nItemID != 0)
                    iTextureArt = DataLocal.Art.GetStaticTexture(nItemID, FormCollection.Graphics.GraphicsDevice);

                float iScaleUp = 1f, iDestSize = 39f;
                if (iTextureArt != null)
                {
                    if (iTextureArt.Width > iTextureArt.Height)
                        iScaleUp = iDestSize / iTextureArt.Height;
                    else
                        iScaleUp = iDestSize / iTextureArt.Width;
                }

                m_GraphicsDevice.SetRenderTarget(0, m_RenderTarget);
                m_GraphicsDevice.Clear(Color.TransparentBlack);

                using (SpriteBatch sprite = new SpriteBatch(m_GraphicsDevice))
                {
                    sprite.Begin();

                    if (nItemID == 0)
                    {
                        sprite.Draw(m_TextureEmpty, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);
                    }
                    else
                    {
                        sprite.Draw(m_TextureBG, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);

                        if (iTextureArt != null)
                        {
                            sprite.Draw(iTextureArt,
                                new Rectangle(
                                    -(int)((iTextureArt.Width * iScaleUp) - iDestSize) / 2,
                                    -(int)(((iTextureArt.Height - 6) * iScaleUp) - iDestSize) / 2,
                                    (int)(iTextureArt.Width * iScaleUp),
                                    (int)(iTextureArt.Height * iScaleUp)),
                                Color.White);
                        }

                        sprite.Draw(m_TextureBorder, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);
                    }
                    sprite.End();
                }
                
                m_GraphicsDevice.SetRenderTarget(0, null);
                Texture2D iTexture = new Texture2D(m_GraphicsDevice, 64, 64);
                int[] iData = new int[64 * 64];
                m_RenderTarget.GetTexture().GetData<int>(iData);
                iTexture.SetData<int>(iData);
                m_IconCache[nItemID] = iTexture;
            }
            return m_IconCache[nItemID];
        }
    }
}