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
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Login.Accounts;
using UltimaXNA.Ultima.Login.Servers;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.Network.Server;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.UI;
#endregion

namespace UltimaXNA.Ultima.Login
{
    public class LoginClient : IDisposable
    {
        private Timer m_KeepAliveTimer;

        private readonly INetworkClient m_Network;
        private readonly UltimaGame m_Engine;
        private readonly UserInterfaceService m_UserInterface;

        private List<Tuple<int, TypedPacketReceiveHandler>> m_RegisteredHandlers;

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

        public LoginClientStatus Status { get; protected set; }

        public LoginClient()
        {
            m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_Engine = ServiceRegistry.GetService<UltimaGame>();
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();

            Status = LoginClientStatus.Unconnected;

            m_RegisteredHandlers = new List<Tuple<int, TypedPacketReceiveHandler>>();

            Initialize();
        }

        /// <summary>
        /// Register all packets that comprise the login and character creation protocols.
        /// </summary>
        private void Initialize()
        {
            Register<LoginConfirmPacket>(0x1B, "Login Confirm", 37, ReceiveLoginConfirmPacket);
            Register<LoginCompletePacket>(0x55, "Login Complete", 1, ReceiveLoginComplete);
            Register<ServerPingPacket>(0x73, "Server Ping Packet", 2, ReceivePingPacket);
            Register<LoginRejectionPacket>(0x82, "Login Rejection", 2, ReceiveLoginRejection);
            Register<DeleteResultPacket>(0x85, "Delete Character Response", 2, ReceiveDeleteCharacterResponse);
            Register<CharacterListUpdatePacket>(0x86, "Character List Update", -1, ReceiveCharacterListUpdate);
            Register<ServerRelayPacket>(0x8C, "ServerRelay", 11, ReceiveServerRelay);
            Register<ServerListPacket>(0xA8, "Game Server List", -1, ReceiveServerList);
            Register<CharacterCityListPacket>(0xA9, "Characters / Starting Locations", -1, ReceiveCharacterList);
            Register<SupportedFeaturesPacket>(0xB9, "Supported Features", 3, ReceiveEnableFeatures);
            Register<VersionRequestPacket>(0xBD, "Version Request", -1, ReceiveVersionRequest);
        }

        public void StartKeepAlivePackets()
        {
            m_KeepAliveTimer = new Timer(
                e => SendKeepAlivePacket(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(60));
        }

        private void StopKeepAlivePackets()
        {
            if (m_KeepAliveTimer != null)
                m_KeepAliveTimer.Dispose();
        }

        private void SendKeepAlivePacket()
        {
            if (!m_Network.IsConnected)
            {
                StopKeepAlivePackets();
                return;
            }

            m_Network.Send(new ClientPingPacket());
        }

        public void Dispose()
        {
            StopKeepAlivePackets();

            for (int i = 0; i < m_RegisteredHandlers.Count; i++)
                m_Network.Unregister(m_RegisteredHandlers[i].Item1, m_RegisteredHandlers[i].Item2);
            m_RegisteredHandlers.Clear();
            m_RegisteredHandlers = null;
        }

        public void Register<T>(int id, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket
        {
            m_RegisteredHandlers.Add(new Tuple<int, TypedPacketReceiveHandler>(id, onReceive));
            m_Network.Register<T>(id, name, length, onReceive);
        }

        /// <summary>
        /// Connect to a server!
        /// </summary>
        /// <param name="host">Address of the host. Can be a website or an ip address. IP addresses should be IPv4.</param>
        /// <param name="port">Port of the server on the host.</param>
        /// <returns></returns>
        public bool Connect(string host, int port)
        {
            Status = LoginClientStatus.LoginServer_Connecting;
            bool success = m_Network.Connect(host, port);

            if (success)
            {
                Status = LoginClientStatus.LoginServer_WaitingForLogin;

                var clientVersion = Settings.UltimaOnline.ClientVersion;

                if (clientVersion.Length != 4)
                    Tracer.Warn("Cannot send seed packet: Version array is incorrectly sized.");
                else
                    m_Network.Send(new SeedPacket(1, clientVersion));
            }
            else
            {
                Status = LoginClientStatus.Error_CannotConnectToServer;
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
                StopKeepAlivePackets();
                m_Network.Disconnect();
            }

            Status = LoginClientStatus.Unconnected;
        }

        /// <summary>
        /// Attempts to login to the connected host.
        /// </summary>
        /// <param name="account">The username of the account to be logged in.</param>
        /// <param name="password">The password of the account to be logged in. This is encrypted in transit.</param>
        public void Login()
        {
            Status = LoginClientStatus.LoginServer_LoggingIn;

            m_Network.Send(new LoginPacket(Settings.Server.UserName, Password.ConvertToUnsecureString()));
        }

        /// <summary>
        /// Connect to the indicated relay server.
        /// </summary>
        /// <param name="account">The username of the account to be logged in.</param>
        /// <param name="password">The password of the account to be logged in. This is encrypted in transit.</param>
        public void Relay()
        {
            Status = LoginClientStatus.LoginServer_Relaying;
            m_Network.Send(new GameLoginPacket(m_ServerRelayKey, Settings.Server.UserName, Password.ConvertToUnsecureString()));
        }

        /// <summary>
        /// Sends a message to the server to request a connection to the specified shard.
        /// </summary>
        /// <param name="index">The index of the shard to connect to.</param>
        public void SelectShard(int index)
        {
            if (Status == LoginClientStatus.LoginServer_HasServerList)
            {
                Status = LoginClientStatus.GameServer_Connecting;
                m_Network.Send(new SelectServerPacket(index));
            }
        }

        /// <summary>
        /// Sends a message to the server to request login with the specified player.
        /// </summary>
        /// <param name="index">The index of the character to login with.</param>
        public void LoginWithCharacter(int index)
        {
            if (Status == LoginClientStatus.GameServer_CharList)
            {
                if (Characters.List[index].Name != string.Empty)
                {
                    m_Engine.QueuedModel = new WorldModel();
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
            m_Engine.QueuedModel = new WorldModel();
            m_Network.Send(packet);
        }

        /// <summary>
        /// Sends a message to the server, requesting that an existing character be deleted.
        /// </summary>
        /// <param name="index">The index of the character to be deleted.</param>
        public void DeleteCharacter(int index)
        {
            if (Status == LoginClientStatus.GameServer_CharList)
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
            DeleteResultPacket p = (DeleteResultPacket)packet;
            MsgBoxGump.Show(p.Result, MsgBoxTypes.OkOnly);
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
            Status = LoginClientStatus.GameServer_CharList;
        }

        private void ReceiveServerList(IRecvPacket packet)
        {
            ServerList.List = ((ServerListPacket)packet).Servers;
            Status = LoginClientStatus.LoginServer_HasServerList;
        }

        private void ReceiveLoginRejection(IRecvPacket packet)
        {
            Disconnect();
            LoginRejectionPacket p = (LoginRejectionPacket)packet;
            switch (p.Reason)
            {
                case LoginRejectionReasons.InvalidAccountPassword:
                    Status = LoginClientStatus.Error_InvalidUsernamePassword;
                    break;
                case LoginRejectionReasons.AccountInUse:
                    Status = LoginClientStatus.Error_InUse;
                    break;
                case LoginRejectionReasons.AccountBlocked:
                    Status = LoginClientStatus.Error_Blocked;
                    break;
                case LoginRejectionReasons.BadPassword:
                    Status = LoginClientStatus.Error_BadPassword;
                    break;
                case LoginRejectionReasons.IdleExceeded:
                    Status = LoginClientStatus.Error_Idle;
                    break;
                case LoginRejectionReasons.BadCommuncation:
                    Status = LoginClientStatus.Error_BadCommunication;
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
            Status = LoginClientStatus.LoginServer_WaitingForRelay;
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

        private void ReceivePingPacket(IRecvPacket packet)
        {

        }



        // ======================================================================
        // New login handling routines
        // ======================================================================

        LoginConfirmPacket m_QueuedLoginConfirmPacket;

        private void ReceiveLoginConfirmPacket(IRecvPacket packet)
        {
            m_QueuedLoginConfirmPacket = (LoginConfirmPacket)packet;
            // set the player serial and create the player entity. Don't need to do anything with it yet.
            WorldModel.PlayerSerial = m_QueuedLoginConfirmPacket.Serial;
            Mobile player = WorldModel.Entities.GetObject<Mobile>(m_QueuedLoginConfirmPacket.Serial, true);
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
            if (Status != LoginClientStatus.WorldServer_InWorld)
            {
                if (m_QueuedLoginConfirmPacket != null && (m_Engine.QueuedModel as WorldModel).MapIndex >= 0)
                {
                    Status = LoginClientStatus.WorldServer_InWorld;

                    m_Engine.ActivateQueuedModel();
                    if (m_Engine.ActiveModel is WorldModel)
                    {
                        ((WorldModel)m_Engine.ActiveModel).LoginToWorld();
                        LoginConfirmPacket packet = m_QueuedLoginConfirmPacket;
                        Mobile player = WorldModel.Entities.GetObject<Mobile>(m_QueuedLoginConfirmPacket.Serial, true);
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
