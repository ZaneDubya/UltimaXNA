using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.TileEngine
{
    public class MapObject
    {
        private Position3D _position;
        public Position3D Position
        {
            get { return _position; }
            set
            {
                _position = value;
                Z = SortZ = (int)_position.Tile_V3.Z;
            }
        }
        public Entity OwnerEntity = null;
        public int Z = 0;
        public int ItemID = 0;

        public int SortZ = 0;           // This is the default sort value of the object.
        public int SortThreshold = 0;   // This is a sort value which should be set based on the type of object.
        public int SortTiebreaker = 0;  // This is a sort value which is used to sort layers of a single object.

        public Serial OwnerSerial
        {
            get { return (OwnerEntity == null) ? (Serial)unchecked((int)0) : OwnerEntity.Serial; }
        }

        public MapObject(Position3D position)
        {
            Position = position;
        }

        internal bool _draw_flip;
        internal int _draw_X, _draw_Y;
        internal int _draw_width, _draw_height;
        internal Vector2 _draw_hue; // x is the hue. y = 0, no hue. y = 1, total hue.  y = 2, partial hue. y = 4 is a 50% transparency bitflag.
        internal Texture2D _draw_texture;
        internal PickTypes _pickType;
        internal bool _draw_IsometricOverlap = false; // if this is true, we will draw any corners that are overlapped by tiles drawn after this object.
        public int DrawHeight { get { return _draw_height; } }

        internal virtual bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            VertexPositionNormalTextureHue[] vertexBuffer;

            if (Z >= maxAlt)
                return false;

            if (_draw_flip)
            {
                // 2   0    
                // |\  |     
                // |  \|     
                // 3   1
                vertexBuffer = VertexPositionNormalTextureHue.PolyBufferFlipped;
                vertexBuffer[0].Position = drawPosition;
                vertexBuffer[0].Position.X += _draw_X + 44;
                vertexBuffer[0].Position.Y -= _draw_Y;

                vertexBuffer[1].Position = vertexBuffer[0].Position;
                vertexBuffer[1].Position.Y += _draw_height;

                vertexBuffer[2].Position = vertexBuffer[0].Position;
                vertexBuffer[2].Position.X -= _draw_width;

                vertexBuffer[3].Position = vertexBuffer[1].Position;
                vertexBuffer[3].Position.X -= _draw_width;
            }
            else
            {
                // 0---1    
                //    /     
                //  /       
                // 2---3
                vertexBuffer = VertexPositionNormalTextureHue.PolyBuffer;
                vertexBuffer[0].Position = drawPosition;
                vertexBuffer[0].Position.X -= _draw_X;
                vertexBuffer[0].Position.Y -= _draw_Y;

                vertexBuffer[1].Position = vertexBuffer[0].Position;
                vertexBuffer[1].Position.X += _draw_width;

                vertexBuffer[2].Position = vertexBuffer[0].Position;
                vertexBuffer[2].Position.Y += _draw_height;

                vertexBuffer[3].Position = vertexBuffer[1].Position;
                vertexBuffer[3].Position.Y += _draw_height;
            }

            if (vertexBuffer[0].Hue != _draw_hue)
                vertexBuffer[0].Hue = vertexBuffer[1].Hue = vertexBuffer[2].Hue = vertexBuffer[3].Hue = _draw_hue;
            
            // if (!(this is MapObjectMobile))
            if (!sb.Draw(_draw_texture, vertexBuffer))
            {
                return false;
            }
            
            if (_draw_IsometricOverlap)
            {
                drawIsometricOverlap(vertexBuffer, new Vector2(drawPosition.X, drawPosition.Y - (Z << 2)), _draw_texture, _draw_flip);
            }
            
            if ((pickType & _pickType) == _pickType)
            {
                if (((!_draw_flip) && molist.IsMouseInObject(vertexBuffer[0].Position, vertexBuffer[3].Position)) ||
                    ((_draw_flip) && molist.IsMouseInObject(vertexBuffer[2].Position, vertexBuffer[1].Position)))
                {
                    MouseOverItem item;
                    if (!_draw_flip)
                    {
                        item = new MouseOverItem(_draw_texture, vertexBuffer[0].Position, this);
                        item.Vertices = new Vector3[4] { vertexBuffer[0].Position, vertexBuffer[1].Position, vertexBuffer[2].Position, vertexBuffer[3].Position };
                    }
                    else
                    {
                        item = new MouseOverItem(_draw_texture, vertexBuffer[2].Position, this);
                        item.Vertices = new Vector3[4] { vertexBuffer[2].Position, vertexBuffer[0].Position, vertexBuffer[3].Position, vertexBuffer[1].Position };
                    }
                    molist.Add2DItem(item);
                }
            }

            return true;
        }

        private void drawIsometricOverlap(VertexPositionNormalTextureHue[] vertices, Vector2 screenPosition, Texture2D texture, bool flip)
        {
            int startX, endX, endXalternative;
            if (flip)
            {
                startX = (int)Math.Ceiling((screenPosition.X - vertices[2].Position.X) / 22);
                endX = (int)Math.Ceiling((_draw_X - 22) / 22f);
                endXalternative = 0;
            }
            else
            {
                startX = (int)Math.Ceiling((screenPosition.X - vertices[0].Position.X) / 22);
                endX = (int)Math.Ceiling((vertices[1].Position.X - vertices[0].Position.X - 44 - _draw_X) / 22f);
                endXalternative = (int)Math.Ceiling((_draw_height - (_draw_Y - 44)) / 22f);
            }

            if (endXalternative > endX)
                endX = endXalternative;

            int x = 0;
            for (int y = 1; y <= (startX + x + 1); y++)
            {
                MapTile tile = IsometricRenderer.InternalMap.GetMapTile(_position.X + x, _position.Y + y, false);
                if (tile != null)
                {
                    if (tile.HasWallAtZ(Z))
                        break;
                    else
                    {
                        MapObjectDeferred deferred = new MapObjectDeferred(this, vertices);
                        tile.AddMapObject_Deferred(deferred);
                    }
                }
            }
            
            for (x = 1; x <= endX; x++)
            {
                int yBounds = (endX + 1 - x);
                for (int y = -yBounds; y <= yBounds; y++)
                {
                    MapTile tile = IsometricRenderer.InternalMap.GetMapTile(_position.X + x, _position.Y + y, false);
                    if (tile.HasWallAtZ(Z))
                        break;
                    if (tile != null)
                    {
                        MapObjectDeferred deferred = new MapObjectDeferred(this, vertices);
                        tile.AddMapObject_Deferred(deferred);
                    }
                }
            }
        }
    }
}