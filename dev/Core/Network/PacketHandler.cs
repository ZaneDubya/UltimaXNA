/***************************************************************************
 *   PacketHandler.cs
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
using System.Linq;
using System.Text;
#endregion

namespace UltimaXNA.Core.Network
{
    public delegate void PacketReceiveHandler(PacketReader reader);
    public delegate void TypedPacketReceiveHandler(IRecvPacket packet);

    public class PacketHandler
    {
        int m_ID;
        int m_Length;
        string m_Name;
        PacketReceiveHandler m_Handler;

        public int Id
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public int Length
        {
            get { return m_Length; }
            set { m_Length = value; }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public PacketReceiveHandler Handler
        {
            get { return m_Handler; }
            set { m_Handler = value; }
        }

        public PacketHandler(int id, string name, int length, PacketReceiveHandler handler)
        {
            m_ID = id;
            m_Name = name;
            m_Length = length;
            m_Handler = handler;
        }
    }

    public class TypedPacketHandler : PacketHandler
    {
        Type m_Type;
        TypedPacketReceiveHandler m_Handler;

        public Type Type
        {
            get { return m_Type; }
        }

        public TypedPacketReceiveHandler TypeHandler
        {
            get { return m_Handler; }
            set { m_Handler = value; }
        }

        private new PacketReceiveHandler Handler
        {
            get { return base.Handler; }
            set { base.Handler = value; }
        }

        public TypedPacketHandler(int id, string name, Type type, int length, TypedPacketReceiveHandler handler)
            : base(id, name, length, null)
        {
            m_Type = type;
            m_Handler = handler;
        }

        public IRecvPacket CreatePacket(PacketReader reader)
        {
                return (IRecvPacket)Activator.CreateInstance(m_Type, new object[] { reader });
        }
    }
}
