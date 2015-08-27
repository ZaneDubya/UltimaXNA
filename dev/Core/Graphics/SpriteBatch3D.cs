/***************************************************************************
 *   SpriteBatch3D.cs
 *   Based on Chase Mosher's UO Renderer, licensed under GPLv3.
 *   Modifications Copyright (c) 2009, 2015 UltimaXNA Development Team
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
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;
#endregion

namespace UltimaXNA.Core.Graphics
{
    public class SpriteBatch3D
    {
        private static float Z; // shared between all spritebatches.
        public float GetNextUniqueZ()
        {
            return Z++;
        }


        private static BoundingBox ViewportArea;
        private static Game Game;

        private readonly List<Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>> m_drawQueue;

        private readonly Effect m_Effect;
        private readonly short[] m_indexBuffer;
        private readonly Queue<List<VertexPositionNormalTextureHue>> m_vertexListQueue;
        
        private List<VertexPositionNormalTextureHue> m_vertices = new List<VertexPositionNormalTextureHue>();

        public SpriteBatch3D(Game game)
        {
            Game = game;

            m_drawQueue = new List<Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>>((int)Techniques.All);
            for (int i = 0; i <= (int)Techniques.All; i++)
                m_drawQueue.Add(new Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>(1024));

            m_indexBuffer = CreateIndexBuffer(0x2000);
            m_vertexListQueue = new Queue<List<VertexPositionNormalTextureHue>>(256);

            m_Effect = Game.Content.Load<Effect>("Shaders/IsometricWorld");
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                if (Game == null)
                {
                    return null;
                }
                return Game.GraphicsDevice;
            }
        }

        public Matrix ProjectionMatrixWorld
        {
            get { return Matrix.Identity; }
        }

        public Matrix ProjectionMatrixScreen
        {
            get { return Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0f, Int16.MinValue, Int16.MaxValue); }
        }

        public static void Reset()
        {
            Z = 0;
            ViewportArea = new BoundingBox(new Vector3(0, 0, Int32.MinValue), new Vector3(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height, Int32.MaxValue));
        }

        /// <summary>
        /// Draws a quad on screen with the specified texture and vertices.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="vertices"></param>
        /// <returns>True if the object was drawn, false otherwise.</returns>
        public bool Draw(Texture2D texture, VertexPositionNormalTextureHue[] vertices, Techniques effect = Techniques.Default)
        {
            bool draw = false;

            // Sanity: do not draw if there is no texture to draw with.
            if (texture == null)
                return false;

            // Check: only draw if the texture is within the visible area.
            for (int i = 0; i < 4; i++) // only draws a 2 triangle tristrip.
            {
                if (ViewportArea.Contains(vertices[i].Position) == ContainmentType.Contains)
                {
                    draw = true;
                    break;
                }
            }
            if (!draw)
                return false;

            // Set the draw position's z value, and increment the z value for the next drawn object.
            vertices[0].Position.Z = vertices[1].Position.Z = vertices[2].Position.Z = vertices[3].Position.Z = Z++;

            // Get the vertex list for this texture. if none exists, dequeue existing or create a new vertex list.
            List<VertexPositionNormalTextureHue> vertexList = GetVertexList(texture, effect);

            // Add the drawn object to the vertex list.
            for(int i = 0; i < vertices.Length; i++)
                vertexList.Add(vertices[i]);

            return true;
        }

        public void DrawShadow(Texture2D texture, VertexPositionNormalTextureHue[] vertices, Vector3 drawPosition, bool useVertex02, float z)
        {
            // Sanity: do not draw if there is no texture to draw with.
            if (texture == null)
                return;

            List<VertexPositionNormalTextureHue> vertexList;
            vertices[0].Position.Z = vertices[1].Position.Z = vertices[2].Position.Z = vertices[3].Position.Z = z;
            float xSkew = drawPosition.X - vertices[2].Position.X;
            vertices[0].Position.X += xSkew; // skew texture
            vertices[useVertex02 ? 2 : 1].Position.X += xSkew; // skew texture

            vertexList = GetVertexList(texture, Techniques.ShadowSet);
            for (int i = 0; i < vertices.Length; i++)
                vertexList.Add(vertices[i]);
        }

        public void FlushSprites(bool doLighting)
        {
            //set up depth/stencil buffer
            DepthStencilState depthDefault = new DepthStencilState();
            depthDefault.DepthBufferEnable = true;
            depthDefault.DepthBufferWriteEnable = true;

            // set up graphics device and texture sampling.
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp; // the sprite texture sampler.
            GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp; // hue sampler (1/2)
            GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp; // hue sampler (2/2)
            GraphicsDevice.SamplerStates[3] = SamplerState.PointWrap; // the minimap sampler.
            // We use lighting parameters to shade vertexes when we're drawing the world.
            m_Effect.Parameters["DrawLighting"].SetValue(doLighting);
            // set up viewport.
            m_Effect.Parameters["ProjectionMatrix"].SetValue(ProjectionMatrixScreen);
            m_Effect.Parameters["WorldMatrix"].SetValue(ProjectionMatrixWorld);
            m_Effect.Parameters["Viewport"].SetValue(new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height));

            GraphicsDevice.DepthStencilState = depthDefault;
            DrawAllVertices(Techniques.FirstDrawn, Techniques.LastDrawn);
        }

        private void DrawAllVertices(Techniques first, Techniques last)
        {
            // draw normal objects
            for (Techniques effect = first; effect <= last; effect += 1)
            {
                switch (effect)
                {
                    case Techniques.Hued:
                        m_Effect.CurrentTechnique = m_Effect.Techniques["HueTechnique"];
                        break;
                    case Techniques.MiniMap:
                        m_Effect.CurrentTechnique = m_Effect.Techniques["MiniMapTechnique"];
                        break;
                    case Techniques.Grayscale:
                        m_Effect.CurrentTechnique = m_Effect.Techniques["GrayscaleTechnique"];
                        break;
                    case Techniques.ShadowSet:
                        m_Effect.CurrentTechnique = m_Effect.Techniques["ShadowSetTechnique"];
                        break;
                    default:
                        Tracer.Critical("Unknown effect in SpriteBatch3D.Flush(). Effect index is {0}", effect);
                        break;
                }
                m_Effect.CurrentTechnique.Passes[0].Apply();

                IEnumerator<KeyValuePair<Texture2D, List<VertexPositionNormalTextureHue>>> vertexEnumerator = m_drawQueue[(int)effect].GetEnumerator();
                while (vertexEnumerator.MoveNext())
                {
                    Texture2D texture = vertexEnumerator.Current.Key;
                    List<VertexPositionNormalTextureHue> vertexList = vertexEnumerator.Current.Value;
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

        private List<VertexPositionNormalTextureHue> GetVertexList(Texture2D texture, Techniques effect)
        {
            List<VertexPositionNormalTextureHue> vertexList;
            if (m_drawQueue[(int)effect].ContainsKey(texture))
            {
                vertexList = m_drawQueue[(int)effect][texture];
            }
            else
            {
                if (m_vertexListQueue.Count > 0)
                {
                    vertexList = m_vertexListQueue.Dequeue();
                    vertexList.Clear();
                }
                else
                {
                    vertexList = new List<VertexPositionNormalTextureHue>(1024);
                }
                m_drawQueue[(int)effect].Add(texture, vertexList);
            }
            return vertexList;
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