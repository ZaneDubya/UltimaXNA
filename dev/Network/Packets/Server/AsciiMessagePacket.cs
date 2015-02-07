/***************************************************************************
 *   AsciiMessagePacket.cs
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
using UltimaXNA.Network;
#endregion

namespace UltimaXNA.Network.Packets.Server
{
    public class AsciiMessagePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _graphic;
        readonly MessageType _type;
        readonly short _hue;
        readonly short _font;
        readonly string _name;
        readonly string _text;

        public Serial Serial
        {
            get { return _serial; }
        }

        public short Graphic
        {
            get { return _graphic; }
        }

        public MessageType MsgType
        {
            get { return _type; }
        }

        public short Hue
        {
            get { return _hue; }
        }

        public short Font
        {
            get { return _font; }
        }

        public string Name1
        {
            get { return _name; }
        } 

        public string Text
        {
            get { return _text; }
        } 


        public AsciiMessagePacket(PacketReader reader)
            : base(0x1C, "Ascii Message")
        {
            _serial = reader.ReadInt32();
            _graphic = reader.ReadInt16();
            _type = (MessageType)reader.ReadByte();
            _hue = reader.ReadInt16();
            _font = reader.ReadInt16();
            _name = reader.ReadString(30);
            _text = reader.ReadString();
        }
    }
}
