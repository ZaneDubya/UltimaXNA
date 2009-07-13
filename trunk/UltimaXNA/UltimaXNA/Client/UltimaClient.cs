#region File Description & Usings
//-----------------------------------------------------------------------------
// UltimaClient.cs
//
// Part of UltimaXNA
//-----------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.GameObjects;
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
        private IGameObjects _GameObjects;
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
            _GameObjects = Game.Services.GetService(typeof(GameObjects.IGameObjects)) as GameObjects.IGameObjects;
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
            PacketRegistry.OnAsciiMessage += receive_Speech;
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
                GameObject iObject = addItem(i.Serial, i.ItemID, i.Hue, i.ContainerSerial, i.Amount);
                iObject.Item_InvX = i.X;
                iObject.Item_InvY = i.Y;
                // ... and add it the container contents of the container.
                GameObject iContainerObject = _GameObjects.GetObject<GameObjects.GameObject>(i.ContainerSerial, true);
                iContainerObject.ContainerObject.AddItem(iObject);
            }
        }

        private void receive_AddSingleItemToContainer(IRecvPacket packet)
        {
            ContainerContentUpdatePacket p = (ContainerContentUpdatePacket)packet;

            // Add the item...
            GameObjects.GameObject iObject = addItem(p.Serial, p.ItemId, p.Hue, p.ContainerSerial, p.Amount);
            iObject.Item_InvX = p.X;
            iObject.Item_InvY = p.Y;
            // ... and add it the container contents of the container.
            GameObject iContainerObject = _GameObjects.GetObject<GameObjects.GameObject>(p.ContainerSerial, true);
            if (iContainerObject != null)
                iContainerObject.ContainerObject.AddItem(iObject);
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
            receive_UnhandledPacket(packet);
        }

        private void receive_CLILOCMessage(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_ChangeCombatant(IRecvPacket packet)
        {
            ChangeCombatantPacket p = (ChangeCombatantPacket)packet;
            _GameState.LastTarget = p.Id;
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
            receive_UnhandledPacket(packet);
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
            GameObjects.GameObject o = _GameObjects.GetObject<GameObjects.GameObject>(p.Serial, false);
            if (o.ObjectType == ObjectType.GameObject)
            {
                _GUI.Container_Open(o, p.GumpId);
            }
            else
            {
                throw (new Exception("No support for container object!"));
            }
        }

        private void receive_CorpseClothing(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_Damage(IRecvPacket packet)
        {
            DamagePacket p = (DamagePacket)packet;
            receive_UnhandledPacket(packet);
        }

        private void receive_DeathAnimation(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_DeleteObject(IRecvPacket packet)
        {
            RemoveEntityPacket p = (RemoveEntityPacket)packet;
            _GameObjects.RemoveObject(p.Serial);
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
            receive_UnhandledPacket(packet);
        }

        private void receive_DisplayGumpFast(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_DisplayMenu(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_EnableFeatures(IRecvPacket packet)
        {
            SupportedFeaturesPacket p = (SupportedFeaturesPacket)packet;
            receive_UnhandledPacket(packet);
        }

        private void receive_Extended0x78(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_GeneralInfo(IRecvPacket packet)
        {
            GeneralInfoPacket p = (GeneralInfoPacket)packet;
            switch (p.Subcommand)
            {
                case 0x08: // Set cursor color / set map
                    // p.MapID_MapID;
                    // 0 = Felucca, cursor not colored / BRITANNIA map
                    // 1 = Trammel, cursor colored gold / BRITANNIA map
                    // 2 = (switch to) ILSHENAR map
                    // !!! unhandled! We default to the fel/tram map
                    break;
                case 0x14: // return context menu
                    parseContextMenu(p.ContextMenu);
                    // !!! Unhandled
                    break;
                case 0x18: // Number of maps
                    // m_ReceiveMapPatches(reader);
                    // !!! Unhandled
                    break;
                case 0x1D: // House revision
                    // !!! Unhandled
                    break;
                case 0x04: // Close generic gump
                    // !!! Unhandled
                    break;
                case 0x19: // Extended stats: http://docs.polserver.com/packets/index.php?Packet=0xBF
                    break;
                default:
                    throw (new System.Exception("Unhandled Subcommand in ServerInfo packet: " + p.Subcommand));
            }
        }

        private void receive_GlobalQueueCount(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_GraphicEffect(IRecvPacket packet)
        {
            // unhandled !!!
            receive_UnhandledPacket(packet);
        }

        private void receive_HuedEffect(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_InvalidMapEnable(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_LoginComplete(IRecvPacket packet)
        {
            // This packet is just one byte, the opcode.
            // Congrats, login complete!

            // We want to make sure we have the client object before we load the world.
            // If we don't, just set the status to login complete, which will then
            // load the world when we finally receive our client object.
            if (_GameObjects.MySerial != 0)
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
            receive_UnhandledPacket(packet);
        }

        private void receive_MobileAttributes(IRecvPacket packet)
        {
            MobileAttributesPacket p = (MobileAttributesPacket)packet;
            receive_UnhandledPacket(packet);
        }

        private void receive_MobileAnimation(IRecvPacket packet)
        {
            MobileAnimationPacket p = (MobileAnimationPacket)packet;

            Unit iObject = _GameObjects.GetObject<Unit>(p.Serial, false);
            iObject.Animation(p.Action, p.FrameCount, p.RepeatCount, p.Reverse, p.Repeat, p.Delay);
        }

        private void receive_MobileIncoming(IRecvPacket packet)
        {
            MobileIncomingPacket p = (MobileIncomingPacket)packet;
            Unit iMobile = _GameObjects.GetObject<Unit>(p.Serial, true);
            iMobile.DisplayBodyID = p.BodyID;
            iMobile.Hue = (int)p.Hue;
            iMobile.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z);
            iMobile.SetFacing(p.Direction & 0x0F);

            for (int i = 0; i < p.Equipment.Length; i++)
            {
                GameObject item = addItem(p.Equipment[i].Serial, p.Equipment[i].GumpId, p.Equipment[i].Hue, 0, 0);
                iMobile.Equipment[p.Equipment[i].Layer] = item;
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

            Unit iObject = _GameObjects.GetObject<GameObjects.Unit>(p.Serial, true);
            iObject.SetFacing(p.Direction);
            iObject.DisplayBodyID = p.BodyID;
            // Issue 16 - Pet not showing at login - http://code.google.com/p/ultimaxna/issues/detail?id=16 - Smjert
            // Since no packet arrives to add your pet, when you move and your pet follows you the client crashes
            if (iObject.Movement.DrawPosition == null)
            {
                iObject.Movement.SetPositionInstant(p.X, p.Y, p.Z);
            }
            else if (iObject.Movement.DrawPosition.PositionV3 == new Vector3(0, 0, 0))
            {
                iObject.Movement.SetPositionInstant(p.X, p.Y, p.Z);
                // Issue 16 - End
            }
            else
            {
                iObject.Move(p.X, p.Y, p.Z);
            }
        }

        private void receive_MobileUpdate(IRecvPacket packet)
        {
            MobileUpdatePacket p = (MobileUpdatePacket)packet;
            Unit iObject = _GameObjects.GetObject<Unit>(p.Serial, true);
            iObject.DisplayBodyID = p.BodyID;
            iObject.Hue = (int)p.Hue;
            iObject.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z);
            iObject.SetFacing(p.Direction & 0x0F);

            if (iObject.Name == string.Empty)
            {
                iObject.Name = "Unknown";
                this.Send(new RequestNamePacket(p.Serial));
            }
        }

        private void receive_MoveAck(IRecvPacket packet)
        {
            MoveAcknowledgePacket p = (MoveAcknowledgePacket)packet;
            _GameObjects.GetPlayerObject().Movement.MoveEventAck(p.Sequence);
        }

        private void receive_MoveRej(IRecvPacket packet)
        {
            MovementRejectPacket p = (MovementRejectPacket)packet;
            _GameObjects.GetPlayerObject().Movement.MoveEventRej(p.Sequence, p.X, p.Y, p.Z, p.Direction);
        }

        private void receive_NewSubserver(IRecvPacket packet)
        {
            SubServerPacket p = (SubServerPacket)packet;
            receive_UnhandledPacket(packet);
        }

        private void receive_ObjectHelpResponse(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_ObjectPropertyList(IRecvPacket packet)
        {
            ObjectPropertyListPacket p = (ObjectPropertyListPacket)packet;

            BaseObject iObject = _GameObjects.GetObject<BaseObject>(p.Serial, false);
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
                    string[] iArgs = p.Arguements[i].Split('\t');
                    iObject.PropertyList.AddProperty(constructCliLoc(iCliLoc, iArgs));
                }
            }
        }

        private void receive_OnParticleEffect(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_OnSwing(IRecvPacket packet)
        {
            SwingPacket p = (SwingPacket)packet;
            receive_UnhandledPacket(packet);
        }

        private void receive_OpenBuyWindow(IRecvPacket packet)
        {
            VendorBuyListPacket p = (VendorBuyListPacket)packet;
            GameObject iObject = _GameObjects.GetObject<GameObject>(p.VendorPackSerial, false);
            if (iObject == null)
                return;
            _GUI.Merchant_Open(iObject, 0);
        }

        private void receive_OpenPaperdoll(IRecvPacket packet)
        {
            // unhandled;
            receive_UnhandledPacket(packet);
        }

        private void receive_OpenWebBrowser(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_OverallLightLevel(IRecvPacket packet)
        {
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F
            // !!! Unhandled
            receive_UnhandledPacket(packet);
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
            receive_UnhandledPacket(packet);
        }

        private void receive_PlayerLocaleAndBody(IRecvPacket packet)
        {
            LoginConfirmPacket p = (LoginConfirmPacket)packet;

            // When loading the player object, we must load the serial before the object.
            _GameObjects.MySerial = p.Serial;
            Player iPlayer = _GameObjects.GetObject<Player>(p.Serial, true);
            iPlayer.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z);
            iPlayer.SetFacing(p.Direction & 0x0F);

            // We want to make sure we have the client object before we load the world...
            if (Status == UltimaClientStatus.WorldServer_LoginComplete)
                Status = UltimaClientStatus.WorldServer_InWorld;
        }

        private void receive_PlayerMove(IRecvPacket packet)
        {
            PlayerMovePacket p = (PlayerMovePacket)packet;
            receive_UnhandledPacket(packet);
        }

        private void receive_PlayMusic(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_PlaySoundEffect(IRecvPacket packet)
        {
            // BYTE[1] mode (0x00=quiet, repeating, 0x01=single normally played sound effect)
            // BYTE[2] SoundModel
            // BYTE[2] unknown3 (speed/volume modifier? Line of sight stuff?)
            // BYTE[2] xLoc
            // BYTE[2] yLoc
            // BYTE[2] zLoc
            receive_UnhandledPacket(packet);
        }

        private void receive_PopupMessage(IRecvPacket packet)
        {
            PopupMessagePacket p = (PopupMessagePacket)packet;
            receive_UnhandledPacket(packet);
        }

        private void receive_QuestArrow(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_RejectMoveItemRequest(IRecvPacket packet)
        {
            LiftRejectionPacket p = (LiftRejectionPacket)packet;
            GUI.GUIHelper.Chat_AddLine("Could not pick up item: " + p.ErrorMessage);
        }

        private void receive_RequestNameResponse(IRecvPacket packet)
        {
            RequestNameResponsePacket p = (RequestNameResponsePacket)packet;
            Unit u = _GameObjects.GetObject <Unit>(p.Serial, false);
            u.Name = p.MobileName;
        }

        private void receive_ResurrectionMenu(IRecvPacket packet)
        {
            // int iAction = reader.ReadByte();
            // 0: Server sent
            // 1: Resurrect
            // 2: Ghost
            // The only use on OSI for this packet is now sending "2C02" for the "You Are Dead" screen upon character death.
            receive_UnhandledPacket(packet);
        }

        private void receive_SeasonalInformation(IRecvPacket packet)
        {
            // Unhandled!!! If iSeason2 = 1, then this is a season change.
            // If season change, then iSeason1 = (0=spring, 1=summer, 2=fall, 3=winter, 4 = desolation)
            receive_UnhandledPacket(packet);
        }

        private void receive_SellList(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_ServerList(IRecvPacket packet)
        {
            ServerListPacket p = (ServerListPacket)packet;
            this.Send(new SelectServerPacket(p.Servers[0].Index));
        }

        private void receive_SetWeather(IRecvPacket packet)
        {
            // !!! Unhandled!
            receive_UnhandledPacket(packet);
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
            // unhandled !!!
            int iPacketType = 0; // reader.ReadByte();
            // 0x00: Full List of skills
            // 0xFF: Single skill update
            // 0x02: Full List of skills with skill cap for each skill
            // 0xDF: Single skill update with skill cap for skill

            switch (iPacketType)
            {
                case 0x00: //Full List of skills
                    receive_UnhandledPacket(packet);
                    break;
                case 0xFF: // single skill update
                    receive_UnhandledPacket(packet);
                    break;
                case 0x02: // full List of skills with skill cap for each skill
                    receive_UnhandledPacket(packet);
                    break;
                case 0xDF: // Single skill update with skill cap for skill
                    receive_UnhandledPacket(packet);
                    break;
                default:
                    throw (new Exception("Unknown PacketType"));
            }
        }

        private void receive_Speech(IRecvPacket packet)
        {
            // BYTE[2] Packet len
            // BYTE[4] itemID (FF FF FF FF = system) 
            // BYTE[2] model (item # - FF FF = system) 
            // BYTE[1] Type of Text
            // BYTE[2] Text Color 
            // BYTE[2] Font 
            // BYTE[30] Name 
            // BYTE[?] Null-Terminated Msg (? = Packet length - 44)

            // Notes
            // This is the packet POL uses to send names when you single click on items or NPCs.
            // This is a much more optimized method (packet size) than OSI's method by using 0xC1

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
            receive_UnhandledPacket(packet);
        }

        private void receive_StatusInfo(IRecvPacket packet)
        {
            MobileStatusCompactPacket p = (MobileStatusCompactPacket)packet;

            if (p.StatusType >= 6)
            {
                throw (new Exception("KR Status not handled."));
            }

            Unit u = _GameObjects.GetObject<Unit>(p.Serial, false);
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
            // !!! Unhandled!
            receive_UnhandledPacket(packet);
        }

        private void receive_TipNotice(IRecvPacket packet)
        {
            receive_UnhandledPacket(packet);
        }

        private void receive_ToolTipRevision(IRecvPacket packet)
        {
            ObjectPropertyListUpdatePacket p = (ObjectPropertyListUpdatePacket)packet;
            BaseObject iObject = _GameObjects.GetObject<BaseObject>(p.Serial, false);
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
            Unit u = _GameObjects.GetObject<Unit>(p.Serial, false);
            u.Health.Update(p.Current, p.Max);
        }

        private void receive_UpdateMana(IRecvPacket packet)
        {
            UpdateManaPacket p = (UpdateManaPacket)packet;
            Unit u = _GameObjects.GetObject<Unit>(p.Serial, false);
            u.Mana.Update(p.Current, p.Max);
        }

        private void receive_UpdateStamina(IRecvPacket packet)
        {
            UpdateStaminaPacket p = (UpdateStaminaPacket)packet;
            Unit u = _GameObjects.GetObject<Unit>(p.Serial, false);
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
                GameObject iObject = _GameObjects.GetObject<GameObject>((int)p.Serial, true);
                iObject.ObjectTypeID = p.ItemID;
                iObject.Item_StackCount = p.StackAmount;
                iObject.Hue = p.Hue;
                iObject.Movement.SetPositionInstant(p.X, p.Y, p.Z);
            }
            else
            {
                // create a multi object. Unhandled !!!
                receive_UnhandledPacket(packet);
            }
        }

        private void receive_WornItem(IRecvPacket packet)
        {
            WornItemPacket p = (WornItemPacket)packet;
            GameObject item = addItem(p.Serial, p.ItemId, p.Hue, 0, 0);
            Unit u = _GameObjects.GetObject<Unit>(p.ParentSerial, false);
            u.Equipment[p.Layer] = item;
            if (item.PropertyList.Hash == 0)
                this.Send(new QueryPropertiesPacket(item.Serial));
        }






        private void receive_UnhandledPacket(IRecvPacket packet)
        {
            GUI.GUIHelper.Chat_AddLine("DEBUG: Unhandled " + packet.Name + ". <" + packet.Id + ">");
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

        private string constructCliLoc(string nBase, string[] nArgs)
        {
            string iConstruct = nBase;
            for (int i = 0; i < nArgs.Length; i++)
            {
                int iBeginReplace = iConstruct.IndexOf('~', 0);
                int iEndReplace = iConstruct.IndexOf('~', iBeginReplace + 1);
                iConstruct = iConstruct.Substring(0, iBeginReplace) + nArgs[i] + iConstruct.Substring(iEndReplace + 1, iConstruct.Length - iEndReplace - 1);
            }
            return iConstruct;
        }

        private void receive_EndVendorSell(Packet reader)
        {
            reader.ReadShort(); // packet length = 8
            int iVendorSerial = reader.ReadInt();
            reader.ReadByte(); // always = 0
        }

        private GameObject addItem(Serial serial, int nItemID, int nHue, int nContainerSerial, int nAmount)
        {
            GameObject iObject = _GameObjects.GetObject<GameObject>(serial, true);
            
            iObject.ObjectTypeID = nItemID;
            iObject.Hue = nHue;
            iObject.Item_StackCount = nAmount;
            iObject.Item_ContainedWithinSerial = nContainerSerial;
            
            return iObject;
        }
    }
}
