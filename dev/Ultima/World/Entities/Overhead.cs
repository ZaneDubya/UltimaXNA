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

            m_TimePersist = 5000;
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

        // ============================================================
        // View management
        // ============================================================

        protected override EntityViews.AEntityView CreateView()
        {
            return new EntityViews.OverheadView(this);
        }
    }
}
