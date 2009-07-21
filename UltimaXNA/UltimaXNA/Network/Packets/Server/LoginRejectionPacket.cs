/***************************************************************************
 *   LoginRejectionPacket.cs
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
    public class LoginRejectionPacket : RecvPacket
    {
        private static Pair<int, string>[] _Reasons = new Pair<int, string>[] {
            new Pair<int, string>(0x00, "Incorrect username and/or password."), 
            new Pair<int, string>(0x01, "Someone is already using this account."),
            new Pair<int, string>(0x02, "Your account has been blocked."),
            new Pair<int, string>(0x03, "Your account credentials are invalid."),
            new Pair<int, string>(0xFE, "Login idle period exceeded."),
            new Pair<int, string>(0xFF, "Communication problem."),
        };

        byte _id;

        public string Reason
        {
            get
            {
                for (int i = 0; i < _Reasons.Length; i++)
                {
                    if (_Reasons[i].ItemA == _id)
                    {
                        return _Reasons[i].ItemB;
                    }
                }
                return (_Reasons[_Reasons.Length - 1].ItemB);
            }
        }

        public LoginRejectionPacket(PacketReader reader)
            : base(0x82, "Login Rejection")
        {
            _id = reader.ReadByte();
        }
    }
}
