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
                Z = SortZ = (int)_position.Point_V3.Z;
            }
        }
        public int Tiebreaker = 0;
        public Entity OwnerEntity = null;
        public int Threshold = 0;
        public int SortZ = 0;
        public int Z = 0;
        public int ItemID = 0;

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
        internal Vector2 _draw_hue; // x is the hue. y = 0, no hue. y = 1, total hue.  y = 2, partial hue.
        internal Texture2D _draw_texture;
        internal PickTypes _pickType;

        internal virtual bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            VertexPositionNormalTextureHue[] vectorBuffer;

            if (Z >= maxAlt)
                return false;

            if (_draw_flip)
            {
                vectorBuffer = VertexPositionNormalTextureHue.PolyBufferFlipped;
                vectorBuffer[0].Position = drawPosition;
                vectorBuffer[0].Position.X += _draw_X + 44;
                vectorBuffer[0].Position.Y -= _draw_Y;

                vectorBuffer[1].Position = vectorBuffer[0].Position;
                vectorBuffer[1].Position.Y += _draw_height;

                vectorBuffer[2].Position = vectorBuffer[0].Position;
                vectorBuffer[2].Position.X -= _draw_width;

                vectorBuffer[3].Position = vectorBuffer[1].Position;
                vectorBuffer[3].Position.X -= _draw_width;
            }
            else
            {
                vectorBuffer = VertexPositionNormalTextureHue.PolyBuffer;
                vectorBuffer[0].Position = drawPosition;
                vectorBuffer[0].Position.X -= _draw_X;
                vectorBuffer[0].Position.Y -= _draw_Y;

                vectorBuffer[1].Position = vectorBuffer[0].Position;
                vectorBuffer[1].Position.X += _draw_width;

                vectorBuffer[2].Position = vectorBuffer[0].Position;
                vectorBuffer[2].Position.Y += _draw_height;

                vectorBuffer[3].Position = vectorBuffer[1].Position;
                vectorBuffer[3].Position.Y += _draw_height;
            }

            if (vectorBuffer[0].Hue != _draw_hue)
            {
                vectorBuffer[0].Hue =
                vectorBuffer[1].Hue =
                vectorBuffer[2].Hue =
                vectorBuffer[3].Hue = _draw_hue;
            }

            if (!sb.Draw(_draw_texture, vectorBuffer))
                return false;

            if ((pickType & _pickType) == _pickType)
            {
                if (((!_draw_flip) && molist.IsMouseInObject(vectorBuffer[0].Position, vectorBuffer[3].Position)) ||
                    ((_draw_flip) && molist.IsMouseInObject(vectorBuffer[2].Position, vectorBuffer[1].Position)))
                {
                    MouseOverItem item;
                    if (!_draw_flip)
                    {
                        item = new MouseOverItem(_draw_texture, vectorBuffer[0].Position, this);
                        item.Vertices = new Vector3[4] { vectorBuffer[0].Position, vectorBuffer[1].Position, vectorBuffer[2].Position, vectorBuffer[3].Position };
                    }
                    else
                    {
                        item = new MouseOverItem(_draw_texture, vectorBuffer[2].Position, this);
                        item.Vertices = new Vector3[4] { vectorBuffer[2].Position, vectorBuffer[0].Position, vectorBuffer[3].Position, vectorBuffer[1].Position };
                    }
                    molist.Add2DItem(item);
                }
            }

            return true;
        }
    }
}
