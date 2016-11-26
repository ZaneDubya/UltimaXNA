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

namespace UltimaXNA.Ultima.Login
{
    public class LoginClient : IDisposable {
        readonly INetworkClient m_Network;
        readonly UltimaGame m_Engine;
        readonly UserInterfaceService m_UserInterface;
        Timer m_KeepAliveTimer;
        string m_UserName;
        SecureString m_Password;

        public LoginClient() {
            m_Network = Services.Get<INetworkClient>();
            m_Engine = Services.Get<UltimaGame>();
            m_UserInterface = Services.Get<UserInterfaceService>();
            Initialize();
        }

        // ============================================================================================================
        // Packet registration and unregistration
        // ============================================================================================================
        void Initialize() {
            Register<LoginConfirmPacket>(0x1B, 37, ReceiveLoginConfirmPacket);
            Register<LoginCompletePacket>(0x55, 1, ReceiveLoginComplete);
            Register<ServerPingPacket>(0x73, 2, ReceivePingPacket);
            Register<LoginRejectionPacket>(0x82, 2, ReceiveLoginRejection);
            Register<DeleteResultPacket>(0x85, 2, ReceiveDeleteCharacterResponse);
            Register<CharacterListUpdatePacket>(0x86, -1, ReceiveCharacterListUpdate);
            Register<ServerRelayPacket>(0x8C, 11, ReceiveServerRelay);
            Register<ServerListPacket>(0xA8, -1, ReceiveServerList);
            Register<CharacterCityListPacket>(0xA9, -1, ReceiveCharacterList);
            Register<SupportedFeaturesPacket>(0xB9, 3, ReceiveEnableFeatures);
            Register<VersionRequestPacket>(0xBD, -1, ReceiveVersionRequest);
        }

        public void Dispose() {
            StopKeepAlivePackets();
            m_Network.Unregister(this);
        }

        public void Register<T>(int id, int length, Action<T> onReceive) where T : IRecvPacket {
            m_Network.Register(this, id, length, onReceive);
        }

        public void Unregister(int id) {
            m_Network.Unregister(this, id);
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
                m_Engine.Models.Next = new WorldModel();
                m_Network.Send(new LoginCharacterPacket(Characters.List[index].Name, index, Utility.IPAddress));
                Macros.Player.Load(Characters.List[index].Name);
            }
        }

        public void CreateCharacter(CreateCharacterPacket packet) {
            m_Engine.Models.Next = new WorldModel();
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
                Register<SupportedFeaturesPacket>(0xB9, 5, ReceiveEnableFeatures);
            }
            m_Network.Send(new ClientVersionPacket(Settings.UltimaOnline.PatchVersion));
        }

        void ReceiveDeleteCharacterResponse(DeleteResultPacket packet) {
            MsgBoxGump.Show(packet.Result, MsgBoxTypes.OkOnly);
        }

        void ReceiveCharacterListUpdate(CharacterListUpdatePacket packet) {
            Characters.SetCharacterList(packet.Characters);
            (m_Engine.Models.Current as LoginModel).ShowCharacterList();
        }

        void ReceiveCharacterList(CharacterCityListPacket packet) {
            Characters.SetCharacterList(packet.Characters);
            Characters.SetStartingLocations(packet.Locations);
            StartKeepAlivePackets();
            (m_Engine.Models.Current as LoginModel).ShowCharacterList();
        }

        void ReceiveServerList(ServerListPacket packet) {
            (m_Engine.Models.Current as LoginModel).ShowServerList((packet).Servers);
        }

        void ReceiveLoginRejection(LoginRejectionPacket packet) {
            Disconnect();
            (m_Engine.Models.Current as LoginModel).ShowLoginRejection(packet.Reason);
        }

        void ReceiveServerRelay(ServerRelayPacket packet) {
            // On OSI, upon receiving this packet, the client would disconnect and
            // log in to the specified server. Since emulated servers use the same
            // server for both shard selection and world, we don't need to disconnect.
            m_Network.IsDecompressionEnabled = true;
            m_Network.Send(new GameLoginPacket(packet.AccountId, m_UserName, m_Password.ConvertToUnsecureString()));
        }

        void ReceiveEnableFeatures(SupportedFeaturesPacket packet) {
            PlayerState.ClientFeatures.SetFlags(packet.Flags);
        }

        void ReceiveVersionRequest(VersionRequestPacket packet) {
            SendClientVersion();
        }

        void ReceivePingPacket(ServerPingPacket packet) {

        }

        // ============================================================================================================
        // Login to World - Nominally, the server should send LoginConfirmPacket, followed by GeneralInfo0x08, and 
        // finally LoginCompletePacket. However, the legacy client finds it valid to receive the packets in any
        // order. The code below allows any of these possibilities.
        // ============================================================================================================
        LoginConfirmPacket m_QueuedLoginConfirmPacket;
        bool m_LoggingInToWorld;

        void ReceiveLoginConfirmPacket(LoginConfirmPacket packet) {
            m_QueuedLoginConfirmPacket = packet;
            // set the player serial and create the player entity. Don't need to do anything with it yet.
            WorldModel.PlayerSerial = packet.Serial;
            Mobile player = WorldModel.Entities.GetObject<Mobile>(WorldModel.PlayerSerial, true);
            if (player == null) {
                Tracer.Critical("Could not create player object.");
            }
            CheckIfOkayToLogin();
        }

        void ReceiveLoginComplete(LoginCompletePacket packet) {
            CheckIfOkayToLogin();
        }

        void CheckIfOkayToLogin() {
            // Before the client logs in, we need to know the player entity's serial, and the
            // map the player will be loading on login. If we don't have either of these, we
            // delay loading until we do.
            if (!m_LoggingInToWorld) {
                if ((m_Engine.Models.Next as WorldModel).MapIndex != 0xffffffff) { // will be 0xffffffff if no map
                    m_LoggingInToWorld = true; // stops double log in attempt caused by out of order packets.
                    m_Engine.Models.ActivateNext();
                    if (m_Engine.Models.Current is WorldModel) {
                        (m_Engine.Models.Current as WorldModel).LoginToWorld();
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