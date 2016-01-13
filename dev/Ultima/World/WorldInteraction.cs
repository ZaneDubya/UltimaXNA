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
                m_Network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, m_lastTarget));
            }
        }

        public void SendSpeech(string text, ChatMode mode) // used by chatwindow.
        {
            MessageTypes speechType = MessageTypes.Normal;
            int hue = 0;

            switch (mode)
            {
                case ChatMode.Default:
                    speechType = MessageTypes.Normal;
                    hue = Settings.UserInterface.SpeechColor;
                    break;
                case ChatMode.Whisper:
                    speechType = MessageTypes.Whisper;
                    hue = Settings.UserInterface.SpeechColor;
                    break;
                case ChatMode.Emote:
                    speechType = MessageTypes.Emote;
                    hue = Settings.UserInterface.EmoteColor;
                    break;
                case ChatMode.Party:
                    speechType = MessageTypes.Party;
                    hue = Settings.UserInterface.PartyMsgColor;
                    string ptCmd = text.ToLower();
                    switch (Party.State)
                    {
                        case PartyState.Alone:
                            if (!ptCmd.StartsWith("add"))
                            {
                                //AddTextMessage(string.Format("Note to self: {0}", cmd.Substring(1)), DefaultFont, Hues.Load(0x3b2));
                                break;
                            }
                            m_Network.Send(new PParty_AddMember());
                            break;

                        case PartyState.Joining:
                            {
                                if (!ptCmd.StartsWith("accept"))
                                {
                                    if (ptCmd.StartsWith("decline"))
                                    {
                                        m_Network.Send(new PParty_Decline(Party.Leader));
                                    }
                                    else
                                    {
                                        ChatMessage("Use '/accept' or '/decline'.", 3, 10, false);
                                    }
                                    break;
                                }
                                m_Network.Send(new PParty_Accept(Party.Leader));
                                break;
                            }
                        case PartyState.Joined:
                            {
                                if (!Party.IsLeader || !ptCmd.StartsWith("add"))
                                {
                                    if (Party.IsLeader && ptCmd.StartsWith("rem"))
                                    {
                                        m_Network.Send(new PParty_RemoveMember());
                                    }
                                    else if (ptCmd.StartsWith("accept"))
                                    {
                                        ChatMessage("You already in a party, so You cannot accept any party", 3, 10, false);
                                    }
                                    else if (ptCmd.StartsWith("decline"))
                                    {
                                        ChatMessage("If you need quit the party. please use /quit", 3, 10, false);
                                    }
                                    else if (ptCmd.StartsWith("quit"))
                                    {
                                        m_Network.Send(new PParty_Quit());
                                    }
                                    else if (ptCmd.StartsWith("loot on"))
                                    {
                                        m_Network.Send(new PParty_SetCanLoot(true));
                                    }
                                    else if (ptCmd.StartsWith("loot off"))
                                    {
                                        m_Network.Send(new PParty_SetCanLoot(false));
                                    }
                                    else if (ptCmd.StartsWith("loot"))
                                    {
                                        ChatMessage("Use '/loot on' or '/loot off'.", 3, 10, false);
                                    }
                                    else if (Party.Members.Length > 1)
                                    {
                                        if ((ptCmd.Length >=1) && char.IsDigit(ptCmd, 0))
                                        {
                                            try
                                            {
                                                int index = Convert.ToInt32(ptCmd.Substring(0, 1)) - 1;
                                                if ((index >= 0) && (index < Party.Members.Length))
                                                {
                                                    if (index == Party.Index)
                                                    {
                                                        ChatMessage(string.Format("Note to self: {0}", text.Substring(2)), 3, 10, false);
                                                    }
                                                    else
                                                    {
                                                        string str4;
                                                        Mobile mobile = WorldModel.Entities.GetPlayerEntity();
                                                        if (((mobile == null) || ((str4 = mobile.Name) == null)) || ((str4 = str4.Trim()).Length <= 0))
                                                        {
                                                            str4 = "You";
                                                        }
                                                        ChatMessage(string.Format("<{0}> {1}", str4, ptCmd.Substring(2)), 3, 60, false);
                                                        m_Network.Send(new PParty_PrivateMessage(Party.Members[index], text.Substring(2)));
                                                    }
                                                    break;
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }
                                        m_Network.Send(new PParty_PublicMessage(ptCmd));
                                    }
                                    else
                                    {
                                        ChatMessage(string.Format("Note to self: {0}", ptCmd.Substring(1)), 3, 10, false);
                                    }
                                    break;
                                }
                                m_Network.Send(new PParty_AddMember());
                                break;
                            }
                    }
                    break;
                case ChatMode.Guild:
                    speechType = MessageTypes.Guild;
                    hue = Settings.UserInterface.GuildMsgColor;
                    break;
                case ChatMode.Alliance:
                    speechType = MessageTypes.Party;
                    hue = Settings.UserInterface.AllianceMsgColor;
                    break;
            }
            m_Network.Send(new AsciiSpeechPacket(speechType, 0, hue + 2, "ENU", text));
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
            ChatMessage(text, 0, 0, true);
        }

        public void ChatMessage(string text, int font, int hue, bool asUnicode)
        {
            m_ChatQueue.Add(new QueuedMessage(text, font, hue, asUnicode));

            ChatControl chat = ServiceRegistry.GetService<ChatControl>();
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
                ChatMessage("[LABEL] " + text, font, hue, asUnicode);
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
