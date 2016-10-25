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
        private const int MAX_VERTICES_PER_DRAW = 0x2000;
        private const int INITIAL_TEXTURE_COUNT = 0x800;
        private const float MAX_ACCURATE_SINGLE_FLOAT = 65536; // this number is somewhat arbitrary; it's the number at which the
        // difference between two subsequent integers is +/-0.005. See http://stackoverflow.com/questions/872544/precision-of-floating-point

        private float m_Z;

        private readonly Game m_Game;
        private readonly Effect m_Effect;
        private readonly short[] m_IndexBuffer;
        private static BoundingBox m_ViewportArea;
        private readonly Queue<List<VertexPositionNormalTextureHue>> m_VertexListQueue;
        private readonly List<Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>> m_DrawQueue;
        private readonly VertexPositionNormalTextureHue[] m_VertexArray;

        public SpriteBatch3D(Game game)
        {
            m_Game = game;

            m_DrawQueue = new List<Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>>((int)Techniques.All);
            for (int i = 0; i <= (int)Techniques.All; i++)
                m_DrawQueue.Add(new Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>(INITIAL_TEXTURE_COUNT));

            m_IndexBuffer = CreateIndexBuffer(MAX_VERTICES_PER_DRAW);
            m_VertexArray = new VertexPositionNormalTextureHue[MAX_VERTICES_PER_DRAW];
            m_VertexListQueue = new Queue<List<VertexPositionNormalTextureHue>>(INITIAL_TEXTURE_COUNT);

            m_Effect = m_Game.Content.Load<Effect>("Shaders/IsometricWorld");
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                if (m_Game == null)
                {
                    return null;
                }
                return m_Game.GraphicsDevice;
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

        public void Reset(bool setZHigh = false)
        {
            m_Z = setZHigh ? MAX_ACCURATE_SINGLE_FLOAT : 0;
            m_ViewportArea = new BoundingBox(new Vector3(0, 0, Int32.MinValue), new Vector3(m_Game.GraphicsDevice.Viewport.Width, m_Game.GraphicsDevice.Viewport.Height, Int32.MaxValue));
        }

        public virtual float GetNextUniqueZ()
        {
            return m_Z++;
        }

        /// <summary>
        /// Draws a quad on screen with the specified texture and vertices.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="vertices"></param>
        /// <returns>True if the object was drawn, false otherwise.</returns>
        public bool DrawSprite(Texture2D texture, VertexPositionNormalTextureHue[] vertices, Techniques effect = Techniques.Default)
        {
            bool draw = false;

            // Sanity: do not draw if there is no texture to draw with.
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
            vertices[0].Position.Z = vertices[1].Position.Z = vertices[2].Position.Z = vertices[3].Position.Z = GetNextUniqueZ();

            // Get the vertex list for this texture. if none exists, dequeue existing or create a new vertex list.
            List<VertexPositionNormalTextureHue> vertexList = GetVertexList(texture, effect);

            // Add the drawn object to the vertex list.
            for(int i = 0; i < vertices.Length; i++)
                vertexList.Add(vertices[i]);

            return true;
        }

        /// <summary>
        /// Draws a special 'shadow' sprite, automatically skewing the passed vertices.
        /// </summary>
        /// <param name="texture">The texture to draw with.</param>
        /// <param name="vertices">An array of four vertices. Note: modified by this routine.</param>
        /// <param name="drawPosition">The draw position at which this sprite begins (should be the center of an isometric tile for non-moving sprites).</param>
        /// <param name="flipVertices">See AEntityView.Draw(); this is equivalent to DrawFlip.</param>
        /// <param name="z">The z depth at which the shadow sprite should be placed.</param>
        public void DrawShadow(Texture2D texture, VertexPositionNormalTextureHue[] vertices, Vector2 drawPosition, bool flipVertices, float z)
        {
            // Sanity: do not draw if there is no texture to draw with.
            if (texture == null)
                return;
            // set proper z depth for this shadow.
            vertices[0].Position.Z = vertices[1].Position.Z = vertices[2].Position.Z = vertices[3].Position.Z = z;
            // skew texture
            float skewHorizTop = (vertices[0].Position.Y - drawPosition.Y) * .5f;
            float skewHorizBottom = (vertices[3].Position.Y - drawPosition.Y) * .5f;
            vertices[0].Position.X -= skewHorizTop;
            vertices[0].Position.Y -= skewHorizTop;
            vertices[flipVertices ? 2 : 1].Position.X -= skewHorizTop;
            vertices[flipVertices ? 2 : 1].Position.Y -= skewHorizTop;
            vertices[flipVertices ? 1 : 2].Position.X -= skewHorizBottom;
            vertices[flipVertices ? 1 : 2].Position.Y -= skewHorizBottom;
            vertices[3].Position.X -= skewHorizBottom; 
            vertices[3].Position.Y -= skewHorizBottom;

            List<VertexPositionNormalTextureHue> vertexList;
            vertexList = GetVertexList(texture, Techniques.ShadowSet);
            for (int i = 0; i < vertices.Length; i++)
                vertexList.Add(vertices[i]);
        }

        public void DrawStencil(Texture2D texture, VertexPositionNormalTextureHue[] vertices)
        {
            // Sanity: do not draw if there is no texture to draw with.
            if (texture == null)
                return;
            // set proper z depth for this shadow.
            vertices[0].Position.Z = vertices[1].Position.Z = vertices[2].Position.Z = vertices[3].Position.Z = GetNextUniqueZ();
        }

        public void FlushSprites(bool doLighting)
        {
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
            // enable depth sorting, disable the stencil
            SetDepthStencilState(true, false);
            DrawAllVertices(Techniques.FirstDrawn, Techniques.LastDrawn);
        }

        private void DrawAllVertices(Techniques first, Techniques last)
        {
            // draw normal objects
            for (Techniques effect = first; effect <= last; effect++)
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
                        SetDepthStencilState(true, true);
                        break;
                    case Techniques.StencilSet:
                        // do nothing;
                        break;
                    default:
                        Tracer.Critical("Unknown effect in SpriteBatch3D.Flush(). Effect index is {0}", effect);
                        break;
                }
                m_Effect.CurrentTechnique.Passes[0].Apply();

                IEnumerator<KeyValuePair<Texture2D, List<VertexPositionNormalTextureHue>>> vertexEnumerator = m_DrawQueue[(int)effect].GetEnumerator();
                while (vertexEnumerator.MoveNext())
                {
                    Texture2D texture = vertexEnumerator.Current.Key;
                    List<VertexPositionNormalTextureHue> vertexList = vertexEnumerator.Current.Value;
                    GraphicsDevice.Textures[0] = texture;

                    GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, CopyVerticesToArray(vertexList), 0,  Math.Min(vertexList.Count,MAX_VERTICES_PER_DRAW), m_IndexBuffer, 0, vertexList.Count / 2);
                    vertexList.Clear();
                    m_VertexListQueue.Enqueue(vertexList);
                }
                m_DrawQueue[(int)effect].Clear();
            }
        }

        private VertexPositionNormalTextureHue[] CopyVerticesToArray(List<VertexPositionNormalTextureHue> vertices)
        {
            int max = vertices.Count <= MAX_VERTICES_PER_DRAW ? vertices.Count : MAX_VERTICES_PER_DRAW;
            vertices.CopyTo(0, m_VertexArray, 0, max);
            return m_VertexArray;
        }

        public void SetLightDirection(Vector3 direction)
        {
            m_Effect.Parameters["lightDirection"].SetValue(direction);
        }

        public void SetLightIntensity(float intensity)
        {
            m_Effect.Parameters["lightIntensity"].SetValue(intensity);
        }

        private void SetDepthStencilState(bool depth, bool stencil)
        {
            // depth is currently ignored.
            DepthStencilState dss = new DepthStencilState();
            dss.DepthBufferEnable = true;
            dss.DepthBufferWriteEnable = true;

            if (stencil)
            {
                dss.StencilEnable = true;
                dss.StencilFunction = CompareFunction.Equal;
                dss.ReferenceStencil = 0;
                dss.StencilPass = StencilOperation.Increment;
                dss.StencilFail = StencilOperation.Keep;
            }

            GraphicsDevice.DepthStencilState = dss;
        }

        private List<VertexPositionNormalTextureHue> GetVertexList(Texture2D texture, Techniques effect)
        {
            List<VertexPositionNormalTextureHue> vertexList;
            if (m_DrawQueue[(int)effect].ContainsKey(texture))
            {
                vertexList = m_DrawQueue[(int)effect][texture];
            }
            else
            {
                if (m_VertexListQueue.Count > 0)
                {
                    vertexList = m_VertexListQueue.Dequeue();
                    vertexList.Clear();
                }
                else
                {
                    vertexList = new List<VertexPositionNormalTextureHue>(1024);
                }
                m_DrawQueue[(int)effect].Add(texture, vertexList);
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
