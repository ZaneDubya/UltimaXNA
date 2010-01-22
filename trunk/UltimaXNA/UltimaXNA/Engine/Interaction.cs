using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Data;
using UltimaXNA.Client;
using UltimaXNA.Entities;
using UltimaXNA.Extensions;
using UltimaXNA.UI;
using UltimaXNA.Input;
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.TileEngine;
using UltimaXNA.UILegacy;

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
                if (_legacyUI.MouseOverControl is UILegacy.Gumplings.ItemGumpling)
                {
                    containerSerial = ((UILegacy.Gumplings.ItemGumpling)_legacyUI.MouseOverControl).ContainerSerial;
                }
                else if (_legacyUI.MouseOverControl is UILegacy.Gumplings.GumpPicContainer)
                {
                    containerSerial = ((UILegacy.Gumplings.GumpPicContainer)_legacyUI.MouseOverControl).ContainerSerial;
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
                Client.UltimaClient.Send(new DropItemPacket(item.Serial, (short)x, (short)y, 0, 0, containerSerial));
                _legacyUI.Cursor.ClearHolding();
                return;
            }
            else
            {
                Client.UltimaClient.Send(new DropItemPacket(item.Serial, (short)X, (short)Y, (byte)Z, 0, unchecked(Serial.World)));
                _legacyUI.Cursor.ClearHolding();
            }
        }
    }
}
