using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimaXNA.Ultima.Data
{
    public enum MessageType
    {
        Regular = 0x00,
        System = 0x01,
        Emote = 0x02,
        Label = 0x06,
        Focus = 0x07,
        Whisper = 0x08,
        Yell = 0x09,
        Spell = 0x0A,

        UIld = 0x0D,
        Alliance = 0x0E,
        Command = 0x0F,

        Encoded = 0xC0
    }
}
