using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InterXLib.XGUI.Support
{
    public class Pin
    {
        public bool Pushed { get; private set; }

        private Vector2 _start, _offset;

        public Vector2 Shift
        {
            get
            {
                var state = Mouse.GetState();
                _offset.X = _start.X - state.X;
                _offset.Y = _start.Y - state.Y;
                return _offset;
            }
        }

        public Pin()
        {
            _start = Vector2.Zero;
            _offset = Vector2.Zero;
        }

        public void Push()
        {
            Pushed = true;
            var state = Mouse.GetState();
            _start = new Vector2(state.X, state.Y);
        }

        public void Pull()
        {
            Pushed = false;
        }
    }    
}
