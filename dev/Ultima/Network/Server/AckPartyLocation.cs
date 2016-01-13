using Microsoft.Xna.Framework.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Entities.Mobiles;

namespace UltimaXNA.Ultima.Network.Server
{
    class AckPartyLocation: RecvPacket
    {
        //for party system
        public AckPartyLocation(PacketReader reader)
            : base(240, "Custom Ack Party Location")
        {
            int serial;
            while ((serial = reader.ReadInt32()) > 0)
            {
                Mobile mobile = WorldModel.Entities.GetObject<Mobile>(serial, false);
                int x = reader.ReadInt16();
                int y = reader.ReadInt16();
                int num4 = reader.ReadByte();
                if (mobile != null)
                {
                    //mobile.m_KUOC_X = x;
                    //mobile.m_KUOC_Y = y;
                    //mobile.m_KUOC_F = num4;
                    //if (((num4 == Engine.m_World) && (mobile.Name != null)) && (mobile.Name.Length > 0))
                    //{
                    //    GRadar.AddTag(x, y, mobile.Name, mobile.Serial);
                    //}
                }
            }
        }
    }
}
