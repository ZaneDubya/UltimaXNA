/***************************************************************************
 *   Client.cs
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
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Network.Compression;
#endregion

namespace UltimaXNA.Core.Network
{
    public sealed class NetworkClient : INetworkClient
    {
        #region Local Variables
        private readonly HuffmanDecompression m_Decompression;
        private readonly List<PacketHandler>[] m_TypedHandlers;
        private readonly List<PacketHandler>[][] m_ExtendedTypedHandlers;
        private readonly List<QueuedPacket> m_QueuedPackets = new List<QueuedPacket>();

        private Socket m_ServerSocket;
        private IPAddress m_ServerAddress;
        private IPEndPoint m_ServerEndPoint;

        private bool m_IsDecompressionEnabled;
        private bool m_IsConnected;
        private bool m_AppendNextMessage;

        private byte[] m_ReceiveBuffer;
        private byte[] m_AppendData;

        private int m_ReceiveBufferPosition;
        #endregion

        public int ClientAddress
        {
            get
            {
                IPHostEntry localEntry = Dns.GetHostEntry(Dns.GetHostName());
                int address;

                if (localEntry.AddressList.Length > 0)
                {
#pragma warning disable 618
                    address = (int)localEntry.AddressList[0].Address;
#pragma warning restore 618
                }
                else
                {
                    address = 0x100007f;
                }

                return ((((address & 0xff) << 0x18) | ((address & 65280) << 8)) | ((address >> 8) & 65280)) | ((address >> 0x18) & 0xff);
            }
        }

        public IPAddress ServerAddress
        {
            get { return m_ServerAddress; }
        }

        public bool IsDecompressionEnabled
        {
            get { return m_IsDecompressionEnabled; }
            set { m_IsDecompressionEnabled = value; }
        }

        public bool IsConnected
        {
            get { return m_IsConnected; }
        }
        
        public NetworkClient()
        {
            m_Decompression = new HuffmanDecompression();
            m_IsDecompressionEnabled = false;

            m_TypedHandlers = new List<PacketHandler>[0x100];
            m_ExtendedTypedHandlers = new List<PacketHandler>[0x100][];

            for (int i = 0; i < m_TypedHandlers.Length; i++)
            {
                m_TypedHandlers[i] = new List<PacketHandler>();
            }
        }

        public void Register<T>(int id, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket
        {
            Type type = typeof(T);
            ConstructorInfo[] ctors = type.GetConstructors();

            bool valid = false;

            for (int i = 0; i < ctors.Length && !valid; i++)
            {
                ParameterInfo[] parameters = ctors[i].GetParameters();
                valid = (parameters.Length == 1 && parameters[0].ParameterType == typeof(PacketReader));
            }

            if (!valid)
            {
                throw new NetworkException(string.Format("Unable to register packet type {0} without a public constructor with a {1} parameter", type, typeof(PacketReader)));
            }

            if (id > byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet id {0:X2} because it is greater than byte.MaxValue", id));
            }

            TypedPacketHandler handler = new TypedPacketHandler(id, name, type, length, onReceive);
            m_TypedHandlers[id].Add(handler);
        }

        public void Unregister(int id, TypedPacketReceiveHandler onRecieve)
        {
            for (int i = 0; i < m_TypedHandlers[id].Count; i++)
            {
                TypedPacketHandler handler = m_TypedHandlers[id][i] as TypedPacketHandler;
                if (handler != null && handler.TypeHandler != null)
                {
                    if (handler.TypeHandler.Method.Equals(onRecieve.Method))
                    {
                        m_TypedHandlers[id].RemoveAt(i);
                        break;
                    }
                }
                else
                {
                    Tracer.Critical("Unable to unregister this onReceive method.");
                }
            }
        }

        public void RegisterExtended<T>(int extendedId, int subId, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket
        {
            Type type = typeof(T);
            ConstructorInfo[] ctors = type.GetConstructors();

            bool valid = false;

            for (int i = 0; i < ctors.Length && !valid; i++)
            {
                ParameterInfo[] parameters = ctors[i].GetParameters();
                valid = (parameters.Length == 1 && parameters[0].ParameterType == typeof(PacketReader));
            }

            if (!valid)
            {
                throw new NetworkException(string.Format("Unable to register packet type {0} without a public constructor with a {1} parameter", type, typeof(PacketReader)));
            }

            if (extendedId > byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet extendedId {0:X2} because it is greater than byte.MaxValue", extendedId));
            }

            if (subId > byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet subId {0:X2} because it is greater than byte.MaxValue", subId));
            }

            if (m_ExtendedTypedHandlers[extendedId] == null)
            {
                m_ExtendedTypedHandlers[extendedId] = new List<PacketHandler>[0x100];

                for (int i = 0; i < m_ExtendedTypedHandlers[extendedId].Length; i++)
                {
                    m_ExtendedTypedHandlers[extendedId][i] = new List<PacketHandler>();
                }
            }

            Tracer.Debug("Registering Extended Command: id: 0x{0:X2} subCommand: 0x{1:X2} Name: {2} Length: {3}", extendedId, subId, name, length);

            TypedPacketHandler handler = new TypedPacketHandler(subId, name, type, length, onReceive);
            m_ExtendedTypedHandlers[extendedId][subId].Add(handler);
        }

        public bool Connect(string ipAddressOrHostName, int port)
        {
            if (IsConnected)
            {
                Disconnect();
            }

            bool success = true;

            try
            {
                if (!IPAddress.TryParse(ipAddressOrHostName, out m_ServerAddress))
                {
                    IPAddress[] ipAddresses = Dns.GetHostAddresses(ipAddressOrHostName);

                    if (ipAddresses.Length == 0)
                    {
                        throw new NetworkException("Host address was unreachable or invalid, unable to obtain an ip address.");
                    }

                    // On Vista and later, the first ip address is an empty one '::1'.
                    // This makes sure we choose the first valid ip address.
                    foreach (IPAddress address in ipAddresses)
                    {
                        if (address.ToString().Length <= 7)
                        {
                            continue;
                        }

                        m_ServerAddress = address;
                        break;
                    }
                }

                m_ServerEndPoint = new IPEndPoint(m_ServerAddress, port);

                Tracer.Debug("Connecting...");

                m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_ServerSocket.Connect(m_ServerEndPoint);

                if (m_ServerSocket.Connected)
                {
                    Tracer.Debug("Connected.");

                    SocketState state = new SocketState(m_ServerSocket, ushort.MaxValue);
                    m_ServerSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }

            }
            catch 
            {
                success = false;
            }

            m_IsConnected = success;
            return success;
        }

        public void Disconnect()
        {
            if (m_ServerSocket != null)
            {
                try
                {
                    m_ServerSocket.Shutdown(SocketShutdown.Both);
                    m_ServerSocket.Close();
                }
                catch
                {

                }

                m_ServerSocket = null;
                m_ServerEndPoint = null;
                m_IsDecompressionEnabled = false;
                Tracer.Debug("Disconnected.");
                m_IsConnected = false;
            }
        }

        public bool Send(ISendPacket packet)
        {
            byte[] buffer = packet.Compile();

            if (IsConnected)
            {
                bool success = Send(buffer, 0, packet.Length, packet.Name);
                if (!success)
                {
                    Disconnect();
                }
                return success;
            }

            return false;
        }

        public bool Send(byte[] buffer, int offset, int length, string name)
        {
            bool success = true;

            if (buffer == null || buffer.Length == 0)
            {
                throw new NetworkException("Unable to send, buffer was null or empty");
            }

            LogPacket(buffer, name, length, false);

            try
            {
                lock (m_ServerSocket)
                {
                    m_ServerSocket.Send(buffer, offset, length, SocketFlags.None);
                }
            }
            catch (Exception e)
            {
                Tracer.Debug(e.ToString());
                success = false;
            }

            return success;
        }

        private void OnReceive(IAsyncResult result)
        {
            SocketState state = result.AsyncState as SocketState;

            if (state == null)
            {
                Tracer.Warn("Socket state was null.");
                return;
            }

            try
            {
                Socket socket = state.Socket;
                if (socket.Connected == false)
                {
                    Disconnect();
                    return;
                }
                int length = socket.EndReceive(result);

                if (length > 0)
                {
                    byte[] buffer = state.Buffer;

                    if (m_ReceiveBuffer == null)
                    {
                        m_ReceiveBuffer = new byte[0x10000];
                    }

                    if (m_IsDecompressionEnabled)
                    {
                        int outsize = 0;
                        byte[] data;

                        if (m_AppendNextMessage)
                        {
                            m_AppendNextMessage = false;
                            data = new byte[m_AppendData.Length + length];

                            Buffer.BlockCopy(m_AppendData, 0, data, 0, m_AppendData.Length);
                            Buffer.BlockCopy(buffer, 0, data, m_AppendData.Length, length);
                        }
                        else
                        {
                            data = new byte[length];
                            Buffer.BlockCopy(buffer, 0, data, 0, length);
                        }

                        while (m_Decompression.DecompressOnePacket(ref data, data.Length, ref m_ReceiveBuffer, ref outsize))
                        {
                            int realLength;
                            List<PacketHandler> packetHandlers = GetHandlers(m_ReceiveBuffer[0], m_ReceiveBuffer[1]);

                            if (!GetPacketSize(packetHandlers, out realLength))
                            {
                                Tracer.Warn("Unhandled packet with id: 0x{0:x2}, possible subid: 0x{1:x2}", m_ReceiveBuffer[0], m_ReceiveBuffer[1]);
                                m_ReceiveBufferPosition = 0;
                                break;
                            }
                            
                            if (realLength != outsize)
                            {
                                throw new Exception("Bad packet size!");
                            }

                            byte[] packetBuffer = new byte[realLength];
                            Buffer.BlockCopy(m_ReceiveBuffer, 0, packetBuffer, 0, realLength);

                            string name = "Unknown";

                            if (packetHandlers.Count > 0)
                                name = packetHandlers[0].Name;

                            AddPacket(name, packetHandlers, packetBuffer, realLength);
                        }

                        // We've run out of data to parse, or the packet was incomplete. If the packet was incomplete,
                        // we should save what's left for socket receive event.
                        if (data.Length > 0)
                        {
                            m_AppendNextMessage = true;
                            m_AppendData = data;
                        }
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, 0, m_ReceiveBuffer, m_ReceiveBufferPosition, length);

                        m_ReceiveBufferPosition += length;

                        int currentIndex = 0;

                        while (currentIndex < m_ReceiveBufferPosition)
                        {
                            int realLength;

                            List<PacketHandler> packetHandlers = GetHandlers(m_ReceiveBuffer[currentIndex], m_ReceiveBuffer[currentIndex + 1]);
                            
                            if (!GetPacketSize(packetHandlers, out realLength))
                            {
                                currentIndex = 0;
                                m_ReceiveBufferPosition = 0;
                                break;
                            }

                            if ((m_ReceiveBufferPosition - currentIndex) >= realLength)
                            {
                                byte[] packetBuffer = new byte[realLength];
                                Buffer.BlockCopy(m_ReceiveBuffer, currentIndex, packetBuffer, 0, realLength);

                                string name = "Unknown";

                                if (packetHandlers.Count > 0)
                                    name = packetHandlers[0].Name;

                                AddPacket(name, packetHandlers, packetBuffer, realLength);

                                currentIndex += realLength;
                            }
                            else
                            {
                                //Need more data
                                break;
                            }
                        }

                        m_ReceiveBufferPosition -= currentIndex;

                        if (m_ReceiveBufferPosition > 0)
                        {
                            Buffer.BlockCopy(m_ReceiveBuffer, currentIndex, m_ReceiveBuffer, 0, m_ReceiveBufferPosition);
                        }
                    }
                }

                if (m_ServerSocket != null)
                {
                    m_ServerSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }
            }
            catch (Exception e)
            {
                Tracer.Debug(e.ToString());
                Disconnect();
            }
        }

        private void AddPacket(string name, List<PacketHandler> packetHandlers, byte[] packetBuffer, int realLength)
        {
            lock (m_QueuedPackets)
            {
                m_QueuedPackets.Add(new QueuedPacket(name, packetHandlers, packetBuffer, realLength));
            }
        }

        public void Slice()
        {
            lock (m_QueuedPackets)
            {
                foreach (QueuedPacket i in m_QueuedPackets)
                {
                    LogPacket(i.PacketBuffer, i.Name, i.RealLength);
                    InvokeHandlers(i.PacketHandlers, i.PacketBuffer, i.RealLength);
                }
                m_QueuedPackets.Clear();
            }
        }

        private bool GetPacketSize(List<PacketHandler> packetHandlers, out int realLength)
        {
            realLength = 0;

            if (packetHandlers.Count > 0)
            {
                if (packetHandlers[0].Length == -1)
                {
                    realLength = m_ReceiveBuffer[2] | (m_ReceiveBuffer[1] << 8);
                }
                else
                {
                    realLength = packetHandlers[0].Length;
                }

                return true;
            }
              
            return false; 
        }
        
        private void LogPacket(byte[] buffer, string name, int length, bool servertoclient = true)
        {
            if (Settings.Debug.LogPackets)
            {
                Tracer.Debug(servertoclient ? "Server - > Client" : "Client - > Server");
                Tracer.Debug("Id: 0x{0:X2} Name: {1} Length: {2}", buffer[0], name, length);
                Tracer.Debug("{1}{0}", Utility.FormatBuffer(buffer, length), Environment.NewLine);
            }
        }

        private void InvokeHandlers(List<PacketHandler> packetHandlers, byte[] buffer, int length)
        {
            if (packetHandlers == null)
            {
                return;
            }

            int count = packetHandlers.Count;

            for (int i = 0; i < count; i++)
            {
                PacketHandler handler = packetHandlers[i];
                TypedPacketHandler typedHandler = packetHandlers[i] as TypedPacketHandler;

                if (typedHandler != null)
                {
                    PacketReader reader = PacketReader.CreateInstance(buffer, length, typedHandler.Length != -1);

                    IRecvPacket recvPacket = typedHandler.CreatePacket(reader);

                    if (typedHandler.TypeHandler != null)
                    {
                        typedHandler.TypeHandler(recvPacket);
                    }
                    else
                    {
                        Tracer.Warn("ClientNetwork: Unknown packet received!");
                    }
                }
                else
                {
                    PacketReader reader = PacketReader.CreateInstance(buffer, length, packetHandlers[i].Length != -1);

                    if (handler.Handler != null)
                    {
                        handler.Handler(reader);
                    }
                }
            }
        }

        private List<PacketHandler> GetHandlers(byte cmd, byte subcommand)
        {
            List<PacketHandler> packetHandlers = new List<PacketHandler>();

            packetHandlers.AddRange(m_TypedHandlers[cmd]);

            if (m_ExtendedTypedHandlers[cmd] != null)
            {
                packetHandlers.AddRange(m_ExtendedTypedHandlers[cmd][subcommand]);
            }

            return packetHandlers;
        }
    }
}
