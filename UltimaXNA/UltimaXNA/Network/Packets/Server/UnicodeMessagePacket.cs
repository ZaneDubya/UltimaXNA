using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class UnicodeMessagePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _model;
        readonly byte _msgType;
        readonly short _hue;
        readonly short _font;
        readonly int _language;
        readonly string _speakerName;
        readonly string _spokenText;

        public Serial Serial 
        {
            get { return _serial; } 
        }
        
        public short Model 
        {
            get { return _model; } 
        }
        
        public byte MsgType 
        {
            get { return _msgType; }
        }
        
        public short Hue 
        {
            get { return _hue; } 
        }
        
        public short Font 
        {
            get { return _font; }
        }

        public int Language 
        {
            get { return _language; }
        }

        public string SpeakerName 
        {
            get { return _speakerName; } 
        }
        
        public string SpokenText 
        {
            get { return _spokenText; } 
        }
        
        public UnicodeMessagePacket(PacketReader reader)
            : base(0xAE, "Unicode Message")
        {
            _serial = reader.ReadInt32();
            _model = reader.ReadInt16();
            _msgType = reader.ReadByte();
            _hue = reader.ReadInt16();
            _font = reader.ReadInt16();
            _language = reader.ReadInt32();
            _speakerName = reader.ReadString(30);
            _spokenText = reader.ReadUnicodeString((reader.Buffer.Length - 48) / 2);
        }
    }
}
