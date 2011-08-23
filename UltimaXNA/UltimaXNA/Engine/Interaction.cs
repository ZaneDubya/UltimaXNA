/***************************************************************************
 *   Interaction.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Client;
using UltimaXNA.Client.Packets.Client;
using UltimaXNA.Entities;
using UltimaXNA.Extensions;
using UltimaXNA.Input;
using UltimaXNA.TileEngine;
using UltimaXNA.UILegacy;

namespace UltimaXNA
{
    class Interaction
    {
        static IInputState _input;
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

        public static void SendClientVersion()
        {
            UltimaClient.Send(new ClientVersionPacket("6.0.1.10"));
        }

        public static void SendClientScreenSize()
        {
            UltimaClient.Send(new ReportClientScreenSizePacket(800, 600));
        }

        public static void SendClientLocalization()
        {
            UltimaClient.Send(new ReportClientLocalizationPacket("ENU"));
        }

        public static void GetMySkills()
        {
            UltimaClient.Send(new GetPlayerStatusPacket(0x05, EntitiesCollection.MySerial));
        }

        public static void GetMyBasicStatus()
        {
            UltimaClient.Send(new GetPlayerStatusPacket(0x04, EntitiesCollection.MySerial));
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

        public static void UseSkill(int index)
        {
            UltimaClient.Send(new RequestSkillUsePacket(index));
        }
    }
}
