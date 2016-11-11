/***************************************************************************
 *   Overhead.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.World.EntityViews;
#endregion

namespace UltimaXNA.Ultima.World.Entities
{
    public class Overhead : AEntity
    {
        public AEntity Parent
        {
            get;
            private set;
        }

        public MessageTypes MessageType
        {
            get;
            private set;
        }

        public string Text
        {
            get;
            private set;
        }

        private int m_TimePersist = 0;

        public Overhead(AEntity parent, MessageTypes msgType, string text)
            : base(parent.Serial, parent.Map)
        {
            Parent = parent;
            MessageType = msgType;
            Text = text;

            string plainText = text.Substring(text.IndexOf('>') + 1);
           
            // Every speech message lasts at least 2.5s, and increases by 100ms for every char, to a max of 10s
            m_TimePersist = 2500 + (plainText.Length * 100);
            if (m_TimePersist > 10000)
                m_TimePersist = 10000;
        }

        public void ResetTimer()
        {
            m_TimePersist = 5000;
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            m_TimePersist -= (int)frameMS;
            if (m_TimePersist <= 0)
                Dispose();
        }

        // ============================================================================================================
        // View management
        // ============================================================================================================

        protected override AEntityView CreateView()
        {
            return new OverheadView(this);
        }
    }
}
