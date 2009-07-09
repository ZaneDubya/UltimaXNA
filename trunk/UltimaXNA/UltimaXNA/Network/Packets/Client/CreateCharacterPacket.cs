using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Network.Packets.Client
{
    public enum Sex
    {
        Male = 0,
        Female = 1
    }

    public enum Race
    {
        Human = 1,
        Elf = 2
    }

    public class CreateCharacterPacket : SendPacket
    {
        public CreateCharacterPacket(string name, Sex sex, Race race,
            byte str, byte dex, byte intel, byte skill1, byte skill1Value,
            byte skill2, byte skill2Value, byte skill3, byte skill3Value, short skinColor,
            short hairStyle, short hairColor, short facialHairStyle, short facialHairColor,
            short locationIndex, short slotNumber, int clientIp, short shirtColor, short pantsColor)
            : base(0x00, "Create Character", 104)
        {
            str = (byte)MathHelper.Clamp(str, 10, 45);
            dex = (byte)MathHelper.Clamp(dex, 10, 45);
            intel = (byte)MathHelper.Clamp(intel, 10, 45);

            if (str + dex + intel > 65)
                throw new Exception("Unable to create character with a combined stat total greater than 65");

            Stream.Write(0xedededed);
            Stream.Write(0xffffffff);
            Stream.Write((byte)0);
            Stream.WriteAsciiFixed(name, 30);
            Stream.WriteAsciiFixed("", 30);
            Stream.Write((byte)((int)sex + (int)race));
            Stream.Write((byte)str);
            Stream.Write((byte)dex);
            Stream.Write((byte)intel);

            Stream.Write((byte)skill1);
            Stream.Write((byte)skill1Value);
            Stream.Write((byte)skill2);
            Stream.Write((byte)skill2Value);
            Stream.Write((byte)skill3);
            Stream.Write((byte)skill3Value);

            Stream.Write((short)skinColor);
            Stream.Write((short)hairStyle);
            Stream.Write((short)hairColor);
            Stream.Write((short)facialHairStyle);
            Stream.Write((short)facialHairColor);
            Stream.Write((short)locationIndex);
            Stream.Write((short)slotNumber);
            Stream.Write((short)0);

            Stream.Write(clientIp);
            Stream.Write((short)shirtColor);
            Stream.Write((short)pantsColor);
        }
    }
}
