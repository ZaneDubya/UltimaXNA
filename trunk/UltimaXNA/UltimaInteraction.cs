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
using UltimaXNA.Network;
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.Entity;
using UltimaXNA.Interface.Input;
using UltimaXNA.TileEngine;
using UltimaXNA.UltimaGUI;
using UltimaXNA.Scene;
using System.Collections.Generic;

namespace UltimaXNA
{
    class UltimaInteraction
    {
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
            UltimaClient.Send(new GetPlayerStatusPacket(0x05, Entities.MySerial));
        }

        public static void GetMyBasicStatus()
        {
            UltimaClient.Send(new GetPlayerStatusPacket(0x04, Entities.MySerial));
        }

        public static void SingleClick(BaseEntity item)
        {
            UltimaClient.Send(new SingleClickPacket(item.Serial));
        }

        public static void DoubleClick(BaseEntity item)
        {
            UltimaClient.Send(new DoubleClickPacket(item.Serial));
        } 

        public static void PickUpItem(Item item, int x, int y)
        {
            if (item.PickUp())
            {
                UltimaClient.Send(new PickupItemPacket(item.Serial, item.Amount));
                UltimaEngine.UserInterface.Cursor.PickUpItem(item, x, y);
            }
        }

        public static void DropItemToWorld(Item item, int X, int Y, int Z)
        {
            if (!UltimaEngine.UserInterface.IsMouseOverUI)
            {
                Serial serial;
                if (IsometricRenderer.MouseOverObject is MapObjectItem && ((Item)IsometricRenderer.MouseOverObject.OwnerEntity).ItemData.Container)
                {
                    serial = IsometricRenderer.MouseOverObject.OwnerEntity.Serial;
                    X = Y = 0xFFFF;
                    Z = 0;
                }
                else
                    serial = Serial.World;
                UltimaClient.Send(new DropItemPacket(item.Serial, (ushort)X, (ushort)Y, (byte)Z, 0, serial));
                UltimaEngine.UserInterface.Cursor.ClearHolding();
            }
        }

        public static void DropItemToContainer(Item item, Container container)
        {
            // get random coords and drop the item there.
            Rectangle bounds = UltimaData.ContainerData.GetData(container.ItemID).Bounds;
            int x = Utility.RandomValue(bounds.Left, bounds.Right);
            int y = Utility.RandomValue(bounds.Top, bounds.Bottom);
            DropItemToContainer(item, container, x, y);
        }

        public static void DropItemToContainer(Item item, Container container, int x, int y)
        {
            Rectangle containerBounds = UltimaData.ContainerData.GetData(container.ItemID).Bounds;
            Texture2D itemTexture = UltimaData.Art.GetStaticTexture(item.DisplayItemID);
            if (x < containerBounds.Left) x = containerBounds.Left;
            if (x > containerBounds.Right - itemTexture.Width) x = containerBounds.Right - itemTexture.Width;
            if (y < containerBounds.Top) y = containerBounds.Top;
            if (y > containerBounds.Bottom - itemTexture.Height) y = containerBounds.Bottom - itemTexture.Height;
            UltimaClient.Send(new DropItemPacket(item.Serial, (ushort)x, (ushort)y, 0, 0, container.Serial));
            UltimaEngine.UserInterface.Cursor.ClearHolding();
        }

        public static void WearItem(Item item)
        {
            UltimaClient.Send(new DropToLayerPacket(item.Serial, 0x00, Entities.MySerial));
            UltimaEngine.UserInterface.Cursor.ClearHolding();
        }

        public static void UseSkill(int index)
        {
            UltimaClient.Send(new RequestSkillUsePacket(index));
        }

        public static Gump OpenContainerGump(BaseEntity entity)
        {
            Gump gump;
            
            if ((gump = UltimaEngine.UserInterface.GetGump(entity.Serial)) != null)
            {
                gump.Dispose();
            }

            gump = new UltimaGUI.ClientsideGumps.ContainerGump(entity, ((Container)entity).ItemID);
            UltimaEngine.UserInterface.AddGump_Local(gump, 64, 64);
            return gump;
        }

        public static void DisconnectToLoginScreen()
        {
            if (UltimaClient.Status != UltimaClientStatus.Unconnected)
                UltimaClient.Disconnect();
            SceneManager.Reset();
        }


        static List<QueuedMessage> m_ChatQueue = new List<QueuedMessage>();

        public static void ChatMessage(string text)
        {
            ChatMessage(text, 0, 0);
        }
        public static void ChatMessage(string text, int hue, int font)
        {
            m_ChatQueue.Add(new QueuedMessage(text, hue, font));

            Gump g = UltimaEngine.UserInterface.GetGump<UltimaGUI.ClientsideGumps.ChatWindow>(0);
            if (g != null)
            {
                foreach (QueuedMessage msg in m_ChatQueue)
                {
                    ((UltimaGUI.ClientsideGumps.ChatWindow)g).AddLine(msg.Text);
                }
                m_ChatQueue.Clear();
            }
        }

        class QueuedMessage
        {
            public string Text;
            public int Hue;
            public int Font;

            public QueuedMessage(string text, int hue, int font)
            {
                Text = text;
                Hue = hue;
                Font = font;
            }
        }
    }
}
