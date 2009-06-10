#region File Description & Usings
//-----------------------------------------------------------------------------
// MobileTile.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.TileEngine
{
    public class MobileTile : IMapObject
    {
        private int m_ID;
        private int m_Tiebreaker;
        private int m_OwnerGUID;
        private int m_Action, m_Direction, m_Hue;
        private float m_Frame;

        private Vector3 m_Position;
        public Vector2 Position { get { return new Vector2(m_Position.X, m_Position.Y); } }
        private Vector3 m_Offset; public Vector3 Offset { get { return m_Offset; } }

        public MobileTile(int nID, Vector3 nPosition, Vector3 nOffset, int nDirection, int nAction, float nActionProgression, int nOwnerGUID, int nLayer, int nHue)
        {
            m_ID = nID;
            m_Direction = nDirection;
            m_Action = nAction;
            m_Frame = nActionProgression;
            m_OwnerGUID = nOwnerGUID;
            m_Tiebreaker = nLayer;
            m_Hue = nHue;
            m_Position = nPosition;
            m_Offset = nOffset;
        }

        /// <summary>
        /// The GUID (int) of the owner GameObject.
        /// </summary>
        public int OwnerGUID
        {
            get { return m_OwnerGUID; }
            set { m_OwnerGUID = value; }
        }

        public int Action
        {
            get { return m_Action; }
            set { m_Action = value; }
        }

        public int Direction
        {
            get { return m_Direction; }
            set { m_Direction = value; }
        }

        public int Hue
        {
            get { return m_Hue; }
            set { m_Hue = value; }
        }

        public int Frame(int nMaxFrames)
        {
            return (int)(m_Frame * (float)nMaxFrames);
        }

        public float ActionProgression
        {
            get { return m_Frame; }
            set { m_Frame = value; if (m_Frame > 1.00f) { m_Frame = 1.00f; } }
        }

        /// <summary>
        /// BodyID of the Animation
        /// </summary>
        public int ID
        {
            get { return m_ID; }
        }

        public int SortZ
        {
            get { return (int)m_Position.Z; }
        }

        public int Threshold
        {
            get { return 0; }
        }

        public int Layer
        {
            get { return m_Tiebreaker; }
            set { m_Tiebreaker = value; }
        }

        public int Tiebreaker
        {
            get { return m_Tiebreaker; }
            set { m_Tiebreaker = value; }
        }

        public MapObjectTypes Type
        {
            get { return MapObjectTypes.MobileTile; }
        }

        public int Z
        {
            get { return (int)m_Position.Z; }
        }
    }
}
