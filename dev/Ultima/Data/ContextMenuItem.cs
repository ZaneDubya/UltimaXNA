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
using UltimaXNA.Core.Resources;
#endregion

namespace UltimaXNA.Ultima.Data
{
    public class ContextMenuItem
    {
        private readonly string m_Caption;
        private readonly int m_ResponseCode;

        public ContextMenuItem(int responseCode, int stringID, int flags, int hue)
        {
            // get the resource provider
            IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
            m_Caption = provider.GetString(stringID);
            m_ResponseCode = responseCode;
        }

        public int ResponseCode
        {
            get { return m_ResponseCode; }
        }

        public string Caption
        {
            get { return m_Caption; }
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}]", m_Caption, m_ResponseCode);
        }
    }
}
