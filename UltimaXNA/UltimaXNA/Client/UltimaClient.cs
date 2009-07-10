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

        private string mAccount; private string mPassword;
        public void SetAccountPassword(string nAccount, string nPassword) { mAccount = nAccount; mPassword = nPassword; } 
        private void clearAccountPassword() { mAccount = string.Empty; mPassword = string.Empty; }

        private UltimaXNA.Network.ClientNetwork m_ClientNetwork;
        private UltimaXNA.GUI.IGUI m_GUI;
        private IGameObjects m_GameObjects;
        private UltimaXNA.IGameState m_GameState;

        public UltimaClient(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IUltimaClient), this);
            this.Status = UltimaClientStatus.Unconnected;
        }

        public override void Initialize()
        {
            m_GUI = Game.Services.GetService(typeof(GUI.IGUI)) as GUI.IGUI;
            m_GameObjects = Game.Services.GetService(typeof(GameObjects.IGameObjects)) as GameObjects.IGameObjects;
            m_GameState = Game.Services.GetService(typeof(IGameState)) as IGameState;

            m_ClientNetwork = new ClientNetwork();
            m_RegisterPackets();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            m_ClientNetwork.Update();
        }

        private void m_RegisterPackets()
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

            PacketRegistry.RegisterNetwork(m_ClientNetwork);
        }

        public bool Connect(string ipAddressOrHostName, int port)
        {
            bool success = m_ClientNetwork.Connect(ipAddressOrHostName, port);
            if (success)
            {
                this.Status = UltimaClientStatus.LoginServer_Connecting;
                m_ClientNetwork.Send(new SeedPacket(1, 6, 0, 6, 2));
            }
            return success;
        }

        public void Disconnect()
        {
            if (m_ClientNetwork.IsConnected)
                m_ClientNetwork.Disconnect();
            this.Status = UltimaClientStatus.Unconnected;
            clearAccountPassword();
        }

        public void Send(ISendPacket packet)
        {
            bool success = m_ClientNetwork.Send(packet);
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
                GameObject iObject = m_AddItem(i.Serial, i.ItemID, i.Hue, i.ContainerGUID, i.Amount);
                iObject.Item_InvX = i.X;
                iObject.Item_InvY = i.Y;
                // ... and add it the container contents of the container.
                GameObject iContainerObject = m_GameObjects.GetObject(i.ContainerGUID, ObjectType.GameObject) as GameObject;
                iContainerObject.ContainerObject.AddItem(iObject);
            }
        }

        private void receive_AddSingleItemToContainer(IRecvPacket packet)
        {
            ContainerContentUpdatePacket p = (ContainerContentUpdatePacket)packet;

            // Add the item...
            GameObjects.GameObject iObject = m_AddItem(p.Serial, p.ItemId, p.Hue, p.ContainerSerial, p.Amount);
            iObject.Item_InvX = p.X;
            iObject.Item_InvY = p.Y;
            // ... and add it the container contents of the container.
            GameObject iContainerObject = m_GameObjects.GetObject(p.ContainerSerial, ObjectType.GameObject) as GameObject;
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

        }

        private void receive_CLILOCMessage(IRecvPacket packet)
        {
            // unhandled !!!
        }

        private void receive_ChangeCombatant(IRecvPacket packet)
        {

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
            GUI.GUIHelper.Chat_AddLine("DEBUG: Compressed gump received but not handled.");
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
            GameObjects.BaseObject iObject = m_GameObjects.GetObject(p.Serial, ObjectType.Object);
            if (iObject.ObjectType == ObjectType.GameObject)
            {
                m_GUI.Container_Open(iObject, p.GumpId);
            }
            else
            {
                throw (new Exception("No support for container object!"));
            }
        }

        private void receive_CorpseClothing(IRecvPacket packet)
        {
            // unhandled !!!
        }

        private void receive_Damage(IRecvPacket packet)
        {
            DamagePacket p = (DamagePacket)packet;
        }

        private void receive_DeathAnimation(IRecvPacket packet)
        {

        }

        private void receive_DeleteObject(IRecvPacket packet)
        {
            RemoveEntityPacket p = (RemoveEntityPacket)packet;
            m_GameObjects.RemoveObject(p.Serial);
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
        }

        private void receive_DisplayGumpFast(IRecvPacket packet)
        {

        }

        private void receive_DisplayMenu(IRecvPacket packet)
        {

        }

        private void receive_EnableFeatures(IRecvPacket packet)
        {
            SupportedFeaturesPacket p = (SupportedFeaturesPacket)packet;
        }

        private void receive_Extended0x78(IRecvPacket packet)
        {

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
                    // m_ReceiveContextMenu(reader);
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
                default:
                    throw (new System.Exception("Unhandled Subcommand in ServerInfo packet: " + p.Subcommand));
            }
        }

        private void receive_GlobalQueueCount(IRecvPacket packet)
        {

        }

        private void receive_GraphicEffect(IRecvPacket packet)
        {
            // unhandled !!!
        }

        private void receive_HuedEffect(IRecvPacket packet)
        {

        }

        private void receive_InvalidMapEnable(IRecvPacket packet)
        {

        }

        private void receive_LoginComplete(IRecvPacket packet)
        {
            // This packet is just one byte, the opcode.
            // Congrats, login complete!

            // We want to make sure we have the client object before we load the world.
            // If we don't, just set the status to login complete, which will then
            // load the world when we finally receive our client object.
            if (m_GameObjects.MyGUID != 0)
                this.Status = UltimaClientStatus.WorldServer_InWorld;
            else
                this.Status = UltimaClientStatus.WorldServer_LoginComplete;
        }

        private void receive_LoginRejection(IRecvPacket packet)
        {
            Disconnect();
            LoginRejectionPacket p = (LoginRejectionPacket)packet;
            m_GUI.Reset();
            m_GUI.ErrorPopup_Modal(p.Reason);
        }

        private void receive_MessageLocalizedAffix(IRecvPacket packet)
        {

        }

        private void receive_MobileAttributes(IRecvPacket packet)
        {
            MobileAttributesPacket p = (MobileAttributesPacket)packet;
        }

        private void receive_MobileAnimation(IRecvPacket packet)
        {
            MobileAnimationPacket p = (MobileAnimationPacket)packet;

            Unit iObject = m_GameObjects.GetObject(p.Serial, ObjectType.Unit) as Unit;
            iObject.Animation(p.Action, p.FrameCount, p.RepeatCount, p.Reverse, p.Repeat, p.Delay);
        }

        private void receive_MobileIncoming(IRecvPacket packet)
        {
            MobileIncomingPacket p = (MobileIncomingPacket)packet;
            Unit iMobile = m_GameObjects.GetObject(p.Serial, ObjectType.Unit) as Unit;
            iMobile.DisplayBodyID = p.BodyID;
            iMobile.Hue = (int)p.Hue;
            iMobile.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z);
            iMobile.SetFacing(p.Direction & 0x0F);

            for (int i = 0; i < p.Equipment.Length; i++)
            {
                GameObject iObject = m_AddItem(p.Equipment[i].Serial, p.Equipment[i].GumpId, p.Equipment[i].Hue, 0, 0);
                iMobile.Equipment[p.Equipment[i].Layer] = iObject;
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

            Unit iObject = m_GameObjects.GetObject(p.Serial, ObjectType.Unit) as Unit;
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
            Unit iObject = m_GameObjects.GetObject(p.Serial, ObjectType.Unit) as Unit;
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
            m_GameObjects.GetPlayerObject().Movement.MoveEventAck(p.Sequence);
        }

        private void receive_MoveRej(IRecvPacket packet)
        {
            MovementRejectPacket p = (MovementRejectPacket)packet;
            m_GameObjects.GetPlayerObject().Movement.MoveEventRej(p.Sequence, p.X, p.Y, p.Z, p.Direction);
        }

        private void receive_NewSubserver(IRecvPacket packet)
        {
            SubServerPacket p = (SubServerPacket)packet;
        }

        private void receive_ObjectHelpResponse(IRecvPacket packet)
        {

        }

        private void receive_ObjectPropertyList(IRecvPacket packet)
        {
            ObjectPropertyListPacket p = (ObjectPropertyListPacket)packet;

            BaseObject iObject = m_GameObjects.GetObject(p.Serial, ObjectType.Object);
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
                    iObject.PropertyList.AddProperty(m_ConstructCliLoc(iCliLoc, iArgs));
                }
            }
        }

        private void receive_ObjectPropertyListUpdate(IRecvPacket packet)
        {
            ObjectPropertyListUpdatePacket p = (ObjectPropertyListUpdatePacket)packet;
            BaseObject iObject = m_GameObjects.GetObject(p.Serial, ObjectType.Object);
            if (iObject.PropertyList.Hash != p.RevisionHash)
            {
                this.Send(new QueryPropertiesPacket(p.Serial));
            }
        }

        private void receive_OnParticleEffect(IRecvPacket packet)
        {

        }

        private void receive_OnSwing(IRecvPacket packet)
        {
            SwingPacket p = (SwingPacket)packet;
        }

        private void receive_OpenBuyWindow(IRecvPacket packet)
        {
            VendorBuyListPacket p = (VendorBuyListPacket)packet;
            GameObject iObject = m_GameObjects.GetObject(p.VendorPackSerial, ObjectType.GameObject) as GameObject;
            m_GUI.Merchant_Open(iObject, 0);
        }

        private void receive_OpenPaperdoll(IRecvPacket packet)
        {
            // unhandled;
        }

        private void receive_OpenWebBrowser(IRecvPacket packet)
        {

        }

        private void receive_OverallLightLevel(IRecvPacket packet)
        {
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F
            // !!! Unhandled
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
        }

        private void receive_PlayerLocaleAndBody(IRecvPacket packet)
        {
            LoginConfirmPacket p = (LoginConfirmPacket)packet;

            // When loading the player object, we must load the serial before the object.
            m_GameObjects.MyGUID = p.Serial;
            Player iPlayer = (Player)m_GameObjects.GetObject(p.Serial, ObjectType.Player);
            iPlayer.Movement.SetPositionInstant((int)p.X, (int)p.Y, (int)p.Z);
            iPlayer.SetFacing(p.Direction & 0x0F);

            // We want to make sure we have the client object before we load the world...
            if (Status == UltimaClientStatus.WorldServer_LoginComplete)
                this.Status = UltimaClientStatus.WorldServer_InWorld;
        }

        private void receive_PlayerMove(IRecvPacket packet)
        {
            PlayerMovePacket p = (PlayerMovePacket)packet;
        }

        private void receive_PlayMusic(IRecvPacket packet)
        {
            // unhandled !!!
        }

        private void receive_PlaySoundEffect(IRecvPacket packet)
        {
            // BYTE[1] mode (0x00=quiet, repeating, 0x01=single normally played sound effect)
            // BYTE[2] SoundModel
            // BYTE[2] unknown3 (speed/volume modifier? Line of sight stuff?)
            // BYTE[2] xLoc
            // BYTE[2] yLoc
            // BYTE[2] zLoc
        }

        private void receive_PopupMessage(IRecvPacket packet)
        {
            PopupMessagePacket p = (PopupMessagePacket)packet;
        }

        private void receive_QuestArrow(IRecvPacket packet)
        {

        }

        private void receive_RejectMoveItemRequest(IRecvPacket packet)
        {
            LiftRejectionPacket p = (LiftRejectionPacket)packet;
            GUI.GUIHelper.Chat_AddLine("Could not pick up item: " + p.ErrorMessage);
        }

        private void receive_RequestNameResponse(IRecvPacket packet)
        {
            RequestNameResponsePacket p = (RequestNameResponsePacket)packet;
            Unit u = m_GameObjects.GetObject(p.Serial, ObjectType.Unit) as Unit;
            u.Name = p.MobileName;
        }

        private void receive_ResurrectionMenu(IRecvPacket packet)
        {
            // int iAction = reader.ReadByte();
            // 0: Server sent
            // 1: Resurrect
            // 2: Ghost
            // The only use on OSI for this packet is now sending "2C02" for the "You Are Dead" screen upon character death.
        }

        private void receive_SeasonalInformation(IRecvPacket packet)
        {
            // Unhandled!!! If iSeason2 = 1, then this is a season change.
            // If season change, then iSeason1 = (0=spring, 1=summer, 2=fall, 3=winter, 4 = desolation)
        }

        private void receive_SellList(IRecvPacket packet)
        {

        }

        private void receive_ServerList(IRecvPacket packet)
        {
            ServerListPacket p = (ServerListPacket)packet;
            this.Send(new SelectServerPacket(0));
        }

        private void receive_SetWeather(IRecvPacket packet)
        {
            // !!! Unhandled!
        }

        private void receive_ServerRelay(IRecvPacket packet)
        {
            ServerRelayPacket p = (ServerRelayPacket)packet;
            // Normally, upon receiving this packet you would disconnect and
            // log in to the specified server. Since we are using RunUO, we don't
            // actually need to do this.
            m_ClientNetwork.IsDecompressionEnabled = true;
            this.Send(new GameLoginPacket(p.AccountId, mAccount, mPassword));
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

                    break;
                case 0xFF: // single skill update

                    break;
                case 0x02: // full List of skills with skill cap for each skill

                    break;
                case 0xDF: // Single skill update with skill cap for skill

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
        }

        private void receive_StatusInfo(IRecvPacket packet)
        {
            MobileStatusCompactPacket p = (MobileStatusCompactPacket)packet;

            if (p.StatusType >= 6)
            {
                throw (new Exception("KR Status not handled."));
            }

            if (p.Serial != m_GameObjects.MyGUID)
            {
                throw new Exception("Assumption that StatusBarInfo packet always is for player is wrong!");
            }

            Unit u = (GameObjects.Unit)m_GameObjects.GetPlayerObject();
            u.Name = p.PlayerName;
            u.Health.Update(p.CurrentHealth, p.MaxHealth);
            u.Stamina.Update(p.CurrentStamina, p.MaxStamina);
            u.Mana.Update(p.CurrentMana, p.MaxMana);
            //  other stuff unhandled !!!
        }

        private void receive_TargetCursor(IRecvPacket packet)
        {
            TargetCursorPacket p = (TargetCursorPacket)packet;
            m_GameState.MouseTargeting(p.CursorID, p.CommandType);
        }

        private void receive_Time(IRecvPacket packet)
        {
            // !!! Unhandled!
        }

        private void receive_TipNotice(IRecvPacket packet)
        {

        }

        private void receive_ToolTipRevision(IRecvPacket packet)
        {

        }

        private void receive_UnicodeMessage(IRecvPacket packet)
        {
            UnicodeMessagePacket p = (UnicodeMessagePacket)packet;
            GUI.GUIHelper.Chat_AddLine(p.SpeakerName + ": " + p.SpokenText);
        }

        private void receive_UpdateHealth(IRecvPacket packet)
        {
            UpdateHealthPacket p = (UpdateHealthPacket)packet;
            Unit u = m_GameObjects.GetObject(p.Serial, ObjectType.Unit) as Unit;
            u.Health.Update(p.Current, p.Max);
        }

        private void receive_UpdateMana(IRecvPacket packet)
        {
            UpdateManaPacket p = (UpdateManaPacket)packet;
            Unit u = m_GameObjects.GetObject(p.Serial, ObjectType.Unit) as Unit;
            u.Health.Update(p.Current, p.Max);
        }

        private void receive_UpdateStamina(IRecvPacket packet)
        {
            UpdateStaminaPacket p = (UpdateStaminaPacket)packet;
            Unit u = m_GameObjects.GetObject(p.Serial, ObjectType.Unit) as Unit;
            u.Health.Update(p.Current, p.Max);
        }

        private void receive_VersionRequest(IRecvPacket packet)
        {
            // Automatically respond.
            this.Send(new ClientVersionPacket("6.0.6.2"));
        }

        private void receive_WarMode(IRecvPacket packet)
        {
            // !!! Unhandled!
        }

        private void receive_WorldItem(IRecvPacket packet)
        {
            WorldItemPacket p = (WorldItemPacket)packet;
            // Now create the GameObject.
            // If the iItemID < 0x4000, this is a regular game object.
            // If the iItemID >= 0x4000, then this is a multiobject.
            if (p.ItemID <= 0x4000)
            {
                GameObject iObject = m_GameObjects.GetObject((int)p.Serial, ObjectType.GameObject) as GameObject;
                iObject.ObjectTypeID = p.ItemID;
                iObject.Item_StackCount = p.StackAmount;
                iObject.Hue = p.Hue;
                iObject.Movement.SetPositionInstant(p.X, p.Y, p.Z);
            }
            else
            {
                // create a multi object. Unhandled !!!
            }
        }

        private void receive_WornItem(IRecvPacket packet)
        {
            WornItemPacket p = (WornItemPacket)packet;
            GameObject iObject = m_AddItem(p.Serial, p.ItemId, p.Hue, 0, 0);
            Unit u = m_GameObjects.GetObject(p.ParentSerial, ObjectType.Unit) as Unit;
            u.Equipment[p.Layer] = iObject;
        }










        private string m_ConstructCliLoc(string nBase, string[] nArgs)
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

        private void m_ReceiveEndVendorSell(Packet reader)
        {
            reader.ReadShort(); // packet length = 8
            int iVendorGUID = reader.ReadInt();
            reader.ReadByte(); // always = 0
        }

        private GameObject m_AddItem(int nGUID, int nItemID, int nHue, int nContainerGUID, int nAmount)
        {
            GameObject iObject = m_GameObjects.GetObject(nGUID, ObjectType.GameObject) as GameObject;
            
            iObject.ObjectTypeID = nItemID;
            iObject.Hue = nHue;
            iObject.Item_StackCount = nAmount;
            iObject.Item_ContainedWithinGUID = nContainerGUID;
            
            return iObject;
        }
    }
}
