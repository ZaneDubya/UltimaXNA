/***************************************************************************
 *   ObjectPropertyListUpdatePacket.cs
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

namespace UltimaXNA.Client.Packets.Server
{
    public class ObjectPropertyListUpdatePacket : RecvPacket
    {
        readonly Serial _serial;
        readonly int _revisionHash;

        public Serial Serial
        {
            get { return _serial; }
        }

        public int RevisionHash 
        {
            get { return _revisionHash; }
        }

        public ObjectPropertyListUpdatePacket(PacketReader reader)
            : base(0xDC, "Object Property List Update")
        {
            _serial = reader.ReadInt32();
            _revisionHash = reader.ReadInt32();
        }
    }
}
