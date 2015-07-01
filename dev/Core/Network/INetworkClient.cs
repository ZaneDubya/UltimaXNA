using System.Net;

namespace UltimaXNA.Core.Network
{
    public interface INetworkClient
    {
        int ClientAddress
        {
            get;
        }

        IPAddress ServerAddress
        {
            get;
        }

        bool IsConnected
        {
            get;
        }

        bool IsDecompressionEnabled
        {
            get;
            set;
        }

        void Register<T>(int id, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket;
        void RegisterExtended<T>(int extendedId, int subId, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket;
        void Unregister(int id, TypedPacketReceiveHandler onRecieve);
        bool Connect(string ipAddressOrHostName, int port);
        void Disconnect();
        bool Send(ISendPacket packet);
        bool Send(byte[] buffer, int offset, int length, string name);
        void Slice();
    }
}