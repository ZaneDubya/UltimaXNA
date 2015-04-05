/***************************************************************************
 *   RecvPacket.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network;
#endregion

namespace UltimaXNA.Core.Network.Packets
{
    public abstract class RecvPacket : IRecvPacket
    {
        readonly int m_Id;
        readonly string m_Name;

        public int Id
        {
            get { return m_Id; }
        }

        public string Name
        {
            get { return m_Name; }
        }

        public RecvPacket(int id, string name)
        {
            m_Id = id;
            m_Name = name;
        }
    }
}
