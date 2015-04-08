using UltimaXNA.ComponentModel;

namespace UltimaXNA.Data
{
    public class Resolution : NotifyPropertyChangedBase
    {
        private int _height;
        private int _width;

        public Resolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        public int Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }
    }
}