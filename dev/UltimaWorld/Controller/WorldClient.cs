using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Patterns.MVC;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaGUI.LoginGumps;
using UltimaXNA.UltimaGUI;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaPackets.Server;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaWorld.Controller;

namespace UltimaXNA.UltimaWorld.Controller
{
    class WorldClient
    {
        private WorldModel m_Model;

        public WorldClient(WorldModel model)
        {
            m_Model = model;
        }

        public void Initialize()
        {
            m_Model.Client.Register<VersionRequestPacket>(0xBD, "Version Request", -1, new TypedPacketReceiveHandler(receive_VersionRequest));
            m_Model.Client.Register<TargetCursorPacket>(0x6C, "TargetCursor", 19, new TypedPacketReceiveHandler(receive_TargetCursor));
            m_Model.Client.Register<TargetCursorMultiPacket>(0x99, "Target Cursor Multi Object", 26, new TypedPacketReceiveHandler(receive_TargetCursorMulti));

            Entity.MobileMovement.SendMoveRequestPacket += InternalOnEntity_SendMoveRequestPacket;
        }

        public void Dispose()
        {
            m_Model.Client.Unregister(0xBD, receive_VersionRequest);
            m_Model.Client.Unregister(0x6C, receive_TargetCursor);
            m_Model.Client.Unregister(0x99, receive_TargetCursorMulti);

            Entity.MobileMovement.SendMoveRequestPacket -= InternalOnEntity_SendMoveRequestPacket;
        }

        public void AfterLoginSequence()
        {
            // this is the after login sequence for 0.6.1.10
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
        }

        public void GetMySkills()
        {
            m_Model.Client.Send(new GetPlayerStatusPacket(0x05, UltimaVars.EngineVars.PlayerSerial));
        }

        public void SendClientVersion(string version_string)
        {
            m_Model.Client.Send(new ClientVersionPacket(version_string));
        }

        public void SendClientScreenSize()
        {
            m_Model.Client.Send(new ReportClientScreenSizePacket(800, 600));
        }

        public void SendClientLocalization()
        {
            m_Model.Client.Send(new ReportClientLocalizationPacket("ENU"));
        }

        public void GetMyBasicStatus()
        {
            m_Model.Client.Send(new GetPlayerStatusPacket(0x04, UltimaVars.EngineVars.PlayerSerial));
        }

        private void receive_VersionRequest(IRecvPacket packet)
        {
            // Automatically respond.
            SendClientVersion("6.0.1.10");
        }

        private void receive_TargetCursor(IRecvPacket packet)
        {
            TargetCursorPacket p = (TargetCursorPacket)packet;
            m_Model.Cursor.SetTargeting((TargetTypes)p.CommandType, p.CursorID);
        }

        private void receive_TargetCursorMulti(IRecvPacket packet)
        {
            // TargetCursorMultiPacket p = (TargetCursorMultiPacket)packet;
            // m_Model.Input.MouseTargeting(TargetTypes.MultiPlacement, 0);
            // UltimaInteraction.Cursor.TargetingMulti = p.MultiModel;
            Diagnostics.Logger.Debug("receive_TargetCursorMulti is not implemented!");
        }

        private void InternalOnEntity_SendMoveRequestPacket(MoveRequestPacket packet)
        {
            m_Model.Client.Send(packet);
        }
    }
}
