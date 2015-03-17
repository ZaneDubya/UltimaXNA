using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.Display
{
    public partial class YSpriteBatch : DrawableGameComponent
    {
        private short[] m_IndexBuffer;
        private Effect m_SpriteBatchShader;
        public Effect SpriteBatchShader
        {
            set
            {
                m_SpriteBatchShader = value;
            }
        }

        class TextureAndVertexList
        {
            public Texture2D Texture;
            public List<VertexPositionTextureHueExtra> Vertices;

            public TextureAndVertexList(Texture2D texture, List<VertexPositionTextureHueExtra> vertices)
            {
                Texture = texture;
                Vertices = vertices;
            }

            public static TextureAndVertexList Null
            {
                get
                {
                    return new TextureAndVertexList(null, null);
                }
            }

            public override string ToString()
            {
                return Vertices.Count.ToString();
            }
        }

        private List<TextureAndVertexList> m_BatchedDrawCommands;
        private List<TextureAndVertexList> m_DrawCommands;
        private Queue<List<VertexPositionTextureHueExtra>> m_VertexListQueue;

        private float m_zOffset = 0f;
        public float ZOffset { get { return m_zOffset; } set { m_zOffset = value; } }

        private Color m_BackgroundColor = Color.DarkGray;
        public Color BackgroundColor
        {
            get { return m_BackgroundColor; }
            set { m_BackgroundColor = value; }
        }

        public YSpriteBatch(Game game)
            : base(game)
        {
            if (Game.Services.GetService(this.GetType()) != null)
                Logging.Fatal("A SpriteBatchExtended service has already been added.");
            Game.Services.AddService(this.GetType(), this);
        }

        public override void Initialize()
        {
            base.Initialize();

            m_IndexBuffer = InternalCreateIndexBuffer(0x2000);
            m_BatchedDrawCommands = new List<TextureAndVertexList>(1024);
            m_DrawCommands = new List<TextureAndVertexList>(1024);
            m_VertexListQueue = new Queue<List<VertexPositionTextureHueExtra>>(1024);
        }

        public override void Draw(GameTime gameTime)
        {
            // At this point, all the draw commands are listed in m_DrawCommands.
            // We batch them up into m_BatchedDrawCommands, and draw those.
            InternalCreateBatchedDrawCommands();

            // Set up the Graphics Defice
            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            // Set up the shader effect
            Effect effect = m_SpriteBatchShader;
            effect.Parameters["World"].SetValue(Matrix.Identity);
            effect.Parameters["View"].SetValue(Matrix.Identity);
            effect.Parameters["Projection"].SetValue(Library.Projections.ProjectionScreen);
            
            // Clear the screen
            GraphicsDevice.Clear(new Color(40, 40, 40, 255));

            // Draw all the objects.
            InternalDrawPass(effect, 0, true);

            // Clear the batched draw commands and reset Z
            m_BatchedDrawCommands.Clear();
            m_zOffset = 0;

            GraphicsDevice.Textures[0] = null;
            GraphicsDevice.Textures[1] = null;
        }

        private void InternalDrawPass(Effect effect, int pass, bool clear)
        {
            List<VertexPositionTextureHueExtra> iVertexList;
            IEnumerator<TextureAndVertexList> iTexturesVertexes = m_BatchedDrawCommands.GetEnumerator();

            effect.CurrentTechnique.Passes[pass].Apply();

            while (iTexturesVertexes.MoveNext())
            {
                iVertexList = iTexturesVertexes.Current.Vertices;
                GraphicsDevice.Textures[0] = iTexturesVertexes.Current.Texture;
                // effect.Parameters["TextureDimensions"].SetValue(new Vector2(iTexturesVertexes.Current.Texture.Width, iTexturesVertexes.Current.Texture.Height));
                if (iVertexList.Count == 0)
                    continue;
                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionTextureHueExtra>(
                    PrimitiveType.TriangleList, iVertexList.ToArray(), 0,
                    iVertexList.Count, m_IndexBuffer, 0, iVertexList.Count / 2);
                if (clear)
                {
                    iVertexList.Clear();
                    m_VertexListQueue.Enqueue(iVertexList);
                }
            }
        }

        private short[] InternalCreateIndexBuffer(int primitiveCount)
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

        private void InternalCreateBatchedDrawCommands()
        {
            TextureAndVertexList batch = TextureAndVertexList.Null;

            for (int i = 0; i < m_DrawCommands.Count; i++)
            {
                TextureAndVertexList current = m_DrawCommands[i];
                if (current.Texture != batch.Texture)
                {
                    if (batch.Texture != null)
                        m_BatchedDrawCommands.Add(batch);
                    batch = new TextureAndVertexList(current.Texture, InternalGetVertexList());
                }
                for (int j = 0; j < current.Vertices.Count; j++)
                    batch.Vertices.Add(current.Vertices[j]);
                current.Vertices.Clear();
                m_VertexListQueue.Enqueue(current.Vertices);
            }
            if (batch.Texture != null && batch.Vertices.Count > 0)
                m_BatchedDrawCommands.Add(batch);
            m_DrawCommands.Clear();
        }

        private List<VertexPositionTextureHueExtra> InternalGetVertexList()
        {
            List<VertexPositionTextureHueExtra> vertexList;

            if (m_VertexListQueue.Count > 0)
            {
                vertexList = m_VertexListQueue.Dequeue();
                vertexList.Clear();
            }
            else
            {
                vertexList = new List<VertexPositionTextureHueExtra>(1024);
            }

            return vertexList;
        }

        public void DrawShape(Shape shape, Texture2D texture)
        {
            List<VertexPositionTextureHueExtra> vertexList = InternalGetVertexList();

            for (int i = 0; i < shape.Vertices.Length; i++)
            {
                shape.Vertices[i].Position.Z = m_zOffset;
                vertexList.Add(shape.Vertices[i]);
            }

            m_DrawCommands.Add(new TextureAndVertexList(texture, vertexList));

            // increment the z offset
            m_zOffset++;
        }

        public void DrawSprite(Texture2D texture, Vector3 position, Vector2 area, Color? hue = null)
        {
            List<VertexPositionTextureHueExtra> vertexList = InternalGetVertexList();

            Shape quad = Shape.CreateQuad(position, area, new Vector4(0, 0, 1, 1), hue);
            for (int i = 0; i < quad.Vertices.Length; i++)
            {
                quad.Vertices[i].Position.Z = m_zOffset;
                vertexList.Add(quad.Vertices[i]);
            }

            m_DrawCommands.Add(new TextureAndVertexList(texture, vertexList));

            // increment the z offset
            m_zOffset++;
        }

        public void DrawRectangle(Vector3 position, Vector2 area, Color hue)
        {
            // Upper edge
            DrawRectangleFilled(position, 
                new Vector2(area.X - 1, 1), hue);
            // Left edge
            DrawRectangleFilled(position,
                new Vector2(1, area.Y), hue);
            // Bottom edge
            DrawRectangleFilled(position + new Vector3(0, area.Y - 1, 0), 
                new Vector2(area.X - 1, 1), hue);
            // Right edge
            DrawRectangleFilled(position + new Vector3(area.X - 1, 0, 0), 
                new Vector2(1, area.Y), hue);
        }

        Texture2D m_TextureForLines;
        private void createTextureForLines()
        {
            m_TextureForLines = new Texture2D(GraphicsDevice, 1, 1);
            m_TextureForLines.SetData(new uint[] { Color.White.PackedValue });
        }

        public void DrawLine(Vector3 start, Vector3 end, Color hue)
        {
            if (m_TextureForLines == null)
                createTextureForLines();

            Shape line = Shape.CreateLine(start, end, hue);
            DrawShape(line, m_TextureForLines);
        }

        public void DrawRectangleFilled(Vector3 position, Vector2 area, Color hue)
        {
            if (m_TextureForLines == null)
                createTextureForLines();

            position.Z = m_zOffset++;

            // clipping
            if (position.X < m_GUIClipRect.X)
            {
                if (position.X + area.X < m_GUIClipRect.X)
                    return;
                float delta = m_GUIClipRect.X - position.X;
                position.X += delta;
            }
            if (position.Y < m_GUIClipRect.Y)
            {
                if (position.Y + area.Y < m_GUIClipRect.Y)
                    return;
                float delta = m_GUIClipRect.Y - position.Y;
                position.Y += delta;
                area.Y -= delta;
            }
            if (position.X + area.X > m_GUIClipRect.Z)
            {
                if (position.X > m_GUIClipRect.Z)
                    return;
                float delta = m_GUIClipRect.Z - (position.X + area.X);
                area.X += delta;
            }
            if (position.Y + area.Y > m_GUIClipRect.W)
            {
                if (position.Y > m_GUIClipRect.W)
                    return;
                float delta = m_GUIClipRect.W - (position.Y + area.Y);
                area.Y += delta;
            }

            DrawSprite(m_TextureForLines, position, area, hue);
        }
    }
}
