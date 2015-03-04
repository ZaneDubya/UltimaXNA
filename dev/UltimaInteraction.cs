using InterXLib.Input.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using UltimaXNA.Entity;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.Controls;
using UltimaXNA.UltimaGUI.Gumps;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld;

namespace UltimaXNA
{
    public static class UltimaInteraction
    {
        private static UltimaCursor m_Cusor = null;
        public static UltimaCursor Cursor { get { return m_Cusor; } }

        static UltimaInteraction()
        {
            m_Cusor = new UltimaCursor();
        }

        public static void Draw()
        {
            // Draw the cursor
            m_Cusor.Draw(UltimaEngine.Input.MousePosition);
        }

        public static void Update()
        {
            if (UltimaEngine.UserInterface.MouseOverControl != null && Cursor.IsHolding)
            {
                Control target = UltimaEngine.UserInterface.MouseOverControl;
                List<InputEventMouse> events = UltimaEngine.Input.GetMouseEvents();
                foreach (InputEventMouse e in events)
                {
                    if (e.EventType == MouseEvent.Up && e.Button == MouseButton.Left)
                    {
                        // dropped an item we were holding.
                        if (target is ItemGumpling && ((ItemGumpling)target).Item.ItemData.Container)
                            DropItemToContainer(Cursor.HoldingItem, (Entity.Container)((ItemGumpling)target).Item);
                        else if (target is GumpPicContainer)
                        {
                            int x = (int)UltimaEngine.Input.MousePosition.X - Cursor.HoldingOffset.X - (target.X + target.Owner.X);
                            int y = (int)UltimaEngine.Input.MousePosition.Y - Cursor.HoldingOffset.Y - (target.Y + target.Owner.Y);
                            DropItemToContainer(Cursor.HoldingItem, (Entity.Container)((GumpPicContainer)target).Item, x, y);
                        }
                    }
                }
            }
        }

        public static MsgBox MsgBox(string msg, MsgBoxTypes type)
        {
            // pop up an error message, modal.
            MsgBox msgbox = new MsgBox(msg, type);
            UltimaEngine.UserInterface.AddControl(msgbox, 0, 0);
            return msgbox;
        }

        private static UltimaClient s_Client;
        public static void Initialize(UltimaClient client)
        {
            s_Client = client;
        }

        public static void SendChat(string text) // used by chatwindow.
        {
            s_Client.Send(new AsciiSpeechPacket(AsciiSpeechPacketTypes.Normal, 0, 0, "ENU", text));
        }

        public static void SingleClick(BaseEntity item) // used by worldinput and itemgumpling.
        {
            s_Client.Send(new SingleClickPacket(item.Serial));
        }

        public static void DoubleClick(BaseEntity item) // used by itemgumpling, paperdollinteractable, topmenu, worldinput.
        {
            s_Client.Send(new DoubleClickPacket(item.Serial));
        } 

        public static void PickUpItem(Item item, int x, int y) // used by itemgumpling, worldinput
        {
            if (item.PickUp())
            {
                s_Client.Send(new PickupItemPacket(item.Serial, item.Amount));
                Cursor.PickUpItem(item, x, y);
            }
        }

        public static void ToggleWarMode() // used by paperdollgump.
        {
            s_Client.Send(new RequestWarModePacket(!((Mobile)EntityManager.GetPlayerObject()).IsWarMode));
        }

        public static void DropItemToWorld(Item item, int X, int Y, int Z) // used by worldinput.
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
                s_Client.Send(new DropItemPacket(item.Serial, (ushort)X, (ushort)Y, (byte)Z, 0, serial));
                Cursor.ClearHolding();
            }
        }

        public static void DropItemToContainer(Item item, Container container) // used by paperdollinteractable and ultimaguistate.
        {
            // get random coords and drop the item there.
            Rectangle bounds = UltimaData.ContainerData.GetData(container.ItemID).Bounds;
            int x = Utility.RandomValue(bounds.Left, bounds.Right);
            int y = Utility.RandomValue(bounds.Top, bounds.Bottom);
            DropItemToContainer(item, container, x, y);
        }

        public static void DropItemToContainer(Item item, Container container, int x, int y) // used by ultimaguistate.
        {
            Rectangle containerBounds = UltimaData.ContainerData.GetData(container.ItemID).Bounds;
            Texture2D itemTexture = UltimaData.ArtData.GetStaticTexture(item.DisplayItemID);
            if (x < containerBounds.Left) x = containerBounds.Left;
            if (x > containerBounds.Right - itemTexture.Width) x = containerBounds.Right - itemTexture.Width;
            if (y < containerBounds.Top) y = containerBounds.Top;
            if (y > containerBounds.Bottom - itemTexture.Height) y = containerBounds.Bottom - itemTexture.Height;
            s_Client.Send(new DropItemPacket(item.Serial, (ushort)x, (ushort)y, 0, 0, container.Serial));
            Cursor.ClearHolding();
        }

        public static void WearItem(Item item) // used by paperdollinteractable.
        {
            s_Client.Send(new DropToLayerPacket(item.Serial, 0x00, EntityManager.MySerial));
            Cursor.ClearHolding();
        }

        public static void UseSkill(int index) // used by ultimainteraction
        {
            s_Client.Send(new RequestSkillUsePacket(index));
        }

        public static Gump OpenContainerGump(BaseEntity entity) // used by ultimaclient.
        {
            Gump gump;

            if ((gump = (Gump)UltimaEngine.UserInterface.GetControl(entity.Serial)) != null)
            {
                gump.Dispose();
            }

            gump = new ContainerGump(entity, ((Container)entity).ItemID);
            UltimaEngine.UserInterface.AddControl(gump, 64, 64);
            return gump;
        }

        public static void DisconnectToLoginScreen() // used by paperdoll gump
        {
            if (s_Client.Status != UltimaClientStatus.Unconnected)
                s_Client.Disconnect();
            UltimaVars.EngineVars.InWorld = false;
            UltimaEngine.ActiveModel = new UltimaXNA.UltimaLogin.LoginModel();
        }

        public static void GumpMenuSelect(int id, int gumpId, int buttonId, int[] switchIds, Tuple<short, string>[] textEntries) // used by gump
        {
            s_Client.Send(new GumpMenuSelectPacket(id, gumpId, buttonId, switchIds, textEntries));
        }


        static List<QueuedMessage> m_ChatQueue = new List<QueuedMessage>();

        public static void ChatMessage(string text) // used by gump
        {
            ChatMessage(text, 0, 0);
        }
        public static void ChatMessage(string text, int hue, int font)
        {
            m_ChatQueue.Add(new QueuedMessage(text, hue, font));

            Gump g = UltimaEngine.UserInterface.GetControl<ChatWindow>(0);
            if (g != null)
            {
                foreach (QueuedMessage msg in m_ChatQueue)
                {
                    ((ChatWindow)g).AddLine(msg.Text);
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

        public static void SendLastTargetPacket(Serial last_target) // used by engine vars, which is used by worldinput and ultimaclient.
        {
            s_Client.Send(new GetPlayerStatusPacket(0x04, last_target));
        }
    }
}
