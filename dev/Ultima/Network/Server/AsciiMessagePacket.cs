/***************************************************************************
 *   AsciiMessagePacket.cs
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
using UltimaXNA.Ultima.Data;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class AsciiMessagePacket : RecvPacket
    {
        readonly Serial m_serial;
        readonly short m_graphic;
        readonly MessageType m_type;
        readonly short m_hue;
        readonly short m_font;
        readonly string m_name;
        readonly string m_text;

        public Serial Serial
        {
            get { return m_serial; }
        }

        public short Graphic
        {
            get { return m_graphic; }
        }

        public MessageType MsgType
        {
            get { return m_type; }
        }

        public short Hue
        {
            get { return m_hue; }
        }

        public short Font
        {
            get { return m_font; }
        }

        public string Name1
        {
            get { return m_name; }
        } 

        public string Text
        {
            get { return m_text; }
        } 


        public AsciiMessagePacket(PacketReader reader)
            : base(0x1C, "Ascii Message")
        {
            m_serial = reader.ReadInt32();
            m_graphic = reader.ReadInt16();
            m_type = (MessageType)reader.ReadByte();
            m_hue = reader.ReadInt16();
            m_font = reader.ReadInt16();
            m_name = reader.ReadString(30);
            m_text = reader.ReadString();
        }
    }
}
