/***************************************************************************
 *   SocketState.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Net.Sockets;
#endregion

namespace UltimaXNA.Core.Network
{
    public class SocketState
    {
        private Socket m_Socket;
        private byte[] m_Buffer;
        private int m_DataLength;

        public Socket Socket
        {
            get { return m_Socket; }
        }

        public byte[] Buffer
        {
            get { return m_Buffer; }
            set { m_Buffer = value; }
        }

        public int DataLength
        {
            get { return m_DataLength; }
            set { m_DataLength = value; }
        }

        public SocketState(Socket socket, byte[] buffer)
        {
            m_Socket = socket;
            m_Buffer = buffer;
            m_DataLength = 0;
        }
    }
}
