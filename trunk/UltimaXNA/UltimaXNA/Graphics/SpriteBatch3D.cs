﻿/***************************************************************************
 *   SpriteBatch3D.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Graphics
{
    public class SpriteBatch3D : GameComponent
    {
        private Dictionary<Texture2D, List<VertexPositionNormalTextureHue>> _drawQueue;
        private Queue<List<VertexPositionNormalTextureHue>> _vertexListQueue;
        private List<VertexPositionNormalTextureHue> _vertices = new List<VertexPositionNormalTextureHue>();
        private short[] _indexBuffer;

        private Effect _effect;
        private BoundingBox _boundingBox;

        static float _z = 0;
        public static void ResetZ()
        {
            _z = 0;
        }

        public SpriteBatch3D(Game game)
            : base(game)
        {
            _boundingBox = new BoundingBox(new Vector3(0, 0, Int32.MinValue), new Vector3(this.Game.GraphicsDevice.Viewport.Width, this.Game.GraphicsDevice.Viewport.Height, Int32.MaxValue));
            _drawQueue = new Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>(256);
            _indexBuffer = CreateIndexBuffer(0x1000);
            _vertexListQueue = new Queue<List<VertexPositionNormalTextureHue>>(256);

            _effect = this.Game.Content.Load<Effect>("Shaders/IsometricWorld");
            _effect.CurrentTechnique = _effect.Techniques["StandardEffect"];
        }

        public bool DrawSimple(Texture2D texture, Vector3 position, Vector2 hue)
        {
            VertexPositionNormalTextureHue[] v = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y, 0), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + texture.Width, position.Y, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y + texture.Height, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + texture.Width, position.Y + texture.Height, 0), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
            };

            v[0].Hue = v[1].Hue = v[2].Hue = v[3].Hue = hue;
            return Draw(texture, v);
        }

        public bool DrawSimple(Texture2D texture, Vector3 position, Rectangle sourceRect, Vector2 hue)
        {
            float minX = (float)sourceRect.X / (float)texture.Width, maxX = (float)(sourceRect.X + sourceRect.Width) / (float)texture.Width;
            float minY = (float)sourceRect.Y / (float)texture.Height, maxY = (float)(sourceRect.Y + sourceRect.Height) / (float)texture.Height;

            VertexPositionNormalTextureHue[] v = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y, 0), new Vector3(0, 0, 1), new Vector3(minX, minY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + sourceRect.Width, position.Y, 0), new Vector3(0, 0, 1), new Vector3(maxX, minY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y + sourceRect.Height, 0), new Vector3(0, 0, 1), new Vector3(minX, maxY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + sourceRect.Width, position.Y + sourceRect.Height, 0), new Vector3(0, 0, 1), new Vector3(maxX, maxY, 0))
            };

            v[0].Hue = v[1].Hue = v[2].Hue =  v[3].Hue = hue;
            return Draw(texture, v);
        }

        public bool DrawSimple(Texture2D texture, Rectangle destRect, Rectangle sourceRect, Vector2 hue)
        {
            float minX = (float)sourceRect.X / (float)texture.Width, maxX = (float)(sourceRect.X + sourceRect.Width) / (float)texture.Width;
            float minY = (float)sourceRect.Y / (float)texture.Height, maxY = (float)(sourceRect.Y + sourceRect.Height) / (float)texture.Height;

            VertexPositionNormalTextureHue[] v = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(destRect.X, destRect.Y, 0), new Vector3(0, 0, 1), new Vector3(minX, minY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X + destRect.Width, destRect.Y, 0), new Vector3(0, 0, 1), new Vector3(maxX, minY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X, destRect.Y + destRect.Height, 0), new Vector3(0, 0, 1), new Vector3(minX, maxY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X + destRect.Width, destRect.Y + destRect.Height, 0), new Vector3(0, 0, 1), new Vector3(maxX, maxY, 0))
            };

            v[0].Hue = v[1].Hue = v[2].Hue = v[3].Hue = hue;
            return Draw(texture, v);
        }

        public bool DrawSimple(Texture2D texture, Rectangle destRect, Vector2 hue)
        {
            VertexPositionNormalTextureHue[] v = new VertexPositionNormalTextureHue[] {
                new VertexPositionNormalTextureHue(new Vector3(destRect.X, destRect.Y, 0), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X + destRect.Width, destRect.Y, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X, destRect.Y + destRect.Height, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X + destRect.Width, destRect.Y + destRect.Height, 0), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
            };

            v[0].Hue = v[1].Hue = v[2].Hue =  v[3].Hue = hue;
            return Draw(texture, v);
        }

        public bool DrawSimpleTiled(Texture2D texture, Rectangle destRect, Vector2 hue)
        {
            int y = destRect.Y;
            int h = destRect.Height;
            Rectangle sRect;

            while (h > 0)
            {
                int x = destRect.X;
                int w = destRect.Width;
                if (h < texture.Height)
                    sRect = new Rectangle(0, 0, texture.Width, h);
                else
                    sRect = new Rectangle(0, 0, texture.Width, texture.Height);
                while (w > 0)
                {
                    if (w < texture.Width)
                        sRect.Width = w;
                    DrawSimple(texture, new Vector3(x, y, 0), sRect, hue);
                    w -= texture.Width;
                    x += texture.Width;
                }
                h -= texture.Height;
                y += texture.Height;
            }

            return true;
        }

        public bool Draw(Texture2D texture, VertexPositionNormalTextureHue[] vertices)
        {
            bool draw = false;

            for (int i = 0; i < 4; i++) // only draws a 2 triangle tristrip.
            {
                if (_boundingBox.Contains(vertices[i].Position) == ContainmentType.Contains)
                {
                    draw = true;
                    break;
                }
            }

            if (!draw)
                return false;

            vertices[0].Position.Z = _z;
            vertices[1].Position.Z = _z;
            vertices[2].Position.Z = _z;
            vertices[3].Position.Z = _z;
            _z += 1;

            List<VertexPositionNormalTextureHue> vertexList;

            if (_drawQueue.ContainsKey(texture))
            {
                vertexList = _drawQueue[texture];
            }
            else
            {
                if (_vertexListQueue.Count > 0)
                {
                    vertexList = _vertexListQueue.Dequeue();

                    vertexList.Clear();
                }
                else
                {
                    vertexList = new List<VertexPositionNormalTextureHue>(1024);
                }

                _drawQueue.Add(texture, vertexList);
            }

            for (int i = 0; i < vertices.Length; i++)
            {
                vertexList.Add(vertices[i]);
            }

            return true;
        }

        public void Prepare(bool doLighting, bool doDepth)
        {
            DepthStencilState depth = new DepthStencilState();
            depth.DepthBufferEnable = doDepth;
            Game.GraphicsDevice.DepthStencilState = depth;

            Game.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            Game.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            Game.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            _effect.Parameters["DrawLighting"].SetValue(doLighting);
            Game.GraphicsDevice.Textures[1] = UltimaXNA.Data.HuesXNA.HueTexture;
        }

        public void Flush()
        {
            float width = Game.GraphicsDevice.Viewport.Width;
            float height = Game.GraphicsDevice.Viewport.Height;
            _effect.Parameters["ProjectionMatrix"].SetValue(Matrix.CreateOrthographicOffCenter(0, width, height, 0f, Int16.MinValue, Int16.MaxValue));
            _effect.Parameters["WorldMatrix"].SetValue(Utility.ProjectionMatrixWorld);
            _effect.Parameters["Viewport"].SetValue(new Vector2(width, height));

            Texture2D iTexture;
            List<VertexPositionNormalTextureHue> iVertexList;

            IEnumerator<KeyValuePair<Texture2D, List<VertexPositionNormalTextureHue>>> keyValuePairs = _drawQueue.GetEnumerator();

            while (keyValuePairs.MoveNext())
            {
                _effect.CurrentTechnique.Passes[0].Apply();
                iTexture = keyValuePairs.Current.Key;
                iVertexList = keyValuePairs.Current.Value;
                Game.GraphicsDevice.Textures[0] = iTexture;
                Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTextureHue>(PrimitiveType.TriangleList, iVertexList.ToArray(), 0, iVertexList.Count, _indexBuffer, 0, iVertexList.Count / 2);
                iVertexList.Clear();
                _vertexListQueue.Enqueue(iVertexList);
            }

            _drawQueue.Clear();
        }

        public void SetLightDirection(Vector3 nDirection)
        {
            _effect.Parameters["lightDirection"].SetValue(nDirection);
        }

		public void SetAmbientLightIntensity(float intensity)
		{
				_effect.Parameters["ambientLightIntensity"].SetValue ( intensity );
		}

		public void SetDirectionalLightIntensity(float intensity)
		{
				_effect.Parameters["lightIntensity"].SetValue ( intensity );
		}

        private short[] CreateIndexBuffer(int primitiveCount)
        {
            short[] indices = new short[primitiveCount * 6];

            for (int i = 0; i < primitiveCount; i++)
            {
                indices[i * 6] = (short)(i * 4);
                indices[i * 6 + 1] = (short)(i * 4 + 1);
                indices[i * 6 + 2] = (short)(i * 4 + 2);
                indices[i * 6 + 3] = (short)(i * 4 + 2);
                indices[i * 6 + 4] = (short)(i * 4 + 1);
                indices[i * 6 + 5] = (short)(i * 4 + 3);
            }

            return indices;
        }

        bool isLandscapeTexture(Texture2D t)
        {
            if (t.Width == 64 && t.Height == 64)
                return true;
            if (t.Width == 128 && t.Height == 128)
                return true;
            return false;
        }
    }
}