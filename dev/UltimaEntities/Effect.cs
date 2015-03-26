/***************************************************************************
 *   Effect.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.UltimaData;
using UltimaXNA.UltimaPackets.Server;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.Model;
using UltimaXNA.Core.Diagnostics;
#endregion

namespace UltimaXNA.UltimaEntities
{
    public class Effect : AEntity
    {
        private bool m_isHued = false;
        private bool m_isInitialized = false;

        private int m_baseItemID;
        public int ItemID { get { return m_baseItemID; } }

        private float m_frameSequence;
        private int m_frameLength;
        private bool m_useGumpArtInsteadOfTileArt = false;

        private float m_timeCurrentSeconds;
        private float m_timeEndTotalSeconds;
        private float m_timeRepeatAnimationSeconds;

        private GraphicEffectType m_effectType;
        private int m_hue;
        private int m_duration;
        private int m_speed;
        private bool m_doesExplode;
        private GraphicEffectBlendMode m_bendMode;
        private int m_targetX, m_targetY, m_targetZ;

        public Effect(Serial serial, Map map)
            : base(serial, map)
        {

        }

        public override void Update(double frameMS)
        {
            if (!m_isInitialized)
            {
                m_timeEndTotalSeconds = m_timeCurrentSeconds = 0;
                m_frameSequence = 0;
                m_isInitialized = true;

                ParticleData itemData;
                switch (m_effectType)
                {
                    case GraphicEffectType.Nothing:
                        this.Dispose();
                        break;
                    case GraphicEffectType.Moving:
                        this.Dispose();
                        break;
                    case GraphicEffectType.Lightning:
                        m_useGumpArtInsteadOfTileArt = true;
                        m_baseItemID = 20000;
                        m_frameLength = 10;
                        m_timeRepeatAnimationSeconds = 0.5f;
                        m_timeEndTotalSeconds += m_timeRepeatAnimationSeconds;
                        break;
                    case GraphicEffectType.FixedXYZ:
                    case GraphicEffectType.FixedFrom:
                        itemData = ParticleData.GetData(m_baseItemID);
                        if (itemData != null)
                        {
                            // we may need to remap baseItemID from the original value:
                            m_baseItemID = itemData.ItemID;
                            m_frameLength = itemData.FrameLength;
                            m_timeRepeatAnimationSeconds = m_duration / (float)(10 + m_speed);
                            m_timeEndTotalSeconds += m_timeRepeatAnimationSeconds;
                        }
                        else
                            this.Dispose();
                        break;
                    case GraphicEffectType.ScreenFade:
                        Logger.Warn("Unhandled ScreenFade effect.");
                        this.Dispose();
                        break;
                }
            }
            else
            {
                m_timeCurrentSeconds += (float)frameMS;
                m_frameSequence = (float)(m_timeCurrentSeconds / m_timeRepeatAnimationSeconds) % 1.0f;
            }

            if (m_timeCurrentSeconds >= m_timeEndTotalSeconds)
            {
                if (m_doesExplode)
                {
                    // Effect effect = EntityManager.AddDynamicObject();
                    // effect.Load_AsExplosion(m_targetX, m_targetY, m_targetZ);
                }
                this.Dispose();
            }

            base.Update(frameMS);
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            int hue = m_isHued ? m_hue : 0;
            // tile.AddMapObject(new MapObjectDynamic(this, position, m_baseItemID, (int)(m_frameSequence * m_frameLength), hue, m_useGumpArtInsteadOfTileArt));
        }

        public void Load_FromPacket(GraphicEffectPacket packet)
        {
            m_baseItemID = packet.BaseItemID;
            m_effectType = packet.EffectType;
            m_isHued = false;
            m_hue = 0;
            Position.Set(packet.SourceX, packet.SourceY, packet.SourceZ);
            m_targetX = packet.TargetX;
            m_targetY = packet.TargetY;
            m_targetZ = packet.TargetZ;
            m_duration = packet.Duration;
            m_speed = packet.Speed;
            m_doesExplode = packet.DoesExplode;
        }

        public void Load_FromPacket(GraphicEffectHuedPacket packet)
        {
            this.Load_FromPacket((GraphicEffectPacket)packet);
            m_isHued = true;
            m_hue = packet.Hue;
            m_bendMode = packet.BlendMode;
        }

        public void Load_AsExplosion(int x, int y, int z)
        {
            m_effectType = GraphicEffectType.FixedXYZ;
            m_isHued = false;
            m_hue = 0;
            Position.Set(x, y, z);
            ParticleData itemData = ParticleData.RandomExplosion;
            m_baseItemID = itemData.ItemID;
            m_duration = itemData.FrameLength;
            m_speed = 0;
            m_doesExplode = false;
        }
    }
}
