using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.UltimaGUI.HTML.Atoms
{
    public class ImageAtom : AAtom
    {
        public Image AssociatedImage;

        private int m_overrideWidth = -1;
        public override int Width
        {
            set
            {
                m_overrideWidth = value;
            }
            get
            {
                if (m_overrideWidth != -1)
                    return m_overrideWidth + 1;
                Texture2D gump = UltimaData.GumpData.GetGumpXNA(Value);
                return gump.Width + 1;
            }
        }

        private int m_overrideHeight = -1;
        public override int Height
        {
            set
            {
                m_overrideHeight = value;
            }
            get
            {
                if (m_overrideHeight != -1)
                    return m_overrideHeight;
                Texture2D gump = UltimaData.GumpData.GetGumpXNA(Value);
                return gump.Height;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return UltimaData.GumpData.GetGumpXNA(Value);
            }
        }

        public int Value = -1;
        public int ValueDown = -1;
        public int ValueOver = -1;

        public ImageAtom(int value)
        {
            Value = value;
        }

        public override string ToString()
        {
            return string.Format("<gImg {0}>", Value);
        }
    }
}
