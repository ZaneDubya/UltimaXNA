using InterXLib.Display;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace InterXLib.XGUI.Rendering
{
    class HorizontalSlidingDoorRenderer : ARenderer
    {
        private readonly Rectangle _center, _edge;
        private readonly int _buffer;

        public int Buffer
        {
            get { return _buffer; }
        }

        public HorizontalSlidingDoorRenderer(Texture2D texture, Rectangle source, int center, int edge, int buffer)
            : base(texture)
        {
            _center = new Rectangle(source.Left, source.Top, center, source.Height);
            _edge = new Rectangle(source.Left + center, source.Top, edge, source.Height);
            _buffer = buffer;
        }

        public override Rectangle BuildChildArea(Point area)
        {
            return new Rectangle(0, 0, area.X, area.Y);
        }

        public override void Render(YSpriteBatch batch, Rectangle destination, Color? color = null)
        {
            Color Color = (color == null) ? Color.White : color.Value;
            Rectangle drawArea = new Rectangle(destination.Left, destination.Top, _edge.Width, _edge.Height);

            batch.GUIDrawSprite(Texture, drawArea, _edge, Color, SpriteEffects.FlipHorizontally);

            drawArea.X = destination.Right - _edge.Width;
            batch.GUIDrawSprite(Texture, drawArea, _edge, Color);

            drawArea.X = destination.Left + _edge.Width;
            drawArea.Width = destination.Width - (2 * _edge.Width);
            batch.GUIDrawSprite(Texture, drawArea, _center, Color);
        }
    }
}
