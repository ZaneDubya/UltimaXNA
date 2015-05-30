using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.Entities.Items.Containers;
using UltimaXNA.Ultima.Entities.Mobiles;
using UltimaXNA.Ultima.Entities.Multis;
using UltimaXNA.Ultima.Entities.Effects;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.World.Gumps;
using UltimaXNA.Ultima.Network;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.Network.Server;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Configuration;

namespace UltimaXNA.Ultima.World.Controllers
{
    class WorldClient
    {
        protected WorldModel World
        {
            get;
            private set;
        }

        private Timer m_KeepAliveTimer;
        private INetworkClient m_Network;
        private UserInterfaceService m_UserInterface;

        private List<Tuple<int, TypedPacketReceiveHandler>> m_RegisteredHandlers;

        public WorldClient(WorldModel world)
        {
            World = world;

            m_RegisteredHandlers = new List<Tuple<int, TypedPacketReceiveHandler>>();

            m_Network = UltimaServices.GetService<INetworkClient>();
            m_UserInterface = UltimaServices.GetService<UserInterfaceService>();
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
            m_KeepAliveTimer = new System.Threading.Timer(
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
            m_Network.Send(new GetPlayerStatusPacket(0x05, EngineVars.PlayerSerial));
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
            m_Network.Send(new GetPlayerStatusPacket(0x04, EngineVars.PlayerSerial));
        }

        private void ReceiveTargetCursor(IRecvPacket packet)
        {
            TargetCursorPacket p = (TargetCursorPacket)packet;
            World.Cursor.SetTargeting((WorldCursor.TargetType)p.CommandType, p.CursorID);
        }

        private void ReceiveTargetCursorMulti(IRecvPacket packet)
        {
            TargetCursorMultiPacket p = (TargetCursorMultiPacket)packet;
            World.Cursor.SetTargetingMulti(p.DeedSerial, p.MultiModel);
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
            EffectsManager.Add((GraphicEffectPacket)packet);
        }

        private void ReceiveHuedEffect(IRecvPacket packet)
        {
            EffectsManager.Add((GraphicEffectHuedPacket)packet);
        }

        private void ReceiveOnParticleEffect(IRecvPacket packet)
        {
            EffectsManager.Add((GraphicEffectExtendedPacket)packet);
        }

        // ======================================================================
        // Entity handling
        // ======================================================================

        private void ReceiveAddMultipleItemsToContainer(IRecvPacket packet)
        {
            ContainerContentPacket p = (ContainerContentPacket)packet;
            foreach (ContentItem i in p.Items)
            {
                // Add the item...
                Item item = add_Item(i.Serial, i.ItemID, i.Hue, i.ContainerSerial, i.Amount);
                item.InContainerPosition = new Point(i.X, i.Y);
                // ... and add it the container contents of the container.
                Container container = EntityManager.GetObject<Container>(i.ContainerSerial, true);
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
            Container iContainerObject = EntityManager.GetObject<Container>(p.ContainerSerial, true);
            if (iContainerObject != null)
                iContainerObject.AddItem(item);
            else
            {
                // Special case for game boards... the server will sometimes send us game pieces for a game board before it sends 
                // the game board! Right now, I am discarding these messages, it might be better to queue them up for when the game
                // board actually exists.
                // Let's throw an exception if anything other than a gameboard is ever sent to us.
                // if (iObject.ItemData.Name != "game piece")
                throw new Exception("Item {" + item.ToString() + "} received before containing object received.");
            }
        }

        private Item add_Item(Serial serial, int itemID, int nHue, Serial parentSerial, int amount)
        {
            Item item;
            if (itemID == 0x2006)
            {
                // special case for corpses.
                item = EntityManager.GetObject<Corpse>((int)serial, true);
                ContainerContentPacket.NextContainerContentsIsPre6017 = true;
            }
            else
            {
                if (IO.TileData.ItemData[itemID].IsContainer)
                    item = EntityManager.GetObject<Container>((int)serial, true);
                else
                    item = EntityManager.GetObject<Item>((int)serial, true);
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
                Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, false);
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
                item = EntityManager.GetObject<Container>(p.Serial, false);
                if (item == null)
                {
                    // log error - item does not exist
                }
                if (item is Corpse)
                {
                    World.Interaction.OpenCorpseGump(item);
                }
                else if (item is Container)
                {
                    World.Interaction.OpenContainerGump(item);
                }
                else
                {
                    World.Interaction.ChatMessage(string.Format("Client: Object {0} has no support for a container object!", item.Serial));
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
                Multi multi = EntityManager.GetObject<Multi>(p.Serial, true);
                multi.Position.Set(p.X, p.Y, p.Z);
                multi.MultiID = p.ItemID;
            }
        }

        private void ReceiveWornItem(IRecvPacket packet)
        {
            WornItemPacket p = (WornItemPacket)packet;
            Item item = add_Item(p.Serial, p.ItemId, p.Hue, p.ParentSerial, 0);
            Mobile m = EntityManager.GetObject<Mobile>(p.ParentSerial, false);
            m.WearItem(item, p.Layer);
            if (item.PropertyList.Hash == 0)
                m_Network.Send(new QueryPropertiesPacket(item.Serial));
        }

        private void ReceiveDeleteObject(IRecvPacket packet)
        {
            RemoveEntityPacket p = (RemoveEntityPacket)packet;
            EntityManager.RemoveObject(p.Serial);
        }

        private void ReceiveMobileIncoming(IRecvPacket packet)
        {
            MobileIncomingPacket p = (MobileIncomingPacket)packet;
            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, true);
            mobile.BodyID = p.BodyID;
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

            if (mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                m_Network.Send(new RequestNamePacket(p.Serial));
            }
        }

        private void ReceiveDeathAnimation(IRecvPacket packet)
        {
            DeathAnimationPacket p = (DeathAnimationPacket)packet;
            Mobile m = EntityManager.GetObject<Mobile>(p.PlayerSerial, false);
            Corpse c = EntityManager.GetObject<Corpse>(p.CorpseSerial, false);
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
            announce_UnhandledPacket(packet);
        }

        private void ReceiveMobileAnimation(IRecvPacket packet)
        {
            MobileAnimationPacket p = (MobileAnimationPacket)packet;
            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;

            mobile.Animate(p.Action, p.FrameCount, p.RepeatCount, p.Reverse, p.Repeat, p.Delay);
        }

        private void ReceiveMobileMoving(IRecvPacket packet)
        {
            MobileMovingPacket p = (MobileMovingPacket)packet;
            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, true);
            if (mobile == null)
                return;

            mobile.BodyID = p.BodyID;
            mobile.Flags = p.Flags;
            mobile.Notoriety = p.Notoriety;

            if (mobile is PlayerMobile)
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
            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, true);
            if (mobile == null)
                return;

            mobile.BodyID = p.BodyID;
            mobile.Flags = p.Flags;
            mobile.Hue = (int)p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);

            if (mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                m_Network.Send(new RequestNamePacket(p.Serial));
            }
        }

        private void ReceiveMoveAck(IRecvPacket packet)
        {
            MoveAcknowledgePacket p = (MoveAcknowledgePacket)packet;
            Mobile player = (Mobile)EntityManager.GetPlayerObject();
            player.PlayerMobile_MoveEventAck(p.Sequence);
            player.Notoriety = p.Notoriety;
        }

        private void ReceiveMoveRej(IRecvPacket packet)
        {
            MovementRejectPacket p = (MovementRejectPacket)packet;
            Mobile player = (Mobile)EntityManager.GetPlayerObject();
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
            World.Interaction.ChatMessage("Could not pick up item: " + p.ErrorMessage);
            World.Interaction.ClearHolding();
        }

        // ======================================================================
        // Corpse handling
        // ======================================================================

        private void ReceiveCorpseClothing(IRecvPacket packet)
        {
            CorpseClothingPacket p = (CorpseClothingPacket)packet;
            Corpse corpse = EntityManager.GetObject<Corpse>(p.CorpseSerial, false);
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
                World.Interaction.LastTarget = p.Serial;
        }

        private void ReceiveDamage(IRecvPacket packet)
        {
            DamagePacket p = (DamagePacket)packet;
            Mobile entity = EntityManager.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;

            World.Interaction.ChatMessage(string.Format("{0} takes {1} damage!", entity.Name, p.Damage));
        }

        private void ReceiveOnSwing(IRecvPacket packet)
        {
            SwingPacket p = (SwingPacket)packet;
            // this changes our last target - does this behavior match legacy?
            if (p.Attacker == EngineVars.PlayerSerial)
            {
                World.Interaction.LastTarget = p.Defender;
            }
        }

        private void ReceiveWarMode(IRecvPacket packet)
        {
            WarModePacket p = (WarModePacket)packet;
            ((Mobile)EntityManager.GetPlayerObject()).Flags.IsWarMode = p.WarMode;
        }

        private void ReceiveUpdateMana(IRecvPacket packet)
        {
            UpdateManaPacket p = (UpdateManaPacket)packet;
            Mobile entity = EntityManager.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Mana.Update(p.Current, p.Max);
        }

        private void ReceiveUpdateStamina(IRecvPacket packet)
        {
            UpdateStaminaPacket p = (UpdateStaminaPacket)packet;
            Mobile entity = EntityManager.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Stamina.Update(p.Current, p.Max);
        }

        private void ReceiveUpdateHealth(IRecvPacket packet)
        {
            UpdateHealthPacket p = (UpdateHealthPacket)packet;
            Mobile entity = EntityManager.GetObject<Mobile>(p.Serial, false);
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

            string iCliLoc = constructCliLoc(IO.StringData.Entry(p.CliLocNumber), p.Arguements);
            ReceiveTextMessage(p.MessageType, iCliLoc, p.Hue, p.Font, p.Serial, p.SpeakerName);
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

        private string constructCliLoc(string nBase, string nArgs)
        {
            string[] iArgs = nArgs.Split('\t');
            for (int i = 0; i < iArgs.Length; i++)
            {
                if ((iArgs[i].Length > 0) && (iArgs[i].Substring(0, 1) == "#"))
                {
                    int clilocID = Convert.ToInt32(iArgs[i].Substring(1));
                    iArgs[i] = IO.StringData.Entry(clilocID);
                }
            }

            string iConstruct = nBase;
            for (int i = 0; i < iArgs.Length; i++)
            {
                int iBeginReplace = iConstruct.IndexOf('~', 0);
                int iEndReplace = iConstruct.IndexOf('~', iBeginReplace + 1);
                if ((iBeginReplace != -1) && (iEndReplace != -1))
                {
                    iConstruct = iConstruct.Substring(0, iBeginReplace) + iArgs[i] + iConstruct.Substring(iEndReplace + 1, iConstruct.Length - iEndReplace - 1);
                }
                else
                {
                    iConstruct = nBase;
                }

            }
            return iConstruct;
        }

        private void ReceiveTextMessage(MessageType msgType, string text, int hue, int font, Serial serial, string speakerName)
        {
            PlayerState.Journaling.AddEntry(text);

            Overhead overhead;
            switch (msgType)
            {
                case MessageType.Regular:
                    overhead = EntityManager.AddOverhead(msgType, serial, "<outline>" + text, font, hue);
                    if (overhead != null)
                    {
                        World.Interaction.ChatMessage(speakerName + ": " + text, font, hue);
                    }
                    else
                    {
                        World.Interaction.ChatMessage(text, font, hue);
                    }
                    break;
                case MessageType.System:
                    World.Interaction.ChatMessage("[SYSTEM] " + text, font, hue);
                    break;
                case MessageType.Emote:
                    World.Interaction.ChatMessage("[EMOTE] " + text, font, hue);
                    break;
                case MessageType.Label:
                    World.Interaction.CreateLabel(msgType, serial, text, font, hue);
                    break;
                case MessageType.Focus: // on player?
                    World.Interaction.ChatMessage("[FOCUS] " + text, font, hue);
                    break;
                case MessageType.Whisper:
                    World.Interaction.ChatMessage("[WHISPER] " + text, font, hue);
                    break;
                case MessageType.Yell:
                    World.Interaction.ChatMessage("[YELL] " + text, font, hue);
                    break;
                case MessageType.Spell:
                    World.Interaction.ChatMessage("[SPELL] " + text, font, hue);
                    break;
                case MessageType.UIld:
                    World.Interaction.ChatMessage("[UILD] " + text, font, hue);
                    break;
                case MessageType.Alliance:
                    World.Interaction.ChatMessage("[ALLIANCE] " + text, font, hue);
                    break;
                case MessageType.Command:
                    World.Interaction.ChatMessage("[COMMAND] " + text, font, hue);
                    break;
                default:
                    World.Interaction.ChatMessage("[[ERROR UNKNOWN COMMAND]] " + msgType.ToString());
                    break;
            }
        }


        // ======================================================================
        // Gump & Menu handling
        // ======================================================================

        private void ReceiveResurrectionMenu(IRecvPacket packet)
        {
            // int iAction = reader.ReadByte();
            // 0: Server sent
            // 1: Resurrect
            // 2: Ghost
            // The only use on OSI for this packet is now sending "2C02" for the "You Are Dead" screen upon character death.
            announce_UnhandledPacket(packet);
        }

        private void ReceivePopupMessage(IRecvPacket packet)
        {
            PopupMessagePacket p = (PopupMessagePacket)packet;
            m_UserInterface.MsgBox(p.Message, MsgBoxTypes.OkOnly);
        }

        private void ReceiveOpenBuyWindow(IRecvPacket packet)
        {
            VendorBuyListPacket p = (VendorBuyListPacket)packet;
            Item entity = EntityManager.GetObject<Item>(p.VendorPackSerial, false);
            if (entity == null)
                return;
            // UserInterface.Merchant_Open(iObject, 0);
            // !!!
        }

        private void ReceiveSellList(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void ReceiveOpenPaperdoll(IRecvPacket packet)
        {
            OpenPaperdollPacket p = packet as OpenPaperdollPacket;
            if (m_UserInterface.GetControl<JournalGump>(p.Serial) == null)
                m_UserInterface.AddControl(new PaperDollGump(EntityManager.GetObject<Mobile>(p.Serial, false)), 400, 100);
        }

        private void ReceiveCompressedGump(IRecvPacket packet)
        {
            CompressedGumpPacket p = (CompressedGumpPacket)packet;
            if (p.HasData)
            {
                string[] gumpPieces;
                if (TryParseGumplings(p.GumpData, out gumpPieces))
                {
                    Gump g = (Gump)m_UserInterface.AddControl(new Gump(p.Serial, p.GumpID, gumpPieces, p.TextLines), p.X, p.Y);
                    g.IsMovable = true;
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

            string localizedString = string.Format(p.Flag_IsPrefix ? "{1}{0}" : "{0}{1}",
                constructCliLoc(IO.StringData.Entry(p.CliLocNumber), p.Arguements), p.Affix);
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

            AEntity entity = EntityManager.GetObject<AEntity>(p.Serial, false);
            if (entity == null)
                return; // received property list for entity that does not exist.

            entity.PropertyList.Hash = p.Hash;
            entity.PropertyList.Clear();

            for (int i = 0; i < p.CliLocs.Count; i++)
            {
                string iCliLoc = IO.StringData.Entry(p.CliLocs[i]);
                if (p.Arguements[i] == string.Empty)
                {
                    entity.PropertyList.AddProperty(iCliLoc);
                }
                else
                {
                    entity.PropertyList.AddProperty(constructCliLoc(iCliLoc, p.Arguements[i]));
                }
            }
        }

        private void ReceiveSendCustomHouse(IRecvPacket packet)
        {
            CustomHousePacket p = (CustomHousePacket)packet;
            IO.CustomHousingData.UpdateCustomHouseData(p.HouseSerial, p.RevisionHash, p.PlaneCount, p.Planes);

            Multi multi = EntityManager.GetObject<Multi>(p.HouseSerial, false);
            if (multi.CustomHouseRevision != p.RevisionHash)
            {
                IO.CustomHouse house = IO.CustomHousingData.GetCustomHouseData(p.HouseSerial);
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

            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, false);
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
            World.Interaction.ChatMessage(string.Format("The current server time is {0}:{1}:{2}", p.Hour, p.Minute, p.Second));
        }

        private void ReceiveTipNotice(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void ReceiveToolTipRevision(IRecvPacket packet)
        {
            ObjectPropertyListUpdatePacket p = (ObjectPropertyListUpdatePacket)packet;
            AEntity entity = EntityManager.GetObject<AEntity>(p.Serial, false);
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


        private void parseContextMenu(ContextMenu context)
        {
            if (context.HasContextMenu)
            {
                if (context.CanSell)
                {
                    m_Network.Send(new ContextMenuResponsePacket(context.Serial, (short)context.GetContextEntry("Sell").ResponseCode));
                }
            }
            else
            {
                // no context menu entries are handled. Send a double click.
                m_Network.Send(new DoubleClickPacket(context.Serial));
            }
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
                    World.MapIndex = p.MapID;
                    break;
                case 0x14: // return context menu
                    parseContextMenu(p.ContextMenu);
                    break;
                case 0x18: // Enable map-diff (files) / number of maps
                    // as of 6.0.0.0, this only tells us the number of maps.
                    World.MapCount = p.MapCount;
                    break;
                case 0x19: // Extended stats
                    if (p.Serial != EngineVars.PlayerSerial)
                        Tracer.Warn("Extended Stats packet (0xBF subcommand 0x19) received for a mobile not our own.");
                    else
                    {
                        PlayerState.StatLocks.StrengthLock = p.StatisticLocks.Strength;
                        PlayerState.StatLocks.DexterityLock = p.StatisticLocks.Dexterity;
                        PlayerState.StatLocks.IntelligenceLock = p.StatisticLocks.Intelligence;
                    }
                    break;
                case 0x1D: // House revision state
                    if (IO.CustomHousingData.IsHashCurrent(p.HouseRevisionState.Serial, p.HouseRevisionState.Hash))
                    {
                        Multi multi = EntityManager.GetObject<Multi>(p.HouseRevisionState.Serial, false);
                        if (multi == null)
                        {
                            // received a house revision for a multi that does not exist.
                        }
                        else
                        {
                            if (multi.CustomHouseRevision != p.HouseRevisionState.Hash)
                            {
                                IO.CustomHouse house = IO.CustomHousingData.GetCustomHouseData(p.HouseRevisionState.Serial);
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
            World.Interaction.ChatMessage("System: There are currently " + p.Count + " available calls in the global queue.");
        }

        private void ReceiveInvalidMapEnable(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void ReceiveOpenWebBrowser(IRecvPacket packet)
        {
            OpenWebBrowserPacket p = (OpenWebBrowserPacket)packet;
            System.Diagnostics.Process.Start("iexplore.exe", p.WebsiteUrl);
        }

        private void ReceiveOverallLightLevel(IRecvPacket packet)
        {
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F

            OverallLightLevelPacket p = (OverallLightLevelPacket)packet;

            ((WorldView)World.GetView()).Isometric.OverallLightning = p.LightLevel;
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

            ((WorldView)World.GetView()).Isometric.PersonalLightning = p.LightLevel;
        }



        private void ReceivePlayMusic(IRecvPacket packet)
        {
            PlayMusicPacket p = (PlayMusicPacket)packet;
            // System.Console.WriteLine ( "Play music, id={0}", p.MusicID );
            IO.MusicData.PlayMusic(p.MusicID);
        }

        private void ReceivePlaySoundEffect(IRecvPacket packet)
        {
            PlaySoundEffectPacket p = (PlaySoundEffectPacket)packet;
            IO.SoundData.PlaySound(p.SoundModel);
        }



        private void ReceiveQuestArrow(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }



        private void ReceiveRequestNameResponse(IRecvPacket packet)
        {
            RequestNameResponsePacket p = (RequestNameResponsePacket)packet;
            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, false);
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
                World.Map.Season = p.Season;
            }
        }




        private void ReceiveSetWeather(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }
    }
}
