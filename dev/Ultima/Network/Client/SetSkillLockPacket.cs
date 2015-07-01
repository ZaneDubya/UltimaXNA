/***************************************************************************
 *   SetSkillLockPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class SetSkillLockPacket : SendPacket
    {
        public SetSkillLockPacket(ushort skillIndex, byte lockType)
            : base(0x3A, "Set skill lock")
        {
            Stream.Write(skillIndex);
            Stream.Write(lockType);
        }
    }
}
