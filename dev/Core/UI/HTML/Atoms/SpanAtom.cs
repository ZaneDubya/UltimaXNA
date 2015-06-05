using UltimaXNA.Ultima.IO.FontsNew;

namespace UltimaXNA.Core.UI.HTML.Atoms
{
    public class SpanAtom : AAtom
    {
        private int m_width = 0;
        public override int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        private int m_height = 0;
        public override int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        public SpanAtom(IUIResourceProvider provider)
        {
            m_height = provider.GetUnicodeFont((int)Font).Height;
        }
    }
}
