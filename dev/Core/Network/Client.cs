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
using UltimaXNA.Core.Network.Compression;
using UltimaXNA.Diagnostics;
#endregion

namespace UltimaXNA.Core.Network
{
    public class Client
    {
        HuffmanDecompression m_decompression;

        Socket m_serverSocket;
        IPAddress m_serverAddress;
        IPEndPoint m_serverEndPoint;

        int[] packetLengths = new int[byte.MaxValue];

        List<PacketHandler>[] m_handlers;
        List<PacketHandler>[] m_typedHandlers;

        List<PacketHandler>[][] m_extendedHandlers;
        List<PacketHandler>[][] m_extendedTypedHandlers;

        bool m_isDecompressionEnabled;
        bool m_isConnected;
        bool m_loggingPackets;
        bool m_appendNextMessage = false;

        byte[] m_receiveBuffer;
        byte[] m_appendData;

        int m_receiveBufferPosition;

        public int ClientAddress
        {
            get
            {
                IPHostEntry localEntry = Dns.GetHostEntry(Dns.GetHostName());
                int address = -1;

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
            get { return m_serverAddress; }
        }

        public bool IsDecompressionEnabled
        {
            get { return m_isDecompressionEnabled; }
            set { m_isDecompressionEnabled = value; }
        }

        public bool IsConnected
        {
            get { return m_isConnected; }
        }

        public bool LogPackets
        {
            get { return m_loggingPackets; }
            set { m_loggingPackets = value; }
        }

        public Client()
        {
            this.m_decompression = new HuffmanDecompression();
            this.m_isDecompressionEnabled = false;

            this.m_handlers = new List<PacketHandler>[0x100];
            this.m_typedHandlers = new List<PacketHandler>[0x100];

            this.m_extendedHandlers = new List<PacketHandler>[0x100][];
            this.m_extendedTypedHandlers = new List<PacketHandler>[0x100][];

            for (int i = 0; i < m_handlers.Length; i++)
            {
                m_handlers[i] = new List<PacketHandler>();
            }

            for (int i = 0; i < m_typedHandlers.Length; i++)
            {
                m_typedHandlers[i] = new List<PacketHandler>();
            }
        }

        public virtual void Register(int id, string name, int length, PacketReceiveHandler onReceive)
        {
            if (id >= byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet id {0:X2} because it is greater than byte.MaxValue", id));
            }

            packetLengths[id] = length;
            PacketHandler handler = new PacketHandler(id, name, length, onReceive);
            m_handlers[id].Add(handler);
        }

        public virtual void Register<T>(int id, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket
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
            m_typedHandlers[id].Add(handler);
        }

        public virtual void Unregister(int id, Action<IRecvPacket> onRecieve)
        {
            for (int i = 0; i < m_typedHandlers[id].Count; i++)
            {
                TypedPacketHandler handler = m_typedHandlers[id][i] as TypedPacketHandler;
                if (handler != null && handler.TypeHandler != null)
                {
                    if (handler.TypeHandler.Method.Equals(onRecieve.Method))
                    {
                        m_typedHandlers[id].RemoveAt(i);
                        break;
                    }
                }
                else
                {
                    Diagnostics.Logger.Fatal("Unable to unregister this onReceive method.");
                }
            }
        }

        public virtual void RegisterExtended(int extendedId, int subId, string name, int length, PacketReceiveHandler onReceive)
        {
            if (subId > byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet id {0:X2} because it is greater than byte.MaxValue", subId));
            }

            if (m_extendedHandlers[extendedId] == null)
            {
                m_extendedHandlers[extendedId] = new List<PacketHandler>[0x100];

                for (int i = 0; i < m_extendedHandlers[extendedId].Length; i++)
                {
                    m_extendedHandlers[extendedId][i] = new List<PacketHandler>();
                }
            }

            Logger.Debug("Registering Extended Command: id: 0x{0:X2} subCommand: 0x{1:X2} Name: {2} Length: {3}", extendedId, subId, name, length);

            PacketHandler handler = new PacketHandler(subId, name, length, onReceive);
            m_extendedHandlers[extendedId][subId].Add(handler);
        }

        public virtual void RegisterExtended<T>(int extendedId, int subId, string name, int length, TypedPacketReceiveHandler onReceive) where T : IRecvPacket
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

            if (m_extendedTypedHandlers[extendedId] == null)
            {
                m_extendedTypedHandlers[extendedId] = new List<PacketHandler>[0x100];

                for (int i = 0; i < m_extendedTypedHandlers[extendedId].Length; i++)
                {
                    m_extendedTypedHandlers[extendedId][i] = new List<PacketHandler>();
                }
            }

            Logger.Debug("Registering Extended Command: id: 0x{0:X2} subCommand: 0x{1:X2} Name: {2} Length: {3}", extendedId, subId, name, length);

            TypedPacketHandler handler = new TypedPacketHandler(subId, name, type, length, onReceive);
            m_extendedTypedHandlers[extendedId][subId].Add(handler);
        }

        public virtual bool Connect(string ipAddressOrHostName, int port)
        {
            if (IsConnected)
            {
                Disconnect();
            }

            bool success = true;

            try
            {
                if (!IPAddress.TryParse(ipAddressOrHostName, out m_serverAddress))
                {
                    IPAddress[] ipAddresses = Dns.GetHostAddresses(ipAddressOrHostName);

                    if (ipAddresses.Length == 0)
                    {
                        throw new NetworkException("Host address was unreachable or invalid, unable to obtain an ip address.");
                    }
                    else
                    {
                        // On Windows7 & Vista, the first ip address is an empty one '::1'.
                        // This makes sure we choose the first valid ip address.
                        for (int i = 0; i < ipAddresses.Length; i++)
                        {
                            if (ipAddresses[i].ToString().Length > 7)
                            {
                                m_serverAddress = ipAddresses[i];
                                break;
                            }
                        }
                    }
                }

                m_serverEndPoint = new IPEndPoint(m_serverAddress, port);

                Logger.Debug("Connecting...");

                m_serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_serverSocket.Connect(m_serverEndPoint);

                if (m_serverSocket.Connected)
                {
                    Logger.Debug("Connected.");

                    SocketState state = new SocketState(m_serverSocket, ushort.MaxValue);
                    m_serverSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }

            }
            catch 
            {
                success = false;
            }

            m_isConnected = success;
            return success;
        }

        public virtual void Disconnect()
        {
            if (m_serverSocket != null)
            {
                try
                {
                    m_serverSocket.Shutdown(SocketShutdown.Both);
                    m_serverSocket.Close();
                }
                catch
                {

                }
                m_serverSocket = null;
                m_serverEndPoint = null;
                m_isDecompressionEnabled = false;
                Logger.Debug("Disconnected.");
                m_isConnected = false;
            }
        }

        public virtual bool Send(ISendPacket packet)
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

        public virtual bool Send(byte[] buffer, int offset, int length, string name)
        {
            bool success = true;

            if (buffer == null || buffer.Length == 0)
            {
                throw new NetworkException("Unable to send, buffer was null or empty");
            }

            if (m_loggingPackets)
            {
                Logger.Debug("Client - > Server");
                Logger.Debug("Id: 0x{0:X2} Name: {1} Length: {2}", buffer[0], name, length);
                Logger.Debug("{1}{0}", Utility.FormatBuffer(buffer, length), Environment.NewLine);
            }

            try
            {
                lock (m_serverSocket)
                {
                    m_serverSocket.Send(buffer, offset, length, SocketFlags.None);
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
                success = false;
            }

            return success;
        }

        protected virtual void OnReceive(IAsyncResult result)
        {
            SocketState state = result.AsyncState as SocketState;

            if (state == null)
            {
                Logger.Warn("Socket state was null.");
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
                    byte[] buffer = null;

                    buffer = state.Buffer;

                    if (m_receiveBuffer == null)
                    {
                        m_receiveBuffer = new byte[0x10000];
                    }

                    // TODO: Clean this up, this is a bit unoptimized.
                    if (m_isDecompressionEnabled)
                    {
                        int outsize = 0;
                        byte[] data;

                        if (m_appendNextMessage)
                        {
                            m_appendNextMessage = false;
                            data = new byte[m_appendData.Length + length];

                            Buffer.BlockCopy(m_appendData, 0, data, 0, m_appendData.Length);
                            Buffer.BlockCopy(buffer, 0, data, m_appendData.Length, length);
                        }
                        else
                        {
                            data = new byte[length];
                            Buffer.BlockCopy(buffer, 0, data, 0, length);
                        }

                        while (m_decompression.DecompressOnePacket(ref data, data.Length, ref m_receiveBuffer, ref outsize))
                        {
                            int realLength;
                            List<PacketHandler> packetHandlers = GetHandlers(m_receiveBuffer[0], m_receiveBuffer[1]);

                            if (!GetPacketSize(packetHandlers, out realLength))
                            {
                                m_receiveBufferPosition = 0;
                                break;
                            }
                            
                            if (realLength != outsize)
                            {
                                throw new Exception("Bad packet size!");
                            }

                            byte[] packetBuffer = new byte[realLength];
                            Buffer.BlockCopy(m_receiveBuffer, 0, packetBuffer, 0, realLength);

                            string name = "Unknown";

                            if (packetHandlers.Count > 0)
                                name = packetHandlers[0].Name;

                            m_AddPacket(name, packetHandlers, packetBuffer, realLength);
                            // LogPacket(packetBuffer, name, realLength);
                            // InvokeHandlers(packetHandlers, packetBuffer, realLength);
                        }

                        // We've run out of data to parse, or the packet was incomplete. If the packet was incomplete,
                        // we should save what's left for next time.
                        if (data.Length > 0)
                        {
                            m_appendNextMessage = true;
                            m_appendData = data;
                        }
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, 0, m_receiveBuffer, m_receiveBufferPosition, length);

                        m_receiveBufferPosition += length;

                        int currentIndex = 0;

                        while (currentIndex < m_receiveBufferPosition)
                        {
                            int realLength = length;

                            List<PacketHandler> packetHandlers = GetHandlers(m_receiveBuffer[currentIndex], m_receiveBuffer[currentIndex + 1]);

                            if (!GetPacketSize(packetHandlers, out realLength))
                            {
                                currentIndex = 0;
                                m_receiveBufferPosition = 0;
                                break;
                            }

                            if ((m_receiveBufferPosition - currentIndex) >= realLength)
                            {
                                byte[] packetBuffer = new byte[realLength];
                                Buffer.BlockCopy(m_receiveBuffer, currentIndex, packetBuffer, 0, realLength);

                                string name = "Unknown";

                                if (packetHandlers.Count > 0)
                                    name = packetHandlers[0].Name;

                                m_AddPacket(name, packetHandlers, packetBuffer, realLength);
                                // LogPacket(packetBuffer, name, realLength);
                                // InvokeHandlers(packetHandlers, packetBuffer, realLength);

                                currentIndex += realLength;
                            }
                            else
                            {
                                //Need more data
                                break;
                            }
                        }

                        m_receiveBufferPosition -= currentIndex;

                        if (m_receiveBufferPosition > 0)
                        {
                            Buffer.BlockCopy(m_receiveBuffer, currentIndex, m_receiveBuffer, 0, m_receiveBufferPosition);
                        }
                    }
                }

                if (m_serverSocket != null)
                {
                    m_serverSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }
            }
            catch (Exception e)
            {
                Logger.Debug(e);
                Disconnect();
            }
        }

        List<QueuedPacket> m_QueuedPackets = new List<QueuedPacket>();
        private void m_AddPacket(string name, List<PacketHandler> packetHandlers, byte[] packetBuffer, int realLength)
        {
            lock (m_QueuedPackets)
            {
                m_QueuedPackets.Add(new QueuedPacket(name, packetHandlers, packetBuffer, realLength));
            }
        }

        public void Update()
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
                    realLength = m_receiveBuffer[2] | (m_receiveBuffer[1] << 8);
                }
                else
                {
                    realLength = packetHandlers[0].Length;
                }

                return true;
            }
              
            return false; 
        }
        
        private void LogPacket(byte[] buffer, string name, int length)
        {
            if (m_loggingPackets)
            {
                Logger.Debug("Server - > Client");
                Logger.Debug("Id: 0x{0:X2} Name: {1} Length: {2}", buffer[0], name, length);
                Logger.Debug("{1}{0}", Utility.FormatBuffer(buffer, length), Environment.NewLine);
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
                TypedPacketHandler handler = packetHandlers[i] as TypedPacketHandler;

                if (handler != null)
                {
                    PacketReader reader = PacketReader.CreateInstance(buffer, length, handler.Length != -1);

                    IRecvPacket recvPacket = handler.CreatePacket(reader);

                    if (handler.TypeHandler != null)
                    {
                        handler.TypeHandler(recvPacket);
                    }
                    else
                    {
                        Diagnostics.Logger.Warn("ClientNetwork: Unknown packet received!");
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

            packetHandlers.AddRange(m_handlers[cmd]);
            packetHandlers.AddRange(m_typedHandlers[cmd]);

            if (m_extendedHandlers[cmd] != null)
            {
                packetHandlers.AddRange(m_extendedHandlers[cmd][subcommand]);
            }

            if (m_extendedTypedHandlers[cmd] != null)
            {
                packetHandlers.AddRange(m_extendedTypedHandlers[cmd][subcommand]);
            }

            return packetHandlers;
        }
    }
}
