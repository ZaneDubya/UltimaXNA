/***************************************************************************
 *   UnicodeMessagePacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
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
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class UnicodeMessagePacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_model;
        readonly MessageTypes m_msgType;
        readonly short m_hue;
        readonly short m_font;
        readonly int m_language;
        readonly string m_speakerName;
        readonly string m_spokenText;

        public Serial Serial 
        {
            get { return m_serial; } 
        }
        
        public short Model 
        {
            get { return m_model; } 
        }

        public MessageTypes MsgType 
        {
            get { return m_msgType; }
        }
        
        public short Hue 
        {
            get { return m_hue; } 
        }
        
        public short Font 
        {
            get { return m_font; }
        }

        public int Language 
        {
            get { return m_language; }
        }

        public string SpeakerName 
        {
            get { return m_speakerName; } 
        }
        
        public string SpokenText 
        {
            get { return m_spokenText; } 
        }
        
        public UnicodeMessagePacket(PacketReader reader)
            : base(0xAE, "Unicode Message")
        {
            m_serial = reader.ReadInt32();
            m_model = reader.ReadInt16();
            m_msgType = (MessageTypes)reader.ReadByte();
            m_hue = reader.ReadInt16();
            m_font = reader.ReadInt16();
            m_language = reader.ReadInt32();
            m_speakerName = reader.ReadString(30);
            m_spokenText = reader.ReadUnicodeString((reader.Buffer.Length - 48) / 2);
        }
    }
}
