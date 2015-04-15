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
using UltimaXNA.Core.Diagnostics.Tracing;
#endregion

namespace UltimaXNA.Core.Graphics
{
    public class SpriteBatch3D
    {
        private static float Z; // shared between all spritebatches.
        private readonly Game m_Game;

        private readonly List<Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>> m_drawQueue;

        private readonly Effect m_Effect;
        private readonly short[] m_indexBuffer;
        private readonly Queue<List<VertexPositionNormalTextureHue>> m_vertexListQueue;
        private BoundingBox m_ViewportArea;
        private List<VertexPositionNormalTextureHue> m_vertices = new List<VertexPositionNormalTextureHue>();

        public SpriteBatch3D(Game game)
        {
            m_Game = game;

            m_drawQueue = new List<Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>>(1024);
            for (int i = 0; i <= (int)Techniques.Max; i++)
                m_drawQueue.Add(new Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>());

            m_ViewportArea = new BoundingBox(new Vector3(0, 0, Int32.MinValue), new Vector3(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, Int32.MaxValue));
            m_indexBuffer = CreateIndexBuffer(0x2000);
            m_vertexListQueue = new Queue<List<VertexPositionNormalTextureHue>>(256);

            m_Effect = m_Game.Content.Load<Effect>("Shaders/IsometricWorld");
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

        /// <summary>
        /// Draws a quad on screen with the specified texture and vertices.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="vertices"></param>
        /// <returns>True if the object was drawn, false otherwise.</returns>
        public bool Draw(Texture2D texture, VertexPositionNormalTextureHue[] vertices, Techniques effects = Techniques.Default)
        {
            bool draw = false;

            // Check: do not draw if there is no texture to draw with.
            if (texture == null)
                return false;

            // Check: only draw if the texture is within the visible area.
            for (int i = 0; i < 4; i++) // only draws a 2 triangle tristrip.
            {
                if (m_ViewportArea.Contains(vertices[i].Position) == ContainmentType.Contains)
                {
                    draw = true;
                    break;
                }
            }
            if (!draw)
                return false;

            // Set the draw position's z value, and increment the z value for the next drawn object.
            vertices[0].Position.Z = vertices[1].Position.Z = vertices[2].Position.Z = vertices[3].Position.Z = Z++;

            // Get the vertex list for this texture. if none exists, dequeue or create a new vertex list.
            List<VertexPositionNormalTextureHue> vertexList;
            if(m_drawQueue[(int)effects].ContainsKey(texture))
            {
                vertexList = m_drawQueue[(int)effects][texture];
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
                m_drawQueue[(int)effects].Add(texture, vertexList);
            }

            // Add the drawn object to the vertex list.
            for(int i = 0; i < vertices.Length; i++)
                vertexList.Add(vertices[i]);

            return true;
        }

        public void Flush(bool doLighting)
        {
            //set up depth buffer
            DepthStencilState depth = new DepthStencilState();
            depth.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = depth;
            // set up graphics device and texture sampling.
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;
            GraphicsDevice.SamplerStates[3] = SamplerState.PointWrap;
            // do normal lighting? Yes in world, no in UI.
            m_Effect.Parameters["DrawLighting"].SetValue(doLighting);
            // set up viewport.
            float width = GraphicsDevice.Viewport.Width;
            float height = GraphicsDevice.Viewport.Height;
            m_Effect.Parameters["ProjectionMatrix"].SetValue(Matrix.CreateOrthographicOffCenter(0, width, height, 0f, Int16.MinValue, Int16.MaxValue));
            m_Effect.Parameters["WorldMatrix"].SetValue(ProjectionMatrixWorld);
            m_Effect.Parameters["Viewport"].SetValue(new Vector2(width, height));

            Texture2D texture;
            List<VertexPositionNormalTextureHue> vertexList;

            for (Techniques effect = 0; effect <= Techniques.Max; effect += 1)
            {
                switch (effect)
                {
                    case Techniques.Hued:
                        m_Effect.CurrentTechnique = m_Effect.Techniques["HueTechnique"];
                        break;
                    case Techniques.MiniMap:
                        m_Effect.CurrentTechnique = m_Effect.Techniques["MiniMapTechnique"];
                        break;
                    default:
                        Tracer.Critical("Unknown effect in SpriteBatch3D.Flush(). Effect index is {0}", effect);
                        break;
                }
                m_Effect.CurrentTechnique.Passes[0].Apply();

                IEnumerator<KeyValuePair<Texture2D, List<VertexPositionNormalTextureHue>>> keyValuePairs = m_drawQueue[(int)effect].GetEnumerator();
                while (keyValuePairs.MoveNext())
                {
                    texture = keyValuePairs.Current.Key;
                    vertexList = keyValuePairs.Current.Value;
                    GraphicsDevice.Textures[0] = texture;
                    GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertexList.ToArray(), 0, vertexList.Count, m_indexBuffer, 0, vertexList.Count / 2);
                    vertexList.Clear();
                    m_vertexListQueue.Enqueue(vertexList);
                }
                m_drawQueue[(int)effect].Clear();
            }
        }

        public void SetLightDirection(Vector3 direction)
        {
            m_Effect.Parameters["lightDirection"].SetValue(direction);
        }

        public void SetLightIntensity(float intensity)
        {
            m_Effect.Parameters["lightIntensity"].SetValue(intensity);
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
    }
}