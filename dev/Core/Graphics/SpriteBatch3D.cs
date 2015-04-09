/***************************************************************************
 *   SpriteBatch3D.cs
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region Usings

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace UltimaXNA.Core.Rendering
{
    public class SpriteBatch3D
    {
        private static float Z;
        private readonly Game m_Game;

        private readonly Dictionary<Texture2D, List<VertexPositionNormalTextureHue>> m_drawQueue;

        private readonly Effect m_effect;
        private readonly short[] m_indexBuffer;
        private readonly Queue<List<VertexPositionNormalTextureHue>> m_vertexListQueue;
        private BoundingBox m_boundingBox;
        private List<VertexPositionNormalTextureHue> m_vertices = new List<VertexPositionNormalTextureHue>();

        public SpriteBatch3D(Game game)
        {
            m_Game = game;

            m_boundingBox = new BoundingBox(new Vector3(0, 0, Int32.MinValue), new Vector3(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, Int32.MaxValue));
            m_drawQueue = new Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>(256);
            m_indexBuffer = CreateIndexBuffer(0x2000);
            m_vertexListQueue = new Queue<List<VertexPositionNormalTextureHue>>(256);

            m_effect = m_Game.Content.Load<Effect>("Shaders/IsometricWorld");
            m_effect.CurrentTechnique = m_effect.Techniques["StandardEffect"];
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                if(m_Game == null)
                {
                    return null;
                }
                return m_Game.GraphicsDevice;
            }
        }

        public static Matrix ProjectionMatrixWorld
        {
            get { return Matrix.Identity; }
        }

        public static Matrix ProjectionMatrixScreen
        {
            get { return Matrix.CreateOrthographicOffCenter(0, 800f, 600f, 0f, Int16.MinValue, Int16.MaxValue); }
        }

        public static void ResetZ()
        {
            Z = 0;
        }

        public bool DrawSimple(Texture2D texture, Vector3 position, Vector2 hue)
        {
            VertexPositionNormalTextureHue[] v =
            {
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
            float minX = sourceRect.X / (float)texture.Width, maxX = (sourceRect.X + sourceRect.Width) / (float)texture.Width;
            float minY = sourceRect.Y / (float)texture.Height, maxY = (sourceRect.Y + sourceRect.Height) / (float)texture.Height;

            VertexPositionNormalTextureHue[] v =
            {
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y, 0), new Vector3(0, 0, 1), new Vector3(minX, minY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + sourceRect.Width, position.Y, 0), new Vector3(0, 0, 1), new Vector3(maxX, minY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X, position.Y + sourceRect.Height, 0), new Vector3(0, 0, 1), new Vector3(minX, maxY, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.X + sourceRect.Width, position.Y + sourceRect.Height, 0), new Vector3(0, 0, 1), new Vector3(maxX, maxY, 0))
            };

            v[0].Hue = v[1].Hue = v[2].Hue = v[3].Hue = hue;
            return Draw(texture, v);
        }

        public bool DrawSimple(Texture2D texture, Rectangle destRect, Rectangle sourceRect, Vector2 hue)
        {
            float minX = sourceRect.X / (float)texture.Width, maxX = (sourceRect.X + sourceRect.Width) / (float)texture.Width;
            float minY = sourceRect.Y / (float)texture.Height, maxY = (sourceRect.Y + sourceRect.Height) / (float)texture.Height;

            VertexPositionNormalTextureHue[] v =
            {
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
            VertexPositionNormalTextureHue[] v =
            {
                new VertexPositionNormalTextureHue(new Vector3(destRect.X, destRect.Y, 0), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X + destRect.Width, destRect.Y, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X, destRect.Y + destRect.Height, 0), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(destRect.X + destRect.Width, destRect.Y + destRect.Height, 0), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
            };

            v[0].Hue = v[1].Hue = v[2].Hue = v[3].Hue = hue;
            return Draw(texture, v);
        }

        public bool DrawSimpleTiled(Texture2D texture, Rectangle destRect, Vector2 hue)
        {
            int y = destRect.Y;
            int h = destRect.Height;
            Rectangle sRect;

            while(h > 0)
            {
                int x = destRect.X;
                int w = destRect.Width;
                if(h < texture.Height)
                {
                    sRect = new Rectangle(0, 0, texture.Width, h);
                }
                else
                {
                    sRect = new Rectangle(0, 0, texture.Width, texture.Height);
                }
                while(w > 0)
                {
                    if(w < texture.Width)
                    {
                        sRect.Width = w;
                    }
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

            if(texture == null)
            {
                return false;
            }

            for(int i = 0; i < 4; i++) // only draws a 2 triangle tristrip.
            {
                if(m_boundingBox.Contains(vertices[i].Position) == ContainmentType.Contains)
                {
                    draw = true;
                    break;
                }
            }

            if(!draw)
            {
                return false;
            }

            vertices[0].Position.Z = Z;
            vertices[1].Position.Z = Z;
            vertices[2].Position.Z = Z;
            vertices[3].Position.Z = Z;
            Z += 1;

            List<VertexPositionNormalTextureHue> vertexList;

            if(m_drawQueue.ContainsKey(texture))
            {
                vertexList = m_drawQueue[texture];
            }
            else
            {
                if(m_vertexListQueue.Count > 0)
                {
                    vertexList = m_vertexListQueue.Dequeue();

                    vertexList.Clear();
                }
                else
                {
                    vertexList = new List<VertexPositionNormalTextureHue>(1024);
                }

                m_drawQueue.Add(texture, vertexList);
            }

            for(int i = 0; i < vertices.Length; i++)
            {
                vertexList.Add(vertices[i]);
            }

            return true;
        }

        public void Prepare(bool doLighting, bool doDepth)
        {
            DepthStencilState depth = new DepthStencilState();
            depth.DepthBufferEnable = doDepth;
            GraphicsDevice.DepthStencilState = depth;

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;

            m_effect.Parameters["DrawLighting"].SetValue(doLighting);
        }

        public void Flush()
        {
            float width = GraphicsDevice.Viewport.Width;
            float height = GraphicsDevice.Viewport.Height;
            m_effect.Parameters["ProjectionMatrix"].SetValue(Matrix.CreateOrthographicOffCenter(0, width, height, 0f, Int16.MinValue, Int16.MaxValue));
            m_effect.Parameters["WorldMatrix"].SetValue(ProjectionMatrixWorld);
            m_effect.Parameters["Viewport"].SetValue(new Vector2(width, height));

            Texture2D iTexture;
            List<VertexPositionNormalTextureHue> iVertexList;

            IEnumerator<KeyValuePair<Texture2D, List<VertexPositionNormalTextureHue>>> keyValuePairs = m_drawQueue.GetEnumerator();

            while(keyValuePairs.MoveNext())
            {
                m_effect.CurrentTechnique.Passes[0].Apply();
                iTexture = keyValuePairs.Current.Key;
                iVertexList = keyValuePairs.Current.Value;
                GraphicsDevice.Textures[0] = iTexture;
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, iVertexList.ToArray(), 0, iVertexList.Count, m_indexBuffer, 0, iVertexList.Count / 2);
                iVertexList.Clear();
                m_vertexListQueue.Enqueue(iVertexList);
            }

            m_drawQueue.Clear();
        }

        public void SetLightDirection(Vector3 direction)
        {
            m_effect.Parameters["lightDirection"].SetValue(direction);
        }

        public void SetAmbientLightIntensity(float intensity)
        {
            m_effect.Parameters["ambientLightIntensity"].SetValue(intensity);
        }

        public void SetDirectionalLightIntensity(float intensity)
        {
            m_effect.Parameters["lightIntensity"].SetValue(intensity);
        }

        private short[] CreateIndexBuffer(int primitiveCount)
        {
            short[] indices = new short[primitiveCount * 6];

            for(int i = 0; i < primitiveCount; i++)
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

        private bool isLandscapeTexture(Texture2D t)
        {
            if(t.Width == 64 && t.Height == 64)
            {
                return true;
            }
            if(t.Width == 128 && t.Height == 128)
            {
                return true;
            }
            return false;
        }
    }
}