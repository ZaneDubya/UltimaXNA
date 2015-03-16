using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.Display
{
    partial class YSpriteBatch
    {
        List<Vector4> m_GUIClipRect_Stack;
        Vector4 m_GUIClipRect = Vector4.Zero;

        public Rectangle GUIClipRect
        {
            set
            {
                m_GUIClipRect = new Vector4(value.X, value.Y, value.Right, value.Bottom);
            }
        }

        public void GUIClipRect_Push(Rectangle value)
        {
            if (m_GUIClipRect_Stack == null)
                m_GUIClipRect_Stack = new List<Vector4>();
            m_GUIClipRect_Stack.Add(m_GUIClipRect);
            m_GUIClipRect = new Vector4(value.X, value.Y, value.Right, value.Bottom);
        }

        public void GUIClipRect_Pop()
        {
            if (m_GUIClipRect_Stack == null || m_GUIClipRect_Stack.Count == 0)
                return;
            m_GUIClipRect = m_GUIClipRect_Stack[m_GUIClipRect_Stack.Count - 1];
            m_GUIClipRect_Stack.RemoveAt(m_GUIClipRect_Stack.Count - 1);
        }

        public void ResetGUIClipRect()
        {
            if (!(m_GUIClipRect_Stack == null))
            {
                while (m_GUIClipRect_Stack.Count > 0)
                    GUIClipRect_Pop();
            }

            m_GUIClipRect = new Vector4(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        public void GUIDrawSprite(Texture2D texture, Rectangle destinationRectangle,
            Rectangle? sourceRectangle = null, Color? color = null, SpriteEffects effects = SpriteEffects.None)
        {
            if (texture == null)
                return;
            Vector4 dest = new Vector4(destinationRectangle.X, destinationRectangle.Y, destinationRectangle.Right, destinationRectangle.Bottom);
            Vector4 srcDelta = new Vector4();
            if (sourceRectangle == null)
                sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);

            // clipping
            if (dest.X < m_GUIClipRect.X)
            {
                if (dest.Z < m_GUIClipRect.X)
                    return;
                float delta = m_GUIClipRect.X - dest.X;
                dest.X += delta;
                srcDelta.X = delta / destinationRectangle.Width;
            }
            if (dest.Z > m_GUIClipRect.Z)
            {
                if (dest.X > m_GUIClipRect.Z)
                    return;
                float delta = m_GUIClipRect.Z - dest.Z;
                dest.Z += delta;
                srcDelta.Z = delta / destinationRectangle.Width;
            }
            if (dest.Y < m_GUIClipRect.Y)
            {
                if (dest.W < m_GUIClipRect.Y)
                    return;
                float delta = m_GUIClipRect.Y - dest.Y;
                dest.Y += delta;
                srcDelta.Y = delta / destinationRectangle.Width;
            }
            if (dest.W > m_GUIClipRect.W)
            {
                if (dest.Y > m_GUIClipRect.W)
                    return;
                float delta = m_GUIClipRect.W - dest.W;
                dest.W += delta;
                srcDelta.W = delta / destinationRectangle.Width;
            }

            Vector4 source = new Vector4(
                sourceRectangle.Value.X, sourceRectangle.Value.Y,
                sourceRectangle.Value.Right, sourceRectangle.Value.Bottom);

            if (effects.HasFlag(SpriteEffects.FlipHorizontally))
            {
                float x = source.X;
                source.X = source.Z;
                source.Z = x;
            }
            if (effects.HasFlag(SpriteEffects.FlipVertically))
            {
                float y = source.Y;
                source.Y = source.W;
                source.W = y;
            }

            float width = texture.Width;
            float height = texture.Height;

            source.X /= width;
            source.Y /= height;
            source.Z /= width;
            source.W /= height;

            source.X += srcDelta.X * ((float)sourceRectangle.Value.Width / width);
            source.Y += srcDelta.Y * ((float)sourceRectangle.Value.Height / height);
            source.Z += srcDelta.Z * ((float)sourceRectangle.Value.Width / width);
            source.W += srcDelta.W * ((float)sourceRectangle.Value.Height / height);

            source.Y -= (0.5f / height);
            source.W -= (0.5f / height);

            Shape quad = Shape.CreateQuad(
                new Vector3(destinationRectangle.X, destinationRectangle.Y, 0),
                new Vector2(destinationRectangle.Width, destinationRectangle.Height),
                source, color);

            List<VertexPositionTextureHueExtra> vertexList = InternalGetVertexList();
            for (int i = 0; i < quad.Vertices.Length; i++)
            {
                quad.Vertices[i].Position.Z = m_zOffset;
                vertexList.Add(quad.Vertices[i]);
            }
            // increment z offset for the spritebatch.
            m_zOffset++;

            m_DrawCommands.Add(new TextureAndVertexList(texture, vertexList));
        }
    }
}
