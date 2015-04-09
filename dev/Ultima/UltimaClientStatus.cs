using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Ultima
{
    public enum UltimaClientStatus
    {
        Unconnected,
        LoginServer_Connecting,
        LoginServer_WaitingForLogin,
        LoginServer_LoggingIn,
        LoginServer_WaitingForRelay,
        LoginServer_Relaying,
        LoginServer_HasServerList,
        GameServer_Connecting,
        GameServer_CharList,
        GameServer_LoggingIn,
        WorldServer_LoginComplete,
        WorldServer_InWorld,
        Error_Undefined,
        Error_CannotConnectToServer,
        Error_InvalidUsernamePassword,
        Error_InUse,
        Error_Blocked,
        Error_BadPassword,
        Error_Idle,
        Error_BadCommunication
    }
}
