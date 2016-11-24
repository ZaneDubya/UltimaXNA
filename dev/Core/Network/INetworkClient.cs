using System;
using System.Net;
using UltimaXNA.Ultima.Login;

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

        bool Connect(string ipAddressOrHostName, int port);
        void Disconnect();
        bool Send(ISendPacket packet);
        bool Send(byte[] buffer, int offset, int length, string name);
        void Slice();

        void Register<T>(object client, int id, int length, Action<T> onReceive) where T : IRecvPacket;
        void RegisterExtended<T>(object client, int extendedId, int subId, int length, Action<T> onReceive) where T : IRecvPacket;
        void Unregister(object client);
        void Unregister(object client, int id);
    }
}