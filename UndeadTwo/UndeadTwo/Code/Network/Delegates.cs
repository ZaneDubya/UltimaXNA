#region File Description & Usings
//-----------------------------------------------------------------------------
// Delegates.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
#endregion

namespace UndeadClient.Network
{
    public delegate void UserEventDlg(object sender,
                                      SocketClient player);

    public delegate void DataReceivedDlg(SocketClient nSender,
                                        Packet nPacket);
}
