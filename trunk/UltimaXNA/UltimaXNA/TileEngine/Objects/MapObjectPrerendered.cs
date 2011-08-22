using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.Entities;
using UltimaXNA.Graphics;

namespace UltimaXNA.TileEngine
{
    public abstract class MapObjectPrerendered : MapObject
    {
        private static List<MapObjectPrerendered> _objects = new List<MapObjectPrerendered>();

        public static void RenderObjects(SpriteBatch3D sb)
        {
            foreach (MapObjectPrerendered obj in _objects)
                obj.Prerender(sb);
            _objects.Clear();
        }

        public MapObjectPrerendered(Position3D position)
            : base(position)
        {
            _objects.Add(this);
        }

        protected abstract void Prerender(SpriteBatch3D sb);
    }
}
