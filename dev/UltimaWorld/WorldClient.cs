using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.WorldGumps;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaPackets.Server;
using UltimaXNA.UltimaWorld.Controllers;

namespace UltimaXNA.UltimaWorld
{
    class WorldClient
    {
        protected WorldModel World
        {
            get;
            private set;
        }

        private List<Tuple<int, TypedPacketReceiveHandler>> m_RegisteredHandlers;

        public WorldClient(WorldModel world)
        {
            World = world;

            m_RegisteredHandlers = new List<Tuple<int, TypedPacketReceiveHandler>>();
        }

        public void Initialize()
        {
            Register<DamagePacket>(0x0B, "Damage", 0x07, new TypedPacketReceiveHandler(receive_Damage));
            Register<MobileStatusCompactPacket>(0x11, "Mobile Status Compact", -1, new TypedPacketReceiveHandler(receive_StatusInfo));
            Register<WorldItemPacket>(0x1A, "World Item", -1, new TypedPacketReceiveHandler(receive_WorldItem));
            Register<AsciiMessagePacket>(0x1C, "Ascii Meessage", -1, new TypedPacketReceiveHandler(receive_AsciiMessage));
            Register<RemoveEntityPacket>(0x1D, "Remove Entity", 5, new TypedPacketReceiveHandler(receive_DeleteObject));
            Register<MobileUpdatePacket>(0x20, "Mobile Update", 19, new TypedPacketReceiveHandler(receive_MobileUpdate));
            Register<MovementRejectPacket>(0x21, "Movement Rejection", 8, new TypedPacketReceiveHandler(receive_MoveRej));
            Register<MoveAcknowledgePacket>(0x22, "Move Acknowledged", 3, new TypedPacketReceiveHandler(receive_MoveAck));
            Register<DragEffectPacket>(0x23, "Drag Effect", 26, new TypedPacketReceiveHandler(receive_DragItem));
            Register<OpenContainerPacket>(0x24, "Open Container", 7, new TypedPacketReceiveHandler(receive_Container));
            Register<ContainerContentUpdatePacket>(0x25, "Container Content Update", 21, new TypedPacketReceiveHandler(receive_AddSingleItemToContainer));
            Register<LiftRejectionPacket>(0x27, "Lift Rejection", 2, new TypedPacketReceiveHandler(receive_RejectMoveItemRequest));
            Register<ResurrectionMenuPacket>(0x2C, "Resurect menu", 2, new TypedPacketReceiveHandler(receive_ResurrectionMenu));
            Register<MobileAttributesPacket>(0x2D, "Mob Attributes", 17, new TypedPacketReceiveHandler(receive_MobileAttributes));
            Register<WornItemPacket>(0x2E, "Worn Item", 15, new TypedPacketReceiveHandler(receive_WornItem));
            Register<SwingPacket>(0x2F, "Swing", 10, new TypedPacketReceiveHandler(receive_OnSwing));
            Register<SendSkillsPacket>(0x3A, "Skills list", -1, new TypedPacketReceiveHandler(receive_SkillsList));
            Register<ContainerContentPacket>(0x3C, "Container Content", -1, new TypedPacketReceiveHandler(receive_AddMultipleItemsToContainer));
            Register<PersonalLightLevelPacket>(0x4E, "Personal Light Level", 6, new TypedPacketReceiveHandler(receive_PersonalLightLevel));
            Register<OverallLightLevelPacket>(0x4F, "Overall Light Level", 2, new TypedPacketReceiveHandler(receive_OverallLightLevel));
            Register<PopupMessagePacket>(0x53, "Popup Message", 2, new TypedPacketReceiveHandler(receive_PopupMessage));
            Register<PlaySoundEffectPacket>(0x54, "Play Sound Effect", 12, new TypedPacketReceiveHandler(receive_PlaySoundEffect));
            Register<TimePacket>(0x5B, "Time", 4, new TypedPacketReceiveHandler(receive_Time));
            Register<WeatherPacket>(0x65, "Set Weather", 4, new TypedPacketReceiveHandler(receive_SetWeather));
            Register<TargetCursorPacket>(0x6C, "TargetCursor", 19, new TypedPacketReceiveHandler(receive_TargetCursor));
            Register<PlayMusicPacket>(0x6D, "Play Music", 3, new TypedPacketReceiveHandler(receive_PlayMusic));
            Register<MobileAnimationPacket>(0x6E, "Character Animation", 14, new TypedPacketReceiveHandler(receive_MobileAnimation));
            Register<GraphicEffectPacket>(0x70, "Graphical Effect 1", 28, new TypedPacketReceiveHandler(receive_GraphicEffect));
            Register<WarModePacket>(0x72, "War Mode", 5, new TypedPacketReceiveHandler(receive_WarMode));
            Register<VendorBuyListPacket>(0x74, "Vendor Buy List", -1, new TypedPacketReceiveHandler(receive_OpenBuyWindow));
            Register<SubServerPacket>(0x76, "New Subserver", 16, new TypedPacketReceiveHandler(receive_NewSubserver));
            Register<MobileMovingPacket>(0x77, "Mobile Moving", 17, new TypedPacketReceiveHandler(receive_MobileMoving));
            Register<MobileIncomingPacket>(0x78, "Mobile Incoming", -1, new TypedPacketReceiveHandler(receive_MobileIncoming));
            Register<DisplayMenuPacket>(0x7C, "Display Menu", -1, new TypedPacketReceiveHandler(receive_DisplayMenu));
            Register<OpenPaperdollPacket>(0x88, "Open Paperdoll", 66, new TypedPacketReceiveHandler(receive_OpenPaperdoll));
            Register<CorpseClothingPacket>(0x89, "Corpse Clothing", -1, new TypedPacketReceiveHandler(receive_CorpseClothing));
            Register<PlayerMovePacket>(0x97, "Player Move", 2, new TypedPacketReceiveHandler(receive_PlayerMove));
            Register<RequestNameResponsePacket>(0x98, "Request Name Response", -1, new TypedPacketReceiveHandler(receive_RequestNameResponse));
            Register<TargetCursorMultiPacket>(0x99, "Target Cursor Multi Object", 26, new TypedPacketReceiveHandler(receive_TargetCursorMulti));
            Register<VendorSellListPacket>(0x9E, "Vendor Sell List", -1, new TypedPacketReceiveHandler(receive_SellList));
            Register<UpdateHealthPacket>(0xA1, "Update Current Health", 9, new TypedPacketReceiveHandler(receive_UpdateHealth));
            Register<UpdateManaPacket>(0xA2, "Update Current Mana", 9, new TypedPacketReceiveHandler(receive_UpdateMana));
            Register<UpdateStaminaPacket>(0xA3, "Update Current Stamina", 9, new TypedPacketReceiveHandler(receive_UpdateStamina));
            Register<OpenWebBrowserPacket>(0xA5, "Open Web Browser", -1, new TypedPacketReceiveHandler(receive_OpenWebBrowser));
            Register<TipNoticePacket>(0xA6, "Tip/Notice Window", -1, new TypedPacketReceiveHandler(receive_TipNotice));
            Register<ChangeCombatantPacket>(0xAA, "Change Combatant", 5, new TypedPacketReceiveHandler(receive_ChangeCombatant));
            Register<UnicodeMessagePacket>(0xAE, "Unicode Message", -1, new TypedPacketReceiveHandler(receive_UnicodeMessage));
            Register<DeathAnimationPacket>(0xAF, "Death Animation", 13, new TypedPacketReceiveHandler(receive_DeathAnimation));
            Register<DisplayGumpFastPacket>(0xB0, "Display Gump Fast", -1, new TypedPacketReceiveHandler(receive_DisplayGumpFast));
            Register<ObjectHelpResponsePacket>(0xB7, "Object Help Response ", -1, new TypedPacketReceiveHandler(receive_ObjectHelpResponse));
            Register<QuestArrowPacket>(0xBA, "Quest Arrow", 6, new TypedPacketReceiveHandler(receive_QuestArrow));
            Register<SeasonChangePacket>(0xBC, "Seasonal Change", 3, new TypedPacketReceiveHandler(receive_SeasonalInformation));
            Register<GeneralInfoPacket>(0xBF, "General Information", -1, new TypedPacketReceiveHandler(receive_GeneralInfo));
            Register<GraphicEffectHuedPacket>(0xC0, "Hued Effect", 36, new TypedPacketReceiveHandler(receive_HuedEffect));
            Register<MessageLocalizedPacket>(0xC1, "Message Localized", -1, new TypedPacketReceiveHandler(receive_CLILOCMessage));
            Register<InvalidMapEnablePacket>(0xC6, "Invalid Map Enable", 1, new TypedPacketReceiveHandler(receive_InvalidMapEnable));
            Register<GraphicEffectExtendedPacket>(0xC7, "Particle Effect", 49, new TypedPacketReceiveHandler(receive_OnParticleEffect));
            Register<GlobalQueuePacket>(0xCB, "Global Queue Count", 7, new TypedPacketReceiveHandler(receive_GlobalQueueCount));
            Register<MessageLocalizedAffixPacket>(0xCC, "Message Localized Affix ", -1, new TypedPacketReceiveHandler(receive_MessageLocalizedAffix));
            Register<Extended0x78Packet>(0xD3, "Extended 0x78", -1, new TypedPacketReceiveHandler(receive_Extended0x78));
            Register<ObjectPropertyListPacket>(0xD6, "Mega Cliloc", -1, new TypedPacketReceiveHandler(receive_ObjectPropertyList));
            Register<CustomHousePacket>(0xD8, "Send Custom House", -1, new TypedPacketReceiveHandler(receive_SendCustomHouse));
            Register<ObjectPropertyListUpdatePacket>(0xDC, "SE Introduced Revision", 9, new TypedPacketReceiveHandler(receive_ToolTipRevision));
            Register<CompressedGumpPacket>(0xDD, "Compressed Gump", -1, new TypedPacketReceiveHandler(receive_CompressedGump));

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








            UltimaEntities.MobileMovement.SendMoveRequestPacket += InternalOnEntity_SendMoveRequestPacket;
        }

        public void Dispose()
        {
            for (int i = 0; i < m_RegisteredHandlers.Count; i++)
                World.Engine.Client.Unregister(m_RegisteredHandlers[i].Item1, m_RegisteredHandlers[i].Item2);
            m_RegisteredHandlers.Clear();
            m_RegisteredHandlers = null;

            UltimaEntities.MobileMovement.SendMoveRequestPacket -= InternalOnEntity_SendMoveRequestPacket;
        }

        public void Register<T>(int id, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket
        {
            m_RegisteredHandlers.Add(new Tuple<int, TypedPacketReceiveHandler>(id, onReceive));
            World.Engine.Client.Register<T>(id, name, length, onReceive);
        }

        public void SendWorldLoginPackets()
        {
            GetMySkills();
            World.Engine.Client.SendClientVersion();
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

        public void GetMySkills()
        {
            World.Engine.Client.Send(new GetPlayerStatusPacket(0x05, UltimaVars.EngineVars.PlayerSerial));
        }

        public void SendClientScreenSize()
        {
            World.Engine.Client.Send(new ReportClientScreenSizePacket(800, 600));
        }

        public void SendClientLocalization()
        {
            World.Engine.Client.Send(new ReportClientLocalizationPacket("ENU"));
        }

        public void GetMyBasicStatus()
        {
            World.Engine.Client.Send(new GetPlayerStatusPacket(0x04, UltimaVars.EngineVars.PlayerSerial));
        }

        private void receive_TargetCursor(IRecvPacket packet)
        {
            TargetCursorPacket p = (TargetCursorPacket)packet;
            World.Cursor.SetTargeting((WorldCursor.TargetType)p.CommandType, p.CursorID);
        }

        private void receive_TargetCursorMulti(IRecvPacket packet)
        {
            TargetCursorMultiPacket p = (TargetCursorMultiPacket)packet;
            World.Cursor.SetTargetingMulti(p.DeedSerial, p.MultiModel);
        }

        private void InternalOnEntity_SendMoveRequestPacket(MoveRequestPacket packet)
        {
            World.Engine.Client.Send(packet);
        }

        // ======================================================================
        // Effect handling
        // ======================================================================

        private void receive_GraphicEffect(IRecvPacket packet)
        {
            EffectsManager.Add((GraphicEffectPacket)packet);
        }

        private void receive_HuedEffect(IRecvPacket packet)
        {
            EffectsManager.Add((GraphicEffectHuedPacket)packet);
        }

        private void receive_OnParticleEffect(IRecvPacket packet)
        {
            EffectsManager.Add((GraphicEffectExtendedPacket)packet);
        }

        // ======================================================================
        // Entity handling
        // ======================================================================

        private void receive_AddMultipleItemsToContainer(IRecvPacket packet)
        {
            ContainerContentPacket p = (ContainerContentPacket)packet;
            foreach (ContentItem i in p.Items)
            {
                // Add the item...
                Item item = add_Item(i.Serial, i.ItemID, i.Hue, i.ContainerSerial, i.Amount);
                item.InContainerPosition = new Point(i.X, i.Y);
                // ... and add it the container contents of the container.
                Container c = EntityManager.GetObject<Container>(i.ContainerSerial, true);
                c.AddItem(item);
            }
        }

        private void receive_AddSingleItemToContainer(IRecvPacket packet)
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
                if (UltimaData.TileData.ItemData[itemID].IsContainer)
                    item = EntityManager.GetObject<Container>((int)serial, true);
                else
                    item = EntityManager.GetObject<Item>((int)serial, true);
            }
            item.Amount = amount;
            item.ItemID = itemID;
            item.Hue = nHue;
            return item;
        }

        private void receive_Container(IRecvPacket packet)
        {
            OpenContainerPacket p = (OpenContainerPacket)packet;

            Container item;
            // Special case for 0x30, which tells us to open a buy from vendor window.
            if (p.GumpId == 0x30)
            {
                Mobile m = EntityManager.GetObject<Mobile>(p.Serial, false);
                item = m.VendorShopContents;
            }
            else
            {
                item = EntityManager.GetObject<Container>(p.Serial, false);
                if (item is Corpse)
                {
                    WorldInteraction.OpenCorpseGump(item);
                }
                else if (item is Container)
                {
                    WorldInteraction.OpenContainerGump(item);
                }
                else
                {
                    WorldInteraction.ChatMessage(string.Format("Client: Object {0} has no support for a container object!", item.Serial));
                }
            }
        }

        private void receive_WorldItem(IRecvPacket packet)
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
                // create a multi object. Unhandled !!!
                int multiID = p.ItemID - 0x4000;
                Multi multi = EntityManager.GetObject<Multi>(p.Serial, true);
                multi.Position.Set(p.X, p.Y, p.Z);
                multi.MultiID = p.ItemID;
            }
        }

        private void receive_WornItem(IRecvPacket packet)
        {
            WornItemPacket p = (WornItemPacket)packet;
            Item item = add_Item(p.Serial, p.ItemId, p.Hue, p.ParentSerial, 0);
            Mobile m = EntityManager.GetObject<Mobile>(p.ParentSerial, false);
            m.WearItem(item, p.Layer);
            if (item.PropertyList.Hash == 0)
                World.Engine.Client.Send(new QueryPropertiesPacket(item.Serial));
        }

        private void receive_DeleteObject(IRecvPacket packet)
        {
            RemoveEntityPacket p = (RemoveEntityPacket)packet;
            EntityManager.RemoveObject(p.Serial);
        }

        private void receive_MobileIncoming(IRecvPacket packet)
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
                    World.Engine.Client.Send(new QueryPropertiesPacket(item.Serial));
            }

            if (mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                World.Engine.Client.Send(new RequestNamePacket(p.Serial));
            }
        }

        private void receive_DeathAnimation(IRecvPacket packet)
        {
            DeathAnimationPacket p = (DeathAnimationPacket)packet;
            Mobile m = EntityManager.GetObject<Mobile>(p.PlayerSerial, false);
            Corpse c = EntityManager.GetObject<Corpse>(p.CorpseSerial, false);
            if (m == null)
                Logger.Warn("DeathAnimation received for mobile which does not exist.");
            else if (c == null)
                Logger.Warn("DeathAnimation received for corpse which does not exist.");
            else
            {
                c.Facing = m.Facing;
                c.MobileSerial = p.PlayerSerial;
                c.PlayDeathAnimation();
            }
        }

        private void receive_DragItem(IRecvPacket packet)
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

        private void receive_MobileAttributes(IRecvPacket packet)
        {
            MobileAttributesPacket p = (MobileAttributesPacket)packet;
            announce_UnhandledPacket(packet);
        }

        private void receive_MobileAnimation(IRecvPacket packet)
        {
            MobileAnimationPacket p = (MobileAnimationPacket)packet;

            Mobile m = EntityManager.GetObject<Mobile>(p.Serial, false);
            m.Animate(p.Action, p.FrameCount, p.RepeatCount, p.Reverse, p.Repeat, p.Delay);
        }

        private void receive_MobileMoving(IRecvPacket packet)
        {
            MobileMovingPacket p = (MobileMovingPacket)packet;

            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, true);
            mobile.BodyID = p.BodyID;
            mobile.Flags = p.Flags;
            mobile.Notoriety = p.Notoriety;
            if (mobile.Position.IsNullPosition)
            {
                mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            }
            else
            {
                mobile.Mobile_AddMoveEvent(p.X, p.Y, p.Z, p.Direction);
            }
        }

        private void receive_MobileUpdate(IRecvPacket packet)
        {
            MobileUpdatePacket p = (MobileUpdatePacket)packet;
            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, true);
            mobile.BodyID = p.BodyID;
            mobile.Flags = p.Flags;
            mobile.Hue = (int)p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);

            if (mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                World.Engine.Client.Send(new RequestNamePacket(p.Serial));
            }
        }

        private void receive_MoveAck(IRecvPacket packet)
        {
            MoveAcknowledgePacket p = (MoveAcknowledgePacket)packet;
            Mobile player = (Mobile)EntityManager.GetPlayerObject();
            player.PlayerMobile_MoveEventAck(p.Sequence);
            player.Notoriety = p.Notoriety;
        }

        private void receive_MoveRej(IRecvPacket packet)
        {
            MovementRejectPacket p = (MovementRejectPacket)packet;
            Mobile player = (Mobile)EntityManager.GetPlayerObject();
            player.PlayerMobile_MoveEventRej(p.Sequence, p.X, p.Y, p.Z, p.Direction);
        }

        private void receive_PlayerMove(IRecvPacket packet)
        {
            PlayerMovePacket p = (PlayerMovePacket)packet;
            announce_UnhandledPacket(packet);
        }

        private void receive_RejectMoveItemRequest(IRecvPacket packet)
        {
            LiftRejectionPacket p = (LiftRejectionPacket)packet;
            WorldInteraction.ChatMessage("Could not pick up item: " + p.ErrorMessage);
            WorldInteraction.ClearHolding();
        }

        // ======================================================================
        // Corpse handling
        // ======================================================================

        private void receive_CorpseClothing(IRecvPacket packet)
        {
            CorpseClothingPacket p = (CorpseClothingPacket)packet;
            Corpse e = EntityManager.GetObject<Corpse>(p.CorpseSerial, false);
            e.LoadCorpseClothing(p.Items);
        }

        // ======================================================================
        // Combat handling
        // ======================================================================

        private void receive_ChangeCombatant(IRecvPacket packet)
        {
            ChangeCombatantPacket p = (ChangeCombatantPacket)packet;
            if (p.Serial > 0x00000000)
                UltimaVars.EngineVars.LastTarget = p.Serial;
        }

        private void receive_Damage(IRecvPacket packet)
        {
            DamagePacket p = (DamagePacket)packet;
            Mobile u = EntityManager.GetObject<Mobile>(p.Serial, false);

            WorldInteraction.ChatMessage(string.Format("{0} takes {1} damage!", u.Name, p.Damage));
        }

        private void receive_OnSwing(IRecvPacket packet)
        {
            SwingPacket p = (SwingPacket)packet;
            if (p.Attacker == UltimaVars.EngineVars.PlayerSerial)
            {
                UltimaVars.EngineVars.LastTarget = p.Defender;
            }
        }

        private void receive_WarMode(IRecvPacket packet)
        {
            WarModePacket p = (WarModePacket)packet;
            ((Mobile)EntityManager.GetPlayerObject()).Flags.IsWarMode = p.WarMode;
        }

        private void receive_UpdateMana(IRecvPacket packet)
        {
            UpdateManaPacket p = (UpdateManaPacket)packet;
            Mobile u = EntityManager.GetObject<Mobile>(p.Serial, false);
            u.Mana.Update(p.Current, p.Max);
        }

        private void receive_UpdateStamina(IRecvPacket packet)
        {
            UpdateStaminaPacket p = (UpdateStaminaPacket)packet;
            Mobile u = EntityManager.GetObject<Mobile>(p.Serial, false);
            u.Stamina.Update(p.Current, p.Max);
        }

        private void receive_UpdateHealth(IRecvPacket packet)
        {
            UpdateHealthPacket p = (UpdateHealthPacket)packet;
            Mobile u = EntityManager.GetObject<Mobile>(p.Serial, false);
            u.Health.Update(p.Current, p.Max);
        }

        // ======================================================================
        // Chat / messaging handling
        // ======================================================================

        private void receive_CLILOCMessage(IRecvPacket packet)
        {
            MessageLocalizedPacket p = (MessageLocalizedPacket)packet;

            string iCliLoc = constructCliLoc(UltimaData.StringData.Entry(p.CliLocNumber), p.Arguements);
            receive_TextMessage(p.MessageType, iCliLoc, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private void receive_AsciiMessage(IRecvPacket packet)
        {
            AsciiMessagePacket p = (AsciiMessagePacket)packet;
            receive_TextMessage(p.MsgType, p.Text, p.Hue, p.Font, p.Serial, p.Name1);
        }

        private void receive_UnicodeMessage(IRecvPacket packet)
        {
            UnicodeMessagePacket p = (UnicodeMessagePacket)packet;
            receive_TextMessage(p.MsgType, p.SpokenText, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private string constructCliLoc(string nBase, string nArgs)
        {
            string[] iArgs = nArgs.Split('\t');
            for (int i = 0; i < iArgs.Length; i++)
            {
                if ((iArgs[i].Length > 0) && (iArgs[i].Substring(0, 1) == "#"))
                {
                    int clilocID = Convert.ToInt32(iArgs[i].Substring(1));
                    iArgs[i] = UltimaData.StringData.Entry(clilocID);
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

        private void receive_TextMessage(MessageType msgType, string text, int hue, int font, Serial serial, string speakerName)
        {
            UltimaVars.Journaling.AddEntry(text);

            Overhead overhead;
            switch (msgType)
            {
                case MessageType.Regular:
                    overhead = EntityManager.AddOverhead(msgType, serial, "<outline>" + text, font, hue);
                    if (overhead != null)
                    {
                        WorldInteraction.ChatMessage(speakerName + ": " + text, font, hue);
                    }
                    else
                    {
                        WorldInteraction.ChatMessage(text, font, hue);
                    }
                    break;
                case MessageType.System:
                    WorldInteraction.ChatMessage("[SYSTEM] " + text, font, hue);
                    break;
                case MessageType.Emote:
                    WorldInteraction.ChatMessage("[EMOTE] " + text, font, hue);
                    break;
                case MessageType.Label:
                    WorldInteraction.CreateLabel(msgType, serial, text, font, hue);
                    break;
                case MessageType.Focus: // on player?
                    WorldInteraction.ChatMessage("[FOCUS] " + text, font, hue);
                    break;
                case MessageType.Whisper:
                    WorldInteraction.ChatMessage("[WHISPER] " + text, font, hue);
                    break;
                case MessageType.Yell:
                    WorldInteraction.ChatMessage("[YELL] " + text, font, hue);
                    break;
                case MessageType.Spell:
                    WorldInteraction.ChatMessage("[SPELL] " + text, font, hue);
                    break;
                case MessageType.UIld:
                    WorldInteraction.ChatMessage("[UILD] " + text, font, hue);
                    break;
                case MessageType.Alliance:
                    WorldInteraction.ChatMessage("[ALLIANCE] " + text, font, hue);
                    break;
                case MessageType.Command:
                    WorldInteraction.ChatMessage("[COMMAND] " + text, font, hue);
                    break;
                default:
                    WorldInteraction.ChatMessage("[[ERROR UNKNOWN COMMAND]] " + msgType.ToString());
                    break;
            }
        }


        // ======================================================================
        // Gump & Menu handling
        // ======================================================================

        private void receive_ResurrectionMenu(IRecvPacket packet)
        {
            // int iAction = reader.ReadByte();
            // 0: Server sent
            // 1: Resurrect
            // 2: Ghost
            // The only use on OSI for this packet is now sending "2C02" for the "You Are Dead" screen upon character death.
            announce_UnhandledPacket(packet);
        }

        private void receive_PopupMessage(IRecvPacket packet)
        {
            PopupMessagePacket p = (PopupMessagePacket)packet;
            WorldInteraction.MsgBox(p.Message, MsgBoxTypes.OkOnly);
        }

        private void receive_OpenBuyWindow(IRecvPacket packet)
        {
            VendorBuyListPacket p = (VendorBuyListPacket)packet;
            Item iObject = EntityManager.GetObject<Item>(p.VendorPackSerial, false);
            if (iObject == null)
                return;
            // UserInterface.Merchant_Open(iObject, 0);
            // !!!
        }

        private void receive_SellList(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_OpenPaperdoll(IRecvPacket packet)
        {
            OpenPaperdollPacket opp = packet as OpenPaperdollPacket;
            World.Engine.UserInterface.AddControl(new PaperDollGump(EntityManager.GetObject<Mobile>(opp.Serial, false)), 400, 100, GUIManager.AddGumpType.OnlyAllowOne);
        }

        private void receive_CompressedGump(IRecvPacket packet)
        {
            CompressedGumpPacket p = (CompressedGumpPacket)packet;
            if (p.HasData)
            {
                string[] gumpPieces = interpretGumpPieces(p.GumpData);
                Gump g = (Gump)World.Engine.UserInterface.AddControl(new Gump(p.Serial, p.GumpID, gumpPieces, p.TextLines), p.X, p.Y);
                g.IsMovable = true;
            }
        }

        private void receive_DisplayGumpFast(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_DisplayMenu(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private string[] interpretGumpPieces(string gumpData)
        {
            List<string> i = new List<string>(); ;
            bool isData = true;
            int dataIndex = 0;
            while (isData)
            {
                if (gumpData.Substring(dataIndex) == "\0")
                {
                    isData = false;
                }
                else
                {
                    int begin = gumpData.IndexOf("{ ", dataIndex);
                    int end = gumpData.IndexOf(" }", dataIndex + 1);
                    if ((begin != -1) && (end != -1))
                    {
                        string sub = gumpData.Substring(begin + 2, end - begin - 2);
                        // iConstruct = iConstruct.Substring(0, iBeginReplace) + iArgs[i] + iConstruct.Substring(iEndReplace + 1, iConstruct.Length - iEndReplace - 1);
                        i.Add(sub);
                        dataIndex += end - begin + 2;
                    }
                    else
                    {
                        isData = false;
                    }
                }
            }

            return i.ToArray();
        }

        //
        // Other packets
        // 

        private void receive_MessageLocalizedAffix(IRecvPacket packet)
        {
            MessageLocalizedAffixPacket p = (MessageLocalizedAffixPacket)packet;

            string localizedString = string.Format(p.Flag_IsPrefix ? "{1}{0}" : "{0}{1}",
                constructCliLoc(UltimaData.StringData.Entry(p.CliLocNumber), p.Arguements), p.Affix);
            receive_TextMessage(p.MessageType, localizedString, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }





        private void receive_NewSubserver(IRecvPacket packet)
        {
            SubServerPacket p = (SubServerPacket)packet;
            announce_UnhandledPacket(packet);
        }

        private void receive_ObjectHelpResponse(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_ObjectPropertyList(IRecvPacket packet)
        {
            ObjectPropertyListPacket p = (ObjectPropertyListPacket)packet;

            AEntity iObject = EntityManager.GetObject<AEntity>(p.Serial, false);
            iObject.PropertyList.Hash = p.Hash;
            iObject.PropertyList.Clear();

            for (int i = 0; i < p.CliLocs.Count; i++)
            {
                string iCliLoc = UltimaData.StringData.Entry(p.CliLocs[i]);
                if (p.Arguements[i] == string.Empty)
                {
                    iObject.PropertyList.AddProperty(iCliLoc);
                }
                else
                {
                    iObject.PropertyList.AddProperty(constructCliLoc(iCliLoc, p.Arguements[i]));
                }
            }
        }

        private void receive_SendCustomHouse(IRecvPacket packet)
        {
            CustomHousePacket p = (CustomHousePacket)packet;
            UltimaData.CustomHousingData.UpdateCustomHouseData(p.HouseSerial, p.RevisionHash, p.PlaneCount, p.Planes);

            Multi e = EntityManager.GetObject<Multi>(p.HouseSerial, false);
            if (e.CustomHouseRevision != p.RevisionHash)
            {
                UltimaData.CustomHouse house = UltimaData.CustomHousingData.GetCustomHouseData(p.HouseSerial);
                e.AddCustomHousingTiles(house);
            }
        }

        private void receive_SkillsList(IRecvPacket packet)
        {
            foreach (SendSkillsPacket_SkillEntry skill in ((SendSkillsPacket)packet).Skills)
            {
                UltimaVars.SkillEntry entry = UltimaVars.Skills.SkillEntry(skill.SkillID);
                if (entry != null)
                {
                    entry.Value = skill.SkillValue;
                    entry.ValueUnmodified = skill.SkillValueUnmodified;
                    entry.LockType = skill.SkillLock;
                    entry.Cap = skill.SkillCap;
                }
            }
        }



        private void receive_StatusInfo(IRecvPacket packet)
        {
            MobileStatusCompactPacket p = (MobileStatusCompactPacket)packet;

            if (p.StatusType >= 6)
            {
                throw (new Exception("KR Status not handled."));
            }

            Mobile u = EntityManager.GetObject<Mobile>(p.Serial, false);
            u.Name = p.PlayerName;
            u.Strength = p.Strength;
            u.Dexterity = p.Dexterity;
            u.Intelligence = p.Intelligence;
            u.Health.Update(p.CurrentHealth, p.MaxHealth);
            u.Stamina.Update(p.CurrentStamina, p.MaxStamina);
            u.Mana.Update(p.CurrentMana, p.MaxMana);
            u.Followers.Update(p.FollowersCurrent, p.FollowersMax);
            u.Weight.Update(p.Weight, p.WeightMax);
            u.StatCap = p.StatCap;
            u.Luck = p.Luck;
            u.Gold = p.GoldInInventory;
            u.ArmorRating = p.ArmorRating;
            u.ResistFire = p.ResistFire;
            u.ResistCold = p.ResistCold;
            u.ResistPoison = p.ResistPoison;
            u.ResistEnergy = p.ResistEnergy;
            u.DamageMin = p.DamageMin;
            u.DamageMax = p.DamageMax;
            //  other stuff unhandled !!!
        }



        private void receive_Time(IRecvPacket packet)
        {
            TimePacket p = (TimePacket)packet;
            WorldInteraction.ChatMessage(string.Format("The current server time is {0}:{1}:{2}", p.Hour, p.Minute, p.Second));
        }

        private void receive_TipNotice(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_ToolTipRevision(IRecvPacket packet)
        {
            ObjectPropertyListUpdatePacket p = (ObjectPropertyListUpdatePacket)packet;
            AEntity iObject = EntityManager.GetObject<AEntity>(p.Serial, false);
            if (iObject != null)
            {
                if (iObject.PropertyList.Hash != p.RevisionHash)
                {
                    World.Engine.Client.Send(new QueryPropertiesPacket(p.Serial));
                }
            }
        }











        private void announce_UnhandledPacket(IRecvPacket packet)
        {
            Logger.Warn(string.Format("Client: Unhandled {0} [ID:{1}]", packet.Name, packet.Id));
        }

        private void announce_UnhandledPacket(IRecvPacket packet, string addendum)
        {
            Logger.Warn(string.Format("Client: Unhandled {0} [ID:{1}] {2}]", packet.Name, packet.Id, addendum));
        }


        private void parseContextMenu(ContextMenu context)
        {
            if (context.HasContextMenu)
            {
                if (context.CanSell)
                {
                    World.Engine.Client.Send(new ContextMenuResponsePacket(context.Serial, (short)context.ContextEntry("Sell").ResponseCode));
                }
            }
            else
            {
                // no context menu entries are handled. Send a double click.
                World.Engine.Client.Send(new DoubleClickPacket(context.Serial));
            }
        }

        private void receive_Extended0x78(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_GeneralInfo(IRecvPacket packet)
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
                    UltimaVars.EngineVars.MapCount = p.MapCount;
                    break;
                case 0x19: // Extended stats
                    if (p.Serial != UltimaVars.EngineVars.PlayerSerial)
                        Logger.Warn("Extended Stats packet (0xBF subcommand 0x19) received for a mobile not our own.");
                    else
                    {
                        UltimaVars.StatLocks.StrengthLock = p.StatisticLocks.Strength;
                        UltimaVars.StatLocks.DexterityLock = p.StatisticLocks.Dexterity;
                        UltimaVars.StatLocks.IntelligenceLock = p.StatisticLocks.Intelligence;
                    }
                    break;
                case 0x1D: // House revision state
                    if (UltimaData.CustomHousingData.IsHashCurrent(p.HouseRevisionState.Serial, p.HouseRevisionState.Hash))
                    {
                        Multi e = EntityManager.GetObject<Multi>(p.HouseRevisionState.Serial, false);
                        if (e.CustomHouseRevision != p.HouseRevisionState.Hash)
                        {
                            UltimaData.CustomHouse house = UltimaData.CustomHousingData.GetCustomHouseData(p.HouseRevisionState.Serial);
                            e.AddCustomHousingTiles(house);
                        }
                    }
                    else
                    {
                        World.Engine.Client.Send(new RequestCustomHousePacket(p.HouseRevisionState.Serial));
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

        private void receive_GlobalQueueCount(IRecvPacket packet)
        {
            GlobalQueuePacket p = (GlobalQueuePacket)packet;
            WorldInteraction.ChatMessage("System: There are currently " + p.Count + " available calls in the global queue.");
        }

        private void receive_InvalidMapEnable(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_OpenWebBrowser(IRecvPacket packet)
        {
            OpenWebBrowserPacket p = (OpenWebBrowserPacket)packet;
            System.Diagnostics.Process.Start("iexplore.exe", p.WebsiteUrl);
        }

        private void receive_OverallLightLevel(IRecvPacket packet)
        {
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F

            OverallLightLevelPacket p = (OverallLightLevelPacket)packet;

            ((WorldView)World.GetView()).Isometric.OverallLightning = p.LightLevel;
        }

        private void receive_PersonalLightLevel(IRecvPacket packet)
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



        private void receive_PlayMusic(IRecvPacket packet)
        {
            PlayMusicPacket p = (PlayMusicPacket)packet;
            // System.Console.WriteLine ( "Play music, id={0}", p.MusicID );
            UltimaData.MusicData.PlayMusic(p.MusicID);
        }

        private void receive_PlaySoundEffect(IRecvPacket packet)
        {
            PlaySoundEffectPacket p = (PlaySoundEffectPacket)packet;
            UltimaData.SoundData.PlaySound(p.SoundModel);
        }



        private void receive_QuestArrow(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }



        private void receive_RequestNameResponse(IRecvPacket packet)
        {
            RequestNameResponsePacket p = (RequestNameResponsePacket)packet;
            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, false);
            if (mobile != null)
                mobile.Name = p.MobileName;
        }



        private void receive_SeasonalInformation(IRecvPacket packet)
        {
            // Only partially handled !!! If iSeason2 = 1, then this is a season change.
            // If season change, then iSeason1 = (0=spring, 1=summer, 2=fall, 3=winter, 4 = desolation)
            SeasonChangePacket p = (SeasonChangePacket)packet;
            UltimaVars.EngineVars.Season = p.Season;
        }




        private void receive_SetWeather(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }
    }
}
