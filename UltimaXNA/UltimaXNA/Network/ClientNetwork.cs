/***************************************************************************
 *   ClientNetwork.cs
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
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using UltimaXNA.Diagnostics;
#endregion

namespace UltimaXNA.Network
{
    public class ClientNetwork : IClientNetwork
    {
        ILoggingService _log;
        IDecompression _decompression;

        Socket _serverSocket;
        IPAddress _serverAddress;
        IPEndPoint _serverEndPoint;

        int[] packetLengths = new int[byte.MaxValue];

        List<PacketHandler>[] _handlers;
        List<PacketHandler>[] _typedHandlers;

        List<PacketHandler>[][] _extendedHandlers;
        List<PacketHandler>[][] _extendedTypedHandlers;

        bool _isDecompressionEnabled;
        bool _isConnected;
        bool _logPackets;
        bool _appendNextMessage = false;

        byte[] _receiveBuffer;
        byte[] _appendData;

        int _receiveBufferPosition;

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
            get { return _serverAddress; }
        }

        public bool IsDecompressionEnabled
        {
            get { return _isDecompressionEnabled; }
            set { _isDecompressionEnabled = value; }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
        }

        public bool LogPackets
        {
            get { return _logPackets; }
            set { _logPackets = value; }
        }

        public ClientNetwork()
        {
            this._log = new Logger(typeof(ClientNetwork));
            this._decompression = new HuffmanDecompression();
            this._isDecompressionEnabled = false;

            this._handlers = new List<PacketHandler>[0x100];
            this._typedHandlers = new List<PacketHandler>[0x100];

            this._extendedHandlers = new List<PacketHandler>[0x100][];
            this._extendedTypedHandlers = new List<PacketHandler>[0x100][];

            for (int i = 0; i < _handlers.Length; i++)
            {
                _handlers[i] = new List<PacketHandler>();
            }

            for (int i = 0; i < _typedHandlers.Length; i++)
            {
                _typedHandlers[i] = new List<PacketHandler>();
            }
        }

        public virtual void Register(int id, string name, int length, PacketReceiveHandler onReceive)
        {
            if (id >= byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet id {0:X2} because it is greater than byte.MaxValue", id));
            }

            // _log.Debug("Register id: 0x{0:X2} Name: {1} Length: {2}", id, name, length);

            packetLengths[id] = length;
            PacketHandler handler = new PacketHandler(id, name, length, onReceive);
            _handlers[id].Add(handler);
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

            // _log.Debug("Register id: 0x{0:X2} Name: {1} Length: {2}", id, name, length);

            TypedPacketHandler handler = new TypedPacketHandler(id, name, type, length, onReceive);
            _typedHandlers[id].Add(handler);
        }

        public virtual void RegisterExtended(int extendedId, int subId, string name, int length, PacketReceiveHandler onReceive)
        {
            if (subId > byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet id {0:X2} because it is greater than byte.MaxValue", subId));
            }

            if (_extendedHandlers[extendedId] == null)
            {
                _extendedHandlers[extendedId] = new List<PacketHandler>[0x100];

                for (int i = 0; i < _extendedHandlers[extendedId].Length; i++)
                {
                    _extendedHandlers[extendedId][i] = new List<PacketHandler>();
                }
            }

            _log.Debug("Registering Extended Command: id: 0x{0:X2} subCommand: 0x{1:X2} Name: {2} Length: {3}", extendedId, subId, name, length);

            PacketHandler handler = new PacketHandler(subId, name, length, onReceive);
            _extendedHandlers[extendedId][subId].Add(handler);
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

            if (_extendedTypedHandlers[extendedId] == null)
            {
                _extendedTypedHandlers[extendedId] = new List<PacketHandler>[0x100];

                for (int i = 0; i < _extendedTypedHandlers[extendedId].Length; i++)
                {
                    _extendedTypedHandlers[extendedId][i] = new List<PacketHandler>();
                }
            }

            _log.Debug("Registering Extended Command: id: 0x{0:X2} subCommand: 0x{1:X2} Name: {2} Length: {3}", extendedId, subId, name, length);

            TypedPacketHandler handler = new TypedPacketHandler(subId, name, type, length, onReceive);
            _extendedTypedHandlers[extendedId][subId].Add(handler);
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
                if (!IPAddress.TryParse(ipAddressOrHostName, out _serverAddress))
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
                                _serverAddress = ipAddresses[i];
                                break;
                            }
                        }
                    }
                }

                _serverEndPoint = new IPEndPoint(_serverAddress, port);

                _log.Debug("Connecting...");

                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Connect(_serverEndPoint);

                if (_serverSocket.Connected)
                {
                    _log.Debug("Connected.");

                    SocketState state = new SocketState(_serverSocket, ushort.MaxValue);
                    _serverSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }

            }
            catch 
            {
                success = false;
            }

            _isConnected = success;
            return success;
        }

        public virtual void Disconnect()
        {
            if (_serverSocket != null)
            {
                _serverSocket.Shutdown(SocketShutdown.Both);
                _serverSocket.Close();
                _serverSocket = null;
                _serverEndPoint = null;
                _isDecompressionEnabled = false;
                _log.Debug("Disconnected.");
                _isConnected = false;
            }
        }

        public virtual bool Send(ISendPacket packet)
        {
            byte[] buffer = packet.Compile();

            return Send(buffer, 0, packet.Length);
        }

        public virtual bool Send(byte[] buffer, int offset, int length)
        {
            bool success = true;

            if (buffer == null || buffer.Length == 0)
            {
                throw new NetworkException("Unable to send, buffer was null or empty");
            }

            if (_logPackets)
            {
                lock (_log)
                {
                    _log.Debug("Client - > Server");
                    _log.Debug("{1}{0}", Utility.FormatBuffer(buffer, length), Environment.NewLine);
                }
            }

            try
            {
                lock (_serverSocket)
                {
                    _serverSocket.Send(buffer, offset, length, SocketFlags.None);
                }
            }
            catch (Exception e)
            {
                _log.Debug(e);
                success = false;
            }

            return success;
        }

        protected virtual void OnReceive(IAsyncResult result)
        {
            SocketState state = result.AsyncState as SocketState;

            if (state == null)
            {

                lock (_log)
                {
                    _log.Warn("Socket state was null.");
                }

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

                    if (_receiveBuffer == null)
                    {
                        _receiveBuffer = new byte[0x10000];
                    }

                    // TODO: Clean this up, this is a bit unoptimized.
                    if (_isDecompressionEnabled)
                    {
                        int outsize = 0;
                        byte[] data;

                        if (_appendNextMessage)
                        {
                            _appendNextMessage = false;
                            data = new byte[_appendData.Length + length];

                            Buffer.BlockCopy(_appendData, 0, data, 0, _appendData.Length);
                            Buffer.BlockCopy(buffer, 0, data, _appendData.Length, length);
                        }
                        else
                        {
                            data = new byte[length];
                            Buffer.BlockCopy(buffer, 0, data, 0, length);
                        }

                        while (_decompression.DecompressOnePacket(ref data, data.Length, ref _receiveBuffer, ref outsize))
                        {
                            int realLength;
                            List<PacketHandler> packetHandlers = GetHandlers(_receiveBuffer[0], _receiveBuffer[1]);

                            if (!GetPacketSize(packetHandlers, out realLength))
                            {
                                _receiveBufferPosition = 0;
                                break;
                            }
                            
                            if (realLength != outsize)
                            {
                                throw new Exception("Bad packet size!");
                            }

                            byte[] packetBuffer = new byte[realLength];
                            Buffer.BlockCopy(_receiveBuffer, 0, packetBuffer, 0, realLength);

                            string name = "Unknown";

                            if (packetHandlers.Count > 0)
                                name = packetHandlers[0].Name;

                            _AddPacket(name, packetHandlers, packetBuffer, realLength);
                            // LogPacket(packetBuffer, name, realLength);
                            // InvokeHandlers(packetHandlers, packetBuffer, realLength);
                        }

                        // We've run out of data to parse, or the packet was incomplete. If the packet was incomplete,
                        // we should save what's left for next time.
                        if (data.Length > 0)
                        {
                            _appendNextMessage = true;
                            _appendData = data;
                        }
                    }
                    else
                    {
                        Buffer.BlockCopy(buffer, 0, _receiveBuffer, _receiveBufferPosition, length);

                        _receiveBufferPosition += length;

                        int currentIndex = 0;

                        while (currentIndex < _receiveBufferPosition)
                        {
                            int realLength = length;

                            List<PacketHandler> packetHandlers = GetHandlers(_receiveBuffer[currentIndex], _receiveBuffer[currentIndex + 1]);

                            if (!GetPacketSize(packetHandlers, out realLength))
                            {
                                currentIndex = 0;
                                _receiveBufferPosition = 0;
                                break;
                            }

                            if ((_receiveBufferPosition - currentIndex) >= realLength)
                            {
                                byte[] packetBuffer = new byte[realLength];
                                Buffer.BlockCopy(_receiveBuffer, currentIndex, packetBuffer, 0, realLength);

                                string name = "Unknown";

                                if (packetHandlers.Count > 0)
                                    name = packetHandlers[0].Name;

                                _AddPacket(name, packetHandlers, packetBuffer, realLength);
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

                        _receiveBufferPosition -= currentIndex;

                        if (_receiveBufferPosition > 0)
                        {
                            Buffer.BlockCopy(_receiveBuffer, currentIndex, _receiveBuffer, 0, _receiveBufferPosition);
                        }
                    }
                }

                if (_serverSocket != null)
                {
                    _serverSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }
            }
            catch (Exception e)
            {
                _log.Debug(e);
                Disconnect();
            }
        }

        List<QueuedPacket> _QueuedPackets = new List<QueuedPacket>();
        private void _AddPacket(string name, List<PacketHandler> packetHandlers, byte[] packetBuffer, int realLength)
        {
            lock (_QueuedPackets)
            {
                _QueuedPackets.Add(new QueuedPacket(name, packetHandlers, packetBuffer, realLength));
            }
        }

        public void Update()
        {
            lock (_QueuedPackets)
            {
                foreach (QueuedPacket i in _QueuedPackets)
                {
                    LogPacket(i.PacketBuffer, i.Name, i.RealLength);
                    InvokeHandlers(i.PacketHandlers, i.PacketBuffer, i.RealLength);
                }
                _QueuedPackets.Clear();
            }
        }

        private bool GetPacketSize(List<PacketHandler> packetHandlers, out int realLength)
        {
            realLength = 0;

            if (packetHandlers.Count > 0)
            {
                if (packetHandlers[0].Length == -1)
                {
                    realLength = _receiveBuffer[2] | (_receiveBuffer[1] << 8);
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
            if (_logPackets)
            {
                lock (_log)
                {
                    _log.Debug("Server - > Client");
                    _log.Debug("Id: 0x{0:X2} Name: {1} Length: {2}", buffer[0], name, length);
                    _log.Debug("{1}{0}", Utility.FormatBuffer(buffer, length), Environment.NewLine);
                }
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

            packetHandlers.AddRange(_handlers[cmd]);
            packetHandlers.AddRange(_typedHandlers[cmd]);

            if (_extendedHandlers[cmd] != null)
            {
                packetHandlers.AddRange(_extendedHandlers[cmd][subcommand]);
            }

            if (_extendedTypedHandlers[cmd] != null)
            {
                packetHandlers.AddRange(_extendedTypedHandlers[cmd][subcommand]);
            }

            return packetHandlers;
        }
    }
}
