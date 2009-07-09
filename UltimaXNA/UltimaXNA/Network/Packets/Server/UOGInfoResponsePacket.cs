using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class UOGInfoResponsePacket : RecvPacket
    {
        readonly string _name;
        readonly int _age;
        readonly int _clientCount;
        readonly int _itemCount;
        readonly int _mobileCount;
        readonly string _memory;

        public int Age
        {
            get { return _age; }
        }

        public int ClientCount
        {
            get { return _clientCount; }
        }

        public int ItemCount
        {
            get { return _itemCount; }
        }

        public int MobileCount
        {
            get { return _mobileCount; }
        }

        public string ServerName
        {
            get { return _name; }
        }

        public string Memory
        {
            get { return _memory; }
        }

        public UOGInfoResponsePacket(PacketReader reader)
            : base(0x52, "UOG Information Response")
        {
            string response = reader.ReadString();
            string[] parts = response.Split(',');

            for (int j = 0; j < parts.Length; j++)
            {
                string[] keyValue = parts[j].Split('=');

                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();

                    switch (key)
                    {
                        case "Name":
                            _name = value;
                            break;
                        case "Age":
                            _age = int.Parse(value);
                            break;
                        case "Clients":
                            _clientCount = int.Parse(value) - 1;
                            break;
                        case "Items":
                            _itemCount = int.Parse(value);
                            break;
                        case "Chars":
                            _mobileCount = int.Parse(value);
                            break;
                        case "Mem":
                            _memory = value;
                            break;
                    }
                }
            }
        }
    }
}
