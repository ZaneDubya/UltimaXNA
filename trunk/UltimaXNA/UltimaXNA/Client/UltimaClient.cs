/***************************************************************************
 *   UltimaClient.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Entities;
using UltimaXNA.Network;
using UltimaXNA.Client.Packets.Client;
using UltimaXNA.Client.Packets.Server;
using UltimaXNA.TileEngine;
using UltimaXNA.Extensions;
using UltimaXNA.UILegacy;
#endregion

namespace UltimaXNA.Client
{
    class UltimaClient
    {
        public static UltimaClientStatus Status { get; protected set; }
        public static bool IsConnected { get { return _ClientNetwork.IsConnected; } }

        // Account name and Password data.
        protected static string _account, _password;
        protected static string _host;
        protected static int _port;
        public static void SetLoginData(string host, int port, string account, string password)
        {
            _host = host; _port = port; _account = account; _password = password;
        }

        static ClientNetwork _ClientNetwork;
        static TileEngine.IIsometricRenderer _worldService;
        static IUIManager _LegacyUI = null;

        static UltimaClient()
        {
            Status = UltimaClientStatus.Unconnected;
            _ClientNetwork = new ClientNetwork();
            registerPackets();
        }

        public static void Initialize(Game game)
        {
            _worldService = game.Services.GetService<TileEngine.IIsometricRenderer>();
            _LegacyUI = game.Services.GetService<IUIManager>();
        }

        public static void Update(GameTime gameTime)
        {
            _ClientNetwork.Update();
        }

        private static void registerPackets()
        {
            PacketRegistry.Damage += receive_Damage;
            PacketRegistry.MobileStatusCompact += receive_StatusInfo;
            PacketRegistry.WorldItem += receive_WorldItem;
            PacketRegistry.LoginConfirm += receive_PlayerLocaleAndBody;
            PacketRegistry.AsciiMessage += receive_AsciiMessage;
            PacketRegistry.RemoveEntity += receive_DeleteObject;
            PacketRegistry.MobileUpdate += receive_MobileUpdate;
            PacketRegistry.MovementRejected += receive_MoveRej;
            PacketRegistry.MoveAcknowledged += receive_MoveAck;
            PacketRegistry.DragEffect += receive_DragItem;
            PacketRegistry.OpenContainer += receive_Container;
            PacketRegistry.ContainerContentUpdate += receive_AddSingleItemToContainer;
            PacketRegistry.LiftRejection += receive_RejectMoveItemRequest;
            PacketRegistry.ResurrectMenu += receive_ResurrectionMenu;
            PacketRegistry.MobileAttributes += receive_MobileAttributes;
            PacketRegistry.WornItem += receive_WornItem;
            PacketRegistry.Swing += receive_OnSwing;
            PacketRegistry.SkillsList += receive_SkillsList;
            PacketRegistry.ContainerContent += receive_AddMultipleItemsToContainer;
            PacketRegistry.PersonalLightLevel += receive_PersonalLightLevel;
            PacketRegistry.OverallLightLevel += receive_OverallLightLevel;
            PacketRegistry.PopupMessage += receive_PopupMessage;
            PacketRegistry.PlaySoundEffect += receive_PlaySoundEffect;
            PacketRegistry.LoginComplete += receive_LoginComplete;
            PacketRegistry.Time += receive_Time;
            PacketRegistry.Weather += receive_SetWeather;
            PacketRegistry.TargetCursor += receive_TargetCursor;
            PacketRegistry.PlayMusic += receive_PlayMusic;
            PacketRegistry.MobileAnimation += receive_MobileAnimation;
            PacketRegistry.GraphicalEffect1 += receive_GraphicEffect;
            PacketRegistry.WarMode += receive_WarMode;
            PacketRegistry.VendorBuyList += receive_OpenBuyWindow;
            PacketRegistry.NewSubserver += receive_NewSubserver;
            PacketRegistry.MobileMoving += receive_MobileMoving;
            PacketRegistry.MobileIncoming += receive_MobileIncoming;
            PacketRegistry.DisplayMenu += receive_DisplayMenu;
            PacketRegistry.LoginRejection += receive_LoginRejection;
            PacketRegistry.DeleteCharacterResponse += receive_DeleteCharacterResponse;
            PacketRegistry.CharacterListUpdate += receive_CharacterListUpdate;
            PacketRegistry.OpenPaperdoll += receive_OpenPaperdoll;
            PacketRegistry.CorpseClothing += receive_CorpseClothing;
            PacketRegistry.ServerRelay += receive_ServerRelay;
            PacketRegistry.PlayerMove += receive_PlayerMove;
            PacketRegistry.RequestNameResponse += receive_RequestNameResponse;
            PacketRegistry.TargetCursorMulti += receive_TargetCursorMulti;
            PacketRegistry.VendorSellList += receive_SellList;
            PacketRegistry.UpdateCurrentHealth += receive_UpdateHealth;
            PacketRegistry.UpdateCurrentMana += receive_UpdateMana;
            PacketRegistry.UpdateCurrentStamina += receive_UpdateStamina;
            PacketRegistry.OpenWebBrowser += receive_OpenWebBrowser;
            PacketRegistry.TipNoticeWindow += receive_TipNotice;
            PacketRegistry.ServerList += receive_ServerList;
            PacketRegistry.CharactersStartingLocations += receive_CharacterList;
            PacketRegistry.ChangeCombatant += receive_ChangeCombatant;
            PacketRegistry.UnicodeMessage += receive_UnicodeMessage;
            PacketRegistry.DeathAnimation += receive_DeathAnimation;
            PacketRegistry.DisplayGumpFast += receive_DisplayGumpFast;
            PacketRegistry.ObjectHelpResponse += receive_ObjectHelpResponse;
            PacketRegistry.SupportedFeatures += receive_EnableFeatures;
            PacketRegistry.QuestArrow += receive_QuestArrow;
            PacketRegistry.SeasonChange += receive_SeasonalInformation;
            PacketRegistry.VersionRequest += receive_VersionRequest;
            PacketRegistry.GeneralInfo += receive_GeneralInfo;
            PacketRegistry.HuedEffect += receive_HuedEffect;
            PacketRegistry.MessageLocalized += receive_CLILOCMessage;
            PacketRegistry.InvalidMapEnable += receive_InvalidMapEnable;
            PacketRegistry.ParticleEffect += receive_OnParticleEffect;
            PacketRegistry.GlobalQueueCount += receive_GlobalQueueCount;
            PacketRegistry.MessageLocalizedAffix += receive_MessageLocalizedAffix;
            PacketRegistry.Extended0x78 += receive_Extended0x78;
            PacketRegistry.MegaCliloc += receive_ObjectPropertyList;
            PacketRegistry.SendCustomHouse += receive_SendCustomHouse;
            PacketRegistry.ToolTipRevision += receive_ToolTipRevision;
            PacketRegistry.CompressedGump += receive_CompressedGump;

            PacketRegistry.RegisterNetwork(_ClientNetwork);
        }

        public static bool Connect()
        {
            Status = UltimaClientStatus.LoginServer_Connecting;
            bool success = _ClientNetwork.Connect(_host, _port);
            if (success)
            {
                Status = UltimaClientStatus.LoginServer_WaitingForLogin;
                _ClientNetwork.Send(new SeedPacket(1, 6, 0, 6, 2));
            }
            else
            {
                Status = UltimaClientStatus.Error_CannotConnectToServer;
            }
            return success;
        }

        public static void Login()
        {
            Status = UltimaClientStatus.LoginServer_LoggingIn;
            Send(new Packets.Client.LoginPacket(_account, _password));
        }

        public static void SelectServer(int index)
        {
            if (Status == UltimaClientStatus.LoginServer_HasServerList)
            {
                Status = UltimaClientStatus.GameServer_Connecting;
                Send(new SelectServerPacket(index));
            }
        }

        public static void SelectCharacter(int index)
        {
            if (Status == UltimaClientStatus.GameServer_CharList)
            {
                if (ClientVars.Characters.List[index].Name != string.Empty)
                {
                    Send(new LoginCharacterPacket(ClientVars.Characters.List[index].Name, index, Utility.IPAddress));
                }
            }
        }

        public static void DeleteCharacter(int index)
        {
            if (Status == UltimaClientStatus.GameServer_CharList)
            {
                if (ClientVars.Characters.List[index].Name != string.Empty)
                {
                    Send(new DeleteCharacterPacket(_password, index, Utility.IPAddress));
                }
            }
        }

        public static void Disconnect()
        {
            if (_ClientNetwork.IsConnected)
                _ClientNetwork.Disconnect();
            Status = UltimaClientStatus.Unconnected;
        }

        public static void Send(ISendPacket packet)
        {
            if (_ClientNetwork.IsConnected)
            {
                bool success = _ClientNetwork.Send(packet);
                if (!success)
                {
                    Disconnect();
                }
            }
        }











        private static void receive_AddMultipleItemsToContainer(IRecvPacket packet)
        {
            ContainerContentPacket p = (ContainerContentPacket)packet;
            foreach (ContentItem i in p.Items)
            {
                // Add the item...
                Item iObject = add_Item(i.Serial, i.ItemID, i.Hue, i.ContainerSerial, i.Amount);
                iObject.X = i.X;
                iObject.Y = i.Y;
                // ... and add it the container contents of the container.
                Container c = EntitiesCollection.GetObject<Container>(i.ContainerSerial, true);
                c.AddItem(iObject);
            }
        }

        private static void receive_AddSingleItemToContainer(IRecvPacket packet)
        {
            ContainerContentUpdatePacket p = (ContainerContentUpdatePacket)packet;

            // Add the item...
            Item iObject = add_Item(p.Serial, p.ItemId, p.Hue, p.ContainerSerial, p.Amount);
            iObject.X = p.X;
            iObject.Y = p.Y;
            // ... and add it the container contents of the container.
            Container iContainerObject = EntitiesCollection.GetObject<Container>(p.ContainerSerial, true);
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

        private static void receive_DeleteCharacterResponse(IRecvPacket packet)
        {
            DeleteCharacterResponsePacket p = (DeleteCharacterResponsePacket)packet;
            _LegacyUI.MsgBox(p.Result, MsgBoxTypes.OkOnly);
        }

        private static void receive_CharacterListUpdate(IRecvPacket packet)
        {
            CharacterListUpdatePacket p = (CharacterListUpdatePacket)packet;
            ClientVars.Characters.SetCharacterList(p.Characters);
        }

        private static void receive_CLILOCMessage(IRecvPacket packet)
        {
            MessageLocalizedPacket p = (MessageLocalizedPacket)packet;

            string iCliLoc = constructCliLoc(Data.StringList.Entry(p.CliLocNumber), p.Arguements);
            receive_TextMessage(p.MessageType, iCliLoc, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private static void receive_ChangeCombatant(IRecvPacket packet)
        {
            ChangeCombatantPacket p = (ChangeCombatantPacket)packet;
            if (p.Serial > 0x00000000)
                ClientVars.EngineVars.LastTarget = p.Serial;
        }

        private static void receive_CharacterList(IRecvPacket packet)
        {
            CharacterCityListPacket p = (CharacterCityListPacket)packet;
            ClientVars.Characters.SetCharacterList(p.Characters);
            ClientVars.Characters.SetStartingLocations(p.Locations);
            Status = UltimaClientStatus.GameServer_CharList;
        }

        private static void receive_CompressedGump(IRecvPacket packet)
        {
            CompressedGumpPacket p = (CompressedGumpPacket)packet;
            if (p.HasData)
            {
                string[] gumpPieces = interpretGumpPieces(p.GumpData);
                _LegacyUI.AddGump_Server(p.Serial, p.GumpID, gumpPieces, p.TextLines, p.X, p.Y);
            }
        }

        private static void receive_Container(IRecvPacket packet)
        {
            OpenContainerPacket p = (OpenContainerPacket)packet;

            Container item;
            // Special case for 0x30, which tells us to open a buy from vendor window.
            if (p.GumpId == 0x30)
            {
                Mobile m = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
                item = m.VendorShopContents;
            }
            else
            {
                item = EntitiesCollection.GetObject<Container>(p.Serial, false);
                if (item is Container)
                {
                    _LegacyUI.AddContainerGump(item, item.ItemID);
                }
                else
                {
                    _LegacyUI.AddMessage_Chat(string.Format("Client: Object {0} has no support for a container object!", item.Serial));
                }
            }
        }

        private static void receive_CorpseClothing(IRecvPacket packet)
        {
            CorpseClothingPacket p = (CorpseClothingPacket)packet;
            Corpse e = EntitiesCollection.GetObject<Corpse>(p.CorpseSerial, false);
            e.LoadCorpseClothing(p.Items);
        }

        private static void receive_Damage(IRecvPacket packet)
        {
            DamagePacket p = (DamagePacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);

            _LegacyUI.AddMessage_Chat(string.Format("{0} takes {1} damage!", u.Name, p.Damage));
        }

        private static void receive_DeathAnimation(IRecvPacket packet)
        {
            DeathAnimationPacket p = (DeathAnimationPacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.PlayerSerial, false);
            Corpse c = EntitiesCollection.GetObject<Corpse>(p.CorpseSerial, false);
            c.Facing = u.Facing;
            c.MobileSerial = p.PlayerSerial;
            c.DeathAnimation();
        }

        private static void receive_DeleteObject(IRecvPacket packet)
        {
            RemoveEntityPacket p = (RemoveEntityPacket)packet;
            EntitiesCollection.RemoveObject(p.Serial);
        }

        private static void receive_DragItem(IRecvPacket packet)
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

        private static void receive_DisplayGumpFast(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_DisplayMenu(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_EnableFeatures(IRecvPacket packet)
        {
            SupportedFeaturesPacket p = (SupportedFeaturesPacket)packet;
            ClientVars.Features.SetFlags(p.Flags);
        }

        private static void receive_Extended0x78(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_GeneralInfo(IRecvPacket packet)
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
                    ClientVars.EngineVars.Map = p.MapID;
                    break;
                case 0x14: // return context menu
                    parseContextMenu(p.ContextMenu);
                    break;
                case 0x18: // Enable map-diff (files) / number of maps
                    // as of 6.0.0.0, this only tells us the number of maps.
                    ClientVars.EngineVars.MapCount = p.MapCount;
                    break;
                case 0x19: // Extended stats
                    if (p.Serial != EntitiesCollection.MySerial)
                        Diagnostics.Logger.Warn("Extended Stats packet (0xBF subcommand 0x19) received for a mobile not our own.");
                    else
                    {
                        ClientVars.Status.StrengthLock = p.StatisticLocks.Strength;
                        ClientVars.Status.DexterityLock = p.StatisticLocks.Dexterity;
                        ClientVars.Status.IntelligenceLock = p.StatisticLocks.Intelligence;
                    }
                    break;
                case 0x1D: // House revision state
                    if (Data.CustomHousing.IsHashCurrent(p.HouseRevisionState.Serial, p.HouseRevisionState.Hash))
                    {
                        Multi e = EntitiesCollection.GetObject<Multi>(p.HouseRevisionState.Serial, false);
                        if (e.CustomHouseRevision != p.HouseRevisionState.Hash)
                        {
                            Data.CustomHouse house = Data.CustomHousing.GetCustomHouseData(p.HouseRevisionState.Serial);
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

        private static void receive_GlobalQueueCount(IRecvPacket packet)
        {
            GlobalQueuePacket p = (GlobalQueuePacket)packet;
            _LegacyUI.AddMessage_Chat("System: There are currently " + p.Count + " available calls in the global queue.");
        }

        private static void receive_GraphicEffect(IRecvPacket packet)
        {
            DynamicObject dynamic = EntitiesCollection.AddDynamicObject();
            dynamic.LoadFromPacket((GraphicEffectPacket)packet);
        }

        private static void receive_HuedEffect(IRecvPacket packet)
        {
            DynamicObject dynamic = EntitiesCollection.AddDynamicObject();
            dynamic.LoadFromPacket((GraphicEffectHuedPacket)packet);
        }

        private static void receive_InvalidMapEnable(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_LoginComplete(IRecvPacket packet)
        {
            // This packet is just one byte, the opcode.
            // We want to make sure we have the client object before we load the world.
            // If we don't, just set the status to login complete, which will then
            // load the world when we finally receive our client object.
            if (EntitiesCollection.MySerial != 0)
                Status = UltimaClientStatus.WorldServer_InWorld;
            else
                Status = UltimaClientStatus.WorldServer_LoginComplete;
        }

        private static void receive_LoginRejection(IRecvPacket packet)
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

        private static void receive_MessageLocalizedAffix(IRecvPacket packet)
        {
            MessageLocalizedAffixPacket p = (MessageLocalizedAffixPacket)packet;

            string localizedString = string.Format(p.Flag_IsPrefix ? "{1}{0}" : "{0}{1}",
                constructCliLoc(Data.StringList.Entry(p.CliLocNumber), p.Arguements), p.Affix);
            receive_TextMessage(p.MessageType, localizedString, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private static void receive_MobileAttributes(IRecvPacket packet)
        {
            MobileAttributesPacket p = (MobileAttributesPacket)packet;
            announce_UnhandledPacket(packet);
        }

        private static void receive_MobileAnimation(IRecvPacket packet)
        {
            MobileAnimationPacket p = (MobileAnimationPacket)packet;

            Mobile m = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            m.Animate(p.Action, p.FrameCount, p.RepeatCount, p.Reverse, p.Repeat, p.Delay);
        }

        private static void receive_MobileIncoming(IRecvPacket packet)
        {
            MobileIncomingPacket p = (MobileIncomingPacket)packet;
            Mobile mobile = EntitiesCollection.GetObject<Mobile>(p.Serial, true);
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

        private static void receive_MobileMoving(IRecvPacket packet)
        {
            MobileMovingPacket p = (MobileMovingPacket)packet;

            Mobile mobile = EntitiesCollection.GetObject<Mobile>(p.Serial, true);
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

        private static void receive_MobileUpdate(IRecvPacket packet)
        {
            MobileUpdatePacket p = (MobileUpdatePacket)packet;
            Mobile mobile = EntitiesCollection.GetObject<Mobile>(p.Serial, true);
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

        private static void receive_MoveAck(IRecvPacket packet)
        {
            MoveAcknowledgePacket p = (MoveAcknowledgePacket)packet;
            Mobile player = (Mobile)EntitiesCollection.GetPlayerObject();
            player.PlayerMobile_MoveEventAck(p.Sequence);
            player.Notoriety = p.Notoriety;
        }

        private static void receive_MoveRej(IRecvPacket packet)
        {
            MovementRejectPacket p = (MovementRejectPacket)packet;
            Mobile player = (Mobile)EntitiesCollection.GetPlayerObject();
            player.PlayerMobile_MoveEventRej(p.Sequence, p.X, p.Y, p.Z, p.Direction);
        }

        private static void receive_NewSubserver(IRecvPacket packet)
        {
            SubServerPacket p = (SubServerPacket)packet;
            announce_UnhandledPacket(packet);
        }

        private static void receive_ObjectHelpResponse(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_ObjectPropertyList(IRecvPacket packet)
        {
            ObjectPropertyListPacket p = (ObjectPropertyListPacket)packet;

            Entity iObject = EntitiesCollection.GetObject<Entity>(p.Serial, false);
            iObject.PropertyList.Hash = p.Hash;
            iObject.PropertyList.Clear();

            for (int i = 0; i < p.CliLocs.Count; i++)
            {
                string iCliLoc = Data.StringList.Entry(p.CliLocs[i]);
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

        private static void receive_SendCustomHouse(IRecvPacket packet)
        {
            CustomHousePacket p = (CustomHousePacket)packet;
            Data.CustomHousing.UpdateCustomHouseData(p.HouseSerial, p.RevisionHash, p.PlaneCount, p.Planes);

            Multi e = EntitiesCollection.GetObject<Multi>(p.HouseSerial, false);
            if (e.CustomHouseRevision != p.RevisionHash)
            {
                Data.CustomHouse house = Data.CustomHousing.GetCustomHouseData(p.HouseSerial);
                e.AddCustomHousingTiles(house);
            }
        }

        private static void receive_OnParticleEffect(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_OnSwing(IRecvPacket packet)
        {
            SwingPacket p = (SwingPacket)packet;
            if (p.Attacker == EntitiesCollection.MySerial)
            {
                ClientVars.EngineVars.LastTarget = p.Defender;
            }
        }

        private static void receive_OpenBuyWindow(IRecvPacket packet)
        {
            VendorBuyListPacket p = (VendorBuyListPacket)packet;
            Item iObject = EntitiesCollection.GetObject<Item>(p.VendorPackSerial, false);
            if (iObject == null)
                return;
            // UserInterface.Merchant_Open(iObject, 0);
            // !!!
        }

        private static void receive_OpenPaperdoll(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_OpenWebBrowser(IRecvPacket packet)
        {
            OpenWebBrowserPacket p = (OpenWebBrowserPacket)packet;
            System.Diagnostics.Process.Start("iexplore.exe", p.WebsiteUrl);
        }

        private static void receive_OverallLightLevel(IRecvPacket packet)
        {
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F

            OverallLightLevelPacket p = (OverallLightLevelPacket)packet;
            // Console.WriteLine("OverallLight: {0}", p.LightLevel);
            _worldService.OverallLightning = p.LightLevel;
        }

        private static void receive_PersonalLightLevel(IRecvPacket packet)
        {
            // int iCreatureID = reader.ReadInt();
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F

            PersonalLightLevelPacket p = (PersonalLightLevelPacket)packet;
            // Console.WriteLine("PersonalLight: {0}", p.LightLevel);
            _worldService.PersonalLightning = p.LightLevel;
        }

        private static void receive_PlayerLocaleAndBody(IRecvPacket packet)
        {
            LoginConfirmPacket p = (LoginConfirmPacket)packet;

            // When loading the player object, we must load the serial before the object.
            EntitiesCollection.MySerial = p.Serial;
            PlayerMobile iPlayer = EntitiesCollection.GetObject<PlayerMobile>(p.Serial, true);
            iPlayer.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            // iPlayer.SetFacing(p.Direction);

            // We want to make sure we have the client object before we load the world...
            if (Status == UltimaClientStatus.WorldServer_LoginComplete)
                Status = UltimaClientStatus.WorldServer_InWorld;
        }

        private static void receive_PlayerMove(IRecvPacket packet)
        {
            PlayerMovePacket p = (PlayerMovePacket)packet;
            announce_UnhandledPacket(packet);
        }

        private static void receive_PlayMusic(IRecvPacket packet)
        {
            PlayMusicPacket p = (PlayMusicPacket)packet;
            // System.Console.WriteLine ( "Play music, id={0}", p.MusicID );
            Data.Music.PlayMusic(p.MusicID);
        }

        private static void receive_PlaySoundEffect(IRecvPacket packet)
        {
            PlaySoundEffectPacket p = (PlaySoundEffectPacket)packet;
            Data.Sounds.PlaySound(p.SoundModel);
        }

        private static void receive_PopupMessage(IRecvPacket packet)
        {
            PopupMessagePacket p = (PopupMessagePacket)packet;
            _LegacyUI.MsgBox(p.Message, MsgBoxTypes.OkOnly);
        }

        private static void receive_QuestArrow(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_RejectMoveItemRequest(IRecvPacket packet)
        {
            LiftRejectionPacket p = (LiftRejectionPacket)packet;
            _LegacyUI.AddMessage_Chat("Could not pick up item: " + p.ErrorMessage);
            _LegacyUI.Cursor.ClearHolding();
        }

        private static void receive_RequestNameResponse(IRecvPacket packet)
        {
            RequestNameResponsePacket p = (RequestNameResponsePacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            u.Name = p.MobileName;
        }

        private static void receive_ResurrectionMenu(IRecvPacket packet)
        {
            // int iAction = reader.ReadByte();
            // 0: Server sent
            // 1: Resurrect
            // 2: Ghost
            // The only use on OSI for this packet is now sending "2C02" for the "You Are Dead" screen upon character death.
            announce_UnhandledPacket(packet);
        }

        private static void receive_SeasonalInformation(IRecvPacket packet)
        {
            // Only partially handled !!! If iSeason2 = 1, then this is a season change.
            // If season change, then iSeason1 = (0=spring, 1=summer, 2=fall, 3=winter, 4 = desolation)
            SeasonChangePacket p = (SeasonChangePacket)packet;
            ClientVars.EngineVars.Season = p.Season;
        }

        private static void receive_SellList(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_ServerList(IRecvPacket packet)
        {
            ClientVars.Servers.List = ((ServerListPacket)packet).Servers;
            Status = UltimaClientStatus.LoginServer_HasServerList;
        }

        private static void receive_SetWeather(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_ServerRelay(IRecvPacket packet)
        {
            ServerRelayPacket p = (ServerRelayPacket)packet;
            // Normally, upon receiving this packet you would disconnect and
            // log in to the specified server. Since we are using RunUO, we don't
            // actually need to do this.
            _ClientNetwork.IsDecompressionEnabled = true;
            Send(new GameLoginPacket(p.AccountId, _account, _password));
        }

        private static void receive_SkillsList(IRecvPacket packet)
        {
            foreach (SendSkillsPacket_SkillEntry skill in ((SendSkillsPacket)packet).Skills)
            {
                ClientVars.SkillEntry entry = ClientVars.Skills.SkillEntry(skill.SkillID);
                entry.Value = skill.SkillValue;
                entry.ValueUnmodified = skill.SkillValueUnmodified;
                entry.LockType = skill.SkillLock;
                entry.Cap = skill.SkillCap;
            }
        }

        private static void receive_AsciiMessage(IRecvPacket packet)
        {
            AsciiMessagePacket p = (AsciiMessagePacket)packet;
            receive_TextMessage(p.MsgType, p.Text, p.Hue, p.Font, p.Serial, p.Name1);
        }

        private static void receive_StatusInfo(IRecvPacket packet)
        {
            MobileStatusCompactPacket p = (MobileStatusCompactPacket)packet;

            if (p.StatusType >= 6)
            {
                throw (new Exception("KR Status not handled."));
            }

            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
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

        private static void receive_TargetCursor(IRecvPacket packet)
        {
            TargetCursorPacket p = (TargetCursorPacket)packet;
            GameState.MouseTargeting((TargetTypes)p.CommandType, p.CursorID);
        }

        private static void receive_TargetCursorMulti(IRecvPacket packet)
        {
            TargetCursorMultiPacket p = (TargetCursorMultiPacket)packet;
            GameState.MouseTargeting(TargetTypes.MultiPlacement, 0);
            _LegacyUI.Cursor.TargetingMulti = p.MultiModel;
        }

        private static void receive_Time(IRecvPacket packet)
        {
            TimePacket p = (TimePacket)packet;
            _LegacyUI.AddMessage_Chat(string.Format("The current server time is {0}:{1}:{2}", p.Hour, p.Minute, p.Second));
        }

        private static void receive_TipNotice(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private static void receive_ToolTipRevision(IRecvPacket packet)
        {
            ObjectPropertyListUpdatePacket p = (ObjectPropertyListUpdatePacket)packet;
            Entity iObject = EntitiesCollection.GetObject<Entity>(p.Serial, false);
            if (iObject != null)
            {
                if (iObject.PropertyList.Hash != p.RevisionHash)
                {
                    Send(new QueryPropertiesPacket(p.Serial));
                }
            }
        }

        private static void receive_UnicodeMessage(IRecvPacket packet)
        {
            UnicodeMessagePacket p = (UnicodeMessagePacket)packet;
            receive_TextMessage(p.MsgType, p.SpokenText, p.Hue, p.Font, p.Serial, p.SpeakerName);
        }

        private static void receive_UpdateHealth(IRecvPacket packet)
        {
            UpdateHealthPacket p = (UpdateHealthPacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            u.Health.Update(p.Current, p.Max);
        }

        private static void receive_UpdateMana(IRecvPacket packet)
        {
            UpdateManaPacket p = (UpdateManaPacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            u.Mana.Update(p.Current, p.Max);
        }

        private static void receive_UpdateStamina(IRecvPacket packet)
        {
            UpdateStaminaPacket p = (UpdateStaminaPacket)packet;
            Mobile u = EntitiesCollection.GetObject<Mobile>(p.Serial, false);
            u.Stamina.Update(p.Current, p.Max);
        }

        private static void receive_VersionRequest(IRecvPacket packet)
        {
            // Automatically respond.
            Interaction.SendClientVersion();
        }

        static void receive_WarMode(IRecvPacket packet)
        {
            WarModePacket p = (WarModePacket)packet;
            ClientVars.EngineVars.WarMode = p.WarMode;
        }

        static void receive_WorldItem(IRecvPacket packet)
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
                item.Facing = (Direction)p.Direction;
            }
            else
            {
                // create a multi object. Unhandled !!!
                int multiID = p.ItemID - 0x4000;
                Multi multi = EntitiesCollection.GetObject<Multi>(p.Serial, true);
                multi.ItemID = p.ItemID;
                multi.X = p.X;
                multi.Y = p.Y;
                multi.Z = p.Z;
            }
        }

        static void receive_WornItem(IRecvPacket packet)
        {
            WornItemPacket p = (WornItemPacket)packet;
            Item item = add_Item(p.Serial, p.ItemId, p.Hue, p.ParentSerial, 0);
            Mobile m = EntitiesCollection.GetObject<Mobile>(p.ParentSerial, false);
            m.WearItem(item, p.Layer);
            if (item.PropertyList.Hash == 0)
                Send(new QueryPropertiesPacket(item.Serial));
        }



        static void announce_UnhandledPacket(IRecvPacket packet)
        {
            Diagnostics.Logger.Warn(string.Format("Client: Unhandled {0} [ID:{1}]", packet.Name, packet.Id));
        }

        static void announce_UnhandledPacket(IRecvPacket packet, string addendum)
        {
            Diagnostics.Logger.Warn(string.Format("Client: Unhandled {0} [ID:{1}] {2}]", packet.Name, packet.Id, addendum));
        }

        private static void receive_TextMessage(MessageType msgType, string text, int hue, int font, Serial serial, string speakerName)
        {
            Overhead overhead;
            switch (msgType)
            {
                case MessageType.Regular:
                    overhead = EntitiesCollection.AddOverhead(msgType, serial, text, font, hue);
                    if (overhead != null)
                    {
                        _LegacyUI.AddMessage_Chat(speakerName + ": " + text, font, hue);
                        overhead.SpeakerName = speakerName;
                    }
                    else
                    {
                        _LegacyUI.AddMessage_Chat(text, font, hue);
                    }
                    break;
                case MessageType.System:
                    _LegacyUI.AddMessage_Chat("<SYSTEM> " + text, font, hue);
                    break;
                case MessageType.Emote:
                    _LegacyUI.AddMessage_Chat("<EMOTE> " + text, font, hue);
                    break;
                case MessageType.Label:
                    if (serial.IsValid)
                    {
                        overhead = EntitiesCollection.AddOverhead(msgType, serial, text, font, hue);
                        overhead.SpeakerName = speakerName;
                        // Labels that are longer than the current name should be set as the name
                        if (serial.IsMobile)
                        {
                            Mobile m = EntitiesCollection.GetObject<Mobile>(serial, false);
                            if (m.Name.Length < text.Length)
                                m.Name = text;
                        }
                    }
                    else
                    {
                        _LegacyUI.AddMessage_Chat("<LABEL> " + text, font, hue);
                    }
                    break;
                case MessageType.Focus: // on player?
                    _LegacyUI.AddMessage_Chat("<FOCUS> " + text, font, hue);
                    break;
                case MessageType.Whisper:
                    _LegacyUI.AddMessage_Chat("<WHISPER> " + text, font, hue);
                    break;
                case MessageType.Yell:
                    _LegacyUI.AddMessage_Chat("<YELL> " + text, font, hue);
                    break;
                case MessageType.Spell:
                    _LegacyUI.AddMessage_Chat("<SPELL> " + text, font, hue);
                    break;
                case MessageType.UIld:
                    _LegacyUI.AddMessage_Chat("<UILD> " + text, font, hue);
                    break;
                case MessageType.Alliance:
                    _LegacyUI.AddMessage_Chat("<ALLIANCE> " + text, font, hue);
                    break;
                case MessageType.Command:
                    _LegacyUI.AddMessage_Chat("<COMMAND> " + text, font, hue);
                    break;
                default:
                    _LegacyUI.AddMessage_Chat("ERROR UNKNOWN COMMAND:" + msgType.ToString());
                    break;
            }
        }

        private static void parseContextMenu(Client.Packets.ContextMenu context)
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

        private static string[] interpretGumpPieces(string gumpData)
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

        private static string constructCliLoc(string nBase, string nArgs)
        {
            string[] iArgs = nArgs.Split('\t');
            for (int i = 0; i < iArgs.Length; i++)
            {
                if ((iArgs[i].Length > 0) && (iArgs[i].Substring(0, 1) == "#"))
                {
                    int clilocID = Convert.ToInt32(iArgs[i].Substring(1));
                    iArgs[i] = Data.StringList.Entry(clilocID);
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

        private static Item add_Item(Serial serial, int itemID, int nHue, Serial parentSerial, int amount)
        {
            Item item;
            if (itemID == 0x2006)
            {
                // special case for corpses.
                item = EntitiesCollection.GetObject<Corpse>((int)serial, true);
                Packets.Server.ContainerContentPacket.NextContainerContentsIsPre6017 = true;
            }
            else
            {
                if (Data.TileData.ItemData[itemID].Container)
                    item = EntitiesCollection.GetObject<Container>((int)serial, true);
                else
                    item = EntitiesCollection.GetObject<Item>((int)serial, true);
            }
            item.Amount = amount;
            item.ItemID = itemID;
            item.Hue = nHue;
            return item;
        }
    }
}
