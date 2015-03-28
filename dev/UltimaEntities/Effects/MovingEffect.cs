/***************************************************************************
 *   MovingEffect.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Model;
#endregion

namespace UltimaXNA.UltimaEntities.Effects
{
    class MovingEffect : AEffect
    {
        AnimData.AnimDataEntry m_AnimData;
        bool m_Animated;
        int m_ItemID;

        public MovingEffect(Map map,int itemID, int hue)
            : base(map)
        {
            Hue = hue;
            itemID &= 0x3fff;
            m_ItemID = itemID | 0x4000;
            m_Animated = UltimaData.TileData.ItemData[itemID].IsAnimation;
            if (m_Animated)
            {
                m_AnimData = AnimData.GetAnimData(itemID);
                m_Animated = m_AnimData.FrameCount > 0;
            }
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
                    base.SetSource(source);
                    Mobile mobile = source as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && ((xSource | ySource | zSource) != 0))
                    {
                        source.Position.Set(xSource, ySource, zSource);
                    }
                }
                else if (source is Item)
                {
                    base.SetSource(source);
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
                    base.SetSource(target);
                    Mobile mobile = target as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && ((xSource | ySource | zSource) != 0))
                    {
                        mobile.Position.Set(xSource, ySource, zSource);
                    }
                }
                else if (target is Item)
                {
                    base.SetSource(target);
                    Item item = target as Item;
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
        }
        #endregion

        public override void Update(double frameMS)
        {
            base.Update(frameMS);

            int sx, sy, sz, tx, ty, tz;
            GetSource(out sx, out sy, out sz);
            GetTarget(out tx, out ty, out tz);

            // Texture2D texture = ArtData.GetStaticTexture(m_ItemID + ((FramesActive / m_AnimData.FrameInterval) % m_AnimData.FrameCount));

            // m_RenderMode:
            // 2: Alpha = 1.0, Additive.
            // 3: Alpha = 1.5, Additive.
            // 4: Alpha = 0.5, AlphaBlend.

            // draw rotated.
        }

        public override string ToString()
        {
            return string.Format("MovingEffect");
        }
    }
}
