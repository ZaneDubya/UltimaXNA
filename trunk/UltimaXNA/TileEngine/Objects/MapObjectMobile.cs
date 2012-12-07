/***************************************************************************
 *   MapObjectMobile.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using UltimaXNA.Graphics;
#endregion

namespace UltimaXNA.TileEngine
{
    public class MapObjectMobile : MapObjectPrerendered
    {
        public int Action { get; set; }
        public int Facing { get; set; }

        private float _frame;
        private int _layerCount = 0;
        private int _frameCount = 0;
        private MapObjectMobileLayer[] _layers;

        public MapObjectMobile(Position3D position, int facing, int action, float frame, BaseEntity ownerEntity)
            : base(position)
        {
            if (frame >= 1.0f)
                return;

            _layers = new MapObjectMobileLayer[(int)EquipLayer.LastUserValid];

            Facing = facing;
            Action = action;
            _frame = frame;
            OwnerEntity = ownerEntity;

            // set pick type
            _pickType = PickTypes.PickObjects;

            // set up draw data
            _draw_flip = (Facing > 4) ? true : false;
            _draw_IsometricOverlap = true;
        }

        public void AddLayer(int bodyID, int hue)
        {
            _layers[_layerCount++] = new MapObjectMobileLayer(bodyID, hue, getFrame(bodyID, hue, Facing, Action, _frame));
            _frameCount = UltimaData.AnimationsXNA.GetAnimationFrameCount(bodyID, Action, Facing, hue);
        }

        private long createHashFromLayers()
        {
            int[] hashArray = new int[_layerCount * 2 + 3];
            hashArray[0] = (int)Action;
            hashArray[1] = Facing;
            hashArray[2] = frameFromSequence(_frame, _frameCount);
            for (int i = 0; i < _layerCount; i++)
            {
                hashArray[3 + i * 2] = _layers[i].BodyID;
                hashArray[4 + i * 2] = _layers[i].Hue;
            }

            long hash = 0;
            for (int i = 0; i < hashArray.Length; i++)
            {
                hash = unchecked(hash * 31 + hashArray[i]);
                // hash ^= hashArray[i];
            }
            return hash;
        }

        private int _mobile_drawCenterX, _mobile_drawCenterY;
        protected override void Prerender(SpriteBatch3D sb)
        {
            if (_layerCount == 1)
            {
                _draw_hue = Utility.GetHueVector(_layers[0].Hue);
                _draw_texture = _layers[0].Frame.Texture;
                _mobile_drawCenterX = _layers[0].Frame.Center.X;
                _mobile_drawCenterY = _layers[0].Frame.Center.Y;
            }
            else
            {
                _draw_hue = Utility.GetHueVector(0);
                long hash = createHashFromLayers();
                _draw_texture = MapObjectPrerendered.RestorePrerenderedTexture(hash, out _mobile_drawCenterX, out _mobile_drawCenterY);
                if (_draw_texture == null)
                {
                    int minX = 0, minY = 0;
                    int maxX = 0, maxY = 0;
                    for (int i = 0; i < _layerCount; i++)
                    {
                        if (_layers[i].Frame != null)
                        {
                            int x, y, w, h;
                            x = _layers[i].Frame.Center.X;
                            y = _layers[i].Frame.Center.Y;
                            w = _layers[i].Frame.Texture.Width;
                            h = _layers[i].Frame.Texture.Height;

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

                    _mobile_drawCenterX = minX;
                    _mobile_drawCenterY = maxY;

                    RenderTarget2D renderTarget = new RenderTarget2D(sb.GraphicsDevice,
                            minX + maxX, minY - maxY, false, SurfaceFormat.Color, DepthFormat.None);

                    sb.GraphicsDevice.SetRenderTarget(renderTarget);
                    sb.GraphicsDevice.Clear(Color.Transparent);

                    for (int i = 0; i < _layerCount; i++)
                        if (_layers[i].Frame != null)
                            sb.DrawSimple(_layers[i].Frame.Texture,
                                new Vector3(
                                    minX - _layers[i].Frame.Center.X,
                                    renderTarget.Height - _layers[i].Frame.Texture.Height + maxY - _layers[i].Frame.Center.Y,
                                    0),
                                    Utility.GetHueVector(_layers[i].Hue));

                    sb.Flush();
                    _draw_texture = renderTarget;
                    MapObjectPrerendered.SavePrerenderedTexture(_draw_texture, hash, _mobile_drawCenterX, _mobile_drawCenterY);
                    sb.GraphicsDevice.SetRenderTarget(null);
                }
            }
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            _draw_width = _draw_texture.Width;
            _draw_height = _draw_texture.Height;
            if (_draw_flip)
            {
                _draw_X = _mobile_drawCenterX - 22 + (int)((Position.X_offset - Position.Y_offset) * 22);
                _draw_Y = _mobile_drawCenterY + (int)((Position.Z_offset + Z) * 4) + _draw_height - 22 - (int)((Position.X_offset + Position.Y_offset) * 22);
            }
            else
            {
                _draw_X = _mobile_drawCenterX - 22 - (int)((Position.X_offset - Position.Y_offset) * 22);
                _draw_Y = _mobile_drawCenterY + (int)((Position.Z_offset + Z) * 4) + _draw_height - 22 - (int)((Position.X_offset + Position.Y_offset) * 22);
            }

            if (UltimaVars.EngineVars.LastTarget != null && UltimaVars.EngineVars.LastTarget == OwnerSerial)
                _draw_hue = new Vector2(((Mobile)OwnerEntity).NotorietyHue - 1, 1);

            // !!! Test highlight code.
            bool isHighlight = false;
            if (isHighlight)
            {
                Vector2 savedHue = _draw_hue;
                int savedX = _draw_X, savedY = _draw_Y;
                _draw_hue = Utility.GetHueVector(1288);
                int offset = 1;
                _draw_X = savedX - offset;
                base.Draw(sb, drawPosition, molist, pickType, maxAlt);
                _draw_X = savedX + offset;
                base.Draw(sb, drawPosition, molist, pickType, maxAlt);
                _draw_X = savedX;
                _draw_Y = savedY - offset;
                base.Draw(sb, drawPosition, molist, pickType, maxAlt);
                _draw_Y = savedY + offset;
                base.Draw(sb, drawPosition, molist, pickType, maxAlt);
                _draw_hue = savedHue;
                _draw_X = savedX;
                _draw_Y = savedY;
            }

            bool isDrawn = base.Draw(sb, drawPosition, molist, pickType, maxAlt);
            return isDrawn;
        }

        private UltimaData.FrameXNA getFrame(int bodyID, int hue, int facing, int action, float frame)
        {
            UltimaData.FrameXNA[] iFrames = UltimaData.AnimationsXNA.GetAnimation(bodyID, action, facing, hue);
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
        public UltimaData.FrameXNA Frame;
        public int BodyID;

        public MapObjectMobileLayer(int bodyID, int hue, UltimaData.FrameXNA frame)
        {
            BodyID = bodyID;
            Hue = hue;
            Frame = frame;
        }
    }
}