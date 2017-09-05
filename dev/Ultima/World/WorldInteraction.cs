﻿/***************************************************************************
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
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.UI;
#endregion

namespace UltimaXNA.Ultima.World {
    /// <summary>
    /// Hosts methods for interacting with the world.
    /// </summary>
    class WorldInteraction
    {
        private WorldModel m_World;
        private INetworkClient m_Network;
        private UserInterfaceService m_UserInterface;

        public WorldInteraction(WorldModel world)
        {
            m_Network = Service.Get<INetworkClient>();
            m_UserInterface = Service.Get<UserInterfaceService>();

            m_World = world;
        }

        private Serial m_lastTarget;
        public Serial LastTarget
        {
            get { return m_lastTarget; }
            set
            {
                m_lastTarget = value;
                m_Network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, m_lastTarget));
            }
        }
        private Serial m_lastWarModeTarget;
        public Serial LastWarModeTarget
        {
            get { return m_lastWarModeTarget; }
            set
            {
                m_lastWarModeTarget = value;
            }
        }

        /// <summary>
        /// For items, if server.expansion is less than AOS, sends single click packet.
        /// For mobiles, always sends a single click.
        /// Requests a context menu regardless of version.
        /// </summary>
        public void SingleClick(AEntity e)
        {
            if (!PlayerState.ClientFeatures.TooltipsEnabled)
            {
                m_Network.Send(new SingleClickPacket(e.Serial));
            }
            m_Network.Send(new RequestContextMenuPacket(e.Serial));
        }

        public void DoubleClick(AEntity e) // used by itemgumpling, paperdollinteractable, topmenu, worldinput.
        {
            if (e != null)
                m_Network.Send(new DoubleClickPacket(e.Serial));
        }
        
        public void AttackRequest(Mobile mobile)
        {
            // Do nothing on Invulnerable
            if (mobile.Notoriety == 0x7){
            }
            // Attack Innocents, Reds and Greys
            else if (mobile.Notoriety == 0x1 || mobile.Notoriety == 0x3 || mobile.Notoriety == 0x4 || mobile.Notoriety == 0x5 || mobile.Notoriety == 0x6)
            {
                m_Network.Send(new AttackRequestPacket(mobile.Serial));
                // Set last war mode target for highlighting
                LastWarModeTarget = mobile.Serial;
            }
            // CrimeQuery is enabled, ask before attacking others
            else if (Settings.UserInterface.CrimeQuery)
            {
                m_UserInterface.AddControl(new CrimeQueryGump(mobile), 0, 0);
            }
            // CrimeQuery is disabled, so attack without asking
            else
            {
                m_Network.Send(new AttackRequestPacket(mobile.Serial));
                // Set last war mode target for highlighting
                LastWarModeTarget = mobile.Serial;
            }
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

        public void BookHeaderNewChange(Serial serial, string title, string author)
        {
            m_Network.Send(new BookHeaderNewChangePacket(serial, title, author));
        }

        public void BookHeaderOldChange(Serial serial, string title, string author)
        {
            // Not yet implemented
            // m_Network.Send(new BookHeaderOldChangePacket(serial, title, author));
        }

        public void BookPageChange(Serial serial, int page, string[] lines)
        {
            m_Network.Send(new BookPageChangePacket(serial, page, lines));
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
                    gump = new SpellbookGump((SpellBook)entity);
                    m_UserInterface.AddControl(gump, 96, 96);
                }
                else
                {
                    gump = new ContainerGump(entity, ((ContainerItem)entity).ItemID);
                    m_UserInterface.AddControl(gump, 64, 64);
                }
            }
            return gump;
        }

        List<QueuedMessage> m_ChatQueue = new List<QueuedMessage>();

        public void ChatMessage(string text) // used by gump
        {
            ChatMessage(text, 0, 0, true);
        }

        public void ChatMessage(string text, int font, int hue, bool asUnicode)
        {
            m_ChatQueue.Add(new QueuedMessage(text, font, hue, asUnicode));

            ChatControl chat = Service.Get<ChatControl>();
            if (chat != null)
            {
                foreach (QueuedMessage msg in m_ChatQueue)
                {
                    chat.AddLine(msg.Text, msg.Font, msg.Hue, msg.AsUnicode);
                }
                m_ChatQueue.Clear();
            }
        }

        class QueuedMessage
        {
            public string Text;
            public int Hue;
            public int Font;
            public bool AsUnicode;

            public QueuedMessage(string text, int font, int hue, bool asUnicode)
            {
                Text = text;
                Hue = hue;
                Font = font;
                AsUnicode = asUnicode;
            }
        }

        public void CreateLabel(MessageTypes msgType, Serial serial, string text, int font, int hue, bool asUnicode)
        {
            if (serial.IsValid)
            {
                WorldModel.Entities.AddOverhead(msgType, serial, text, font, hue, asUnicode);
            }
            else
            {
                ChatMessage(text, font, hue, asUnicode);
            }
        }

        // ============================================================================================================
        // Cursor handling routines.
        // ============================================================================================================

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
