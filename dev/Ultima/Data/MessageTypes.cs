/***************************************************************************
 *   MessageTypes.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Ultima.Data
{
    public enum MessageTypes
    {
        Normal = 0x00,
        System = 0x01,
        Emote = 0x02,
        SpeechUnknown = 0x03,
        Information = 0x04,     //SphereServer uses it for overhead information messages
        Label = 0x06,
        Focus = 0x07,
        Whisper = 0x08,
        Yell = 0x09,
        Spell = 0x0A,

        Guild = 0x0D,
        Party = 0x0E, // party ?
        Command = 0x0F,

        EncodedTriggers = 0xC0
    }
}
