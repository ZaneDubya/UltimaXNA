using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class AsciiMessagePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _graphic;
        readonly byte _type;
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

        public byte Type
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
            _type = reader.ReadByte();
            _hue = reader.ReadInt16();
            _font = reader.ReadInt16();
            _name = reader.ReadString(30);
            _text = reader.ReadString();
        }
    }
}
