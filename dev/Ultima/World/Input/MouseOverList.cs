/***************************************************************************
 *   MouseOverList.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.World.Entities.Items;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.World.Input
{
    public class MouseOverList
    {
        List<MouseOverItem> m_overList;

        public PickType PickType = PickType.PickNothing;

        Vector3 m_mousePosition;
        protected Point MousePosition
        {
            set
            {
                m_mousePosition = new Vector3(value.X, value.Y, 1);
            }
        }

        public MouseOverList(MousePicking mousePicking)
        {
            m_overList = new List<MouseOverItem>();
            MousePosition = mousePicking.Position;
            PickType = mousePicking.PickOnly;
        }

        internal MouseOverItem GetForemostMouseOverItem(Point mousePosition)
        {
            // Parse list backwards to find topmost mouse over object.
            foreach (MouseOverItem item in CreateReverseIterator(m_overList))
            {
                if (item.TextureContainsPoint(mousePosition))
                {
                    return item;
                }
            }
            return null;
        }

        internal MouseOverItem GetForemostMouseOverItem<T>(Point mousePosition) where T : AEntity
        {
            // Parse list backwards to find topmost mouse over object.
            foreach (MouseOverItem item in CreateReverseIterator(m_overList))
            {
                if (item.Entity.GetType() == typeof(T))
                {
                    if (item.TextureContainsPoint(mousePosition))
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
            m_overList.Add(item);
        }

        public bool IsMouseInObject(Vector3 min, Vector3 max)
        {
            BoundingBox iBoundingBox = new BoundingBox(min, max);
            // Must correct for bounding box being one pixel larger than actual texture.
            iBoundingBox.Max.X--; iBoundingBox.Max.Y--;

            Vector3 iMousePosition = new Vector3(m_mousePosition.X, m_mousePosition.Y, min.Z);
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

            if (high < m_mousePosition.Y)
                return false;
            if (low > m_mousePosition.Y)
                return false;
            if (v[1].Position.X < m_mousePosition.X)
                return false;
            if (v[2].Position.X > m_mousePosition.X)
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
            if (iBoundingBox.Contains(m_mousePosition) == ContainmentType.Contains)
            {
                Point[] p = new Point[4];
                p[0] = new Point((int)v[0].Position.X, (int)v[0].Position.Y);
                p[1] = new Point((int)v[1].Position.X, (int)v[1].Position.Y);
                p[2] = new Point((int)v[3].Position.X, (int)v[3].Position.Y);
                p[3] = new Point((int)v[2].Position.X, (int)v[2].Position.Y);
                if (pointInPolygon(new Point((int)m_mousePosition.X, (int)m_mousePosition.Y), p))
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

    public class MouseOverItem
    {
        public Vector3[] Vertices;
        public Texture2D Texture;
        public Vector3 Position;
        public Vector2 InTexturePosition;
        public AEntity Entity;

        internal MouseOverItem(Texture2D texture, Vector3 position, AEntity entity)
        {
            Texture = texture;
            Position = position;
            Entity = entity;
        }

        internal bool TextureContainsPoint(Point mousePosition)
        {
            if (Entity is Ground)
            {
                // we already know we are within this polygon
                InTexturePosition = new Vector2(mousePosition.X - Position.X, mousePosition.Y - Position.Y);
                return true;
            }
            else if (Entity is Item)
            {
                // Allow selection if there is a non-transparent pixel below the mouse cursor or at an offset of
                // (-1,0), (0,-1), (1,0), or (1,1). This will allow selection even when the mouse cursor is directly
                // over a transparent pixel, and will also increase the 'selection space' of an item by one pixel in
                // each dimension - thus a very thin object (2-3 pixels wide) will be increased.

                int x = (int)(mousePosition.X - Position.X);
                int y = (int)(mousePosition.Y - Position.Y);

                if (Texture.Bounds.Contains(new Point(x, y)))
                {
                    if (x == 0)
                        x++;
                    if (x == Texture.Width - 1)
                        x--;
                    if (y == 0)
                        y++;
                    if (y == Texture.Height - 1)
                        y--;

                    ushort[] pixelData = new ushort[9];
                    Texture.GetData<ushort>(0, new Rectangle(x - 1, y - 1, 3, 3), pixelData, 0, 9);
                    if ((pixelData[1] > 0) || (pixelData[3] > 0) ||
                        (pixelData[4] > 0) || (pixelData[5] > 0) ||
                        (pixelData[7] > 0))
                    {
                        InTexturePosition = new Vector2(x, y);
                        return true;
                    }
                }
            }
            else if (Entity is Mobile)
            {
                Rectangle pRect = new Rectangle((int)mousePosition.X - (int)Position.X, (int)mousePosition.Y - (int)Position.Y, 1, 1);
                if (Texture.Bounds.Contains(new Point(pRect.X, pRect.Y)))
                {
                    ushort[] pixelData = new ushort[1];
                    Texture.GetData<ushort>(0, pRect, pixelData, 0, 1);
                    if (pixelData[0] > 0)
                        return true;
                }
            }
            else
            {
                return false;
            }

            return false;
        }
    }
}
