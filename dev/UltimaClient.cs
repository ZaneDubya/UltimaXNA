/***************************************************************************
 *   UltimaClient.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaPackets.Server;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaVars;
#endregion

namespace UltimaXNA
{
    public class UltimaClient : Client
    {
        public UltimaClientStatus Status { get; protected set; }
        private int m_ServerRelayKey = 0;

        protected UltimaEngine Engine { get; private set; }

        public UltimaClient(UltimaEngine engine)
        {
            Engine = engine;

            Status = UltimaClientStatus.Unconnected;

            Initialize();
        }

        /// <summary>
        /// Register all packets that comprise the login and character creation protocols.
        /// </summary>
        private void Initialize()
        {
            Register<LoginConfirmPacket>(0x1B, "Login Confirm", 37, new TypedPacketReceiveHandler(ReceiveLoginConfirmPacket));
            Register<LoginCompletePacket>(0x55, "Login Complete", 1, new TypedPacketReceiveHandler(ReceiveLoginComplete));
            Register<LoginRejectionPacket>(0x82, "Login Rejection", 2, new TypedPacketReceiveHandler(ReceiveLoginRejection));
            Register<DeleteCharacterResponsePacket>(0x85, "Delete Character Response", 2, new TypedPacketReceiveHandler(ReceiveDeleteCharacterResponse));
            Register<CharacterListUpdatePacket>(0x86, "Character List Update", -1, new TypedPacketReceiveHandler(ReceiveCharacterListUpdate));
            Register<ServerRelayPacket>(0x8C, "ServerRelay", 11, new TypedPacketReceiveHandler(ReceiveServerRelay));
            Register<ServerListPacket>(0xA8, "Game Server List", -1, new TypedPacketReceiveHandler(ReceiveServerList));
            Register<CharacterCityListPacket>(0xA9, "Characters / Starting Locations", -1, new TypedPacketReceiveHandler(ReceiveCharacterList));
            Register<SupportedFeaturesPacket>(0xB9, "Supported Features", 3, new TypedPacketReceiveHandler(ReceiveEnableFeatures));
            Register<VersionRequestPacket>(0xBD, "Version Request", -1, new TypedPacketReceiveHandler(ReceiveVersionRequest));
        }

        /// <summary>
        /// Connect to a server!
        /// </summary>
        /// <param name="host">Address of the host. Can be a website or an ip address. IP addresses should be IPv4.</param>
        /// <param name="port">Port of the server on the host.</param>
        /// <returns></returns>
        public override bool Connect(string host, int port)
        {
            Status = UltimaClientStatus.LoginServer_Connecting;
            bool success = base.Connect(host, port);
            if (success)
            {
                Status = UltimaClientStatus.LoginServer_WaitingForLogin;
                if (EngineVars.Version.Length != 4)
                    Core.Diagnostics.Logger.Warn("Cannot send seed packet: Version array is incorrectly sized.");
                else
                    Send(new SeedPacket(1, EngineVars.Version));
                
            }
            else
            {
                Status = UltimaClientStatus.Error_CannotConnectToServer;
            }
            return success;
        }

        /// <summary>
        /// Disconnects from the server.
        /// </summary>
        public override void Disconnect()
        {
            if (IsConnected)
                base.Disconnect();
            Status = UltimaClientStatus.Unconnected;
        }

        /// <summary>
        /// Attempts to login to the connected host.
        /// </summary>
        /// <param name="account">The username of the account to be logged in.</param>
        /// <param name="password">The password of the account to be logged in. This is encrypted in transit.</param>
        public void SendAccountLogin(string account, string password)
        {
            Status = UltimaClientStatus.LoginServer_LoggingIn;
            Send(new LoginPacket(account, password));
        }

        /// <summary>
        /// Connect to the indicated relay server.
        /// </summary>
        /// <param name="account">The username of the account to be logged in.</param>
        /// <param name="password">The password of the account to be logged in. This is encrypted in transit.</param>
        public void SendServerRelay(string account, string password)
        {
            Status = UltimaClientStatus.LoginServer_Relaying;
            Send(new GameLoginPacket(m_ServerRelayKey, account, password));
        }

        /// <summary>
        /// Sends a message to the server to request a connection to the specified shard.
        /// </summary>
        /// <param name="index">The index of the shard to connect to.</param>
        public void SelectShard(int index)
        {
            if (Status == UltimaClientStatus.LoginServer_HasServerList)
            {
                Status = UltimaClientStatus.GameServer_Connecting;
                Send(new SelectServerPacket(index));
            }
        }

        /// <summary>
        /// Sends a message to the server to request login with the specified player.
        /// </summary>
        /// <param name="index">The index of the character to login with.</param>
        public void LoginWithCharacter(int index)
        {
            if (Status == UltimaClientStatus.GameServer_CharList)
            {
                if (UltimaVars.Characters.List[index].Name != string.Empty)
                {
                    Engine.QueuedModel = new WorldModel();
                    Send(new LoginCharacterPacket(UltimaVars.Characters.List[index].Name, index, Utility.IPAddress));
                }
            }
        }

        /// <summary>
        /// Sends a message to the server, requesting creation of a new character.
        /// </summary>
        /// <param name="packet">A completed character creation packet.</param>
        public void CreateCharacter(CreateCharacterPacket packet)
        {
            Engine.QueuedModel = new WorldModel();
            Send(packet);
        }

        /// <summary>
        /// Sends a message to the server, requesting that an existing character be deleted.
        /// </summary>
        /// <param name="index">The index of the character to be deleted.</param>
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

        /// <summary>
        /// Sends the server the client version. Version is specified in EngineVars.
        /// </summary>
        public void SendClientVersion()
        {
            if (EngineVars.Version.Length != 4)
                Logger.Warn("Cannot send seed packet: Version array is incorrectly sized.");
            else
                Send(new ClientVersionPacket(EngineVars.Version));
        }












        private void ReceiveDeleteCharacterResponse(IRecvPacket packet)
        {
            DeleteCharacterResponsePacket p = (DeleteCharacterResponsePacket)packet;
            Engine.UserInterface.MsgBox(p.Result, MsgBoxTypes.OkOnly);
        }

        private void ReceiveCharacterListUpdate(IRecvPacket packet)
        {
            CharacterListUpdatePacket p = (CharacterListUpdatePacket)packet;
            UltimaVars.Characters.SetCharacterList(p.Characters);
        }

        private void ReceiveCharacterList(IRecvPacket packet)
        {
            CharacterCityListPacket p = (CharacterCityListPacket)packet;
            UltimaVars.Characters.SetCharacterList(p.Characters);
            UltimaVars.Characters.SetStartingLocations(p.Locations);
            Status = UltimaClientStatus.GameServer_CharList;
        }

        private void ReceiveServerList(IRecvPacket packet)
        {
            UltimaVars.Servers.List = ((ServerListPacket)packet).Servers;
            Status = UltimaClientStatus.LoginServer_HasServerList;
        }

        private void ReceiveLoginRejection(IRecvPacket packet)
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

        private void ReceiveServerRelay(IRecvPacket packet)
        {
            ServerRelayPacket p = (ServerRelayPacket)packet;
            m_ServerRelayKey = p.AccountId;
            // On OSI, upon receiving this packet, the client would disconnect and
            // log in to the specified server. Since emulated servers use the same
            // server for both shard selection and world, we don't need to disconnect.
            IsDecompressionEnabled = true;
            Status = UltimaClientStatus.LoginServer_WaitingForRelay;
        }

        private void ReceiveEnableFeatures(IRecvPacket packet)
        {
            SupportedFeaturesPacket p = (SupportedFeaturesPacket)packet;
            UltimaVars.Features.SetFlags(p.Flags);
        }

        private void ReceiveVersionRequest(IRecvPacket packet)
        {
            SendClientVersion();
        }



        // ======================================================================
        // New login handling routines
        // ======================================================================

        LoginConfirmPacket m_QueuedLoginConfirmPacket;

        private void ReceiveLoginConfirmPacket(IRecvPacket packet)
        {
            m_QueuedLoginConfirmPacket = (LoginConfirmPacket)packet;
            // set the player serial var and create the player entity. Don't need to do anything with it yet.
            UltimaVars.EngineVars.PlayerSerial = m_QueuedLoginConfirmPacket.Serial;
            PlayerMobile player = EntityManager.GetObject<PlayerMobile>(m_QueuedLoginConfirmPacket.Serial, true);
            if (player == null)
                Logger.Fatal("Could not create player object.");
            CheckIfOkayToLogin();
        }

        private void ReceiveLoginComplete(IRecvPacket packet)
        {
            // This packet is just one byte, the opcode.
            CheckIfOkayToLogin();
        }

        private void CheckIfOkayToLogin()
        {
            // Before the client logs in, we need to know the player entity's serial, and the
            // map the player will be loading on login. If we don't have either of these, we
            // delay loading until we do.
            if (Status != UltimaClientStatus.WorldServer_InWorld)
            {
                if (m_QueuedLoginConfirmPacket != null && (Engine.QueuedModel as WorldModel).MapIndex >= 0)
                {
                    Status = UltimaClientStatus.WorldServer_InWorld;

                    Engine.ActivateQueuedModel();
                    if (Engine.ActiveModel is WorldModel)
                    {
                        ((WorldModel)Engine.ActiveModel).LoginSequence();
                        LoginConfirmPacket packet = m_QueuedLoginConfirmPacket;
                        m_QueuedLoginConfirmPacket = null;
                        PlayerMobile player = EntityManager.GetObject<PlayerMobile>(packet.Serial, true);
                        if (player == null)
                            Logger.Fatal("No player object ready in CheckIfOkayToLogin().");
                        player.Move_Instant(packet.X, packet.Y, packet.Z, packet.Direction);
                        
                    }
                    else
                    {
                        Logger.Fatal("Not in world model at login.");
                    }
                }
            }
            else
            {
                // already logged in, nothing else to do!
            }
        }
    }
}
