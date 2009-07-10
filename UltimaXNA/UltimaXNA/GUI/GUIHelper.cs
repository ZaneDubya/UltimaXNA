using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;
using UltimaXNA.Network.Packets.Client;

namespace UltimaXNA.GUI
{
    static class GUIHelper
    {
        private static bool m_IsPrepared = false;
        private static EngineGUI m_GUI = null;
        private static Client.IUltimaClient m_GameClientService = null;
        private static GameObjects.IGameObjects m_GameObjectsService = null;
        private static Texture2D m_TextureBorder = null;
        private static Texture2D m_TextureBG = null;
        private static Texture2D m_TextureEmpty = null;
        private static RenderTarget2D m_RenderTarget;
        public static Texture2D[] m_IconCache;
        private static GraphicsDevice m_GraphicsDevice = null;

        public static string TooltipMsg = "";
        public static int TooltipX = 0;
        public static int TooltipY = 0;

        private static List<string> m_QueuedChatText = new List<string>();

        public static GameObjects.BaseObject MouseHoldingItem;
        private static GameObjects.GameObject mToolTipItem;

        public static void Network_SendChat(string nChatText)
        {
            if (nChatText != string.Empty)
                m_GameClientService.Send(new UnicodeSpeechPacket(0, 0, "ENU", nChatText));
        }

        public static void Chat_AddLine(string nChatText)
        {
            // add text to the chat window.
            try
            {
                ((Window_Chat)m_GUI.Window("ChatFrame")).AddText(nChatText);
            }
            catch
            {
                m_QueuedChatText.Add(nChatText);
                // The chat frame isn't loaded yet. Save this text for later.
            }
        }

        public static void Update()
        {
            if (m_QueuedChatText.Count > 0)
            {
                Window_Chat w = m_GUI.Window("ChatFrame") as Window_Chat;
                if (w != null)
                {
                    foreach (string s in m_QueuedChatText)
                    {
                        w.AddText(s);
                    }
                    m_QueuedChatText.Clear();
                }
            }
        }

        public static void PickUpItem(GameObjects.GameObject nObject)
        {
            MouseHoldingItem = nObject;
        }

        public static void DropItemIntoSlot(GameObjects.BaseObject nDestContainer, int nDestSlot)
        {
            GameObjects.GameObject iHeldObject = (GameObjects.GameObject)MouseHoldingItem;
            GameObjects.GameObject iDestContainer = (GameObjects.GameObject)nDestContainer;

            if (iHeldObject.Wearer != null)
            {
                // the object is being worn as equipment. We can only remove items that belong to us.
                if (iHeldObject.Wearer.GUID == m_GameObjectsService.MyGUID)
                {
                    // we are wearing this item. Go ahead and drop it into the requested slot.
                    m_GameClientService.Send(new PickupItemPacket(iHeldObject.GUID, (short)0));
                    m_GameClientService.Send(new DropItemPacket(iHeldObject.GUID, 0, 0,
                        0, 0, iDestContainer.GUID));
                }
                else
                {
                    // this object is being worn by someone else. We can't move it.
                }
            }
            else
            {
                if (iHeldObject.Item_ContainedWithinGUID == iDestContainer.GUID)
                {
                    iDestContainer.ContainerObject.Event_MoveItemToSlot(iHeldObject, nDestSlot);
                }
                else
                {
                    if (iDestContainer.ContainerObject.GetContents(nDestSlot) == null)
                    {
                        m_GameClientService.Send(new PickupItemPacket(iHeldObject.GUID, (short)0));
                        m_GameClientService.Send(new DropItemPacket(iHeldObject.GUID, (short)nDestSlot, (short)(nDestSlot ^ 0x7fff),
                            0, 0, iDestContainer.GUID));
                    }
                    else
                    {
                        m_GameClientService.Send(new PickupItemPacket(iHeldObject.GUID, (short)0));
                        m_GameClientService.Send(new DropItemPacket(iHeldObject.GUID, (short)0, (short)0,
                            0, 0, iDestContainer.GUID));
                    }
                }
            }
            GUI.GUIHelper.MouseHoldingItem = null;
        }

        public static void WearItem(GameObjects.BaseObject nDestMobile, int nDestSlot)
        {
            GameObjects.GameObject iHeldObject = (GameObjects.GameObject)MouseHoldingItem;
            GameObjects.Unit iDestMobile = (GameObjects.Unit)nDestMobile;

            if (iHeldObject.Item_ContainedWithinGUID != 0)
            {
                GameObjects.GameObject iSourceContainer = (GameObjects.GameObject)m_GameObjectsService.GetObject(iHeldObject.Item_ContainedWithinGUID, UltimaXNA.GameObjects.ObjectType.GameObject);
                iHeldObject.Item_ContainedWithinGUID = 0;
                iSourceContainer.ContainerObject.RemoveItem(iHeldObject.GUID);
            }
            // m_GameClientService.Send(new PickupItemPacket(iHeldObject.GUID, (short)0));
            m_GameClientService.Send(new DropToLayerPacket(iHeldObject.GUID, (byte)nDestSlot, iDestMobile.GUID));

            GUI.GUIHelper.MouseHoldingItem = null;
        }

        public static void BuyItemFromVendor(int nVendorGUID, int nItemGUID, int nAmount)
        {
            // Due to the way that the RunUO server ( and all UO servers ) are set up,
            // if we want to buy one item at a item, we need to:
            // 1. Buy the item.
            // 2. Re-request the context menu.
            // 3. Send the context menu response for buying. (handled automatically)
            Network.Pair<int, short>[] iBuyItem = new Network.Pair<int, short>[1];
            iBuyItem[0].ItemA = nItemGUID;
            iBuyItem[0].ItemB = (short)nAmount;
            m_GameClientService.Send(new BuyItemsPacket(nVendorGUID, iBuyItem));
            m_GameClientService.Send(new RequestContextMenuPacket(nVendorGUID));
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
                    Data.ItemData iData = mToolTipItem.ItemData;
                    string iItemName = string.Empty;
                    if (mToolTipItem.Item_StackCount > 1)
                        iItemName += mToolTipItem.Item_StackCount.ToString() + @" ";
                    if (iData.Name.Contains(@"%"))
                    {
                        iItemName += mToolTipItem.Item_StackCount.ToString() + @" ";
                        int iMultiplePosition = iData.Name.IndexOf(@"%");
                        string iBaseString = iData.Name.Substring(0, iMultiplePosition);
                        string iMultipleString = iData.Name.Substring(iMultiplePosition + 1, iData.Name.IndexOf(@"%", iMultiplePosition + 1) - iMultiplePosition - 1);
                        iItemName = mToolTipItem.Item_StackCount.ToString() + @" " + iBaseString;
                        if (mToolTipItem.Item_StackCount > 1)
                            iItemName += iMultipleString;
                    }
                    else
                    {
                        iItemName += iData.Name;
                    }
                    TooltipMsg = iItemName + Environment.NewLine +
                        "GUID:" + GUIDHex(mToolTipItem.GUID);
                    if (mToolTipItem.PropertyList.HasProperties)
                    {
                        TooltipMsg += Environment.NewLine +
                            mToolTipItem.PropertyList.Properties;
                    }
                }
            }
            get
            {
                return mToolTipItem;
            }
        }

        public static string GUIDHex(int nGUID)
        {
            return MiscUtil.HexEncoding.ToString(
                MiscUtil.Conversion.EndianBitConverter.Big.GetBytes(nGUID)
            );
        }

        public static void SetObjects(
            GraphicsDevice nDevice, EngineGUI nGUI, Client.IUltimaClient nGameClientService, GameObjects.IGameObjects nGameObjectsService)
        {
            m_GraphicsDevice = nDevice;
            m_GUI = nGUI;
            m_GameClientService = nGameClientService;
            m_GameObjectsService = nGameObjectsService;
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
                    iTextureArt = Data.Art.GetStaticTexture(nItemID, FormCollection.Graphics.GraphicsDevice);

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
                    sprite.Begin(SpriteBlendMode.AlphaBlend);

                    if (nItemID == 0)
                    {
                        sprite.Draw(m_TextureEmpty, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);
                    }
                    else
                    {
                        sprite.Draw(m_TextureBG, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);

                        if (iTextureArt != null)
                        {
                            Rectangle iSrcRect = new Rectangle(
                                    -(int)((iTextureArt.Width * iScaleUp) - iDestSize) / 2,
                                    -(int)(((iTextureArt.Height) * iScaleUp) - iDestSize) / 2,
                                    (int)(iTextureArt.Width * iScaleUp),
                                    (int)(iTextureArt.Height * iScaleUp));
                            sprite.Draw(iTextureArt,
                                new Rectangle(1, 1, 37, 37),
                                iSrcRect,
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
