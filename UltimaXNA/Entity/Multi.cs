/***************************************************************************
 *   Multi.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.UltimaData;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entity
{
    class Multi : BaseEntity
    {
        MultiComponentList _components;
        List<Point2D> _unloadedTiles = new List<Point2D>();

        int _customHouseRevision = 0x7FFFFFFF;
        StaticTile[] _customHouseTiles;
        public int CustomHouseRevision { get { return _customHouseRevision; } }

        bool _hasCustomTiles = false;
        CustomHouse _customHouse;
        public void AddCustomHousingTiles(CustomHouse house)
        {
            _hasCustomTiles = true;
            _customHouse = house;
            _customHouseTiles = house.GetStatics(_components.Width, _components.Height);
            redrawAllTiles();
        }

        int _ItemID;
        public int ItemID
        {
            get { return _ItemID; }
            set
            {
                if (_ItemID != value)
                {
                    _ItemID = value;
                    redrawAllTiles();
                }
            }
        }

        void redrawAllTiles()
        {
            _components = Multis.GetComponents(_ItemID);
            _unloadedTiles.Clear();
            for (int y = 0; y < _components.Height + 1; y++)
            {
                for (int x = 0; x < _components.Width; x++)
                {
                    Point2D p = new Point2D();
                    p.X = x;
                    p.Y = y;
                    _unloadedTiles.Add(p);
                }
            }
        }

        public Multi(Serial serial)
			: base(serial)
		{
		}

        public override void Update(GameTime gameTime)
        {
            if (_unloadedTiles.Count > 0)
                HasBeenDrawn = false;

            base.Update(gameTime);
        }

        internal override void Draw(MapTile tile, Position3D position)
        {
            if (_unloadedTiles.Count == 0)
                return;

            List<Point2D> drawnTiles = new List<Point2D>();

            foreach (Point2D p in _unloadedTiles)
            {
                int x = tile.X + p.X - _components.Center.X;
                int y = tile.Y + p.Y - _components.Center.Y;

                MapTile t = IsometricRenderer.Map.GetMapTile(x, y, false);
                if (t != null)
                {
                    drawnTiles.Add(p);

                    if (!_hasCustomTiles)
                    {
                        if (p.X < _components.Width && p.Y < _components.Height)
                        {
                            foreach (StaticTile s in _components.Tiles[p.X][p.Y])
                            {
                                t.AddMapObject(new MapObjectStatic(s.ID, 0, new Position3D(x, y, s.Z)));
                            }
                        }
                    }
                    else
                    {
                        foreach (StaticTile s in _customHouseTiles)
                        {
                            if ((s.X == p.X) && (s.Y == p.Y))
                            {
                                t.AddMapObject(new MapObjectStatic(s.ID, 0, new Position3D(s.X, s.Y, s.Z)));
                            }
                        }
                    }
                }
            }

            foreach (Point2D p in drawnTiles)
            {
                _unloadedTiles.Remove(p);
            }
       }
    }
}
