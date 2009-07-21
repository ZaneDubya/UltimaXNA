/***************************************************************************
 *   IClientNetwork.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using System;
using System.Net;
#endregion

namespace UltimaXNA.Network
{
    public interface IClientNetwork
    {
        bool IsConnected { get; }
        int ClientAddress { get; }
        IPAddress ServerAddress { get; }

        bool Connect(string address, int port);
        void Disconnect();
        void Register(int id, string name, int length, PacketReceiveHandler onReceive);
        void Register<T>(int id, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket;
        void RegisterExtended(int extendedId, int subId, string name, int length, PacketReceiveHandler onReceive);
        void RegisterExtended<T>(int extendedId, int subId, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket;
        bool Send(byte[] buffer, int offset, int length);
    }
}
