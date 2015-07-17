/***************************************************************************
 *   ClientVersionPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    /// <summary>
    /// The legacy client sends this packet every four seconds. Not certain what use it has, but
    /// it serves to keep the connection with the server alive.
    /// </summary>
    public class UOSEKeepAlivePacket : SendPacket
    {
        public UOSEKeepAlivePacket()
            : base(0xBF, "UOSE Keep Alive")
        {
            byte value = (byte)Utility.RandomValue(0x20, 0x80);
            Stream.Write((ushort)0x0024);
            Stream.Write((byte)value);
        }
    }
}
