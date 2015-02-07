
namespace UltimaXNA.Input
{
    public class InputEventM : InputEvent
    {
        private readonly MouseEvent _eventType;
        public MouseEvent EventType
        {
            get { return _eventType; }
        }

        protected new EventArgsMouse _args
        {
            get { return (EventArgsMouse)base._args; }
        }

        public MouseButton Button
        {
            get { return _args.Button; }
        }

        public Point2D Position
        {
            get { return _args.Position; }
        }

        private const int WHEEL_DELTA = 120;
        public int WheelValue
        {
            get { return (_args.Clicks / WHEEL_DELTA); }
        }

        public InputEventM(MouseEvent eventType, EventArgsMouse args)
            : base(args)
        {
            _eventType = eventType;
        }
    }

    public enum MouseEvent
    {
        Move,
        Down,
        Up,
        WheelScroll,
        DragBegin,
        DragEnd,
        Click,
        DoubleClick,
    }

    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2,
        XButton1 = 3,
        XButton2 = 4,
        None = 0x7f
    }
}
