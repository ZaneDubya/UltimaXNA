using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.Display
{
    public enum FontJustification
    {
        Left = 0,
        Center = 1,
        Right = 2,
        Top = 0,
        CenterVertically = 4,
        Bottom = 8
    }

    partial class YSpriteBatch
    {
        public void DrawString(YSpriteFont font, string text, Vector2 position, float font_size, 
            Vector2? area = null, Color? color = null, FontJustification justification = FontJustification.Left)
        {
            position.X = (int)position.X;
            position.Y = (int)position.Y + font.Line0VerticalOffset;
            Color c = (color.HasValue) ? color.Value : new Color(1,1,1,1f);
            Vector2 a = (area.HasValue ? area.Value : Vector2.Zero);
            InternalBeginDrawString(font.Texture);
            font.DrawInto(this, text, position, c, a, justification, font_size);
            InternalEndDrawString();
        }

        private void InternalBeginDrawString(Texture2D texture)
        {
            if (m_DrawString_InProgress)
                Logging.Fatal("EndDrawString() must be called before another BeginDrawString() can be called.");

            m_DrawString_InProgress = true;
            m_DrawString_Texture = texture;
            m_DrawString_VertexList = InternalGetVertexList();
            m_DrawString_Depth = m_zOffset++;
        }

        private void InternalEndDrawString()
        {
            if (!m_DrawString_InProgress)
                Logging.Fatal("BeginDrawString() must be called before EndDrawString()");

            m_DrawCommands.Add(new TextureAndVertexList(m_DrawString_Texture, m_DrawString_VertexList));

            m_DrawString_InProgress = false;
            m_DrawString_Texture = null;
            m_DrawString_VertexList = null;
        }

        private bool m_DrawString_InProgress = false;
        private Texture2D m_DrawString_Texture;
        private List<VertexPositionTextureHueExtra> m_DrawString_VertexList;
        private float m_DrawString_Depth;

        internal void DrawSpriteGlyph(Texture2D texture, Vector4 dest, Vector4 source, Color color)
        {
            if (!m_DrawString_InProgress)
                Logging.Fatal("BeginDrawString() must be called before DrawSpriteGlyph()");

            Vector4 uv = new Vector4(
                (float)source.X / texture.Width,
                (float)source.Y / texture.Height,
                (float)(source.X + source.Z) / texture.Width,
                (float)(source.Y + source.W) / texture.Height);

            VertexPositionTextureHueExtra[] v = new VertexPositionTextureHueExtra[4]
            {
                new VertexPositionTextureHueExtra(new Vector3(dest.X, dest.Y, m_DrawString_Depth), new Vector2(uv.X, uv.Y), color, Vector4.Zero), // top left
                new VertexPositionTextureHueExtra(new Vector3(dest.X + dest.Z, dest.Y, m_DrawString_Depth), new Vector2(uv.Z, uv.Y), color, Vector4.Zero), // top right
                new VertexPositionTextureHueExtra(new Vector3(dest.X, dest.Y + dest.W, m_DrawString_Depth), new Vector2(uv.X, uv.W), color, Vector4.Zero), // bottom left
                new VertexPositionTextureHueExtra(new Vector3(dest.X + dest.Z, dest.Y + dest.W, m_DrawString_Depth), new Vector2(uv.Z, uv.W), color, Vector4.Zero) // bottom right
            };

            /*if (shadow != null)
            {
                Color shadow2 = new Color(
                    shadow.Value.R, shadow.Value.G,
                    shadow.Value.B, 128);
                for (int i = 0; i < 4; i++)
                {
                    VertexPositionTextureHueExtra v0 = v[i];
                    v0.Hue = shadow.Value;
                    v0.Position.Y += 1f;
                    m_DrawString_VertexList.Add(v0);
                }
            }*/
            for (int i = 0; i < 4; i++)
                m_DrawString_VertexList.Add(v[i]);
        }

        internal void DrawSpriteGlyphDF(Texture2D texture, Vector4 dest, Vector4 source, Color color, float font_scale)
        {
            if (!m_DrawString_InProgress)
                Logging.Fatal("BeginDrawString() must be called before DrawSpriteGlyph()");

            Vector4 uv = new Vector4(
                (float)source.X / texture.Width,
                (float)source.Y / texture.Height,
                (float)(source.X + source.Z) / texture.Width,
                (float)(source.Y + source.W) / texture.Height);

            float smoothing = 0.4f * Math.Pow(0.333f, font_scale * 2f) + 0.05f;
            Vector4 extra = new Vector4(0, 0, smoothing, 2);

            VertexPositionTextureHueExtra[] v = new VertexPositionTextureHueExtra[4]
            {
                new VertexPositionTextureHueExtra(new Vector3(dest.X, dest.Y, m_DrawString_Depth), new Vector2(uv.X, uv.Y), color, extra), // top left
                new VertexPositionTextureHueExtra(new Vector3(dest.X + dest.Z, dest.Y, m_DrawString_Depth), new Vector2(uv.Z, uv.Y), color, extra), // top right
                new VertexPositionTextureHueExtra(new Vector3(dest.X, dest.Y + dest.W, m_DrawString_Depth), new Vector2(uv.X, uv.W), color, extra), // bottom left
                new VertexPositionTextureHueExtra(new Vector3(dest.X + dest.Z, dest.Y + dest.W, m_DrawString_Depth), new Vector2(uv.Z, uv.W), color, extra) // bottom right
            };

            for (int i = 0; i < 4; i++)
                m_DrawString_VertexList.Add(v[i]);
        }
    }
}
