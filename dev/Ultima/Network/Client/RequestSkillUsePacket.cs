/***************************************************************************
 *   RequestSkillUsePacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Client
{
    public class RequestSkillUsePacket : SendPacket
    {
        public RequestSkillUsePacket(int id)
            : base(0x12, "Request Skill Use")
        {
            Stream.Write((byte)0x24);
            Stream.WriteAsciiNull(String.Format("{0} 0", id));
        }
    }
}
