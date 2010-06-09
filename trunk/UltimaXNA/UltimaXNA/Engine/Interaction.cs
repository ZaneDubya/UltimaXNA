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
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.TileEngine;
using UltimaXNA.UILegacy;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA
{
    class Interaction
    {
        static IInputService _input;
        static IWorld _world;
        static IUIManager _legacyUI;

        public static void Initialize(Game game)
        {
            _input = game.Services.GetService<IInputService>();
            _world = game.Services.GetService<IWorld>();
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

        public static void DropItem(Item item, int X, int Y, int Z)
        {
            // drop into container?
            if (_legacyUI.IsMouseOverUI)
            {
                Serial containerSerial;
                int x, y;
                if (_legacyUI.MouseOverControl is ItemGumpling)
                {
                    // If we drop onto a container object, drop *inside* the container object.
                    ItemGumpling g = (ItemGumpling)_legacyUI.MouseOverControl;
                    if (g.Item.ItemData.Container)
                    {
                        containerSerial = g.Item.Serial;
                        X = Y = 0xFFFF;
                        Z = 0;
                    }
                    else
                        containerSerial = g.ContainerSerial;
                }
                else if (_legacyUI.MouseOverControl is GumpPicContainer)
                {
                    containerSerial = ((GumpPicContainer)_legacyUI.MouseOverControl).ContainerSerial;
                }
                else
                {
                    return;
                }
                Container container = EntitiesCollection.GetObject<Container>(containerSerial, false);
                Rectangle containerBounds = Data.ContainerData.GetData(container.ItemID).Bounds;
                Texture2D itemTexture = Data.Art.GetStaticTexture(item.DisplayItemID);
                x = (int)_input.CurrentMousePosition.X - (_legacyUI.MouseOverControl.X + _legacyUI.MouseOverControl.Owner.X) - X;
                y = (int)_input.CurrentMousePosition.Y - (_legacyUI.MouseOverControl.Y + _legacyUI.MouseOverControl.Owner.Y) - Y;
                if (x < containerBounds.Left) x = containerBounds.Left;
                if (x > containerBounds.Right - itemTexture.Width) x = containerBounds.Right - itemTexture.Width;
                if (y < containerBounds.Top) y = containerBounds.Top;
                if (y > containerBounds.Bottom - itemTexture.Height) y = containerBounds.Bottom - itemTexture.Height;
                Client.UltimaClient.Send(new DropItemPacket(item.Serial, (ushort)x, (ushort)y, 0, 0, containerSerial));
                _legacyUI.Cursor.ClearHolding();
                return;
            }
            else
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
                Client.UltimaClient.Send(new DropItemPacket(item.Serial, (ushort)X, (ushort)Y, (byte)Z, 0, serial));
                _legacyUI.Cursor.ClearHolding();
            }
        }
    }
}
