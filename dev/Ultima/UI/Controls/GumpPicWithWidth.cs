using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.UI;
using Microsoft.Xna.Framework;

namespace UltimaXNA.Ultima.UI.Controls
{
    class GumpPicWithWidth : GumpPic
    {
        private float m_PercentWidthDrawn = 1.0f;

        /// <summary>
        /// The percent of this gump pic's width which is drawn. Clipped to 0.0f to 1.0f.
        /// </summary>
        public float PercentWidthDrawn
        {
            get
            {
                return m_PercentWidthDrawn;
            }
            set
            {
                if (value < 0f)
                    value = 0f;
                else if (value > 1f)
                    value = 1f;
                m_PercentWidthDrawn = 1f;
            }
        }

        public GumpPicWithWidth(AControl owner, int x, int y, int gumpID, int hue, float percentWidth)
            : base(owner, x, y, gumpID, hue)
        {
            PercentWidthDrawn = percentWidth;
        }

        public override void Draw(Core.Graphics.SpriteBatchUI spriteBatch, Microsoft.Xna.Framework.Point position)
        {
            Vector3 hueVector = Utility.GetHueVector(Hue);
            int width = (int)(m_PercentWidthDrawn * Width);
            spriteBatch.Draw2D(m_Texture, new Rectangle(position.X, position.Y, width, Height), new Rectangle(0, 0, width, Height), hueVector);
            base.Draw(spriteBatch, position);
        }
    }
}
