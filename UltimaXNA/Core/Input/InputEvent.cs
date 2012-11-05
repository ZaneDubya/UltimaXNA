namespace UltimaXNA.Input
{
    public class InputEvent
    {
        protected EventArgs _args;
        protected bool _handled;

        public bool Alt
        {
            get { return _args.Alt; }
        }

        public bool Control
        {
            get { return _args.Control; }
        }

        public bool Shift
        {
            get { return _args.Shift; }
        }

        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        public InputEvent(EventArgs args)
        {
            _args = args;
        }

        public void SuppressEvent()
        {
            _handled = true;
        }
    }
}
