/***************************************************************************
 *   WorldClient.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
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
using System.Diagnostics;
using System.Threading;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Audio;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.Network.Server;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.Entities.Multis;
using UltimaXNA.Ultima.World.Input;
#endregion

namespace UltimaXNA.Ultima.World
{
    class WorldClient : IDisposable
    {
        private Timer m_KeepAliveTimer;
        private INetworkClient m_Network;
        private UserInterfaceService m_UserInterface;

        private WorldModel m_World;

        private List<Tuple<int, TypedPacketReceiveHandler>> m_RegisteredHandlers;

        public WorldClient(WorldModel world)
        {
            m_World = world;

            m_RegisteredHandlers = new List<Tuple<int, TypedPacketReceiveHandler>>();
            m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
        }

        public void Initialize()
        {
            Register<DamagePacket>(0x0B, "Damage", 0x07, new TypedPacketReceiveHandler(ReceiveDamage));
            Register<MobileStatusCompactPacket>(0x11, "Mobile Status Compact", -1, new TypedPacketReceiveHandler(ReceiveStatusInfo));
            Register<WorldItemPacket>(0x1A, "World Item", -1, new TypedPacketReceiveHandler(ReceiveWorldItem));
            Register<AsciiMessagePacket>(0x1C, "Ascii Meessage", -1, new TypedPacketReceiveHandler(ReceiveAsciiMessage));
            Register<RemoveEntityPacket>(0x1D, "Remove Entity", 5, new TypedPacketReceiveHandler(ReceiveDeleteObject));
            Register<MobileUpdatePacket>(0x20, "Mobile Update", 19, new TypedPacketReceiveHandler(ReceiveMobileUpdate));
            Register<MovementRejectPacket>(0x21, "Movement Rejection", 8, new TypedPacketReceiveHandler(ReceiveMoveRej));
            Register<MoveAcknowledgePacket>(0x22, "Move Acknowledged", 3, new TypedPacketReceiveHandler(ReceiveMoveAck));
            Register<DragEffectPacket>(0x23, "Drag Effect", 26, new TypedPacketReceiveHandler(ReceiveDragItem));
            Register<OpenContainerPacket>(0x24, "Open Container", 7, new TypedPacketReceiveHandler(ReceiveContainer));
            Register<ContainerContentUpdatePacket>(0x25, "Container Content Update", 21, new TypedPacketReceiveHandler(ReceiveAddSingleItemToContainer));
            Register<LiftRejectionPacket>(0x27, "Lift Rejection", 2, new TypedPacketReceiveHandler(ReceiveRejectMoveItemRequest));
            Register<ResurrectionMenuPacket>(0x2C, "Resurect menu", 2, new TypedPacketReceiveHandler(ReceiveResurrectionMenu));
            Register<MobileAttributesPacket>(0x2D, "Mob Attributes", 17, new TypedPacketReceiveHandler(ReceiveMobileAttributes));
            Register<WornItemPacket>(0x2E, "Worn Item", 15, new TypedPacketReceiveHandler(ReceiveWornItem));
            Register<SwingPacket>(0x2F, "Swing", 10, new TypedPacketReceiveHandler(ReceiveOnSwing));
            Register<SendSkillsPacket>(0x3A, "Skills list", -1, new TypedPacketReceiveHandler(ReceiveSkillsList));
            Register<ContainerContentPacket>(0x3C, "Container Content", -1, new TypedPacketReceiveHandler(ReceiveAddMultipleItemsToContainer));
            Register<PersonalLightLevelPacket>(0x4E, "Personal Light Level", 6, new TypedPacketReceiveHandler(ReceivePersonalLightLevel));
            Register<OverallLightLevelPacket>(0x4F, "Overall Light Level", 2, new TypedPacketReceiveHandler(ReceiveOverallLightLevel));
            Register<PopupMessagePacket>(0x53, "Popup Message", 2, new TypedPacketReceiveHandler(ReceivePopupMessage));
            Register<PlaySoundEffectPacket>(0x54, "Play Sound Effect", 12, new TypedPacketReceiveHandler(ReceivePlaySoundEffect));
            Register<TimePacket>(0x5B, "Time", 4, new TypedPacketReceiveHandler(ReceiveTime));
            Register<WeatherPacket>(0x65, "Set Weather", 4, new TypedPacketReceiveHandler(ReceiveSetWeather));
            Register<TargetCursorPacket>(0x6C, "TargetCursor", 19, new TypedPacketReceiveHandler(ReceiveTargetCursor));
            Register<PlayMusicPacket>(0x6D, "Play Music", 3, new TypedPacketReceiveHandler(ReceivePlayMusic));
            Register<MobileAnimationPacket>(0x6E, "Character Animation", 14, new TypedPacketReceiveHandler(ReceiveMobileAnimation));
            Register<GraphicEffectPacket>(0x70, "Graphical Effect 1", 28, new TypedPacketReceiveHandler(ReceiveGraphicEffect));
            Register<WarModePacket>(0x72, "War Mode", 5, new TypedPacketReceiveHandler(ReceiveWarMode));
            Register<VendorBuyListPacket>(0x74, "Vendor Buy List", -1, new TypedPacketReceiveHandler(ReceiveOpenBuyWindow));
            Register<SubServerPacket>(0x76, "New Subserver", 16, new TypedPacketReceiveHandler(ReceiveNewSubserver));
            Register<MobileMovingPacket>(0x77, "Mobile Moving", 17, new TypedPacketReceiveHandler(ReceiveMobileMoving));
            Register<MobileIncomingPacket>(0x78, "Mobile Incoming", -1, new TypedPacketReceiveHandler(ReceiveMobileIncoming));
            Register<DisplayMenuPacket>(0x7C, "Display Menu", -1, new TypedPacketReceiveHandler(ReceiveDisplayMenu));
            Register<OpenPaperdollPacket>(0x88, "Open Paperdoll", 66, new TypedPacketReceiveHandler(ReceiveOpenPaperdoll));
            Register<CorpseClothingPacket>(0x89, "Corpse Clothing", -1, new TypedPacketReceiveHandler(ReceiveCorpseClothing));
            Register<PlayerMovePacket>(0x97, "Player Move", 2, new TypedPacketReceiveHandler(ReceivePlayerMove));
            Register<RequestNameResponsePacket>(0x98, "Request Name Response", -1, new TypedPacketReceiveHandler(ReceiveRequestNameResponse));
            Register<TargetCursorMultiPacket>(0x99, "Target Cursor Multi Object", 26, new TypedPacketReceiveHandler(ReceiveTargetCursorMulti));
            Register<VendorSellListPacket>(0x9E, "Vendor Sell List", -1, new TypedPacketReceiveHandler(ReceiveSellList));
            Register<UpdateHealthPacket>(0xA1, "Update Current Health", 9, new TypedPacketReceiveHandler(ReceiveUpdateHealth));
            Register<UpdateManaPacket>(0xA2, "Update Current Mana", 9, new TypedPacketReceiveHandler(ReceiveUpdateMana));
            Register<UpdateStaminaPacket>(0xA3, "Update Current Stamina", 9, new TypedPacketReceiveHandler(ReceiveUpdateStamina));
            Register<OpenWebBrowserPacket>(0xA5, "Open Web Browser", -1, new TypedPacketReceiveHandler(ReceiveOpenWebBrowser));
            Register<TipNoticePacket>(0xA6, "Tip/Notice Window", -1, new TypedPacketReceiveHandler(ReceiveTipNotice));
            Register<ChangeCombatantPacket>(0xAA, "Change Combatant", 5, new TypedPacketReceiveHandler(ReceiveChangeCombatant));
            Register<UnicodeMessagePacket>(0xAE, "Unicode Message", -1, new TypedPacketReceiveHandler(ReceiveUnicodeMessage));
            Register<DeathAnimationPacket>(0xAF, "Death Animation", 13, new TypedPacketReceiveHandler(ReceiveDeathAnimation));
            Register<DisplayGumpFastPacket>(0xB0, "Display Gump Fast", -1, new TypedPacketReceiveHandler(ReceiveDisplayGumpFast));
            Register<ObjectHelpResponsePacket>(0xB7, "Object Help Response ", -1, new TypedPacketReceiveHandler(ReceiveObjectHelpResponse));
            Register<QuestArrowPacket>(0xBA, "Quest Arrow", 6, new TypedPacketReceiveHandler(ReceiveQuestArrow));
            Register<SeasonChangePacket>(0xBC, "Seasonal Change", 3, new TypedPacketReceiveHandler(ReceiveSeasonalInformation));
            Register<GeneralInfoPacket>(0xBF, "General Information", -1, new TypedPacketReceiveHandler(ReceiveGeneralInfo));
            Register<GraphicEffectHuedPacket>(0xC0, "Hued Effect", 36, new TypedPacketReceiveHandler(ReceiveHuedEffect));
            Register<MessageLocalizedPacket>(0xC1, "Message Localized", -1, new TypedPacketReceiveHandler(ReceiveCLILOCMessage));
            Register<InvalidMapEnablePacket>(0xC6, "Invalid Map Enable", 1, new TypedPacketReceiveHandler(ReceiveInvalidMapEnable));
            Register<GraphicEffectExtendedPacket>(0xC7, "Particle Effect", 49, new TypedPacketReceiveHandler(ReceiveOnParticleEffect));
            Register<GlobalQueuePacket>(0xCB, "Global Queue Count", 7, new TypedPacketReceiveHandler(ReceiveGlobalQueueCount));
            Register<MessageLocalizedAffixPacket>(0xCC, "Message Localized Affix ", -1, new TypedPacketReceiveHandler(ReceiveMessageLocalizedAffix));
            Register<Extended0x78Packet>(0xD3, "Extended 0x78", -1, new TypedPacketReceiveHandler(ReceiveExtended0x78));
            Register<ObjectPropertyListPacket>(0xD6, "Mega Cliloc", -1, new TypedPacketReceiveHandler(ReceiveObjectPropertyList));
            Register<CustomHousePacket>(0xD8, "Send Custom House", -1, new TypedPacketReceiveHandler(ReceiveSendCustomHouse));
            Register<ObjectPropertyListUpdatePacket>(0xDC, "SE Introduced Revision", 9, new TypedPacketReceiveHandler(ReceiveToolTipRevision));
            Register<CompressedGumpPacket>(0xDD, "Compressed Gump", -1, new TypedPacketReceiveHandler(ReceiveCompressedGump));

            /* Deprecated (not used by RunUO) and/or not implmented
             * Left them here incase we need to implement in the future
            network.Register<HealthBarStatusPacket>(0x17, "Health Bar Status Update", 12, OnHealthBarStatusUpdate);
            network.Register<KickPlayerPacket>(0x26, "Kick Player", 5, OnKickPlayer);
            network.Register<DropItemFailedPacket>(0x28, "Drop Item Failed", 5, OnDropItemFailed);
            network.Register<PaperdollClothingAddAckPacket>(0x29, "Paperdoll Clothing Add Ack", 1, OnPaperdollClothingAddAck
            network.Register<RecvPacket>(0x30, "Attack Ok", 5, OnAttackOk);
            network.Register<BloodPacket>(0x2A, "Blood", 5, OnBlood);
            network.Register<RecvPacket>(0x33, "Pause Client", -1, OnPauseClient);
            network.Register<RecvPacket>(0x90, "Map Message", -1, OnMapMessage);
            network.Register<RecvPacket>(0x9C, "Help Request", -1, OnHelpRequest);
            network.Register<RecvPacket>(0xAB, "Gump Text Entry Dialog", -1, OnGumpDialog);
            network.Register<RecvPacket>(0xB2, "Chat Message", -1, OnChatMessage);
            network.Register<RecvPacket>(0xC4, "Semivisible", -1, OnSemivisible);
            network.Register<RecvPacket>(0xD2, "Extended 0x20", -1, OnExtended0x20);
            network.Register<RecvPacket>(0xDB, "Character Transfer Log", -1, OnCharacterTransferLog);
            network.Register<RecvPacket>(0xDC, "SE Introduced Revision", -1, OnToolTipRevision);
            network.Register<RecvPacket>(0xDE, "Update Mobile Status", -1, OnUpdateMobileStatus);
            network.Register<RecvPacket>(0xDF, "Buff/Debuff System", -1, OnBuffDebuff);
            network.Register<RecvPacket>(0xE2, "Mobile status/Animation update", -1, OnMobileStatusAnimationUpdate);
            network.Register<RecvPacket>(0xF0, "Krrios client special", -1, OnKrriosClientSpecial);
            */
            MobileMovement.SendMoveRequestPacket += InternalOnEntity_SendMoveRequestPacket;
        }

        public void Dispose()
        {
            StopKeepAlivePackets();

            for (int i = 0; i < m_RegisteredHandlers.Count; i++)
                m_Network.Unregister(m_RegisteredHandlers[i].Item1, m_RegisteredHandlers[i].Item2);
            m_RegisteredHandlers.Clear();
            m_RegisteredHandlers = null;

            MobileMovement.SendMoveRequestPacket -= InternalOnEntity_SendMoveRequestPacket;
        }

        public void Register<T>(int id, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket
        {
            m_RegisteredHandlers.Add(new Tuple<int, TypedPacketReceiveHandler>(id, onReceive));
            m_Network.Register<T>(id, name, length, onReceive);
        }
        
        public void SendWorldLoginPackets()
        {
            GetMySkills();
            SendClientVersion();
            SendClientScreenSize();
            SendClientLocalization();
            // Packet: BF 00 0A 00 0F 0A 00 00 00 1F
            // Packet: 09 00 00 00 02  
            // Packet: 06 80 00 00 17
            GetMyBasicStatus();
            // Packet: D6 00 0B 00 00 00 02 00 00 00 17
            // Packet: D6 00 37 40 00 00 FB 40 00 00 FD 40 00 00 FE 40
            //         00 00 FF 40 00 01 00 40 00 01 02 40 00 01 03 40
            //         00 01 04 40 00 01 05 40 00 01 06 40 00 01 07 40
            //         00 01 24 40 00 01 26 
        }

        public void StartKeepAlivePackets()
        {
            m_KeepAliveTimer = new Timer(
                e => SendKeepAlivePacket(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(4));
        }

        private void StopKeepAlivePackets()
        {
            if (m_KeepAliveTimer != null)
                m_KeepAliveTimer.Dispose();
        }

        private void SendKeepAlivePacket()
        {
            m_Network.Send(new UOSEKeepAlivePacket());
        }

        public void SendGumpMenuSelect(int id, int gumpId, int buttonId, int[] switchIds, Tuple<short, string>[] textEntries)
        {
            m_Network.Send(new GumpMenuSelectPacket(id, gumpId, buttonId, switchIds, textEntries));
        }

        /// <summary>
        /// Sends the server the client version. Version is specified in EngineVars.
        /// </summary>
        public void SendClientVersion()
        {
            if (Settings.UltimaOnline.ClientVersion.Length != 4)
            {
                Tracer.Warn("Cannot send seed packet: Version array is incorrectly sized.");
            }
            else
            {
                m_Network.Send(new ClientVersionPacket(Settings.UltimaOnline.ClientVersion));
            }
        }

        public void GetMySkills()
        {
            m_Network.Send(new GetPlayerStatusPacket(0x05, WorldModel.PlayerSerial));
        }

        public void SendClientScreenSize()
        {
            m_Network.Send(new ReportClientScreenSizePacket(800, 600));
        }

        public void SendClientLocalization()
        {
            m_Network.Send(new ReportClientLocalizationPacket("ENU"));
        }

        public void GetMyBasicStatus()
        {
            m_Network.Send(new GetPlayerStatusPacket(0x04, WorldModel.PlayerSerial));
        }

        private void ReceiveTargetCursor(IRecvPacket packet)
        {
            TargetCursorPacket p = (TargetCursorPacket)packet;
            m_World.Cursor.SetTargeting((WorldCursor.TargetType)p.CommandType, p.CursorID);
        }

        private void ReceiveTargetCursorMulti(IRecvPacket packet)
        {
            TargetCursorMultiPacket p = (TargetCursorMultiPacket)packet;
            m_World.Cursor.SetTargetingMulti(p.DeedSerial, p.MultiModel);
        }

        private void InternalOnEntity_SendMoveRequestPacket(MoveRequestPacket packet)
        {
            m_Network.Send(packet);
        }

        // ======================================================================
        // Effect handling
        // ======================================================================

        private void ReceiveGraphicEffect(IRecvPacket packet)
        {
            WorldModel.Effects.Add((GraphicEffectPacket)packet);
        }

        private void ReceiveHuedEffect(IRecvPacket packet)
        {
            WorldModel.Effects.Add((GraphicEffectHuedPacket)packet);
        }

        private void ReceiveOnParticleEffect(IRecvPacket packet)
        {
            WorldModel.Effects.Add((GraphicEffectExtendedPacket)packet);
        }

        // ======================================================================
        // Entity handling
        // ======================================================================

        private void ReceiveAddMultipleItemsToContainer(IRecvPacket packet)
        {
            ContainerContentPacket p = (ContainerContentPacket)packet;
            foreach (ItemInContainer i in p.Items)
            {
                // Add the item...
                Item item = add_Item(i.Serial, i.ItemID, i.Hue, i.ContainerSerial, i.Amount);
                item.InContainerPosition = new Point(i.X, i.Y);
                // ... and add it the container contents of the container.
                Container container = WorldModel.Entities.GetObject<Container>(i.ContainerSerial, true);
                if (container != null)
                    container.AddItem(item);
            }
        }

        private void ReceiveAddSingleItemToContainer(IRecvPacket packet)
        {
            ContainerContentUpdatePacket p = (ContainerContentUpdatePacket)packet;

            // Add the item...
            Item item = add_Item(p.Serial, p.ItemId, p.Hue, p.ContainerSerial, p.Amount);
            item.InContainerPosition = new Point(p.X, p.Y);
            // ... and add it the container contents of the container.
            AEntity container = WorldModel.Entities.GetObject<AEntity>(p.ContainerSerial, false);
            if (container == null)
            {
                // shouldn't we already have the container? Throw an error?
                Tracer.Warn("SingleItemToContainer packet arrived before container entity created.");
            }
            if (container is Container) // place in container
            {
                (container as Container).AddItem(item);
            }
            else if (container is Mobile) // secure trade
            {

            }
        }

        private Item add_Item(Serial serial, int itemID, int nHue, Serial parentSerial, int amount)
        {
            Item item;
            if (itemID == 0x2006)
            {
                // special case for corpses.
                item = WorldModel.Entities.GetObject<Corpse>((int)serial, true);
                ContainerContentPacket.NextContainerContentsIsPre6017 = true;
            }
            else
            {
                if (TileData.ItemData[itemID].IsContainer)
                {
                    // special case for spellbooks.
                    if (SpellBook.IsSpellBookItem((ushort)itemID))
                    {
                        item = WorldModel.Entities.GetObject<SpellBook>(serial, true);
                    }
                    else
                    {
                        item = WorldModel.Entities.GetObject<Container>(serial, true);
                    }
                }
                else
                    item = WorldModel.Entities.GetObject<Item>(serial, true);
            }
            if (item == null)
                return null;
            item.Amount = amount;
            item.ItemID = itemID;
            item.Hue = nHue;
            return item;
        }

        private void ReceiveContainer(IRecvPacket packet)
        {
            OpenContainerPacket p = (OpenContainerPacket)packet;

            Container item;
            // Special case for 0x30, which tells us to open a buy from vendor window.
            if (p.GumpId == 0x30)
            {
                Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
                if (mobile == null)
                {
                    // log error - shopkeeper does not exist?
                }
                else
                {
                    item = mobile.VendorShopContents;
                }
            }
            else
            {
                item = WorldModel.Entities.GetObject<Container>(p.Serial, false);
                if (item == null)
                {
                    // log error - item does not exist
                    m_World.Interaction.ChatMessage(string.Format("Client: Object {0} has no support for a container object!", item.Serial));
                }
                else
                {
                    m_World.Interaction.OpenContainerGump(item);
                }
            }
        }

        private void ReceiveWorldItem(IRecvPacket packet)
        {
            WorldItemPacket p = (WorldItemPacket)packet;
            // Now create the GameObject.
            // If the iItemID < 0x4000, this is a regular game object.
            // If the iItemID >= 0x4000, then this is a multiobject.
            if (p.ItemID <= 0x4000)
            {
                Item item = add_Item(p.Serial, p.ItemID, p.Hue, 0, p.StackAmount);
                item.Position.Set(p.X, p.Y, p.Z);
            }
            else
            {
                int multiID = p.ItemID - 0x4000;
                Multi multi = WorldModel.Entities.GetObject<Multi>(p.Serial, true);
                multi.Position.Set(p.X, p.Y, p.Z);
                multi.MultiID = p.ItemID;
            }
        }

        private void ReceiveWornItem(IRecvPacket packet)
        {
            WornItemPacket p = (WornItemPacket)packet;
            Item item = add_Item(p.Serial, p.ItemId, p.Hue, p.ParentSerial, 0);
            Mobile m = WorldModel.Entities.GetObject<Mobile>(p.ParentSerial, false);
            m.WearItem(item, p.Layer);
            if (item.PropertyList.Hash == 0)
                m_Network.Send(new QueryPropertiesPacket(item.Serial));
        }

        private void ReceiveDeleteObject(IRecvPacket packet)
        {
            RemoveEntityPacket p = (RemoveEntityPacket)packet;
            WorldModel.Entities.RemoveEntity(p.Serial);
        }

        private void ReceiveMobileIncoming(IRecvPacket packet)
        {
            MobileIncomingPacket p = (MobileIncomingPacket)packet;
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, true);
            mobile.Body = p.BodyID;
            mobile.Hue = (int)p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            mobile.Flags = p.Flags;
            mobile.Notoriety = p.Notoriety;
            mobile.Notoriety = p.Notoriety;

            for (int i = 0; i < p.Equipment.Length; i++)
            {
                Item item = add_Item(p.Equipment[i].Serial, p.Equipment[i].GumpId, p.Equipment[i].Hue, p.Serial, 0);
                mobile.WearItem(item, p.Equipment[i].Layer);
                if (item.PropertyList.Hash == 0)
                    m_Network.Send(new QueryPropertiesPacket(item.Serial));
            }

            if (mobile.Name == null || mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                m_Network.Send(new RequestNamePacket(p.Serial));
            }
        }

        private void ReceiveDeathAnimation(IRecvPacket packet)
        {
            DeathAnimationPacket p = (DeathAnimationPacket)packet;
            Mobile m = WorldModel.Entities.GetObject<Mobile>(p.PlayerSerial, false);
            Corpse c = WorldModel.Entities.GetObject<Corpse>(p.CorpseSerial, false);
            if (m == null)
                Tracer.Warn("DeathAnimation received for mobile which does not exist.");
            else if (c == null)
                Tracer.Warn("DeathAnimation received for corpse which does not exist.");
            else
            {
                c.Facing = m.Facing;
                c.MobileSerial = p.PlayerSerial;
                c.PlayDeathAnimation();
            }
        }

        private void ReceiveDragItem(IRecvPacket packet)
        {
            DragEffectPacket p = (DragEffectPacket)packet;
            // This is sent by the server to display an item being dragged from one place to another.
            // Note that this does not actually move the item, it just displays an animation.

            // bool iSourceGround = false;
            // bool iDestGround = false;
            if (p.SourceContainer == 0xFFFFFFFF)
            {
                // iSourceGround = true;
            }

            if (p.DestContainer == 0xFFFFFFFF)
            {
                // iDestGround = true;
            }
            announce_UnhandledPacket(packet);
        }

        private void ReceiveMobileAttributes(IRecvPacket packet)
        {
            MobileAttributesPacket p = (MobileAttributesPacket)packet;
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;

            mobile.Health.Current = p.CurrentHits;
            mobile.Health.Max = p.MaxHits;

            mobile.Mana.Current = p.CurrentMana;
            mobile.Mana.Max = p.MaxMana;

            mobile.Stamina.Current = p.CurrentStamina;
            mobile.Stamina.Max = p.MaxStamina;
        }

        private void ReceiveMobileAnimation(IRecvPacket packet)
        {
            MobileAnimationPacket p = (MobileAnimationPacket)packet;
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;

            mobile.Animate(p.Action, p.FrameCount, p.RepeatCount, p.Reverse, p.Repeat, p.Delay);
        }

        private void ReceiveMobileMoving(IRecvPacket packet)
        {
            MobileMovingPacket p = (MobileMovingPacket)packet;
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, true);
            if (mobile == null)
                return;

            mobile.Body = p.BodyID;
            mobile.Flags = p.Flags;
            mobile.Notoriety = p.Notoriety;

            if (mobile.IsClientEntity)
                return;

            if (mobile.Position.IsNullPosition)
            {
                mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            }
            else
            {
                mobile.Mobile_AddMoveEvent(p.X, p.Y, p.Z, p.Direction);
            }
        }

        private void ReceiveMobileUpdate(IRecvPacket packet)
        {
            MobileUpdatePacket p = (MobileUpdatePacket)packet;
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, true);
            if (mobile == null)
                return;

            mobile.Body = p.BodyID;
            mobile.Flags = p.Flags;
            mobile.Hue = (int)p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);

            if (mobile.Name == null || mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                m_Network.Send(new RequestNamePacket(p.Serial));
            }
        }

        private void ReceiveMoveAck(IRecvPacket packet)
        {
            MoveAcknowledgePacket p = (MoveAcknowledgePacket)packet;
            Mobile player = (Mobile)WorldModel.Entities.GetPlayerEntity();
            player.PlayerMobile_MoveEventAck(p.Sequence);
            player.Notoriety = p.Notoriety;
        }

        private void ReceiveMoveRej(IRecvPacket packet)
        {
            MovementRejectPacket p = (MovementRejectPacket)packet;
            Mobile player = (Mobile)WorldModel.Entities.GetPlayerEntity();
            player.PlayerMobile_MoveEventRej(p.Sequence, p.X, p.Y, p.Z, p.Direction);
        }

        private void ReceivePlayerMove(IRecvPacket packet)
        {
            PlayerMovePacket p = (PlayerMovePacket)packet;
            announce_UnhandledPacket(packet);
        }

        private void ReceiveRejectMoveItemRequest(IRecvPacket packet)
        {
            LiftRejectionPacket p = (LiftRejectionPacket)packet;
            m_World.Interaction.ChatMessage("Could not pick up item: " + p.ErrorMessage);
            m_World.Interaction.ClearHolding();
        }

        // ======================================================================
        // Corpse handling
        // ======================================================================

        private void ReceiveCorpseClothing(IRecvPacket packet)
        {
            CorpseClothingPacket p = (CorpseClothingPacket)packet;
            Corpse corpse = WorldModel.Entities.GetObject<Corpse>(p.CorpseSerial, false);
            if (corpse == null)
                return;
            corpse.LoadCorpseClothing(p.Items);
        }

        // ======================================================================
        // Combat handling
        // ======================================================================

        private void ReceiveChangeCombatant(IRecvPacket packet)
        {
            ChangeCombatantPacket p = (ChangeCombatantPacket)packet;
            if (p.Serial > 0x00000000)
                m_World.Interaction.LastTarget = p.Serial;
        }

        private void ReceiveDamage(IRecvPacket packet)
        {
            DamagePacket p = (DamagePacket)packet;
            Mobile entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;

            m_World.Interaction.ChatMessage(string.Format("{0} takes {1} damage!", entity.Name, p.Damage));
        }

        private void ReceiveOnSwing(IRecvPacket packet)
        {
            SwingPacket p = (SwingPacket)packet;
            // this changes our last target - does this behavior match legacy?
            if (p.Attacker == WorldModel.PlayerSerial)
            {
                m_World.Interaction.LastTarget = p.Defender;
            }
        }

        private void ReceiveWarMode(IRecvPacket packet)
        {
            WarModePacket p = (WarModePacket)packet;
            ((Mobile)WorldModel.Entities.GetPlayerEntity()).Flags.IsWarMode = p.WarMode;
        }

        private void ReceiveUpdateMana(IRecvPacket packet)
        {
            UpdateManaPacket p = (UpdateManaPacket)packet;
            Mobile entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Mana.Update(p.Current, p.Max);
        }

        private void ReceiveUpdateStamina(IRecvPacket packet)
        {
            UpdateStaminaPacket p = (UpdateStaminaPacket)packet;
            Mobile entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Stamina.Update(p.Current, p.Max);
        }

        private void ReceiveUpdateHealth(IRecvPacket packet)
        {
            UpdateHealthPacket p = (UpdateHealthPacket)packet;
            Mobile entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Health.Update(p.Current, p.Max);
        }

        // ======================================================================
        // Chat / messaging handling
        // ======================================================================

        private void ReceiveCLILOCMessage(IRecvPacket packet)
        {
            MessageLocalizedPacket p = (MessageLocalizedPacket)packet;

            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            string strCliLoc = constructCliLoc(provider.GetString(p.CliLocNumber), p.Arguements);
            ReceiveTextMessage(p.MessageType, strCliLoc, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private void ReceiveAsciiMessage(IRecvPacket packet)
        {
            AsciiMessagePacket p = (AsciiMessagePacket)packet;
            ReceiveTextMessage(p.MsgType, p.Text, p.Hue, p.Font, p.Serial, p.Name1);
        }

        private void ReceiveUnicodeMessage(IRecvPacket packet)
        {
            UnicodeMessagePacket p = (UnicodeMessagePacket)packet;
            ReceiveTextMessage(p.MsgType, p.SpokenText, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private string constructCliLoc(string baseCliloc, string arg = null, bool capitalize = false)
        {
            if (string.IsNullOrEmpty(baseCliloc))
                return string.Empty;

            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

            if (arg == null)
            {
                if (capitalize)
                {
                    return Utility.CapitalizeFirstCharacter(baseCliloc);
                }
                else
                {
                    return baseCliloc;
                }
            }
            else
            {
                string[] args = arg.Split('\t');
                for (int i = 0; i < args.Length; i++)
                {
                    if ((args[i].Length > 0) && (args[i].Substring(0, 1) == "#"))
                    {
                        int clilocID = Convert.ToInt32(args[i].Substring(1));
                        args[i] = provider.GetString(clilocID);
                    }
                }

                string construct = baseCliloc;
                for (int i = 0; i < args.Length; i++)
                {
                    int iBeginReplace = construct.IndexOf('~', 0);
                    int iEndReplace = construct.IndexOf('~', iBeginReplace + 1);
                    if ((iBeginReplace != -1) && (iEndReplace != -1))
                    {
                        construct = construct.Substring(0, iBeginReplace) + args[i] + construct.Substring(iEndReplace + 1, construct.Length - iEndReplace - 1);
                    }
                    else
                    {
                        construct = baseCliloc;
                    }

                }

                if (capitalize)
                {
                    return Utility.CapitalizeFirstCharacter(construct);
                }
                else
                {
                    return construct;
                }
            }
        }

        private void ReceiveTextMessage(MessageTypes msgType, string text, int hue, int font, Serial serial, string speakerName)
        {
            PlayerState.Journaling.AddEntry(text);

            Overhead overhead;
            switch (msgType)
            {
                case MessageTypes.Regular:
                case MessageTypes.SpeechUnknown:
                    overhead = WorldModel.Entities.AddOverhead(msgType, serial, "<outline>" + text, font, hue);
                    if (overhead != null)
                    {
                        m_World.Interaction.ChatMessage(speakerName + ": " + text, font, hue);
                    }
                    else
                    {
                        m_World.Interaction.ChatMessage(text, font, hue);
                    }
                    break;
                case MessageTypes.System:
                    m_World.Interaction.ChatMessage("[SYSTEM] " + text, font, hue);
                    break;
                case MessageTypes.Emote:
                    m_World.Interaction.ChatMessage("[EMOTE] " + text, font, hue);
                    break;
                case MessageTypes.Label:
                    m_World.Interaction.CreateLabel(msgType, serial, text, font, hue);
                    break;
                case MessageTypes.Focus: // on player?
                    m_World.Interaction.ChatMessage("[FOCUS] " + text, font, hue);
                    break;
                case MessageTypes.Whisper:
                    m_World.Interaction.ChatMessage("[WHISPER] " + text, font, hue);
                    break;
                case MessageTypes.Yell:
                    m_World.Interaction.ChatMessage("[YELL] " + text, font, hue);
                    break;
                case MessageTypes.Spell:
                    m_World.Interaction.ChatMessage("[SPELL] " + text, font, hue);
                    break;
                case MessageTypes.UIld:
                    m_World.Interaction.ChatMessage("[UILD] " + text, font, hue);
                    break;
                case MessageTypes.Alliance:
                    m_World.Interaction.ChatMessage("[ALLIANCE] " + text, font, hue);
                    break;
                case MessageTypes.Command:
                    m_World.Interaction.ChatMessage("[COMMAND] " + text, font, hue);
                    break;
                default:
                    Tracer.Warn("Speech packet with unknown msgType parameter received. MsgType={0} Msg={1}", msgType, text);
                    break;
            }
        }

        // ======================================================================
        // Gump & Menu handling
        // ======================================================================

        private void ReceiveResurrectionMenu(IRecvPacket packet)
        {
            ResurrectionMenuPacket p = (ResurrectionMenuPacket)packet;
            switch (p.ResurrectionAction)
            {
                case 0x00: // Notify client of their death.
                    break;
                case 0x01: // Client has chosen to resurrect with penalties.
                    break;
                case 0x02: // Client has chosen to play as ghost.
                    break;
            }
        }

        private void ReceivePopupMessage(IRecvPacket packet)
        {
            PopupMessagePacket p = (PopupMessagePacket)packet;
            MsgBoxGump.Show(p.Message, MsgBoxTypes.OkOnly);
        }

        private void ReceiveOpenBuyWindow(IRecvPacket packet)
        {
            VendorBuyListPacket p = (VendorBuyListPacket)packet;
            Item entity = WorldModel.Entities.GetObject<Item>(p.VendorPackSerial, false);
            if (entity == null)
                return;
            m_UserInterface.RemoveControl<VendorBuyGump>();
            m_UserInterface.AddControl(new VendorBuyGump(entity, p), 200, 200);
        }

        private void ReceiveSellList(IRecvPacket packet)
        {
            VendorSellListPacket p = (VendorSellListPacket)packet;
            m_UserInterface.RemoveControl<VendorSellGump>();
            m_UserInterface.AddControl(new VendorSellGump(p), 200, 200);
        }

        private void ReceiveOpenPaperdoll(IRecvPacket packet)
        {
            OpenPaperdollPacket p = packet as OpenPaperdollPacket;
            if (m_UserInterface.GetControl<JournalGump>(p.Serial) == null)
                m_UserInterface.AddControl(new PaperDollGump(p.Serial), 400, 100);
        }

        private void ReceiveCompressedGump(IRecvPacket packet)
        {
            CompressedGumpPacket p = (CompressedGumpPacket)packet;
            if (p.HasData)
            {
                string[] gumpPieces;
                if (TryParseGumplings(p.GumpData, out gumpPieces))
                {
                    Gump g = (Gump)m_UserInterface.AddControl(new Gump(p.GumpSerial, p.GumpTypeID, gumpPieces, p.TextLines), p.X, p.Y);
                    g.IsMoveable = true;
                }
            }
        }

        private void ReceiveDisplayGumpFast(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void ReceiveDisplayMenu(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private bool TryParseGumplings(string gumpData, out string[] pieces)
        {
            List<string> i = new List<string>(); ;
            int dataIndex = 0;
            while (dataIndex < gumpData.Length)
            {
                if (gumpData.Substring(dataIndex) == "\0")
                {
                    break;
                }
                else
                {
                    int begin = gumpData.IndexOf("{", dataIndex);
                    int end = gumpData.IndexOf("}", dataIndex + 1);
                    if ((begin != -1) && (end != -1))
                    {
                        string sub = gumpData.Substring(begin + 1, end - begin - 1).Trim();
                        i.Add(sub);
                        dataIndex = end;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            pieces = i.ToArray();
            return (pieces.Length > 0);
        }

        //
        // Other packets
        // 

        private void ReceiveMessageLocalizedAffix(IRecvPacket packet)
        {
            MessageLocalizedAffixPacket p = (MessageLocalizedAffixPacket)packet;

            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            string localizedString = string.Format(p.Flag_IsPrefix ? "{1}{0}" : "{0}{1}",
                constructCliLoc(provider.GetString(p.CliLocNumber), p.Arguements), p.Affix);
            ReceiveTextMessage(p.MessageType, localizedString, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private void ReceiveNewSubserver(IRecvPacket packet)
        {
            SubServerPacket p = (SubServerPacket)packet;
            announce_UnhandledPacket(packet);
        }

        private void ReceiveObjectHelpResponse(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void ReceiveObjectPropertyList(IRecvPacket packet)
        {
            ObjectPropertyListPacket p = (ObjectPropertyListPacket)packet;

            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();

            AEntity entity = WorldModel.Entities.GetObject<AEntity>(p.Serial, false);
            if (entity == null)
                return; // received property list for entity that does not exist.

            entity.PropertyList.Hash = p.Hash;
            entity.PropertyList.Clear();

            for (int i = 0; i < p.CliLocs.Count; i++)
            {
                string strCliLoc = provider.GetString(p.CliLocs[i]);
                if (p.Arguements[i] == string.Empty)
                    strCliLoc = constructCliLoc(strCliLoc, capitalize: true);
                else
                    strCliLoc = constructCliLoc(strCliLoc, p.Arguements[i], true);
                if (i == 0)
                    strCliLoc = string.Format("<span color='#ff0'>{0}</span>", strCliLoc);
                entity.PropertyList.AddProperty(strCliLoc);
            }
        }

        private void ReceiveSendCustomHouse(IRecvPacket packet)
        {
            CustomHousePacket p = (CustomHousePacket)packet;
            CustomHousing.UpdateCustomHouseData(p.HouseSerial, p.RevisionHash, p.PlaneCount, p.Planes);

            Multi multi = WorldModel.Entities.GetObject<Multi>(p.HouseSerial, false);
            if (multi.CustomHouseRevision != p.RevisionHash)
            {
                CustomHouse house = CustomHousing.GetCustomHouseData(p.HouseSerial);
                multi.AddCustomHousingTiles(house);
            }
        }

        private void ReceiveSkillsList(IRecvPacket packet)
        {
            foreach (SendSkillsPacket_SkillEntry skill in ((SendSkillsPacket)packet).Skills)
            {
                SkillEntry entry = PlayerState.Skills.SkillEntry(skill.SkillID);
                if (entry != null)
                {
                    entry.Value = skill.SkillValue;
                    entry.ValueUnmodified = skill.SkillValueUnmodified;
                    entry.LockType = skill.SkillLock;
                    entry.Cap = skill.SkillCap;
                }
            }
        }

        private void ReceiveStatusInfo(IRecvPacket packet)
        {
            MobileStatusCompactPacket p = (MobileStatusCompactPacket)packet;

            if (p.StatusType >= 6)
            {
                throw (new Exception("KR Status not handled."));
            }

            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;

            mobile.Name = p.PlayerName;
            mobile.Strength = p.Strength;
            mobile.Dexterity = p.Dexterity;
            mobile.Intelligence = p.Intelligence;
            mobile.Health.Update(p.CurrentHealth, p.MaxHealth);
            mobile.Stamina.Update(p.CurrentStamina, p.MaxStamina);
            mobile.Mana.Update(p.CurrentMana, p.MaxMana);
            mobile.Followers.Update(p.FollowersCurrent, p.FollowersMax);
            mobile.Weight.Update(p.Weight, p.WeightMax);
            mobile.StatCap = p.StatCap;
            mobile.Luck = p.Luck;
            mobile.Gold = p.GoldInInventory;
            mobile.ArmorRating = p.ArmorRating;
            mobile.ResistFire = p.ResistFire;
            mobile.ResistCold = p.ResistCold;
            mobile.ResistPoison = p.ResistPoison;
            mobile.ResistEnergy = p.ResistEnergy;
            mobile.DamageMin = p.DamageMin;
            mobile.DamageMax = p.DamageMax;
            //  other stuff unhandled !!!
        }

        private void ReceiveTime(IRecvPacket packet)
        {
            TimePacket p = (TimePacket)packet;
            m_World.Interaction.ChatMessage(string.Format("The current server time is {0}:{1}:{2}", p.Hour, p.Minute, p.Second));
        }

        private void ReceiveTipNotice(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void ReceiveToolTipRevision(IRecvPacket packet)
        {
            ObjectPropertyListUpdatePacket p = (ObjectPropertyListUpdatePacket)packet;
            AEntity entity = WorldModel.Entities.GetObject<AEntity>(p.Serial, false);
            if (entity == null)
            {
                // received a tool tip revision for an entity.
            }
            else
            {
                if (entity.PropertyList.Hash != p.RevisionHash)
                {
                    m_Network.Send(new QueryPropertiesPacket(p.Serial));
                }
            }
        }

        private void announce_UnhandledPacket(IRecvPacket packet)
        {
            Tracer.Warn(string.Format("Client: Unhandled {0} [ID:{1}]", packet.Name, packet.Id));
        }

        private void announce_UnhandledPacket(IRecvPacket packet, string addendum)
        {
            Tracer.Warn(string.Format("Client: Unhandled {0} [ID:{1}] {2}]", packet.Name, packet.Id, addendum));
        }

        private void ReceiveExtended0x78(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void ReceiveGeneralInfo(IRecvPacket packet)
        {
            // Documented here: http://docs.polserver.com/packets/index.php?Packet=0xBF
            GeneralInfoPacket p = (GeneralInfoPacket)packet;
            switch (p.Subcommand)
            {
                case 0x04: // Close generic gump
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x06: // party system
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x08: // set map
                    m_World.MapIndex = p.MapID;
                    break;
                case 0x14: // return context menu
                    {
                        InputManager input = ServiceRegistry.GetService<InputManager>();
                        m_UserInterface.AddControl(new ContextMenuGump(p.ContextMenu), input.MousePosition.X - 10, input.MousePosition.Y - 20);
                        break;
                    }
                case 0x18: // Enable map-diff (files) / number of maps
                    // as of 6.0.0.0, this only tells us the number of maps.
                    m_World.MapCount = p.MapCount;
                    break;
                case 0x19: // Extended stats
                    if (p.Serial != WorldModel.PlayerSerial)
                        Tracer.Warn("Extended Stats packet (0xBF subcommand 0x19) received for a mobile not our own.");
                    else
                    {
                        PlayerState.StatLocks.StrengthLock = p.StatisticLocks.Strength;
                        PlayerState.StatLocks.DexterityLock = p.StatisticLocks.Dexterity;
                        PlayerState.StatLocks.IntelligenceLock = p.StatisticLocks.Intelligence;
                    }
                    break;
                case 0x1B: // spellbook data
                    SpellbookData spellbook = p.Spellbook;
                    WorldModel.Entities.GetObject<SpellBook>(spellbook.Serial, true).ReceiveSpellData(spellbook.BookType, spellbook.SpellsBitfield);
                    break;
                case 0x1D: // House revision state
                    if (CustomHousing.IsHashCurrent(p.HouseRevisionState.Serial, p.HouseRevisionState.Hash))
                    {
                        Multi multi = WorldModel.Entities.GetObject<Multi>(p.HouseRevisionState.Serial, false);
                        if (multi == null)
                        {
                            // received a house revision for a multi that does not exist.
                        }
                        else
                        {
                            if (multi.CustomHouseRevision != p.HouseRevisionState.Hash)
                            {
                                CustomHouse house = CustomHousing.GetCustomHouseData(p.HouseRevisionState.Serial);
                                multi.AddCustomHousingTiles(house);
                            }
                        }
                    }
                    else
                    {
                        m_Network.Send(new RequestCustomHousePacket(p.HouseRevisionState.Serial));
                    }
                    break;
                case 0x21: // (AOS) Ability icon confirm.
                    // no data, just (bf 00 05 21)
                    // ???
                    break;
                default:
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
            }
        }

        private void ReceiveGlobalQueueCount(IRecvPacket packet)
        {
            GlobalQueuePacket p = (GlobalQueuePacket)packet;
            m_World.Interaction.ChatMessage("System: There are currently " + p.Count + " available calls in the global queue.");
        }

        private void ReceiveInvalidMapEnable(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void ReceiveOpenWebBrowser(IRecvPacket packet)
        {
            OpenWebBrowserPacket p = (OpenWebBrowserPacket)packet;
            Process.Start("iexplore.exe", p.WebsiteUrl);
        }

        private void ReceiveOverallLightLevel(IRecvPacket packet)
        {
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F

            OverallLightLevelPacket p = (OverallLightLevelPacket)packet;

            ((WorldView)m_World.GetView()).Isometric.Lighting.OverallLightning = p.LightLevel;
        }

        private void ReceivePersonalLightLevel(IRecvPacket packet)
        {
            // int iCreatureID = reader.ReadInt();
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F

            PersonalLightLevelPacket p = (PersonalLightLevelPacket)packet;

            ((WorldView)m_World.GetView()).Isometric.Lighting.PersonalLightning = p.LightLevel;
        }

        private void ReceivePlayMusic(IRecvPacket packet)
        {
            PlayMusicPacket p = (PlayMusicPacket)packet;
            AudioService service = ServiceRegistry.GetService<AudioService>();
            service.PlayMusic(p.MusicID);
        }

        private void ReceivePlaySoundEffect(IRecvPacket packet)
        {
            PlaySoundEffectPacket p = (PlaySoundEffectPacket)packet;
            AudioService service = ServiceRegistry.GetService<AudioService>();
            service.PlaySound(p.SoundModel);
        }

        private void ReceiveQuestArrow(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void ReceiveRequestNameResponse(IRecvPacket packet)
        {
            RequestNameResponsePacket p = (RequestNameResponsePacket)packet;
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;
            mobile.Name = p.MobileName;
        }

        /// <summary>
        /// Handle a season change packet.
        /// </summary>
        /// <param name="packet">Should be of type SeasonChangePacket.</param>
        private void ReceiveSeasonalInformation(IRecvPacket packet)
        {
            SeasonChangePacket p = (SeasonChangePacket)packet;
            if (p.SeasonChanged)
            {
                m_World.Map.Season = p.Season;
            }
        }

        private void ReceiveSetWeather(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }
    }
}
