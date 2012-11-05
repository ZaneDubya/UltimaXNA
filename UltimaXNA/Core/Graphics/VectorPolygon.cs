/***************************************************************************
 *   VectorPolygon.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   Based on LineBatch.cs, made available as part of the 
 *   Microsoft XNA Community Game Platform
 *   
 *   Copyright (C) Microsoft Corporation. All rights reserved.
 *   
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Interface.Graphics
{
    /// <summary>
    /// A series of points that may be drawn together to form a line.
    /// </summary>
    public class VectorPolygon
    {
        /// <summary>
        /// The raw set of points, in "model space".
        /// </summary>
        private Vector3[] _points;
        public Vector3[] Points
        {
            get { return _points; }
        }

        private bool _isClosed;
        public bool IsClosed
        {
            get { return _isClosed; }
        }

        /// <summary>
        /// Constructs a new VectorPolygon object from the given points.
        /// </summary>
        /// <param name="points">The raw set of points.</param>
        public VectorPolygon(Vector3[] points, bool isClosed)
        {
            this._points = points;
            _isClosed = isClosed;
        }

        /// <summary>
        /// Creates a polygon in the shape of a circle.
        /// </summary>
        /// <param name="center">The offset of the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="segments">The number of segments used in the circle.</param>
        /// <returns>A new VectorPolygon object in the shape of a circle.</returns>
        public static VectorPolygon CreateCircle(Vector2 center, float radius,
            int segments)
        {
            Vector3[] points = new Vector3[segments];
            float angle = MathHelper.TwoPi / points.Length;

            for (int i = 0; i <= points.Length - 1; i++)
            {
                points[i] = new Vector3(
                    center.X + radius * (float)Math.Round(Math.Sin(angle * i), 4),
                    center.Y + radius * (float)Math.Round(Math.Cos(angle * i), 4),
                    0);
            }

            return new VectorPolygon(points, true);
        }
    }
}
