using System;
using System.Net;
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
