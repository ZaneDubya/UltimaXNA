using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.Display
{
    public struct VertexPositionTextureHueExtra : IVertexType
    {
        public Vector3 Position;
        public Vector2 TextureCoordinate;
        public Color Hue;
        public Vector4 Extra;

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 5, VertexElementFormat.Color, VertexElementUsage.Color, 0),
            new VertexElement(sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1)
        );

        VertexDeclaration IVertexType.VertexDeclaration { get { return VertexDeclaration; } }

        public VertexPositionTextureHueExtra(Vector3 position, Vector2 textureCoordinate)
        {
            this.Position = position;
            this.TextureCoordinate = textureCoordinate;
            Hue = Color.White;
            this.Extra = Vector4.Zero;
        }

        public VertexPositionTextureHueExtra(Vector3 position, Vector2 textureCoordinate, Color hue, Vector4 extra)
        {
            this.Position = position;
            this.TextureCoordinate = textureCoordinate;
            Hue = hue;
            this.Extra = extra;
        }

        public static readonly VertexPositionTextureHueExtra[] PolyBuffer = {
                new VertexPositionTextureHueExtra(new Vector3(), new Vector2(0, 0)),
                new VertexPositionTextureHueExtra(new Vector3(), new Vector2(1, 0)),
                new VertexPositionTextureHueExtra(new Vector3(), new Vector2(0, 1)),
                new VertexPositionTextureHueExtra(new Vector3(), new Vector2(1, 1))
            };

        public static readonly VertexPositionTextureHueExtra[] PolyBufferFlipped = {
                new VertexPositionTextureHueExtra(new Vector3(),  new Vector2(0, 0)),
                new VertexPositionTextureHueExtra(new Vector3(), new Vector2(0, 1)),
                new VertexPositionTextureHueExtra(new Vector3(), new Vector2(1, 0)),
                new VertexPositionTextureHueExtra(new Vector3(), new Vector2(1, 1))
            };

        public static int SizeInBytes { get { return sizeof(float) * 10; } }

        public override string ToString()
        {
            return string.Format("VPTHE: <{0}> <{1}>", this.Position.ToString(), this.TextureCoordinate.ToString());
        }
    }
}