/***************************************************************************
 *   ContainerContentUpdatePacket.cs
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class AddSingleItemToContainerPacket : RecvPacket
    {
        public int ItemId
        {
            get;
            private set;
        }

        public int Amount
        {
            get;
            private set;
        }

        public int X
        {
            get;
            private set;
        }

        public int Y
        {
            get;
            private set;
        }

        public int GridLocation
        {
            get;
            private set;
        }

        public Serial ContainerSerial
        {
            get;
            private set;
        }

        public int Hue
        {
            get;
            private set;
        }

        public Serial Serial
        {
            get;
            private set;
        }

        public AddSingleItemToContainerPacket(PacketReader reader)
            : base(0x25, "Add Single Item")
        {
            Serial = reader.ReadInt32();
            ItemId = reader.ReadUInt16();
            reader.ReadByte(); // unknown 
            Amount = reader.ReadUInt16();
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            if (reader.Buffer.Length == 21)
                GridLocation = reader.ReadByte(); // always 0 in RunUO.
            else
                GridLocation = 0;
            ContainerSerial = (Serial)reader.ReadInt32();
            Hue = reader.ReadUInt16();
        }
    }
}
