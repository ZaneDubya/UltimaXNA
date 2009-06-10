#region File Description & Usings
//-----------------------------------------------------------------------------
// SocketClient.cs
//
// By Greeeat, greeeat (at) gmail (dot) com
//-----------------------------------------------------------------------------
using System;
using System.Net.Sockets;
using System.Net;
#endregion

namespace UltimaXNA.Network
{
    class SocketListener
    {
        private TcpListener listener;
        // When we have received a User we send an event.
        public event UserEventDlg userAdded;
        // A variable to keep track of how many users we have added
        private bool[] idsUsed = new bool[256];

        public SocketListener(int portNr)
        {
            this.listener = new TcpListener(IPAddress.Any, portNr);
        }

        ~SocketListener()
        {
            this.Stop();
        }

        public void Start()
        {
            this.listener.Start();
            this.ListenForNewClient();
        }

        public void Stop()
        {
            this.listener.Stop();
        }

        private void ListenForNewClient()
        {
            // Just as for the Read/BeginRead-methods in the TcpClient the BeginAcceptTcpClient
            // is the AcceptTcpClient() asyncrounous brother.
            this.listener.BeginAcceptTcpClient(this.AcceptClient, null);
        }

        private void AcceptClient(IAsyncResult ar)
        {
            // The EndAcceptTcpClient works similar to the EndRead on the TcpClient but instead
            // it returns the client that has connected. Remember that this method is executed
            // on a different thread than the mainprogram.
            TcpClient newClient = this.listener.EndAcceptTcpClient(ar);

            // Find an empty id
            byte idForNewPlayer = 0;
            for (byte i = 0; i < idsUsed.Length; i++)
            {
                if (this.idsUsed[i] == false)
                {
                    idForNewPlayer = i;
                    break;
                }
            }
            this.idsUsed[idForNewPlayer] = true;

            // Create a new SocketClient from the newly connected TcpClient and fire an event to tell
            // everyone else that we have got new client.
            SocketClient newSocketClient = new SocketClient(newClient, idForNewPlayer);
            newSocketClient.UserDisconnected += 
                            new UserEventDlg(newSocketClient_UserDisconnected);

            if (this.userAdded != null)
                this.userAdded(this, newSocketClient);

            this.ListenForNewClient();
        }

        void newSocketClient_UserDisconnected(object sender, SocketClient player)
        {
            // Reset the id if someone disconnected. We do NOT send a UserDisconnected-event here,
            // the program should register that event itself when it receives a new SocketClient in the
            // userAdded-event.
            this.idsUsed[player.id] = false;
        }
    }
} 
