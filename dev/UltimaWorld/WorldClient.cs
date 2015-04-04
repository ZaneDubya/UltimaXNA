using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaPackets;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaPackets.Server;

namespace UltimaXNA.UltimaWorld
{
    class WorldClient
    {
        protected WorldModel World
        {
            get;
            private set;
        }

        private List<Tuple<int, TypedPacketReceiveHandler>> m_RegisteredHandlers;

        public WorldClient(WorldModel world)
        {
            World = world;

            m_RegisteredHandlers = new List<Tuple<int, TypedPacketReceiveHandler>>();
        }

        public void Initialize()
        {
            Register<VersionRequestPacket>(0xBD, "Version Request", -1, new TypedPacketReceiveHandler(receive_VersionRequest));
            Register<TargetCursorPacket>(0x6C, "TargetCursor", 19, new TypedPacketReceiveHandler(receive_TargetCursor));
            Register<TargetCursorMultiPacket>(0x99, "Target Cursor Multi Object", 26, new TypedPacketReceiveHandler(receive_TargetCursorMulti));

            Register<GraphicEffectPacket>(0x70, "Graphical Effect 1", 28, new TypedPacketReceiveHandler(receive_GraphicEffect));
            Register<GraphicEffectHuedPacket>(0xC0, "Hued Effect", 36, new TypedPacketReceiveHandler(receive_HuedEffect));
            Register<GraphicEffectExtendedPacket>(0xC7, "Particle Effect", 49, new TypedPacketReceiveHandler(receive_OnParticleEffect));

            Register<ContainerContentUpdatePacket>(0x25, "Container Content Update", 21, new TypedPacketReceiveHandler(receive_AddSingleItemToContainer));
            Register<ContainerContentPacket>(0x3C, "Container Content", -1, new TypedPacketReceiveHandler(receive_AddMultipleItemsToContainer));

            Register<MobileIncomingPacket>(0x78, "Mobile Incoming", -1, new TypedPacketReceiveHandler(receive_MobileIncoming));

            Register<WorldItemPacket>(0x1A, "World Item", -1, new TypedPacketReceiveHandler(receive_WorldItem));
            Register<WornItemPacket>(0x2E, "Worn Item", 15, new TypedPacketReceiveHandler(receive_WornItem));

            UltimaEntities.MobileMovement.SendMoveRequestPacket += InternalOnEntity_SendMoveRequestPacket;
        }

        public void Dispose()
        {
            for (int i = 0; i < m_RegisteredHandlers.Count; i++)
                World.Engine.Client.Unregister(m_RegisteredHandlers[i].Item1, m_RegisteredHandlers[i].Item2);
            m_RegisteredHandlers.Clear();
            m_RegisteredHandlers = null;

            UltimaEntities.MobileMovement.SendMoveRequestPacket -= InternalOnEntity_SendMoveRequestPacket;
        }

        public void Register<T>(int id, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket
        {
            m_RegisteredHandlers.Add(new Tuple<int, TypedPacketReceiveHandler>(id, onReceive));
            World.Engine.Client.Register<T>(id, name, length, onReceive);
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

        // ======================================================================
        // Item handling
        // ======================================================================

        private void receive_AddMultipleItemsToContainer(IRecvPacket packet)
        {
            ContainerContentPacket p = (ContainerContentPacket)packet;
            foreach (ContentItem i in p.Items)
            {
                // Add the item...
                Item item = add_Item(i.Serial, i.ItemID, i.Hue, i.ContainerSerial, i.Amount);
                item.InContainerPosition = new Point(i.X, i.Y);
                // ... and add it the container contents of the container.
                Container c = EntityManager.GetObject<Container>(i.ContainerSerial, true);
                c.AddItem(item);
            }
        }

        private void receive_AddSingleItemToContainer(IRecvPacket packet)
        {
            ContainerContentUpdatePacket p = (ContainerContentUpdatePacket)packet;

            // Add the item...
            Item item = add_Item(p.Serial, p.ItemId, p.Hue, p.ContainerSerial, p.Amount);
            item.InContainerPosition = new Point(p.X, p.Y);
            // ... and add it the container contents of the container.
            Container iContainerObject = EntityManager.GetObject<Container>(p.ContainerSerial, true);
            if (iContainerObject != null)
                iContainerObject.AddItem(item);
            else
            {
                // Special case for game boards... the server will sometimes send us game pieces for a game board before it sends 
                // the game board! Right now, I am discarding these messages, it might be better to queue them up for when the game
                // board actually exists.
                // Let's throw an exception if anything other than a gameboard is ever sent to us.
                // if (iObject.ItemData.Name != "game piece")
                throw new Exception("Item {" + item.ToString() + "} received before containing object received.");
            }
        }

        // ======================================================================
        // Item handling
        // ======================================================================

        private Item add_Item(Serial serial, int itemID, int nHue, Serial parentSerial, int amount)
        {
            Item item;
            if (itemID == 0x2006)
            {
                // special case for corpses.
                item = EntityManager.GetObject<Corpse>((int)serial, true);
                ContainerContentPacket.NextContainerContentsIsPre6017 = true;
            }
            else
            {
                if (UltimaData.TileData.ItemData[itemID].IsContainer)
                    item = EntityManager.GetObject<Container>((int)serial, true);
                else
                    item = EntityManager.GetObject<Item>((int)serial, true);
            }
            item.Amount = amount;
            item.ItemID = itemID;
            item.Hue = nHue;
            return item;
        }

        private void receive_WorldItem(IRecvPacket packet)
        {
            WorldItemPacket p = (WorldItemPacket)packet;
            // Now create the GameObject.
            // If the iItemID < 0x4000, this is a regular game object.
            // If the iItemID >= 0x4000, then this is a multiobject.
            if (p.ItemID <= 0x4000)
            {
                Item item = add_Item(p.Serial, p.ItemID, p.Hue, 0, p.StackAmount);
                item.Position.Set(p.X, p.Y, p.Z);
            }
            else
            {
                // create a multi object. Unhandled !!!
                int multiID = p.ItemID - 0x4000;
                Multi multi = EntityManager.GetObject<Multi>(p.Serial, true);
                multi.Position.Set(p.X, p.Y, p.Z);
                multi.MultiID = p.ItemID;
            }
        }

        private void receive_WornItem(IRecvPacket packet)
        {
            WornItemPacket p = (WornItemPacket)packet;
            Item item = add_Item(p.Serial, p.ItemId, p.Hue, p.ParentSerial, 0);
            Mobile m = EntityManager.GetObject<Mobile>(p.ParentSerial, false);
            m.WearItem(item, p.Layer);
            if (item.PropertyList.Hash == 0)
                World.Engine.Client.Send(new QueryPropertiesPacket(item.Serial));
        }

        // ======================================================================
        // Mobile handling
        // ======================================================================

        private void receive_MobileIncoming(IRecvPacket packet)
        {
            MobileIncomingPacket p = (MobileIncomingPacket)packet;
            Mobile mobile = EntityManager.GetObject<Mobile>(p.Serial, true);
            mobile.BodyID = p.BodyID;
            mobile.Hue = (int)p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            mobile.Flags = p.Flags;
            mobile.Notoriety = p.Notoriety;
            mobile.Notoriety = p.Notoriety;

            for (int i = 0; i < p.Equipment.Length; i++)
            {
                Item item = add_Item(p.Equipment[i].Serial, p.Equipment[i].GumpId, p.Equipment[i].Hue, p.Serial, 0);
                mobile.WearItem(item, p.Equipment[i].Layer);
                if (item.PropertyList.Hash == 0)
                    World.Engine.Client.Send(new QueryPropertiesPacket(item.Serial));
            }

            if (mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                World.Engine.Client.Send(new RequestNamePacket(p.Serial));
            }
        }
    }
}
