using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace UltimaXNA.Network
{
    public class SocketState
    {
        private Socket socket;
        private byte[] buffer;
        private int dataLength;

        public Socket Socket
        {
            get { return socket; }
        }

        public byte[] Buffer
        {
            get { return buffer; }
            set { buffer = value; }
        }

        public int DataLength
        {
            get { return dataLength; }
            set { dataLength = value; }
        }

        public SocketState(Socket socket, int bufferSize)
        {
            this.socket = socket;
            this.buffer = new byte[bufferSize];
            this.dataLength = 0;
        }
    }
}
