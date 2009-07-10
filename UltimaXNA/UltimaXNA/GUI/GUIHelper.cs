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
        private static Client.IUltimaClient _GameClientService = null;
        private static GameObjects.IGameObjects _GameObjectsService = null;
        private static GUI.EngineGUI _GUI = null;

        private static bool _IsPrepared = false;
        private static Texture2D _TextureBorder = null;
        private static Texture2D _TextureBG = null;
        private static Texture2D _TextureEmpty = null;
        private static RenderTarget2D _RenderTarget;
        private static Texture2D[] _IconCache;
        private static GraphicsDevice _GraphicsDevice = null;

        public static string TooltipMsg = string.Empty;
        public static int TooltipX = 0;
        public static int TooltipY = 0;

        private static List<string> _QueuedChatText = new List<string>();

        public static GameObjects.BaseObject MouseHoldingItem;
        private static GameObjects.GameObject _ToolTipItem;

        public static void Network_SendChat(string nChatText)
        {
            if (nChatText != string.Empty)
                _GameClientService.Send(new UnicodeSpeechPacket(0, 0, "ENU", nChatText));
        }

        public static void Chat_AddLine(string nChatText)
        {
            // add text to the chat window.
            try
            {
                ((Window_Chat)_GUI.Window("ChatFrame")).AddText(nChatText);
            }
            catch
            {
                // The chat frame isn't loaded yet. Save this text for later.
                _QueuedChatText.Add(nChatText);
            }
        }

        public static void Update()
        {
            if (_QueuedChatText.Count > 0)
            {
                Window_Chat w = _GUI.Window("ChatFrame") as Window_Chat;
                if (w != null)
                {
                    foreach (string s in _QueuedChatText)
                    {
                        w.AddText(s);
                    }
                    _QueuedChatText.Clear();
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
                if (iHeldObject.Wearer.Serial == _GameObjectsService.MySerial)
                {
                    // we are wearing this item. Go ahead and drop it into the requested slot.
                    _GameClientService.Send(new PickupItemPacket(iHeldObject.Serial, (short)iHeldObject.Item_StackCount));
                    _GameClientService.Send(new DropItemPacket(iHeldObject.Serial, (short)nDestSlot, (short)0x7FFF,
                        0, 0, iDestContainer.Serial));
                }
                else
                {
                    // this object is being worn by someone else. We can't move it.
                }
            }
            else
            {
                if (iHeldObject.Item_ContainedWithinSerial == iDestContainer.Serial)
                {
                    iDestContainer.ContainerObject.Event_MoveItemToSlot(iHeldObject, nDestSlot);
                }
                else
                {
                    if (iDestContainer.ContainerObject.GetContents(nDestSlot) == null)
                    {
                        _GameClientService.Send(new PickupItemPacket(iHeldObject.Serial, (short)iHeldObject.Item_StackCount));
                        _GameClientService.Send(new DropItemPacket(iHeldObject.Serial, (short)nDestSlot, (short)0x7FFF,
                            0, 0, iDestContainer.Serial));
                    }
                    else
                    {
                        _GameClientService.Send(new PickupItemPacket(iHeldObject.Serial, (short)iHeldObject.Item_StackCount));
                        _GameClientService.Send(new DropItemPacket(iHeldObject.Serial, (short)0, (short)0,
                            0, 0, iDestContainer.Serial));
                    }
                }
            }
            GUI.GUIHelper.MouseHoldingItem = null;
        }

        public static void DropItemOntoGround(int x, int y, int z)
        {
            GameObjects.GameObject iHeldObject = (GameObjects.GameObject)MouseHoldingItem;
            _GameClientService.Send(new PickupItemPacket(iHeldObject.Serial, (short)iHeldObject.Item_StackCount));
            _GameClientService.Send(new DropItemPacket(iHeldObject.Serial, (short)x, (short)y, (byte)z, 0, 0));
            GUI.GUIHelper.MouseHoldingItem = null;
        }

        public static void WearItem(GameObjects.BaseObject nDestMobile, int nDestSlot)
        {
            GameObjects.GameObject iHeldObject = (GameObjects.GameObject)MouseHoldingItem;
            GameObjects.Unit iDestMobile = (GameObjects.Unit)nDestMobile;

            if (iHeldObject.Item_ContainedWithinSerial != 0)
            {
                GameObjects.GameObject iSourceContainer = (GameObjects.GameObject)_GameObjectsService.GetObject(iHeldObject.Item_ContainedWithinSerial, UltimaXNA.GameObjects.ObjectType.GameObject);
                iHeldObject.Item_ContainedWithinSerial = 0;
                iSourceContainer.ContainerObject.RemoveItem(iHeldObject.Serial);
            }
            _GameClientService.Send(new PickupItemPacket(iHeldObject.Serial, (short)0));
            _GameClientService.Send(new DropToLayerPacket(iHeldObject.Serial, (byte)nDestSlot, iDestMobile.Serial));

            GUI.GUIHelper.MouseHoldingItem = null;
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
            _GameClientService.Send(new BuyItemsPacket(nVendorSerial, iBuyItem));
            _GameClientService.Send(new RequestContextMenuPacket(nVendorSerial));
        }

        public static GameObjects.GameObject ToolTipItem
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
                    Data.ItemData iData = _ToolTipItem.ItemData;
                    string iItemName = string.Empty;
                    if (_ToolTipItem.Item_StackCount > 1)
                        iItemName += _ToolTipItem.Item_StackCount.ToString() + @" ";
                    if (iData.Name.Contains(@"%"))
                    {
                        iItemName += _ToolTipItem.Item_StackCount.ToString() + @" ";
                        int iMultiplePosition = iData.Name.IndexOf(@"%");
                        string iBaseString = iData.Name.Substring(0, iMultiplePosition);
                        string iMultipleString = iData.Name.Substring(iMultiplePosition + 1, iData.Name.IndexOf(@"%", iMultiplePosition + 1) - iMultiplePosition - 1);
                        iItemName = _ToolTipItem.Item_StackCount.ToString() + @" " + iBaseString;
                        if (_ToolTipItem.Item_StackCount > 1)
                            iItemName += iMultipleString;
                    }
                    else
                    {
                        iItemName += iData.Name;
                    }
                    TooltipMsg = iItemName + Environment.NewLine +
                        "Serial:" + SerialHex(_ToolTipItem.Serial);
                    if (_ToolTipItem.PropertyList.HasProperties)
                    {
                        TooltipMsg += Environment.NewLine +
                            _ToolTipItem.PropertyList.Properties;
                    }
                }
            }
            get
            {
                return _ToolTipItem;
            }
        }

        public static string SerialHex(Serial serial)
        {
            return MiscUtil.HexEncoding.ToString(
                MiscUtil.Conversion.EndianBitConverter.Big.GetBytes(serial)
            );
        }

        public static void SetObjects(
            GraphicsDevice nDevice, EngineGUI nGUI, Client.IUltimaClient nGameClientService, GameObjects.IGameObjects nGameObjectsService)
        {
            _GraphicsDevice = nDevice;
            _GUI = nGUI;
            _GameClientService = nGameClientService;
            _GameObjectsService = nGameObjectsService;
        }

        private static void _PrepareHelper()
        {
            _TextureBorder = Texture2D.FromFile(_GraphicsDevice,
               FormCollection.ContentManager.RootDirectory + @"GUI\COMMON\UI-Slot-Border.png");
            _TextureBG = Texture2D.FromFile(_GraphicsDevice,
                FormCollection.ContentManager.RootDirectory + @"GUI\COMMON\UI-Slot-BG.png");
            _TextureEmpty = Texture2D.FromFile(_GraphicsDevice,
                FormCollection.ContentManager.RootDirectory + @"GUI\COMMON\UI-Slot-Empty.png");
            _RenderTarget = new RenderTarget2D(_GraphicsDevice, 64, 64, 0, SurfaceFormat.Color, RenderTargetUsage.PreserveContents);
            _IconCache = new Texture2D[0x4000];
            _IsPrepared = true;
        }

        public static Texture2D ItemIcon(int nItemID)
        {
            if (!_IsPrepared)
                _PrepareHelper();

            if (_IconCache[nItemID] == null)
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

                _GraphicsDevice.SetRenderTarget(0, _RenderTarget);
                _GraphicsDevice.Clear(Color.TransparentBlack);

                using (SpriteBatch sprite = new SpriteBatch(_GraphicsDevice))
                {
                    sprite.Begin(SpriteBlendMode.AlphaBlend);

                    if (nItemID == 0)
                    {
                        sprite.Draw(_TextureEmpty, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);
                    }
                    else
                    {
                        sprite.Draw(_TextureBG, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);

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

                        sprite.Draw(_TextureBorder, new Rectangle(0, 0, 39, 39), new Rectangle(12, 12, 39, 39), Color.White);
                    }
                    sprite.End();
                }

                _GraphicsDevice.SetRenderTarget(0, null);
                Texture2D iTexture = new Texture2D(_GraphicsDevice, 64, 64);
                int[] iData = new int[64 * 64];
                _RenderTarget.GetTexture().GetData<int>(iData);
                iTexture.SetData<int>(iData);
                _IconCache[nItemID] = iTexture;
            }
            return _IconCache[nItemID];
        }
    }
}
