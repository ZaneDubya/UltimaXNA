/***************************************************************************
 *   LightningEffect.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.Entities.Effects
{
    class LightningEffect : AEffect
    {
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
            AEntity source = WorldModel.Entities.GetObject<AEntity>(sourceSerial, false);
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
        }

        public override string ToString()
        {
            return string.Format("LightningEffect");
        }

        protected override AEntityView CreateView()
        {
            return new LightningEffectView(this);
        }
    }
}
