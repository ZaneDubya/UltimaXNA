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

using System.Security;
using UltimaXNA.Configuration;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Patterns.IoC;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Data.Accounts;
using UltimaXNA.Ultima.Data.Servers;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.Network.Server;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima
{
    public class Client
    {
        private readonly INetworkClient m_Network;
        private readonly IEngine m_Engine;
        private readonly IContainer m_Container;

        private int m_ServerRelayKey;

        internal string UserName
        {
            get;
            set;
        }

        internal SecureString Password
        {
            get;
            set;
        }

        public UltimaClientStatus Status { get; protected set; }

        public Client(IContainer container)//UltimaEngine engine)
        {
            m_Container = container;
            m_Network = container.Resolve<INetworkClient>();
            m_Engine = container.Resolve<IEngine>();

            Status = UltimaClientStatus.Unconnected;

            Initialize();
        }

        /// <summary>
        /// Register all packets that comprise the login and character creation protocols.
        /// </summary>
        private void Initialize()
        {
            m_Network.Register<LoginConfirmPacket>(0x1B, "Login Confirm", 37, ReceiveLoginConfirmPacket);
            m_Network.Register<LoginCompletePacket>(0x55, "Login Complete", 1, ReceiveLoginComplete);
            m_Network.Register<LoginRejectionPacket>(0x82, "Login Rejection", 2, ReceiveLoginRejection);
            m_Network.Register<DeleteCharacterResponsePacket>(0x85, "Delete Character Response", 2, ReceiveDeleteCharacterResponse);
            m_Network.Register<CharacterListUpdatePacket>(0x86, "Character List Update", -1, ReceiveCharacterListUpdate);
            m_Network.Register<ServerRelayPacket>(0x8C, "ServerRelay", 11, ReceiveServerRelay);
            m_Network.Register<ServerListPacket>(0xA8, "Game Server List", -1, ReceiveServerList);
            m_Network.Register<CharacterCityListPacket>(0xA9, "Characters / Starting Locations", -1, ReceiveCharacterList);
            m_Network.Register<SupportedFeaturesPacket>(0xB9, "Supported Features", 3, ReceiveEnableFeatures);
            m_Network.Register<VersionRequestPacket>(0xBD, "Version Request", -1, ReceiveVersionRequest);
        }

        /// <summary>
        /// Connect to a server!
        /// </summary>
        /// <param name="host">Address of the host. Can be a website or an ip address. IP addresses should be IPv4.</param>
        /// <param name="port">Port of the server on the host.</param>
        /// <returns></returns>
        public bool Connect(string host, int port)
        {
            Status = UltimaClientStatus.LoginServer_Connecting;
            bool success = m_Network.Connect(host, port);

            if (success)
            {
                Status = UltimaClientStatus.LoginServer_WaitingForLogin;

                var clientVersion = Settings.UltimaOnline.ClientVersion;

                if (clientVersion.Length != 4)
                    Tracer.Warn("Cannot send seed packet: Version array is incorrectly sized.");
                else
                    m_Network.Send(new SeedPacket(1, clientVersion));
                
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
        public void Disconnect()
        {
            if (m_Network.IsConnected)
            {
                m_Network.Disconnect();
            }

            Status = UltimaClientStatus.Unconnected;
        }

        /// <summary>
        /// Attempts to login to the connected host.
        /// </summary>
        /// <param name="account">The username of the account to be logged in.</param>
        /// <param name="password">The password of the account to be logged in. This is encrypted in transit.</param>
        public void Login()
        {
            Status = UltimaClientStatus.LoginServer_LoggingIn;

            m_Network.Send(new LoginPacket(Settings.Server.UserName, Password.ConvertToUnsecureString()));
        }

        /// <summary>
        /// Connect to the indicated relay server.
        /// </summary>
        /// <param name="account">The username of the account to be logged in.</param>
        /// <param name="password">The password of the account to be logged in. This is encrypted in transit.</param>
        public void Relay()
        {
            Status = UltimaClientStatus.LoginServer_Relaying;
            m_Network.Send(new GameLoginPacket(m_ServerRelayKey, Settings.Server.UserName, Password.ConvertToUnsecureString()));
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
                m_Network.Send(new SelectServerPacket(index));
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
                if (Characters.List[index].Name != string.Empty)
                {
                    m_Engine.QueuedModel = new WorldModel(m_Container);
                    m_Network.Send(new LoginCharacterPacket(Characters.List[index].Name, index, Utility.IPAddress));
                }
            }
        }

        /// <summary>
        /// Sends a message to the server, requesting creation of a new character.
        /// </summary>
        /// <param name="packet">A completed character creation packet.</param>
        public void CreateCharacter(CreateCharacterPacket packet)
        {
            m_Engine.QueuedModel = new WorldModel(m_Container);
            m_Network.Send(packet);
        }

        /// <summary>
        /// Sends a message to the server, requesting that an existing character be deleted.
        /// </summary>
        /// <param name="index">The index of the character to be deleted.</param>
        public void DeleteCharacter(int index)
        {
            if (Status == UltimaClientStatus.GameServer_CharList)
            {
                if (Characters.List[index].Name != string.Empty)
                {
                    m_Network.Send(new DeleteCharacterPacket(index, Utility.IPAddress));
                }
            }
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

        private void ReceiveDeleteCharacterResponse(IRecvPacket packet)
        {
            DeleteCharacterResponsePacket p = (DeleteCharacterResponsePacket)packet;
            m_Engine.UserInterface.MsgBox(p.Result, MsgBoxTypes.OkOnly);
        }

        private void ReceiveCharacterListUpdate(IRecvPacket packet)
        {
            CharacterListUpdatePacket p = (CharacterListUpdatePacket)packet;
            Characters.SetCharacterList(p.Characters);
        }

        private void ReceiveCharacterList(IRecvPacket packet)
        {
            CharacterCityListPacket p = (CharacterCityListPacket)packet;
            Characters.SetCharacterList(p.Characters);
            Characters.SetStartingLocations(p.Locations);
            Status = UltimaClientStatus.GameServer_CharList;
        }

        private void ReceiveServerList(IRecvPacket packet)
        {
            Servers.List = ((ServerListPacket)packet).Servers;
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
            m_Network.IsDecompressionEnabled = true;
            Status = UltimaClientStatus.LoginServer_WaitingForRelay;
        }

        private void ReceiveEnableFeatures(IRecvPacket packet)
        {
            SupportedFeaturesPacket p = (SupportedFeaturesPacket)packet;
            Features.SetFlags(p.Flags);
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
            // set the player serial and create the player entity. Don't need to do anything with it yet.
            EngineVars.PlayerSerial = m_QueuedLoginConfirmPacket.Serial;
            PlayerMobile player = EntityManager.GetObject<PlayerMobile>(m_QueuedLoginConfirmPacket.Serial, true);
            if (player == null)
                Tracer.Critical("Could not create player object.");
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
                if (m_QueuedLoginConfirmPacket != null && (m_Engine.QueuedModel as WorldModel).MapIndex >= 0)
                {
                    Status = UltimaClientStatus.WorldServer_InWorld;

                    m_Engine.ActivateQueuedModel();
                    if (m_Engine.ActiveModel is WorldModel)
                    {
                        ((WorldModel)m_Engine.ActiveModel).LoginSequence();
                        LoginConfirmPacket packet = m_QueuedLoginConfirmPacket;
                        PlayerMobile player = EntityManager.GetObject<PlayerMobile>(m_QueuedLoginConfirmPacket.Serial, true);
                        if (player == null)
                            Tracer.Critical("No player object ready in CheckIfOkayToLogin().");
                        player.Move_Instant(packet.X, packet.Y, packet.Z, packet.Direction);
                        // iPlayer.SetFacing(p.Direction);
                    }
                    else
                    {
                        Tracer.Critical("Not in world model at login.");
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
