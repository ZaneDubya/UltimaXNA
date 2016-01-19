/***************************************************************************
 *   NetworkClient.cs
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
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.IO;
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
        private readonly BufferPool m_BufferPool = new BufferPool("Network Client - Buffer Pool", 8, 0x10000);
        private readonly object m_SyncRoot = new object();
        private readonly PacketChunk m_IncompleteDecompressionPacket;
        private readonly PacketChunk m_IncompletePacket;

        private Queue<QueuedPacket> m_Queue = new Queue<QueuedPacket>();
        private Queue<QueuedPacket> m_WorkingQueue = new Queue<QueuedPacket>();

        private Socket m_ServerSocket;
        private IPAddress m_ServerAddress;
        private IPEndPoint m_ServerEndPoint;

        private bool m_IsDecompressionEnabled;
        private bool m_IsConnected;
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

            m_IncompletePacket = new PacketChunk(m_BufferPool.AcquireBuffer());
            m_IncompleteDecompressionPacket = new PacketChunk(m_BufferPool.AcquireBuffer());

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

            if (m_TypedHandlers[id].Any())
            {
                int requiredLength = m_TypedHandlers[id][0].Length;

                Guard.Requires(requiredLength == length,
                    "Invalid packet length.  All packet handlers for 0x{0:X2} must specify a length of {1}.", id,
                    requiredLength);
            }

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
            lock (m_SyncRoot)
            {
                m_WorkingQueue.Clear();
            }

            m_IncompletePacket.Clear();
            m_IncompleteDecompressionPacket.Clear();
            
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

                Tracer.Debug("Connecting to {0}:{1}...", m_ServerAddress, port);

                m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_ServerSocket.Connect(m_ServerEndPoint);

                if (m_ServerSocket.Connected)
                {
                    Tracer.Debug("Connected.");

                    SocketState state = new SocketState(m_ServerSocket, m_BufferPool.AcquireBuffer());
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
                Tracer.Warn("NetworkClient: Send - " + e.ToString());
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

                    if (m_IsDecompressionEnabled)
                    {
                        DecompressBuffer(ref buffer, ref length);
                    }

                    if (m_IncompletePacket.Length > 0)
                    {
                        m_IncompletePacket.Prepend(buffer, length);
                        m_IncompletePacket.Clear();
                    }

                    int offset = 0;

                    ProcessBuffer(buffer, ref offset, length);
                    
                    // Not all the data was processed, due to an incomplete packet
                    if(offset < length)
                    {
                        m_IncompletePacket.Write(buffer, offset, length - offset);
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

        private void DecompressBuffer(ref byte[] buffer, ref int length)
        {
            byte[] source = m_BufferPool.AcquireBuffer();

            int incompleteLength = m_IncompleteDecompressionPacket.Length;
            int sourceLength = incompleteLength + length;

            if (incompleteLength > 0)
            {
                m_IncompleteDecompressionPacket.Prepend(source, 0);
                m_IncompleteDecompressionPacket.Clear();
            }

            Buffer.BlockCopy(buffer, 0, source, incompleteLength, length);

            int outSize;
            int processedOffset = 0;
            int sourceOffset = 0;
            int offset = 0;

            while (m_Decompression.DecompressChunk(ref source, ref sourceOffset, sourceLength, ref buffer, offset, out outSize))
            {
                processedOffset = sourceOffset;
                offset += outSize;
            }
                
            length = offset;

            // We've run out of data to parse, or the packet was incomplete. If the packet was incomplete,
            // we should save what's left for the next socket receive event.
            if (processedOffset >= sourceLength)
            {
                m_BufferPool.ReleaseBuffer(source);
                return;
            }

            m_IncompleteDecompressionPacket.Write(source, processedOffset, sourceLength - processedOffset);
        }
        
        private void ProcessBuffer(byte[] buffer, ref int offset, int length)
        {
            int index = offset;

            while (index < length)
            {
                int realLength;
                PacketHandler packetHandler;
                List<PacketHandler> packetHandlers = GetHandlers(buffer[index], buffer[index + 1]);

                // Ensure we have the proper handler for the size of this packet
                if (!GetPacketSizeAndHandler(packetHandlers, buffer, index, out realLength, out packetHandler))
                {
                    byte[] formatBuffer = m_BufferPool.AcquireBuffer();
                    Buffer.BlockCopy(buffer, index, formatBuffer, 0, length - index);
                    Tracer.Warn("Unhandled packet with id: 0x{0:x2}, possible subid: 0x{1:x2}{2}{3}", buffer[index], buffer[index + 1], Environment.NewLine, Utility.FormatBuffer(formatBuffer, length - index));
                    index += length;
                    break;
                }

                // Entire packet exist, so we can process it
                if ((length - index) >= realLength)
                {
                    // TODO: Move this to a buffer pool, need to investigate max byte[].length and pool size
                    byte[] packetBuffer = new byte[realLength];
                    Buffer.BlockCopy(buffer, index, packetBuffer, 0, realLength);

                    string name = packetHandler.Name;
                    AddPacket(name, packetHandler, packetBuffer, realLength);

                    index += realLength;
                }
                else
                {
                    //Need more data
                    break;
                }
            }

            offset = index;
        }

        private void AddPacket(string name, PacketHandler packetHandler, byte[] packetBuffer, int realLength)
        {
            lock (m_SyncRoot)
            {
                m_WorkingQueue.Enqueue(new QueuedPacket(name, packetHandler, packetBuffer, realLength));
            }
        }

        public void Slice()
        {
            lock (m_SyncRoot)
            {
                Queue<QueuedPacket> temp = m_WorkingQueue;
                m_WorkingQueue = m_Queue;
                m_Queue = temp;
            }

            while (m_Queue.Count > 0)
            {
                QueuedPacket packet = m_Queue.Dequeue();

                LogPacket(packet.PacketBuffer, packet.Name, packet.RealLength);
                InvokeHandler(packet.PacketHandler, packet.PacketBuffer, packet.RealLength);
            }
        }

        /// <summary>
        /// Determines the correct packet size of the packet, and if there is a packetHandler that will handle this packet.
        /// </summary>
        /// <param name="packetHandlers">List of possible packet handlers for this packet. A packet handler with length of -1 must be first, if any.</param>
        /// <param name="realLength">The real length of the packet.</param>
        /// <returns>True if there is a packetHandler that will handle this packet.</returns>
        private bool GetPacketSizeAndHandler(List<PacketHandler> packetHandlers, byte[] buffer, int offset, out int realLength, out PacketHandler packetHandler)
        {
            realLength = 0;
            packetHandler = null;

            if (packetHandlers.Count == 0)
                return false;

            foreach (PacketHandler ph in packetHandlers)
            {
                if (ph.Length == -1)
                {
                    realLength = buffer[offset + 2] | (buffer[offset + 1] << 8);
                    packetHandler = ph;
                    return true;
                }
                
                realLength = ph.Length;
                packetHandler = ph;
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

        private void InvokeHandler(PacketHandler packetHandler, byte[] buffer, int length)
        {
            if (packetHandler == null)
            {
                return;
            }

            TypedPacketHandler typedHandler = packetHandler as TypedPacketHandler;

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
                    Tracer.Warn("NetworkClient: Unhandled packet received! [typedHandler != null]");
                }
            }
            else
            {
                PacketReader reader = PacketReader.CreateInstance(buffer, length, packetHandler.Length != -1);

                if (packetHandler.Handler != null)
                {
                    packetHandler.Handler(reader);
                }
                else
                {
                    Tracer.Warn("NetworkClient: Unhandled packet received! [typedHandler == null]");
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

    public class PacketChunk
    {
        private readonly byte[] m_Buffer;
        private int m_Length;

        public PacketChunk(byte[] buffer)
        {
            m_Buffer = buffer;
        }

        public int Length
        {
            get { return m_Length; }
        }

        public void Write(byte[] source, int offset, int length)
        {
            Buffer.BlockCopy(source, offset, m_Buffer, m_Length, length);
            
            m_Length += length;
        }

        public void Prepend(byte[] dest, int length)
        {
            // Offset the intial buffer by the amount we need to prepend
            if (length > 0)
            {
                Buffer.BlockCopy(dest, 0, dest, m_Length, length);
            }

            // Prepend the buffer to the destination buffer
            Buffer.BlockCopy(m_Buffer, 0, dest, 0, m_Length);
        }

        public void Clear()
        {
            m_Length = 0;
        }
    }
}
