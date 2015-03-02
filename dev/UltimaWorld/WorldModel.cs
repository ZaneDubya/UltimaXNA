using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Patterns.MVC;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaGUI.Gumps;
using UltimaXNA.UltimaGUI;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaPackets.Server;
using UltimaXNA.Core.Network;

namespace UltimaXNA.UltimaWorld
{
    class WorldModel : AUltimaModel
    {
        private EntityManager m_Entities;
        public EntityManager Entities
        {
            get { return m_Entities; }
        }

        public WorldModel()
        {
            m_Entities = new EntityManager();
        }

        protected override void OnInitialize()
        {
            Client.Register<VersionRequestPacket>(0xBD, "Version Request", -1, new TypedPacketReceiveHandler(receive_VersionRequest));
            Entity.Movement.SendMoveRequestPacket += InternalOnEntity_SendMoveRequestPacket;

            UltimaEngine.UserInterface.AddControl(new TopMenu(0), 0, 0);
            UltimaEngine.UserInterface.AddControl(new ChatWindow(), 0, 0);

            // this is the login sequence for 0.6.1.10
            GetMySkills();
            SendClientVersion("6.0.1.10");
            SendClientScreenSize();
            SendClientLocalization();
            // Packet: BF 00 0A 00 0F 0A 00 00 00 1F
            // Packet: 09 00 00 00 02  
            // Packet: 06 80 00 00 17
            GetMyBasicStatus();
            // Packet: D6 00 0B 00 00 00 02 00 00 00 17
            // Packet: D6 00 37 40 00 00 FB 40 00 00 FD 40 00 00 FE 40
            //         00 00 FF 40 00 01 00 40 00 01 02 40 00 01 03 40
            //         00 01 04 40 00 01 05 40 00 01 06 40 00 01 07 40
            //         00 01 24 40 00 01 26 
            IsometricRenderer.LightDirection = -0.6f;
            UltimaVars.EngineVars.InWorld = true;
        }

        protected override void OnDispose()
        {
            Client.Unregister(0xBD, receive_VersionRequest);
            Entity.Movement.SendMoveRequestPacket -= InternalOnEntity_SendMoveRequestPacket;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (!Client.IsConnected)
            {
                if (UltimaEngine.UserInterface.IsModalControlOpen == false)
                {
                    MsgBox g = UltimaEngine.UltimaUI.MsgBox("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
                    g.OnClose = onCloseLostConnectionMsgBox;
                }
            }
            else
            {
                EntityManager.Update(frameMS);
                IsometricRenderer.CenterPosition = EntityManager.GetPlayerObject().Position;
                IsometricRenderer.Update(totalMS, frameMS);

                // Toggle for logout
                if (UltimaEngine.Input.HandleKeyboardEvent(KeyboardEventType.Down, WinKeys.Q, false, false, true))
                    UltimaInteraction.DisconnectToLoginScreen();
            }
        }

        void onCloseLostConnectionMsgBox()
        {
            Client.Disconnect();
            UltimaInteraction.DisconnectToLoginScreen();
        }

        public void GetMySkills()
        {
            Client.Send(new GetPlayerStatusPacket(0x05, EntityManager.MySerial));
        }

        public void SendClientVersion(string version_string)
        {
            Client.Send(new ClientVersionPacket(version_string));
        }

        public void SendClientScreenSize()
        {
            Client.Send(new ReportClientScreenSizePacket(800, 600));
        }

        public void SendClientLocalization()
        {
            Client.Send(new ReportClientLocalizationPacket("ENU"));
        }

        public void GetMyBasicStatus()
        {
            Client.Send(new GetPlayerStatusPacket(0x04, EntityManager.MySerial));
        }

        private void receive_VersionRequest(IRecvPacket packet)
        {
            // Automatically respond.
            SendClientVersion("6.0.1.10");
        }

        private void InternalOnEntity_SendMoveRequestPacket(MoveRequestPacket packet)
        {
            Client.Send(packet);
        }
    }
}
