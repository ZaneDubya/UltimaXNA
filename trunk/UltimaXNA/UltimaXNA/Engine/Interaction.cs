using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Data;
using UltimaXNA.Client;
using UltimaXNA.Entities;
using UltimaXNA.Extensions;
using UltimaXNA.Input;
using UltimaXNA.Client.Packets.Client;
using UltimaXNA.TileEngine;
using UltimaXNA.UILegacy;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA
{
    class Interaction
    {
        static IInputState _input;
        // static IInputService _input;
        static IIsometricRenderer _world;
        static IUIManager _legacyUI;

        public static void Initialize(Game game)
        {
            _input = game.Services.GetService<IInputState>();
            _world = game.Services.GetService<IIsometricRenderer>();
            _legacyUI = game.Services.GetService<IUIManager>();
        }

        public static void SendChat(string text)
        {
            UltimaClient.Send(new AsciiSpeechPacket(AsciiSpeechPacketTypes.Normal, 0, 0, "ENU", text));
        }

        public static void SingleClick(Entity item)
        {
            UltimaClient.Send(new SingleClickPacket(item.Serial));
        }

        public static void DoubleClick(Entity item)
        {
            UltimaClient.Send(new DoubleClickPacket(item.Serial));
        } 

        public static void PickUpItem(Item item, int x, int y)
        {
            if (item.PickUp())
            {
                UltimaClient.Send(new PickupItemPacket(item.Serial, item.Amount));
                _legacyUI.Cursor.PickUpItem(item, x, y);
            }
        }

        public static void DropItemToWorld(Item item, int X, int Y, int Z)
        {
            if (!_legacyUI.IsMouseOverUI)
            {
                Serial serial;
                if (_world.MouseOverObject is MapObjectItem && ((Item)_world.MouseOverObject.OwnerEntity).ItemData.Container)
                {
                    serial = _world.MouseOverObject.OwnerEntity.Serial;
                    X = Y = 0xFFFF;
                    Z = 0;
                }
                else
                    serial = Serial.World;
                UltimaClient.Send(new DropItemPacket(item.Serial, (ushort)X, (ushort)Y, (byte)Z, 0, serial));
                _legacyUI.Cursor.ClearHolding();
            }
        }

        public static void DropItemToContainer(Item item, Container container)
        {
            // get random coords and drop the item there.
            Rectangle bounds = Data.ContainerData.GetData(container.ItemID).Bounds;
            int x = Utility.RandomValue(bounds.Left, bounds.Right);
            int y = Utility.RandomValue(bounds.Top, bounds.Bottom);
            DropItemToContainer(item, container, x, y);
        }

        public static void DropItemToContainer(Item item, Container container, int x, int y)
        {
            Rectangle containerBounds = Data.ContainerData.GetData(container.ItemID).Bounds;
            Texture2D itemTexture = Data.Art.GetStaticTexture(item.DisplayItemID);
            if (x < containerBounds.Left) x = containerBounds.Left;
            if (x > containerBounds.Right - itemTexture.Width) x = containerBounds.Right - itemTexture.Width;
            if (y < containerBounds.Top) y = containerBounds.Top;
            if (y > containerBounds.Bottom - itemTexture.Height) y = containerBounds.Bottom - itemTexture.Height;
            UltimaClient.Send(new DropItemPacket(item.Serial, (ushort)x, (ushort)y, 0, 0, container.Serial));
            _legacyUI.Cursor.ClearHolding();
        }

        public static void WearItem(Item item)
        {
            UltimaClient.Send(new DropToLayerPacket(item.Serial, 0x00, EntitiesCollection.MySerial));
            _legacyUI.Cursor.ClearHolding();
        }
    }
}
