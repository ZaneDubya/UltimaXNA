/***************************************************************************
 *   MapObjectMobile.cs
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
#endregion

namespace UltimaXNA.UltimaWorld.View
{
    public class MapObjectMobile : AMapObject
    {
        public int Action { get; set; }
        public int Facing { get; set; }

        private float m_CurrentFrame;
        private int m_layerCount = 0;
        private int m_FrameCount = 0;
        private MapObjectMobileLayer[] m_MobileLayers;

        public MapObjectMobile(Position3D position, int facing, int action, float frame, BaseEntity ownerEntity)
            : base(position)
        {
            if (frame >= 1.0f)
                return;

            m_MobileLayers = new MapObjectMobileLayer[(int)EquipLayer.LastUserValid];

            Facing = facing;
            Action = action;
            m_CurrentFrame = frame;
            OwnerEntity = ownerEntity;

            // set pick type
            m_pickType = PickTypes.PickObjects;

            // set up draw data
            m_draw_flip = (Facing > 4) ? true : false;
            m_draw_IsometricOverlap = true;
        }

        public void AddLayer(int bodyID, int hue)
        {
            m_MobileLayers[m_layerCount++] = new MapObjectMobileLayer(bodyID, hue, InternalGetFrame(bodyID, hue, Facing, Action, m_CurrentFrame));
            m_FrameCount = UltimaData.AnimationData.GetAnimationFrameCount(bodyID, Action, Facing, hue);
        }

        private int m_mobile_drawCenterX, m_mobile_drawCenterY;

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            m_draw_texture = m_MobileLayers[0].Frame.Texture;
            m_mobile_drawCenterX = m_MobileLayers[0].Frame.Center.X;
            m_mobile_drawCenterY = m_MobileLayers[0].Frame.Center.Y;
            if (m_draw_flip)
            {
                m_draw_X = m_mobile_drawCenterX - 22 + (int)((Position.X_offset - Position.Y_offset) * 22);
                m_draw_Y = m_mobile_drawCenterY + (int)((Position.Z_offset + Z) * 4) + m_draw_height - 22 - (int)((Position.X_offset + Position.Y_offset) * 22);
            }
            else
            {
                m_draw_X = m_mobile_drawCenterX - 22 - (int)((Position.X_offset - Position.Y_offset) * 22);
                m_draw_Y = m_mobile_drawCenterY + (int)((Position.Z_offset + Z) * 4) + m_draw_height - 22 - (int)((Position.X_offset + Position.Y_offset) * 22);
            }

            if (UltimaVars.EngineVars.LastTarget != null && UltimaVars.EngineVars.LastTarget == OwnerSerial)
                m_draw_hue = new Vector2(((Mobile)OwnerEntity).NotorietyHue - 1, 1);

            for (int i = 0; i < m_layerCount; i++)
            {
                if (m_MobileLayers[i].Frame != null)
                {
                    float x = (!m_draw_flip) ?
                        drawPosition.X + m_mobile_drawCenterX - (m_draw_X + m_MobileLayers[i].Frame.Center.X) :
                        drawPosition.X + 44 - m_mobile_drawCenterX + (m_draw_X + m_MobileLayers[i].Frame.Center.X);
                    float y = drawPosition.Y - m_draw_Y - (m_MobileLayers[i].Frame.Texture.Height + m_MobileLayers[i].Frame.Center.Y) + m_mobile_drawCenterY;

                    Rectangle dest = new Rectangle(
                        (int)x, (int)y,
                        (!m_draw_flip) ? m_MobileLayers[i].Frame.Texture.Width : -m_MobileLayers[i].Frame.Texture.Width, 
                        m_MobileLayers[i].Frame.Texture.Height);

                    sb.DrawSimple(m_MobileLayers[i].Frame.Texture, dest, Utility.GetHueVector(m_MobileLayers[i].Hue));
                }
            }

            return true;
        }

        private UltimaData.AnimationFrame InternalGetFrame(int bodyID, int hue, int facing, int action, float frame)
        {
            UltimaData.AnimationFrame[] iFrames = UltimaData.AnimationData.GetAnimation(bodyID, action, facing, hue);
            if (iFrames == null)
                return null;
            int iFrame = InternalGetFrameFromSequence(frame, iFrames.Length);
            if (iFrames[iFrame].Texture == null)
                return null;
            return iFrames[iFrame];
        }

        private int InternalGetFrameFromSequence(float frame, int maxFrames)
        {
            return (int)(frame * (float)maxFrames);
        }

        public override string ToString()
        {
            return string.Format("MapObjectMobile\n   Facing:{1}\n{0}", base.ToString(), Facing);
        }
    }

    struct MapObjectMobileLayer
    {
        public int Hue;
        public UltimaData.AnimationFrame Frame;
        public int BodyID;

        public MapObjectMobileLayer(int bodyID, int hue, UltimaData.AnimationFrame frame)
        {
            BodyID = bodyID;
            Hue = hue;
            Frame = frame;
        }
    }
}