﻿/***************************************************************************
 *   CustomHousing.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Network;

namespace UltimaXNA.UltimaData
{
    class CustomHousing
    {
        static Dictionary<Serial, CustomHouse> _customHouses = new Dictionary<Serial,CustomHouse>();

        public static bool IsHashCurrent(Serial serial, int hash)
        {
            if (_customHouses.ContainsKey(serial))
            {
                CustomHouse h = _customHouses[serial];
                if (h.Hash == hash)
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }
            
        }

        public static CustomHouse GetCustomHouseData(Serial serial)
        {
            return _customHouses[serial];
        }

        public static void UpdateCustomHouseData(Serial serial, int hash, int planecount, CustomHousePlane[] planes)
        {
            CustomHouse house;
            if (_customHouses.ContainsKey(serial))
            {
                house = _customHouses[serial];
                
            }
            else
            {
                house = new CustomHouse(serial);
                _customHouses.Add(serial, house);
            }
            house.Update(hash, planecount, planes);
        }
    }

    class CustomHouse
    {
        public Serial Serial;
        public int Hash;

        int _planeCount;
        CustomHousePlane[] _planes;

        public CustomHouse(Serial serial)
        {
            Serial = serial;
        }

        public void Update(int hash, int planecount, CustomHousePlane[] planes)
        {
            Hash = hash;
            _planeCount = planecount;
            _planes = planes;
        }

        public StaticTile[] GetStatics(int width, int height)
        {
            List<StaticTile> statics = new List<StaticTile>();

            // Custom Houses are sent in 'planes' of four different types. We determine which type we're looking at the index and the size.
            int sizeFloor = ((width - 1) * (height - 1));
            int sizeWalls = (width * height);
            // There is no z data for most planes, so we have to determine their z by their relative position to preceeding planes of the same type.
            int numTilesInLastPlane = 0;
            int zIndex = 0;

            for (int plane = 0; plane < _planeCount; plane++)
            {
                int numTiles = _planes[plane].ItemData.Length >> 1;

                if ((plane == _planeCount - 1) &&
                    (numTiles != sizeFloor) &&
                    (numTiles != sizeWalls))
                {
                    numTiles = _planes[plane].ItemData.Length / 5;
                    int index = 0;
                    for (int j = 0; j < numTiles; j++)
                    {
                        StaticTile s = new StaticTile();
                        s.ID = (short)((_planes[plane].ItemData[index++] << 8) + _planes[plane].ItemData[index++]);
                        int x = (sbyte)_planes[plane].ItemData[index++];
                        int y = (sbyte)_planes[plane].ItemData[index++];
                        int z = (sbyte)_planes[plane].ItemData[index++];
                        s.X = (byte)((width >> 1) + x - 1);
                        s.Y = (byte)((height >> 1) + y);
                        s.Z = (sbyte)z;
                        statics.Add(s);
                    }
                }
                else
                {
                    int iWidth = width, iHeight = height;
                    int iX = 0, iY = 0;

                    int x = 0, y = 0, z = 0;

                    if (plane == 0)
                    {
                        zIndex = 0;
                        iWidth += 1;
                        iHeight += 1;
                    }
                    else if (numTiles == sizeFloor)
                    {
                        if (numTilesInLastPlane != sizeFloor)
                            zIndex = 1;
                        else
                            zIndex++;
                        iWidth -= 1;
                        iHeight -= 1;
                        iX = 1;
                        iY = 1;
                    }
                    else if (numTiles == sizeWalls)
                    {
                        if (numTilesInLastPlane != sizeWalls)
                            zIndex = 1;
                        else
                            zIndex++;
                    }



                    switch (zIndex)
                    {
                        case 0: z = 0; break;
                        case 1: z = 7; break;
                        case 2: z = 27; break;
                        case 3: z = 47; break;
                        case 4: z = 67; break;
                        default: continue;
                    }

                    int index = 0;
                    for (int j = 0; j < numTiles; j++)
                    {
                        StaticTile s = new StaticTile();
                        s.ID = (short)((_planes[plane].ItemData[index++] << 8) + _planes[plane].ItemData[index++]);
                        s.X = (byte)(x + iX);
                        s.Y = (byte)(y + iY);
                        s.Z = (sbyte)z;
                        y++;
                        if (y >= iHeight)
                        {
                            y = 0;
                            x++;
                        }
                        statics.Add(s);
                    }
                    numTilesInLastPlane = numTiles;
                }
            }
            return statics.ToArray();
        }
    }

    public class CustomHousePlane
    {
        public readonly int Index;
        public readonly bool IsFloor;
        public readonly byte[] ItemData;

        public CustomHousePlane(PacketReader reader)
        {
            byte[] data = reader.ReadBytes(4);
            Index = data[0];
            int uncompressedsize = data[1] + ((data[3] & 0xF0) << 4);
            int compressedLength = data[2] + ((data[3] & 0xF) << 8);
            ItemData = new byte[uncompressedsize];
            Compression.Unpack(ItemData, ref uncompressedsize, reader.ReadBytes(compressedLength), compressedLength);

            IsFloor = ((Index & 0x20) == 0x20);
            Index &= 0x1F;
        }
    }
}
