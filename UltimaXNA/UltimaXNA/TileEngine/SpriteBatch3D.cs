/***************************************************************************
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

namespace UltimaXNA.TileEngine
{
    class SpriteBatch3D : GameComponent
    {
        private Dictionary<Texture2D, List<VertexPositionNormalTextureHue>> m_DrawQueue;
        //private BasicEffect m_Effect2;
        private Effect m_Effect;
        private short[] m_IndexBuffer;
        private Queue<List<VertexPositionNormalTextureHue>> m_VertexListQueue;
        private Matrix m_WorldMatrix;
        private BoundingBox m_BoundingBox;

        public Matrix WorldMatrix { get { return m_WorldMatrix; } }

        public SpriteBatch3D(Game game)
            : base(game)
        {
            m_BoundingBox = new BoundingBox(new Vector3(0, 0, Int32.MinValue), new Vector3(this.Game.GraphicsDevice.Viewport.Width, this.Game.GraphicsDevice.Viewport.Height, Int32.MaxValue));
            m_DrawQueue = new Dictionary<Texture2D, List<VertexPositionNormalTextureHue>>(256);
            m_IndexBuffer = CreateIndexBuffer(0x1000);
            m_VertexListQueue = new Queue<List<VertexPositionNormalTextureHue>>(256);

            m_WorldMatrix = Matrix.CreateOrthographicOffCenter(0, this.Game.GraphicsDevice.Viewport.Width, this.Game.GraphicsDevice.Viewport.Height, 0, Int32.MinValue, Int32.MaxValue);

            //m_Effect2 = new BasicEffect(game.GraphicsDevice, null);
            m_Effect = this.Game.Content.Load<Effect>("Shaders/Basic");
            m_Effect.Parameters["world"].SetValue(m_WorldMatrix);

            this.Game.GraphicsDevice.RenderState.AlphaTestEnable = true;
            this.Game.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;
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

        public void Draw(Texture2D texture, VertexPositionNormalTextureHue[] vertices)
        {
            bool draw = false;

            for (int i = 0; i < vertices.Length; i++)
            {
                if (m_BoundingBox.Contains(vertices[i].Position) == ContainmentType.Contains)
                {
                    draw = true;

                    break;
                }
            }

            if (!draw)
            {
                return;
            }

            List<VertexPositionNormalTextureHue> vertexList;

            if (m_DrawQueue.ContainsKey(texture))
            {
                vertexList = m_DrawQueue[texture];
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

                m_DrawQueue.Add(texture, vertexList);
            }

            //int position = vertexList.Count; Poplicola 5/9/2009 - commented out this variable, which is never used.
            for (int i = 0; i < vertices.Length; i++)
            {
                vertexList.Add(vertices[i]);
            }
        }

        public void SetLightDirection(Vector3 nDirection)
        {
            m_Effect.Parameters["lightDirection"].SetValue(nDirection);
        }

        public void Flush()
        {
            this.Game.GraphicsDevice.VertexDeclaration = new VertexDeclaration(this.Game.GraphicsDevice, VertexPositionNormalTextureHue.VertexElements);

            Texture2D iTexture;
            List<VertexPositionNormalTextureHue> iVertexList;

            IEnumerator<KeyValuePair<Texture2D, List<VertexPositionNormalTextureHue>>> keyValuePairs = m_DrawQueue.GetEnumerator();
            m_Effect.CurrentTechnique = m_Effect.Techniques["StandardEffect"];
            
            Game.GraphicsDevice.RenderState.DepthBufferEnable = true;
            Game.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            Game.GraphicsDevice.RenderState.AlphaTestEnable = true;
            Game.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.None;
            Game.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.None;
            Game.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.None;
            m_Effect.Parameters["DrawLighting"].SetValue(true);
            m_Effect.Begin();
            m_Effect.CurrentTechnique.Passes[0].Begin();

            this.Game.GraphicsDevice.Textures[1] = UltimaXNA.Data.HuesXNA.HueTexture;

            while (keyValuePairs.MoveNext())
            {
                iTexture = keyValuePairs.Current.Key;
                iVertexList = keyValuePairs.Current.Value;
                this.Game.GraphicsDevice.Textures[0] = iTexture;
                this.Game.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTextureHue>(PrimitiveType.TriangleList, iVertexList.ToArray(), 0, iVertexList.Count, m_IndexBuffer, 0, iVertexList.Count / 2);
                m_VertexListQueue.Enqueue(iVertexList);
            }

            m_Effect.CurrentTechnique.Passes[0].End();
            m_Effect.End();

            m_DrawQueue.Clear();
        }
    }
}