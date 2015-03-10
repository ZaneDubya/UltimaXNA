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
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;
#endregion

namespace UltimaXNA.UltimaWorld.View
{
    public class MapObjectMobile : AMapObject
    {
        public int Action { get; set; }
        public int Facing { get; set; }

        private float m_frame;
        private int m_layerCount = 0;
        private int m_frameCount = 0;
        private MapObjectMobileLayer[] m_layers;

        public MapObjectMobile(Position3D position, int facing, int action, float frame, BaseEntity ownerEntity)
            : base(position)
        {
            if (frame >= 1.0f)
                return;

            m_layers = new MapObjectMobileLayer[(int)EquipLayer.LastUserValid];

            Facing = facing;
            Action = action;
            m_frame = frame;
            OwnerEntity = ownerEntity;

            // set pick type
            m_pickType = PickTypes.PickObjects;

            // set up draw data
            m_draw_flip = (Facing > 4) ? true : false;
            m_draw_IsometricOverlap = true;
        }

        public void AddLayer(int bodyID, int hue)
        {
            m_layers[m_layerCount++] = new MapObjectMobileLayer(bodyID, hue, getFrame(bodyID, hue, Facing, Action, m_frame));
            m_frameCount = UltimaData.AnimationData.GetAnimationFrameCount(bodyID, Action, Facing, hue);
        }

        private long createHashFromLayers()
        {
            int[] hashArray = new int[m_layerCount * 2 + 3];
            hashArray[0] = (int)Action;
            hashArray[1] = Facing;
            hashArray[2] = frameFromSequence(m_frame, m_frameCount);
            for (int i = 0; i < m_layerCount; i++)
            {
                hashArray[3 + i * 2] = m_layers[i].BodyID;
                hashArray[4 + i * 2] = m_layers[i].Hue;
            }

            long hash = 0;
            for (int i = 0; i < hashArray.Length; i++)
            {
                hash = unchecked(hash * 31 + hashArray[i]);
                // hash ^= hashArray[i];
            }
            return hash;
        }

        private int m_mobile_drawCenterX, m_mobile_drawCenterY;
        protected void Prerender(SpriteBatch3D sb)
        {
            if (m_layerCount == 1)
            {
                m_draw_hue = Utility.GetHueVector(m_layers[0].Hue);
                m_draw_texture = m_layers[0].Frame.Texture;
                m_mobile_drawCenterX = m_layers[0].Frame.Center.X;
                m_mobile_drawCenterY = m_layers[0].Frame.Center.Y;
            }
            else
            {
                m_draw_hue = Utility.GetHueVector(0);
                long hash = createHashFromLayers();
                // m_draw_texture = MapObjectPrerendered.RestorePrerenderedTexture(hash, out m_mobile_drawCenterX, out m_mobile_drawCenterY);
                if (m_draw_texture == null)
                {
                    int minX = 0, minY = 0;
                    int maxX = 0, maxY = 0;
                    for (int i = 0; i < m_layerCount; i++)
                    {
                        if (m_layers[i].Frame != null)
                        {
                            int x, y, w, h;
                            x = m_layers[i].Frame.Center.X;
                            y = m_layers[i].Frame.Center.Y;
                            w = m_layers[i].Frame.Texture.Width;
                            h = m_layers[i].Frame.Texture.Height;

                            if (minX < x)
                                minX = x;
                            if (maxX < w - x)
                                maxX = w - x;

                            if (minY < h + y)
                                minY = h + y;
                            if (maxY > y)
                                maxY = y;
                        }
                    }

                    m_mobile_drawCenterX = minX;
                    m_mobile_drawCenterY = maxY;

                    RenderTarget2D renderTarget = new RenderTarget2D(sb.GraphicsDevice,
                            minX + maxX, minY - maxY, false, SurfaceFormat.Color, DepthFormat.None);

                    sb.GraphicsDevice.SetRenderTarget(renderTarget);
                    sb.GraphicsDevice.Clear(Color.Transparent);

                    for (int i = 0; i < m_layerCount; i++)
                        if (m_layers[i].Frame != null)
                            sb.DrawSimple(m_layers[i].Frame.Texture,
                                new Vector3(
                                    minX - m_layers[i].Frame.Center.X,
                                    renderTarget.Height - m_layers[i].Frame.Texture.Height + maxY - m_layers[i].Frame.Center.Y,
                                    0),
                                    Utility.GetHueVector(m_layers[i].Hue));

                    sb.Flush();
                    m_draw_texture = renderTarget;
                    // MapObjectPrerendered.SavePrerenderedTexture(m_draw_texture, hash, m_mobile_drawCenterX, m_mobile_drawCenterY);
                    sb.GraphicsDevice.SetRenderTarget(null);
                }
            }
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            m_draw_texture = m_layers[0].Frame.Texture;
            m_mobile_drawCenterX = m_layers[0].Frame.Center.X;
            m_mobile_drawCenterY = m_layers[0].Frame.Center.Y;
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
                if (m_layers[i].Frame != null)
                {
                    float x = drawPosition.X - m_draw_X - m_layers[i].Frame.Center.X;
                    float y = drawPosition.Y - m_draw_Y - (m_layers[i].Frame.Texture.Height - m_layers[i].Frame.Center.Y);
                    sb.DrawSimple(m_layers[i].Frame.Texture, new Vector3(x, y, 0), Utility.GetHueVector(m_layers[i].Hue));
                }
            }

            bool isDrawn = true;
            return isDrawn;
        }

        private UltimaData.AnimationFrame getFrame(int bodyID, int hue, int facing, int action, float frame)
        {
            UltimaData.AnimationFrame[] iFrames = UltimaData.AnimationData.GetAnimation(bodyID, action, facing, hue);
            if (iFrames == null)
                return null;
            int iFrame = frameFromSequence(frame, iFrames.Length);
            if (iFrames[iFrame].Texture == null)
                return null;
            return iFrames[iFrame];
        }

        private int frameFromSequence(float frame, int maxFrames)
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