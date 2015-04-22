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
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.Entities.Mobiles;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.World.Gumps;
using UltimaXNA.Ultima.Entities.Items.Containers;
#endregion

namespace UltimaXNA.Ultima.World.Controllers
{
    /// <summary>
    /// Hosts methods for interacting with the world.
    /// </summary>
    internal class WorldInteraction
    {
        private INetworkClient m_Network;
        private UserInterfaceService m_UserInterface;

        protected WorldModel World
        {
            get;
            private set;
        }

        public WorldInteraction(WorldModel world)
        {
            m_Network = UltimaServices.GetService<INetworkClient>();
            m_UserInterface = UltimaServices.GetService<UserInterfaceService>();

            World = world;
        }

        private Serial m_lastTarget;
        public Serial LastTarget
        {
            get { return m_lastTarget; }
            set
            {
                m_lastTarget = value;
                // send last target packet to server.
                m_Network.Send(new GetPlayerStatusPacket(0x04, m_lastTarget));
            }
        }

        public void SendChat(string text) // used by chatwindow.
        {
            m_Network.Send(new AsciiSpeechPacket(AsciiSpeechPacketTypes.Normal, 0, 0, "ENU", text));
        }

        public void SingleClick(AEntity item) // used by worldinput and itemgumpling.
        {
            m_Network.Send(new SingleClickPacket(item.Serial));
        }

        public void DoubleClick(AEntity item) // used by itemgumpling, paperdollinteractable, topmenu, worldinput.
        {
            m_Network.Send(new DoubleClickPacket(item.Serial));
        } 

        public void ToggleWarMode() // used by paperdollgump.
        {
            m_Network.Send(new RequestWarModePacket(!((Mobile)EntityManager.GetPlayerObject()).Flags.IsWarMode));
        }

        public void UseSkill(int index) // used by WorldInteraction
        {
            m_Network.Send(new RequestSkillUsePacket(index));
        }

        public Gump OpenContainerGump(AEntity entity) // used by ultimaclient.
        {
            Gump gump;

            if ((gump = (Gump)m_UserInterface.GetControl(entity.Serial)) != null)
            {
                gump.Dispose();
            }

            gump = new ContainerGump(entity, ((Container)entity).ItemID);
            m_UserInterface.AddControl(gump, 64, 64);
            return gump;
        }

        public Gump OpenCorpseGump(AEntity entity) // used by UltimaClient
        {
            Gump gump;

            if ((gump = (Gump)m_UserInterface.GetControl(entity.Serial)) != null)
            {
                gump.Dispose();
            }

            gump = new ContainerGump(entity, 0x2006);
            m_UserInterface.AddControl(gump, 96, 96);
            return gump;
        }


        List<QueuedMessage> m_ChatQueue = new List<QueuedMessage>();

        public void ChatMessage(string text) // used by gump
        {
            ChatMessage(text, 0, 0);
        }
        public void ChatMessage(string text, int hue, int font)
        {
            m_ChatQueue.Add(new QueuedMessage(text, hue, font));

            ChatControl chat = UltimaServices.GetService<ChatControl>();
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

        public void CreateLabel(MessageType msgType, Serial serial, string text, int hue, int font)
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
                ChatMessage("[LABEL] " + text, font, hue);
            }
        }

        // ======================================================================
        // Cursor handling routines.
        // ======================================================================

        public Action<Item, int, int> OnPickupItem;
        public Action OnClearHolding;

        internal void PickupItem(Item item, Point offset)
        {
            if (item == null)
                return;
            if (OnPickupItem == null)
                return;

            OnPickupItem(item, offset.X, offset.Y);
        }

        internal void ClearHolding()
        {
            if (OnClearHolding == null)
                return;

            OnClearHolding();
        }
    }
}
