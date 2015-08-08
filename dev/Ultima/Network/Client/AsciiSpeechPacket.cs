/***************************************************************************
 *   AsciiSpeechPacket.cs
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
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.Resources;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    // from http://docs.polserver.com/packets/index.php?Packet=0xAD
    public enum AsciiSpeechPacketTypes
    {
        Normal = 0x00,
        System = 0x01,
        Emote = 0x02,
        Message = 0x06,
        MessageWithName = 0x07,
        Whisper = 0x08,
        Yell = 0x09,
        Spell = 0x0A,
        Guild = 0x0D,
        Alliance = 0x0E,
        Command = 0x0F,
    }

    public class AsciiSpeechPacket : SendPacket
    {
        const byte HasTriggers = 0xC0;

        public AsciiSpeechPacket(AsciiSpeechPacketTypes type, int color, int font, string lang, string text)
            : base(0xAD, "Ascii Speech")
        {
            // get triggers
            int triggerCount; int[] triggers;
            SpeechData.GetSpeechTriggers(text, lang, out triggerCount, out triggers);
            if (triggerCount > 0)
                type = (AsciiSpeechPacketTypes)((byte)type | HasTriggers);

            Stream.Write((byte)type);
            Stream.Write((short)color);
            Stream.Write((short)font);
            Stream.WriteAsciiNull(lang);
            if (triggerCount > 0)
            {
                byte[] t = new byte[(int)Math.Ceiling((triggerCount + 1) * 1.5f)];
                // write 12 bits at a time. first write count: byte then half byte.
                t[0] = (byte)((triggerCount & 0x0FF0) >> 4);
                t[1] = (byte)((triggerCount & 0x000F) << 4);
                for (int i = 0; i < triggerCount; i++)
                {
                    int index = (int)((i + 1) * 1.5f);
                    if (i % 2 == 0) // write half byte and then byte
                    {
                        t[index + 0] |= (byte)((triggers[i] & 0x0F00) >> 8);
                        t[index + 1] = (byte)(triggers[i] & 0x00FF);
                    }
                    else // write byte and then half byte
                    {
                        t[index] = (byte)((triggers[i] & 0x0FF0) >> 4);
                        t[index + 1] = (byte)((triggers[i] & 0x000F) << 4);
                    }
                }
                Stream.BaseStream.Write(t, 0, t.Length);
                Stream.WriteAsciiNull(text);
            }
            else
            {
                Stream.WriteBigUniNull(text);
            }
        }

        List<int> getSpeechTriggers(string text)
        {
            List<int> triggers = new List<int>();

            return triggers;
        }
    }
}
