using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Network
{
    public class NetworkException : Exception
    {
        public NetworkException(string message)
            : base(message)
        {

        }
    }
}
