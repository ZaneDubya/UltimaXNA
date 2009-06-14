#region File Description & Usings
//-----------------------------------------------------------------------------
// Map.cs
//
// Created by ClintXNA, Modifications by Poplicola
//-----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.TileEngine
{
    public interface IMapObject
    {
        int ID { get; }
        int SortZ { get; }
        int Threshold { get; }
        int Tiebreaker { get; }
        MapObjectTypes Type { get; }
        Vector2 Position { get; }
        int Z { get; }
        int OwnerGUID { get; }
    }

    public enum MapObjectTypes
    {
        GroundTile = 0,
        StaticTile = 1,
        MobileTile = 2,
        GameObjectTile = 3,
    }

    class TileComparer : IComparer<IMapObject>
    {
        public static readonly TileComparer Comparer = new TileComparer();

        public int Compare(IMapObject x, IMapObject y)
        {
            int result = (x.SortZ + x.Threshold) - (y.SortZ + y.Threshold);

            if (result == 0)
                result = x.Type - y.Type;

            if (result == 0)
                result = x.Threshold - y.Threshold;

            if (result == 0)
                result = x.Tiebreaker - y.Tiebreaker;

            return result;
        }
    }

    public class Map
    {
        private int m_GameSize, m_GameSizeUp, m_GameSizeDown;
        private List<int> m_KeysToRemove;
        private Dictionary<int, MapCell> m_MapCells;
        private DataLocal.TileMatrix m_TileMatrix;
        private int m_X;
        private int m_Y;

        public Map(int index, int gameSize, int gameSizeUp, int gameSizeDown)
        {
            m_GameSize = gameSize;
            m_GameSizeUp = gameSizeUp;
            m_GameSizeDown = gameSizeDown;

            m_KeysToRemove = new List<int>();
            m_MapCells = new Dictionary<int, MapCell>((m_GameSize + gameSizeUp + gameSizeDown) ^ 2);
            m_TileMatrix = new DataLocal.TileMatrix(index, index);
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
            return x * 100000 + y;
        }

        // This pulls a tile from the TileMatrix.
        public DataLocal.Tile GetLandTile(int x, int y)
        {
            return m_TileMatrix.GetLandTile(x, y);
        }

        // Poplicola 5/9/2009
        // This references a tile that already exists in THIS dictionary.
        public GroundTile GetGroundTile(int x, int y)
        {
            return m_MapCells[GetKey(x, y)].GroundTile;
        }

        public MapCell GetMapCell(int x, int y)
        {
            // Poplicola - added a line to make sure we don't try t
            //reference an entry that doesn't exist - in a dictionary
            // of this size that might be slow.
            int iX = x - m_StartX;
            int iY = y - m_StartY;
            if ((iX < GameSize) && (iX >= 0))
                if ((iY < GameSize) && (iY >= 0))
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
        public void UpdateLocation(int x, int y)
        {
            if (m_X != x || m_Y != y)
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
            GroundTile groundTile;
            MapCell mapCell;
            IEnumerator<MapCell> mapCellsEnumerator;
            DataLocal.Point2D worldLocation;

            if (m_MapCells.ContainsKey(GetKey(nX, nY)))
            {
                return;
            }

            groundTile = new GroundTile(m_TileMatrix.GetLandTile(nX, nY), new Vector2(nX,nY));
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

            DataLocal.StaticTile[] staticTiles = m_TileMatrix.GetStaticTiles(nX, nY);

            for (int i = 0; i < staticTiles.Length; i++)
            {
                mapCell.Add(new StaticItem(staticTiles[i], i, new Vector2(nX, nY)));
            }

            m_MapCells.Add(GetKey(mapCell), mapCell);

            m_KeysToRemove.Clear();

            mapCellsEnumerator = m_MapCells.Values.GetEnumerator();

            worldLocation = new DataLocal.Point2D(World.X, World.Y);

            while (mapCellsEnumerator.MoveNext())
            {
                if (!DataLocal.Helpers.InRange(worldLocation, mapCellsEnumerator.Current, m_GameSize / 2))
                {
                    m_KeysToRemove.Add(GetKey(mapCellsEnumerator.Current));
                }
            }

            for (int i = 0; i < m_KeysToRemove.Count; i++)
            {
                m_MapCells.Remove(m_KeysToRemove[i]);
            }
        }
    }

    public class MapCell : DataLocal.IPoint2D
    {
        private bool m_NeedsSorting;
        private List<IMapObject> m_Objects;
        private IMapObject[] m_Sorted;

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
            m_Objects = new List<IMapObject>();
            m_X = x;
            m_Y = y;
            m_Identifier = m_X.ToString() + ":" + m_Y.ToString();
        }

        // Check if under a roof. --Poplicola 6/2/2009.
        public bool UnderRoof(int nAltitude)
        {
            IMapObject[] iObjects = this.GetSortedObjects();
            for (int i = iObjects.Length - 1; i >= 0; i-- )
            {
                if (iObjects[i].Z <= nAltitude)
                    return false;
                if (iObjects[i].Type == MapObjectTypes.StaticTile)
                {
                    DataLocal.ItemData iData = DataLocal.TileData.ItemData[((StaticItem)iObjects[i]).ID - 0x4000];
                    if (iData.Roof)
                        return true;
                    if (iData.Surface)
                        return true;
                }
                int k = iObjects[i].Z;
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
        public void FlushObjectsByGUID(int nGUID)
        {
            List<IMapObject> iObjects = new List<IMapObject>();
            foreach (IMapObject iObject in m_Objects)
            {
                if (iObject.OwnerGUID == nGUID)
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

        // Poplicola 5/10/2009. Updated 5/14/2009.
        public void AddMobileTile(MobileTile nMobile)
        {
            m_Objects.Add(nMobile);
            m_NeedsSorting = true;
        }

        public void AddGameObjectTile(GameObjectTile nObject)
        {
            m_Objects.Add(nObject);
            m_NeedsSorting = true;
        }

        // Poplicola 5/9/2009
        public GroundTile GroundTile
        {
            get { return (GroundTile)m_Objects.Find(IsGroundTile); }

        }
        // Poplicola 5/9/2009
        private static bool IsGroundTile(object i)
        {
            Type t = typeof(GroundTile);
            return t == i.GetType() || i.GetType().IsSubclassOf(t);
        }
        // Poplicola 5/10/2009
        private static bool IsMobile(object i)
        {
            Type t = typeof(MobileTile);
            return t == i.GetType() || i.GetType().IsSubclassOf(t);
        }

        public void Add(GroundTile groundTile)
        {
            m_Objects.Add(groundTile);

            m_NeedsSorting = true;
        }

        public void Add(StaticItem[] staticItems)
        {
            for (int i = 0; i < staticItems.Length; i++)
            {
                m_Objects.Add(staticItems[i]);
            }

            m_NeedsSorting = true;
        }

        public void Add(StaticItem staticItem)
        {
            m_Objects.Add(staticItem);

            m_NeedsSorting = true;
        }

        public IMapObject[] GetSortedObjects()
        {
            if (m_NeedsSorting)
            {
                m_Objects.Sort(TileComparer.Comparer);
                m_NeedsSorting = false;
                m_Sorted = m_Objects.ToArray();
            }
            return m_Sorted;
        }
    }
}