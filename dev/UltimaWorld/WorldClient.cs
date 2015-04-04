using UltimaXNA.Core.Network;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaPackets.Server;
using UltimaXNA.UltimaWorld.Controllers;

namespace UltimaXNA.UltimaWorld
{
    class WorldClient
    {
        protected WorldModel World
        {
            get;
            private set;
        }

        public WorldClient(WorldModel world)
        {
            World = world;
        }

        public void Initialize()
        {
            Register<VersionRequestPacket>(0xBD, "Version Request", -1, new TypedPacketReceiveHandler(receive_VersionRequest));
            Register<TargetCursorPacket>(0x6C, "TargetCursor", 19, new TypedPacketReceiveHandler(receive_TargetCursor));
            Register<TargetCursorMultiPacket>(0x99, "Target Cursor Multi Object", 26, new TypedPacketReceiveHandler(receive_TargetCursorMulti));

            Register<GraphicEffectPacket>(0x70, "Graphical Effect 1", 28, new TypedPacketReceiveHandler(receive_GraphicEffect));
            Register<GraphicEffectHuedPacket>(0xC0, "Hued Effect", 36, new TypedPacketReceiveHandler(receive_HuedEffect));
            Register<GraphicEffectExtendedPacket>(0xC7, "Particle Effect", 49, new TypedPacketReceiveHandler(receive_OnParticleEffect));

            UltimaEntities.MobileMovement.SendMoveRequestPacket += InternalOnEntity_SendMoveRequestPacket;
        }

        public void Dispose()
        {
            Unregister(0xBD, receive_VersionRequest);
            Unregister(0x6C, receive_TargetCursor);
            Unregister(0x99, receive_TargetCursorMulti);

            UltimaEntities.MobileMovement.SendMoveRequestPacket -= InternalOnEntity_SendMoveRequestPacket;
        }

        public void Register<T>(int id, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket
        {

        }

        public void Unregister(int id, TypedPacketReceiveHandler onReceive)
        {

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
            World.Engine.Client.Send(new GetPlayerStatusPacket(0x05, UltimaVars.EngineVars.PlayerSerial));
        }

        public void SendClientVersion(string version_string)
        {
            World.Engine.Client.Send(new ClientVersionPacket(version_string));
        }

        public void SendClientScreenSize()
        {
            World.Engine.Client.Send(new ReportClientScreenSizePacket(800, 600));
        }

        public void SendClientLocalization()
        {
            World.Engine.Client.Send(new ReportClientLocalizationPacket("ENU"));
        }

        public void GetMyBasicStatus()
        {
            World.Engine.Client.Send(new GetPlayerStatusPacket(0x04, UltimaVars.EngineVars.PlayerSerial));
        }

        private void receive_VersionRequest(IRecvPacket packet)
        {
            // Automatically respond.
            SendClientVersion("6.0.1.10");
        }

        private void receive_TargetCursor(IRecvPacket packet)
        {
            TargetCursorPacket p = (TargetCursorPacket)packet;
            World.Cursor.SetTargeting((WorldCursor.TargetType)p.CommandType, p.CursorID);
        }

        private void receive_TargetCursorMulti(IRecvPacket packet)
        {
            TargetCursorMultiPacket p = (TargetCursorMultiPacket)packet;
            World.Cursor.SetTargetingMulti(p.DeedSerial, p.MultiModel);
        }

        private void InternalOnEntity_SendMoveRequestPacket(MoveRequestPacket packet)
        {
            World.Engine.Client.Send(packet);
        }

        // ======================================================================
        // Effect handling
        // ======================================================================

        private void receive_GraphicEffect(IRecvPacket packet)
        {
            EffectsManager.Add((GraphicEffectPacket)packet);
        }

        private void receive_HuedEffect(IRecvPacket packet)
        {
            EffectsManager.Add((GraphicEffectHuedPacket)packet);
        }

        private void receive_OnParticleEffect(IRecvPacket packet)
        {
            EffectsManager.Add((GraphicEffectExtendedPacket)packet);
        }
    }
}
