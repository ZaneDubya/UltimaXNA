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
    class TileComparer : IComparer<MapObject>
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

    public class Map
    {
        public int UpdateTicker;
        private int m_GameSize, m_GameSizeUp, m_GameSizeDown;
        private List<int> m_KeysToRemove;
        public SortedDictionary<int, MapCell> m_MapCells;
        private Data.TileMatrix m_TileMatrix;
        private int m_X;
        private int m_Y;

        public Map(int index, int gameSize, int gameSizeUp, int gameSizeDown)
        {
            m_GameSize = gameSize;
            m_GameSizeUp = gameSizeUp;
            m_GameSizeDown = gameSizeDown;

            m_KeysToRemove = new List<int>();
            m_MapCells = new SortedDictionary<int, MapCell>();
            m_TileMatrix = new Data.TileMatrix(index, index);
        }

        public int Height
        {
            get { return m_TileMatrix.Height; }
        }
        public int Width
        {
            get { return m_TileMatrix.Width; }
        }

        public void GetAverageZ(int x, int y, ref int z, ref int avg, ref int top)
        {
            int zTop, zLeft, zRight, zBottom;
            try
            {
                zTop = GetMapCell(x, y).GroundTile.Z;
                zLeft = GetMapCell(x, y + 1).GroundTile.Z;
                zRight = GetMapCell(x + 1, y).GroundTile.Z;
                zBottom = GetMapCell(x + 1, y + 1).GroundTile.Z;
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
            get { return m_GameSize; }
            set { m_GameSize = value; }
        }

        private int GetKey(MapCell mapCell)
        {
            return GetKey(mapCell.X, mapCell.Y);
        }

        private int GetKey(int x, int y)
        {
            return (x << 18) + y;
        }

        // This pulls a tile from the TileMatrix.
        public Data.Tile GetLandTile(int x, int y)
        {
            return m_TileMatrix.GetLandTile(x, y);
        }

        // Poplicola 5/9/2009
        // This references a tile that already exists in THIS dictionary.
        public MapObjectGround GetGroundTile(int x, int y)
        {
            return m_MapCells[GetKey(x, y)].GroundTile;
        }

        public MapCell GetMapCell(int x, int y)
        {
            // Poplicola - added a line to make sure we don't try t
            //reference an entry that doesn't exist - in a dictionary
            // of this size that might be slow.
            int iX = x - m_StartX;
            if ((iX < m_GameSize) && (iX >= 0))
            {
                int iY = y - m_StartY;
                if ((iY < m_GameSize) && (iY >= 0))
                {
                    try
                    {
                        return m_MapCells[GetKey(x, y)];
                    }
                    catch
                    {
                        // not in dictionary.
                        return null;
                    }
                }
            }
            return null;
        }

        public MapCell GetMapCell(Vector2 iLocation)
        {
            try
            {
                return m_MapCells[GetKey((int)iLocation.X, (int)iLocation.Y)];
            }
            catch
            {
                // not in dictionary.
                return null;
            }
        }

        private int m_StartX, m_StartY;
        public void Update(int x, int y)
        {
            if (m_X != World.X || m_Y != World.Y)
            {
                m_X = x;
                m_Y = y;

                m_StartX = m_X - m_GameSize / 2;
                m_StartY = m_Y - m_GameSize / 2;

                for (x = 0; x < m_GameSize; x++)
                {
                    for (y = 0; y < m_GameSize; y++)
                    {
                        mLoadCell(m_StartX + x, m_StartY + y);
                    }
                }
            }
        }

        private void mLoadCell(int nX, int nY)
        {
            MapObjectGround groundTile;
            MapCell mapCell;
            IEnumerator<MapCell> mapCellsEnumerator;
            Data.Point2D worldLocation;

            if (m_MapCells.ContainsKey(GetKey(nX, nY)))
                return;

            Data.Tile landTile = m_TileMatrix.GetLandTile(nX, nY);
            groundTile = new MapObjectGround(landTile, new Vector3(nX, nY, landTile.Z));
            groundTile.Surroundings = new Surroundings(
                m_TileMatrix.GetLandTile(nX + 1, nY + 1).Z,
                m_TileMatrix.GetLandTile(nX + 1, nY).Z,
                m_TileMatrix.GetLandTile(nX, nY + 1).Z);
            groundTile.CalculateNormals(
                m_TileMatrix.GetLandTile(nX - 1, nY).Z,
                m_TileMatrix.GetLandTile(nX - 1, nY + 1).Z,
                m_TileMatrix.GetLandTile(nX, nY - 1).Z,
                m_TileMatrix.GetLandTile(nX + 1, nY - 1).Z,
                m_TileMatrix.GetLandTile(nX, nY + 2).Z,
                m_TileMatrix.GetLandTile(nX + 1, nY + 2).Z,
                m_TileMatrix.GetLandTile(nX + 2, nY).Z,
                m_TileMatrix.GetLandTile(nX + 2, nY + 1).Z
                );

            if (Math.Abs(groundTile.Z - groundTile.Surroundings.Down) >= Math.Abs(groundTile.Surroundings.South - groundTile.Surroundings.East))
            {
                groundTile.SortZ = (Math.Min(groundTile.Z, groundTile.Surroundings.Down) + Math.Abs(groundTile.Surroundings.South - groundTile.Surroundings.East) / 2);
            }
            else
            {
                groundTile.SortZ = (Math.Min(groundTile.Z, groundTile.Surroundings.Down) + Math.Abs(groundTile.Z - groundTile.Surroundings.Down) / 2);
            }

            mapCell = new MapCell(nX, nY);
            mapCell.Add(groundTile);

            Data.StaticTile[] staticTiles = m_TileMatrix.GetStaticTiles(nX, nY);

            for (int i = 0; i < staticTiles.Length; i++)
            {
                mapCell.Add(new MapObjectStatic(staticTiles[i], i, new Vector3(nX, nY, staticTiles[i].Z)));
            }

            m_MapCells.Add(GetKey(mapCell), mapCell);

            m_KeysToRemove.Clear();

            mapCellsEnumerator = m_MapCells.Values.GetEnumerator();

            worldLocation = new Data.Point2D(World.X, World.Y);

            while (mapCellsEnumerator.MoveNext())
            {
                if (!Data.Helpers.InRange(worldLocation, mapCellsEnumerator.Current, m_GameSize / 2))
                {
                    m_KeysToRemove.Add(GetKey(mapCellsEnumerator.Current));
                }
            }

            for (int i = 0; i < m_KeysToRemove.Count; i++)
            {
                m_MapCells.Remove(m_KeysToRemove[i]);
            }

            UpdateTicker++;
        }

		// Issue 10 - Speed problems (Partial) - http://code.google.com/p/ultimaxna/issues/detail?id=10 - Smjert
		public static int GetDistanceToSqrt(int orgx, int orgy, int goalx, int goaly)
		{
			int xDelta = goalx - orgx;
			int yDelta = goaly - orgy;

			return (int)Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
		}
		// Issue 10 - End
    }

    public class MapCell : Data.IPoint2D
    {
        public List<MapObject> Objects { get { return m_Objects; } }
        private bool m_NeedsSorting;
        private List<MapObject> m_Objects;
        private MapObject[] m_Sorted;

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

        public MapCell(int x, int y)
        {
            m_Objects = new List<MapObject>();
            m_X = x;
            m_Y = y;
            m_Identifier = m_X.ToString() + ":" + m_Y.ToString();
        }

        // Check if under a roof. --Poplicola 6/2/2009.
        public bool UnderRoof(int nAltitude)
        {
            MapObject[] iObjects = this.GetSortedObjects();
            for (int i = iObjects.Length - 1; i >= 0; i-- )
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

        public MapObject[] GetSortedObjects()
        {
            if (m_NeedsSorting)
            {
                m_Objects.Sort(TileComparer.Comparer);
                m_NeedsSorting = false;
                m_Sorted = m_Objects.ToArray();
            }
            return m_Sorted;
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