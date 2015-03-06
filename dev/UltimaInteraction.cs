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
using UltimaXNA.UltimaWorld.View;

namespace UltimaXNA
{
    public static class UltimaInteraction
    {
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

        public static void ToggleWarMode() // used by paperdollgump.
        {
            s_Client.Send(new RequestWarModePacket(!((Mobile)EntityManager.GetPlayerObject()).IsWarMode));
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

            ChatWindow chat = UltimaEngine.UserInterface.GetControl<ChatWindow>(0);
            if (chat != null)
            {
                foreach (QueuedMessage msg in m_ChatQueue)
                {
                    chat.AddLine(msg.Text);
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

        // ======================================================================
        // Cursor handling routines.
        // ======================================================================

        internal static Action<Item, int, int> OnPickupItem;
        internal static Action OnClearHolding;

        internal static void PickupItem(Item item, Point2D offset)
        {
            if (item == null)
                return;
            if (OnPickupItem == null)
                return;

            OnPickupItem(item, offset.X, offset.Y);
        }

        internal static void ClearHolding()
        {
            if (OnClearHolding == null)
                return;

            OnClearHolding();
        }
    }
}
