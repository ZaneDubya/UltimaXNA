/***************************************************************************
 *   GumplingEventArgs.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;

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
