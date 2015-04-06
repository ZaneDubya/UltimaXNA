/***************************************************************************
 *   VectorRenderer.cs
 *   
 *   Based on LineBatch.cs, made available as part of the 
 *   Microsoft XNA Community Game Platform
 *   
 *   Copyright (C) Microsoft Corporation. All rights reserved.
 *   
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Core.Rendering
{
    /// <summary>
    /// Batches line "draw" calls from the game, and renders them at one time.
    /// </summary>
    public class VectorRenderer
    {
        public const float Pi = (float)Math.PI;

        GraphicsDevice m_graphics;
        ContentManager m_content;
        Effect m_effect;

        const int m_maxPrimitives = 0x1000;
        VertexPositionColorTexture[] vertices;
        Texture2D m_texture;
        short[] m_indices;
        int currentIndex;
        int lineCount;

        public VectorRenderer(GraphicsDevice g, ContentManager c)
        {
            m_graphics = g;
            m_content = c;
            m_effect = m_content.Load<Effect>("Shaders/VectorRenderer");

            Color[] data = new Color[] { Color.White };
            m_texture = new Texture2D(m_graphics, 1, 1);
            m_texture.SetData<Color>(data);

            // create the vertex and indices array
            vertices = new VertexPositionColorTexture[m_maxPrimitives * 2];
            m_indices = createIndexBuffer(m_maxPrimitives);
            currentIndex = 0;
            lineCount = 0;
        }

        private short[] createIndexBuffer(int primitiveCount)
        {
            short[] indices = new short[primitiveCount * 2];
            for (int i = 0; i < primitiveCount; i++)
            {
                indices[i * 2] = (short)(i * 2);
                indices[i * 2 + 1] = (short)(i * 2 + 1);
            }
            return indices;
        }

        public void DrawDot_World(Vector3 origin, Color color)
        {
            DrawLine(origin + new Vector3(0, -.01f, 0), origin + new Vector3(0, .01f, 0), color);
            DrawLine(origin + new Vector3(-.01f, 0, 0), origin + new Vector3(.01f, 0, 0), color);
        }

        /// <summary>
        /// Draw a line from one point to another with the same color.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="color">The color throughout the line.</param>
        public void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            DrawLine(
                new VertexPositionColorTexture(start, color, new Vector2()),
                new VertexPositionColorTexture(end, color, new Vector2()));
        }


        /// <summary>
        /// Draw a line from one point to another with different colors at each end.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="startColor">The color at the starting point.</param>
        /// <param name="endColor">The color at the ending point.</param>
        public void DrawLine(Vector3 start, Vector3 end, Color startColor, Color endColor)
        {
            DrawLine(
                new VertexPositionColorTexture(start, startColor, new Vector2()),
                new VertexPositionColorTexture(end, endColor, new Vector2()));
        }


        /// <summary>
        /// Draws a line from one vertex to another.
        /// </summary>
        /// <param name="start">The starting vertex.</param>
        /// <param name="end">The ending vertex.</param>
        public void DrawLine(VertexPositionColorTexture start, VertexPositionColorTexture end)
        {
            if (lineCount >= m_maxPrimitives)
                throw new Exception("Raster graphics count has exceeded limit.");

            vertices[currentIndex++] = start;
            vertices[currentIndex++] = end;

            lineCount++;
        }


        /// <summary>
        /// Draws the given polygon.
        /// </summary>
        /// <param name="polygon">The polygon to render.</param>
        /// <param name="color">The color to use when drawing the polygon.</param>
        public void DrawPolygon(VectorPolygon polygon, Color color)
        {
            DrawPolygon(polygon, color, false);
        }

        /// <summary>
        /// Draws the given polygon.
        /// </summary>
        /// <param name="polygon">The polygon to render.</param>
        /// <param name="color">The color to use when drawing the polygon.</param>
        /// <param name="dashed">If true, the polygon will be "dashed".</param>
        public void DrawPolygon(VectorPolygon polygon, Color color, bool dashed)
        {
            if (polygon == null)
                return;

            int step = (dashed == true) ? 2 : 1;
            int length = polygon.Points.Length + ((polygon.IsClosed) ? 0 : -1);
            for (int i = 0; i < length; i += step)
            {
                if (lineCount >= m_maxPrimitives)
                    throw new Exception("Raster graphics count has exceeded limit.");

                vertices[currentIndex].Position = polygon.Points[i % polygon.Points.Length];
                vertices[currentIndex++].Color = color;
                vertices[currentIndex].Position = polygon.Points[(i + 1) % polygon.Points.Length];
                vertices[currentIndex++].Color = color;
                lineCount++;
            }
        }

        public void DrawCircle(Vector2 origin, float radius, float z, Color color, int numVectors)
        {
            float radiansPerPoint = (Pi * 2) / numVectors;
            Vector3[] points = new Vector3[numVectors];
            for (int i = 0; i < numVectors; i++)
            {
                points[i] = new Vector3(
                    origin.X + (float)Math.Cos(radiansPerPoint * i) * radius,
                    origin.Y + (float)Math.Sin(radiansPerPoint * i) * radius,
                    z);
            }
            DrawPolygon(new VectorPolygon(points, true), color);
        }

        public void DrawCircle(Vector2 origin, float radius, float z, Color color)
        {
            DrawCircle(origin, radius, z, color, 20);
        }

        public void Render_WorldSpace(Vector2 rotation, float zoom)
        {
            // if we don't have any vertices, then we can exit early
            if (currentIndex == 0)
                return;

            float zOffset = (zoom > 1000) ? (zoom - 1000) : 0;

            Matrix projection = Matrix.CreateRotationY(rotation.X) * Matrix.CreateRotationX(rotation.Y) * Matrix.CreateTranslation(0, 0, -zOffset) * SpriteBatch3D.ProjectionMatrixWorld;
            m_effect.Parameters["ProjectionMatrix"].SetValue(projection);
            m_effect.Parameters["WorldMatrix"].SetValue(Matrix.CreateScale(zoom * .99f));
            m_effect.Parameters["Viewport"].SetValue(new Vector2(m_graphics.Viewport.Width, m_graphics.Viewport.Height));

            m_effect.CurrentTechnique.Passes[0].Apply();

            m_graphics.Textures[0] = m_texture;
            m_graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList, vertices, 0, currentIndex, m_indices, 0, lineCount);

            currentIndex = 0;
            lineCount = 0;
        }

        public void Render_ViewportSpace()
        {
            // if we don't have any vertices, then we can exit early
            if (currentIndex == 0)
                return;

            m_graphics.BlendState = BlendState.AlphaBlend;
            m_graphics.DepthStencilState = DepthStencilState.None;
            m_graphics.SamplerStates[0] = SamplerState.PointClamp;
            m_graphics.RasterizerState = RasterizerState.CullNone;

            m_effect.Parameters["ProjectionMatrix"].SetValue(SpriteBatch3D.ProjectionMatrixScreen);
            m_effect.Parameters["WorldMatrix"].SetValue(Matrix.Identity);
            m_effect.Parameters["Viewport"].SetValue(new Vector2(m_graphics.Viewport.Width, m_graphics.Viewport.Height));

            m_effect.CurrentTechnique.Passes[0].Apply();

            m_graphics.Textures[0] = m_texture;
            m_graphics.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.LineList, vertices, 0, currentIndex, m_indices, 0, lineCount);

            currentIndex = 0;
            lineCount = 0;
        }

        

    }
}
