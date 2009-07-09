using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.TileEngine
{
    public struct VertexPositionNormalTextureHue
    {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TextureCoordinate;
        public Vector2 Hue;

        public VertexPositionNormalTextureHue(Vector3 Position, Vector3 Normal, Vector2 TextureCoordinate)
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
            new VertexElement(0, sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(0, sizeof(float) * 8, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1)
        };

        public static int SizeInBytes { get { return sizeof( float ) * 9 + 4; } }

    }
}
