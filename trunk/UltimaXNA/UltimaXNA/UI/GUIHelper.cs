/***************************************************************************
 *   UIHelper.cs
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
using xWinFormsLib;
using System.IO;
using UltimaXNA.Entities;
using UltimaXNA.Network.Packets.Client;
#endregion

namespace UltimaXNA.UI
{
    public static class UIHelper
    {
        private static FormCollection _formCollection;
        private static GraphicsDevice _graphicsDevice;
        public static FormCollection FormCollection { get { return _formCollection; } }

        private static bool _IsPrepared = false;
        private static Texture2D _TextureBorder = null;
        private static Texture2D _TextureBG = null;
        private static Texture2D _TextureEmpty = null;
        private static RenderTarget2D _RenderTarget;
        private static Dictionary<int, Texture2D> _IconCache;

        public static string TooltipMsg = string.Empty;
        public static int TooltipX = 0;
        public static int TooltipY = 0;

        private static List<string> _queuedChatText = new List<string>();

        public static Item MouseHoldingItem;
        private static Item _ToolTipItem;

        public static void SetObjects(GraphicsDevice graphicsDevice, FormCollection formCollection)
        {
            _graphicsDevice = graphicsDevice;
            _formCollection = formCollection;
        }

        private static void _PrepareHelper()
        {
            _TextureBorder = Texture2D.FromFile(_graphicsDevice,
               FormCollection.ContentManager.RootDirectory + @"GUI\COMMON\UI-Slot-Border.png");
            _TextureBG = Texture2D.FromFile(_graphicsDevice,
                FormCollection.ContentManager.RootDirectory + @"GUI\COMMON\UI-Slot-BG.png");
            _TextureEmpty = Texture2D.FromFile(_graphicsDevice,
                FormCollection.ContentManager.RootDirectory + @"GUI\COMMON\UI-Slot-Empty.png");
            _RenderTarget = new RenderTarget2D(_graphicsDevice, 64, 64, 0, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            _IconCache = new Dictionary<int, Texture2D>();
            _IsPrepared = true;
        }

        public static Texture2D LoadTexture(string filepath)
        {
            return Texture2D.FromFile(_graphicsDevice, FormCollection.ContentManager.RootDirectory + filepath);
        }

        /*
        public static void Network_SendChat(string nChatText)
        {
            if (nChatText != string.Empty)
                Client.UltimaClient.Send(new UnicodeSpeechPacket(0, 0, "ENU", nChatText));
        }

        public static void Chat_AddLine(string nChatText)
        {
            // add text to the chat window.
            try
            {
                // ((Window_Chat)UserInterface.Window("ChatFrame")).AddText(nChatText);
            }
            catch
            {
                // The chat frame isn't loaded yet. Save this text for later.
                _queuedChatText.Add(nChatText);
            }
        }
        */

        public static Texture2D MiniMapTexture
        {
            get 
            {
                return null;
            }
        }

        public static void Update()
        {
            if (_queuedChatText.Count > 0)
            {
                /*
                Window_Chat w = UserInterface.Window("ChatFrame") as Window_Chat;
                if (w != null)
                {
                    foreach (string s in _queuedChatText)
                    {
                        w.AddText(s);
                    }
                    _queuedChatText.Clear();
                }*/
            }
        }

        public static void PickUpItem(Item item)
        {
            Events.PickupItem(item);
            MouseHoldingItem = item;
        }

        public static void DropItemIntoSlot(Entity nDestContainer, int nDestSlot)
        {
            /*
            Item iHeldObject = (Item)MouseHoldingItem;
            Container iDestContainer = (Container)nDestContainer;

            if (iHeldObject.Wearer != null)
            {
                // the object is being worn as equipment. We can only remove items that belong to us.
                if (iHeldObject.Wearer.Serial == Entities.EntitiesCollection.MySerial)
                {
                    // we are wearing this item. Go ahead and drop it into the requested slot.
                    UI.Events.DropItem(iHeldObject, nDestSlot, 0x7FFF, 0, iDestContainer.Serial);
                }
                else
                {
                    // this object is being worn by someone else. We can't move it.
                }
            }
            else
            {
                
                /*if (iHeldObject.Item_ContainedWithinSerial == iDestContainer.Serial)
                {
                    // moving between slots in a single container.
                    iDestContainer.AddItem(iHeldObject);
                }
                else
                {
                    // moving between two containers, or moving from ground to a container.
                    /*
                    if (iDestContainer.Contents.GetContents(nDestSlot) == null)
                    {
                        // dest slot is empty.
                        if (iHeldObject.Item_ContainedWithinSerial.IsValid)
                        {
                            Entities.EntitiesCollection.GetObject<Container>(iHeldObject.Item_ContainedWithinSerial, false).Contents.RemoveItem(iHeldObject.Serial);
                        }
                        UI.Events.DropItem(iHeldObject, nDestSlot, 0x7FFF, 0, iDestContainer.Serial);
                    }
                    else
                    {
                        if (iHeldObject.Item_ContainedWithinSerial.IsValid)
                        {
                            Entities.EntitiesCollection.GetObject<ContainerItem>(iHeldObject.Item_ContainedWithinSerial, false).Contents.RemoveItem(iHeldObject.Serial);
                        }
                        UI.Events.DropItem(iHeldObject, 0, 0, 0, iDestContainer.Serial);
                    }*/
                // }
            // }
            // UI.UIHelper.MouseHoldingItem = null;
        }

        public static void DropItemOntoGround(int x, int y, int z)
        {
            int groundSerial = unchecked((int)0xffffffff);
            Item iHeldObject = (Item)MouseHoldingItem;
            UI.Events.DropItem(iHeldObject, x, y, z, groundSerial);
            UI.UIHelper.MouseHoldingItem = null;
        }

        public static void WearItem(Entity nDestMobile, int nDestSlot)
        {
            /*
            Item iHeldObject = (Item)MouseHoldingItem;
            Mobile iDestMobile = (Mobile)nDestMobile;

            if (iHeldObject.Item_ContainedWithinSerial.IsValid)
            {
                Container iSourceContainer = Entities.EntitiesCollection.GetObject<Container>(iHeldObject.Item_ContainedWithinSerial, false);
                iHeldObject.Item_ContainedWithinSerial = 0;
                iSourceContainer.RemoveItem(iHeldObject);
            }
            Client.UltimaClient.Send(new DropToLayerPacket(iHeldObject.Serial, (byte)nDestSlot, iDestMobile.Serial));

            UI.UIHelper.MouseHoldingItem = null;*/
        }

        public static void BuyItemFromVendor(int nVendorSerial, int nItemSerial, int nAmount)
        {
            // Due to the way that the RunUO server ( and all UO servers ) are set up,
            // if we want to buy one item at a item, we need to:
            // 1. Buy the item.
            // 2. Re-request the context menu.
            // 3. Send the context menu response for buying. (handled automatically)
            Network.Pair<int, short>[] iBuyItem = new Network.Pair<int, short>[1];
            iBuyItem[0].ItemA = nItemSerial;
            iBuyItem[0].ItemB = (short)nAmount;
            Client.UltimaClient.Send(new BuyItemsPacket(nVendorSerial, iBuyItem));
            Client.UltimaClient.Send(new RequestContextMenuPacket(nVendorSerial));
        }

        public static Item ToolTipItem
        {
            set
            {
                if (value == null)
                {
                    _ToolTipItem = null;
                    TooltipMsg = String.Empty;
                }
                else
                {
                    _ToolTipItem = value;
                    if (_ToolTipItem.PropertyList.HasProperties)
                    {
                        TooltipMsg =
                            _ToolTipItem.PropertyList.Properties;
                    }
                    else
                    {
                        Data.ItemData iData = _ToolTipItem.ItemData;
                        string iItemName = string.Empty;
                        if (_ToolTipItem.Amount > 1)
                            iItemName += _ToolTipItem.Amount.ToString() + @" ";
                        if (iData.Name.Contains(@"%"))
                        {
                            iItemName += _ToolTipItem.Amount.ToString() + @" ";
                            int iMultiplePosition = iData.Name.IndexOf(@"%");
                            string iBaseString = iData.Name.Substring(0, iMultiplePosition);
                            int length = iData.Name.IndexOf(@"%", iMultiplePosition + 1) - iMultiplePosition - 1;
                            string iMultipleString = string.Empty;
                            if (length > 0)
                                iMultipleString = iData.Name.Substring(iMultiplePosition + 1, length);
                            iItemName = _ToolTipItem.Amount.ToString() + @" " + iBaseString;
                            if (_ToolTipItem.Amount > 1)
                                iItemName += iMultipleString;
                        }
                        else
                        {
                            iItemName += iData.Name;
                        }
                    }
                    TooltipMsg += Environment.NewLine + "Serial:" + _ToolTipItem.Serial;
                }
            }
            get
            {
                return _ToolTipItem;
            }
        }

        public static Texture2D TextTexture(string text, int fontId, int hueId)
        {
            if (!_IsPrepared)
                _PrepareHelper();
            return Data.ASCIIText.GetTextTexture(text, fontId);
        }

        public static Texture2D ItemIcon(Item item)
        {
            if (!_IsPrepared)
                _PrepareHelper();

            if (!_IconCache.ContainsKey(iconKey(item)))
            {
                int itemID = (item == null) ? 0 : item.DisplayItemID;
                Texture2D texture = (item == null) ? null : Data.Art.GetStaticTexture(itemID);

                float iScaleUp = 1f, iDestSize = 39f;
                if (texture != null)
                {
                    if (texture.Width > texture.Height)
                        iScaleUp = iDestSize / texture.Height;
                    else
                        iScaleUp = iDestSize / texture.Width;
                }

                _graphicsDevice.SetRenderTarget(0, _RenderTarget);
                _graphicsDevice.Clear(Color.TransparentBlack);

                using (SpriteBatch sprite = new SpriteBatch(_graphicsDevice))
                {
                    sprite.Begin(SpriteBlendMode.AlphaBlend);

                    if (itemID == 0)
                    {
                        sprite.Draw(_TextureEmpty, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);
                    }
                    else
                    {
                        sprite.Draw(_TextureBG, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);

                        if (texture != null)
                        {
                            Rectangle iSrcRect = new Rectangle(
                                    -(int)((texture.Width * iScaleUp) - iDestSize) / 2,
                                    -(int)(((texture.Height) * iScaleUp) - iDestSize) / 2,
                                    (int)(texture.Width * iScaleUp),
                                    (int)(texture.Height * iScaleUp));
                            sprite.Draw(texture,
                                new Rectangle(1, 1, 37, 37),
                                iSrcRect,
                                Color.White);
                        }

                        sprite.Draw(_TextureBorder, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);
                    }
                    sprite.End();
                }

                _graphicsDevice.SetRenderTarget(0, null);
                Texture2D iTexture = new Texture2D(_graphicsDevice, 64, 64);
                int[] iData = new int[64 * 64];
                _RenderTarget.GetTexture().GetData<int>(iData);
                iTexture.SetData<int>(iData);
                _IconCache[iconKey(item)] = iTexture;
            }
            return _IconCache[iconKey(item)];
        }

        private static int iconKey(Item item)
        { 
            if (item == null)
                return 0;
            else
            {
                return item.DisplayItemID + (item.Hue >> 16);
            }
        }

        public static Texture2D TextureFromStream(Stream s)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(s);
            uint[] pixels = new uint[bmp.Width * bmp.Height];
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    Random r = new Random();
                    int a = r.Next(255);
                    System.Drawing.Color c = bmp.GetPixel(x, y);
                    pixels[(y * bmp.Width) + x] = (uint)c.ToArgb();
                }
            }

            Texture2D t = new Texture2D(_graphicsDevice, bmp.Width, bmp.Height, 1, TextureUsage.None, SurfaceFormat.Color);
            t.SetData<uint>(pixels);
            return t;
        }
    }
}
