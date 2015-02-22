using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using InterXLib.Display;

namespace InterXLib.XGUI.Rendering
{
    class BorderedRenderer : ARenderer
    {
        private Rectangle m_UL, m_UR, m_LL, m_LR;
        private Rectangle m_T, m_R, m_B, m_L;
        private Rectangle m_BG;

        public int BorderWidth { get; protected set; }

        public BorderedRenderer(Texture2D texture, Rectangle source, int borderWidth)
            : base(texture)
        {
            BorderWidth = borderWidth;

            m_UL = new Rectangle(source.Left, source.Top, BorderWidth, BorderWidth);
            m_UR = new Rectangle(source.Right - BorderWidth, source.Top, BorderWidth, BorderWidth);
            m_LL = new Rectangle(source.Left, source.Bottom - BorderWidth, BorderWidth, BorderWidth);
            m_LR = new Rectangle(source.Right - BorderWidth, source.Bottom - BorderWidth, BorderWidth, BorderWidth);

            m_T = new Rectangle(source.Left + BorderWidth, source.Top, source.Width - 2 * BorderWidth, BorderWidth);
            m_R = new Rectangle(source.Right - BorderWidth, source.Top + BorderWidth, BorderWidth, source.Height - 2 * BorderWidth);
            m_B = new Rectangle(source.Left + BorderWidth, source.Bottom - BorderWidth, source.Width - 2 * BorderWidth, BorderWidth);
            m_L = new Rectangle(source.Left, source.Top + BorderWidth, BorderWidth, source.Height - 2 * BorderWidth);

            m_BG = new Rectangle(source.Left + BorderWidth, source.Top + BorderWidth, source.Width - 2 * BorderWidth, source.Height - 2 * BorderWidth);
        }

        public override Rectangle BuildChildArea(Point area)
        {
            return new Rectangle(BorderWidth, BorderWidth, area.X - 2 * BorderWidth, area.Y - 2 * BorderWidth);
        }

        public override void Render(YSpriteBatch batch, Rectangle area, Color? color = null)
        {
            Color Color = (color == null) ? Color.White : color.Value;

            // ##### Draw Background ##### //
            Rectangle drawArea = new Rectangle(
                area.Left + BorderWidth,
                area.Top + BorderWidth,
                area.Width - (2 * BorderWidth),
                area.Height - (2 * BorderWidth));
            batch.GUIDrawSprite(Texture, drawArea, m_BG, Color);

            // ##### Draw Corners ##### //
            drawArea.Width = BorderWidth;
            drawArea.Height = BorderWidth;

            //Top Left
            drawArea.X = area.Left;
            drawArea.Y = area.Top;
            batch.GUIDrawSprite(Texture, drawArea, m_UL, Color);

            //Top Right
            drawArea.X = area.Right - BorderWidth;
            drawArea.Y = area.Top;
            batch.GUIDrawSprite(Texture, drawArea, m_UR, Color);

            //Bottom Right
            drawArea.X = area.Right - BorderWidth;
            drawArea.Y = area.Bottom - BorderWidth;
            batch.GUIDrawSprite(Texture, drawArea, m_LR, Color);

            //Bottom Left
            drawArea.X = area.Left;
            drawArea.Y = area.Bottom - BorderWidth;
            batch.GUIDrawSprite(Texture, drawArea, m_LL, Color);

            // ##### Draw Edges ##### //

            //Top Edge
            drawArea.X = area.Left + BorderWidth;
            drawArea.Y = area.Top;
            drawArea.Width = area.Width - (2 * BorderWidth);
            batch.GUIDrawSprite(Texture, drawArea, m_T, Color);

            //Bottom Edge
            drawArea.Y = area.Bottom - BorderWidth;
            batch.GUIDrawSprite(Texture, drawArea, m_B, Color);

            //Left Edge
            drawArea.X = area.Left;
            drawArea.Y = area.Top + BorderWidth;
            drawArea.Width = BorderWidth;
            drawArea.Height = area.Height - (2 * BorderWidth);
            batch.GUIDrawSprite(Texture, drawArea, m_L, Color);

            //Right Edge
            drawArea.X = area.Right - BorderWidth;
            batch.GUIDrawSprite(Texture, drawArea, m_R, Color);
        } 
    }
}
