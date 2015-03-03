using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InterXLib.XGUI.Support
{
    public class Pin
    {
        public bool Pushed { get; private set; }

        private Vector2 m_start, m_offset;

        public Vector2 Shift
        {
            get
            {
                var state = Mouse.GetState();
                m_offset.X = m_start.X - state.X;
                m_offset.Y = m_start.Y - state.Y;
                return m_offset;
            }
        }

        public Pin()
        {
            m_start = Vector2.Zero;
            m_offset = Vector2.Zero;
        }

        public void Push()
        {
            Pushed = true;
            var state = Mouse.GetState();
            m_start = new Vector2(state.X, state.Y);
        }

        public void Pull()
        {
            Pushed = false;
        }
    }    
}
