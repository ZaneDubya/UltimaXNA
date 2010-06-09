/***************************************************************************
 *   MovementCheck.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from RunUO: http://www.runuo.com
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
using Microsoft.Xna.Framework;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    static class MovementCheck
    {
        private static List<Item>[] m_Pools = new List<Item>[4]
			{
				new List<Item>(), new List<Item>(),
				new List<Item>(), new List<Item>(),
			};

        private const TileFlag ImpassableSurface = TileFlag.Impassable | TileFlag.Surface;
        private const int PersonHeight = 16;
        private const int StepHeight = 2;

        public static bool CheckMovement(Mobile m, Map map, Vector3 loc, Direction d, out int newZ)
        {
            if (map == null)
            {
                newZ = 0;
                return true;
            }

            int xStart = (int)loc.X;
            int yStart = (int)loc.Y;
            int xForward = xStart, yForward = yStart;
            int xRight = xStart, yRight = yStart;
            int xLeft = xStart, yLeft = yStart;

            bool checkDiagonals = ((int)d & 0x1) == 0x1;

            offsetXY(d, ref xForward, ref yForward);
            offsetXY((Direction)(((int)d - 1) & 0x7), ref xLeft, ref yLeft);
            offsetXY((Direction)(((int)d + 1) & 0x7), ref xRight, ref yRight);

            if (xForward < 0 || yForward < 0 || xForward >= map.Width || yForward >= map.Height)
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
                MapTile sectorStart = map.GetMapTile(xStart, yStart, false);
                MapTile sectorForward = map.GetMapTile(xForward, yForward, false);
                MapTile sectorLeft = map.GetMapTile(xLeft, yLeft, false);
                MapTile sectorRight = map.GetMapTile(xRight, yRight, false);
                if ((sectorForward == null) || (sectorStart == null) || (sectorLeft == null) || (sectorRight == null))
                {
                    newZ = (int)loc.Z;
                    return false;
                }

                List<MapTile> sectors = new List<MapTile>(); //m_Sectors;

                sectors.Add(sectorStart);
                sectors.Add(sectorForward);
                sectors.Add(sectorLeft);
                sectors.Add(sectorRight);

                for (int i = 0; i < sectors.Count; ++i)
                {
                    MapTile sector = sectors[i];

                    for (int j = 0; j < sector.Items.Count; ++j)
                    {
                        Entity entity = sector.Items[j].OwnerEntity; //Item item = sector.Items[j];

                        // if (ignoreMovableImpassables && item.Movable && item.ItemData.Impassable)
                        //     continue;

                        if (entity is Item)
                        {
                            Item item = (Item)entity;

                            if ((item.ItemData.Flags & reqFlags) == 0)
                                continue;

                            if (sector == sectorStart && item.AtWorldPoint(xStart, yStart) && item.ItemID < 0x4000)
                                itemsStart.Add(item);
                            else if (sector == sectorForward && item.AtWorldPoint(xForward, yForward) && item.ItemID < 0x4000)
                                itemsForward.Add(item);
                            else if (sector == sectorLeft && item.AtWorldPoint(xLeft, yLeft) && item.ItemID < 0x4000)
                                itemsLeft.Add(item);
                            else if (sector == sectorRight && item.AtWorldPoint(xRight, yRight) && item.ItemID < 0x4000)
                                itemsRight.Add(item);
                        }
                    }
                }
            }
            else
            {
                MapTile sectorStart = map.GetMapTile(xStart, yStart, false);
                MapTile sectorForward = map.GetMapTile(xForward, yForward, false);
                if ((sectorForward == null) || (sectorStart == null))
                {
                    newZ = (int)loc.Z;
                    return false;
                }

                if (sectorStart == sectorForward)
                {
                    for (int i = 0; i < sectorStart.Items.Count; ++i)
                    {
                        Entity entity = sectorStart.Items[i].OwnerEntity; // Item item = sectorStart.Items[i];

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
                    for (int i = 0; i < sectorForward.Items.Count; ++i)
                    {
                        Entity entity = sectorForward.Items[i].OwnerEntity; // Item item = sectorForward.Items[i];

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

                    for (int i = 0; i < sectorStart.Items.Count; ++i)
                    {
                        Entity entity = sectorStart.Items[i].OwnerEntity; // Item item = sectorStart.Items[i];

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

            bool moveIsOk = check(map, m, itemsForward, xForward, yForward, startTop, startZ, out newZ);

            if (moveIsOk && checkDiagonals)
            {
                int hold;

                if (!check(map, m, itemsLeft, xLeft, yLeft, startTop, startZ, out hold) && !check(map, m, itemsRight, xRight, yRight, startTop, startZ, out hold))
                    moveIsOk = false;
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

        private static bool check(Map map, Mobile m, List<Item> items, int x, int y, int startTop, int startZ, out int newZ)
        {
            newZ = 0;

            MapTile mapTile = map.GetMapTile(x, y, false);
            if (mapTile == null)
                return false;

            MapObjectStatic[] tiles = mapTile.GetStatics().ToArray();
            MapObjectGround landTile = mapTile.GroundTile;
            Data.LandData landData = Data.TileData.LandData[landTile.ItemID & 0x3FFF];


            bool landBlocks = (landData.Flags & TileFlag.Impassable) != 0;
            bool considerLand = !landTile.Ignored;

            //if (landBlocks && canSwim && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) != 0)	//Impassable, Can Swim, and Is water.  Don't block it.
            //    landBlocks = false;
            // else 
            // if (cantWalk && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) == 0)	//Can't walk and it's not water
            //     landBlocks = true;

            int landZ = 0, landCenter = 0, landTop = 0;

            map.GetAverageZ(x, y, ref landZ, ref landCenter, ref landTop);

            bool moveIsOk = false;

            int stepTop = startTop + StepHeight;
            int checkTop = startZ + PersonHeight;

            bool ignoreDoors = (!m.Alive || m.BodyID == 0x3DB);

            #region Tiles
            for (int i = 0; i < tiles.Length; ++i)
            {
                MapObjectStatic tile = tiles[i];
                Data.ItemData itemData = Data.TileData.ItemData[tile.ItemID & 0x3FFF];
                TileFlag flags = itemData.Flags;

                if ((flags & ImpassableSurface) == TileFlag.Surface) //  || (canSwim && (flags & TileFlag.Wet) != 0) Surface && !Impassable
                {
                    // if (cantWalk && (flags & TileFlag.Wet) == 0)
                    //     continue;

                    int itemZ = tile.Z;
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

                    if (!itemData.Bridge)
                        itemTop += itemData.Height;

                    if (stepTop >= itemTop)
                    {
                        int landCheck = itemZ;

                        if (itemData.Height >= StepHeight)
                            landCheck += StepHeight;
                        else
                            landCheck += itemData.Height;

                        if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landZ)
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
                Data.ItemData itemData = item.ItemData;
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

                    if (!itemData.Bridge)
                        itemTop += itemData.Height;

                    if (stepTop >= itemTop)
                    {
                        int landCheck = itemZ;

                        if (itemData.Height >= StepHeight)
                            landCheck += StepHeight;
                        else
                            landCheck += itemData.Height;

                        if (considerLand && landCheck < landCenter && landCenter > ourZ && testTop > landZ)
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

            if (considerLand && !landBlocks && (stepTop) >= landZ)
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

        private static bool IsOk(bool ignoreDoors, int ourZ, int ourTop, MapObjectStatic[] tiles, List<Item> items)
        {
            for (int i = 0; i < tiles.Length; ++i)
            {
                MapObjectStatic check = tiles[i];
                Data.ItemData itemData = Data.TileData.ItemData[check.ItemID & 0x3FFF];

                if ((itemData.Flags & ImpassableSurface) != 0) // Impassable || Surface
                {
                    int checkZ = check.Z;
                    int checkTop = checkZ + itemData.CalcHeight;

                    if (checkTop > ourZ && ourTop > checkZ)
                        return false;
                }
            }

            for (int i = 0; i < items.Count; ++i)
            {
                Item item = items[i];
                int itemID = item.ItemID & 0x3FFF;
                Data.ItemData itemData = Data.TileData.ItemData[itemID];
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

        public static Vector3 OffsetTile(Vector3 currentTile, Direction facing)
        {
            Vector3 nextTile = currentTile;

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

        private static void offsetXY(Direction d, ref int x, ref int y)
        {
            switch (d & Direction.FacingMask)
            {
                case Direction.North: --y; break;
                case Direction.South: ++y; break;
                case Direction.West: --x; break;
                case Direction.East: ++x; break;
                case Direction.Right: ++x; --y; break;
                case Direction.Left: --x; ++y; break;
                case Direction.Down: ++x; ++y; break;
                case Direction.Up: --x; --y; break;
            }
        }

        private static void getStartZ(Entity m, Map map, Vector3 loc, List<Item> itemList, out int zLow, out int zTop)
        {
            int xCheck = (int)loc.X, yCheck = (int)loc.Y;

            MapTile mapTile = map.GetMapTile(xCheck, yCheck, false);
            if (mapTile == null)
            {
                zLow = int.MinValue;
                zTop = int.MinValue;
            }

            MapObjectGround landTile = mapTile.GroundTile; //map.Tiles.GetLandTile(xCheck, yCheck);
            int landZ = 0, landCenter = 0, landTop = 0;
            bool landBlocks = (Data.TileData.LandData[landTile.ItemID & 0x3FFF].Flags & TileFlag.Impassable) != 0; //(TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Impassable) != 0;

            // if (landBlocks && m.CanSwim && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) != 0)
            //     landBlocks = false;
            // else if (m.CantWalk && (TileData.LandTable[landTile.ID & 0x3FFF].Flags & TileFlag.Wet) == 0)
            //     landBlocks = true;

            map.GetAverageZ(xCheck, yCheck, ref landZ, ref landCenter, ref landTop);

            bool considerLand = !landTile.Ignored;

            int zCenter = zLow = zTop = 0;
            bool isSet = false;

            if (considerLand && !landBlocks && loc.Z >= landCenter)
            {
                zLow = landZ;
                zCenter = landCenter;

                if (!isSet || landTop > zTop)
                    zTop = landTop;

                isSet = true;
            }

            MapObjectStatic[] staticTiles = mapTile.GetStatics().ToArray();

            for (int i = 0; i < staticTiles.Length; ++i)
            {
                MapObjectStatic tile = staticTiles[i];
                Data.ItemData id = Data.TileData.ItemData[tile.ItemID & 0x3FFF];

                int calcTop = (tile.Z + id.CalcHeight);

                if ((!isSet || calcTop >= zCenter) && ((id.Flags & TileFlag.Surface) != 0) && loc.Z >= calcTop)
                {
                    //  || (m.CanSwim && (id.Flags & TileFlag.Wet) != 0)
                    // if (m.CantWalk && (id.Flags & TileFlag.Wet) == 0)
                    //     continue;

                    zLow = tile.Z;
                    zCenter = calcTop;

                    int top = tile.Z + id.Height;

                    if (!isSet || top > zTop)
                        zTop = top;

                    isSet = true;
                }
            }

            for (int i = 0; i < itemList.Count; ++i)
            {
                Item item = itemList[i];

                Data.ItemData id = item.ItemData;

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
                zLow = zTop = (int)loc.Z;
            else if (loc.Z > zTop)
                zTop = (int)loc.Z;
        }
    }
}
