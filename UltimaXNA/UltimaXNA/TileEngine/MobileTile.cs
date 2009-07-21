/***************************************************************************
 *   MobileTile.cs
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
	// Issue 14 - Wrong layer draw order - http://code.google.com/p/ultimaxna/issues/detail?id=14 - Smjert
	public enum MobileTileTypes
	{
		Mount = 0,
		Body,
		Equipment
	}
	// Issue 14 - End
    public class MobileTile : IMapObject
    {
        private int _ID;
        private int _Tiebreaker;
        private Entity _ownerEntity;
        private int _Action, _Direction, _Hue;
        private float _Frame;
		// Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
		private bool _Mounted;
		// Issue 6 - End
		// Issue 14 - Wrong layer draw order - http://code.google.com/p/ultimaxna/issues/detail?id=14 - Smjert
		private MobileTileTypes _SubType;
		// Issue 14 - End

        private Vector3 _Position;
        public Vector2 Position { get { return new Vector2(_Position.X, _Position.Y); } }
        private Vector3 _Offset;
        public Vector3 Offset { get { return _Offset; } }

		// Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
        public MobileTile(int nID, Vector3 nPosition, Vector3 nOffset, int nDirection, int nAction, float nActionProgression, Entities.Entity nOwnerEntity, int nLayer, int nHue, bool nMounted)
		// Issue 6 - End
        {
            _ID = nID;
            _Direction = nDirection;
            _Action = nAction;
            _Frame = nActionProgression;
            _ownerEntity = nOwnerEntity;
            _Tiebreaker = nLayer;
            _Hue = nHue;
            _Position = nPosition;
            _Offset = nOffset;
			// Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
			_Mounted = nMounted;
			// Issue 6 - End
        }

        /// <summary>
        /// The Serial (int) of the owner GameObject.
        /// </summary>
        public int OwnerSerial
        {
            get { return _ownerEntity.Serial; }
        }
        public Entity OwnerEntity
        {
            get { return _ownerEntity; }
        }

        public int Action
        {
            get { return _Action; }
            set { _Action = value; }
        }

        public int Direction
        {
            get { return _Direction; }
            set { _Direction = value; }
        }

        public int Hue
        {
            get { return _Hue; }
            set { _Hue = value; }
        }

        public int Frame(int nMaxFrames)
        {
            return (int)(_Frame * (float)nMaxFrames);
        }

        public float ActionProgression
        {
            get { return _Frame; }
            set { _Frame = value; if (_Frame > 1.00f) { _Frame = 1.00f; } }
        }

        /// <summary>
        /// BodyID of the Animation
        /// </summary>
        public int ID
        {
            get { return _ID; }
        }

        public int SortZ
        {
            get { return (int)_Position.Z; }
        }

        public int Threshold
        {
            get { return 0; }
        }

        public int Layer
        {
            get { return _Tiebreaker; }
            set { _Tiebreaker = value; }
        }

        public int Tiebreaker
        {
            get { return _Tiebreaker; }
            set { _Tiebreaker = value; }
        }

        public MapObjectTypes Type
        {
            get { return MapObjectTypes.MobileTile; }
        }
		// Issue 14 - Wrong layer draw order - http://code.google.com/p/ultimaxna/issues/detail?id=14 - Smjert
		public MobileTileTypes SubType
		{
			get { return _SubType; }
			set { _SubType = value; }
		}
		// Issue 14 - End
        public int Z
        {
            get { return (int)_Position.Z; }
        }

		// Issue 6 - Missing mounted animations - http://code.google.com/p/ultimaxna/issues/detail?id=6 - Smjert
		public bool Mounted
		{
			get { return _Mounted; }
		}
		// Issue 6 - End
    }
}
