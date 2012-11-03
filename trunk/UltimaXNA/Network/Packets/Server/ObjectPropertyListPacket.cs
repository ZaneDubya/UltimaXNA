﻿/***************************************************************************
 *   ObjectPropertyListPacket.cs
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
using System.Collections.Generic;
using UltimaXNA.Network;
#endregion

namespace UltimaXNA.Network.Packets.Server
{
    public class ObjectPropertyListPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly int _hash;
        readonly List<int> _clilocs;
        readonly List<string> _arguments;

        public Serial Serial
        {
            get { return _serial; }
        }
        
        public int Hash
        {
            get { return _hash; }
        }
        
        public List<int> CliLocs
        {
            get { return _clilocs; }
        }
        
        public List<string> Arguements
        {
            get { return _arguments; }
        }
        
        public ObjectPropertyListPacket(PacketReader reader)
            : base(0xD6, "Object Property List")
        {
            reader.ReadInt16(); // Always 0x0001
            _serial = reader.ReadInt32();

            reader.ReadInt16(); // Always 0x0000
            _hash = reader.ReadInt32();

            _clilocs = new List<int>();
            _arguments = new List<string>();

            // Loop of all the item/creature's properties to display in the order to display them. The name is always the first entry.
            int clilocId = reader.ReadInt32();

            while (clilocId != 0)
            {
                _clilocs.Add(clilocId);

                int textLength = reader.ReadUInt16();
                string args = string.Empty;

                if (textLength > 0)
                {
                    args = reader.ReadUnicodeStringReverse(textLength / 2);
                }

                _arguments.Add(args);

                clilocId = reader.ReadInt32();
            }
        }
    }
}
