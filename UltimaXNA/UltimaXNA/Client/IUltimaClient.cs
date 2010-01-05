/***************************************************************************
 *   IUltimaClient.cs
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
#endregion

namespace UltimaXNA.Client
{
    public enum UltimaClientStatus
    {
        Unconnected,
        LoginServer_Connecting,
        LoginServer_WaitingForLogin,
        LoginServer_LoggingIn,
        LoginServer_HasServerList,
        LoginServer_ServerSelected,
        GameServer_Connecting,
        GameServer_ConnectedAndCharList,
        GameServer_LoggingIn,
        WorldServer_LoginComplete,
        WorldServer_InWorld,
        Error_Undefined,
        Error_CannotConnect
    }

    public interface IUltimaClient
    {
        UltimaClientStatus Status { get; }
        
        bool Connect(string ipAddressOrHostName, int port);
        void Disconnect();
        void Send(UltimaXNA.Network.ISendPacket packet);
        void SetAccountPassword(string nAccount, string nPassword);
        bool IsConnected { get; }
    }
}
