/***************************************************************************
 *   PacketHandler.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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

namespace UltimaXNA.Network
{
    public delegate void PacketReceiveHandler(PacketReader reader);
    public delegate void TypedPacketReceiveHandler(IRecvPacket packet);

    public class PacketHandler
    {
        int id;
        int length;
        string name;
        PacketReceiveHandler handler;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public PacketReceiveHandler Handler
        {
            get { return handler; }
            set { handler = value; }
        }

        public PacketHandler(int id, string name, int length, PacketReceiveHandler handler)
        {
            this.id = id;
            this.name = name;
            this.length = length;
            this.handler = handler;
        }
    }

    public class TypedPacketHandler : PacketHandler
    {
        Type type;
        TypedPacketReceiveHandler handler;

        public Type Type
        {
            get { return type; }
        }

        public TypedPacketReceiveHandler TypeHandler
        {
            get { return handler; }
            set { handler = value; }
        }

        private new PacketReceiveHandler Handler
        {
            get { return null; }
            set { base.Handler = value; }
        }

        public TypedPacketHandler(int id, string name, Type type, int length, TypedPacketReceiveHandler handler)
            : base(id, name, length, null)
        {
            this.type = type;
            this.handler = handler;
        }

        public IRecvPacket CreatePacket(PacketReader reader)
        {
                return (IRecvPacket)Activator.CreateInstance(type, new object[] { reader });
        }
    }
}
