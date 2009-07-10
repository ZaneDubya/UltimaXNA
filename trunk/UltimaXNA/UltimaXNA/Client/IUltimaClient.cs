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
    }

    interface IUltimaClient
    {
        UltimaClientStatus Status { get; }
        
        bool Connect(string ipAddressOrHostName, int port);
        void Disconnect();
        void Send(UltimaXNA.Network.ISendPacket packet);
        void SetAccountPassword(string nAccount, string nPassword);
    }
}
