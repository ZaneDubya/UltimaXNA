/***************************************************************************
 *   CreateCharacterPacket.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Network.Packets;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Login.States;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class CreateCharacterPacket : SendPacket
    {
        internal CreateCharacterPacket(CreateCharacterData data, short locationIndex, short slotNumber, int clientIp)
            : base(0x00, "Create Character", 104)
        {
            int str = (byte)MathHelper.Clamp(data.Attributes[0], 10, 60);
            int dex = (byte)MathHelper.Clamp(data.Attributes[1], 10, 60);
            int intel = (byte)MathHelper.Clamp(data.Attributes[2], 10, 60);
                
            if (str + dex + intel != 80)
                throw new Exception("Unable to create character with a combined stat total not equal to 80.");

            Stream.Write(0xedededed);
            Stream.Write(0xffffffff);
            Stream.Write((byte)0);
            Stream.WriteAsciiFixed(data.Name, 30);
            Stream.WriteAsciiFixed(string.Empty, 30);
            Stream.Write((byte)((int)(Genders)data.Gender + (int)(Races)0));
            Stream.Write((byte)str);
            Stream.Write((byte)dex);
            Stream.Write((byte)intel);

            Stream.Write((byte)data.SkillIndexes[0]);
            Stream.Write((byte)data.SkillValues[0]);
            Stream.Write((byte)data.SkillIndexes[1]);
            Stream.Write((byte)data.SkillValues[1]);
            Stream.Write((byte)data.SkillIndexes[2]);
            Stream.Write((byte)data.SkillValues[2]);

            Stream.Write((short)data.SkinHue);
            Stream.Write((short)data.HairStyleID);
            Stream.Write((short)data.HairHue);
            Stream.Write((short)data.FacialHairStyleID);
            Stream.Write((short)data.FacialHairHue);
            Stream.Write((short)locationIndex);
            Stream.Write((short)slotNumber);
            Stream.Write((short)0);

            Stream.Write(clientIp);
            Stream.Write((short)data.ShirtColor);
            Stream.Write((short)data.PantsColor);
        }
    }
}
