using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.UILegacy
{
    public class GumplingEventArgs : EventArgs
    {
        public Serial Serial = 0;
        public int Parameter = 0;
        public int ButtonID = 0;

        public GumplingEventArgs(Serial serial, int parameter, int buttonID)
        {
            Serial = serial;
            Parameter = parameter;
            ButtonID = buttonID;
        }
    }
}
