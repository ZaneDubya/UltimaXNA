#region Usings

using UltimaXNA.Core.ComponentModel;

#endregion

namespace UltimaXNA.Configuration
{
    public class Resolution : NotifyPropertyChangedBase
    {
        private int m_height;
        private int m_width;

        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height
        {
            get { return m_height; }
            set { SetProperty(ref m_height, value); }
        }

        public int Width
        {
            get { return m_width; }
            set { SetProperty(ref m_width, value); }
        }
    }
}