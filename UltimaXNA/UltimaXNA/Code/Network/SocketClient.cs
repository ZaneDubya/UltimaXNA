#region File Description & Usings
//-----------------------------------------------------------------------------
// SocketClient.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.Net.Sockets;
using MiscUtil.Conversion;
#endregion

namespace UltimaXNA.Network
{
    public enum ConnectionStatus
    {
        Connected,
        ConnectionError
    }
    public class SocketClient
    {
        internal MiscUtil.LogFile LogFile;
        public bool UnpackPackets = false;
        private TcpClient mClient;

        
        private const int mReadBufferSize = 4096 * 8;          // The buffer where we will store the received data.
        private byte[] mReadBuffer = new byte[mReadBufferSize];
        public event UserEventDlg UserDisconnected;         // Events to call when the client received data...
        public event DataReceivedDlg DataReceived;          // Or got disconnected
        public readonly int id;                             // unique id for use with the server
        public bool disconnected = false;                   // To keep track if we have already disconnected.

        private const int mOutBufferSize = 4096 * 8;          // Increased to 32kb from 16kb issue9 (http://code.google.com/p/ultimaxna/issues/detail?id=9) --ZDW 6/14/2009
        private byte[] outdata = new byte[mOutBufferSize];

        private bool mAppendNextMessage = false;
        private byte[] mAppendData;

        // This constructor should only be called from the listener
        // * client - The TcpClient received from the listner
        // * id - unique id for the client
        internal SocketClient(TcpClient client, int id)
        {
            this.id = id;
            this.mClient = client;
            this.mClient.NoDelay = true;
            this.StartListening();
        }

        // Creates a SocketClient that automaticly connects.
        public SocketClient(string ip, int port, out ConnectionStatus status)
        {
            // For the client we do not bother about the id so just set it to
            // byte.max(255). This way you can easily separate client-SocketClients
            // and server-SocketClients when you debug.
            this.id = int.MaxValue;
            this.mClient = new TcpClient();
            // If we do not set this to true the TcpClient might wait a couple of
            // millseconds before it sends away the data so it can pack more data
            // into one package and send everything at once. We are only interested
            // in delivering the data as fast as possible and do not realy care
            // about speed so we set it to true.
            this.mClient.NoDelay = true;
            try
            {
                this.mClient.Connect(ip, port);
                this.StartListening();
                status = ConnectionStatus.Connected;
            }
            catch (SocketException e)
            {
                string iErrorMsg = e.Message;
                status = ConnectionStatus.ConnectionError;
            }
        }

        // When the Client is destroyed we want to close the connection. I honestly
        // do not know how necessary this is but we do it anyway.
        ~SocketClient()
        {
                this.Disconnect();
        }

        public void Disconnect()
        {
            // Checking if you are connected or not is harder than you might think. Even if
            // the Connected-property on the TcpClient is true you might already be disconnected.
            // This is why we surround the code with try/catch and manually store a bool that
            // indicates if we have disconnected or not. The Close(500)-method will give the
            // TcpClient 500 milliseconds to send away pending data while Close() just breaks
            // the connection instantly.
            if (!this.disconnected)
            {
                this.disconnected = true;
                try
                {
                    if (this.mClient.Connected)
                        this.mClient.GetStream().Close(500);
                    else
                        this.mClient.Close();
                }
                catch (ObjectDisposedException)
                {
                    this.mClient.Close();
                }
                // When we have disconnected we fire an event to tell the program that we have got disconnected.
                if (this.UserDisconnected != null)
                    this.UserDisconnected(this, this);
            }
        }

        private void StartListening()
        {
            // When data has been received the method in the 4th parameter(this.StreamReceived)
            // will be called and an IAsyncResult will be passed to that method.
            this.mClient.GetStream().BeginRead(mReadBuffer, 0, mReadBufferSize, this.StreamReceived, null);
        }

        private void StreamReceived(IAsyncResult ar)
        {
            // Note: This metod is not executed on the same thread as the one that called BeginRead.
            // This means that you have to be careful when you modify data in the DataReceived-event.
            int bytesRead = 0;
            try
            {
                //  Lock the stream to prevent objects from other threads to access it at the same
                // time Then call EndRead(ar) where ar is the IAsyncResult the TcpClient sent to us.
                // This will return the number of bytes that has been received.
                lock (mClient.GetStream())
                bytesRead = this.mClient.GetStream().EndRead(ar);
            }
            catch (Exception e)
            {
                string iErrorMessage = e.Message;
                // If we get an exception bytesread will remain 0 and we will disconnect.
                throw (e);
            }
            if (bytesRead == 0)
            {
                // If bytesRead is 0 we have lost connection to the server.
                this.Disconnect();
                return;
            }

            byte[] data;
            if (mAppendNextMessage)
            {
                mAppendNextMessage = false;
                data = new byte[mAppendData.Length + bytesRead];
                Array.Copy(mAppendData, 0, data, 0, mAppendData.Length);
                Array.Copy(mReadBuffer, 0, data, mAppendData.Length, bytesRead);
            }
            else
            {
                // Create a new buffer with just enough space to fit the received data.
                data = new byte[bytesRead];
                // Copy the data from the readBuffer to data. The reason why we do this instead of
                // sending the entire readBuffer with the DataReceived-event is that we want to start
                // listening for new messages asap. But if we start listening before we call the
                // DataReceived-event and new data is received before the DataReceived-method has
                // finished the readBuffer will change and the DataReceived-method will process
                // completly corrupt data.
                Array.Copy(mReadBuffer, 0, data, 0, bytesRead);
            }
            // Start listening for new data
            this.StartListening();

            // Turn the data into a packet...
            if (this.UnpackPackets)
            {
                int outsize = 0;
                while (MiscUtil.HuffmanDecomp.DecompressNew(ref data, data.Length, ref outdata, ref outsize))
                {
                    for (int iPosition = 0; iPosition < outsize; )
                    {
                        int iSize = mGetSize(ref outdata, iPosition, outsize);
                        if (iSize != outsize)
                            throw new Exception("Weird packet size!");

                        byte[] iData = new byte[iSize];
                        Array.Copy(outdata, iPosition, iData, 0, iSize);
                        // LogFile.WritePacket(iData, ((OpCodes)iData[0]).ToString());
                        Packet iPacket = new Packet(iData);
                        // Tell the program that owns the client that we have received data.
                        if (this.DataReceived != null)
                            this.DataReceived(this, iPacket);
                        iPosition += iSize;
                    }
                }
                // We've run out of data to parse, or the packet was incomplete. If the packet was incomplete,
                // we should save what's left for next time.
                if (data.Length > 0)
                {
                    mAppendNextMessage = true;
                    mAppendData = data;
                }

            }
            else
            {
                // LogFile.WritePacket(data, ((OpCodes)data[0]).ToString());
                Packet iPacket = new Packet(data);
                // Tell the program that owns the client that we have received data.
                if (this.DataReceived != null)
                    this.DataReceived(this, iPacket);
            }
            
        }

        private int mGetSize(ref byte[] data, int nPosition, int nTotalLength)
        {
            byte iOpCode = data[nPosition];

            int iret;

            switch ((OpCodes)data[nPosition])
            {
                case OpCodes.SMSG_STATUSINFO :
                    iret = (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                    return iret;
                case OpCodes.SMSG_MOBILEUPDATE :
                    return 19;
                case OpCodes.SMSG_CHARLOCALE :
                    return 37;
                case OpCodes.SMSG_LIGHTLEVEL :
                    return 2;
                case OpCodes.SMSG_PERSONALLIGHTLEVEL :
                    return 6;
                case OpCodes.SMSG_MobileIncoming:
                    iret = (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                    return iret;
                case OpCodes.SMSG_ENABLEFEATURES :
                    return 3;
                case OpCodes.SMSG_CHARACTERLIST :
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.MSG_CLIENTVERSION :
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.MSG_GENERALINFO :
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_SEASONALINFORMATION :
                    return 3;
                case OpCodes.SMSG_SETWEATHER :
                    return 4;
                case OpCodes.MSG_WARMODE :
                    return 5;
                case OpCodes.SMSG_OBJECTPROPERTYLIST:
                    return 9;
                case OpCodes.SMSG_LOGINCOMPLETE:
                    return 1;
                case OpCodes.SMSG_THETIME:
                    return 4;
                case OpCodes.SMSG_UNICODEMESSAGE:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_WorldItem:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_MobileAnimation:
                    return 14;
                case OpCodes.SMSG_DELETEOBJECT:
                    return 5;
                case OpCodes.SMSG_MobileMoving:
                    return 17;
                case OpCodes.SMSG_SENDSPEECH:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_MOVEACK:
                    return 3;
                case OpCodes.SMSG_PLAYSOUNDEFFECT:
                    return 12;
                case OpCodes.SMSG_MOVEREJ:
                    return 8;
                case OpCodes.SMSG_CONTAINER:
                    return 7;
                case OpCodes.SMSG_ADDMULTIPLEITEMSTOCONTAINER:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_ADDSINGLEITEMTOCONTAINER:
                    return 21;
                case OpCodes.SMSG_REJECTMOVEITEMREQ:
                    return 2;
                case OpCodes.MSG_SENDSKILLS:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_CORPSECLOTHING:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1); 
                case OpCodes.SMSG_CLILOCMSG:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_GRAPHICALEFFECT:
                    return 36;
                case OpCodes.SMSG_UPDATECURRENTHEALTH:
                    return 9;
                case OpCodes.SMSG_UPDATECURRENTMANA:
                    return 9;
                case OpCodes.SMSG_UPDATECURRENTSTAMINA:
                    return 9;
                case OpCodes.MSG_RESURRECTIONMENU:
                    return 2;
                case OpCodes.SMSG_WORNITEM:
                    return 15;
                case OpCodes.MSG_REQUESTNAME:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_COMPRESSEDGUMP:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_PlayMusic:
                    return 3;
                case OpCodes.SMSG_OpenBuyWindow:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_OpenPaperdoll:
                    return 66;
                case OpCodes.MSG_BatchQueryProperties:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.MSG_BuyItemFromVendor:
                    return (int)EndianBitConverter.Big.ToUInt16(data, nPosition + 1);
                case OpCodes.SMSG_DRAGITEM:
                    return 26;
                case OpCodes.MSG_TargetCursor:
                    return 19;
                default :
                    throw new Exception("Unknown packet! Opcocde: 0x" + MiscUtil.HexEncoding.ToString(iOpCode));
            }
        }

        public void SendData(byte[] b)
        {
            try
            {
                // Lock the stream and send away data asyncronously. We can register a callback-method
                // to be called when the data has been received like we did with BeginRead but in this
                // example there is no need to know that the data has been sent so we set it to null.
                // BeginWrite can throw an exception when the client has been disconnected or when you
                // pass a buffer with incorrect size, we only catch the ones that occur when you have got
                // disconnected.
                lock (mClient.GetStream())
                    this.mClient.GetStream().BeginWrite(b, 0, b.Length, null, null);
            }
            catch (System.IO.IOException ioe)
            {
                this.Disconnect();
                throw (ioe);
                
            }
            catch (InvalidOperationException ioe)
            {
                this.Disconnect();
                throw (ioe);
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Copies the data in the memorystream from position 0 to the memorystreams current position and then
        // sends the data away.
        public void SendMemoryStream(System.IO.MemoryStream ms)
        {
            lock (ms)
            {
                // We will probably have to do this 100 times in our mainprogram to get a bytebuffer from a
                // memorystream so we can just as well put a method for it in here. Do not bother about this
                // method untill you have read the packaging-section.
                int bytesWritten = (int)ms.Position;
                byte[] result = new byte[bytesWritten];

                ms.Position = 0;
                ms.Read(result, 0, bytesWritten);
                this.SendData(result);
            }
        }
    }
}
