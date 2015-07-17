/***************************************************************************
 *   AnimatedItemEfect.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.Entities.Effects
{
    public class AnimatedItemEffect : AEffect
    {
        public int ItemID;
        public int Duration;

        public AnimatedItemEffect(Map map, int itemID, int hue, int duration)
            : base(map)
        {
            ItemID = (itemID & 0x3fff) | 0x4000;
            Hue = hue;
            Duration = duration;
        }

        public AnimatedItemEffect(Map map, AEntity Source, int ItemID, int Hue, int duration)
            : this(map, ItemID, Hue, duration)
        {
            SetSource(Source);
        }

        public AnimatedItemEffect(Map map, int Source, int ItemID, int Hue, int duration)
            : this(map, Source, 0, 0, 0, ItemID, Hue, duration)
        {

        }

        public AnimatedItemEffect(Map map, int xSource, int ySource, int zSource, int ItemID, int Hue, int duration)
            : this(map, ItemID, Hue, duration)
        {
            SetSource(xSource, ySource, zSource);
        }

        public AnimatedItemEffect(Map map, int sourceSerial, int xSource, int ySource, int zSource, int ItemID, int Hue, int duration)
            : this(map, ItemID, Hue, duration)
        {
            AEntity source = WorldModel.Entities.GetObject<AEntity>(sourceSerial, false);
            if (source != null)
            {
                if (source is Mobile)
                {
                    Mobile mobile = source as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && (((xSource != 0) || (ySource != 0)) || (zSource != 0)))
                    {
                        mobile.Position.Set(xSource, ySource, zSource);
                    }
                    SetSource(mobile);
                }
                else if (source is Item)
                {
                    Item item = source as Item;
                    if (((xSource != 0) || (ySource != 0)) || (zSource != 0))
                    {
                        item.Position.Set(xSource, ySource, zSource);
                    }
                    SetSource(item);
                }
                else
                {
                    SetSource(xSource, ySource, zSource);
                }
            }
            else
            {
                SetSource(xSource, ySource, zSource);
            }
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            if (FramesActive >= Duration)
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

        protected override AEntityView CreateView()
        {
            return new AnimatedItemEffectView(this);
        }

        public override string ToString()
        {
            return string.Format("AnimatedItemEffect");
        }
    }
}
