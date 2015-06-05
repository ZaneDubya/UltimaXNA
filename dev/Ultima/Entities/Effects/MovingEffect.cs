/***************************************************************************
 *   MovingEffect.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.Entities.Mobiles;
using UltimaXNA.Ultima.World;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.Entities.Effects
{
    public class MovingEffect : AEffect
    {
        public float AngleToTarget = 0f;

        int m_ItemID;
        public int ItemID
        {
            get { return m_ItemID; }
        }

        public MovingEffect(Map map,int itemID, int hue)
            : base(map)
        {
            Hue = hue;
            itemID &= 0x3fff;
            m_ItemID = itemID | 0x4000;
        }

        #region Constructors
        public MovingEffect(Map map,AEntity Source, AEntity Target, int itemID, int hue)
            : this(map, itemID, hue)
        {
            base.SetSource(Source);
            base.SetTarget(Target);
        }

        public MovingEffect(Map map,AEntity Source, int xTarget, int yTarget, int zTarget, int itemID, int hue)
            : this(map, itemID, hue)
        {
            base.SetSource(Source);
            base.SetTarget(xTarget, yTarget, zTarget);
        }

        public MovingEffect(Map map,int xSource, int ySource, int zSource, AEntity Target, int itemID, int hue)
            : this(map, itemID, hue)
        {
            base.SetSource(xSource, ySource, zSource);
            base.SetTarget(Target);
        }

        public MovingEffect(Map map,int xSource, int ySource, int zSource, int xTarget, int yTarget, int zTarget, int itemID, int hue)
            : this(map, itemID, hue)
        {
            base.SetSource(xSource, ySource, zSource);
            base.SetTarget(xTarget, yTarget, zTarget);
        }

        public MovingEffect(Map map,int sourceSerial, int targetSerial, int xSource, int ySource, int zSource, int xTarget, int yTarget, int zTarget, int itemID, int hue)
            : this(map, itemID, hue)
        {
            AEntity source = EntityManager.GetObject<AEntity>(sourceSerial, false);
            if (source != null)
            {
                if (source is Mobile)
                {
                    base.SetSource(source.X, source.Y, source.Z);
                    Mobile mobile = source as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && ((xSource | ySource | zSource) != 0))
                    {
                        source.Position.Set(xSource, ySource, zSource);
                    }
                }
                else if (source is Item)
                {
                    base.SetSource(source.X, source.Y, source.Z);
                    Item item = source as Item;
                    if ((xSource | ySource | zSource) != 0)
                    {
                        item.Position.Set(xSource, ySource, zSource);
                    }
                }
                else
                {
                    base.SetSource(xSource, ySource, zSource);
                }
            }
            else
            {
                base.SetSource(xSource, ySource, zSource);
            }
            AEntity target = EntityManager.GetObject<AEntity>(targetSerial, false);
            if (target != null)
            {
                if (target is Mobile)
                {
                    base.SetTarget(target);
                    Mobile mobile = target as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && ((xTarget | yTarget | zTarget) != 0))
                    {
                        mobile.Position.Set(xTarget, yTarget, zTarget);
                    }
                }
                else if (target is Item)
                {
                    base.SetTarget(target);
                    Item item = target as Item;
                    if ((xTarget | yTarget | zTarget) != 0)
                    {
                        item.Position.Set(xTarget, yTarget, zTarget);
                    }
                }
                else
                {
                    base.SetSource(xTarget, yTarget, zTarget);
                }
            }
        }
        #endregion

        public override void Update(double frameMS)
        {
            base.Update(frameMS);

            int sx, sy, sz, tx, ty, tz;
            GetSource(out sx, out sy, out sz);
            GetTarget(out tx, out ty, out tz);

            if (m_TimeUntilHit == 0f)
            {
                m_TimeActive = 0f;
                m_TimeUntilHit = (float)System.Math.Sqrt(System.Math.Pow((tx - sx), 2) + System.Math.Pow((ty - sy), 2) + System.Math.Pow((tz - sz), 2)) * 75f;
            }
            else
            {
                m_TimeActive += (float)frameMS;
            }

            if (m_TimeActive >= m_TimeUntilHit)
            {
                Dispose();
                return;
            }
            else
            {
                float x, y, z;
                x = (sx + (m_TimeActive / m_TimeUntilHit) * (float)(tx - sx));
                y = (sy + (m_TimeActive / m_TimeUntilHit) * (float)(ty - sy));
                z = (sz + (m_TimeActive / m_TimeUntilHit) * (float)(tz - sz));
                Position.Set((int)x, (int)y, (int)z);
                Position.Offset = new Vector3(x % 1, y % 1, z % 1);
                AngleToTarget = -((float)System.Math.Atan2((ty - sy), (tx - sx)) + (float)(System.Math.PI) * (1f / 4f)); // In radians
            }

            // m_RenderMode:
            // 2: Alpha = 1.0, Additive.
            // 3: Alpha = 1.5, Additive.
            // 4: Alpha = 0.5, AlphaBlend.

            // draw rotated.
            
        }

        private float m_TimeActive = 0f;
        private float m_TimeUntilHit = 0f;

        protected override EntityViews.AEntityView CreateView()
        {
            return new EntityViews.MovingEffectView(this);
        }

        public override string ToString()
        {
            return string.Format("MovingEffect");
        }
    }
}
