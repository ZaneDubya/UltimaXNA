/***************************************************************************
 *   UnicodeMessagePacket.cs
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
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.UltimaPackets.Server
{
    public class UnicodeMessagePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _model;
        readonly MessageType _msgType;
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

        public MessageType MsgType 
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
            _msgType = (MessageType)reader.ReadByte();
            _hue = reader.ReadInt16();
            _font = reader.ReadInt16();
            _language = reader.ReadInt32();
            _speakerName = reader.ReadString(30);
            _spokenText = reader.ReadUnicodeString((reader.Buffer.Length - 48) / 2);
        }
    }
}
