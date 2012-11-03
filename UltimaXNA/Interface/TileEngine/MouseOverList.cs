/***************************************************************************
 *   MouseOverList.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Interface.Graphics;

namespace UltimaXNA.Interface.TileEngine
{
    class MouseOverList
    {
        List<MouseOverItem> _overList;

        Vector3 _mousePosition;
        public Point2D MousePosition
        {
            set
            {
                _mousePosition = new Vector3(value.X, value.Y, 1);
            }
        }

        public MouseOverList()
        {
            _overList = new List<MouseOverItem>();
        }

        internal MouseOverItem GetForemostMouseOverItem(Point2D mousePosition)
        {
            // Parse list backwards to find topmost mouse over object.
            foreach (MouseOverItem item in CreateReverseIterator(_overList))
            {
                if (item.Contains(mousePosition))
                {
                    return item;
                }
            }
            return null;
        }

        internal MouseOverItem GetForemostMouseOverItem<T>(Point2D mousePosition) where T : MapObject
        {
            // Parse list backwards to find topmost mouse over object.
            foreach (MouseOverItem item in CreateReverseIterator(_overList))
            {
                if (item.Object.GetType() == typeof(T))
                {
                    if (item.Contains(mousePosition))
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        static IEnumerable<MouseOverItem> CreateReverseIterator<MouseOverItem>(IList<MouseOverItem> list)
        {
            int count = list.Count;
            for (int i = count - 1; i >= 0; --i)
            {
                yield return list[i];
            }
        }

        public void Add2DItem(MouseOverItem item)
        {
            _overList.Add(item);
        }

        public bool IsMouseInObject(Vector3 min, Vector3 max)
        {
            BoundingBox iBoundingBox = new BoundingBox(min, max);
            // Must correct for bounding box being one pixel larger than actual texture.
            iBoundingBox.Max.X--; iBoundingBox.Max.Y--;

            Vector3 iMousePosition = new Vector3(_mousePosition.X, _mousePosition.Y, min.Z);
            if (iBoundingBox.Contains(iMousePosition) == ContainmentType.Contains)
                return true;
            return false;
        }

        public bool IsMouseInObjectIsometric(VertexPositionNormalTextureHue[] v)
        {
            if (v.Length != 4)
                return false;

            float high = -50000, low = 50000;
            for (int i = 0; i < 4; i++)
            {
                if (v[i].Position.Y > high)
                    high = v[i].Position.Y;
                if (v[i].Position.Y < low)
                    low = v[i].Position.Y;
            }

            if (high < _mousePosition.Y)
                return false;
            if (low > _mousePosition.Y)
                return false;
            if (v[1].Position.X < _mousePosition.X)
                return false;
            if (v[2].Position.X > _mousePosition.X)
                return false;

            float minX = v[0].Position.X, maxX = v[0].Position.X;
            float minY = v[0].Position.Y, maxY = v[0].Position.Y;

            for (int i = 1; i < v.Length; i++)
            {
                if (v[i].Position.X < minX)
                    minX = v[i].Position.X;
                if (v[i].Position.X > maxX)
                    maxX = v[i].Position.X;
                if (v[i].Position.Y < minY)
                    minY = v[i].Position.Y;
                if (v[i].Position.Y > maxY)
                    maxY = v[i].Position.Y;
            }

            // Added cursor picking -Poplicola 5/19/2009
            BoundingBox iBoundingBox = new BoundingBox(new Vector3(minX, minY, 0), new Vector3(maxX, maxY, 10));
            if (iBoundingBox.Contains(_mousePosition) == ContainmentType.Contains)
            {
                Point[] p = new Point[4];
                p[0] = new Point((int)v[0].Position.X, (int)v[0].Position.Y);
                p[1] = new Point((int)v[1].Position.X, (int)v[1].Position.Y);
                p[2] = new Point((int)v[3].Position.X, (int)v[3].Position.Y);
                p[3] = new Point((int)v[2].Position.X, (int)v[2].Position.Y);
                if (pointInPolygon(new Point((int)_mousePosition.X, (int)_mousePosition.Y), p))
                {
                    return true;
                }
            }
            return false;
        }

        private bool pointInPolygon(Point p, Point[] poly)
        {
            // Taken from http://social.msdn.microsoft.com/forums/en-US/winforms/thread/95055cdc-60f8-4c22-8270-ab5f9870270a/
            Point p1, p2;
            bool inside = false;
            if (poly.Length < 3)
            {
                return inside;
            }
            Point oldPoint = new Point(
                poly[poly.Length - 1].X, poly[poly.Length - 1].Y);

            for (int i = 0; i < poly.Length; i++)
            {
                Point newPoint = new Point(poly[i].X, poly[i].Y);
                if (newPoint.X > oldPoint.X)
                {
                    p1 = oldPoint;
                    p2 = newPoint;
                }
                else
                {
                    p1 = newPoint;
                    p2 = oldPoint;
                }

                if ((newPoint.X < p.X) == (p.X <= oldPoint.X)
                    && ((long)p.Y - (long)p1.Y) * (long)(p2.X - p1.X)
                    < ((long)p2.Y - (long)p1.Y) * (long)(p.X - p1.X))
                {
                    inside = !inside;
                }
                oldPoint = newPoint;
            }
            return inside;
        }
    }

    class MouseOverItem
    {
        public Vector3[] Vertices;
        public Texture2D Texture;
        public Vector3 Position;
        public Vector2 InTexturePosition;
        public MapObject Object;

        internal MouseOverItem(Texture2D nTexture, Vector3 nPosition, MapObject nObject)
        {
            Texture = nTexture;
            Position = nPosition;
            Object = nObject;
        }

        internal bool Contains(Point2D mousePosition)
        {
            if (Object.GetType() == typeof(MapObjectGround))
            {
                // we already know we are within this polygon
                InTexturePosition = new Vector2(mousePosition.X - Position.X, mousePosition.Y - Position.Y);
                return true;
            }
            else
            {
                uint[] iPixel = new uint[1];
                Rectangle pRect = new Rectangle((int)mousePosition.X - (int)Position.X, (int)mousePosition.Y - (int)Position.Y, 1, 1);
                if (Texture.Bounds.Contains(new Point(pRect.X, pRect.Y)))
                {
                    Texture.GetData<uint>(0, pRect, iPixel, 0, 1);
                    if (iPixel[0] != 0)
                    {
                        InTexturePosition = new Vector2(pRect.X, pRect.Y);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
