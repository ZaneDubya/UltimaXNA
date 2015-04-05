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
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaGUI;
using UltimaXNA.UltimaGUI.WorldGumps;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaPackets.Server;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Controllers;
using UltimaXNA.UltimaWorld.Views;
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

        private void Initialize()
        {
            Register<LoginConfirmPacket>(0x1B, "Login Confirm", 37, new TypedPacketReceiveHandler(receive_LoginConfirmPacket));
            Register<LoginCompletePacket>(0x55, "Login Complete", 1, new TypedPacketReceiveHandler(receive_LoginComplete));
            Register<LoginRejectionPacket>(0x82, "Login Rejection", 2, new TypedPacketReceiveHandler(receive_LoginRejection));
            Register<DeleteCharacterResponsePacket>(0x85, "Delete Character Response", 2, new TypedPacketReceiveHandler(receive_DeleteCharacterResponse));
            Register<CharacterListUpdatePacket>(0x86, "Character List Update", -1, new TypedPacketReceiveHandler(receive_CharacterListUpdate));
            Register<ServerRelayPacket>(0x8C, "ServerRelay", 11, new TypedPacketReceiveHandler(receive_ServerRelay));
            Register<ServerListPacket>(0xA8, "Game Server List", -1, new TypedPacketReceiveHandler(receive_ServerList));
            Register<CharacterCityListPacket>(0xA9, "Characters / Starting Locations", -1, new TypedPacketReceiveHandler(receive_CharacterList));
            Register<SupportedFeaturesPacket>(0xB9, "Supported Features", 3, new TypedPacketReceiveHandler(receive_EnableFeatures));
            Register<VersionRequestPacket>(0xBD, "Version Request", -1, new TypedPacketReceiveHandler(receive_VersionRequest));
        }

        public override bool Connect(string host, int port)
        {
            Status = UltimaClientStatus.LoginServer_Connecting;
            bool success = base.Connect(host, port);
            if (success)
            {
                Status = UltimaClientStatus.LoginServer_WaitingForLogin;
                Send(new SeedPacket(1, 6, 0, 6, 2));
            }
            else
            {
                Status = UltimaClientStatus.Error_CannotConnectToServer;
            }
            return success;
        }

        public void SendAccountLogin(string account, string password)
        {
            Status = UltimaClientStatus.LoginServer_LoggingIn;
            Send(new LoginPacket(account, password));
        }

        public void SendServerRelay(string account, string password)
        {
            Status = UltimaClientStatus.LoginServer_Relaying;
            Send(new GameLoginPacket(m_ServerRelayKey, account, password));
        }

        public void SelectServer(int index)
        {
            if (Status == UltimaClientStatus.LoginServer_HasServerList)
            {
                Status = UltimaClientStatus.GameServer_Connecting;
                Send(new SelectServerPacket(index));
            }
        }

        public void SelectCharacter(int index)
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

        public void CreateCharacter(CreateCharacterPacket packet)
        {
            Engine.QueuedModel = new WorldModel();
            Send(packet);
        }

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

        public override void Disconnect()
        {
            if (IsConnected)
                base.Disconnect();
            Status = UltimaClientStatus.Unconnected;
        }













        private void receive_DeleteCharacterResponse(IRecvPacket packet)
        {
            DeleteCharacterResponsePacket p = (DeleteCharacterResponsePacket)packet;
            WorldInteraction.MsgBox(p.Result, MsgBoxTypes.OkOnly);
        }

        private void receive_CharacterListUpdate(IRecvPacket packet)
        {
            CharacterListUpdatePacket p = (CharacterListUpdatePacket)packet;
            UltimaVars.Characters.SetCharacterList(p.Characters);
        }

        private void receive_CharacterList(IRecvPacket packet)
        {
            CharacterCityListPacket p = (CharacterCityListPacket)packet;
            UltimaVars.Characters.SetCharacterList(p.Characters);
            UltimaVars.Characters.SetStartingLocations(p.Locations);
            Status = UltimaClientStatus.GameServer_CharList;
        }

        private void receive_ServerList(IRecvPacket packet)
        {
            UltimaVars.Servers.List = ((ServerListPacket)packet).Servers;
            Status = UltimaClientStatus.LoginServer_HasServerList;
        }

        private void receive_LoginRejection(IRecvPacket packet)
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

        private void receive_ServerRelay(IRecvPacket packet)
        {
            ServerRelayPacket p = (ServerRelayPacket)packet;
            m_ServerRelayKey = p.AccountId;
            // Normally, upon receiving this packet you would disconnect and
            // log in to the specified server. Since we are using RunUO, we don't
            // actually need to do 
            IsDecompressionEnabled = true;
            Status = UltimaClientStatus.LoginServer_WaitingForRelay;
        }

        private void receive_EnableFeatures(IRecvPacket packet)
        {
            SupportedFeaturesPacket p = (SupportedFeaturesPacket)packet;
            UltimaVars.Features.SetFlags(p.Flags);
        }

        private void receive_VersionRequest(IRecvPacket packet)
        {
            // Automatically respond.
            SendClientVersion("6.0.1.10");
        }

        public void SendClientVersion(string version_string)
        {
            Send(new ClientVersionPacket(version_string));
        }

        // ======================================================================
        // New login handling routines
        // ======================================================================

        LoginConfirmPacket m_QueuedLoginConfirmPacket;

        private void receive_LoginConfirmPacket(IRecvPacket packet)
        {
            m_QueuedLoginConfirmPacket = (LoginConfirmPacket)packet;
            // set the player serial var and create the player entity. Don't need to do anything with it yet.
            UltimaVars.EngineVars.PlayerSerial = m_QueuedLoginConfirmPacket.Serial;
            PlayerMobile player = EntityManager.GetObject<PlayerMobile>(m_QueuedLoginConfirmPacket.Serial, true);
            InternalCheckLogin();
        }

        private void receive_LoginComplete(IRecvPacket packet)
        {
            // This packet is just one byte, the opcode.
            InternalCheckLogin();
        }

        private void InternalCheckLogin()
        {
            // We want to make sure we have the client object before we load the world.
            // If we don't, just set the status to login complete, which will then
            // load the world when we finally receive our client object.
            if (Status != UltimaClientStatus.WorldServer_InWorld)
            {
                if (m_QueuedLoginConfirmPacket != null && (Engine.QueuedModel as WorldModel).MapIndex >= 0)
                {
                    Status = UltimaClientStatus.WorldServer_InWorld;

                    Engine.ActivateQueuedModel();
                    if (Engine.ActiveModel is WorldModel)
                    {
                        ((WorldModel)Engine.ActiveModel).LoginSequence();

                        LoginConfirmPacket p = m_QueuedLoginConfirmPacket;

                        PlayerMobile player = EntityManager.GetObject<PlayerMobile>(p.Serial, true);
                        player.Move_Instant(p.X, p.Y, p.Z, p.Direction);
                        // iPlayer.SetFacing(p.Direction);
                    }
                    else
                    {
                        Logger.Fatal("Not in world model at login.");
                    }
                }
            }
            else
            {
                // already logged in!
            }
        }
    }
}
