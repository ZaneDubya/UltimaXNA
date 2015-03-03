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
using UltimaXNA.Rendering;
#endregion

namespace UltimaXNA.UltimaWorld
{
    public class MapObjectGround : AMapObject
    {
        public bool m_mustUpdateSurroundings = true;
        private Surroundings m_surroundingTiles;
        private bool m_noDraw;
        private Vector3[] m_normals;

        public bool Ignored
        {
            get { return (ItemID == 2 || ItemID == 0x1DB || (ItemID >= 0x1AE && ItemID <= 0x1B5)); }
        }

        public MapObjectGround(int tileID, Position3D position)
            : base(position)
        {
            ItemID = tileID;
            m_normals = new Vector3[4];
            SortThreshold = -1;
            SortTiebreaker = -1;

            // set no draw flag
            m_noDraw = (ItemID < 3 || (ItemID >= 0x1AF && ItemID <= 0x1B5));

            // get draw data
            UltimaData.LandData landData = UltimaData.TileData.LandData[ItemID & 0x3FFF];
            if (landData.TextureID <= 0)
            {
                m_draw_3DStretched = false;
                m_draw_texture = UltimaData.ArtData.GetLandTexture(ItemID);
                m_draw_width = m_draw_height = 44;
                m_draw_X = 0;
                m_draw_Y = (int)(Z * 4);
                m_draw_hue = Vector2.Zero;
                m_pickType = PickTypes.PickGroundTiles;
                m_draw_flip = false;
            }
            else
            {
                m_draw_3DStretched = true;
                m_draw_texture = UltimaData.TexmapData.GetTexmapTexture(landData.TextureID);
            }

            // set pick type
            m_pickType = PickTypes.PickGroundTiles;
        }

        private bool m_draw_3DStretched;
        internal override bool Draw(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            if (m_noDraw || m_mustUpdateSurroundings || !IsometricRenderer.DrawTerrain)
                return false;
            if (!m_draw_3DStretched)
                return base.Draw(sb, drawPosition, molist, pickType, 255);
            else
                return Draw3DStretched(sb, drawPosition, molist, pickType, 255);
        }

        VertexPositionNormalTextureHue[] m_vertexBufferAlternate = new VertexPositionNormalTextureHue[] {
                    new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 0, 0)),
                    new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 0, 0)),
                    new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 1, 0)),
                    new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 1, 0))
                };

        private Vector3 m_vertex0_yOffset, m_vertex1_yOffset, m_vertex2_yOffset, m_vertex3_yOffset;

        private bool Draw3DStretched(SpriteBatch3D sb, Vector3 drawPosition, MouseOverList molist, PickTypes pickType, int maxAlt)
        {
            // this is an isometric stretched tile and needs a specialized draw routine.
            m_vertexBufferAlternate[0].Position = drawPosition + m_vertex0_yOffset;
            m_vertexBufferAlternate[1].Position = drawPosition + m_vertex1_yOffset;
            m_vertexBufferAlternate[2].Position = drawPosition + m_vertex2_yOffset;
            m_vertexBufferAlternate[3].Position = drawPosition + m_vertex3_yOffset;

            if (!sb.Draw(m_draw_texture, m_vertexBufferAlternate))
                return false;

            if ((pickType & m_pickType) == m_pickType)
                if (molist.IsMouseInObjectIsometric(m_vertexBufferAlternate))
                {
                    MouseOverItem item = new MouseOverItem(m_draw_texture, m_vertexBufferAlternate[0].Position, this);
                    item.Vertices = new Vector3[4] { m_vertexBufferAlternate[0].Position, m_vertexBufferAlternate[1].Position, m_vertexBufferAlternate[2].Position, m_vertexBufferAlternate[3].Position };
                    molist.Add2DItem(item);
                }

            return true;
        }

        public void FlushSurroundings()
        {
            m_mustUpdateSurroundings = true;
        }

        public void UpdateSurroundingsIfNecessary(Map map)
        {
            if (!m_mustUpdateSurroundings)
                return;

            updateSurroundingsAndNormals(map);
            m_mustUpdateSurroundings = false;
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

            m_surroundingTiles = new Surroundings(
                surroundingTilesZ[7], surroundingTilesZ[3], surroundingTilesZ[6]);

            bool isFlat = m_surroundingTiles.IsFlat && m_surroundingTiles.East == Z;
            if (!isFlat)
            {
                int low = 0, high = 0, sort = 0;
                sort = map.GetAverageZ((int)Z, (int)m_surroundingTiles.South, (int)m_surroundingTiles.East, (int)m_surroundingTiles.Down, ref low, ref high);
                if (sort != SortZ)
                {
                    SortZ = sort;
                    map.GetMapTile(Position.X, Position.Y, false).Resort();
                }
            }

            m_normals[0] = calculateNormal_Old(
                surroundingTilesZ[2], surroundingTilesZ[3],
                surroundingTilesZ[0], surroundingTilesZ[6]);
            m_normals[1] = calculateNormal_Old(
                Z, surroundingTilesZ[4],
                surroundingTilesZ[1], surroundingTilesZ[7]);
            m_normals[2] = calculateNormal_Old(
                surroundingTilesZ[5], surroundingTilesZ[7],
                Z, surroundingTilesZ[9]);
            m_normals[3] = calculateNormal_Old(
                surroundingTilesZ[6], surroundingTilesZ[8],
                surroundingTilesZ[3], surroundingTilesZ[10]);

            updateVertexBuffer();
        }

        private void updateVertexBuffer()
        {
            m_vertex0_yOffset = new Vector3(22, -(Z * 4), 0);
            m_vertex1_yOffset = new Vector3(44, 22 - (m_surroundingTiles.East * 4), 0);
            m_vertex2_yOffset = new Vector3(0, 22 - (m_surroundingTiles.South * 4), 0);
            m_vertex3_yOffset = new Vector3(22, 44 - (m_surroundingTiles.Down * 4), 0);

            m_vertexBufferAlternate[0].Normal = m_normals[0];
            m_vertexBufferAlternate[1].Normal = m_normals[1];
            m_vertexBufferAlternate[2].Normal = m_normals[2];
            m_vertexBufferAlternate[3].Normal = m_normals[3];

            if (m_vertexBufferAlternate[0].Hue != m_draw_hue)
            {
                m_vertexBufferAlternate[0].Hue =
                m_vertexBufferAlternate[1].Hue =
                m_vertexBufferAlternate[2].Hue =
                m_vertexBufferAlternate[3].Hue = m_draw_hue;
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
            return string.Format("MapObjectGround\n   Z:{1} ({2},{3},{4})\n{0}", base.ToString(), Z, m_surroundingTiles.South, m_surroundingTiles.Down, m_surroundingTiles.East);
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