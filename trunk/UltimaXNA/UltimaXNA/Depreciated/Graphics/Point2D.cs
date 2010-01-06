namespace UltimaXNA.Graphics
{
    public class Point2D : IPoint2D
    {
        private int _x;
        private int _y;

        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public Point2D(int x, int y)
        {
            _x = x;
            _y = y;
        }
    }
}
