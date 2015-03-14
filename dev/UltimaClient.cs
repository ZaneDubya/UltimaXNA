/***************************************************************************
 *   UltimaClient.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Core.Network;
using UltimaXNA.Entity;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaPackets.Server;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.View;
#endregion

namespace UltimaXNA
{
    public class UltimaClient : Client
    {
        public UltimaClientStatus Status { get; protected set; }
        private int m_ServerRelayKey = 0;

        public UltimaClient()
        {
            Status = UltimaClientStatus.Unconnected;
            InternalRegisterAllPackets();
        }

        private void InternalRegisterAllPackets()
        {
            Register<DamagePacket>(0x0B, "Damage", 0x07, new TypedPacketReceiveHandler(receive_Damage));
            Register<MobileStatusCompactPacket>(0x11, "Mobile Status Compact", -1, new TypedPacketReceiveHandler(receive_StatusInfo));
            Register<WorldItemPacket>(0x1A, "World Item", -1, new TypedPacketReceiveHandler(receive_WorldItem));
            Register<LoginConfirmPacket>(0x1B, "Login Confirm", 37, new TypedPacketReceiveHandler(receive_PlayerLocaleAndBody));
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
            Register<LoginCompletePacket>(0x55, "Login Complete", 1, new TypedPacketReceiveHandler(receive_LoginComplete));
            Register<TimePacket>(0x5B, "Time", 4, new TypedPacketReceiveHandler(receive_Time));
            Register<WeatherPacket>(0x65, "Set Weather", 4, new TypedPacketReceiveHandler(receive_SetWeather));
            Register<PlayMusicPacket>(0x6D, "Play Music", 3, new TypedPacketReceiveHandler(receive_PlayMusic));
            Register<MobileAnimationPacket>(0x6E, "Character Animation", 14, new TypedPacketReceiveHandler(receive_MobileAnimation));
            Register<GraphicEffectPacket>(0x70, "Graphical Effect 1", 28, new TypedPacketReceiveHandler(receive_GraphicEffect));
            Register<WarModePacket>(0x72, "War Mode", 5, new TypedPacketReceiveHandler(receive_WarMode));
            Register<VendorBuyListPacket>(0x74, "Vendor Buy List", -1, new TypedPacketReceiveHandler(receive_OpenBuyWindow));
            Register<SubServerPacket>(0x76, "New Subserver", 16, new TypedPacketReceiveHandler(receive_NewSubserver));
            Register<MobileMovingPacket>(0x77, "Mobile Moving", 17, new TypedPacketReceiveHandler(receive_MobileMoving));
            Register<MobileIncomingPacket>(0x78, "Mobile Incoming", -1, new TypedPacketReceiveHandler(receive_MobileIncoming));
            Register<DisplayMenuPacket>(0x7C, "Display Menu", -1, new TypedPacketReceiveHandler(receive_DisplayMenu));
            Register<LoginRejectionPacket>(0x82, "Login Rejection", 2, new TypedPacketReceiveHandler(receive_LoginRejection));
            Register<DeleteCharacterResponsePacket>(0x85, "Delete Character Response", 2, new TypedPacketReceiveHandler(receive_DeleteCharacterResponse));
            Register<CharacterListUpdatePacket>(0x86, "Character List Update", -1, new TypedPacketReceiveHandler(receive_CharacterListUpdate));
            Register<OpenPaperdollPacket>(0x88, "Open Paperdoll", 66, new TypedPacketReceiveHandler(receive_OpenPaperdoll));
            Register<CorpseClothingPacket>(0x89, "Corpse Clothing", -1, new TypedPacketReceiveHandler(receive_CorpseClothing));
            Register<ServerRelayPacket>(0x8C, "ServerRelay", 11, new TypedPacketReceiveHandler(receive_ServerRelay));
            Register<PlayerMovePacket>(0x97, "Player Move", 2, new TypedPacketReceiveHandler(receive_PlayerMove));
            Register<RequestNameResponsePacket>(0x98, "Request Name Response", -1, new TypedPacketReceiveHandler(receive_RequestNameResponse));
            Register<VendorSellListPacket>(0x9E, "Vendor Sell List", -1, new TypedPacketReceiveHandler(receive_SellList));
            Register<UpdateHealthPacket>(0xA1, "Update Current Health", 9, new TypedPacketReceiveHandler(receive_UpdateHealth));
            Register<UpdateManaPacket>(0xA2, "Update Current Mana", 9, new TypedPacketReceiveHandler(receive_UpdateMana));
            Register<UpdateStaminaPacket>(0xA3, "Update Current Stamina", 9, new TypedPacketReceiveHandler(receive_UpdateStamina));
            Register<OpenWebBrowserPacket>(0xA5, "Open Web Browser", -1, new TypedPacketReceiveHandler(receive_OpenWebBrowser));
            Register<TipNoticePacket>(0xA6, "Tip/Notice Window", -1, new TypedPacketReceiveHandler(receive_TipNotice));
            Register<ServerListPacket>(0xA8, "Game Server List", -1, new TypedPacketReceiveHandler(receive_ServerList));
            Register<CharacterCityListPacket>(0xA9, "Characters / Starting Locations", -1, new TypedPacketReceiveHandler(receive_CharacterList));
            Register<ChangeCombatantPacket>(0xAA, "Change Combatant", 5, new TypedPacketReceiveHandler(receive_ChangeCombatant));
            Register<UnicodeMessagePacket>(0xAE, "Unicode Message", -1, new TypedPacketReceiveHandler(receive_UnicodeMessage));
            Register<DeathAnimationPacket>(0xAF, "Death Animation", 13, new TypedPacketReceiveHandler(receive_DeathAnimation));
            Register<DisplayGumpFastPacket>(0xB0, "Display Gump Fast", -1, new TypedPacketReceiveHandler(receive_DisplayGumpFast));
            Register<ObjectHelpResponsePacket>(0xB7, "Object Help Response ", -1, new TypedPacketReceiveHandler(receive_ObjectHelpResponse));
            Register<SupportedFeaturesPacket>(0xB9, "Supported Features", 3, new TypedPacketReceiveHandler(receive_EnableFeatures));
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
            network.Register<RecvPacket>(0x30, "Attack Ok", 5, OnAttackOk);
            network.Register<HealthBarStatusPacket>(0x17, "Health Bar Status Update", 12, OnHealthBarStatusUpdate);
            network.Register<KickPlayerPacket>(0x26, "Kick Player", 5, OnKickPlayer);
            network.Register<DropItemFailedPacket>(0x28, "Drop Item Failed", 5, OnDropItemFailed);
            network.Register<PaperdollClothingAddAckPacket>(0x29, "Paperdoll Clothing Add Ack", 1, OnPaperdollClothingAddAck);
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
        }

        public override bool Connect(string host, int port)
        {
            Status = UltimaClientStatus.LoginServer_Connecting;
            bool success = base.Connect(host, port);
            if (success)
            {
                Status = UltimaClientStatus.LoginServer_WaitingForLogin;
                Send(new SeedPacket(1, 6, 0, 6, 2));
            }
            else
            {
                Status = UltimaClientStatus.Error_CannotConnectToServer;
            }
            return success;
        }

        public void SendAccountLogin(string account, string password)
        {
            Status = UltimaClientStatus.LoginServer_LoggingIn;
            Send(new LoginPacket(account, password));
        }

        public void SendServerRelay(string account, string password)
        {
            Status = UltimaClientStatus.LoginServer_Relaying;
            Send(new GameLoginPacket(m_ServerRelayKey, account, password));
        }

        public void SelectServer(int index)
        {
            if (Status == UltimaClientStatus.LoginServer_HasServerList)
            {
                Status = UltimaClientStatus.GameServer_Connecting;
                Send(new SelectServerPacket(index));
            }
        }

        public void SelectCharacter(int index)
        {
            if (Status == UltimaClientStatus.GameServer_CharList)
            {
                if (UltimaVars.Characters.List[index].Name != string.Empty)
                {
                    Send(new LoginCharacterPacket(UltimaVars.Characters.List[index].Name, index, Utility.IPAddress));
                }
            }
        }

        public void DeleteCharacter(int index)
        {
            if (Status == UltimaClientStatus.GameServer_CharList)
            {
                if (UltimaVars.Characters.List[index].Name != string.Empty)
                {
                    Send(new DeleteCharacterPacket(index, Utility.IPAddress));
                }
            }
        }

        public override void Disconnect()
        {
            if (IsConnected)
                base.Disconnect();
            Status = UltimaClientStatus.Unconnected;
        }

        public override bool Send(ISendPacket packet)
        {
            if (IsConnected)
            {
                bool success = base.Send(packet);
                if (!success)
                {
                    Disconnect();
                }
                return success;
            }
            else
            {
                return false;
            }
        }











        private void receive_AddMultipleItemsToContainer(IRecvPacket packet)
        {
            ContainerContentPacket p = (ContainerContentPacket)packet;
            foreach (ContentItem i in p.Items)
            {
                // Add the item...
                Item iObject = add_Item(i.Serial, i.ItemID, i.Hue, i.ContainerSerial, i.Amount);
                iObject.X = i.X;
                iObject.Y = i.Y;
                // ... and add it the container contents of the container.
                Container c = EntityManager.GetObject<Container>(i.ContainerSerial, true);
                c.AddItem(iObject);
            }
        }

        private void receive_AddSingleItemToContainer(IRecvPacket packet)
        {
            ContainerContentUpdatePacket p = (ContainerContentUpdatePacket)packet;

            // Add the item...
            Item iObject = add_Item(p.Serial, p.ItemId, p.Hue, p.ContainerSerial, p.Amount);
            iObject.X = p.X;
            iObject.Y = p.Y;
            // ... and add it the container contents of the container.
            Container iContainerObject = EntityManager.GetObject<Container>(p.ContainerSerial, true);
            if (iContainerObject != null)
                iContainerObject.AddItem(iObject);
            else
            {
                // Special case for game boards... the server will sometimes send us game pieces for a game board before it sends 
                // the game board! Right now, I am discarding these messages, it might be better to queue them up for when the game
                // board actually exists.
                // Let's throw an exception if anything other than a gameboard is ever sent to us.
                // if (iObject.ItemData.Name != "game piece")
                throw new Exception("Item {" + iObject.ToString() + "} received before containing object received.");
            }
        }

        private void receive_DeleteCharacterResponse(IRecvPacket packet)
        {
            DeleteCharacterResponsePacket p = (DeleteCharacterResponsePacket)packet;
            UltimaInteraction.MsgBox(p.Result, MsgBoxTypes.OkOnly);
        }

        private void receive_CharacterListUpdate(IRecvPacket packet)
        {
            CharacterListUpdatePacket p = (CharacterListUpdatePacket)packet;
            UltimaVars.Characters.SetCharacterList(p.Characters);
        }

        private void receive_CLILOCMessage(IRecvPacket packet)
        {
            MessageLocalizedPacket p = (MessageLocalizedPacket)packet;

            string iCliLoc = constructCliLoc(UltimaData.StringData.Entry(p.CliLocNumber), p.Arguements);
            receive_TextMessage(p.MessageType, iCliLoc, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private void receive_ChangeCombatant(IRecvPacket packet)
        {
            ChangeCombatantPacket p = (ChangeCombatantPacket)packet;
            if (p.Serial > 0x00000000)
                UltimaVars.EngineVars.LastTarget = p.Serial;
        }

        private void receive_CharacterList(IRecvPacket packet)
        {
            CharacterCityListPacket p = (CharacterCityListPacket)packet;
            UltimaVars.Characters.SetCharacterList(p.Characters);
            UltimaVars.Characters.SetStartingLocations(p.Locations);
            Status = UltimaClientStatus.GameServer_CharList;
        }

        private void receive_CompressedGump(IRecvPacket packet)
        {
            CompressedGumpPacket p = (CompressedGumpPacket)packet;
            if (p.HasData)
            {
                string[] gumpPieces = interpretGumpPieces(p.GumpData);
                Gump g = (Gump)UltimaEngine.UserInterface.AddControl(new Gump(p.Serial, p.GumpID, gumpPieces, p.TextLines), p.X, p.Y);
                g.IsMovable = true;
            }
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
                if (item is Container)
                {
                    UltimaInteraction.OpenContainerGump(item);
                }
                else
                {
                    UltimaInteraction.ChatMessage(string.Format("Client: Object {0} has no support for a container object!", item.Serial));
                }
            }
        }

        private void receive_CorpseClothing(IRecvPacket packet)
        {
            CorpseClothingPacket p = (CorpseClothingPacket)packet;
            Corpse e = EntityManager.GetObject<Corpse>(p.CorpseSerial, false);
            e.LoadCorpseClothing(p.Items);
        }

        private void receive_Damage(IRecvPacket packet)
        {
            DamagePacket p = (DamagePacket)packet;
            Mobile u = EntityManager.GetObject<Mobile>(p.Serial, false);

            UltimaInteraction.ChatMessage(string.Format("{0} takes {1} damage!", u.Name, p.Damage));
        }

        private void receive_DeathAnimation(IRecvPacket packet)
        {
            DeathAnimationPacket p = (DeathAnimationPacket)packet;
            Mobile u = EntityManager.GetObject<Mobile>(p.PlayerSerial, false);
            Corpse c = EntityManager.GetObject<Corpse>(p.CorpseSerial, false);
            c.Facing = u.Facing;
            c.MobileSerial = p.PlayerSerial;
            c.DeathAnimation();
        }

        private void receive_DeleteObject(IRecvPacket packet)
        {
            RemoveEntityPacket p = (RemoveEntityPacket)packet;
            EntityManager.RemoveObject(p.Serial);
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

        private void receive_DisplayGumpFast(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_DisplayMenu(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_EnableFeatures(IRecvPacket packet)
        {
            SupportedFeaturesPacket p = (SupportedFeaturesPacket)packet;
            UltimaVars.Features.SetFlags(p.Flags);
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
                case 0x08: // Set cursor color / set map
                    UltimaVars.EngineVars.Map = p.MapID;
                    break;
                case 0x14: // return context menu
                    parseContextMenu(p.ContextMenu);
                    break;
                case 0x18: // Enable map-diff (files) / number of maps
                    // as of 6.0.0.0, this only tells us the number of maps.
                    UltimaVars.EngineVars.MapCount = p.MapCount;
                    break;
                case 0x19: // Extended stats
                    if (p.Serial != EntityManager.MySerial)
                        Diagnostics.Logger.Warn("Extended Stats packet (0xBF subcommand 0x19) received for a mobile not our own.");
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
                        Send(new RequestCustomHousePacket(p.HouseRevisionState.Serial));
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
            UltimaInteraction.ChatMessage("System: There are currently " + p.Count + " available calls in the global queue.");
        }

        private void receive_GraphicEffect(IRecvPacket packet)
        {
            DynamicObject dynamic = EntityManager.AddDynamicObject();
            dynamic.Load_FromPacket((GraphicEffectPacket)packet);
        }

        private void receive_HuedEffect(IRecvPacket packet)
        {
            DynamicObject dynamic = EntityManager.AddDynamicObject();
            dynamic.Load_FromPacket((GraphicEffectHuedPacket)packet);
        }

        private void receive_InvalidMapEnable(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_LoginComplete(IRecvPacket packet)
        {
            // This packet is just one byte, the opcode.
            // We want to make sure we have the client object before we load the world.
            // If we don't, just set the status to login complete, which will then
            // load the world when we finally receive our client object.
            if (EntityManager.MySerial != 0)
                Status = UltimaClientStatus.WorldServer_InWorld;
            else
                Status = UltimaClientStatus.WorldServer_LoginComplete;
        }

        private void receive_LoginRejection(IRecvPacket packet)
        {
            Disconnect();
            LoginRejectionPacket p = (LoginRejectionPacket)packet;
            switch (p.Reason)
            {
                case LoginRejectionReasons.InvalidAccountPassword:
                    Status = UltimaClientStatus.Error_InvalidUsernamePassword;
                    break;
                case LoginRejectionReasons.AccountInUse:
                    Status = UltimaClientStatus.Error_InUse;
                    break;
                case LoginRejectionReasons.AccountBlocked:
                    Status = UltimaClientStatus.Error_Blocked;
                    break;
                case LoginRejectionReasons.BadPassword:
                    Status = UltimaClientStatus.Error_BadPassword;
                    break;
                case LoginRejectionReasons.IdleExceeded:
                    Status = UltimaClientStatus.Error_Idle;
                    break;
                case LoginRejectionReasons.BadCommuncation:
                    Status = UltimaClientStatus.Error_BadCommunication;
                    break;
            }
        }

        private void receive_MessageLocalizedAffix(IRecvPacket packet)
        {
            MessageLocalizedAffixPacket p = (MessageLocalizedAffixPacket)packet;

            string localizedString = string.Format(p.Flag_IsPrefix ? "{1}{0}" : "{0}{1}",
                constructCliLoc(UltimaData.StringData.Entry(p.CliLocNumber), p.Arguements), p.Affix);
            receive_TextMessage(p.MessageType, localizedString, p.Hue, p.Font, p.Serial, p.SpeakerName);
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

        private void receive_MobileIncoming(IRecvPacket packet)
        {
            MobileIncomingPacket p = (MobileIncomingPacket)packet;
            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, true);
            mobile.BodyID = p.BodyID;
            mobile.Hue = (int)p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            mobile.IsFemale = p.Flags.IsFemale;
            mobile.IsPoisoned = p.Flags.IsPoisoned;
            mobile.IsBlessed = p.Flags.IsBlessed;
            mobile.IsWarMode = p.Flags.IsWarMode;
            mobile.IsHidden = p.Flags.IsHidden;
            mobile.Notoriety = p.Notoriety;
            mobile.Notoriety = p.Notoriety;

            for (int i = 0; i < p.Equipment.Length; i++)
            {
                Item item = add_Item(p.Equipment[i].Serial, p.Equipment[i].GumpId, p.Equipment[i].Hue, p.Serial, 0);
                mobile.WearItem(item, p.Equipment[i].Layer);
                if (item.PropertyList.Hash == 0)
                    Send(new QueryPropertiesPacket(item.Serial));
            }

            if (mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                Send(new RequestNamePacket(p.Serial));
            }
        }

        private void receive_MobileMoving(IRecvPacket packet)
        {
            MobileMovingPacket p = (MobileMovingPacket)packet;

            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, true);
            mobile.BodyID = p.BodyID;
            mobile.IsFemale = p.Flags.IsFemale;
            mobile.IsPoisoned = p.Flags.IsPoisoned;
            mobile.IsBlessed = p.Flags.IsBlessed;
            mobile.IsWarMode = p.Flags.IsWarMode;
            mobile.IsHidden = p.Flags.IsHidden;
            mobile.Notoriety = p.Notoriety;
            // Issue 16 - Pet not showing at login - http://code.google.com/p/ultimaxna/issues/detail?id=16 - Smjert
            // Since no packet arrives to add your pet, when you move and your pet follows you the client crashes
            if (mobile.Position.IsNullPosition)
            {
                mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);
                // Issue 16 - End
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
            mobile.IsFemale = p.Flags.IsFemale;
            mobile.IsPoisoned = p.Flags.IsPoisoned;
            mobile.IsBlessed = p.Flags.IsBlessed;
            mobile.IsWarMode = p.Flags.IsWarMode;
            mobile.IsHidden = p.Flags.IsHidden;
            mobile.Hue = (int)p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);

            if (mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                Send(new RequestNamePacket(p.Serial));
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

            BaseEntity iObject = EntityManager.GetObject<BaseEntity>(p.Serial, false);
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

        private void receive_OnParticleEffect(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_OnSwing(IRecvPacket packet)
        {
            SwingPacket p = (SwingPacket)packet;
            if (p.Attacker == EntityManager.MySerial)
            {
                UltimaVars.EngineVars.LastTarget = p.Defender;
            }
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

        private void receive_OpenPaperdoll(IRecvPacket packet)
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
            // Console.WriteLine("OverallLight: {0}", p.LightLevel);
            IsometricRenderer.OverallLightning = p.LightLevel;
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
            // Console.WriteLine("PersonalLight: {0}", p.LightLevel);
            IsometricRenderer.PersonalLightning = p.LightLevel;
        }

        private void receive_PlayerLocaleAndBody(IRecvPacket packet)
        {
            LoginConfirmPacket p = (LoginConfirmPacket)packet;

            // When loading the player object, we must load the serial before the object.
            EntityManager.MySerial = p.Serial;
            PlayerMobile iPlayer = EntityManager.GetObject<PlayerMobile>(p.Serial, true);
            iPlayer.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            // iPlayer.SetFacing(p.Direction);

            // We want to make sure we have the client object before we load the world...
            if (Status == UltimaClientStatus.WorldServer_LoginComplete)
                Status = UltimaClientStatus.WorldServer_InWorld;
        }

        private void receive_PlayerMove(IRecvPacket packet)
        {
            PlayerMovePacket p = (PlayerMovePacket)packet;
            announce_UnhandledPacket(packet);
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

        private void receive_PopupMessage(IRecvPacket packet)
        {
            PopupMessagePacket p = (PopupMessagePacket)packet;
            UltimaInteraction.MsgBox(p.Message, MsgBoxTypes.OkOnly);
        }

        private void receive_QuestArrow(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_RejectMoveItemRequest(IRecvPacket packet)
        {
            LiftRejectionPacket p = (LiftRejectionPacket)packet;
            UltimaInteraction.ChatMessage("Could not pick up item: " + p.ErrorMessage);
            UltimaInteraction.ClearHolding();
        }

        private void receive_RequestNameResponse(IRecvPacket packet)
        {
            RequestNameResponsePacket p = (RequestNameResponsePacket)packet;
            Mobile u = EntityManager.GetObject<Mobile>(p.Serial, false);
            u.Name = p.MobileName;
        }

        private void receive_ResurrectionMenu(IRecvPacket packet)
        {
            // int iAction = reader.ReadByte();
            // 0: Server sent
            // 1: Resurrect
            // 2: Ghost
            // The only use on OSI for this packet is now sending "2C02" for the "You Are Dead" screen upon character death.
            announce_UnhandledPacket(packet);
        }

        private void receive_SeasonalInformation(IRecvPacket packet)
        {
            // Only partially handled !!! If iSeason2 = 1, then this is a season change.
            // If season change, then iSeason1 = (0=spring, 1=summer, 2=fall, 3=winter, 4 = desolation)
            SeasonChangePacket p = (SeasonChangePacket)packet;
            UltimaVars.EngineVars.Season = p.Season;
        }

        private void receive_SellList(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_ServerList(IRecvPacket packet)
        {
            UltimaVars.Servers.List = ((ServerListPacket)packet).Servers;
            Status = UltimaClientStatus.LoginServer_HasServerList;
        }

        private void receive_SetWeather(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_ServerRelay(IRecvPacket packet)
        {
            ServerRelayPacket p = (ServerRelayPacket)packet;
            m_ServerRelayKey = p.AccountId;
            // Normally, upon receiving this packet you would disconnect and
            // log in to the specified server. Since we are using RunUO, we don't
            // actually need to do this.
            IsDecompressionEnabled = true;
            Status = UltimaClientStatus.LoginServer_WaitingForRelay;
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

        private void receive_AsciiMessage(IRecvPacket packet)
        {
            AsciiMessagePacket p = (AsciiMessagePacket)packet;
            receive_TextMessage(p.MsgType, p.Text, p.Hue, p.Font, p.Serial, p.Name1);
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
            UltimaInteraction.ChatMessage(string.Format("The current server time is {0}:{1}:{2}", p.Hour, p.Minute, p.Second));
        }

        private void receive_TipNotice(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_ToolTipRevision(IRecvPacket packet)
        {
            ObjectPropertyListUpdatePacket p = (ObjectPropertyListUpdatePacket)packet;
            BaseEntity iObject = EntityManager.GetObject<BaseEntity>(p.Serial, false);
            if (iObject != null)
            {
                if (iObject.PropertyList.Hash != p.RevisionHash)
                {
                    Send(new QueryPropertiesPacket(p.Serial));
                }
            }
        }

        private void receive_UnicodeMessage(IRecvPacket packet)
        {
            UnicodeMessagePacket p = (UnicodeMessagePacket)packet;
            receive_TextMessage(p.MsgType, p.SpokenText, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private void receive_UpdateHealth(IRecvPacket packet)
        {
            UpdateHealthPacket p = (UpdateHealthPacket)packet;
            Mobile u = EntityManager.GetObject<Mobile>(p.Serial, false);
            u.Health.Update(p.Current, p.Max);
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

        private void receive_WarMode(IRecvPacket packet)
        {
            WarModePacket p = (WarModePacket)packet;
            UltimaVars.EngineVars.WarMode = p.WarMode;
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
                item.X = p.X;
                item.Y = p.Y;
                item.Z = p.Z;
            }
            else
            {
                // create a multi object. Unhandled !!!
                int multiID = p.ItemID - 0x4000;
                Multi multi = EntityManager.GetObject<Multi>(p.Serial, true);
                multi.ItemID = p.ItemID;
                multi.X = p.X;
                multi.Y = p.Y;
                multi.Z = p.Z;
            }
        }

        private void receive_WornItem(IRecvPacket packet)
        {
            WornItemPacket p = (WornItemPacket)packet;
            Item item = add_Item(p.Serial, p.ItemId, p.Hue, p.ParentSerial, 0);
            Mobile m = EntityManager.GetObject<Mobile>(p.ParentSerial, false);
            m.WearItem(item, p.Layer);
            if (item.PropertyList.Hash == 0)
                Send(new QueryPropertiesPacket(item.Serial));
        }



        private void announce_UnhandledPacket(IRecvPacket packet)
        {
            Diagnostics.Logger.Warn(string.Format("Client: Unhandled {0} [ID:{1}]", packet.Name, packet.Id));
        }

        private void announce_UnhandledPacket(IRecvPacket packet, string addendum)
        {
            Diagnostics.Logger.Warn(string.Format("Client: Unhandled {0} [ID:{1}] {2}]", packet.Name, packet.Id, addendum));
        }

        private void receive_TextMessage(MessageType msgType, string text, int hue, int font, Serial serial, string speakerName)
        {
            Overhead overhead;
            switch (msgType)
            {
                case MessageType.Regular:
                    overhead = EntityManager.AddOverhead(msgType, serial, text, font, hue);
                    if (overhead != null)
                    {
                        UltimaInteraction.ChatMessage(speakerName + ": " + text, font, hue);
                        overhead.SpeakerName = speakerName;
                    }
                    else
                    {
                        UltimaInteraction.ChatMessage(text, font, hue);
                    }
                    break;
                case MessageType.System:
                    UltimaInteraction.ChatMessage("[SYSTEM] " + text, font, hue);
                    break;
                case MessageType.Emote:
                    UltimaInteraction.ChatMessage("[EMOTE] " + text, font, hue);
                    break;
                case MessageType.Label:
                    if (serial.IsValid)
                    {
                        overhead = EntityManager.AddOverhead(msgType, serial, text, font, hue);
                        overhead.SpeakerName = speakerName;
                        // Labels that are longer than the current name should be set as the name
                        if (serial.IsMobile)
                        {
                            Mobile m = EntityManager.GetObject<Mobile>(serial, false);
                            if (m.Name.Length < text.Length)
                                m.Name = text;
                        }
                    }
                    else
                    {
                        UltimaInteraction.ChatMessage("[LABEL] " + text, font, hue);
                    }
                    break;
                case MessageType.Focus: // on player?
                    UltimaInteraction.ChatMessage("[FOCUS] " + text, font, hue);
                    break;
                case MessageType.Whisper:
                    UltimaInteraction.ChatMessage("[WHISPER] " + text, font, hue);
                    break;
                case MessageType.Yell:
                    UltimaInteraction.ChatMessage("[YELL] " + text, font, hue);
                    break;
                case MessageType.Spell:
                    UltimaInteraction.ChatMessage("[SPELL] " + text, font, hue);
                    break;
                case MessageType.UIld:
                    UltimaInteraction.ChatMessage("[UILD] " + text, font, hue);
                    break;
                case MessageType.Alliance:
                    UltimaInteraction.ChatMessage("[ALLIANCE] " + text, font, hue);
                    break;
                case MessageType.Command:
                    UltimaInteraction.ChatMessage("[COMMAND] " + text, font, hue);
                    break;
                default:
                    UltimaInteraction.ChatMessage("[[ERROR UNKNOWN COMMAND]] " + msgType.ToString());
                    break;
            }
        }

        private void parseContextMenu(ContextMenu context)
        {
            if (context.HasContextMenu)
            {
                if (context.CanSell)
                {
                    Send(new ContextMenuResponsePacket(context.Serial, (short)context.ContextEntry("Sell").ResponseCode));
                }
            }
            else
            {
                // no context menu entries are handled. Send a double click.
                Send(new DoubleClickPacket(context.Serial));
            }
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
    }
}
