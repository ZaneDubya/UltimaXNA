/***************************************************************************
 *   LoginClient.cs
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
using UltimaXNA.Ultima.Input;
using UltimaXNA.Ultima.Login.Accounts;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.Network.Server;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.Login {
    public class LoginClient : IDisposable {
        readonly INetworkClient m_Network;
        readonly UltimaGame m_Engine;
        readonly UserInterfaceService m_UserInterface;
        Timer m_KeepAliveTimer;
        List<Tuple<int, TypedPacketReceiveHandler>> m_RegisteredHandlers;
        string m_UserName;
        SecureString m_Password;

        public LoginClient() {
            m_Network = ServiceRegistry.GetService<INetworkClient>();
            m_Engine = ServiceRegistry.GetService<UltimaGame>();
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_RegisteredHandlers = new List<Tuple<int, TypedPacketReceiveHandler>>();
            Initialize();
        }

        // ============================================================================================================
        // Packet registration and unregistration
        // ============================================================================================================
        void Initialize() {
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

        public void Dispose() {
            StopKeepAlivePackets();
            for (int i = 0; i < m_RegisteredHandlers.Count; i++) {
                m_Network.Unregister(m_RegisteredHandlers[i].Item1, m_RegisteredHandlers[i].Item2);
            }
            m_RegisteredHandlers.Clear();
            m_RegisteredHandlers = null;
        }

        public void Register<T>(int id, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket {
            m_RegisteredHandlers.Add(new Tuple<int, TypedPacketReceiveHandler>(id, onReceive));
            m_Network.Register<T>(id, name, length, onReceive);
        }

        public void Unregister(int id) {
            for (int i = 0; i < m_RegisteredHandlers.Count; i++) {
                if (m_RegisteredHandlers[i].Item1 == id) {
                    m_Network.Unregister(m_RegisteredHandlers[i].Item1, m_RegisteredHandlers[i].Item2);
                    m_RegisteredHandlers.RemoveAt(i);
                    i--;
                }
            }
        }

        // ============================================================================================================
        // Connection and Disconnect
        // ============================================================================================================
        public bool Connect(string host, int port, string account, SecureString password) {
            Disconnect();
            if (m_Network.Connect(host, port)) {
                m_UserName = account;
                m_Password = password;
                m_Network.Send(new SeedPacket(0x1337BEEF, Settings.UltimaOnline.PatchVersion));
                Login(m_UserName, m_Password);
                return true;
            }
            return false;
        }

        public void Disconnect() {
            StopKeepAlivePackets();
            if (m_Network.IsConnected) {
                m_Network.Disconnect();
            }
        }

        // ============================================================================================================
        // Login sequence routines
        // ============================================================================================================
        public void Login(string account, SecureString password) {
            m_Network.Send(new LoginPacket(account, password.ConvertToUnsecureString()));
        }

        public void SelectShard(int index) {
            m_Network.Send(new SelectServerPacket(index));
        }

        public void LoginWithCharacter(int index) {
            if (Characters.List[index].Name != string.Empty) {
                m_Engine.QueuedModel = new WorldModel();
                m_Network.Send(new LoginCharacterPacket(Characters.List[index].Name, index, Utility.IPAddress));
                Macros.Player.Load(Characters.List[index].Name);
            }
        }

        public void CreateCharacter(CreateCharacterPacket packet) {
            m_Engine.QueuedModel = new WorldModel();
            m_Network.Send(packet);
        }

        public void DeleteCharacter(int index) {
            if (index == -1) {
                return;
            }
            if (Characters.List[index].Name != string.Empty) {
                m_Network.Send(new DeleteCharacterPacket(index, Utility.IPAddress));
            }
        }

        public void SendClientVersion() {
            if (ClientVersion.HasExtendedFeatures(Settings.UltimaOnline.PatchVersion)) {
                Tracer.Info("Client version is greater than 6.0.14.2, enabling extended 0xB9 packet.");
                Unregister(0xB9);
                Register<SupportedFeaturesPacket>(0xB9, "Supported Features Extended", 5, ReceiveEnableFeatures);
            }
            m_Network.Send(new ClientVersionPacket(Settings.UltimaOnline.PatchVersion));
        }

        void ReceiveDeleteCharacterResponse(IRecvPacket packet) {
            DeleteResultPacket p = (DeleteResultPacket)packet;
            MsgBoxGump.Show(p.Result, MsgBoxTypes.OkOnly);
        }

        void ReceiveCharacterListUpdate(IRecvPacket packet) {
            CharacterListUpdatePacket p = (CharacterListUpdatePacket)packet;
            Characters.SetCharacterList(p.Characters);
            (m_Engine.ActiveModel as LoginModel).ShowCharacterList();
        }

        void ReceiveCharacterList(IRecvPacket packet) {
            CharacterCityListPacket p = (CharacterCityListPacket)packet;
            Characters.SetCharacterList(p.Characters);
            Characters.SetStartingLocations(p.Locations);
            StartKeepAlivePackets();
            (m_Engine.ActiveModel as LoginModel).ShowCharacterList();
        }

        void ReceiveServerList(IRecvPacket packet) {
            (m_Engine.ActiveModel as LoginModel).ShowServerList(((ServerListPacket)packet).Servers);
        }

        void ReceiveLoginRejection(IRecvPacket packet) {
            Disconnect();
            LoginRejectionPacket p = (LoginRejectionPacket)packet;
            (m_Engine.ActiveModel as LoginModel).ShowLoginRejection(p.Reason);
        }

        void ReceiveServerRelay(IRecvPacket packet) {
            // On OSI, upon receiving this packet, the client would disconnect and
            // log in to the specified server. Since emulated servers use the same
            // server for both shard selection and world, we don't need to disconnect.
            ServerRelayPacket p = (ServerRelayPacket)packet;
            m_Network.IsDecompressionEnabled = true;
            m_Network.Send(new GameLoginPacket(p.AccountId, m_UserName, m_Password.ConvertToUnsecureString()));
        }

        void ReceiveEnableFeatures(IRecvPacket packet) {
            SupportedFeaturesPacket p = (SupportedFeaturesPacket)packet;
            PlayerState.ClientFeatures.SetFlags(p.Flags);
        }

        void ReceiveVersionRequest(IRecvPacket packet) {
            SendClientVersion();
        }

        void ReceivePingPacket(IRecvPacket packet) {

        }

        // ============================================================================================================
        // Login to World - Nominally, the server should send LoginConfirmPacket, followed by GeneralInfo0x08, and 
        // finally LoginCompletePacket. However, the legacy client finds it valid to receive the packets in any
        // order. The code below allows any of these possibilities.
        // ============================================================================================================
        LoginConfirmPacket m_QueuedLoginConfirmPacket;
        bool m_LoggingInToWorld;

        void ReceiveLoginConfirmPacket(IRecvPacket packet) {
            m_QueuedLoginConfirmPacket = (LoginConfirmPacket)packet;
            // set the player serial and create the player entity. Don't need to do anything with it yet.
            WorldModel.PlayerSerial = m_QueuedLoginConfirmPacket.Serial;
            Mobile player = WorldModel.Entities.GetObject<Mobile>(WorldModel.PlayerSerial, true);
            if (player == null) {
                Tracer.Critical("Could not create player object.");
            }
            CheckIfOkayToLogin();
        }

        void ReceiveLoginComplete(IRecvPacket packet) {
            CheckIfOkayToLogin();
        }

        void CheckIfOkayToLogin() {
            // Before the client logs in, we need to know the player entity's serial, and the
            // map the player will be loading on login. If we don't have either of these, we
            // delay loading until we do.
            if (!m_LoggingInToWorld) {
                if ((m_Engine.QueuedModel as WorldModel).MapIndex != 0xffffffff) { // will be 0xffffffff if no map
                    m_LoggingInToWorld = true; // stops double log in attempt caused by out of order packets.
                    m_Engine.ActivateQueuedModel();
                    if (m_Engine.ActiveModel is WorldModel) {
                        (m_Engine.ActiveModel as WorldModel).LoginToWorld();
                        Mobile player = WorldModel.Entities.GetObject<Mobile>(m_QueuedLoginConfirmPacket.Serial, true);
                        if (player == null) {
                            Tracer.Critical("No player object ready in CheckIfOkayToLogin().");
                        }
                        player.Move_Instant(
                            m_QueuedLoginConfirmPacket.X, m_QueuedLoginConfirmPacket.Y,
                            m_QueuedLoginConfirmPacket.Z, m_QueuedLoginConfirmPacket.Direction);
                    }
                    else {
                        Tracer.Critical("Not in world model at login.");
                    }
                }
            }
        }

        // ============================================================================================================
        // Keep-alive packets - These are necessary because the client does not otherwise send packets during character
        // creation, and the server will disconnect after a given length of inactivity.
        // ============================================================================================================
        void StartKeepAlivePackets() {
            if (m_KeepAliveTimer == null) {
                m_KeepAliveTimer = new Timer(e => SendKeepAlivePacket(), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
            }
        }

        void StopKeepAlivePackets() {
            if (m_KeepAliveTimer != null) {
                m_KeepAliveTimer.Dispose();
            }
        }

        void SendKeepAlivePacket() {
            if (m_Network.IsConnected) {
                m_Network.Send(new ClientPingPacket());
            }
            else {
                StopKeepAlivePackets();
            }
        }
    }
}