using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.TileEngine
{
    public class MapObjectDeferred : MapObject
    {
        private VertexPositionNormalTextureHue[] _verticesParent;
        private VertexPositionNormalTextureHue[] _verticesMine;
        private MapObject _parent;
        public MapObjectDeferred(MapObject parent, VertexPositionNormalTextureHue[] verts)
            : base(parent.Position)
        {
            _parent = parent;
            _verticesParent = getVerticesFromBuffer();
            _verticesMine = getVerticesFromBuffer();
            for (int i = 0; i < 4; i++)
                _verticesParent[i] = verts[i];
        }

        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            // We need to map these ... to this.
            //  0---1     2   0            0
            //     /      |\  |           / \
            //   /        |  \|          2---1
            //  2---3     3   1           \ /
            // (normal) (flipped)          3

            if (_parent._draw_flip)
                for (int i = 0; i < 4; i++)
                    _verticesMine[i].TextureCoordinate.X = 1f - _verticesMine[i].TextureCoordinate.X;
            _verticesMine[0].Normal = _verticesMine[1].Normal = _verticesMine[2].Normal = _verticesMine[3].Normal = _verticesParent[0].Normal;
            _verticesMine[0].Hue = _verticesMine[1].Hue = _verticesMine[2].Hue = _verticesMine[3].Hue = _verticesParent[0].Hue;
            sb.Draw(_parent._draw_texture, _verticesMine);
            return false;
        }

        public void Setup_New(Vector3 drawPosition)
        {
            // Parent vertices ...
            //  0---1     2   0     
            //     /      |\  |    
            //   /        |  \|     
            //  2---3     3   1   
            // (normal) (flipped) 
            
            _verticesMine[0].Normal = _verticesMine[1].Normal = _verticesMine[2].Normal = _verticesMine[3].Normal = _verticesParent[0].Normal;
            _verticesMine[0].Hue = _verticesMine[1].Hue = _verticesMine[2].Hue = _verticesMine[3].Hue = _verticesParent[0].Hue;

            _verticesMine[0].Position = new Vector3(drawPosition.X, _verticesParent[0].Position.Y, 0f);
            _verticesMine[1].Position = new Vector3(drawPosition.X + 44, _verticesParent[0].Position.Y, 0f);
            _verticesMine[2].Position = new Vector3(drawPosition.X, _verticesParent[3].Position.Y, 0f);
            _verticesMine[3].Position = new Vector3(drawPosition.X + 44, _verticesParent[3].Position.Y, 0f);

            float uLeft, uRight;
            if (_parent._draw_flip)
            {
                uLeft = (_verticesMine[0].Position.X - _verticesParent[2].Position.X) / _parent._draw_width;
                uRight = (_verticesMine[1].Position.X - _verticesParent[2].Position.X) / _parent._draw_width;
            }
            else
            {
                uLeft = (_verticesMine[0].Position.X - _verticesParent[2].Position.X) / _parent._draw_width;
                uRight = (_verticesMine[1].Position.X - _verticesParent[2].Position.X) / _parent._draw_width;
            }

            _verticesMine[0].TextureCoordinate = new Vector3(uLeft, 0f, 0f);
            _verticesMine[1].TextureCoordinate = new Vector3(uRight, 0f, 0f);
            _verticesMine[2].TextureCoordinate = new Vector3(uLeft, 1f, 0f);
            _verticesMine[3].TextureCoordinate = new Vector3(uRight, 1f, 0f);

            /*for (int i = 0; i < 4; i++)
                _verticesMine[i].TextureCoordinate = new Vector3(.5f, .5f, 0f);*/
        }

        public void Setup(VertexPositionNormalTextureHue[] verts)
        {
            SortZ = _parent.SortZ;
            SortThreshold = _parent.SortThreshold;
            SortTiebreaker = _parent.SortTiebreaker;

            for (int i = 0; i < 4; i++)
                _verticesMine[i] = verts[i];

            Vector3 tlOrigin = (_parent._draw_flip) ? _verticesParent[2].Position : _verticesParent[0].Position;
            _verticesMine[0].TextureCoordinate = new Vector3(
                (verts[0].Position.X - tlOrigin.X) / _parent._draw_texture.Width,
                (verts[0].Position.Y - tlOrigin.Y) / _parent._draw_texture.Height, 0);
            _verticesMine[1].TextureCoordinate = new Vector3(
                (verts[1].Position.X - tlOrigin.X) / _parent._draw_texture.Width,
                (verts[1].Position.Y - tlOrigin.Y) / _parent._draw_texture.Height, 0);
            _verticesMine[2].TextureCoordinate = new Vector3(
                (verts[2].Position.X - tlOrigin.X) / _parent._draw_texture.Width,
                (verts[2].Position.Y - tlOrigin.Y) / _parent._draw_texture.Height, 0);
            _verticesMine[3].TextureCoordinate = new Vector3(
                (verts[3].Position.X - tlOrigin.X) / _parent._draw_texture.Width,
                (verts[3].Position.Y - tlOrigin.Y) / _parent._draw_texture.Height, 0);
        }

        public void Dispose()
        {
            releaseVerticesToBuffer(_verticesParent);
            releaseVerticesToBuffer(_verticesMine);
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
