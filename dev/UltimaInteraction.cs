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
using System.Collections.Generic;
using UltimaXNA.Entity;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld;

namespace UltimaXNA
{
    class UltimaInteraction
    {
        public static void SendChat(string text)
        {
            UltimaClient.UltimaSend(new AsciiSpeechPacket(AsciiSpeechPacketTypes.Normal, 0, 0, "ENU", text));
        }

        public static void SingleClick(BaseEntity item)
        {
            UltimaClient.UltimaSend(new SingleClickPacket(item.Serial));
        }

        public static void DoubleClick(BaseEntity item)
        {
            UltimaClient.UltimaSend(new DoubleClickPacket(item.Serial));
        } 

        public static void PickUpItem(Item item, int x, int y)
        {
            if (item.PickUp())
            {
                UltimaClient.UltimaSend(new PickupItemPacket(item.Serial, item.Amount));
                UltimaEngine.UltimaUI.Cursor.PickUpItem(item, x, y);
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
                UltimaClient.UltimaSend(new DropItemPacket(item.Serial, (ushort)X, (ushort)Y, (byte)Z, 0, serial));
                UltimaEngine.UltimaUI.Cursor.ClearHolding();
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
            Texture2D itemTexture = UltimaData.ArtData.GetStaticTexture(item.DisplayItemID);
            if (x < containerBounds.Left) x = containerBounds.Left;
            if (x > containerBounds.Right - itemTexture.Width) x = containerBounds.Right - itemTexture.Width;
            if (y < containerBounds.Top) y = containerBounds.Top;
            if (y > containerBounds.Bottom - itemTexture.Height) y = containerBounds.Bottom - itemTexture.Height;
            UltimaClient.UltimaSend(new DropItemPacket(item.Serial, (ushort)x, (ushort)y, 0, 0, container.Serial));
            UltimaEngine.UltimaUI.Cursor.ClearHolding();
        }

        public static void WearItem(Item item)
        {
            UltimaClient.UltimaSend(new DropToLayerPacket(item.Serial, 0x00, EntityManager.MySerial));
            UltimaEngine.UltimaUI.Cursor.ClearHolding();
        }

        public static void UseSkill(int index)
        {
            UltimaClient.UltimaSend(new RequestSkillUsePacket(index));
        }

        public static Gump OpenContainerGump(BaseEntity entity)
        {
            Gump gump;

            if ((gump = (Gump)UltimaEngine.UserInterface.GetControl(entity.Serial)) != null)
            {
                gump.Dispose();
            }

            gump = new UltimaGUI.Gumps.ContainerGump(entity, ((Container)entity).ItemID);
            UltimaEngine.UserInterface.AddControl(gump, 64, 64);
            return gump;
        }

        public static void DisconnectToLoginScreen()
        {
            if (UltimaClient.Status != UltimaClientStatus.Unconnected)
                UltimaClient.UltimaDisconnect();
            UltimaVars.EngineVars.InWorld = false;
            UltimaEngine.ActiveModel = new UltimaXNA.UltimaLogin.LoginModel();
        }


        static List<QueuedMessage> m_ChatQueue = new List<QueuedMessage>();

        public static void ChatMessage(string text)
        {
            ChatMessage(text, 0, 0);
        }
        public static void ChatMessage(string text, int hue, int font)
        {
            m_ChatQueue.Add(new QueuedMessage(text, hue, font));

            Gump g = UltimaEngine.UserInterface.GetControl<UltimaGUI.Gumps.ChatWindow>(0);
            if (g != null)
            {
                foreach (QueuedMessage msg in m_ChatQueue)
                {
                    ((UltimaGUI.Gumps.ChatWindow)g).AddLine(msg.Text);
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
