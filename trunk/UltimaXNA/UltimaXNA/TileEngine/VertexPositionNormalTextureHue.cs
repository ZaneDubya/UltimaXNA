/***************************************************************************
 *   VertexPositionNormalTextureHue.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace UltimaXNA.TileEngine
{
    public struct VertexPositionNormalTextureHue
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector3 TextureCoordinate;
        public Vector2 Hue;

        public VertexPositionNormalTextureHue(Vector3 Position, Vector3 Normal, Vector3 TextureCoordinate)
        {
            this.Position = Position;
            this.Normal = Normal;
            this.TextureCoordinate = TextureCoordinate;
            this.Hue = Vector2.Zero;
        }

        public static readonly VertexElement[] VertexElements = new VertexElement[]
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0),
            new VertexElement(0, sizeof(float) * 6, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(0, sizeof(float) * 9, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
        };

        public static readonly VertexPositionNormalTextureHue[] PolyBuffer = {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
            };

        public static readonly VertexPositionNormalTextureHue[] PolyBufferFlipped = {
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(), new Vector3(0, 0, 1), new Vector3(1, 1, 0))
            };

        public static int SizeInBytes { get { return sizeof( float ) * 11; } }

    }
}
