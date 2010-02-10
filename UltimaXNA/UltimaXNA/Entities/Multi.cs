/***************************************************************************
 *   Multi.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : Feb 09, 2010
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
using System.Collections.Generic;
using UltimaXNA.Data;
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    class Multi : Entity
    {
        MultiComponentList _components;
        List<Point2D> _unloadedTiles = new List<Point2D>();

        int _ItemID;
        public int ItemID
        {
            get { return _ItemID; }
            set
            {
                _ItemID = value;
                HasBeenDrawn = false;
                _components = Multis.GetComponents(_ItemID);
                _unloadedTiles.Clear();
                for (int y = 0; y < _components.Height; y++)
                {
                    for (int x = 0; x < _components.Width; x++)
                    {
                        _unloadedTiles.Add(new Point2D(x, y));
                    }
                }
            }
        }

        public Multi(Serial serial, IWorld world)
			: base(serial, world)
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

                MapTile t = World.Map.GetMapTile(x, y, false);
                if (t != null)
                {
                    drawnTiles.Add(p);

                    foreach (StaticTile s in _components.Tiles[p.X][p.Y])
                    {
                        t.Add(new MapObjectStatic(s, 0, new Position3D(x, y, s.Z)));
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
