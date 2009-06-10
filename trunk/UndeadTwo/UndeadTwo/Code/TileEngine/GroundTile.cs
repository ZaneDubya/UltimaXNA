#region File Description & Usings
//-----------------------------------------------------------------------------
// GroundTile.cs
//
// Created by ClintXNA
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UndeadClient.TileEngine
{
    public class GroundTile : IMapObject
    {
        private int m_ID;
        private int m_SortZ;
        private Surroundings m_Surroundings;
        private int m_Z;
        public int OwnerGUID { get { return -1; } }

        private Vector2 m_Position;
        public Vector2 Position { get { return m_Position; } }
        public Vector3[] Normals;

        public GroundTile(DataLocal.Tile landTile, Vector2 nPosition)
        {
            m_ID = landTile.ID;
            m_SortZ = m_Z = landTile.Z;
            m_Position = nPosition;
            Normals = new Vector3[4];
        }

        public GroundTile(int id, int z)
        {
            m_ID = id;
            m_SortZ = m_Z = z;
        }

        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        public bool Ignored
        {
            get { return (m_ID == 2 || m_ID == 0x1DB || (m_ID >= 0x1AE && m_ID <= 0x1B5)); }
        }

        public int SortZ
        {
            get { return m_SortZ; }
            set { m_SortZ = value; }
        }

        public Surroundings Surroundings
        {
            get { return m_Surroundings; }
            set { m_Surroundings = value; }
        }

        public int Threshold
        {
            get { return 0; }
        }

        public int Tiebreaker
        {
            get { return 0; }
        }

        public MapObjectTypes Type
        {
            get { return MapObjectTypes.GroundTile; }
        }

        public int Z
        {
            get { return m_Z; }
        }

        public void CalculateNormals()
        {
            VertexPositionNormalTexture[] m_VertexBufferForStretchedTile = new VertexPositionNormalTexture[4];

            m_VertexBufferForStretchedTile[0].Position.X += 22;
            m_VertexBufferForStretchedTile[1].Position.X += 44;
            m_VertexBufferForStretchedTile[1].Position.Y += 22;
            m_VertexBufferForStretchedTile[2].Position.Y += 22;
            m_VertexBufferForStretchedTile[3].Position.X += 22;
            m_VertexBufferForStretchedTile[3].Position.Y += 44;

            m_VertexBufferForStretchedTile[0].Position.Z = this.Z;
            m_VertexBufferForStretchedTile[1].Position.Z = this.Surroundings.East;
            m_VertexBufferForStretchedTile[2].Position.Z = this.Surroundings.South;
            m_VertexBufferForStretchedTile[3].Position.Z = this.Surroundings.Down;

            Normals[0] = Vector3.Normalize(Vector3.Cross(
                 m_VertexBufferForStretchedTile[1].Position - m_VertexBufferForStretchedTile[0].Position,
                 m_VertexBufferForStretchedTile[2].Position - m_VertexBufferForStretchedTile[0].Position));
            Normals[1] = Vector3.Normalize(Vector3.Cross(
                 m_VertexBufferForStretchedTile[3].Position - m_VertexBufferForStretchedTile[1].Position,
                 m_VertexBufferForStretchedTile[0].Position - m_VertexBufferForStretchedTile[1].Position));
            Normals[2] = Vector3.Normalize(Vector3.Cross(
                 m_VertexBufferForStretchedTile[0].Position - m_VertexBufferForStretchedTile[2].Position,
                 m_VertexBufferForStretchedTile[3].Position - m_VertexBufferForStretchedTile[2].Position));
            Normals[3] = Vector3.Normalize(Vector3.Cross(
                 m_VertexBufferForStretchedTile[2].Position - m_VertexBufferForStretchedTile[3].Position,
                 m_VertexBufferForStretchedTile[1].Position - m_VertexBufferForStretchedTile[3].Position));
        }
    }

    public class Surroundings
    {
        public int Down;
        public int East;
        public int South;
    }
}