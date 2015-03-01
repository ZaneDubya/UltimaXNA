/***************************************************************************
 *   MapObjectPrerendered.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from ClintXNA's renderer: http://www.runuo.com/forums/xna/92023-hi.html
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.Entity;
using UltimaXNA.Rendering;

namespace UltimaXNA.UltimaWorld
{
    public abstract class MapObjectPrerendered : AMapObject
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

        private const int _kCapacity = 0x1000;
        private static long[] _textureHashes = new long[_kCapacity];
        private static Dictionary<long, TextureAndOffsets> _textureLibrary = new Dictionary<long, TextureAndOffsets>(_kCapacity);
        private static int _textureHashes_LastHashRemoved = 0;

        protected static void SavePrerenderedTexture(Texture2D texture, long hash, int xOffset, int yOffset)
        {
            if (_textureHashes[_textureHashes_LastHashRemoved] != 0)
                _textureLibrary.Remove(_textureHashes[_textureHashes_LastHashRemoved]);
            _textureLibrary.Add(hash, new TextureAndOffsets(texture, xOffset, yOffset));
            _textureHashes[_textureHashes_LastHashRemoved] = hash;
            _textureHashes_LastHashRemoved++;
            if (_textureHashes_LastHashRemoved >= 1000)
                _textureHashes_LastHashRemoved = 0;
        }

        protected static Texture2D RestorePrerenderedTexture(long hash, out int xOffset, out int yOffset)
        {
            if (_textureLibrary.ContainsKey(hash))
            {
                TextureAndOffsets t = _textureLibrary[hash];
                xOffset = t.X;
                yOffset = t.Y;
                return t.Texture;
            }
            else
            {
                xOffset = 0;
                yOffset = 0;
                return null;
            }
        }
    }

    struct TextureAndOffsets
    {
        public Texture2D Texture;
        public int X, Y;

        public TextureAndOffsets(Texture2D t, int x, int y)
        {
            Texture = t;
            X = x;
            Y = y;
        }
    }
}
