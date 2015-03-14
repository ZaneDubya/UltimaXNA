/***************************************************************************
 *   MapObjectDeferred.cs
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *  
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Rendering;

namespace UltimaXNA.UltimaWorld.View
{
    public class MapObjectDeferred : AMapObject
    {
        private Texture2D m_texture;
        public VertexPositionNormalTextureHue[] Vertices;
        public MapObjectDeferred(Texture2D texture, AMapObject parent)
            : base(new Position3D(parent.Position.X, parent.Position.Y, (int)parent.Z))
        {
            m_texture = texture;
            Vertices = getVerticesFromBuffer();
            SortZ = parent.SortZ;
            SortThreshold = parent.SortThreshold;
            SortTiebreaker = parent.SortTiebreaker;
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            sb.Draw(m_texture, Vertices);
            return false;
        }

        public void Dispose()
        {
            releaseVerticesToBuffer(Vertices);
        }

        public override string ToString()
        {
            return string.Format("Deferred texture from tile ({0},{1})", Position.X, Position.Y);
        }

        private static List<VertexPositionNormalTextureHue[]> deferredVertexBuffer;
        private const int kDeferredVertexBufferCount = 1000;
        private static VertexPositionNormalTextureHue[] getVerticesFromBuffer()
        {
            if (deferredVertexBuffer == null)
            {
                deferredVertexBuffer = new List<VertexPositionNormalTextureHue[]>();
                for (int i = 0; i < kDeferredVertexBufferCount; i++)
                {
                    deferredVertexBuffer.Add(new VertexPositionNormalTextureHue[4]);
                }
            }
            if (deferredVertexBuffer.Count == 0)
                deferredVertexBuffer.Add(new VertexPositionNormalTextureHue[4]);
            VertexPositionNormalTextureHue[] v = deferredVertexBuffer[deferredVertexBuffer.Count - 1];
            deferredVertexBuffer.RemoveAt(deferredVertexBuffer.Count - 1);
            return v;
        }

        private static void releaseVerticesToBuffer(VertexPositionNormalTextureHue[] v)
        {
            deferredVertexBuffer.Add(v);
        }
    }
}
