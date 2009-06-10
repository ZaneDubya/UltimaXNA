﻿/***************************************************************************
 *                                NetState.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: NetState.cs 214 2007-08-27 08:49:15Z mark $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Server;
using Server.Accounting;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Menus;
using Server.HuePickers;
using Server.Diagnostics;

namespace Server.Network
{
    public interface IPacketEncoder
    {
        void EncodeOutgoingPacket(NetState to, ref byte[] buffer, ref int length);
        void DecodeIncomingPacket(NetState from, ref byte[] buffer, ref int length);
    }

    public delegate void NetStateCreatedCallback(NetState ns);

    public class NetState
    {
        private Socket m_Socket;
        private IPAddress m_Address;
        private ByteQueue m_Buffer;
        private byte[] m_RecvBuffer;
        private SendQueue m_SendQueue;
        private bool m_Seeded;
        private bool m_Running;
        private AsyncCallback m_OnReceive, m_OnSend;
        private MessagePump m_MessagePump;
        private ServerInfo[] m_ServerInfo;
        private IAccount m_Account;
        private Mobile m_Mobile;
        private CityInfo[] m_CityInfo;
        private List<Gump> m_Gumps;
        private List<HuePicker> m_HuePickers;
        private List<IMenu> m_Menus;
        private List<SecureTrade> m_Trades;
        private int m_Sequence;
        private bool m_CompressionEnabled;
        private string m_ToString;
        private ClientVersion m_Version;
        private bool m_SentFirstPacket;
        private bool m_BlockAllPackets;

        private DateTime m_ConnectedOn;

        public DateTime ConnectedOn
        {
            get
            {
                return m_ConnectedOn;
            }
        }

        public TimeSpan ConnectedFor
        {
            get
            {
                return (DateTime.Now - m_ConnectedOn);
            }
        }

        internal int m_Seed;
        internal int m_AuthID;

        public IPAddress Address
        {
            get
            {
                return m_Address;
            }
        }

        private int m_Flags;

        private static bool m_Paused;

        [Flags]
        private enum AsyncState
        {
            Pending = 0x01,
            Paused = 0x02
        }

        private AsyncState m_AsyncState;
        private object m_AsyncLock = new object();

        public static void Pause()
        {
            m_Paused = true;

            for (int i = 0; i < m_Instances.Count; ++i)
            {
                NetState ns = m_Instances[i];

                lock (ns.m_AsyncLock)
                {
                    ns.m_AsyncState |= AsyncState.Paused;
                }
            }
        }

        private void InternalBeginReceive()
        {
            m_AsyncState |= AsyncState.Pending;

            m_Socket.BeginReceive(m_RecvBuffer, 0, m_RecvBuffer.Length, SocketFlags.None, m_OnReceive, m_Socket);
        }

        public static void Resume()
        {
            m_Paused = false;

            for (int i = 0; i < m_Instances.Count; ++i)
            {
                NetState ns = m_Instances[i];

                if (ns.m_Socket == null)
                {
                    continue;
                }

                lock (ns.m_AsyncLock)
                {
                    ns.m_AsyncState &= ~AsyncState.Paused;

                    try
                    {
                        if ((ns.m_AsyncState & AsyncState.Pending) == 0)
                            ns.InternalBeginReceive();
                    }
                    catch (Exception ex)
                    {
                        TraceException(ex);
                        ns.Dispose(false);
                    }
                }
            }
        }

        private IPacketEncoder m_Encoder = null;

        public IPacketEncoder PacketEncoder
        {
            get
            {
                return m_Encoder;
            }
            set
            {
                m_Encoder = value;
            }
        }

        private static NetStateCreatedCallback m_CreatedCallback;

        public static NetStateCreatedCallback CreatedCallback
        {
            get
            {
                return m_CreatedCallback;
            }
            set
            {
                m_CreatedCallback = value;
            }
        }

        public bool SentFirstPacket
        {
            get
            {
                return m_SentFirstPacket;
            }
            set
            {
                m_SentFirstPacket = value;
            }
        }

        public bool BlockAllPackets
        {
            get
            {
                return m_BlockAllPackets;
            }
            set
            {
                m_BlockAllPackets = value;
            }
        }

        public int Flags
        {
            get
            {
                return m_Flags;
            }
            set
            {
                m_Flags = value;
            }
        }

        public ClientVersion Version
        {
            get
            {
                return m_Version;
            }
            set
            {
                m_Version = value;

                if (value >= m_Version6017)
                    m_Post6017 = true;
                else
                    m_Post6017 = false;
            }
        }

        public bool IsUOTDClient
        {
            get
            {
                return ((m_Flags & 0x100) != 0 || (m_Version != null && m_Version.Type == ClientType.UOTD));
            }
        }

        private static ClientVersion m_Version6017 = new ClientVersion("6.0.1.7");

        private bool m_Post6017;

        public bool IsPost6017
        {
            get
            {
                return m_Post6017;
            }
        }

        public List<SecureTrade> Trades
        {
            get
            {
                return m_Trades;
            }
        }

        public void ValidateAllTrades()
        {
            for (int i = m_Trades.Count - 1; i >= 0; --i)
            {
                if (i >= m_Trades.Count)
                {
                    continue;
                }

                SecureTrade trade = m_Trades[i];

                if (trade.From.Mobile.Deleted || trade.To.Mobile.Deleted || !trade.From.Mobile.Alive || !trade.To.Mobile.Alive || !trade.From.Mobile.InRange(trade.To.Mobile, 2) || trade.From.Mobile.Map != trade.To.Mobile.Map)
                {
                    trade.Cancel();
                }
            }
        }

        public void CancelAllTrades()
        {
            for (int i = m_Trades.Count - 1; i >= 0; --i)
            {
                if (i < m_Trades.Count)
                {
                    m_Trades[i].Cancel();
                }
            }
        }

        public void RemoveTrade(SecureTrade trade)
        {
            m_Trades.Remove(trade);
        }

        public SecureTrade FindTrade(Mobile m)
        {
            for (int i = 0; i < m_Trades.Count; ++i)
            {
                SecureTrade trade = m_Trades[i];

                if (trade.From.Mobile == m || trade.To.Mobile == m)
                {
                    return trade;
                }
            }

            return null;
        }

        public SecureTradeContainer FindTradeContainer(Mobile m)
        {
            for (int i = 0; i < m_Trades.Count; ++i)
            {
                SecureTrade trade = m_Trades[i];

                SecureTradeInfo from = trade.From;
                SecureTradeInfo to = trade.To;

                if (from.Mobile == m_Mobile && to.Mobile == m)
                {
                    return from.Container;
                }
                else if (from.Mobile == m && to.Mobile == m_Mobile)
                {
                    return to.Container;
                }
            }

            return null;
        }

        public SecureTradeContainer AddTrade(NetState state)
        {
            SecureTrade newTrade = new SecureTrade(m_Mobile, state.m_Mobile);

            m_Trades.Add(newTrade);
            state.m_Trades.Add(newTrade);

            return newTrade.From.Container;
        }

        public bool CompressionEnabled
        {
            get
            {
                return m_CompressionEnabled;
            }
            set
            {
                m_CompressionEnabled = value;
            }
        }

        public int Sequence
        {
            get
            {
                return m_Sequence;
            }
            set
            {
                m_Sequence = value;
            }
        }

        public IEnumerable<Gump> Gumps
        {
            get
            {
                return m_Gumps;
            }
        }

        public IEnumerable<HuePicker> HuePickers
        {
            get
            {
                return m_HuePickers;
            }
        }

        public IEnumerable<IMenu> Menus
        {
            get
            {
                return m_Menus;
            }
        }

        private static int m_GumpCap = 512, m_HuePickerCap = 512, m_MenuCap = 512;

        public static int GumpCap
        {
            get
            {
                return m_GumpCap;
            }
            set
            {
                m_GumpCap = value;
            }
        }

        public static int HuePickerCap
        {
            get
            {
                return m_HuePickerCap;
            }
            set
            {
                m_HuePickerCap = value;
            }
        }

        public static int MenuCap
        {
            get
            {
                return m_MenuCap;
            }
            set
            {
                m_MenuCap = value;
            }
        }

        public void WriteConsole(string text)
        {
            Console.WriteLine("Client: {0}: {1}", this, text);
        }

        public void WriteConsole(string format, params object[] args)
        {
            WriteConsole(String.Format(format, args));
        }

        public void AddMenu(IMenu menu)
        {
            if (m_Menus == null)
            {
                m_Menus = new List<IMenu>();
            }

            if (m_Menus.Count < m_MenuCap)
            {
                m_Menus.Add(menu);
            }
            else
            {
                WriteConsole("Exceeded menu cap, disconnecting...");
                Dispose();
            }
        }

        public void RemoveMenu(IMenu menu)
        {
            if (m_Menus != null)
            {
                m_Menus.Remove(menu);
            }
        }

        public void RemoveMenu(int index)
        {
            if (m_Menus != null)
            {
                m_Menus.RemoveAt(index);
            }
        }

        public void ClearMenus()
        {
            if (m_Menus != null)
            {
                m_Menus.Clear();
            }
        }

        public void AddHuePicker(HuePicker huePicker)
        {
            if (m_HuePickers == null)
            {
                m_HuePickers = new List<HuePicker>();
            }

            if (m_HuePickers.Count < m_HuePickerCap)
            {
                m_HuePickers.Add(huePicker);
            }
            else
            {
                WriteConsole("Exceeded hue picker cap, disconnecting...");
                Dispose();
            }
        }

        public void RemoveHuePicker(HuePicker huePicker)
        {
            if (m_HuePickers != null)
            {
                m_HuePickers.Remove(huePicker);
            }
        }

        public void RemoveHuePicker(int index)
        {
            if (m_HuePickers != null)
            {
                m_HuePickers.RemoveAt(index);
            }
        }

        public void ClearHuePickers()
        {
            if (m_HuePickers != null)
            {
                m_HuePickers.Clear();
            }
        }

        public void AddGump(Gump gump)
        {
            if (m_Gumps == null)
            {
                m_Gumps = new List<Gump>();
            }

            if (m_Gumps.Count < m_GumpCap)
            {
                m_Gumps.Add(gump);
            }
            else
            {
                WriteConsole("Exceeded gump cap, disconnecting...");
                Dispose();
            }
        }

        public void RemoveGump(Gump gump)
        {
            if (m_Gumps != null)
            {
                m_Gumps.Remove(gump);
            }
        }

        public void RemoveGump(int index)
        {
            if (m_Gumps != null)
            {
                m_Gumps.RemoveAt(index);
            }
        }

        public void ClearGumps()
        {
            if (m_Gumps != null)
            {
                m_Gumps.Clear();
            }
        }

        public CityInfo[] CityInfo
        {
            get
            {
                return m_CityInfo;
            }
            set
            {
                m_CityInfo = value;
            }
        }

        public Mobile Mobile
        {
            get
            {
                return m_Mobile;
            }
            set
            {
                m_Mobile = value;
            }
        }

        public ServerInfo[] ServerInfo
        {
            get
            {
                return m_ServerInfo;
            }
            set
            {
                m_ServerInfo = value;
            }
        }

        public IAccount Account
        {
            get
            {
                return m_Account;
            }
            set
            {
                m_Account = value;
            }
        }

        public override string ToString()
        {
            return m_ToString;
        }

        private static List<NetState> m_Instances = new List<NetState>();

        public static List<NetState> Instances
        {
            get
            {
                return m_Instances;
            }
        }

        private static BufferPool m_ReceiveBufferPool = new BufferPool("Receive", 2048, 2048);

        public NetState(Socket socket, MessagePump messagePump)
        {
            m_Socket = socket;
            m_Buffer = new ByteQueue();
            m_Seeded = false;
            m_Running = false;
            m_RecvBuffer = m_ReceiveBufferPool.AcquireBuffer();
            m_MessagePump = messagePump;
            m_Gumps = new List<Gump>();
            m_HuePickers = new List<HuePicker>();
            m_Menus = new List<IMenu>();
            m_Trades = new List<SecureTrade>();

            m_SendQueue = new SendQueue();

            m_NextCheckActivity = DateTime.Now + TimeSpan.FromMinutes(0.5);

            m_Instances.Add(this);

            try
            {
                m_Address = Utility.Intern(((IPEndPoint)m_Socket.RemoteEndPoint).Address);
                m_ToString = m_Address.ToString();
            }
            catch (Exception ex)
            {
                TraceException(ex);
                m_Address = IPAddress.None;
                m_ToString = "(error)";
            }

            m_ConnectedOn = DateTime.Now;

            if (m_CreatedCallback != null)
            {
                m_CreatedCallback(this);
            }
        }

        public PacketHandler GetHandler(int packetID)
        {
            if (IsPost6017)
                return PacketHandlers.Get6017Handler(packetID);
            else
                return PacketHandlers.GetHandler(packetID);
        }

        public virtual void Send(Packet p)
        {
            if (m_Socket == null || m_BlockAllPackets)
            {
                p.OnSend();
                return;
            }

            PacketSendProfile prof = PacketSendProfile.Acquire(p.GetType());

            int length;
            byte[] buffer = p.Compile(m_CompressionEnabled, out length);

            if (buffer != null)
            {
                if (buffer.Length <= 0 || length <= 0)
                {
                    p.OnSend();
                    return;
                }

                if (prof != null)
                {
                    prof.Start();
                }

                if (m_Encoder != null)
                {
                    m_Encoder.EncodeOutgoingPacket(this, ref buffer, ref length);
                }

                try
                {
                    SendQueue.Gram gram;

                    lock (m_SendQueue)
                    {
                        gram = m_SendQueue.Enqueue(buffer, length);
                    }

                    if (gram != null)
                    {
                        try
                        {
                            m_Socket.BeginSend(gram.Buffer, 0, gram.Length, SocketFlags.None, m_OnSend, m_Socket);
                        }
                        catch (Exception ex)
                        {
                            TraceException(ex);
                            Dispose(false);
                        }
                    }
                }
                catch (CapacityExceededException)
                {
                    Console.WriteLine("Client: {0}: Too much data pending, disconnecting...", this);
                    Dispose(false);
                }

                p.OnSend();

                if (prof != null)
                {
                    prof.Finish(length);
                }
            }
            else
            {
                Console.WriteLine("Client: {0}: null buffer send, disconnecting...", this);
                using (StreamWriter op = new StreamWriter("null_send.log", true))
                {
                    op.WriteLine("{0} Client: {1}: null buffer send, disconnecting...", DateTime.Now, this);
                    op.WriteLine(new System.Diagnostics.StackTrace());
                }
                Dispose();
            }
        }

        public static void FlushAll()
        {
            for (int i = 0; i < m_Instances.Count; ++i)
            {
                NetState ns = m_Instances[i];

                ns.Flush();
            }
        }

        public bool Flush()
        {
            if (m_Socket == null || !m_SendQueue.IsFlushReady)
            {
                return false;
            }

            SendQueue.Gram gram;

            lock (m_SendQueue)
            {
                gram = m_SendQueue.CheckFlushReady();
            }

            if (gram != null)
            {
                try
                {
                    m_Socket.BeginSend(gram.Buffer, 0, gram.Length, SocketFlags.None, m_OnSend, m_Socket);
                    return true;
                }
                catch (Exception ex)
                {
                    TraceException(ex);
                    Dispose(false);
                }
            }

            return false;
        }

        private static int m_CoalesceSleep = -1;

        public static int CoalesceSleep
        {
            get
            {
                return m_CoalesceSleep;
            }
            set
            {
                m_CoalesceSleep = value;
            }
        }

        private void OnSend(IAsyncResult asyncResult)
        {
            Socket s = (Socket)asyncResult.AsyncState;

            try
            {
                int bytes = s.EndSend(asyncResult);

                if (bytes <= 0)
                {
                    Dispose(false);
                    return;
                }

                m_NextCheckActivity = DateTime.Now + TimeSpan.FromMinutes(1.2);

                if (m_CoalesceSleep >= 0)
                {
                    Thread.Sleep(m_CoalesceSleep);
                }

                SendQueue.Gram gram;

                lock (m_SendQueue)
                {
                    gram = m_SendQueue.Dequeue();
                }

                if (gram != null)
                {
                    try
                    {
                        s.BeginSend(gram.Buffer, 0, gram.Length, SocketFlags.None, m_OnSend, s);
                    }
                    catch (Exception ex)
                    {
                        TraceException(ex);
                        Dispose(false);
                    }
                }
            }
            catch (Exception)
            {
                Dispose(false);
            }
        }

        public void Start()
        {
            m_OnReceive = new AsyncCallback(OnReceive);
            m_OnSend = new AsyncCallback(OnSend);

            m_Running = true;

            if (m_Socket == null || m_Paused)
            {
                return;
            }

            try
            {
                lock (m_AsyncLock)
                {
                    if ((m_AsyncState & (AsyncState.Pending | AsyncState.Paused)) == 0)
                    {
                        InternalBeginReceive();
                    }
                }
            }
            catch (Exception ex)
            {
                TraceException(ex);
                Dispose(false);
            }
        }

        public void LaunchBrowser(string url)
        {
            Send(new MessageLocalized(Serial.MinusOne, -1, MessageType.Label, 0x35, 3, 501231, "", ""));
            Send(new LaunchBrowser(url));
        }

        private DateTime m_NextCheckActivity;

        public bool CheckAlive()
        {
            if (m_Socket == null)
                return false;

            if (DateTime.Now < m_NextCheckActivity)
            {
                return true;
            }

            Console.WriteLine("Client: {0}: Disconnecting due to inactivity...", this);

            Dispose();
            return false;
        }

        private void OnReceive(IAsyncResult asyncResult)
        {
            Socket s = (Socket)asyncResult.AsyncState;

            try
            {
                int byteCount = s.EndReceive(asyncResult);

                if (byteCount > 0)
                {
                    m_NextCheckActivity = DateTime.Now + TimeSpan.FromMinutes(1.2);

                    byte[] buffer = m_RecvBuffer;

                    if (m_Encoder != null)
                        m_Encoder.DecodeIncomingPacket(this, ref buffer, ref byteCount);

                    lock (m_Buffer)
                        m_Buffer.Enqueue(buffer, 0, byteCount);

                    m_MessagePump.OnReceive(this);

                    lock (m_AsyncLock)
                    {
                        m_AsyncState &= ~AsyncState.Pending;

                        if ((m_AsyncState & AsyncState.Paused) == 0)
                        {
                            try
                            {
                                InternalBeginReceive();
                            }
                            catch (Exception ex)
                            {
                                TraceException(ex);
                                Dispose(false);
                            }
                        }
                    }
                }
                else
                {
                    Dispose(false);
                }
            }
            catch
            {
                Dispose(false);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public static void TraceException(Exception ex)
        {
            try
            {
                using (StreamWriter op = new StreamWriter("network-errors.log", true))
                {
                    op.WriteLine("# {0}", DateTime.Now);

                    op.WriteLine(ex);

                    op.WriteLine();
                    op.WriteLine();
                }
            }
            catch
            {
            }

            try
            {
                Console.WriteLine(ex);
            }
            catch
            {
            }
        }

        private bool m_Disposing;

        public virtual void Dispose(bool flush)
        {
            if (m_Socket == null || m_Disposing)
            {
                return;
            }

            m_Disposing = true;

            if (flush)
                flush = Flush();

            try
            {
                m_Socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException ex)
            {
                TraceException(ex);
            }

            try
            {
                m_Socket.Close();

                SocketPool.ReleaseSocket(m_Socket);
            }
            catch (SocketException ex)
            {
                TraceException(ex);
            }

            if (m_RecvBuffer != null)
                m_ReceiveBufferPool.ReleaseBuffer(m_RecvBuffer);

            m_Socket = null;

            m_Buffer = null;
            m_RecvBuffer = null;
            m_OnReceive = null;
            m_OnSend = null;
            m_Running = false;

            m_Disposed.Enqueue(this);

            if ( /*!flush &&*/ !m_SendQueue.IsEmpty)
            {
                lock (m_SendQueue)
                    m_SendQueue.Clear();
            }
        }

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.5), new TimerCallback(CheckAllAlive));
        }

        public static void CheckAllAlive()
        {
            try
            {
                for (int i = 0; i < m_Instances.Count; ++i)
                {
                    m_Instances[i].CheckAlive();
                }
            }
            catch (Exception ex)
            {
                TraceException(ex);
            }
        }

        private static Queue m_Disposed = Queue.Synchronized(new Queue());

        public static void ProcessDisposedQueue()
        {
            int breakout = 0;

            while (breakout < 200 && m_Disposed.Count > 0)
            {
                ++breakout;

                NetState ns = (NetState)m_Disposed.Dequeue();

                Mobile m = ns.m_Mobile;
                IAccount a = ns.m_Account;

                if (m != null)
                {
                    m.NetState = null;
                    ns.m_Mobile = null;
                }

                ns.m_Gumps.Clear();
                ns.m_Menus.Clear();
                ns.m_HuePickers.Clear();
                ns.m_Account = null;
                ns.m_ServerInfo = null;
                ns.m_CityInfo = null;

                m_Instances.Remove(ns);

                if (a != null)
                {
                    ns.WriteConsole("Disconnected. [{0} Online] [{1}]", m_Instances.Count, a);
                }
                else
                {
                    ns.WriteConsole("Disconnected. [{0} Online]", m_Instances.Count);
                }
            }
        }

        public bool Running
        {
            get
            {
                return m_Running;
            }
        }

        public bool Seeded
        {
            get
            {
                return m_Seeded;
            }
            set
            {
                m_Seeded = value;
            }
        }

        public Socket Socket
        {
            get
            {
                return m_Socket;
            }
        }

        public ByteQueue Buffer
        {
            get
            {
                return m_Buffer;
            }
        }

        public ExpansionInfo ExpansionInfo
        {
            get
            {
                for (int i = ExpansionInfo.Table.Length - 1; i >= 0; i--)
                {
                    ExpansionInfo info = ExpansionInfo.Table[i];

                    if ((info.RequiredClient != null && this.Version >= info.RequiredClient) || ((this.Flags & info.NetStateFlag) != 0))
                    {
                        return info;
                    }
                }

                return ExpansionInfo.GetInfo(Expansion.None);
            }
        }

        public Expansion Expansion
        {
            get
            {
                return (Expansion)this.ExpansionInfo.ID;
            }
        }

        public bool SupportsExpansion(ExpansionInfo info, bool checkCoreExpansion)
        {
            if (info == null || (checkCoreExpansion && (int)Core.Expansion < info.ID))
                return false;

            if (info.RequiredClient != null)
                return (this.Version >= info.RequiredClient);

            return ((this.Flags & info.NetStateFlag) != 0);
        }

        public bool SupportsExpansion(Expansion ex, bool checkCoreExpansion)
        {
            return SupportsExpansion(ExpansionInfo.GetInfo(ex), checkCoreExpansion);
        }

        public bool SupportsExpansion(Expansion ex)
        {
            return SupportsExpansion(ex, true);
        }

        public bool SupportsExpansion(ExpansionInfo info)
        {
            return SupportsExpansion(info, true);
        }
    }
}