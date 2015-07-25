/***************************************************************************
 *   ContextMenu.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region usings
using UltimaXNA.Ultima.IO;
#endregion

namespace UltimaXNA.Ultima.Data
{
    public class ContextMenuItem
    {
        private readonly string m_caption;
        private readonly int m_responseCode;

        public ContextMenuItem(int nResponseCode, int iStringID, int iFlags, int iHue)
        {
            m_caption = StringData.Entry(iStringID);
            m_responseCode = nResponseCode;
        }

        public int ResponseCode
        {
            get { return m_responseCode; }
        }

        public string Caption
        {
            get { return m_caption; }
        }
    }
}
