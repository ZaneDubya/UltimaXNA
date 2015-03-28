/***************************************************************************
 *   LightningEffect.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.UltimaEntities;
using UltimaXNA.UltimaWorld.Model;
using UltimaXNA.UltimaWorld;
#endregion

namespace UltimaXNA.UltimaEntities.Effects
{
    class LightningEffect : AEffect
    {
        protected Texture m_tCache;

        private int m_Frame = 0;

        public LightningEffect(Map map, int hue)
            : base (map)
        {
            Hue = hue;
        }

        public LightningEffect(Map map, AEntity Source, int hue)
            : this(map, hue)
        {
            SetSource(Source);
        }

        public LightningEffect(Map map, int xSource, int ySource, int zSource, int hue)
            : this(map, hue)
        {
            SetSource(xSource, ySource, zSource);
        }

        public LightningEffect(Map map, int sourceSerial, int xSource, int ySource, int zSource, int hue)
            : this(map, hue)
        {
            AEntity source = EntityManager.GetObject<AEntity>(sourceSerial, false);
            if (source != null)
            {
                SetSource(source);
            }
            else
            {
                SetSource(xSource, ySource, zSource);
            }
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            if (FramesActive >= 10)
            {
                Dispose();
            }
            else
            {
                int x, y, z;
                GetSource(out x, out y, out z);
                Position.Set(x, y, z);
            }

            Texture lightningTexture = UltimaData.GumpData.GetGumpXNA(0x4e20 + m_Frame++);
            // draw this.m_vCache.DrawGame(gump, num10 - (gump.Width / 2), num11 - gump.Height);
        }

        public override string ToString()
        {
            return string.Format("LightningEffect");
        }
    }
}
