/***************************************************************************
 *   LiftRejectionPacket.cs
 *   Copyright (c) 2009 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Core.Network.Packets;
#endregion

namespace UltimaXNA.Ultima.Network.Server
{
    public class LiftRejectionPacket : RecvPacket
    {
        private static string[] m_reasons = {
              "Cannot lift the item.",
              "Out of range.",
              "Out of sight.",
              "Belongs to another.",
              "Already holding something.",
              "???"
            };

        readonly byte m_errorCode;

        public byte ErrorCode 
        {
            get { return m_errorCode; }
        }

        public string ErrorMessage
        {
            get { return m_reasons[m_errorCode]; }
        }

        public LiftRejectionPacket(PacketReader reader)
            : base(0x27, "Request Move Item Request")
        {
            m_errorCode = reader.ReadByte();
        }
    }
}
