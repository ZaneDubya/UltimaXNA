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
using UltimaXNA.Ultima.Network.Server.GeneralInfo;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.Player.Partying;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.WorldGumps;
using UltimaXNA.Ultima.World.Data;
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
        Timer m_KeepAliveTimer;
        INetworkClient m_Network;
        UserInterfaceService m_UserInterface;
        WorldModel m_World;

        public WorldClient(WorldModel world)
        {
            m_World = world;
            m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
        }

        public void Initialize()
        {
            Register<DamagePacket>(0x0B, 0x07, ReceiveDamage);
            Register<StatusInfoPacket>(0x11, -1, ReceiveStatusInfo);
            Register<ObjectInfoPacket>(0x1A, -1, ReceiveWorldItem);
            Register<AsciiMessagePacket>(0x1C, -1, ReceiveAsciiMessage);
            Register<RemoveEntityPacket>(0x1D, 5, ReceiveDeleteObject);
            Register<MobileUpdatePacket>(0x20, 19, ReceiveMobileUpdate);
            Register<MovementRejectPacket>(0x21, 8, ReceiveMoveRej);
            Register<MoveAcknowledgePacket>(0x22, 3, ReceiveMoveAck);
            Register<DragEffectPacket>(0x23, 26, ReceiveDragItem);
            Register<OpenContainerPacket>(0x24, 7, ReceiveContainer);
            Register<AddSingleItemToContainerPacket>(0x25, 
                ClientVersion.HasExtendedAddItemPacket(Settings.UltimaOnline.PatchVersion) ? 21 : 20,
                ReceiveAddSingleItemToContainer);
            Register<LiftRejectionPacket>(0x27, 2, ReceiveRejectMoveItemRequest);
            Register<ResurrectionMenuPacket>(0x2C, 2, ReceiveResurrectionMenu);
            Register<MobileAttributesPacket>(0x2D, 17, ReceiveMobileAttributes);
            Register<WornItemPacket>(0x2E, 15, ReceiveWornItem);
            Register<SwingPacket>(0x2F, 10, ReceiveOnSwing);
            Register<SendSkillsPacket>(0x3A, -1, ReceiveSkillsList);
            Register<ContainerContentPacket>(0x3C, -1, ReceiveAddMultipleItemsToContainer);
            Register<PersonalLightLevelPacket>(0x4E, 6, ReceivePersonalLightLevel);
            Register<OverallLightLevelPacket>(0x4F, 2, ReceiveOverallLightLevel);
            Register<PopupMessagePacket>(0x53, 2, ReceivePopupMessage);
            Register<PlaySoundEffectPacket>(0x54, 12, ReceivePlaySoundEffect);
            Register<TimePacket>(0x5B, 4, ReceiveTime);
            Register<WeatherPacket>(0x65, 4, ReceiveSetWeather);
            Register<BookPagesPacket>(0x66, -1, ReceiveBookPages);
            Register<TargetCursorPacket>(0x6C, 19, ReceiveTargetCursor);
            Register<PlayMusicPacket>(0x6D, 3, ReceivePlayMusic);
            Register<MobileAnimationPacket>(0x6E, 14, ReceiveMobileAnimation);
            Register<GraphicEffectPacket>(0x70, 28, ReceiveGraphicEffect);
            Register<WarModePacket>(0x72, 5, ReceiveWarMode);
            Register<VendorBuyListPacket>(0x74, -1, ReceiveOpenBuyWindow);
            Register<SubServerPacket>(0x76, 16, ReceiveNewSubserver);
            Register<MobileMovingPacket>(0x77, 17, ReceiveMobileMoving);
            Register<MobileIncomingPacket>(0x78, -1, ReceiveMobileIncoming);
            Register<DisplayMenuPacket>(0x7C, -1, ReceiveDisplayMenu);
            Register<OpenPaperdollPacket>(0x88, 66, ReceiveOpenPaperdoll);
            Register<CorpseClothingPacket>(0x89, -1, ReceiveCorpseClothing);
            Register<BookHeaderOldPacket>(0x93, 99, ReceiveBookHeaderOld);
            Register<PlayerMovePacket>(0x97, 2, ReceivePlayerMove);
            Register<RequestNameResponsePacket>(0x98, -1, ReceiveRequestNameResponse);
            Register<TargetCursorMultiPacket>(0x99, 26, ReceiveTargetCursorMulti);
            Register<VendorSellListPacket>(0x9E, -1, ReceiveSellList);
            Register<UpdateHealthPacket>(0xA1, 9, ReceiveUpdateHealth);
            Register<UpdateManaPacket>(0xA2, 9, ReceiveUpdateMana);
            Register<UpdateStaminaPacket>(0xA3, 9, ReceiveUpdateStamina);
            Register<OpenWebBrowserPacket>(0xA5, -1, ReceiveOpenWebBrowser);
            Register<TipNoticePacket>(0xA6, -1, ReceiveTipNotice);
            Register<ChangeCombatantPacket>(0xAA, 5, ReceiveChangeCombatant);
            Register<UnicodeMessagePacket>(0xAE, -1, ReceiveUnicodeMessage);
            Register<DeathAnimationPacket>(0xAF, 13, ReceiveDeathAnimation);
            Register<DisplayGumpFastPacket>(0xB0, -1, ReceiveDisplayGumpFast);
            Register<ObjectHelpResponsePacket>(0xB7, -1, ReceiveObjectHelpResponse);
            Register<SupportedFeaturesPacket>(0xB9, 
                ClientVersion.HasExtendedFeatures(Settings.UltimaOnline.PatchVersion) ? 5 : 3,
                ReceiveEnableFeatures);
            Register<QuestArrowPacket>(0xBA, 6, ReceiveQuestArrow);
            Register<SeasonChangePacket>(0xBC, 3, ReceiveSeasonalInformation);
            Register<GeneralInfoPacket>(0xBF, -1, ReceiveGeneralInfo);
            Register<GraphicEffectHuedPacket>(0xC0, 36, ReceiveHuedEffect);
            Register<MessageLocalizedPacket>(0xC1, -1, ReceiveCLILOCMessage);
            Register<InvalidMapEnablePacket>(0xC6, 1, ReceiveInvalidMapEnable);
            Register<GraphicEffectExtendedPacket>(0xC7, 49, ReceiveOnParticleEffect);
            Register<GlobalQueuePacket>(0xCB, 7, ReceiveGlobalQueueCount);
            Register<MessageLocalizedAffixPacket>(0xCC, -1, ReceiveMessageLocalizedAffix);
            Register<Extended0x78Packet>(0xD3, -1, ReceiveExtended0x78);
            Register<BookHeaderNewPacket>(0xD4, -1, ReceiveBookHeaderNew);
            Register<ObjectPropertyListPacket>(0xD6, -1, ReceiveObjectPropertyList);
            Register<CustomHousePacket>(0xD8, -1, ReceiveSendCustomHouse);
            Register<ObjectPropertyListUpdatePacket>(0xDC, 9, ReceiveToolTipRevision);
            Register<CompressedGumpPacket>(0xDD, -1, ReceiveCompressedGump);
            Register<ProtocolExtensionPacket>(0xF0, -1, ReceiveProtocolExtension);
            /* Deprecated (not used by RunUO) and/or not implmented
             * Left them here incase we need to implement in the future
            network.Register<HealthBarStatusPacket>(0x17, 12, OnHealthBarStatusUpdate);
            network.Register<KickPlayerPacket>(0x26, 5, OnKickPlayer);
            network.Register<DropItemFailedPacket>(0x28, 5, OnDropItemFailed);
            network.Register<PaperdollClothingAddAckPacket>(0x29, 1, OnPaperdollClothingAddAck
            network.Register<RecvPacket>(0x30, 5, OnAttackOk);
            network.Register<BloodPacket>(0x2A, 5, OnBlood);
            network.Register<RecvPacket>(0x33, -1, OnPauseClient);
            network.Register<RecvPacket>(0x90, -1, OnMapMessage);
            network.Register<RecvPacket>(0x9C, -1, OnHelpRequest);
            network.Register<RecvPacket>(0xAB, -1, OnGumpDialog);
            network.Register<RecvPacket>(0xB2, -1, OnChatMessage);
            network.Register<RecvPacket>(0xC4, -1, OnSemivisible);
            network.Register<RecvPacket>(0xD2, -1, OnExtended0x20);
            network.Register<RecvPacket>(0xDB, -1, OnCharacterTransferLog);
            network.Register<RecvPacket>(0xDE, -1, OnUpdateMobileStatus);
            network.Register<RecvPacket>(0xDF, -1, OnBuffDebuff);
            network.Register<RecvPacket>(0xE2, -1, OnMobileStatusAnimationUpdate);
            */
            MobileMovement.SendMoveRequestPacket += InternalOnEntity_SendMoveRequestPacket;
        }

        public void Dispose()
        {
            StopKeepAlivePackets();
            m_Network.Unregister(this);
            MobileMovement.SendMoveRequestPacket -= InternalOnEntity_SendMoveRequestPacket;
        }

        public void Register<T>(int id, int length, Action<T> onReceive) where T : IRecvPacket
        {
            m_Network.Register(this, id, length, onReceive);
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

        void StopKeepAlivePackets()
        {
            if (m_KeepAliveTimer != null)
                m_KeepAliveTimer.Dispose();
        }

        void SendKeepAlivePacket()
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
            if (Settings.UltimaOnline.PatchVersion.Length != 4)
            {
                Tracer.Warn("Cannot send seed packet: Version array is incorrectly sized.");
            }
            else
            {
                m_Network.Send(new ClientVersionPacket(Settings.UltimaOnline.PatchVersion));
            }
        }

        public void GetMySkills()
        {
            m_Network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.Skills, WorldModel.PlayerSerial));
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
            m_Network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, WorldModel.PlayerSerial));
        }

        void ReceiveTargetCursor(TargetCursorPacket p)
        {
            m_World.Cursor.SetTargeting((WorldCursor.TargetType)p.CommandType, p.CursorID);
        }

        void ReceiveTargetCursorMulti(TargetCursorMultiPacket p)
        {
            m_World.Cursor.SetTargetingMulti(p.DeedSerial, p.MultiModel);
        }

        void InternalOnEntity_SendMoveRequestPacket(MoveRequestPacket p)
        {
            m_Network.Send(p);
        }

        // ============================================================================================================
        // Effect handling
        // ============================================================================================================

        void ReceiveGraphicEffect(GraphicEffectPacket p)
        {
            WorldModel.Effects.Add(p);
        }

        void ReceiveHuedEffect(GraphicEffectHuedPacket p)
        {
            WorldModel.Effects.Add(p);
        }

        void ReceiveOnParticleEffect(GraphicEffectExtendedPacket p)
        {
            WorldModel.Effects.Add(p);
        }

        // ============================================================================================================
        // Entity handling
        // ============================================================================================================

        void ReceiveAddMultipleItemsToContainer(ContainerContentPacket p)
        {
            if (p.Items.Length == 0)
                return;
            // special handling for spellbook contents
            if (p.AllItemsInSameContainer)
            {
                Container container = WorldModel.Entities.GetObject<Container>(p.Items[0].ContainerSerial, true);
                if (SpellbookData.GetSpellBookTypeFromItemID(container.ItemID) != SpellBookTypes.Unknown)
                {
                    SpellbookData data = new SpellbookData(container, p);
                    (container as SpellBook).ReceiveSpellData(data.BookType, data.SpellsBitfield);
                }
            }
            foreach (ItemInContainer pItem in p.Items)
            {
                Item item = CreateItem(pItem.Serial, pItem.ItemID, pItem.Hue, pItem.Amount);
                item.InContainerPosition = new Point(pItem.X, pItem.Y);
                PlaceItemInContainer(item, pItem.ContainerSerial);
            }
        }

        void ReceiveAddSingleItemToContainer(AddSingleItemToContainerPacket p)
        {
            Item item = CreateItem(p.Serial, p.ItemId, p.Hue, p.Amount);
            item.InContainerPosition = new Point(p.X, p.Y);
            PlaceItemInContainer(item, p.ContainerSerial);
        }

        void PlaceItemInContainer(Item item, Serial cSerial)
        {
            // This code is necessary sanity checking: It may be possible that the server will ask us to add an item to
            // a mobile, which this codebase does not currently handle.
            AEntity cGeneric = WorldModel.Entities.GetObject<AEntity>(cSerial, false);
            if (cGeneric == null)
            {
                cGeneric = WorldModel.Entities.GetObject<Container>(cSerial, true);
            }
            if (cGeneric is Container)
            {
                (cGeneric as Container).AddItem(item);
            }
            else
            {
                Tracer.Warn($"Illegal PlaceItemInContainer({item}, {cSerial}): container is {cGeneric.GetType()}.");
            }
        }

        Item CreateItem(Serial serial, int itemID, int nHue, int amount)
        {
            Item item;
            if (itemID == 0x2006)
            {
                // special case for corpses.
                item = WorldModel.Entities.GetObject<Corpse>((int)serial, true);
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
                {
                    // special case for books
                    if (Books.IsBookItem((ushort)itemID))
                    {
                        item = WorldModel.Entities.GetObject<BaseBook>(serial, true);
                    }
                    else
                    {
                        item = WorldModel.Entities.GetObject<Item>(serial, true);
                    }
                }
            }
            if (item == null)
            {
                return null;
            }
            item.Amount = amount;
            item.ItemID = itemID;
            item.Hue = nHue;
            return item;
        }

        void ReceiveContainer(OpenContainerPacket p)
        {
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
                    m_World.Interaction.ChatMessage("Client: Object {item.Serial} has no support for a container object!");
                }
                else
                {
                    m_World.Interaction.OpenContainerGump(item);
                }
            }
        }

        void ReceiveWorldItem(ObjectInfoPacket p)
        {
            // Now create the GameObject.
            // If the iItemID < 0x4000, this is a regular game object.
            // If the iItemID >= 0x4000, then this is a multiobject.
            if (p.ItemID <= 0x4000)
            {
                Item item = CreateItem(p.Serial, p.ItemID, p.Hue, p.Amount);
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

        void ReceiveWornItem(WornItemPacket p)
        {
            Item item = CreateItem(p.Serial, p.ItemId, p.Hue, 0);
            WorldModel.Entities.AddWornItem(item, p.Layer, p.ParentSerial);
            if (item.PropertyList.Hash == 0)
            {
                m_Network.Send(new QueryPropertiesPacket(item.Serial));
            }
        }

        void ReceiveDeleteObject(RemoveEntityPacket p)
        {
            WorldModel.Entities.RemoveEntity(p.Serial);
        }

        void ReceiveMobileIncoming(MobileIncomingPacket p)
        {
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, true);
            mobile.Body = p.BodyID;
            mobile.Hue = p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            mobile.Flags = p.Flags;
            mobile.Notoriety = p.Notoriety;
            mobile.Notoriety = p.Notoriety;
            for (int i = 0; i < p.Equipment.Length; i++)
            {
                Item item = CreateItem(p.Equipment[i].Serial, p.Equipment[i].GumpId, p.Equipment[i].Hue, 1);
                mobile.WearItem(item, p.Equipment[i].Layer);
                if (item.PropertyList.Hash == 0)
                {
                    m_Network.Send(new QueryPropertiesPacket(item.Serial));
                }
            }
            if (mobile.Name == null || mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                m_Network.Send(new RequestNamePacket(p.Serial));
            }
            m_Network.Send(new SingleClickPacket(p.Serial)); // look at the object so we receive its stats.
        }

        void ReceiveDeathAnimation(DeathAnimationPacket p)
        {
            Mobile m = WorldModel.Entities.GetObject<Mobile>(p.PlayerSerial, false);
            Corpse c = WorldModel.Entities.GetObject<Corpse>(p.CorpseSerial, false);
            if (m == null)
                Tracer.Warn("DeathAnimation received for mobile which does not exist.");
            else if (c == null)
                Tracer.Warn("DeathAnimation received for corpse which does not exist.");
            else
            {
                c.Facing = m.DrawFacing;
                c.MobileSerial = p.PlayerSerial;
                c.PlayDeathAnimation();
            }
        }

        void ReceiveDragItem(DragEffectPacket p)
        {
            // This is sent by the server to display an item being dragged from one place to another.
            // Note that this does not actually move the item, it just displays an animation.

            // bool iSourceGround = false;
            // bool iDestGround = false;
            if (p.Source == Serial.World)
            {
                // iSourceGround = true;
            }

            if (p.Destination == Serial.World)
            {
                // iDestGround = true;
            }
            AnnounceUnhandledPacket(p);
        }

        void ReceiveMobileAttributes(MobileAttributesPacket p)
        {
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

        void ReceiveMobileAnimation(MobileAnimationPacket p)
        {
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;

            mobile.Animate(p.Action, p.FrameCount, p.RepeatCount, p.Reverse, p.Repeat, p.Delay);
        }

        void ReceiveMobileMoving(MobileMovingPacket p)
        {
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

        void ReceiveMobileUpdate(MobileUpdatePacket p)
        {
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, true);
            if (mobile == null)
                return;

            mobile.Body = p.BodyID;
            mobile.Flags = p.Flags;
            mobile.Hue = p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);

            if (mobile.Name == null || mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                m_Network.Send(new RequestNamePacket(p.Serial));
            }
        }

        void ReceiveMoveAck(MoveAcknowledgePacket p)
        {
            Mobile player = WorldModel.Entities.GetPlayerEntity();
            player.PlayerMobile_MoveEventAck(p.Sequence);
            player.Notoriety = p.Notoriety;
        }

        void ReceiveMoveRej(MovementRejectPacket p)
        {
            Mobile player = WorldModel.Entities.GetPlayerEntity();
            player.PlayerMobile_MoveEventRej(p.Sequence, p.X, p.Y, p.Z, p.Direction);
        }

        void ReceivePlayerMove(PlayerMovePacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveRejectMoveItemRequest(LiftRejectionPacket p)
        {
            m_World.Interaction.ChatMessage("Could not pick up item: " + p.ErrorMessage);
            m_World.Interaction.ClearHolding();
        }

        // ============================================================================================================
        // Corpse handling
        // ============================================================================================================

        void ReceiveCorpseClothing(CorpseClothingPacket p)
        {
            Corpse corpse = WorldModel.Entities.GetObject<Corpse>(p.CorpseSerial, false);
            if (corpse == null)
                return;
            foreach (CorpseClothingPacket.CorpseItem i in p.Items)
            {
                Item item = WorldModel.Entities.GetObject<Item>(i.Serial, false);
                if (item != null)
                    corpse.Equipment[i.Layer] = item;
            }
        }

        // ============================================================================================================
        // Combat handling
        // ============================================================================================================

        void ReceiveChangeCombatant(ChangeCombatantPacket p)
        {
            if (p.Serial > 0x00000000)
                m_World.Interaction.LastTarget = p.Serial;
        }

        void ReceiveDamage(DamagePacket p)
        {
            Mobile entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;

            m_World.Interaction.ChatMessage(string.Format("{0} takes {1} damage!", entity.Name, p.Damage));
        }

        void ReceiveOnSwing(SwingPacket p)
        {
            // this changes our last target - does this behavior match legacy?
            if (p.Attacker == WorldModel.PlayerSerial)
            {
                m_World.Interaction.LastTarget = p.Defender;
            }
        }

        void ReceiveWarMode(WarModePacket p)
        {
            WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode = p.WarMode;
        }

        void ReceiveUpdateMana(UpdateManaPacket p)
        {
            Mobile entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Mana.Update(p.Current, p.Max);
        }

        void ReceiveUpdateStamina(UpdateStaminaPacket p)
        {
            Mobile entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Stamina.Update(p.Current, p.Max);
        }

        void ReceiveUpdateHealth(UpdateHealthPacket p)
        {
            Mobile entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Health.Update(p.Current, p.Max);
        }

        // ============================================================================================================
        // Chat / messaging handling
        // ============================================================================================================

        void ReceiveCLILOCMessage(MessageLocalizedPacket p)
        {
            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            string strCliLoc = constructCliLoc(provider.GetString(p.CliLocNumber), p.Arguements);
            ReceiveTextMessage(p.MessageType, strCliLoc, p.Font, p.Hue, p.Serial, p.SpeakerName, true);
        }

        void ReceiveAsciiMessage(AsciiMessagePacket p)
        {
            ReceiveTextMessage(p.MsgType, p.Text, p.Font, p.Hue, p.Serial, p.SpeakerName, false);
        }

        void ReceiveUnicodeMessage(UnicodeMessagePacket p)
        {
            ReceiveTextMessage(p.MsgType, p.Text, p.Font, p.Hue, p.Serial, p.SpeakerName, true);
        }

        void ReceiveMessageLocalizedAffix(MessageLocalizedAffixPacket p)
        {
            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            string localizedString = string.Format(p.Flag_IsPrefix ? "{1}{0}" : "{0}{1}",
                constructCliLoc(provider.GetString(p.CliLocNumber), p.Arguements), p.Affix);
            ReceiveTextMessage(p.MessageType, localizedString, p.Font, p.Hue, p.Serial, p.SpeakerName, true);
        }

        string constructCliLoc(string baseCliloc, string arg = null, bool capitalize = false)
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

        void ReceiveTextMessage(MessageTypes msgType, string text, int font, ushort hue, Serial serial, string speakerName, bool asUnicode)
        {
            // PlayerState.Journaling.AddEntry(text, font, hue, speakerName, asUnicode);
            Overhead overhead;
            switch (msgType)
            {
                case MessageTypes.Normal:
                case MessageTypes.SpeechUnknown:
                    if (serial.IsValid)
                    {
                        overhead = WorldModel.Entities.AddOverhead(msgType, serial, text, font, hue, asUnicode);
                        PlayerState.Journaling.AddEntry(text, font, hue, speakerName, asUnicode);
                    }
                    else
                    {
                        m_World.Interaction.ChatMessage(text, font, hue, asUnicode);
                        PlayerState.Journaling.AddEntry(text, font, hue, string.Empty, asUnicode);
                    }
                    break;
                case MessageTypes.System:
                    m_World.Interaction.ChatMessage("[SYSTEM] " + text, font, hue, asUnicode);
                    break;
                case MessageTypes.Emote:
                    if (serial.IsValid)
                    {
                        overhead = WorldModel.Entities.AddOverhead(msgType, serial, string.Format("* {0} *", text), font, hue, asUnicode);
                        PlayerState.Journaling.AddEntry(string.Format("* {0} *", text), font, hue, speakerName, asUnicode);
                    }
                    else
                    {
                        PlayerState.Journaling.AddEntry(text, font, hue, string.Format("* {0} *", text), asUnicode);
                    }
                    break;
                case MessageTypes.Label:
                    m_World.Interaction.CreateLabel(msgType, serial, text, font, hue, asUnicode);
                    PlayerState.Journaling.AddEntry(text, font, hue, "You see", asUnicode);
                    break;
                case MessageTypes.Focus: // on player?
                    m_World.Interaction.ChatMessage("[FOCUS] " + text, font, hue, asUnicode);
                    break;
                case MessageTypes.Whisper:
                    m_World.Interaction.ChatMessage("[WHISPER] " + text, font, hue, asUnicode);
                    break;
                case MessageTypes.Yell:
                    m_World.Interaction.ChatMessage("[YELL] " + text, font, hue, asUnicode);
                    break;
                case MessageTypes.Spell:
                    if (serial.IsValid)
                    {
                        overhead = WorldModel.Entities.AddOverhead(msgType, serial, text, font, hue, asUnicode);
                        PlayerState.Journaling.AddEntry(text, font, hue, speakerName, asUnicode);
                    }
                    break;
                case MessageTypes.Guild:
                    m_World.Interaction.ChatMessage($"[GUILD] {speakerName}: {text}", font, hue, asUnicode);
                    break;
                case MessageTypes.Alliance:
                    m_World.Interaction.ChatMessage($"[ALLIANCE] {speakerName}: {text}", font, hue, asUnicode);
                    break;
                case MessageTypes.Command:
                    m_World.Interaction.ChatMessage("[COMMAND] " + text, font, hue, asUnicode);
                    break;
                case MessageTypes.PartyDisplayOnly:
                    m_World.Interaction.ChatMessage($"[PARTY] {speakerName}: {text}", font, hue, asUnicode);
                    break;
                case MessageTypes.Information:
                    m_World.Interaction.CreateLabel(msgType, serial, text, font, hue, asUnicode);
                    break;
                default:
                    Tracer.Warn("Speech p with unknown msgType parameter received. MsgType={0} Msg={1}", msgType, text);
                    break;
            }
        }

        // ============================================================================================================
        // Gump & Menu handling
        // ============================================================================================================

        void ReceiveResurrectionMenu(ResurrectionMenuPacket p)
        {
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

        void ReceivePopupMessage(PopupMessagePacket p)
        {
            MsgBoxGump.Show(p.Message, MsgBoxTypes.OkOnly);
        }

        void ReceiveOpenBuyWindow(VendorBuyListPacket p)
        {
            Item entity = WorldModel.Entities.GetObject<Item>(p.VendorPackSerial, false);
            if (entity == null)
                return;
            m_UserInterface.RemoveControl<VendorBuyGump>();
            m_UserInterface.AddControl(new VendorBuyGump(entity, p), 200, 200);
        }

        void ReceiveSellList(VendorSellListPacket p)
        {
            m_UserInterface.RemoveControl<VendorSellGump>();
            m_UserInterface.AddControl(new VendorSellGump(p), 200, 200);
        }

        void ReceiveOpenPaperdoll(OpenPaperdollPacket p)
        {
            if (m_UserInterface.GetControl<PaperDollGump>(p.Serial) == null)
                m_UserInterface.AddControl(new PaperDollGump(p.Serial, p.MobileTitle), 400, 100);
        }
        void ReceiveCompressedGump(CompressedGumpPacket p)
        {
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

        void ReceiveDisplayGumpFast(DisplayGumpFastPacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveDisplayMenu(DisplayMenuPacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        bool TryParseGumplings(string gumpData, out string[] pieces)
        {
            List<string> i = new List<string>();
            ;
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

        void ReceiveNewSubserver(SubServerPacket p)
        {
            // this packet does not matter on modern server software that handles an entire shard on one server.
        }

        void ReceiveObjectHelpResponse(ObjectHelpResponsePacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveObjectPropertyList(ObjectPropertyListPacket p)
        {
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
                    strCliLoc = string.Format("<span color='#ff0'>{0}</span>", Utility.CapitalizeFirstCharacter(strCliLoc.Trim()));
                entity.PropertyList.AddProperty(strCliLoc);
            }
        }

        void ReceiveSendCustomHouse(CustomHousePacket p)
        {
            CustomHousing.UpdateCustomHouseData(p.HouseSerial, p.RevisionHash, p.PlaneCount, p.Planes);

            Multi multi = WorldModel.Entities.GetObject<Multi>(p.HouseSerial, false);
            if (multi.CustomHouseRevision != p.RevisionHash)
            {
                CustomHouse house = CustomHousing.GetCustomHouseData(p.HouseSerial);
                multi.AddCustomHousingTiles(house);
            }
        }

        void ReceiveSkillsList(SendSkillsPacket p)
        {
            foreach (SendSkillsPacket_SkillEntry skill in p.Skills)
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

        void ReceiveStatusInfo(StatusInfoPacket p)
        {
            if (p.StatusTypeFlag >= 6)
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
            mobile.Followers.Update(p.Followers, p.MaxFollowers);
            mobile.Weight.Update(p.Weight, p.MaxWeight);
            mobile.StatCap = p.StatCap;
            mobile.Luck = p.Luck;
            mobile.Gold = p.GoldInInventory;
            mobile.ArmorRating = p.ArmorRating;
            mobile.ResistFire = p.ResistFire;
            mobile.ResistCold = p.ResistCold;
            mobile.ResistPoison = p.ResistPoison;
            mobile.ResistEnergy = p.ResistEnergy;
            mobile.DamageMin = p.DmgMin;
            mobile.DamageMax = p.DmgMax;
            mobile.PlayerCanChangeName = p.NameChangeFlag;
        }

        void ReceiveTime(TimePacket p)
        {
            m_World.Interaction.ChatMessage(string.Format("The current server time is {0}:{1}:{2}", p.Hour, p.Minute, p.Second));
        }

        void ReceiveTipNotice(TipNoticePacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveToolTipRevision(ObjectPropertyListUpdatePacket p)
        {
            if (!PlayerState.ClientFeatures.TooltipsEnabled)
                return;
            AEntity entity = WorldModel.Entities.GetObject<AEntity>(p.Serial, false);
            if (entity == null)
            {
                Tracer.Warn($"Received tooltip for entity {p.Serial} before entity received.");
            }
            else
            {
                if (entity.PropertyList.Hash != p.RevisionHash)
                {
                    m_Network.Send(new QueryPropertiesPacket(p.Serial));
                }
            }
        }

        void AnnounceUnhandledPacket(IRecvPacket p)
        {
            Tracer.Warn($"Client: Unhandled {p.GetType().Name} [ID:{p.Id}]");
        }

        void ReceiveExtended0x78(Extended0x78Packet p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveGeneralInfo(GeneralInfoPacket p)
        {
            // Documented here: http://docs.polserver.com/packets/index.php?Packet=0xBF
            switch (p.Subcommand)
            {
                case GeneralInfoPacket.CloseGump:
                    CloseGumpInfo closeGumpInfo = p.Info as CloseGumpInfo;
                    AControl control = m_UserInterface.GetControlByTypeID(closeGumpInfo.GumpTypeID);
                    (control as Gump)?.OnButtonClick(closeGumpInfo.GumpButtonID);
                    break;
                case GeneralInfoPacket.Party:
                    PartyInfo partyInfo = p.Info as PartyInfo;
                    switch (partyInfo.SubsubCommand)
                    {
                        case PartyInfo.CommandPartyList:
                            PlayerState.Partying.ReceivePartyMemberList(partyInfo.Info as PartyMemberListInfo);
                            break;
                        case PartyInfo.CommandRemoveMember:
                            PlayerState.Partying.ReceiveRemovePartyMember(partyInfo.Info as PartyRemoveMemberInfo);
                            break;
                        case PartyInfo.CommandPrivateMessage:
                        case PartyInfo.CommandPublicMessage:
                            PartyMessageInfo msg = partyInfo.Info as PartyMessageInfo;
                            PartyMember member = PlayerState.Partying.GetMember(msg.Source);
                            // note: msx752 identified hue 50 for "targeted to : " and 34 for "Help me.. I'm stunned !!"
                            ushort hue = (ushort)(msg.IsPrivate ? Settings.UserInterface.PartyPrivateMsgColor : Settings.UserInterface.PartyMsgColor);
                            ReceiveTextMessage(MessageTypes.PartyDisplayOnly, msg.Message, 3, hue, 0xFFFFFFF, member.Name, true);
                            break;
                        case PartyInfo.CommandInvitation:
                            PlayerState.Partying.ReceiveInvitation(partyInfo.Info as PartyInvitationInfo);
                            break;
                    }
                    break;
                case GeneralInfoPacket.SetMap:
                    MapIndexInfo mapInfo = p.Info as MapIndexInfo;
                    m_World.MapIndex = mapInfo.MapID;
                    break;
                case GeneralInfoPacket.ContextMenu:
                    ContextMenuInfo menuInfo = p.Info as ContextMenuInfo;
                    InputManager input = ServiceRegistry.GetService<InputManager>();
                    m_UserInterface.AddControl(new ContextMenuGump(menuInfo.Menu), input.MousePosition.X - 10, input.MousePosition.Y - 20);
                    break;
                case GeneralInfoPacket.MapDiff:
                    TileMatrixDataPatch.EnableMapDiffs(p.Info as MapDiffInfo);
                    m_World.Map.ReloadStatics();
                    break;
                case GeneralInfoPacket.ExtendedStats:
                    ExtendedStatsInfo extendedStats = p.Info as ExtendedStatsInfo;
                    if (extendedStats.Serial != WorldModel.PlayerSerial)
                    {
                        Tracer.Warn("Extended Stats packet (0xBF subcommand 0x19) received for a mobile not our own.");
                    }
                    else
                    {
                        PlayerState.StatLocks.StrengthLock = extendedStats.Locks.Strength;
                        PlayerState.StatLocks.DexterityLock = extendedStats.Locks.Dexterity;
                        PlayerState.StatLocks.IntelligenceLock = extendedStats.Locks.Intelligence;
                    }
                    break;
                case GeneralInfoPacket.SpellBookContents:
                    SpellbookData spellbook = (p.Info as SpellBookContentsInfo).Spellbook;
                    WorldModel.Entities.GetObject<SpellBook>(spellbook.Serial, true).ReceiveSpellData(spellbook.BookType, spellbook.SpellsBitfield);
                    break;
                case GeneralInfoPacket.HouseRevision:
                    HouseRevisionInfo houseInfo = p.Info as HouseRevisionInfo;
                    if (CustomHousing.IsHashCurrent(houseInfo.Revision.Serial, houseInfo.Revision.Hash))
                    {
                        Multi multi = WorldModel.Entities.GetObject<Multi>(houseInfo.Revision.Serial, false);
                        if (multi == null)
                        {
                            // received a house revision for a multi that does not exist.
                        }
                        else
                        {
                            if (multi.CustomHouseRevision != houseInfo.Revision.Hash)
                            {
                                CustomHouse house = CustomHousing.GetCustomHouseData(houseInfo.Revision.Serial);
                                multi.AddCustomHousingTiles(house);
                            }
                        }
                    }
                    else
                    {
                        m_Network.Send(new RequestCustomHousePacket(houseInfo.Revision.Serial));
                    }
                    break;
                case GeneralInfoPacket.AOSAbilityIconConfirm: // (AOS) Ability icon confirm.
                    // no data, just (bf 00 05 21)
                    // What do we do with this???
                    break;
            }
        }

        void ReceiveGlobalQueueCount(GlobalQueuePacket p)
        {
            m_World.Interaction.ChatMessage("System: There are currently " + p.Count + " available calls in the global queue.");
        }

        void ReceiveInvalidMapEnable(InvalidMapEnablePacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveOpenWebBrowser(OpenWebBrowserPacket p)
        {
            if (!string.IsNullOrEmpty(p.WebsiteUrl))
            {
                try
                {
                    System.Diagnostics.Process.Start(p.WebsiteUrl);
                }
                catch (System.ComponentModel.Win32Exception noBrowser)
                {
                    if (noBrowser.ErrorCode == -2147467259)
                        UI.MsgBoxGump.Show(noBrowser.Message, UI.MsgBoxTypes.OkOnly);
                }
                catch (System.Exception other)
                {
                    UI.MsgBoxGump.Show(other.Message, UI.MsgBoxTypes.OkOnly);
                }
            }
        }

        void ReceiveOverallLightLevel(OverallLightLevelPacket p)
        {
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F
            ((WorldView)m_World.GetView()).Isometric.Lighting.OverallLightning = p.LightLevel;
        }

        void ReceivePersonalLightLevel(PersonalLightLevelPacket p)
        {
            // int iCreatureID = reader.ReadInt();
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F
            ((WorldView)m_World.GetView()).Isometric.Lighting.PersonalLightning = p.LightLevel;
        }

        void ReceivePlayMusic(PlayMusicPacket p)
        {
            AudioService service = ServiceRegistry.GetService<AudioService>();
            service.PlayMusic(p.MusicID);
        }

        void ReceivePlaySoundEffect(PlaySoundEffectPacket p)
        {
            AudioService service = ServiceRegistry.GetService<AudioService>();
            service.PlaySound(p.SoundModel, spamCheck: true);
        }

        void ReceiveQuestArrow(QuestArrowPacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveRequestNameResponse(RequestNameResponsePacket p)
        {
            Mobile mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;
            mobile.Name = p.MobileName;
        }

        void ReceiveSeasonalInformation(SeasonChangePacket p)
        {
            if (p.SeasonChanged)
            {
                m_World.Map.Season = p.Season;
            }
        }

        void ReceiveSetWeather(WeatherPacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveBookPages(BookPagesPacket p)
        {
            BaseBook book = WorldModel.Entities.GetObject<BaseBook>(p.Serial, false);
            book.Pages = p.Pages;
            m_UserInterface.AddControl(new BookGump(book), 200, 200);
        }

        void ReceiveBookHeaderNew(BookHeaderNewPacket p)
        {
            BaseBook book = WorldModel.Entities.GetObject<BaseBook>(p.Serial, true);
            book.IsEditable = (p.Flag0 == 1 && p.Flag1 == 1);
            book.Title = p.Title;
            book.Author = p.Author;
        }

        void ReceiveBookHeaderOld(BookHeaderOldPacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveEnableFeatures(SupportedFeaturesPacket p) {
            PlayerState.ClientFeatures.SetFlags(p.Flags);
        }

        void ReceiveProtocolExtension(ProtocolExtensionPacket p)
        {
            switch (p.Subcommand)
            {
                case ProtocolExtensionPacket.SubcommandNegotiateFeatures:
                    PlayerState.DisabledFeatures = p.DisabledFeatures;
                    m_Network.Send(new NegotiateFeatureResponsePacket());
                    break;
                default:
                    Tracer.Warn($"Unhandled protocol extension packet (0xF0) with subcommand: 0x{p.Subcommand:x2}");
                    break;
            }
        }
    }
}
