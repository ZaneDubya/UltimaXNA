using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace InterXLib.Display
{
    /// <summary>
    /// A collection of Shapes.
    /// </summary>
    public class Shape
    {
        public VertexPositionTextureHueExtra[] Vertices;
        private static int max_radius_for_smooth_circle = 512;

        private static int segmentsForSmoothCircle(float radius)
        {
            if (radius > max_radius_for_smooth_circle)
                radius = max_radius_for_smooth_circle;
            float circumference = Math.PI * 2 * radius;
            int numSegments = (int)(circumference / 24) + 20;
            return numSegments;
        }

        public static Shape CreateFilledCircle(Vector3 origin, float border_radius, float border_width, float border_origin_radians, Vector4 uv)
        {
            int numSegments = segmentsForSmoothCircle(border_radius);

            float radiansPerPoint = (Math.PI * 2) / numSegments;
            Vector3[] pointsOutside = new Vector3[numSegments + 1], pointsInside = new Vector3[numSegments + 1];

            // really, we only need to generate numPoints, but generating the final point (which is equal to the first point
            // saves us a mod operation in the next for loop.
            for (int i = 0; i < numSegments + 1; i++)
            {
                pointsOutside[i] = new Vector3(
                    origin.X + Math.Cos(radiansPerPoint * i + border_origin_radians) * (border_radius),
                    origin.Y + Math.Sin(radiansPerPoint * i + border_origin_radians) * (border_radius),
                    origin.Z);
                pointsInside[i] = new Vector3(
                    origin.X + Math.Cos(radiansPerPoint * i + border_origin_radians) * (border_radius - border_width),
                    origin.Y + Math.Sin(radiansPerPoint * i + border_origin_radians) * (border_radius - border_width),
                    origin.Z);
            }

            VertexPositionTextureHueExtra[] vertices = new VertexPositionTextureHueExtra[(numSegments) * 4];

            for (int i = 0; i < numSegments; i++)
            {
                int i4 = i * 4;
                int i6 = i * 6;

                vertices[i4 + 0] = new VertexPositionTextureHueExtra(pointsOutside[i + 0], new Vector2(uv.X, uv.Y));
                vertices[i4 + 1] = new VertexPositionTextureHueExtra(pointsOutside[i + 1], new Vector2(uv.Z, uv.Y));
                vertices[i4 + 2] = new VertexPositionTextureHueExtra(pointsInside[i + 0], new Vector2(uv.X, uv.W));
                vertices[i4 + 3] = new VertexPositionTextureHueExtra(pointsInside[i + 1], new Vector2(uv.Z, uv.W));
            }

            Shape shape = new Shape();
            shape.Vertices = vertices;

            return shape;
        }

        public enum BorderStyle
        {
            Centered,
            Inside,
            Outside
        }
        
        /// <summary>
        /// Creates a circle shape with a designated border width. The circle begins and ends at border_origin_radians.
        /// </summary>
        public static Shape CreateBorderedCircle(Vector3 origin, float radius, float border_width, float border_origin_radians, Vector4 uv, BorderStyle style, Color? hue = null)
        {
            float inner_delta = 0f, outer_delta = 0f;
            switch (style)
            {
                case BorderStyle.Centered:
                    inner_delta = outer_delta = border_width / 2f;
                    break;
                case BorderStyle.Inside:
                    inner_delta = border_width;
                    break;
                case BorderStyle.Outside:
                    outer_delta = border_width;
                    break;
            }

            int numSegments = segmentsForSmoothCircle(radius);

            float radiansPerPoint = (Math.PI * 2) / numSegments;
            Vector3[] pointsOutside = new Vector3[numSegments + 1], pointsInside = new Vector3[numSegments + 1];

            // really, we only need to generate numPoints, but generating the final point (which is equal to the first point
            // saves us a mod operation in the next for loop.
            for (int i = 0; i < numSegments + 1; i++)
            {
                pointsOutside[i] = new Vector3(
                    origin.X + Math.Cos(radiansPerPoint * i + border_origin_radians) * (radius + outer_delta),
                    origin.Y + Math.Sin(radiansPerPoint * i + border_origin_radians) * (radius + outer_delta),
                    origin.Z);
                pointsInside[i] = new Vector3(
                    origin.X + Math.Cos(radiansPerPoint * i + border_origin_radians) * (radius - inner_delta),
                    origin.Y + Math.Sin(radiansPerPoint * i + border_origin_radians) * (radius - inner_delta),
                    origin.Z);
            }

            VertexPositionTextureHueExtra[] vertices = new VertexPositionTextureHueExtra[(numSegments) * 4];

            Color color = (hue.HasValue) ? hue.Value : Color.White;

            for (int i = 0; i < numSegments; i++)
            {
                int i4 = i * 4;
                int i6 = i * 6;

                vertices[i4 + 0] = new VertexPositionTextureHueExtra(pointsOutside[i + 0], new Vector2(uv.X, uv.Y), color, Vector4.Zero);
                vertices[i4 + 1] = new VertexPositionTextureHueExtra(pointsOutside[i + 1], new Vector2(uv.Z, uv.Y), color, Vector4.Zero);
                vertices[i4 + 2] = new VertexPositionTextureHueExtra(pointsInside[i + 0], new Vector2(uv.X, uv.W), color, Vector4.Zero);
                vertices[i4 + 3] = new VertexPositionTextureHueExtra(pointsInside[i + 1], new Vector2(uv.Z, uv.W), color, Vector4.Zero);
            }

            Shape shape = new Shape();
            shape.Vertices = vertices;

            return shape;
        }

        public static Shape CreateQuadCentered(Vector3 origin, float radius, Vector4 uv, float? rotation = null, Color? hue = null, Vector4? extra = null)
        {
            float iRotation = (rotation == null) ? 0.0f : rotation.Value;
            Color iHue = (hue == null) ? Color.White : hue.Value;
            Vector4 iExtra = (extra == null) ? Vector4.Zero : extra.Value;

            Shape shape = new Shape();
            if (iRotation == 0)
            {
                shape.Vertices = new VertexPositionTextureHueExtra[]{
                    new VertexPositionTextureHueExtra(origin + new Vector3(-radius, -radius, 0f), new Vector2(uv.X, uv.Y), iHue, iExtra), // top left
                    new VertexPositionTextureHueExtra(origin + new Vector3(+radius, -radius, 0f), new Vector2(uv.Z, uv.Y), iHue, iExtra), // top right
                    new VertexPositionTextureHueExtra(origin + new Vector3(-radius, +radius, 0f), new Vector2(uv.X, uv.W), iHue, iExtra), // bottom left
                    new VertexPositionTextureHueExtra(origin + new Vector3(+radius, +radius, 0f), new Vector2(uv.Z, uv.W), iHue, iExtra), // bottom right
                };
            }
            else
            {
                shape.Vertices = new VertexPositionTextureHueExtra[]{
                    new VertexPositionTextureHueExtra(origin + rotateV3(new Vector3(-radius, -radius, 0f), iRotation), new Vector2(uv.X, uv.Y), iHue, iExtra), // top left
                    new VertexPositionTextureHueExtra(origin + rotateV3(new Vector3(+radius, -radius, 0f), iRotation), new Vector2(uv.Z, uv.Y), iHue, iExtra), // top right
                    new VertexPositionTextureHueExtra(origin + rotateV3(new Vector3(-radius, +radius, 0f), iRotation), new Vector2(uv.X, uv.W), iHue, iExtra), // bottom left
                    new VertexPositionTextureHueExtra(origin + rotateV3(new Vector3(+radius, +radius, 0f), iRotation), new Vector2(uv.Z, uv.W), iHue, iExtra), // bottom right
                };
            }

            return shape;
        }

        private static Vector3 rotateV3(Vector3 v, float radians)
        {
            Vector3 v_out = new Vector3(0, 0, v.Z);
            v_out.X = (float)(v.X * Math.Cos(radians) - v.Y * Math.Sin(radians));
            v_out.Y = (float)(v.Y * Math.Cos(radians) + v.X * Math.Sin(radians));
            return v_out;
        }

        public static Shape CreateQuad(Vector3 position, Vector2 area, Vector4 uv, Color? hue = null, Vector4? extra = null)
        {
            Shape shape = new Shape();
            Color iHue = (hue == null) ? Color.White : hue.Value;
            Vector4 iExtra = (extra == null) ? Vector4.Zero : extra.Value;

            shape.Vertices = new VertexPositionTextureHueExtra[]{
                new VertexPositionTextureHueExtra(position, new Vector2(uv.X, uv.Y), iHue, iExtra), // top left
                new VertexPositionTextureHueExtra(position + new Vector3(area.X, 0, 0), new Vector2(uv.Z, uv.Y), iHue, iExtra), // top right
                new VertexPositionTextureHueExtra(position + new Vector3(0, area.Y, 0), new Vector2(uv.X, uv.W), iHue, iExtra), // bottom left
                new VertexPositionTextureHueExtra(position + new Vector3(area, 0), new Vector2(uv.Z, uv.W), iHue, iExtra), // bottom right
            };

            return shape;
        }

        public static Shape CreateLine(Vector3 start, Vector3 end, Color? hue = null, Vector4? extra = null)
        {
            Shape shape = new Shape();
            Color iHue = (hue == null) ? Color.White : hue.Value;
            Vector4 iExtra = (extra == null) ? Vector4.Zero : extra.Value;

            float atan = Math.Atan(-(end.Y - start.Y) / (end.X - start.X));
            Vector3 offset = new Vector3(Math.Sin(atan) / 2f, Math.Cos(atan) / 2f, 0);
            shape.Vertices = new VertexPositionTextureHueExtra[]{
                new VertexPositionTextureHueExtra(start - offset, new Vector2(0, 0), iHue, iExtra), // top left
                new VertexPositionTextureHueExtra(start + offset, new Vector2(1, 0), iHue, iExtra), // top right
                new VertexPositionTextureHueExtra(end - offset, new Vector2(0, 1), iHue, iExtra), // bottom left
                new VertexPositionTextureHueExtra(end + offset, new Vector2(1, 1), iHue, iExtra), // bottom right
            };

            return shape;
        }

        public static Shape CreateVector(Vector3 v0, Vector3 v1, float width, Vector4 uv, float cut_short = 0f, Color? hue = null, Vector4? extra = null)
        {
            Shape shape = new Shape();
            Color iHue = (hue == null) ? Color.White : hue.Value;
            Vector4 iExtra = (extra == null) ? Vector4.Zero : extra.Value;

            Vector3 normalized = Vector3.Normalize((v1 - v0));
            Vector3 perpendicular = new Vector3(normalized.Y, -normalized.X, 0) * width;
            Vector3 v1shortened = v1 - normalized * cut_short;

            shape.Vertices = new VertexPositionTextureHueExtra[]{
                new VertexPositionTextureHueExtra(v0 - perpendicular, new Vector2(uv.X, uv.Y), iHue, iExtra), // top left
                new VertexPositionTextureHueExtra(v0 + perpendicular, new Vector2(uv.Z, uv.Y), iHue, iExtra), // top right
                new VertexPositionTextureHueExtra(v1shortened - perpendicular, new Vector2(uv.X, uv.W), iHue, iExtra), // bottom left
                new VertexPositionTextureHueExtra(v1shortened + perpendicular, new Vector2(uv.Z, uv.W), iHue, iExtra), // bottom right
            };

            return shape;
        }
    }
}
