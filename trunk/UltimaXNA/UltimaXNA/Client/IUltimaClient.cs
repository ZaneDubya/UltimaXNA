namespace UltimaXNA.Client
{
    public enum UltimaClientStatus
    {
        Unconnected,
        LoginServer_Connecting,
        LoginServer_HasServerList,
        LoginServer_ServerSelected,
        GameServer_Connecting,
        GameServer_ConnectedAndCharList,
        GameServer_LoggingIn,
        WorldServer_LoginComplete,
        WorldServer_InWorld,
        Error_Undefined,
        Disconnected
    }

    interface IUltimaClient
    {
        UltimaClientStatus Status { get; }
        void SetAccountPassword(string nAccount, string nPassword);
        bool Connect(string ipAddressOrHostName, int port);
        void Disconnect();
        void Send(UltimaXNA.Network.ISendPacket packet);
    }
}
