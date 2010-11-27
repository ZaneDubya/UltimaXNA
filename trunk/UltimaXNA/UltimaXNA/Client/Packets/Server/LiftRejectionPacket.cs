/***************************************************************************
 *   LiftRejectionPacket.cs
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

namespace UltimaXNA.Network.Packets.Server
{
    public class LiftRejectionPacket : RecvPacket
    {
        private static string[] _reasons = new string[] {
              "Cannot lift the item.",
              "Out of range.",
              "Out of sight.",
              "Belongs to another.",
              "Already holding something.",
              "???",
            };

        readonly byte _errorCode;

        public byte ErrorCode 
        {
            get { return _errorCode; }
        }

        public string ErrorMessage
        {
            get { return _reasons[_errorCode]; }
        }

        public LiftRejectionPacket(PacketReader reader)
            : base(0x27, "Request Move Item Request")
        {
            _errorCode = reader.ReadByte();
        }
    }
}
