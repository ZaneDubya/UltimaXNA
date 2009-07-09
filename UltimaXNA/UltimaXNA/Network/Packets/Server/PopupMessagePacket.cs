using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network.Packets.Server
{
    public class PopupMessagePacket : RecvPacket
    {
        public static string[] Messages = new string[] {
                "Character does not exist",
		        "Character already exists",
		        "A character is already logged in",
		        "Loggin sync error",
		        "You have been idle for to long"
            };

        readonly byte _id;

        public string Message
        {
            get
            {
                switch (_id)
                {
                    case 1:
                        return Messages[0];
                    case 2:
                        return Messages[1];
                    case 5:
                        return Messages[2];
                    case 6:
                        return Messages[3];
                    case 7:
                        return Messages[4];
                    default:
                        return "Error: No message defined!";
                }
            }
        }

        public PopupMessagePacket(PacketReader reader)
            : base(0x53, "Popup Message")
        {
            _id = reader.ReadByte();
        }
    }
}
