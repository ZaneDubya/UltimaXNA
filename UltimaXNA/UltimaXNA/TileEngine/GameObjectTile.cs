/***************************************************************************
 *   GameObjectTile.cs
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
using Microsoft.Xna.Framework;
using UltimaXNA.Entities;
#endregion

namespace UltimaXNA.TileEngine
{
    public class GameObjectTile : IMapObject
    {
        private int m_ID;
        private int m_Hue;
        private int m_Tiebreaker;
        private Entity m_OwnerEntity;
        private int m_Direction;
        private int m_BodyID;
        private int m_Frame;

        private Vector3 m_Position;
        public Vector2 Position { get { return new Vector2(m_Position.X, m_Position.Y); } }

        public GameObjectTile(int nID, Vector3 nPosition, int nDirection, Entity ownerEntity, int nHue)
        {
            m_ID = nID;
            m_Direction = nDirection;
            m_OwnerEntity = ownerEntity;
            m_Tiebreaker = 0;
            m_Hue = nHue;
            m_Position = nPosition;
        }

        public GameObjectTile(int nID, Vector3 nPosition, int nDirection, Entity ownerEntity, int nHue, int nBodyID, float nFrame)
        {
            m_ID = nID;
            m_Direction = nDirection;
            m_OwnerEntity = ownerEntity;
            m_Tiebreaker = 0;
            m_Hue = nHue;
            m_Position = nPosition;
            m_BodyID = nBodyID;
            m_Frame = (int)(nFrame * Data.BodyConverter.DeathAnimationFrameCount(nBodyID));
        }

        /// <summary>
        /// The Serial (int) of the owner GameObject.
        /// </summary>
        public int OwnerSerial
        {
            get { return m_OwnerEntity.Serial; }
        }

        public Entity OwnerEntity
        {
            get { return m_OwnerEntity; }
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

        /// <summary>
        /// ArtID of this object
        /// </summary>
        public int ID
        {
            get { return m_ID; }
        }

        public bool IsCorpse { get { return (m_ID == 0x2006); } }
        public int CorpseBody { get { return m_BodyID; } }
        public int CorpseFrame { get { return m_Frame; } }

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
            get { return MapObjectTypes.GameObjectTile; }
        }

        public int Z
        {
            get { return (int)m_Position.Z; }
        }
    }
}
