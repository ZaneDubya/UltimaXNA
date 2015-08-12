/***************************************************************************
 *   WorldInteraction.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.UI;
#endregion

namespace UltimaXNA.Ultima.World
{
    /// <summary>
    /// Hosts methods for interacting with the world.
    /// </summary>
    internal class WorldInteraction
    {
        private WorldModel m_World;
        private INetworkClient m_Network;
        private UserInterfaceService m_UserInterface;

        public WorldInteraction(WorldModel world)
        {
            m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();

            m_World = world;
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

        /// <summary>
        /// Informs the server we have single-clicked on an entity. Also requests a context menu.
        /// </summary>
        /// <param name="item">The entity clicked on.</param>
        public void SingleClick(AEntity item)
        {
            m_Network.Send(new SingleClickPacket(item.Serial));
            m_Network.Send(new RequestContextMenuPacket(item.Serial));
        }

        public void DoubleClick(AEntity item) // used by itemgumpling, paperdollinteractable, topmenu, worldinput.
        {
            if (item!=null)
                m_Network.Send(new DoubleClickPacket(item.Serial));
        } 

        public void ToggleWarMode() // used by paperdollgump.
        {
            m_Network.Send(new RequestWarModePacket(!((Mobile)WorldModel.Entities.GetPlayerEntity()).Flags.IsWarMode));
        }

        public void UseSkill(int index) // used by WorldInteraction
        {
            m_Network.Send(new RequestSkillUsePacket(index));
        }

        public void CastSpell(int index)
        {
            m_Network.Send(new CastSpellPacket(index));
        }

        public void ChangeSkillLock(SkillEntry skill)
        {
            if (skill == null)
                return;
            byte nextLockState = (byte)(skill.LockType + 1);
            if (nextLockState > 2)
                nextLockState = 0;
            m_Network.Send(new SetSkillLockPacket((ushort)skill.Index, nextLockState));
            skill.LockType = nextLockState;
        }

        public Gump OpenContainerGump(AEntity entity) // used by ultimaclient.
        {
            Gump gump;
            if ((gump = (Gump)m_UserInterface.GetControl(entity.Serial)) != null)
            {
                gump.Dispose();
            }
            else
            {

                if (entity is Corpse)
                {
                    gump = new ContainerGump(entity, 0x2006);
                    m_UserInterface.AddControl(gump, 96, 96);
                }
                else if (entity is SpellBook)
                {
                    gump = new SpellbookGump((SpellBook)entity, ((Container)entity).ItemID);
                    m_UserInterface.AddControl(gump, 96, 96);
                }
                else
                {
                    gump = new ContainerGump(entity, ((Container)entity).ItemID);
                    m_UserInterface.AddControl(gump, 64, 64);
                }
            }
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

            ChatControl chat = ServiceRegistry.GetService<ChatControl>();
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

        public void CreateLabel(MessageTypes msgType, Serial serial, string text, int hue, int font)
        {
            if (serial.IsValid)
            {
                WorldModel.Entities.AddOverhead(msgType, serial, "<outline>" + text, font, hue);
            }
            else
            {
                ChatMessage("[LABEL] " + text, font, hue);
            }
        }

        // ======================================================================
        // Cursor handling routines.
        // ======================================================================

        public Action<Item, int, int, int?> OnPickupItem;
        public Action OnClearHolding;

        internal void PickupItem(Item item, Point offset, int? amount = null)
        {
            if (item == null)
                return;
            if (OnPickupItem == null)
                return;

            OnPickupItem(item, offset.X, offset.Y, amount);
        }

        internal void ClearHolding()
        {
            if (OnClearHolding == null)
                return;

            OnClearHolding();
        }
    }
}
