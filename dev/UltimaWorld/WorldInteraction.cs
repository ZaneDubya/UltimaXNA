/***************************************************************************
 *   WorldInteraction.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.WorldGumps;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld;
#endregion

namespace UltimaXNA.UltimaWorld
{
    /// <summary>
    /// Static class that hosts methods for interacting with the world.
    /// </summary>
    public static class WorldInteraction
    {
        public static MsgBox MsgBox(string msg, MsgBoxTypes type)
        {
            // pop up an error message, modal.
            MsgBox msgbox = new MsgBox(msg, type);
            Engine.UserInterface.AddControl(msgbox, 0, 0);
            return msgbox;
        }

        private static UltimaEngine Engine;
        public static void Initialize(UltimaEngine engine)
        {
            Engine = engine;
        }

        public static void SendChat(string text) // used by chatwindow.
        {
            Engine.Client.Send(new AsciiSpeechPacket(AsciiSpeechPacketTypes.Normal, 0, 0, "ENU", text));
        }

        public static void SingleClick(AEntity item) // used by worldinput and itemgumpling.
        {
            Engine.Client.Send(new SingleClickPacket(item.Serial));
        }

        public static void DoubleClick(AEntity item) // used by itemgumpling, paperdollinteractable, topmenu, worldinput.
        {
            Engine.Client.Send(new DoubleClickPacket(item.Serial));
        } 

        public static void ToggleWarMode() // used by paperdollgump.
        {
            Engine.Client.Send(new RequestWarModePacket(!((Mobile)EntityManager.GetPlayerObject()).Flags.IsWarMode));
        }

        public static void UseSkill(int index) // used by WorldInteraction
        {
            Engine.Client.Send(new RequestSkillUsePacket(index));
        }

        public static Gump OpenContainerGump(AEntity entity) // used by ultimaclient.
        {
            Gump gump;

            if ((gump = (Gump)Engine.UserInterface.GetControl(entity.Serial)) != null)
            {
                gump.Dispose();
            }

            gump = new ContainerGump(entity, ((Container)entity).ItemID);
            Engine.UserInterface.AddControl(gump, 64, 64);
            return gump;
        }

        public static Gump OpenCorpseGump(AEntity entity) // used by UltimaClient
        {
            Gump gump;

            if ((gump = (Gump)Engine.UserInterface.GetControl(entity.Serial)) != null)
            {
                gump.Dispose();
            }

            gump = new ContainerGump(entity, 0x2006);
            Engine.UserInterface.AddControl(gump, 96, 96);
            return gump;
        }

        public static void DisconnectToLoginScreen() // used by paperdoll gump
        {
            if (Engine.Client.Status != UltimaClientStatus.Unconnected)
                Engine.Client.Disconnect();
            UltimaVars.EngineVars.InWorld = false;
            Engine.ActiveModel = new UltimaXNA.UltimaLogin.LoginModel();
        }

        public static void GumpMenuSelect(int id, int gumpId, int buttonId, int[] switchIds, Tuple<short, string>[] textEntries) // used by gump
        {
            Engine.Client.Send(new GumpMenuSelectPacket(id, gumpId, buttonId, switchIds, textEntries));
        }


        static List<QueuedMessage> m_ChatQueue = new List<QueuedMessage>();

        public static void ChatMessage(string text) // used by gump
        {
            ChatMessage(text, 0, 0);
        }
        public static void ChatMessage(string text, int hue, int font)
        {
            m_ChatQueue.Add(new QueuedMessage(text, hue, font));

            ChatWindow chat = Engine.UserInterface.GetControl<ChatWindow>(0);
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

        public static void CreateLabel(MessageType msgType, Serial serial, string text, int hue, int font)
        {
            Overhead overhead;

            if (serial.IsValid)
            {
                overhead = EntityManager.AddOverhead(msgType, serial, "<outline>" + text, font, hue);
                // Labels that are longer than the current name should be set as the name
                if (serial.IsMobile)
                {
                    Mobile m = EntityManager.GetObject<Mobile>(serial, false);
                    if (m == null)
                    {
                        // received a label for a mobile that does not exist!
                    }
                    else
                    {
                        if (m.Name == null || m.Name.Length < text.Length)
                            m.Name = text;
                    }
                }
            }
            else
            {
                WorldInteraction.ChatMessage("[LABEL] " + text, font, hue);
            }
        }

        public static void SendLastTargetPacket(Serial last_target) // used by engine vars, which is used by worldinput and ultimaclient.
        {
            Engine.Client.Send(new GetPlayerStatusPacket(0x04, last_target));
        }

        // ======================================================================
        // Cursor handling routines.
        // ======================================================================

        internal static Action<Item, int, int> OnPickupItem;
        internal static Action OnClearHolding;

        internal static void PickupItem(Item item, Point offset)
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
