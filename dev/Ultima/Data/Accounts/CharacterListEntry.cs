using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Network;

namespace UltimaXNA.Ultima.Data.Accounts
{
    public class CharacterListEntry
    {
        string name;
        string password;

        public string Name
        {
            get { return name; }
        }

        public string Password
        {
            get { return password; }
        }

        public CharacterListEntry(PacketReader reader)
        {
            name = reader.ReadString(30);
            password = reader.ReadString(30);
        }
    }
}
