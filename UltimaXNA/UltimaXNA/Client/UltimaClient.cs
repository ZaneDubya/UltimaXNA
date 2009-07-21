/***************************************************************************
 *   UltimaClient.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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
using UltimaXNA.Network.Packets.Client;
using UltimaXNA.Network.Packets.Server;
#endregion

namespace UltimaXNA.Client
{
    class UltimaClient : GameComponent, IUltimaClient
    {
        public UltimaClientStatus Status { get; protected set; }

        private string _Account; private string _Password;
        public void SetAccountPassword(string account, string password) { _Account = account; _Password = password; }
        private void clearAccountPassword() { _Account = string.Empty; _Password = string.Empty; }

        private UltimaXNA.Network.ClientNetwork _ClientNetwork;
        private UltimaXNA.GUI.IGUI _GUI;
        private IEntitiesService _Entities;
        private UltimaXNA.IGameState _GameState;

        public UltimaClient(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IUltimaClient), this);
            Status = UltimaClientStatus.Unconnected;
        }

        public override void Initialize()
        {
            _GUI = Game.Services.GetService(typeof(GUI.IGUI)) as GUI.IGUI;
            _Entities = Game.Services.GetService(typeof(IEntitiesService)) as IEntitiesService;
            _GameState = Game.Services.GetService(typeof(IGameState)) as IGameState;

            _ClientNetwork = new ClientNetwork();
            registerPackets();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _ClientNetwork.Update();
        }

        private void registerPackets()
        {
            PacketRegistry.OnDamage += receive_Damage;
            PacketRegistry.OnMobileStatusCompact += receive_StatusInfo;
            PacketRegistry.OnWorldItem += receive_WorldItem;
            PacketRegistry.OnLoginConfirm += receive_PlayerLocaleAndBody;
            PacketRegistry.OnAsciiMessage += receive_AsciiMessage;
            PacketRegistry.OnRemoveEntity += receive_DeleteObject;
            PacketRegistry.OnMobileUpdate += receive_MobileUpdate;
            PacketRegistry.OnMovementRejected += receive_MoveRej;
            PacketRegistry.OnMoveAcknowledged += receive_MoveAck;
            PacketRegistry.OnDragEffect += receive_DragItem;
            PacketRegistry.OnOpenContainer += receive_Container;
            PacketRegistry.OnContainerContentUpdate += receive_AddSingleItemToContainer;
            PacketRegistry.OnLiftRejection += receive_RejectMoveItemRequest;
            PacketRegistry.OnResurrectMenu += receive_ResurrectionMenu;
            PacketRegistry.OnMobileAttributes += receive_MobileAttributes;
            PacketRegistry.OnWornItem += receive_WornItem;
            PacketRegistry.OnSwing += receive_OnSwing;
            PacketRegistry.OnSkillsList += receive_SkillsList;
            PacketRegistry.OnContainerContent += receive_AddMultipleItemsToContainer;
            PacketRegistry.OnPersonalLightLevel += receive_PersonalLightLevel;
            PacketRegistry.OnOverallLightLevel += receive_OverallLightLevel;
            PacketRegistry.OnPopupMessage += receive_PopupMessage;
            PacketRegistry.OnPlaySoundEffect += receive_PlaySoundEffect;
            PacketRegistry.OnLoginComplete += receive_LoginComplete;
            PacketRegistry.OnTime += receive_Time;
            PacketRegistry.OnWeather += receive_SetWeather;
            PacketRegistry.OnTargetCursor += receive_TargetCursor;
            PacketRegistry.OnPlayMusic += receive_PlayMusic;
            PacketRegistry.OnMobileAnimation += receive_MobileAnimation;
            PacketRegistry.OnGraphicalEffect1 += receive_GraphicEffect;
            PacketRegistry.OnWarMode += receive_WarMode;
            PacketRegistry.OnVendorBuyList += receive_OpenBuyWindow;
            PacketRegistry.OnNewSubserver += receive_NewSubserver;
            PacketRegistry.OnMobileMoving += receive_MobileMoving;
            PacketRegistry.OnMobileIncoming += receive_MobileIncoming;
            PacketRegistry.OnDisplayMenu += receive_DisplayMenu;
            PacketRegistry.OnLoginRejection += receive_LoginRejection;
            PacketRegistry.OnCharacterListUpdate += receive_CharacterListUpdate;
            PacketRegistry.OnOpenPaperdoll += receive_OpenPaperdoll;
            PacketRegistry.OnCorpseClothing += receive_CorpseClothing;
            PacketRegistry.OnServerRelay += receive_ServerRelay;
            PacketRegistry.OnPlayerMove += receive_PlayerMove;
            PacketRegistry.OnRequestNameResponse += receive_RequestNameResponse;
            PacketRegistry.OnVendorSellList += receive_SellList;
            PacketRegistry.OnUpdateCurrentHealth += receive_UpdateHealth;
            PacketRegistry.OnUpdateCurrentMana += receive_UpdateMana;
            PacketRegistry.OnUpdateCurrentStamina += receive_UpdateStamina;
            PacketRegistry.OnOpenWebBrowser += receive_OpenWebBrowser;
            PacketRegistry.OnTipNoticeWindow += receive_TipNotice;
            PacketRegistry.OnServerList += receive_ServerList;
            PacketRegistry.OnCharactersStartingLocations += receive_CharacterList;
            PacketRegistry.OnChangeCombatant += receive_ChangeCombatant;
            PacketRegistry.OnUnicodeMessage += receive_UnicodeMessage;
            PacketRegistry.OnDeathAnimation += receive_DeathAnimation;
            PacketRegistry.OnDisplayGumpFast += receive_DisplayGumpFast;
            PacketRegistry.OnObjectHelpResponse += receive_ObjectHelpResponse;
            PacketRegistry.OnSupportedFeatures += receive_EnableFeatures;
            PacketRegistry.OnQuestArrow += receive_QuestArrow;
            PacketRegistry.OnSeasonChange += receive_SeasonalInformation;
            PacketRegistry.OnVersionRequest += receive_VersionRequest;
            PacketRegistry.OnGeneralInfo += receive_GeneralInfo;
            PacketRegistry.OnHuedEffect += receive_HuedEffect;
            PacketRegistry.OnMessageLocalized += receive_CLILOCMessage;
            PacketRegistry.OnInvalidMapEnable += receive_InvalidMapEnable;
            PacketRegistry.OnParticleEffect += receive_OnParticleEffect;
            PacketRegistry.OnGlobalQueueCount += receive_GlobalQueueCount;
            PacketRegistry.OnMessageLocalizedAffix += receive_MessageLocalizedAffix;
            PacketRegistry.OnExtended0x78 += receive_Extended0x78;
            PacketRegistry.OnMegaCliloc += receive_ObjectPropertyList;
            PacketRegistry.OnToolTipRevision += receive_ToolTipRevision;
            PacketRegistry.OnCompressedGump += receive_CompressedGump;

            PacketRegistry.RegisterNetwork(_ClientNetwork);
        }

        public bool Connect(string ipAddressOrHostName, int port)
        {
            bool success = _ClientNetwork.Connect(ipAddressOrHostName, port);
            if (success)
            {
                Status = UltimaClientStatus.LoginServer_Connecting;
                _ClientNetwork.Send(new SeedPacket(1, 6, 0, 6, 2));
            }
            return success;
        }

        public void Disconnect()
        {
            if (_ClientNetwork.IsConnected)
                _ClientNetwork.Disconnect();
            Status = UltimaClientStatus.Unconnected;
            clearAccountPassword();
        }

        public void Send(ISendPacket packet)
        {
            bool success = _ClientNetwork.Send(packet);
            if (!success)
            {
                this.Disconnect();
            }
        }











        private void receive_AddMultipleItemsToContainer(IRecvPacket packet)
        {
            ContainerContentPacket p = (ContainerContentPacket)packet;
            foreach (ContentItem i in p.Items)
            {
                // Add the item...
                Item iObject = add_Item(i.Serial, i.ItemID, i.Hue, i.ContainerSerial, i.Amount);
                iObject.Item_InvX = i.X;
                iObject.Item_InvY = i.Y;
                // ... and add it the container contents of the container.
                ContainerItem iContainerObject = _Entities.GetObject<ContainerItem>(i.ContainerSerial, true);
                iContainerObject.Contents.AddItem(iObject);
            }
        }

        private void receive_AddSingleItemToContainer(IRecvPacket packet)
        {
            ContainerContentUpdatePacket p = (ContainerContentUpdatePacket)packet;

            // Add the item...
            Item iObject = add_Item(p.Serial, p.ItemId, p.Hue, p.ContainerSerial, p.Amount);
            iObject.Item_InvX = p.X;
            iObject.Item_InvY = p.Y;
            // ... and add it the container contents of the container.
            ContainerItem iContainerObject = _Entities.GetObject<ContainerItem>(p.ContainerSerial, true);
            if (iContainerObject != null)
                iContainerObject.Contents.AddItem(iObject);
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

        private void receive_CharacterListUpdate(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_CLILOCMessage(IRecvPacket packet)
        {
            MessageLocalizedPacket p = (MessageLocalizedPacket)packet;

            string iCliLoc = constructCliLoc(Data.StringList.Table[p.CliLocNumber].ToString(), p.Arguements);
            GUI.GUIHelper.Chat_AddLine(iCliLoc);
        }

        private void receive_ChangeCombatant(IRecvPacket packet)
        {
            ChangeCombatantPacket p = (ChangeCombatantPacket)packet;
            _GameState.LastTarget = p.Serial;
        }

        private void receive_CharacterList(IRecvPacket packet)
        {
            byte[] iIPAdress = new byte[4] { 127, 0, 0, 1 };
            int iAddress = BitConverter.ToInt32(iIPAdress, 0);
            CharacterCityListPacket p = (CharacterCityListPacket)packet;
            this.Send(new LoginCharacterPacket(p.Characters[0].Name, 0, iAddress));
        }

        private void receive_CompressedGump(IRecvPacket packet)
        {
            CompressedGumpPacket p = (CompressedGumpPacket)packet;
            string[] gumpPieces = interpretGumpPieces(p.GumpData);
            _GUI.AddWindow("Gump:" + p.GumpID, new GUI.Window_CompressedGump(p.GumpID, gumpPieces, p.TextLines, p.X, p.Y));
        }

        private void receive_Container(IRecvPacket packet)
        {
            OpenContainerPacket p = (OpenContainerPacket)packet;

            // We can safely ignore 0x30 - this is the buy window.
            if (p.GumpId == 0x30)
                return;

            // Only try to open a container of type Container. Note that GameObjects can
            // have container objects and will expose them when called through GetContainerObject(int)
            // instead of GetObject(int).
            ContainerItem o = _Entities.GetObject<ContainerItem>(p.Serial, false);
            if (o is Item)
            {
                _GUI.Container_Open(o, p.GumpId);
            }
            else
            {
                throw (new Exception("This Object has no support for a container object!"));
            }
        }

        private void receive_CorpseClothing(IRecvPacket packet)
        {
            CorpseClothingPacket p = (CorpseClothingPacket)packet;
            Corpse e = _Entities.GetObject<Corpse>(p.CorpseSerial, false);
            e.LoadCorpseClothing(p.Items);
        }

        private void receive_Damage(IRecvPacket packet)
        {
            DamagePacket p = (DamagePacket)packet;
            Mobile u = _Entities.GetObject<Mobile>(p.Serial, false);
            GUI.GUIHelper.Chat_AddLine(u.Name + " takes " + p.Damage + " damage!");
        }

        private void receive_DeathAnimation(IRecvPacket packet)
        {
            DeathAnimationPacket p = (DeathAnimationPacket)packet;
            Mobile u = _Entities.GetObject<Mobile>(p.PlayerSerial, false);
            Corpse c = _Entities.GetObject<Corpse>(p.CorpseSerial, false);
            c.Movement.Facing = u.Movement.Facing;
            c.MobileSerial = p.PlayerSerial;
            c.DeathAnimation();
        }

        private void receive_DeleteObject(IRecvPacket packet)
        {
            RemoveEntityPacket p = (RemoveEntityPacket)packet;
            _Entities.RemoveObject(p.Serial);
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
            announce_UnhandledPacket(packet);
        }

        private void receive_Extended0x78(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_GeneralInfo(IRecvPacket packet)
        {
            GeneralInfoPacket p = (GeneralInfoPacket)packet;
            switch (p.Subcommand)
            {
                case 0x06: // party system
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x08: // Set cursor color / set map
                    // p.MapID_MapID;
                    // 0 = Felucca, cursor not colored / BRITANNIA map
                    // 1 = Trammel, cursor colored gold / BRITANNIA map
                    // 2 = (switch to) ILSHENAR map
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x14: // return context menu
                    parseContextMenu(p.ContextMenu);
                    break;
                case 0x18: // Number of maps
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x1D: // House revision
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x04: // Close generic gump
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                case 0x19: // Extended stats: http://docs.polserver.com/packets/index.php?Packet=0xBF
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
                default:
                    announce_UnhandledPacket(packet, "subcommand " + p.Subcommand);
                    break;
            }
        }

        private void receive_GlobalQueueCount(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_GraphicEffect(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_HuedEffect(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_InvalidMapEnable(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_LoginComplete(IRecvPacket packet)
        {
            // This packet is just one byte, the opcode.
            // Congrats, login complete!

            // We want to make sure we have the client object before we load the world.
            // If we don't, just set the status to login complete, which will then
            // load the world when we finally receive our client object.
            if (_Entities.MySerial != 0)
                Status = UltimaClientStatus.WorldServer_InWorld;
            else
                Status = UltimaClientStatus.WorldServer_LoginComplete;
        }

        private void receive_LoginRejection(IRecvPacket packet)
        {
            Disconnect();
            LoginRejectionPacket p = (LoginRejectionPacket)packet;
            _GUI.Reset();
            _GUI.ErrorPopup_Modal(p.Reason);
        }

        private void receive_MessageLocalizedAffix(IRecvPacket packet)
        {
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

            Mobile iObject = _Entities.GetObject<Mobile>(p.Serial, false);
            iObject.Animation(p.Action, p.FrameCount, p.RepeatCount, p.Reverse, p.Repeat, p.Delay);
        }

        private void receive_MobileIncoming(IRecvPacket packet)
        {
            MobileIncomingPacket p = (MobileIncomingPacket)packet;
            Mobile iMobile = _Entities.GetObject<Mobile>(p.Serial, true);
            iMobile.BodyID = p.BodyID;
            iMobile.Hue = (int)p.Hue;
            iMobile.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z, p.Direction);
            iMobile.IsWarMode = p.Flags.IsWarMode;

            for (int i = 0; i < p.Equipment.Length; i++)
            {
                Item item = add_Item(p.Equipment[i].Serial, p.Equipment[i].GumpId, p.Equipment[i].Hue, 0, 0);
                iMobile.equipment[p.Equipment[i].Layer] = item;
                if (item.PropertyList.Hash == 0)
                    this.Send(new QueryPropertiesPacket(item.Serial));
            }

            if (iMobile.Name == string.Empty)
            {
                iMobile.Name = "Unknown";
                this.Send(new RequestNamePacket(p.Serial));
            }
        }

        private void receive_MobileMoving(IRecvPacket packet)
        {
            MobileMovingPacket p = (MobileMovingPacket)packet;

            Mobile iObject = _Entities.GetObject<Mobile>(p.Serial, true);
            iObject.BodyID = p.BodyID;
            iObject.IsWarMode = p.Flags.IsWarMode;
            // Issue 16 - Pet not showing at login - http://code.google.com/p/ultimaxna/issues/detail?id=16 - Smjert
            // Since no packet arrives to add your pet, when you move and your pet follows you the client crashes
            if (iObject.Movement.DrawPosition == null)
            {
                iObject.Movement.SetPositionInstant(p.X, p.Y, p.Z, p.Direction);
            }
            else if (iObject.Movement.DrawPosition.PositionV3 == new Vector3(0, 0, 0))
            {
                iObject.Movement.SetPositionInstant(p.X, p.Y, p.Z, p.Direction);
                // Issue 16 - End
            }
            else
            {
                iObject.Move(p.X, p.Y, p.Z, p.Direction);
            }
        }

        private void receive_MobileUpdate(IRecvPacket packet)
        {
            MobileUpdatePacket p = (MobileUpdatePacket)packet;
            Mobile iObject = _Entities.GetObject<Mobile>(p.Serial, true);
            iObject.BodyID = p.BodyID;
            iObject.Hue = (int)p.Hue;
            iObject.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z, p.Direction);

            if (iObject.Name == string.Empty)
            {
                iObject.Name = "Unknown";
                this.Send(new RequestNamePacket(p.Serial));
            }
        }

        private void receive_MoveAck(IRecvPacket packet)
        {
            MoveAcknowledgePacket p = (MoveAcknowledgePacket)packet;
            _Entities.GetPlayerObject().Movement.MoveEventAck(p.Sequence);
        }

        private void receive_MoveRej(IRecvPacket packet)
        {
            MovementRejectPacket p = (MovementRejectPacket)packet;
            _Entities.GetPlayerObject().Movement.MoveEventRej(p.Sequence, p.X, p.Y, p.Z, p.Direction);
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

            Entity iObject = _Entities.GetObject<Entity>(p.Serial, false);
            iObject.PropertyList.Hash = p.Hash;
            iObject.PropertyList.Clear();

            for (int i = 0; i < p.CliLocs.Count; i++)
            {
                string iCliLoc = Data.StringList.Table[p.CliLocs[i]].ToString();
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

        private void receive_OnParticleEffect(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_OnSwing(IRecvPacket packet)
        {
            SwingPacket p = (SwingPacket)packet;
            if (p.Attacker == _Entities.MySerial)
            {
                _GameState.LastTarget = p.Defender;
            }
        }

        private void receive_OpenBuyWindow(IRecvPacket packet)
        {
            VendorBuyListPacket p = (VendorBuyListPacket)packet;
            Item iObject = _Entities.GetObject<Item>(p.VendorPackSerial, false);
            if (iObject == null)
                return;
            _GUI.Merchant_Open(iObject, 0);
        }

        private void receive_OpenPaperdoll(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_OpenWebBrowser(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_OverallLightLevel(IRecvPacket packet)
        {
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F
            // !!! Unhandled
            announce_UnhandledPacket(packet);
        }

        private void receive_PersonalLightLevel(IRecvPacket packet)
        {
            // int iCreatureID = reader.ReadInt();
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F
            // !!! Unhandled
            announce_UnhandledPacket(packet);
        }

        private void receive_PlayerLocaleAndBody(IRecvPacket packet)
        {
            LoginConfirmPacket p = (LoginConfirmPacket)packet;

            // When loading the player object, we must load the serial before the object.
            _Entities.MySerial = p.Serial;
            PlayerMobile iPlayer = _Entities.GetObject<PlayerMobile>(p.Serial, true);
            iPlayer.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z, p.Direction);
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
            announce_UnhandledPacket(packet);
        }

        private void receive_PlaySoundEffect(IRecvPacket packet)
        {
            PlaySoundEffectPacket p = (PlaySoundEffectPacket)packet;
            Data.Sounds.PlaySound(p.SoundModel);
        }

        private void receive_PopupMessage(IRecvPacket packet)
        {
            PopupMessagePacket p = (PopupMessagePacket)packet;
            announce_UnhandledPacket(packet);
        }

        private void receive_QuestArrow(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_RejectMoveItemRequest(IRecvPacket packet)
        {
            LiftRejectionPacket p = (LiftRejectionPacket)packet;
            GUI.GUIHelper.Chat_AddLine("Could not pick up item: " + p.ErrorMessage);
        }

        private void receive_RequestNameResponse(IRecvPacket packet)
        {
            RequestNameResponsePacket p = (RequestNameResponsePacket)packet;
            Mobile u = _Entities.GetObject<Mobile>(p.Serial, false);
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
            // Unhandled!!! If iSeason2 = 1, then this is a season change.
            // If season change, then iSeason1 = (0=spring, 1=summer, 2=fall, 3=winter, 4 = desolation)
            announce_UnhandledPacket(packet);
        }

        private void receive_SellList(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_ServerList(IRecvPacket packet)
        {
            ServerListPacket p = (ServerListPacket)packet;
            this.Send(new SelectServerPacket(p.Servers[0].Index));
        }

        private void receive_SetWeather(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_ServerRelay(IRecvPacket packet)
        {
            ServerRelayPacket p = (ServerRelayPacket)packet;
            // Normally, upon receiving this packet you would disconnect and
            // log in to the specified server. Since we are using RunUO, we don't
            // actually need to do this.
            _ClientNetwork.IsDecompressionEnabled = true;
            this.Send(new GameLoginPacket(p.AccountId, _Account, _Password));
            clearAccountPassword();
        }

        private void receive_SkillsList(IRecvPacket packet)
        {
            int iPacketType = 0; // reader.ReadByte();
            // 0x00: Full List of skills
            // 0xFF: Single skill update
            // 0x02: Full List of skills with skill cap for each skill
            // 0xDF: Single skill update with skill cap for skill

            switch (iPacketType)
            {
                case 0x00: //Full List of skills
                    announce_UnhandledPacket(packet, "0x00");
                    break;
                case 0xFF: // single skill update
                    announce_UnhandledPacket(packet, "0xFF");
                    break;
                case 0x02: // full List of skills with skill cap for each skill
                    announce_UnhandledPacket(packet, "0x02");
                    break;
                case 0xDF: // Single skill update with skill cap for skill
                    announce_UnhandledPacket(packet, "0xDF");
                    break;
                default:
                    announce_UnhandledPacket(packet, "Unknown subcommand " + iPacketType);
                    break;
            }
        }

        private void receive_AsciiMessage(IRecvPacket packet)
        {
            AsciiMessagePacket p = (AsciiMessagePacket)packet;

            switch (p.Type)
            {
                case 0x00: // 0x00 - Normal 
                    GUI.GUIHelper.Chat_AddLine(p.Name1 + ": " + p.Text);
                    break;
                default:
                    announce_UnhandledPacket(packet);
                    break;
            }
            // The various types of text is as follows:
            // 0x00 - Normal 
            // 0x01 - Broadcast/System
            // 0x02 - Emote 
            // 0x06 - System/Lower Corner
            // 0x07 - Message/Corner With Name
            // 0x08 - Whisper
            // 0x09 - Yell
            // 0x0A - Spell
            // 0x0D - Guild Chat
            // 0x0E - Alliance Chat
            // 0x0F - Command Prompts
        }

        private void receive_StatusInfo(IRecvPacket packet)
        {
            MobileStatusCompactPacket p = (MobileStatusCompactPacket)packet;

            if (p.StatusType >= 6)
            {
                throw (new Exception("KR Status not handled."));
            }

            Mobile u = _Entities.GetObject<Mobile>(p.Serial, false);
            u.Name = p.PlayerName;
            u.Health.Update(p.CurrentHealth, p.MaxHealth);
            u.Stamina.Update(p.CurrentStamina, p.MaxStamina);
            u.Mana.Update(p.CurrentMana, p.MaxMana);
            //  other stuff unhandled !!!
        }

        private void receive_TargetCursor(IRecvPacket packet)
        {
            TargetCursorPacket p = (TargetCursorPacket)packet;
            _GameState.MouseTargeting(p.CursorID, p.CommandType);
        }

        private void receive_Time(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_TipNotice(IRecvPacket packet)
        {
            announce_UnhandledPacket(packet);
        }

        private void receive_ToolTipRevision(IRecvPacket packet)
        {
            ObjectPropertyListUpdatePacket p = (ObjectPropertyListUpdatePacket)packet;
            Entity iObject = _Entities.GetObject<Entity>(p.Serial, false);
            if (iObject != null)
            {
                if (iObject.PropertyList.Hash != p.RevisionHash)
                {
                    this.Send(new QueryPropertiesPacket(p.Serial));
                }
            }
        }

        private void receive_UnicodeMessage(IRecvPacket packet)
        {
            UnicodeMessagePacket p = (UnicodeMessagePacket)packet;
            GUI.GUIHelper.Chat_AddLine(p.SpeakerName + ": " + p.SpokenText);
        }

        private void receive_UpdateHealth(IRecvPacket packet)
        {
            UpdateHealthPacket p = (UpdateHealthPacket)packet;
            Mobile u = _Entities.GetObject<Mobile>(p.Serial, false);
            u.Health.Update(p.Current, p.Max);
        }

        private void receive_UpdateMana(IRecvPacket packet)
        {
            UpdateManaPacket p = (UpdateManaPacket)packet;
            Mobile u = _Entities.GetObject<Mobile>(p.Serial, false);
            u.Mana.Update(p.Current, p.Max);
        }

        private void receive_UpdateStamina(IRecvPacket packet)
        {
            UpdateStaminaPacket p = (UpdateStaminaPacket)packet;
            Mobile u = _Entities.GetObject<Mobile>(p.Serial, false);
            u.Stamina.Update(p.Current, p.Max);
        }

        private void receive_VersionRequest(IRecvPacket packet)
        {
            // Automatically respond.
            this.Send(new ClientVersionPacket("6.0.6.2"));
        }

        private void receive_WarMode(IRecvPacket packet)
        {
            WarModePacket p = (WarModePacket)packet;
            _GameState.WarMode = p.WarMode;
        }

        private void receive_WorldItem(IRecvPacket packet)
        {
            WorldItemPacket p = (WorldItemPacket)packet;
            // Now create the GameObject.
            // If the iItemID < 0x4000, this is a regular game object.
            // If the iItemID >= 0x4000, then this is a multiobject.
            if (p.ItemID <= 0x4000)
            {
                Item item = add_Item(p.Serial, p.ItemID, p.Hue, unchecked((int)0xFFFFFFFF), p.StackAmount);
                item.Movement.SetPositionInstant(p.X, p.Y, p.Z, p.Direction);
            }
            else
            {
                // create a multi object. Unhandled !!!
                announce_UnhandledPacket(packet);
            }
        }

        private void receive_WornItem(IRecvPacket packet)
        {
            WornItemPacket p = (WornItemPacket)packet;
            Item item = add_Item(p.Serial, p.ItemId, p.Hue, 0, 0);
            Mobile u = _Entities.GetObject<Mobile>(p.ParentSerial, false);
            u.equipment[p.Layer] = item;
            if (item.PropertyList.Hash == 0)
                this.Send(new QueryPropertiesPacket(item.Serial));
        }






        private void announce_UnhandledPacket(IRecvPacket packet)
        {
            GUI.GUIHelper.Chat_AddLine("DEBUG: Unhandled " + packet.Name + ". <" + packet.Id + ">");
        }

        private void announce_UnhandledPacket(IRecvPacket packet, string addendum)
        {
            GUI.GUIHelper.Chat_AddLine("DEBUG: Unhandled " + packet.Name + ". <" + packet.Id + ">" + " " + addendum);
        }

        private void announce_Packet(IRecvPacket packet)
        {
            // GUI.GUIHelper.Chat_AddLine("DEBUG: Recv'd " + packet.Name + ". <" + packet.Id + ">");
        }

        private void parseContextMenu(ContextMenuNew context)
        {
            if (context.HasContextMenu)
            {
                if (context.CanBuy)
                {
                    this.Send(new ContextMenuResponsePacket(context.Serial, (short)context.ContextEntry("Buy").ResponseCode));
                }
            }
            else
            {
                // no context menu entries are handled. Send a double click.
                this.Send(new DoubleClickPacket(context.Serial));
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

        private Item add_Item(Serial serial, int itemID, int nHue, int containerSerial, int amount)
        {
            Item item;
            if (itemID == 0x2006)
            {
                // special case for corpses.
                item = _Entities.GetObject<Corpse>((int)serial, true);
            }
            else
            {
                if (Data.TileData.ItemData[itemID].Container)
                    item = _Entities.GetObject<ContainerItem>((int)serial, true);
                else
                    item = _Entities.GetObject<Item>((int)serial, true);
            }
            item.ItemID = itemID;
            item.Hue = nHue;
            item.Item_StackCount = amount;
            item.Item_ContainedWithinSerial = containerSerial;

            return item;
        }
    }
}
