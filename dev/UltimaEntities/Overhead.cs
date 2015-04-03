/***************************************************************************
 *   Overhead.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Maps;
using UltimaXNA.UltimaGUI;
#endregion

namespace UltimaXNA.UltimaEntities
{
    public class Overhead : AEntity
    {
        public AEntity Parent
        {
            get;
            private set;
        }

        public MessageType MessageType
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

        public Overhead(AEntity parent, MessageType msgType, string text)
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
                this.Dispose();
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
