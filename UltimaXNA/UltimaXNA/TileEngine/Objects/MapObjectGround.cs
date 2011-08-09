/***************************************************************************
 *   MapObjectGround.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.TileEngine
{
    public class MapObjectGround : MapObject
    {
        public bool _mustUpdateSurroundings = true;
        private Surroundings _surroundingTiles;
        private bool _noDraw;
        private Vector3[] _normals;

        public bool Ignored
        {
            get { return (ItemID == 2 || ItemID == 0x1DB || (ItemID >= 0x1AE && ItemID <= 0x1B5)); }
        }

        public override string ToString()
        {
            return string.Format("Ground Z:{0}, SortZ:{4}, <{1},{2},{3}>", Z, _surroundingTiles.South, _surroundingTiles.Down, _surroundingTiles.East, SortZ);
        }

        public MapObjectGround(int tileID, Position3D position)
            : base(position)
        {
            ItemID = tileID;
            _normals = new Vector3[4];
            SortThreshold = -1;
            SortTiebreaker = -1;

            // set no draw flag
            _noDraw = (ItemID < 3 || (ItemID >= 0x1AF && ItemID <= 0x1B5));

            // get draw data
            Data.LandData landData = Data.TileData.LandData[ItemID & 0x3FFF];
            if (landData.TextureID <= 0)
            {
                _draw_3DStretched = false;
                _draw_texture = Data.Art.GetLandTexture(ItemID);
                _draw_width = _draw_height = 44;
                _draw_X = 0;
                _draw_Y = (Z << 2);
                _draw_hue = Vector2.Zero;
                _pickType = PickTypes.PickGroundTiles;
                _draw_flip = false;
            }
            else
            {
                _draw_3DStretched = true;
                _draw_texture = Data.Texmaps.GetTexmapTexture(landData.TextureID);
            }

            // set pick type
            _pickType = PickTypes.PickGroundTiles;
        }

        private bool _draw_3DStretched;
        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            if (_noDraw || _mustUpdateSurroundings || !IsometricRenderer.DrawTerrain)
                return false;
            if (!_draw_3DStretched)
                return base.Draw(sb, drawPosition, molist, pickType, 255);
            else
                return Draw3DStretched(sb, drawPosition, molist, pickType, 255);
        }

        VertexPositionNormalTextureHue[] _vertexBufferAlternate = new VertexPositionNormalTextureHue[] {
                    new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 0, 0)),
                    new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 0, 0)),
                    new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 1, 0)),
                    new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 1, 0))
                };

        private Vector3 _vertex0_yOffset, _vertex1_yOffset, _vertex2_yOffset, _vertex3_yOffset;

        private bool Draw3DStretched(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            // this is an isometric stretched tile and needs a specialized draw routine.
            _vertexBufferAlternate[0].Position = drawPosition + _vertex0_yOffset;
            _vertexBufferAlternate[1].Position = drawPosition + _vertex1_yOffset;
            _vertexBufferAlternate[2].Position = drawPosition + _vertex2_yOffset;
            _vertexBufferAlternate[3].Position = drawPosition + _vertex3_yOffset;

            if (!sb.Draw(_draw_texture, _vertexBufferAlternate))
                return false;

            if (((pickType & _pickType) == _pickType) || ClientVars.DEBUG_HighlightMouseOverObjects)
                if (molist.IsMouseInObjectIsometric(_vertexBufferAlternate))
                {
                    MouseOverItem item = new MouseOverItem(_draw_texture, _vertexBufferAlternate[0].Position, this);
                    item.Vertices = new Vector3[4] { _vertexBufferAlternate[0].Position, _vertexBufferAlternate[1].Position, _vertexBufferAlternate[2].Position, _vertexBufferAlternate[3].Position };
                    molist.Add2DItem(item);
                }

            return true;
        }

        public void FlushSurroundings()
        {
            _mustUpdateSurroundings = true;
        }

        public void UpdateSurroundingsIfNecessary(Map m)
        {
            if (!_mustUpdateSurroundings)
                return;

            int x = (int)Position.X;
            int y = (int)Position.Y;

            int[] zValues = new int[16];

            for (int iy = 0; iy < 4; iy++)
                for (int ix = 0; ix < 4; ix++)
                    zValues[ix + iy * 4] = m.GetTileZ(x + ix - 1, y + iy - 1);

            _surroundingTiles = new Surroundings(
                zValues[2 + 2 * 4],
                zValues[2 + 1 * 4],
                zValues[1 + 2 * 4]);

            bool isFlat = _surroundingTiles.IsFlat && _surroundingTiles.East == Z;
            if (!isFlat)
            {
                int low = 0, high = 0, sort = 0;
                sort = m.GetAverageZ(Z, _surroundingTiles.South, _surroundingTiles.East, _surroundingTiles.Down, ref low, ref high);
                if (sort != SortZ)
                {
                    SortZ = sort;
                    m.GetMapTile(x, y, false).Resort();
                }
            }

            calculateNormals(
                zValues[0 + 1 * 4],
                zValues[0 + 2 * 4],
                zValues[1 + 0 * 4],
                zValues[2 + 0 * 4],
                zValues[1 + 3 * 4],
                zValues[2 + 3 * 4],
                zValues[3 + 1 * 4],
                zValues[3 + 2 * 4]);

            updateVertexBuffer();
            _mustUpdateSurroundings = false;
        }

        private void updateVertexBuffer()
        {
            _vertex0_yOffset = new Vector3(22, -(Z << 2), 0);
            _vertex1_yOffset = new Vector3(44, 22 - (_surroundingTiles.East << 2), 0);
            _vertex2_yOffset = new Vector3(0, 22 - (_surroundingTiles.South << 2), 0);
            _vertex3_yOffset = new Vector3(22, 44 - (_surroundingTiles.Down << 2), 0);

            _vertexBufferAlternate[0].Normal = _normals[0];
            _vertexBufferAlternate[1].Normal = _normals[1];
            _vertexBufferAlternate[2].Normal = _normals[2];
            _vertexBufferAlternate[3].Normal = _normals[3];

            if (_vertexBufferAlternate[0].Hue != _draw_hue)
            {
                _vertexBufferAlternate[0].Hue =
                _vertexBufferAlternate[1].Hue =
                _vertexBufferAlternate[2].Hue =
                _vertexBufferAlternate[3].Hue = _draw_hue;
            }
        }

        private void calculateNormals(
            int NorthWest0, int NorthWest2, int NorthEast0, int NorthEast1,
            int SouthWest2, int SouthWest3, int SouthEast1, int SouthEast3)
        {
            _normals[0] = calculateNormal(
                NorthWest0, _surroundingTiles.East,
                NorthEast0, _surroundingTiles.South);
            _normals[1] = calculateNormal(
                this.Z, SouthEast1,
                NorthEast1, _surroundingTiles.Down);
            _normals[2] = calculateNormal(
                NorthWest2, _surroundingTiles.Down,
                this.Z, SouthWest2);
            _normals[3] = calculateNormal(
                _surroundingTiles.South, SouthEast3,
                _surroundingTiles.East, SouthWest3);
        }

        private Vector3 calculateNormal(float A, float B, float C, float D)
        {
            Vector3 iVector = new Vector3(
                (A - B) / 2f,
                1f,
                (C - D) / 2f);
            iVector.Normalize();
            return iVector;
        }
    }

    public class Surroundings
    {
        public int Down;
        public int East;
        public int South;

        public Surroundings(int nDown, int nEast, int nSouth)
        {
            Down = nDown;
            East = nEast;
            South = nSouth;
        }

        public bool IsFlat
        {
            get
            {
                return (Down == East && East == South);
            }
        }
    }
}