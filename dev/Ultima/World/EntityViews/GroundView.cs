﻿using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Input;
using UltimaXNA.Ultima.World.Maps;
using UltimaXNA.Ultima.World.WorldViews;

namespace UltimaXNA.Ultima.World.EntityViews
{
    public class GroundView : AEntityView
    {
        new Ground Entity
        {
            get { return (Ground)base.Entity; }
        }

        bool m_DrawAs3DStretched;
        bool m_NoDraw;

        public GroundView(Ground ground)
            : base(ground)
        {
            PickType = PickType.PickGroundTiles;
            m_NoDraw = (Entity.LandDataID < 3 || (Entity.LandDataID >= 0x1AF && Entity.LandDataID <= 0x1B5));
             DrawFlip = false;
            if (Entity.LandData.TextureID <= 0)
            {
                m_DrawAs3DStretched = false;
                DrawTexture = Provider.GetLandTexture(Entity.LandDataID);
                DrawArea = new Rectangle(0, Entity.Z * 4, IsometricRenderer.TILE_SIZE_INTEGER, IsometricRenderer.TILE_SIZE_INTEGER);
            }
            else
            {
                m_DrawAs3DStretched = true;
                DrawTexture = Provider.GetTexmapTexture(Entity.LandData.TextureID);
            }
        }

        protected override void Pick(MouseOverList mouseOver, VertexPositionNormalTextureHue[] vertexBuffer)
        {
            // TODO: This is called when the tile is not stretched - just drawn as a 44x44 tile.
            // Because this is not written, no flat tiles can ever be picked.
        }

        public override bool Draw(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (m_NoDraw)
            {
                return false;
            }
            if (m_MustUpdateSurroundings)
            {
                updateSurroundingsAndNormals(Entity.Map);
                m_MustUpdateSurroundings = false;
            }
            if (!m_DrawAs3DStretched)
            {
                return base.Draw(spriteBatch, drawPosition, mouseOver, map, roofHideFlag);
            }
            return Draw3DStretched(spriteBatch, drawPosition, mouseOver, map);
        }

        Vector3 m_vertex0_yOffset, m_vertex1_yOffset, m_vertex2_yOffset, m_vertex3_yOffset;
        VertexPositionNormalTextureHue[] m_vertexBufferAlternate = {
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 0, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 0, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(0, 1, 0)),
            new VertexPositionNormalTextureHue(new Vector3(), new Vector3(),  new Vector3(1, 1, 0))
        };

        bool Draw3DStretched(SpriteBatch3D spriteBatch, Vector3 drawPosition, MouseOverList mouseOver, Map map)
        {
            // this is an isometric stretched tile and needs a specialized draw routine.
            m_vertexBufferAlternate[0].Position = drawPosition + m_vertex0_yOffset;
            m_vertexBufferAlternate[1].Position = drawPosition + m_vertex1_yOffset;
            m_vertexBufferAlternate[2].Position = drawPosition + m_vertex2_yOffset;
            m_vertexBufferAlternate[3].Position = drawPosition + m_vertex3_yOffset;
            if (!spriteBatch.DrawSprite(DrawTexture, m_vertexBufferAlternate, Technique))
            {
                return false;
            }
            if ((mouseOver.PickType & PickType) == PickType)
            {
                if (mouseOver.IsMouseInObjectIsometric(m_vertexBufferAlternate))
                {
                    mouseOver.AddItem(Entity, m_vertexBufferAlternate[0].Position);
                }
            }
            return true;
        }

        bool m_MustUpdateSurroundings = true;
        Surroundings m_SurroundingTiles;
        Vector3[] m_Normals = new Vector3[4];

        void updateVertexBuffer()
        {
            m_vertex0_yOffset = new Vector3(IsometricRenderer.TILE_SIZE_INTEGER_HALF, -(Entity.Z * 4), 0);
            m_vertex1_yOffset = new Vector3(IsometricRenderer.TILE_SIZE_FLOAT, IsometricRenderer.TILE_SIZE_INTEGER_HALF - (m_SurroundingTiles.East * 4), 0);
            m_vertex2_yOffset = new Vector3(0, IsometricRenderer.TILE_SIZE_INTEGER_HALF - (m_SurroundingTiles.South * 4), 0);
            m_vertex3_yOffset = new Vector3(IsometricRenderer.TILE_SIZE_INTEGER_HALF, IsometricRenderer.TILE_SIZE_FLOAT - (m_SurroundingTiles.Down * 4), 0);

            m_vertexBufferAlternate[0].Normal = m_Normals[0];
            m_vertexBufferAlternate[1].Normal = m_Normals[1];
            m_vertexBufferAlternate[2].Normal = m_Normals[2];
            m_vertexBufferAlternate[3].Normal = m_Normals[3];
            Vector3 hue = Utility.GetHueVector(Entity.Hue);
            if (m_vertexBufferAlternate[0].Hue != hue)
            {
                m_vertexBufferAlternate[0].Hue =
                m_vertexBufferAlternate[1].Hue =
                m_vertexBufferAlternate[2].Hue =
                m_vertexBufferAlternate[3].Hue = hue;
            }
        }

        static Point[] kSurroundingsIndexes = { 
            new Point(0, -1), new Point(1, -1), 
            new Point(-1, 0), new Point(1, 0), new Point(2, 0), 
            new Point(-1, 1), new Point(0, 1), new Point(1, 1), new Point(2, 1), 
            new Point(0, 2), new Point(1, 2) };

        void updateSurroundingsAndNormals(Map map)
        {
            Point origin = new Point(Entity.Position.X, Entity.Position.Y);

            float[] surroundingTilesZ = new float[kSurroundingsIndexes.Length];


            for (int i = 0; i < kSurroundingsIndexes.Length; i++)
                surroundingTilesZ[i] = map.GetTileZ(origin.X + kSurroundingsIndexes[i].X, origin.Y + kSurroundingsIndexes[i].Y);

            m_SurroundingTiles = new Surroundings(
                surroundingTilesZ[7], surroundingTilesZ[3], surroundingTilesZ[6]);

            bool isFlat = m_SurroundingTiles.IsFlat && m_SurroundingTiles.East == Entity.Z;
            if (!isFlat)
            {
                int low = 0, high = 0, sort = 0;
                sort = map.GetAverageZ(Entity.Z, (int)m_SurroundingTiles.South, (int)m_SurroundingTiles.East, (int)m_SurroundingTiles.Down, ref low, ref high);
                if (sort != SortZ)
                {
                    SortZ = sort;
                    map.GetMapTile(Entity.Position.X, Entity.Position.Y).ForceSort();
                }
            }

            m_Normals[0] = calculateNormal(
                surroundingTilesZ[2], surroundingTilesZ[3],
                surroundingTilesZ[0], surroundingTilesZ[6]);
            m_Normals[1] = calculateNormal(
                Entity.Z, surroundingTilesZ[4],
                surroundingTilesZ[1], surroundingTilesZ[7]);
            m_Normals[2] = calculateNormal(
                surroundingTilesZ[5], surroundingTilesZ[7],
                Entity.Z, surroundingTilesZ[9]);
            m_Normals[3] = calculateNormal(
                surroundingTilesZ[6], surroundingTilesZ[8],
                surroundingTilesZ[3], surroundingTilesZ[10]);

            updateVertexBuffer();
        }

        public static float Y_Normal = 1f;
        Vector3 calculateNormal(float A, float B, float C, float D)
        {
            Vector3 iVector = new Vector3(
                (A - B),
                Y_Normal,
                (C - D));
            iVector.Normalize();
            return iVector;
        }

        class Surroundings
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
}
