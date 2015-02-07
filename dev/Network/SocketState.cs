/***************************************************************************
 *   SocketState.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
#endregion

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
