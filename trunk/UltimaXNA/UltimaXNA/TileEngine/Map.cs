/***************************************************************************
 *   Map.cs
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
using System.Collections.Generic;
using UltimaXNA.Entities;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.TileEngine
{
    sealed class TileComparer : IComparer<MapObject>
    {
        public static readonly TileComparer Comparer = new TileComparer();

        public int Compare(MapObject x, MapObject y)
        {
            int result = (x.SortZ + x.Threshold) - (y.SortZ + y.Threshold);
            
            if (result == 0)
                result = typeSortValue(x) - typeSortValue(y);

            if (result == 0)
                result = x.Threshold - y.Threshold;

            if (result == 0)
                result = x.Tiebreaker - y.Tiebreaker;

            return result;
        }

        private int typeSortValue(MapObject mapobject)
        {
            Type type = mapobject.GetType();
            if (type == typeof(MapObject))
                return -1;
            else if (type == typeof(MapObjectGround))
                return 0;
            else if (type == typeof(MapObjectStatic))
                return 1;
            else if (type == typeof(MapObjectItem))
                return 2;
            else if (type == typeof(MapObjectMobile))
                return 4;
            else if (type == typeof(MapObjectText))
                return 4;
            return -100;
        }
    }

    public sealed class Map
    {
        public int UpdateTicker;
        int _renderSize, _renderSizeUp, _renderSizeDown;
        MapCell[] _cells;
        Data.TileMatrix _tileMatrix;
        int _x, _y;
        bool _firstUpdate = true;

        int _index = -1;
        public int Index { get { return _index; } }

        public Map(int index, int gameSize, int gameSizeUp, int gameSizeDown)
        {
            _renderSize = gameSize;
            _renderSizeUp = gameSizeUp;
            _renderSizeDown = gameSizeDown;

            _index = index;
            _tileMatrix = new Data.TileMatrix(_index, _index);
            _cells = new MapCell[_tileMatrix.BlockWidth * _tileMatrix.BlockHeight];
        }

        public int Height
        {
            get { return _tileMatrix.Height; }
        }
        public int Width
        {
            get { return _tileMatrix.Width; }
        }

        public void GetAverageZ(int x, int y, ref int z, ref int avg, ref int top)
        {
            int zTop, zLeft, zRight, zBottom;

            try
            {
                zTop = GetMapTile(x, y).GroundTile.Z;
                zLeft = GetMapTile(x, y + 1).GroundTile.Z;
                zRight = GetMapTile(x + 1, y).GroundTile.Z;
                zBottom = GetMapTile(x + 1, y + 1).GroundTile.Z;
            }
            catch
            {
                z = int.MinValue;
                avg = int.MinValue;
                top = int.MinValue;
                return;
            }

            z = zTop;
            if (zLeft < z)
                z = zLeft;
            if (zRight < z)
                z = zRight;
            if (zBottom < z)
                z = zBottom;

            top = zTop;
            if (zLeft > top)
                top = zLeft;
            if (zRight > top)
                top = zRight;
            if (zBottom > top)
                top = zBottom;

            if (Math.Abs(zTop - zBottom) > Math.Abs(zLeft - zRight))
                avg = FloorAverage(zLeft, zRight);
            else
                avg = FloorAverage(zTop, zBottom);
        }

        private static int FloorAverage(int a, int b)
        {
            int v = a + b;

            if (v < 0)
                --v;

            return (v / 2);
        }

        public int GameSize
        {
            get { return _renderSize; }
            set { _renderSize = value; }
        }

        private int GetKey(MapCell cell)
        {
            return GetKey(cell.X, cell.Y);
        }

        private int GetKey(int x, int y)
        {
            return (x << 18) + y;
        }

        // This pulls a tile from the TileMatrix.
        public Data.Tile GetLandTile(int x, int y)
        {
            return _tileMatrix.GetLandTile(x, y);
        }

        public MapTile GetMapTile(int x, int y)
        {
            MapCell c = getMapCell(x, y);
            if (c == null)
                return null;
            else
                return c.m_Tiles[x % 8 + ((y % 8) << 3)];
        }

        MapCell getMapCell(int x, int y)
        {
            return _cells[(x >> 3) + ((y >> 3) *  _tileMatrix.BlockWidth)];
        }

        public void Update(int centerX, int centerY)
        {
            if (_x != centerX || _y != centerY || _firstUpdate)
            {
                _x = centerX;
                _y = centerY;
                _firstUpdate = false;

                int renderBeginX = centerX - _renderSize / 2;
                int renderBeginY = centerY - _renderSize / 2;

                for (int x = 0; x < _renderSize; x++)
                {
                    for (int y = 0; y < _renderSize; y++)
                    {
                        loadTile(renderBeginX + x, renderBeginY + y);
                    }
                }

                clearOlderCells();
            }
        }

        void loadTile(int x, int y)
        {
            MapCell c = getMapCell(x, y);
            if (c == null)
                c = _cells[(x >> 3) + ((y >> 3) * _tileMatrix.BlockWidth)] = new MapCell(this, _tileMatrix, x - x % 8, y - y % 8);
            if (c.Tile(x, y) == null)
                c.LoadTile(x, y);
        }

        public void UpdateSurroundings(MapObjectGround g)
        {
            int x = (int)g.Position.X;
            int y = (int)g.Position.Y;

            int[] zValues = new int[16]; // _matrix.GetElevations(x - 1, y - 1, 4, 4);

            for (int iy = -1; iy < 3; iy++)
            {
                for (int ix = -1; ix < 3; ix++)
                {
                    MapTile t = GetMapTile(x + ix, y + iy);
                    zValues[(ix + 1) + (iy + 1) * 4] =
                        (t == null) ? _tileMatrix.GetLandTile(x + ix, y + iy).Z : t.GroundTile.Z;
                }
            }

            g.Surroundings = new Surroundings(
                zValues[2 + 2 * 4],
                zValues[2 + 1 * 4],
                zValues[1 + 2 * 4]);
            g.CalculateNormals(
                zValues[0 + 1 * 4],
                zValues[0 + 2 * 4],
                zValues[1 + 0 * 4],
                zValues[2 + 0 * 4],
                zValues[1 + 3 * 4],
                zValues[2 + 3 * 4],
                zValues[3 + 1 * 4],
                zValues[3 + 2 * 4]);

            if (Math.Abs(g.Z - g.Surroundings.Down) >= Math.Abs(g.Surroundings.South - g.Surroundings.East))
            {
                g.SortZ = (Math.Min(g.Z, g.Surroundings.Down) + Math.Abs(g.Surroundings.South - g.Surroundings.East) / 2);
            }
            else
            {
                g.SortZ = (Math.Min(g.Z, g.Surroundings.Down) + Math.Abs(g.Z - g.Surroundings.Down) / 2);
            }
        }

        private void clearOlderCells()
        {
            // !!! This no longer works. Need to fix it
            // IEnumerator<MapCell> MapTilesEnumerator;
            // Data.Point2D worldLocation;
            // m_KeysToRemove.Clear();
        }
    }

    public sealed class MapCell : Data.IPoint2D
    {
        public MapTile[] m_Tiles = new MapTile[64];
        Map _map;
        Data.TileMatrix _matrix;

        #region XY
        int _x, _y;
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        #endregion

        public MapCell(Map map, Data.TileMatrix matrix, int x, int y)
        {
            _map = map;
            _matrix = matrix;
            _x = x;
            _y = y;
        }

        public MapTile Tile(int x, int y)
        {
            return m_Tiles[x % 8 + (y % 8) * 8];
        }

        public void LoadTile(int x, int y)
        {
            MapObjectGround groundTile;
            MapTile tile;

            Data.Tile landTile = _matrix.GetLandTile(x, y);
            groundTile = new MapObjectGround(landTile, new Vector3(x, y, landTile.Z));
            _map.UpdateSurroundings(groundTile);

            tile = new MapTile(x, y);
            tile.Add(groundTile);

            Data.StaticTile[] staticTiles = _matrix.GetStaticTiles(x, y);

            for (int i = 0; i < staticTiles.Length; i++)
            {
                tile.Add(new MapObjectStatic(staticTiles[i], i, new Vector3(x, y, staticTiles[i].Z)));
            }

            m_Tiles[tile.X % 8 + (tile.Y % 8) * 8] = tile;
        }
    }

    public sealed class MapTile : Data.IPoint2D
    {
        public List<MapObject> Objects { get { return m_Objects; } }
        private bool m_NeedsSorting;
        private List<MapObject> m_Objects;

        #region X
        private int m_X;
        public int X
        {
            get { return m_X; }
            set { m_X = value; }
        }
        #endregion
        #region Y
        private int m_Y;
        public int Y
        {
            get { return m_Y; }
            set { m_Y = value; }
        }
        #endregion
        #region IdentifierString
        private string m_Identifier = null;
        public string IdentifierString
        {
            get { return m_Identifier; }
        }
        #endregion

        public MapTile(int x, int y)
        {
            m_Objects = new List<MapObject>();
            m_X = x;
            m_Y = y;
            m_Identifier = m_X.ToString() + ":" + m_Y.ToString();
        }

        // Check if under a roof. --Poplicola 6/2/2009.
        public bool UnderRoof(int nAltitude)
        {
            List<MapObject> iObjects = this.GetSortedObjects();
            for (int i = iObjects.Count - 1; i >= 0; i-- )
            {
                if (iObjects[i].Z <= nAltitude)
                    return false;
                if (iObjects[i] is MapObjectStatic)
                {
                    Data.ItemData iData = Data.TileData.ItemData[((MapObjectStatic)iObjects[i]).ItemID - 0x4000];
                    if (iData.Roof)
                        return true;
                    if (iData.Surface)
                        return true;
                }
            }
            return false;
        }

        // --Poplicola 5/13/2009. Updated 5/14/2009.
        public void FlushMobilesAll()
        {
            m_Objects.RemoveAll(IsMobile);
            m_NeedsSorting = true;
        }

        // Poplicola 5/14/2009.
        public void FlushObjectsBySerial(Serial serial)
        {
            List<MapObject> iObjects = new List<MapObject>();
            foreach (MapObject iObject in m_Objects)
            {
                if (iObject.OwnerSerial == serial)
                {
                    // Do nothing. Object is skipped.
                }
                else
                {
                    iObjects.Add(iObject);
                }
                m_Objects = iObjects;
                m_NeedsSorting = true;
            }
            m_NeedsSorting = true;
        }

        // Poplicola 5/9/2009
        public MapObjectGround GroundTile
        {
            get { return (MapObjectGround)m_Objects.Find(IsGroundTile); }

        }
        // Poplicola 5/9/2009
        private static bool IsGroundTile(object i)
        {
            Type t = typeof(MapObjectGround);
            return t == i.GetType() || i.GetType().IsSubclassOf(t);
        }
        // Poplicola 5/10/2009
        private static bool IsMobile(object i)
        {
            Type t = typeof(MapObjectMobile);
            return t == i.GetType() || i.GetType().IsSubclassOf(t);
        }
		// Issue 5 - Statics (bridge, stairs, etc) should be walkable - http://code.google.com/p/ultimaxna/issues/detail?id=5 - Smjert
		private static bool IsStaticItem(object i)
		{
			Type t = typeof(MapObjectStatic);
			return t == i.GetType() || i.GetType().IsSubclassOf(t);
		}

		private static bool IsGOTile(object i)
		{
			Type t = typeof(MapObjectItem);
			return t == i.GetType() || i.GetType().IsSubclassOf(t);
		}

		public List<MapObjectStatic> GetStatics()
		{
            List<MapObjectStatic> sitems = new List<MapObjectStatic>();

            List<MapObject> objs = m_Objects.FindAll(IsStaticItem);
            if (objs == null || objs.Count == 0)
            {
                // empty list.
                return sitems;
            }

            foreach (MapObject obj in objs)
			{
				sitems.Add((MapObjectStatic)obj);
			}

			return sitems;
		}

		public List<MapObjectItem> GetGOTiles()
		{
            List<MapObject> objs = m_Objects.FindAll(IsGOTile);

			if ( objs == null || objs.Count == 0 )
				return null;

			List<MapObjectItem> goitems = new List<MapObjectItem>();
            foreach (MapObject obj in objs)
			{
				goitems.Add((MapObjectItem)obj);
			}

			return goitems;
		}
		// I leave this since could be useful, even if now isn't used.
		public bool OnStairs()
		{
            List<MapObject> staticobjs = m_Objects.FindAll(IsStaticItem);
			
			bool result = false;

			if ( staticobjs == null || staticobjs.Count == 0) 
				return false;

            foreach (MapObject obj in staticobjs)
			{
				Data.ItemData iData = Data.TileData.ItemData[obj.ItemID - 0x4000];
				if(iData.Stairs)
				{
					result = true;
					break;
				}
			}

			if(!result)
			{
                List<MapObject> goobjs = m_Objects.FindAll(IsGOTile);
				if ( goobjs == null || goobjs.Count == 0 )
					return false;
                foreach (MapObject obj in goobjs)
				{
					Data.ItemData iData = Data.TileData.ItemData[obj.ItemID];
					if(iData.Stairs)
					{
						result = true;
						break;
					}
				}
			}

			return result;
		}

        public void Add(MapObject item)
        {
            m_Objects.Add(item);
            m_NeedsSorting = true;
        }

        public void Add(MapObject[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                m_Objects.Add(items[i]);
            }

            m_NeedsSorting = true;
        }

        public List<MapObject> GetSortedObjects()
        {
            if (m_NeedsSorting)
            {
                m_Objects.Sort(TileComparer.Comparer);
                m_NeedsSorting = false;
            }
            return m_Objects;
        }

        public List<MapObject> Items
        {
            get
            {
                return new List<MapObject>(this.GetSortedObjects());
            }
        }
    }
}