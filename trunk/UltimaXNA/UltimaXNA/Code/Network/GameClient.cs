#region File Description & Usings
//-----------------------------------------------------------------------------
// GameClient.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.Network
{
    public delegate void UserEventDlg(object sender,
                                      SocketClient player);

    public delegate void DataReceivedDlg(SocketClient nSender,
                                        Packet nPacket);

    public enum ClientStatus
    {
        Unconnected,
        LoginServer_Connecting,
        LoginServer_HasServerList,
        LoginServer_ServerSelected,
        GameServer_Connecting,
        GameServer_ConnectedAndCharList,
        GameServer_LoggingIn,
        InWorld,
        Error_CouldNotConnectToLoginServer,
    }

    public interface IGameClient
    {
        void Send_MoveRequest(int nDirection, int nSequence, int nFastWalkKey);
        void Send_ConnectToLoginServer(string nIPAdress, int nPort, string nAccount, string nPassword);
        void Send_UseRequest(int nGUID);
        void Disconnect();
        void Send_PickUpItem(int nGUID, int nNumInStack);
        void Send_DropItem(int nGUID, int nX, int nY, int nZ, int nContainerGUID);
    }

    class GameClient : GameComponent, IGameClient
    {
        public bool Disconnected = false;
        public ServerList ServerList;
        public ClientStatus Status;
        private SocketClient m_Client;

        // Temporary variables to store account information.
        // We clear these once we no longer need them (after
        // we log in).
        private char[] m_Account;
        private char[] m_Password;

        private List<Packet> m_Packets = new List<Packet>();
        private List<Packet> m_SecondaryPackets = new List<Packet>();
        private bool m_ProcessingPackets = false;

        internal MiscUtil.LogFile LogFile;
        GUI.IGUI m_GUIService;
        GameObjects.IGameObjects m_GameObjectsService;
        IGameState m_GameStateService;

        public GameClient(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IGameClient), this);
            this.Status = ClientStatus.Unconnected;
        }

        public override void Initialize()
        {
            this.LogFile = new MiscUtil.LogFile("log-" + System.DateTime.Now.ToString("dd-MM-yy HH-mm-ss") + ".txt");
            m_GUIService = (GUI.IGUI)Game.Services.GetService(typeof(GUI.IGUI));
            m_GameObjectsService = (GameObjects.IGameObjects)Game.Services.GetService(typeof(GameObjects.IGameObjects));
            m_GameStateService = (IGameState)Game.Services.GetService(typeof(IGameState));
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Process the packets received since the last update.
            // Setting m_ProcessingPackets = true will stop GameClient
            // from adding any additional packets to this list while
            // we are processing it. During this time, any new packets
            // are added to m_SecondaryPackets, so we will have to process
            // those as well.
            m_ProcessingPackets = true;
            m_ProcessPackets(m_Packets);
            m_ProcessingPackets = false;
            // Process the packets received while we were processing m_Packets.
            m_ProcessPackets(m_SecondaryPackets);
            base.Update(gameTime);
        }

        private void m_ProcessPackets(List<Packet> nPackets)
        {
            foreach (Packet iPacket in nPackets)
            {
                switch (iPacket.OpCode)
                {
                    case OpCodes.SMSG_STATUSINFO:
                        m_ReceiveStatusInfo(iPacket);
                        break;
                    case OpCodes.SMSG_MOBILEUPDATE:
                        m_ReceiveMobileUpdate(iPacket);
                        break;
                    case OpCodes.SMSG_LIGHTLEVEL:
                        m_ReceiveLightLevel(iPacket);
                        break;
                    case OpCodes.SMSG_PERSONALLIGHTLEVEL:
                        m_ReceivePersonalLightLevel(iPacket);
                        break;
                    case OpCodes.SMSG_CHARLOCALE:
                        m_ReceivePlayerLocaleAndBody(iPacket);
                        break;
                    case OpCodes.SMSG_MobileIncoming:
                        m_ReceiveMobileIncoming(iPacket);
                        break;
                    case OpCodes.SMSG_SERVERLIST:
                        m_ReceiveServerList(iPacket);
                        break;
                    case OpCodes.SMSG_SERVER_REDIRECT:
                        m_ReceiveServerRedirect(iPacket);
                        break;
                    case OpCodes.SMSG_CHARACTERLIST:
                        m_ReceiveCharacterList(iPacket);
                        break;
                    case OpCodes.MSG_CHAT:
                        m_ReceiveChatMessage(iPacket);
                        break;
                    case OpCodes.SMSG_ENABLEFEATURES:
                        m_ReceiveServerEnableFeatures(iPacket);
                        break;
                    case OpCodes.MSG_CLIENTVERSION:
                        m_ReceiveServerRequestClientVersion(iPacket);
                        break;
                    case OpCodes.MSG_GENERALINFO:
                        m_ReceiveServerGeneralInfo(iPacket);
                        break;
                    case OpCodes.SMSG_SEASONALINFORMATION:
                        m_ReceiveSeasonal(iPacket);
                        break;
                    case OpCodes.SMSG_SETWEATHER:
                        m_ReceiveSetWeather(iPacket);
                        break;
                    case OpCodes.MSG_WARMODE:
                        m_ReceiveWarMode(iPacket);
                        break;
                    case OpCodes.SMSG_OBJECTPROPERTYLIST:
                        m_ReceiveObjectPropertyList(iPacket);
                        break;
                    case OpCodes.SMSG_LOGINCOMPLETE:
                        m_ReceiveLoginComplete(iPacket);
                        break;
                    case OpCodes.SMSG_THETIME:
                        m_ReceiveTheTime(iPacket);
                        break;
                    case OpCodes.SMSG_UNICODEMESSAGE:
                        m_ReceiveUnicodeMessage(iPacket);
                        break;
                    case OpCodes.SMSG_WorldItem:
                        m_ReceiveWorldItem(iPacket);
                        break;
                    case OpCodes.SMSG_MobileAnimation:
                        m_ReceiveMobileAnimation(iPacket);
                        break;
                    case OpCodes.SMSG_DELETEOBJECT:
                        m_ReceiveDeleteObject(iPacket);
                        break;
                    case OpCodes.SMSG_MobileMoving:
                        m_ReceivMobileMoving(iPacket);
                        break;
                    case OpCodes.SMSG_SENDSPEECH:
                        m_ReceiveSendSpeech(iPacket);
                        break;
                    case OpCodes.SMSG_MOVEACK:
                        m_ReceiveMoveAck(iPacket);
                        break;
                    case OpCodes.SMSG_PLAYSOUNDEFFECT:
                        m_ReceivePlaySoundEffect(iPacket);
                        break;
                    case OpCodes.SMSG_MOVEREJ:
                        m_ReceiveMoveRej(iPacket);
                        break;
                    case OpCodes.SMSG_CONTAINER:
                        m_ReceiveContainer(iPacket);
                        break;
                    case OpCodes.SMSG_ADDMULTIPLEITEMSTOCONTAINER:
                        m_ReceiveAddMultipleItemsToContainer(iPacket);
                        break;
                    case OpCodes.SMSG_ADDSINGLEITEMTOCONTAINER:
                        m_ReceiveAddSingleItemToContainer(iPacket);
                        break;
                    case OpCodes.SMSG_REJECTMOVEITEMREQ:
                        m_ReceiveRejectMoveItemRequest(iPacket);
                        break;
                    default:
                        // throw (new System.Exception("Unknown Opcode: " + nPacket.OpCode));
                        break;
                }
            }
            nPackets.Clear();
        }

        public void Send_ConnectToLoginServer(string nIPAdress, int nPort, string nAccount, string nPassword)
        {
            ConnectionStatus iStatus;
            this.Status = ClientStatus.LoginServer_Connecting;
            m_Client = new SocketClient(nIPAdress, nPort, out iStatus);
            m_Client.LogFile = this.LogFile;
            if (iStatus == ConnectionStatus.Connected)
            {
                m_Client.DataReceived += new DataReceivedDlg(client_DataReceived);
                m_Client.UserDisconnected += new UserEventDlg(client_UserDisconnected);
                mSend_Seed();
                mSend_AccountLogin(nAccount, nPassword);
            }
            else
            {
                string iErrText = "Connection error: could not connect to " + nIPAdress + ":" + nPort.ToString() + ".";
                LogFile.WriteLine(iErrText);
                m_GUIService.ErrorPopup_Modal(iErrText);
                this.Status = ClientStatus.Error_CouldNotConnectToLoginServer;
            }
        }

        public void SelectServer(int nServerIndex)
        {
            mSend_ServerSelect(nServerIndex);
        }

        private void ConnectToGameServer(string nIPAdress, ushort nPort, uint nEncryption)
        {
            ConnectionStatus iStatus;
            m_Client = new SocketClient(nIPAdress, (int)nPort, out iStatus);
            m_Client.DataReceived += new DataReceivedDlg(client_DataReceived);
            m_Client.UserDisconnected += new UserEventDlg(client_UserDisconnected);
            mSend_GameServerLogin(nEncryption);
        }

        private void mSend_AccountLogin(string nAccount, string nPassword)
        {
            Packet iPacket = new Packet(OpCodes.CMSG_ACCOUNTLOGINREQUEST);
            m_Account = new char[30];
            nAccount.CopyTo(0, m_Account, 0, nAccount.Length);
            m_Password = new char[30];
            nPassword.CopyTo(0, m_Password, 0, nPassword.Length);
            iPacket.Write(m_Account);
            iPacket.Write(m_Password);
            iPacket.Write((byte)0x5d);
            this.SendPacket(iPacket);
        }

        private void mSend_Seed()
        {
            Packet iPacket = new Packet(OpCodes.CMSG_SEED);
            iPacket.Write((int)1);
            this.SendPacket(iPacket);
        }

        private void mSend_ServerSelect(int ServerIndex)
        {
            // This does not work :(
            Packet iPacket = new Packet(OpCodes.CMSG_SERVERSELECT);
            iPacket.Write((ushort)ServerIndex);
            this.SendPacket(iPacket);
            this.Status = ClientStatus.LoginServer_ServerSelected;
        }

        private void mSend_GameServerLogin(uint nEncryption)
        {
            Packet iPacket = new Packet(OpCodes.CMSG_GAMESERVER_LOGIN);
            iPacket.Write(nEncryption);
            iPacket.Write(m_Account);
            iPacket.Write(m_Password);
            this.SendPacket(iPacket);
            this.Status = ClientStatus.GameServer_Connecting;
            m_Account = null;
            m_Password = null;
            m_Client.UnpackPackets = true;
        }

        private void mSend_LoginCharacter(string nCharacterName, byte nSlotChosen)
        {
            Packet iPacket = new Packet(OpCodes.CMSG_LOGINCHARACTER);
            iPacket.Write((uint)0xedededed);

            char[] iCharacterName = new char[30];
            nCharacterName.CopyTo(0, iCharacterName, 0, nCharacterName.Length);
            iPacket.Write(iCharacterName);
            iPacket.Write(new byte[33]);
            iPacket.Write(nSlotChosen);
            iPacket.Write(new byte[4] {127, 0, 0, 1 } );
            this.SendPacket(iPacket);
            this.Status = ClientStatus.GameServer_LoggingIn;
        }

        private void mSend_ClientVersion()
        {
            Packet iPacket = new Packet(OpCodes.MSG_CLIENTVERSION);
            string iVersionString = "6.0.6.2";
            char[] iVersion = new char[iVersionString.Length + 1];
            iVersionString.CopyTo(0, iVersion, 0, iVersionString.Length);
            iPacket.Write((ushort)(1 + 2 + iVersion.Length));
            iPacket.Write(iVersion);
            this.SendPacket(iPacket);
        }

        public void Send_MoveRequest(int nDirection, int nSequence, int nFastWalkKey)
        {
            Packet iPacket = new Packet(OpCodes.CMSG_MOVEREQUEST);
            iPacket.Write((byte)nDirection);
            iPacket.Write((byte)nSequence);
            iPacket.Write(nFastWalkKey);
            this.SendPacket(iPacket);
        }

        public void Send_UseRequest(int nGUID)
        {
            Packet iPacket = new Packet(OpCodes.CMSG_USEOBJECT);
            iPacket.Write((int)nGUID);
            this.SendPacket(iPacket);
        }

        public void Send_PickUpItem(int nGUID, int nNumInStack)
        {
            Packet iPacket = new Packet(OpCodes.CMSG_PICKUPITEM);
            iPacket.Write((int)nGUID);
            iPacket.Write((short)nNumInStack);
            this.SendPacket(iPacket);
        }

        public void Send_DropItem(int nGUID, int nX, int nY, int nZ, int nContainerGUID)
        {
            Packet iPacket = new Packet(OpCodes.CMSG_DROPITEM);
            iPacket.Write((int)nGUID);
            iPacket.Write((short)nX);
            iPacket.Write((short)nY);
            iPacket.Write((byte)nZ);
            iPacket.Write((byte)0);
            iPacket.Write((int)nContainerGUID);
            this.SendPacket(iPacket);
        }

        private void client_DataReceived(SocketClient sender, Packet nPacket)
        {
            // Since packets are received on a different thread than the game thread,
            // and we don't want to modify data that the game is working on, we will
            // save any packets we receive and process them all at once when the client
            // tells us it's safe to do so.
            if (m_ProcessingPackets)
                m_SecondaryPackets.Add(nPacket);
            else
                m_Packets.Add(nPacket);
        }

        private void m_ReceiveServerList(Packet nPacket)
        {
            ushort iSize = nPacket.ReadUShort();
            byte iFlags = nPacket.ReadByte();
            ushort iNumServers = nPacket.ReadUShort();
            this.ServerList = new ServerList(nPacket, iNumServers);
            this.Status = ClientStatus.LoginServer_HasServerList;
        }

        private void m_ReceiveServerRedirect(Packet nPacket)
        {
            byte[] iAddress = nPacket.ReadBytes(4);
            ushort iPort = nPacket.ReadUShort();
            uint iEncryptionKey = nPacket.ReadUInt();
            this.ServerList = null;
            //this.Disconnect();
            //string iIPAdress = iAddress[0] + "." +  iAddress[1] + "." +  iAddress[2] + "." +  iAddress[3];
            //this.ConnectToGameServer(iIPAdress, iPort, iEncryptionKey);
            this.mSend_GameServerLogin(iEncryptionKey);
        }

        private void m_ReceiveServerEnableFeatures(Packet nPacket)
        {
            ushort iFlags = nPacket.ReadUShort();
            // 0x01: enable T2A features: chat, regions
            // 0x02: enable renaissance features
            // 0x04: enable third dawn features
            // 0x08: enable LBR features: skills, map
            // 0x10: enable AOS features: skills, map, spells, fightbook
            // 0x20: 6th character slot
            // 0x40: enable SE features
            // 0x80: enable ML features: elven race, spells, skills
            // 0x100: enable 8th age splash screen
            // 0x200: enable 9th age splash screen
            // 0x1000: 7th character slot
            // 0x2000: 10th age KR faces
        }

        private void m_ReceiveChatMessage(Packet nPacket)
        {
            ushort iLength = nPacket.ReadUShort();
            byte[] iLanguageCode = nPacket.ReadBytes(3);
            byte[] iUnknown = nPacket.ReadBytes(2);
            byte iTextType = nPacket.ReadByte();
        }

        private void m_ReceiveCharacterList(Packet nPacket)
        {
            string iFirstCharName = null;
            ushort iSize = nPacket.ReadUShort();
            byte iNumCharacters = nPacket.ReadByte();
            for (int i = 0; i < iNumCharacters; i++)
            {
                string iCharName = System.Text.ASCIIEncoding.ASCII.GetString((nPacket.ReadBytes(30)));
                if (iFirstCharName == null)
                    iFirstCharName = iCharName;
                string iPassword = System.Text.ASCIIEncoding.ASCII.GetString((nPacket.ReadBytes(30)));
            }

            byte iNumStartingLocations = nPacket.ReadByte();
            for (int i = 0; i < iNumStartingLocations; i++)
            {
                byte iLocationIndex = nPacket.ReadByte();
                string iCityName = System.Text.ASCIIEncoding.ASCII.GetString((nPacket.ReadBytes(31)));
                string iAreaName = System.Text.ASCIIEncoding.ASCII.GetString((nPacket.ReadBytes(31)));
            }

            uint iFlags = nPacket.ReadUInt();

            this.Status = ClientStatus.GameServer_ConnectedAndCharList;
            mSend_LoginCharacter(iFirstCharName, 0);
        }

        private void m_ReceiveServerRequestClientVersion(Packet nPacket)
        {
            ushort iSize = nPacket.ReadUShort(); // Always = 0x03
            // Automatically respond.
            mSend_ClientVersion();
        }

        private void m_ReceivePlayerLocaleAndBody(Packet nPacket)
        {
            int iPlayerSerial = nPacket.ReadInt();
            int iUnknown = nPacket.ReadInt(); // Always 0
            ushort iBodyType = nPacket.ReadUShort();
            ushort iX = nPacket.ReadUShort();
            ushort iY = nPacket.ReadUShort();
            short iZ = nPacket.ReadShort();
            byte iFacing = nPacket.ReadByte();
            byte iUnknown5 = nPacket.ReadByte(); // Always 0
            int iUnknown3 = nPacket.ReadInt(); // Always -1

            // Next is the server boundaries. We can ignore these on RunUO.
            short iServerBoundaryX = nPacket.ReadShort(); // Always 0 on RunUO
            short iServerBoundaryY = nPacket.ReadShort(); // Always 0 on RunUO
            short iServerBoundaryWidth = nPacket.ReadShort(); // Always map width on RunUO
            short iServerBoundaryHeight = nPacket.ReadShort(); // Always map height on RunUO
            ushort iUnknown6 = nPacket.ReadUShort();
            uint iUnknown7 = nPacket.ReadUInt();

            // When loading the player object, we must load the serial before the object.
            m_GameObjectsService.MyGUID = iPlayerSerial;
            GameObjects.Player iPlayer = (GameObjects.Player)m_GameObjectsService.AddObject(
                new GameObjects.Player(m_GameObjectsService.MyGUID));
            iPlayer.Movement.SetPositionInstant((int)iX, (int)iY, (int)iZ);
            iPlayer.SetFacing(iFacing & 0x0F);

            // We want to make sure we have the client object before we load the world...
            if (this.Status == ClientStatus.InWorld)
                m_GameStateService.InWorld = true;
        }

        private void m_ReceiveServerGeneralInfo(Packet nPacket)
        {
            ushort iLength = nPacket.ReadUShort();
            ushort iSubCommand = nPacket.ReadUShort();

            switch (iSubCommand)
            {
                case 0x8: // Set cursor color / set map
                    byte iMapID = nPacket.ReadByte();
                    // 0 = Felucca, cursor not colored / BRITANNIA map
                    // 1 = Trammel, cursor colored gold / BRITANNIA map
                    // 2 = (switch to) ILSHENAR map
                    // !!! unhandled! We default to the fel/tram map
                    break;
                case 0x18: // Number of maps
                    m_ReceiveMapPatches(nPacket);
                    break;
                default:
                    throw (new System.Exception("Unhandled Subcommand in ServerInfo packet: " + iSubCommand));
            }
        }

        private void m_ReceiveMapPatches(Packet nPacket)
        {
            int iNumMaps = nPacket.ReadInt();
            int[][] iMapPatches = new int[iNumMaps][];
            // Maps: 0 = Felucca, 1 = Trammel, 2 = Ilshenar, 3 = Malas
            // MapPatch[0] = Patch.StaticBlocks, MapPatch[1] = Patch.LandBlocks
            // Unhandled!!!  Not sure what these are for - they seem to be zero ...
            for (int i = 0; i < iNumMaps; i++)
            {
                iMapPatches[i] = new int[2];
                iMapPatches[i][0] = nPacket.ReadInt();
                iMapPatches[i][1] = nPacket.ReadInt();
            }
        }

        private void m_ReceiveSeasonal(Packet nPacket)
        {
            byte iSeason1 = nPacket.ReadByte();
            byte iSeason2 = nPacket.ReadByte();
            if (iSeason2 == 1)
            {
                // Unhandled!!! If iSeason2 = 1, then this is a season change.
                // If season change, then iSeason1 = (0=spring, 1=summer, 2=fall, 3=winter, 4 = desolation)
            }
        }

        private void m_ReceiveMobileUpdate(Packet nPacket)
        {
            byte[] iZero = new byte[4];
            
            int iMobileSerial = nPacket.ReadInt();
            short iBodyID = nPacket.ReadShort();
            iZero = nPacket.ReadBytes(1); // Always 0
            ushort iHue = (ushort)(nPacket.ReadUShort()); // Skin hue
            byte iPacketFlags = nPacket.ReadByte();
            // These are the only flags sent by RunUO
            // 0x02 = female
            // 0x04 = poisoned
            // 0x08 = blessed/yellow health bar
            // 0x40 = warmode
            // 0x80 = hidden
            short iX = nPacket.ReadShort();
            short iY = nPacket.ReadShort();
            iZero = nPacket.ReadBytes(2);
            byte iFacing = nPacket.ReadByte();
            sbyte iZ = nPacket.ReadSByte();

            GameObjects.Unit iObject = (GameObjects.Unit)m_GameObjectsService.GetObject(iMobileSerial);
            iObject.DisplayBodyID = iBodyID;
            iObject.Hue = (int)iHue;

            iObject.Movement.SetPositionInstant((int)iX, (int)iY, (int)iZ);
            iObject.SetFacing(iFacing & 0x0F);
        }

        private void m_ReceiveLightLevel(Packet nPacket)
        {
            byte iLightLevel = nPacket.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F
            // !!! Unhandled
        }

        private void m_ReceivePersonalLightLevel(Packet nPacket)
        {
            int iCreatureID = nPacket.ReadInt();
            byte iLightLevel = nPacket.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F
            // !!! Unhandled
        }

        private void m_ReceiveMobileIncoming(Packet nPacket)
        {
            // Mobile
            ushort iPacketLength = nPacket.ReadUShort();
            uint iMobileSerial = nPacket.ReadUInt();
            ushort iBodyID = nPacket.ReadUShort();
            short iX = nPacket.ReadShort();
            short iY = nPacket.ReadShort();
            sbyte iZ = nPacket.ReadSByte();
            byte iFacing = nPacket.ReadByte();
            int iHue = nPacket.ReadUShort();
            byte iPacketFlags = nPacket.ReadByte();
            // These are the only flags sent by RunUO
            // 0x02 = female
            // 0x04 = poisoned
            // 0x08 = blessed/yellow health bar
            // 0x40 = warmode
            // 0x80 = hidden

            byte iNotoriety = nPacket.ReadByte();
            // 0x1: Innocent (Blue)
            // 0x2: Friend (Green)
            // 0x3: Grey (Grey - Animal)
            // 0x4: Criminal (Grey)
            // 0x5: Enemy (Orange)
            // 0x6: Murderer (Red)
            // 0x7: Invulnerable (Yellow)

            GameObjects.Unit iMobile = (GameObjects.Unit)m_GameObjectsService.AddObject(
                new GameObjects.Unit((int)iMobileSerial));
            iMobile.Movement.SetPositionInstant((int)iX, (int)iY, (int)iZ);
            iMobile.SetFacing(iFacing & 0x0F);
            iMobile.Hue = iHue;
            iMobile.DisplayBodyID = iBodyID;

            // Read equipment - nine bytes ea.
            int iEquipCount = 0;
            while (true)
            {
                int iEquipmentSerial = nPacket.ReadInt();
                if ( iEquipmentSerial == 0 )
				{
                    // Creature has no equip, since we read the serial we need to go move back the pointer or we go out of the stream - Smjert
                    nPacket.Stream.Seek(-4, SeekOrigin.Current);
                    break;
                }
                int iGraphic = nPacket.ReadUShort();
                int iLayer = nPacket.ReadByte();
                int iItemHue = 0;
                if ((iGraphic & 0x8000) == 0x8000)
                {
                    iGraphic = iGraphic - 0x8000;
                    iItemHue = (int)nPacket.ReadUShort();
                }
                iEquipCount++;

                // Create the object ... !!! We set the containerGUID to 0 here, should it be the guid of the wearer?
                GameObjects.GameObject iObject = m_AddItem(iEquipmentSerial, iGraphic, iItemHue, 0, 0);
                // Now equip the object to the unit we have created.
                iMobile.Equipment[iLayer] =  iObject;
            }

            if (iEquipCount == 0)
            {
                // zero terminated
                byte iZero = nPacket.ReadByte();
            }
        }

        private void m_ReceiveStatusInfo(Packet nPacket)
        {
            // !!! Unhandled!
        }

        private void m_ReceiveSetWeather(Packet nPacket)
        {
            // !!! Unhandled!
        }

        private void m_ReceiveWarMode(Packet nPacket)
        {
            // !!! Unhandled!
        }

        private void m_ReceiveObjectPropertyList(Packet nPacket)
        {
            // !!! Unhandled!
        }

        private void m_ReceiveLoginComplete(Packet nPacket)
        {
            // This packet is just one byte, the opcode.
            // Congrats, login complete!
			this.Status = ClientStatus.InWorld;
            // We want to make sure we have the client object before we load the world...
            if (m_GameObjectsService.MyGUID != 0)
			    m_GameStateService.InWorld = true;
        }

        private void m_ReceiveTheTime(Packet nPacket)
        {
            // !!! Unhandled!
        }

        private void m_ReceiveUnicodeMessage(Packet nPacket)
        {
            /*
            BYTE[2] length
            BYTE[4] ID
            BYTE[2] Model
            BYTE[1] Type
            BYTE[2] Color
            BYTE[2] Font
            BYTE[4] Language
            BYTE[30] Name
            BYTE[?][2] Msg - Null Terminated (blockSize - 48)
             */
            short iLength = nPacket.ReadShort();
            int iGUID = nPacket.ReadInt();
            short iModel = nPacket.ReadShort();
            short iMsgType = nPacket.ReadByte();
            short iHue = nPacket.ReadShort();
            short iFont = nPacket.ReadShort();
            int iLanguage = nPacket.ReadInt();
            string iStrName = m_RemoveNullGarbageFromString(nPacket.ReadBytes(30));
            string iText = m_RemoveNullGarbageFromString(nPacket.ReadChars((iLength - 48)));

            m_GUIService.AddChatText(
                iStrName + ": " + iText
                );
        }

        private void m_ReceiveWorldItem(Packet nPacket)
        {
            // BYTE[1] cmd 
            // BYTE[2] blockSize 
            // BYTE[4] itemID 
            // BYTE[2] model # 
            // If (itemID & 0x80000000)
            //     BYTE[2] item count (or model # for corpses)
            // If (model & 0x8000) 
            //     BYTE Incr Counter (increment model by this #)
            // BYTE[2] xLoc (only use lowest significant 15 bits) 
            // BYTE[2] yLoc 
            // If (xLoc & 0x8000) 
            //     BYTE direction
            // BYTE zLoc 
            // If (yLoc & 0x8000) 
            //     BYTE[2] dye
            // If (yLoc & 0x4000)
            //     BYTE flag byte ( 0x80 = invisible, 0x20 = movable)

            ushort iPacketSize = nPacket.ReadUShort();
            uint iObjectSerial = nPacket.ReadUInt();
            ushort iItemID = nPacket.ReadUShort();
            ushort iAmount = 0;
            if ((iObjectSerial & 0x80000000) == 0x80000000)
                iAmount = nPacket.ReadUShort();

			// Doesn't exist this thing in the packet
            /*byte iIncrement = 0;
            if ((iItemID & 0x8000) == 0x8000)
            {
                iIncrement = nPacket.ReadByte();
                iObjectSerial += iIncrement;
            }*/
            ushort iX = nPacket.ReadUShort();
            ushort iY = nPacket.ReadUShort();
            byte iDirection = 0;
            if ((iX & 0x8000) == 0x8000)
                iDirection = nPacket.ReadByte();
            sbyte iZ = nPacket.ReadSByte();
            ushort iHue = 0;
            if ((iY & 0x8000) == 0x8000)
                iHue = nPacket.ReadUShort();
            byte iFlags = 0;
            if ((iY & 0x4000) == 0x4000)
                iFlags = nPacket.ReadByte();

            iObjectSerial &= 0x7FFFFFFF;
            iItemID &= 0x7FFF;
            iX &= 0x7FFF;
            iY &= 0x3FFF;

            // Now create the GameObject.
            GameObjects.GameObject iObject = (GameObjects.GameObject)m_GameObjectsService.AddObject(
                new GameObjects.GameObject((int)iObjectSerial));
            iObject.ObjectTypeID = iItemID;
            iObject.Movement.SetPositionInstant(iX, iY, iZ);
        }

        private void m_ReceiveMobileAnimation(Packet nPacket)
        {
            int iSerial = nPacket.ReadInt();
            short iAction = nPacket.ReadShort();
            short iFrameCount = nPacket.ReadShort();
            short iRepeatCount = nPacket.ReadShort();
            bool iReverse = nPacket.ReadBool();
            bool iRepeat = nPacket.ReadBool();
            byte iDelay = nPacket.ReadByte();

            GameObjects.Unit iObject = (GameObjects.Unit)m_GameObjectsService.GetObject(iSerial);
            iObject.Animation(iAction, iFrameCount, iRepeatCount, iReverse, iRepeat, iDelay);
        }

        private void m_ReceiveDeleteObject(Packet nPacket)
        {
            // !!! Unhandled!
        }

        private void m_ReceivMobileMoving(Packet nPacket)
        {
            int iSerial = nPacket.ReadInt();
            ushort iBodyID = nPacket.ReadUShort();
            short iX = nPacket.ReadShort();
            short iY = nPacket.ReadShort();
            sbyte iZ = nPacket.ReadSByte();
            byte iFacing = nPacket.ReadByte();
            ushort iHue = nPacket.ReadUShort();
            byte iFlags = nPacket.ReadByte();
            // These are the only flags sent by RunUO
            // 0x02 = female
            // 0x04 = poisoned
            // 0x08 = blessed/yellow health bar
            // 0x40 = warmode
            // 0x80 = hidden
            byte iNotoriety = nPacket.ReadByte();

            GameObjects.Unit iObject = (GameObjects.Unit)m_GameObjectsService.GetObject(iSerial);
			// Issue 16 - Pet not showing at login - http://code.google.com/p/ultimaxna/issues/detail?id=16 - Smjert
			// Since no packet arrives to add your pet, when you move and your pet follows you the client crashes
			if ( iObject == null )
			{
				iObject = m_GameObjectsService.AddObject(new GameObjects.Unit(iSerial)) as GameObjects.Unit;
				iObject.SetFacing(iFacing);
				iObject.DisplayBodyID = iBodyID;
				iObject.Movement.SetPositionInstant(iX, iY, iZ);
			}
			// Issue 16 - End
            iObject.Move(iX, iY, iZ);
        }

        private void m_ReceiveSendSpeech(Packet nPacket)
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

        private void m_ReceiveMoveAck(Packet nPacket)
        {
            int iSequence = nPacket.ReadByte(); // (matches sent sequence)
            int iNotoriety = nPacket.ReadByte(); // Not sure why it sends this.
            m_GameObjectsService.GetObject(m_GameObjectsService.MyGUID).Movement.MoveEventAck(iSequence);
        }

        private void m_ReceiveMoveRej(Packet nPacket)
        {
            int iSequence = nPacket.ReadByte(); // (matches sent sequence)
            int iX = nPacket.ReadUShort();
            int iY = nPacket.ReadUShort();
            int iDirection = nPacket.ReadByte();
            int iZ = nPacket.ReadByte();
            m_GameObjectsService.GetObject(m_GameObjectsService.MyGUID).Movement.MoveEventRej(iSequence, iX, iY, iZ, iDirection);
        }

        private void m_ReceivePlaySoundEffect(Packet nPacket)
        {
            // BYTE[1] mode (0x00=quiet, repeating, 0x01=single normally played sound effect)
            // BYTE[2] SoundModel
            // BYTE[2] unknown3 (speed/volume modifier? Line of sight stuff?)
            // BYTE[2] xLoc
            // BYTE[2] yLoc
            // BYTE[2] zLoc
        }

        private void m_ReceiveContainer(Packet nPacket)
        {
            int iGUID = nPacket.ReadInt();
            int iGumpModel = nPacket.ReadUShort();

            // Only try to open a container of type Container. Note that GameObjects can
            // have container objects and will expose them when called through GetContainerObject(int)
            // instead of GetObject(int).
            GameObjects.BaseObject iObject = m_GameObjectsService.GetContainerObject(iGUID); // Changed from GetObject(iGUID) to GetContainerObject(iGUID) per issue8 (http://code.google.com/p/ultimaxna/issues/detail?id=8) --ZDW 6/14/2009
            if (iObject.ObjectType == UltimaXNA.GameObjects.ObjectType.Container)
            {
                m_GUIService.Container_Open(iObject, iGumpModel);
            }
            else
            {
                throw (new Exception("No support for container object!"));
            }
        }

        private void m_ReceiveAddMultipleItemsToContainer(Packet nPacket)
        {
            int iPacketLength = nPacket.ReadUShort();
            int iNumItems = nPacket.ReadUShort();

            for (int i = 0; i < iNumItems; i++)
            {
                int iGUID = nPacket.ReadInt();
                int iItemID = nPacket.ReadUShort();
                int iUnknown = nPacket.ReadByte(); // signed, itemID offset. always 0 in RunUO.
                int iAmount = nPacket.ReadUShort();
                int iX = nPacket.ReadShort();
                int iY = nPacket.ReadShort();
                int iGridLocation = nPacket.ReadByte(); // always 0 in RunUO.
                int iContainerGUID = nPacket.ReadInt();
                int iHue = nPacket.ReadUShort();

                // Add the item...
                GameObjects.GameObject iObject = m_AddItem(iGUID, iItemID, iHue, iContainerGUID, iAmount);
                iObject.Item_InvX_SlotIndex = iX;
                iObject.Item_InvY_SlotChecksum = iY;
                // ... and add it the container contents of the container.
                GameObjects.Container iContainerObject = (GameObjects.Container)m_GameObjectsService.GetContainerObject((int)iContainerGUID);
                iContainerObject.AddItem(iObject);
            }
        }

        private void m_ReceiveAddSingleItemToContainer(Packet nPacket)
        {
            int iGUID = nPacket.ReadInt();
            int iItemID = nPacket.ReadUShort();
            int iUnknown = nPacket.ReadByte(); // always 0 in RunUO.
            int iAmount = nPacket.ReadUShort();
            int iX = nPacket.ReadShort();
            int iY = nPacket.ReadShort();
            int iGridLocation = nPacket.ReadByte(); // always 0 in RunUO.
            int iContainerGUID = nPacket.ReadInt();
            int iHue = nPacket.ReadUShort();

            // Add the item...
            GameObjects.GameObject iObject = m_AddItem(iGUID, iItemID, iHue, iContainerGUID, iAmount);
            iObject.Item_InvX_SlotIndex = iX;
            iObject.Item_InvY_SlotChecksum = iY;
            // ... and add it the container contents of the container.
            GameObjects.Container iContainerObject = (GameObjects.Container)m_GameObjectsService.GetContainerObject((int)iContainerGUID);
            iContainerObject.AddItem(iObject);
        }

        private void m_ReceiveRejectMoveItemRequest(Packet nPacket)
        {
            int iReason = nPacket.ReadByte();
            // 0x0: Cannot lift the item
            // 0x1: Out of range
            // 0x2: Out of sight
            // 0x3: Belongs to another
            // 0x4: Already holding something
            // 0x5: empty message on client?
        }

        private GameObjects.GameObject m_AddItem(int nGUID, int nItemID, int nHue, int nContainerGUID, int nAmount)
        {
            // Create the object. If an item has the 'Container' flag, then make it a container!
            GameObjects.GameObject iObject;
            if (DataLocal.TileData.ItemData[nItemID].Container)
            {
                iObject = (GameObjects.Container)m_GameObjectsService.AddObject(
                    new GameObjects.Container((int)nGUID));
            }
            else
            {
                iObject = (GameObjects.GameObject)m_GameObjectsService.AddObject(
                    new GameObjects.GameObject((int)nGUID));
            }

            iObject.ObjectTypeID = nItemID;
            iObject.Hue = nHue;
            iObject.Item_StackCount = nAmount;
            iObject.Item_ContainedWithinGUID = nContainerGUID;

            return iObject;
        }

        public void Disconnect()
        {
            if (m_Client != null)
            {
                m_Client.Disconnect();
                m_Client = null;
            }
        }

        public void SendPacket(Packet nPacket)
        {
            this.m_Client.SendMemoryStream(nPacket.Stream);
        }

        private void client_UserDisconnected(object sender, SocketClient player)
        {
            // If the connection to the server is lost just exit the game.
            this.Disconnected = true;
        }

        protected void m_RemoveNullGarbageFromString(ref string str)
        {
            while (str[str.Length - 1] == '\0')
            {
                str = str.Remove(str.Length - 1);
            }
        }

        protected string m_RemoveNullGarbageFromString(byte[] ndata)
        {
            string str = System.Text.Encoding.Default.GetString(ndata);
            int i = 0;
            while (i < str.Length)
            {
                if (str[i] == '\0')
                    str = str.Remove(i, 1);
                else
                    i++;
            }
            return str;
        }

        protected string m_RemoveNullGarbageFromString(char[] ndata)
        {
            string str = new string(ndata);
            int i = 0;
            while (i < str.Length)
            {
                if (str[i] == '\0')
                    str = str.Remove(i, 1);
                else
                    i++;
            }
            return str;
        }
    }

    public class ServerList
    {
        public ServerList_Entry[] List;
        public int NumServers;
        public ServerList(Packet nPacket, int nNumServers)
        {
            this.NumServers = nNumServers;
            this.List = new ServerList_Entry[nNumServers];
            for (int i = 0; i < nNumServers; i++)
            {
                this.List[i] = new ServerList_Entry();
                this.List[i].Index = nPacket.ReadUShort();
                this.List[i].Name = nPacket.ReadChars(32);
                this.List[i].Full = nPacket.ReadByte();
                this.List[i].Timezone = nPacket.ReadByte();
                this.List[i].Address = nPacket.ReadBytes(4);
            }
        }
    }

    public class ServerList_Entry
    {
        public ushort Index;
        public char[] Name;
        public byte Full;
        public byte Timezone;
        public byte[] Address;
    }
}
