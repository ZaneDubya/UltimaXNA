/***************************************************************************
 *   MapObjectGround.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Entity;
using UltimaXNA.Interface.Graphics;
#endregion

namespace UltimaXNA.Interface.TileEngine
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
                _draw_Y = (int)(Z * 4);
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

            if ((pickType & _pickType) == _pickType)
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

        public void UpdateSurroundingsIfNecessary(Map map)
        {
            if (!_mustUpdateSurroundings)
                return;

            updateSurroundingsAndNormals(map);
            _mustUpdateSurroundings = false;
        }

        static Point[] kSurroundingsIndexes = new Point[11] { 
            new Point(0, -1), new Point(1, -1), 
            new Point(-1, 0), new Point(1, 0), new Point(2, 0), 
            new Point(-1, 1), new Point(0, 1), new Point(1, 1), new Point(2, 1), 
            new Point(0, 2), new Point(1, 2) };

        private void updateSurroundingsAndNormals(Map map)
        {
            Point origin = new Point(Position.X, Position.Y);

            float[] surroundingTilesZ = new float[kSurroundingsIndexes.Length];
            for (int i = 0; i < kSurroundingsIndexes.Length; i++)
                surroundingTilesZ[i] = map.GetTileZ(origin.X + kSurroundingsIndexes[i].X, origin.Y + kSurroundingsIndexes[i].Y);

            _surroundingTiles = new Surroundings(
                surroundingTilesZ[7], surroundingTilesZ[3], surroundingTilesZ[6]);

            bool isFlat = _surroundingTiles.IsFlat && _surroundingTiles.East == Z;
            if (!isFlat)
            {
                int low = 0, high = 0, sort = 0;
                sort = map.GetAverageZ((int)Z, (int)_surroundingTiles.South, (int)_surroundingTiles.East, (int)_surroundingTiles.Down, ref low, ref high);
                if (sort != SortZ)
                {
                    SortZ = sort;
                    map.GetMapTile(Position.X, Position.Y, false).Resort();
                }
            }

            _normals[0] = calculateNormal_Old(
                surroundingTilesZ[2], surroundingTilesZ[3],
                surroundingTilesZ[0], surroundingTilesZ[6]);
            _normals[1] = calculateNormal_Old(
                Z, surroundingTilesZ[4],
                surroundingTilesZ[1], surroundingTilesZ[7]);
            _normals[2] = calculateNormal_Old(
                surroundingTilesZ[5], surroundingTilesZ[7],
                Z, surroundingTilesZ[9]);
            _normals[3] = calculateNormal_Old(
                surroundingTilesZ[6], surroundingTilesZ[8],
                surroundingTilesZ[3], surroundingTilesZ[10]);

            updateVertexBuffer();
        }

        private void updateVertexBuffer()
        {
            _vertex0_yOffset = new Vector3(22, -(Z * 4), 0);
            _vertex1_yOffset = new Vector3(44, 22 - (_surroundingTiles.East * 4), 0);
            _vertex2_yOffset = new Vector3(0, 22 - (_surroundingTiles.South * 4), 0);
            _vertex3_yOffset = new Vector3(22, 44 - (_surroundingTiles.Down * 4), 0);

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

        private Vector3 calculateNormal(Vector3 origin, Vector3 p2, Vector3 p3)
        {
            Vector3 U = (p2 - origin);
            Vector3 V = (p3 - origin);
            Vector3 N = new Vector3(
                U.Y * V.Z - U.Z * V.Y,
                U.Z * V.X - U.X * V.Z,
                U.X * V.Y - U.Y * V.X);
            N.Normalize();

            return N;
        }

        public static float Y_Normal = 1f;
        private Vector3 calculateNormal_Old(float A, float B, float C, float D)
        {
            Vector3 iVector = new Vector3(
                (A - B) / 2f,
                Y_Normal,
                (C - D) / 2f);
            iVector.Normalize();
            return iVector;
        }

        public override string ToString()
        {
            return string.Format("MapObjectGround\n   Z:{1} ({2},{3},{4})\n{0}", base.ToString(), Z, _surroundingTiles.South, _surroundingTiles.Down, _surroundingTiles.East);
        }
    }

    public class Surroundings
    {
        public float Down;
        public float East;
        public float South;

        public Surroundings(float down, float east, float south)
        {
            Down = down;
            East = east;
            South = south;
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