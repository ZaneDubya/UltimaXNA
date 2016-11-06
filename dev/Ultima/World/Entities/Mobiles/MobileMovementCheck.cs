/***************************************************************************
 *   MovementCheck.cs
 *   Based on code from RunUO: http://www.runuo.com
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
using Microsoft.Xna.Framework;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Maps;
#endregion

namespace UltimaXNA.Ultima.World.Entities.Mobiles
{
	static class MobileMovementCheck
	{
        static List<Item>[] m_Pools = { new List<Item>(), new List<Item>(),  new List<Item>(), new List<Item>() };
        static List<MapTile> m_Tiles = new List<MapTile>();
        const TileFlag ImpassableSurface = TileFlag.Impassable | TileFlag.Surface;
        const int PersonHeight = 16;
        const int StepHeight = 2;

        public static int GetNextZ(Mobile m, Position3D loc, Direction d)
		{
            int newZ;
            if (CheckMovement(m, loc, d, out newZ, true))
                return newZ;
            return loc.Z;
		}

		public static bool CheckMovement(Mobile m, Position3D loc, Direction d, out int newZ, bool forceOK = false)
		{
			Map map = m.Map;

			if (map == null)
			{
				newZ = 0;
				return true;
			}

			int xStart = loc.X;
			int yStart = loc.Y;
			int xForward = xStart, yForward = yStart;
			int xRight = xStart, yRight = yStart;
			int xLeft = xStart, yLeft = yStart;

			bool checkDiagonals = ((int)d & 0x1) == 0x1;

			offsetXY(d, ref xForward, ref yForward);
			offsetXY((Direction)(((int)d - 1) & 0x7), ref xLeft, ref yLeft);
			offsetXY((Direction)(((int)d + 1) & 0x7), ref xRight, ref yRight);

			if (xForward < 0 || yForward < 0 || xForward >= map.TileWidth || yForward >= map.TileHeight)
			{
				newZ = 0;
				return false;
			}

			int startZ, startTop;

			List<Item> itemsStart = m_Pools[0];
			List<Item> itemsForward = m_Pools[1];
			List<Item> itemsLeft = m_Pools[2];
			List<Item> itemsRight = m_Pools[3];

			TileFlag reqFlags = ImpassableSurface;

			// if (m.CanSwim)
			//     reqFlags |= TileFlag.Wet;

			if (checkDiagonals)
			{
				MapTile tileStart = map.GetMapTile(xStart, yStart);
				MapTile tileForward = map.GetMapTile(xForward, yForward);
				MapTile tileLeft = map.GetMapTile(xLeft, yLeft);
				MapTile tileRight = map.GetMapTile(xRight, yRight);
				if ((tileForward == null) || (tileStart == null) || (tileLeft == null) || (tileRight == null))
				{
					newZ = loc.Z;
					return false;
				}

				List<MapTile> tiles = m_Tiles;

				tiles.Add(tileStart);
				tiles.Add(tileForward);
				tiles.Add(tileLeft);
				tiles.Add(tileRight);

				for (int i = 0; i < tiles.Count; ++i)
				{
					MapTile tile = tiles[i];

					for (int j = 0; j < tile.Entities.Count; ++j)
					{
						AEntity entity = tile.Entities[j];

						// if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
						//     continue;

						if (entity is Item)
						{
							Item item = (Item)entity;

							if ((item.ItemData.Flags & reqFlags) == 0)
								continue;

							if (tile == tileStart && item.AtWorldPoint(xStart, yStart) && item.ItemID < 0x4000)
								itemsStart.Add(item);
							else if (tile == tileForward && item.AtWorldPoint(xForward, yForward) && item.ItemID < 0x4000)
								itemsForward.Add(item);
							else if (tile == tileLeft && item.AtWorldPoint(xLeft, yLeft) && item.ItemID < 0x4000)
								itemsLeft.Add(item);
							else if (tile == tileRight && item.AtWorldPoint(xRight, yRight) && item.ItemID < 0x4000)
								itemsRight.Add(item);
						}
					}
				}
			}
			else
			{
				MapTile tileStart = map.GetMapTile(xStart, yStart);
				MapTile tileForward = map.GetMapTile(xForward, yForward);
				if ((tileForward == null) || (tileStart == null))
				{
					newZ = loc.Z;
					return false;
				}

				if (tileStart == tileForward)
				{
					for (int i = 0; i < tileStart.Entities.Count; ++i)
					{
						AEntity entity = tileStart.Entities[i];

						if (entity is Item)
						{
							Item item = (Item)entity;

							// if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
							//     continue;

							if ((item.ItemData.Flags & reqFlags) == 0)
								continue;

							if (item.AtWorldPoint(xStart, yStart) && item.ItemID < 0x4000)
								itemsStart.Add(item);
							else if (item.AtWorldPoint(xForward, yForward) && item.ItemID < 0x4000)
								itemsForward.Add(item);
						}
					}
				}
				else
				{
					for (int i = 0; i < tileForward.Entities.Count; ++i)
					{
						AEntity entity = tileForward.Entities[i];

						if (entity is Item)
						{
							Item item = (Item)entity;

							// if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
							//     continue;

							if ((item.ItemData.Flags & reqFlags) == 0)
								continue;

							if (item.AtWorldPoint(xForward, yForward) && item.ItemID < 0x4000)
								itemsForward.Add(item);
						}
					}

					for (int i = 0; i < tileStart.Entities.Count; ++i)
					{
						AEntity entity = tileStart.Entities[i];

						if (entity is Item)
						{
							Item item = (Item)entity;

							// if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
							//     continue;

							if ((item.ItemData.Flags & reqFlags) == 0)
								continue;

							if (item.AtWorldPoint(xStart, yStart) && item.ItemID < 0x4000)
								itemsStart.Add(item);
						}
					}
				}
			}

			getStartZ(m, map, loc, itemsStart, out startZ, out startTop);

			bool moveIsOk = check(map, m, itemsForward, xForward, yForward, startTop, startZ, out newZ) || forceOK;

			if (moveIsOk && checkDiagonals)
			{
				int hold;
                if (Settings.UltimaOnline.AllowCornerMovement)
                {
                    if (!check(map, m, itemsLeft, xLeft, yLeft, startTop, startZ, out hold) && !check(map, m, itemsRight, xRight, yRight, startTop, startZ, out hold))
                    {
                        moveIsOk = false;
                    }
                }
                else
                {
                    if (!check(map, m, itemsLeft, xLeft, yLeft, startTop, startZ, out hold) || !check(map, m, itemsRight, xRight, yRight, startTop, startZ, out hold))
                    {
                        moveIsOk = false;
                    }
                }
			}

			for (int i = 0; i < (checkDiagonals ? 4 : 2); ++i)
			{
				if (m_Pools[i].Count > 0)
					m_Pools[i].Clear();
			}

			if (!moveIsOk)
				newZ = startZ;

			return moveIsOk;
		}

        static bool check(Map map, Mobile m, List<Item> items, int x, int y, int startTop, int startZ, out int newZ)
        {
            newZ = 0;

            MapTile mapTile = map.GetMapTile(x, y);
            if (mapTile == null)
                return false;

            StaticItem[] tiles = mapTile.GetStatics().ToArray();

            bool landBlocks = (mapTile.Ground.LandData.Flags & TileFlag.Impassable) != 0;
            bool considerLand = !mapTile.Ground.IsIgnored;

            //if (landBlocks && canSwim && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) != 0)	//Impassable, Can Swim, and Is water.  Don't block it.
            //    landBlocks = false;
            // else 
            // if (cantWalk && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) == 0)	//Can't walk and it's not water
            //     landBlocks = true;

            int landLow = 0, landCenter = 0, landTop = 0;
            landCenter = map.GetAverageZ(x, y, ref landLow, ref landTop);

            bool moveIsOk = false;

            int stepTop = startTop + StepHeight;
            int checkTop = startZ + PersonHeight;

            bool ignoreDoors = (!m.IsAlive || m.Body == 0x3DB);

            #region Tiles
            for (int i = 0; i < tiles.Length; ++i)
            {
                StaticItem tile = tiles[i];

                if ((tile.ItemData.Flags & ImpassableSurface) == TileFlag.Surface) //  || (canSwim && (flags & TileFlag.Wet) != 0) Surface && !Impassable
                {
                    // if (cantWalk && (flags & TileFlag.Wet) == 0)
                    //     continue;

                    int itemZ = tile.Z;
                    int itemTop = itemZ;
                    int ourZ = itemZ + tile.ItemData.CalcHeight;
                    int ourTop = ourZ + PersonHeight;
                    int testTop = checkTop;

                    if (moveIsOk)
                    {
                        int cmp = Math.Abs(ourZ - m.Z) - Math.Abs(newZ - m.Z);

                        if (cmp > 0 || (cmp == 0 && ourZ > newZ))
                            continue;
                    }

                    if (ourZ + PersonHeight > testTop)
                        testTop = ourZ + PersonHeight;

                    if (!tile.ItemData.IsBridge)
                        itemTop += tile.ItemData.Height;

                    if (stepTop >= itemTop)
                    {
                        int landCheck = itemZ;

                        if (tile.ItemData.Height >= StepHeight)
                            landCheck += StepHeight;
                        else
                            landCheck += tile.ItemData.Height;

                        if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landLow)
                            continue;

                        if (IsOk(ignoreDoors, ourZ, testTop, tiles, items))
                        {
                            newZ = ourZ;
                            moveIsOk = true;
                        }
                    }
                }
            }
            #endregion

            #region Items
            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[i];
                ItemData itemData = item.ItemData;
                TileFlag flags = itemData.Flags;

                if ((flags & ImpassableSurface) == TileFlag.Surface) // Surface && !Impassable && !Movable
                {
                    //  || (m.CanSwim && (flags & TileFlag.Wet) != 0))
                    // !item.Movable && 
                    // if (cantWalk && (flags & TileFlag.Wet) == 0)
                    //     continue;

                    int itemZ = item.Z;
                    int itemTop = itemZ;
                    int ourZ = itemZ + itemData.CalcHeight;
                    int ourTop = ourZ + PersonHeight;
                    int testTop = checkTop;

                    if (moveIsOk)
                    {
                        int cmp = Math.Abs(ourZ - m.Z) - Math.Abs(newZ - m.Z);

                        if (cmp > 0 || (cmp == 0 && ourZ > newZ))
                            continue;
                    }

                    if (ourZ + PersonHeight > testTop)
                        testTop = ourZ + PersonHeight;

                    if (!itemData.IsBridge)
                        itemTop += itemData.Height;

                    if (stepTop >= itemTop)
                    {
                        int landCheck = itemZ;

                        if (itemData.Height >= StepHeight)
                            landCheck += StepHeight;
                        else
                            landCheck += itemData.Height;

                        if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landLow)
                            continue;

                        if (IsOk(ignoreDoors, ourZ, testTop, tiles, items))
                        {
                            newZ = ourZ;
                            moveIsOk = true;
                        }
                    }
                }
            }

            #endregion

            if (considerLand && !landBlocks && (stepTop) >= landLow)
            {
                int ourZ = landCenter;
                int ourTop = ourZ + PersonHeight;
                int testTop = checkTop;

                if (ourZ + PersonHeight > testTop)
                    testTop = ourZ + PersonHeight;

                bool shouldCheck = true;

                if (moveIsOk)
                {
                    int cmp = Math.Abs(ourZ - m.Z) - Math.Abs(newZ - m.Z);

                    if (cmp > 0 || (cmp == 0 && ourZ > newZ))
                        shouldCheck = false;
                }

                if (shouldCheck && IsOk(ignoreDoors, ourZ, testTop, tiles, items))
                {
                    newZ = ourZ;
                    moveIsOk = true;
                }
            }

            return moveIsOk;
        }

        static bool IsOk(bool ignoreDoors, int ourZ, int ourTop, StaticItem[] tiles, List<Item> items)
        {
            for (int i = 0; i < tiles.Length; ++i)
            {
                StaticItem item = tiles[i];
                if ((item.ItemData.Flags & ImpassableSurface) != 0) // Impassable || Surface
                {
                    int checkZ = item.Z;
                    int checkTop = checkZ + item.ItemData.CalcHeight;

                    if (checkTop > ourZ && ourTop > checkZ)
                        return false;
                }
            }

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[i];
                int itemID = item.ItemID & 0x3FFF;
                ItemData itemData = TileData.ItemData[itemID];
                TileFlag flags = itemData.Flags;

                if ((flags & ImpassableSurface) != 0) // Impassable || Surface
                {
                    if (ignoreDoors && ((flags & TileFlag.Door) != 0 || itemID == 0x692 || itemID == 0x846 || itemID == 0x873 || (itemID >= 0x6F5 && itemID <= 0x6F6)))
                        continue;

                    int checkZ = item.Z;
                    int checkTop = checkZ + itemData.CalcHeight;

                    if (checkTop > ourZ && ourTop > checkZ)
                        return false;
                }
            }

            return true;
        }

        public static Point OffsetTile(Position3D currentTile, Direction facing)
		{
			Point nextTile = currentTile.Tile;

			switch (facing & Direction.FacingMask)
			{
				case Direction.North: nextTile.Y--; break;
				case Direction.South: nextTile.Y++; break;
				case Direction.West: nextTile.X--; break;
				case Direction.East: nextTile.X++; break;
				case Direction.Right: nextTile.X++; nextTile.Y--; break;
				case Direction.Left: nextTile.X--; nextTile.Y++; break;
				case Direction.Down: nextTile.X++; nextTile.Y++; break;
				case Direction.Up: nextTile.X--; nextTile.Y--; break;
			}

			return nextTile;
		}

        static void offsetXY(Direction d, ref int x, ref int y)
        {
            switch (d & Direction.FacingMask)
            {
                case Direction.North:
                    --y;
                    break;
                case Direction.South:
                    ++y;
                    break;
                case Direction.West:
                    --x;
                    break;
                case Direction.East:
                    ++x;
                    break;
                case Direction.Right:
                    ++x;
                    --y;
                    break;
                case Direction.Left:
                    --x;
                    ++y;
                    break;
                case Direction.Down:
                    ++x;
                    ++y;
                    break;
                case Direction.Up:
                    --x;
                    --y;
                    break;
            }
        }

        static void getStartZ(AEntity m, Map map, Position3D loc, List<Item> itemList, out int zLow, out int zTop)
        {
            int xCheck = loc.X, yCheck = loc.Y;

            MapTile mapTile = map.GetMapTile(xCheck, yCheck);
            if (mapTile == null)
            {
                zLow = int.MinValue;
                zTop = int.MinValue;
            }

            bool landBlocks = mapTile.Ground.LandData.IsImpassible; //(TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Impassable) != 0;

            // if (landBlocks && m.CanSwim && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) != 0)
            //     landBlocks = false;
            // else if (m.CantWalk && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) == 0)
            //     landBlocks = true;

            int landLow = 0, landCenter = 0, landTop = 0;
            landCenter = map.GetAverageZ(xCheck, yCheck, ref landLow, ref landTop);

            bool considerLand = !mapTile.Ground.IsIgnored;

            int zCenter = zLow = zTop = 0;
            bool isSet = false;

            if (considerLand && !landBlocks && loc.Z >= landCenter)
            {
                zLow = landLow;
                zCenter = landCenter;

                if (!isSet || landTop > zTop)
                    zTop = landTop;

                isSet = true;
            }

            StaticItem[] staticTiles = mapTile.GetStatics().ToArray();

            for (int i = 0; i < staticTiles.Length; ++i)
            {
                StaticItem tile = staticTiles[i];

                int calcTop = tile.Z + tile.ItemData.CalcHeight;

                if ((!isSet || calcTop >= zCenter) && ((tile.ItemData.Flags & TileFlag.Surface) != 0) && loc.Z >= calcTop)
                {
                    //  || (m.CanSwim && (id.Flags & TileFlag.Wet) != 0)
                    // if (m.CantWalk && (id.Flags & TileFlag.Wet) == 0)
                    //     continue;

                    zLow = tile.Z;
                    zCenter = calcTop;

                    int top = tile.Z + tile.ItemData.Height;

                    if (!isSet || top > zTop)
                        zTop = top;

                    isSet = true;
                }
            }

            for (int i = 0; i < itemList.Count; ++i)
            {
                Item item = itemList[i];

                ItemData id = item.ItemData;

                int calcTop = item.Z + id.CalcHeight;

                if ((!isSet || calcTop >= zCenter) && ((id.Flags & TileFlag.Surface) != 0) && loc.Z >= calcTop)
                {
                    //  || (m.CanSwim && (id.Flags & TileFlag.Wet) != 0)
                    // if (m.CantWalk && (id.Flags & TileFlag.Wet) == 0)
                    //     continue;

                    zLow = item.Z;
                    zCenter = calcTop;

                    int top = item.Z + id.Height;

                    if (!isSet || top > zTop)
                        zTop = top;

                    isSet = true;
                }
            }

            if (!isSet)
                zLow = zTop = loc.Z;
            else if (loc.Z > zTop)
                zTop = loc.Z;
        }
    }
}
