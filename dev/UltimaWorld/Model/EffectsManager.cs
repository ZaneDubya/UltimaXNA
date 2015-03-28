using System.Collections.Generic;
using UltimaXNA.UltimaEntities.Effects;
using UltimaXNA.UltimaPackets.Server;
using UltimaXNA.UltimaWorld.Model;

namespace UltimaXNA.UltimaWorld
{
    class EffectsManager
    {
        static WorldModel m_Model;
        static List<AEffect> m_Effects;

        public EffectsManager(WorldModel model)
        {
            m_Model = model;
            m_Effects = new List<AEffect>();
        }

        public static void Add(GraphicEffectPacket packet)
        {
            bool hasHueData = (packet as GraphicEffectHuedPacket != null);
            bool hasParticles = (packet as GraphicEffectExtendedPacket != null); // we don't yet handle these.
            if (hasParticles)
            {
                Core.Diagnostics.Logger.Warn("Unhandled particles in an effects packet.");
            }

            AEffect effect = null;
            int hue = hasHueData ? ((GraphicEffectHuedPacket)packet).Hue : 0;
            int blend = hasHueData ? (int)((GraphicEffectHuedPacket)packet).BlendMode : 0;

            switch (packet.EffectType)
            {
                case GraphicEffectType.Moving:
                    if (packet.ItemID <= 0)
                        return;
                    effect = new MovingEffect(m_Model.Map, packet.SourceSerial, packet.TargetSerial,
                        packet.SourceX, packet.SourceY, packet.SourceZ,
                        packet.TargetX, packet.TargetY, packet.TargetZ, 
                        packet.ItemID, hue);
                    effect.BlendMode = blend;
                    if (packet.DoesExplode)
                    {
                        effect.Children.Add(new AnimatedItemEffect(m_Model.Map, packet.TargetSerial, 
                            packet.TargetX, packet.TargetY, packet.TargetZ,
                            0x36cb, hue, 9));
                    }
                    break;
                case GraphicEffectType.Lightning:
                    effect = new LightningEffect(m_Model.Map, packet.SourceSerial, 
                        packet.SourceX, packet.SourceY, packet.SourceZ, hue);
                    break;
                case GraphicEffectType.FixedXYZ:
                    if (packet.ItemID <= 0)
                        return;
                    effect = new AnimatedItemEffect(m_Model.Map, 
                        packet.SourceX, packet.SourceY, packet.SourceZ,
                        packet.ItemID, hue, packet.Duration);
                    effect.BlendMode = blend;
                    break;
                case GraphicEffectType.FixedFrom:
                    if (packet.ItemID <= 0)
                        return;
                    effect = new AnimatedItemEffect(m_Model.Map, packet.SourceSerial, 
                        packet.SourceX, packet.SourceY, packet.SourceZ,
                        packet.ItemID, hue, packet.Duration);
                    effect.BlendMode = blend;
                    break;
                case GraphicEffectType.ScreenFade:
                    Core.Diagnostics.Logger.Warn("Unhandled screen fade effect.");
                    break;
                default:
                    Core.Diagnostics.Logger.Warn("Unhandled effect.");
                    return;
            }

            if (effect != null)
                Add(effect);
        }

        public static void Add(AEffect e)
        {
            m_Effects.Add(e);
        }

        public void Update(double frameMS)
        {
            for (int i = 0; i < m_Effects.Count; i++)
            {
                AEffect effect = m_Effects[i];
                effect.Update(frameMS);
                if (effect.IsDisposed)
                {
                    m_Effects.RemoveAt(i);
                    i--;
                    if (effect.ChildrenCount > 0)
                    {
                        for (int j = 0; j < effect.Children.Count; j++)
                        {
                            m_Effects.Add(effect.Children[j]);
                        }
                    }
                }
            }
        }
    }
}
