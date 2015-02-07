/***************************************************************************
 *   RecvPacket.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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

namespace UltimaXNA.Network.Packets
{
    public abstract class RecvPacket : IRecvPacket
    {
        readonly int id;
        readonly string name;

        public int Id
        {
            get { return id; }
        }

        public string Name
        {
            get { return name; }
        }

        public RecvPacket(int id, string name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
