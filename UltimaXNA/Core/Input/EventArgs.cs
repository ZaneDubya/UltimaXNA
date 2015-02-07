
namespace UltimaXNA.Input
{
    public class EventArgs
    {
        private readonly WinKeys _modifiers;

        public virtual bool Alt
        {
            get { return ((_modifiers & WinKeys.Alt) == WinKeys.Alt); }
        }

        public bool Control
        {
            get { return ((_modifiers & WinKeys.Control) == WinKeys.Control); }
        }

        public virtual bool Shift
        {
            get { return ((_modifiers & WinKeys.Shift) == WinKeys.Shift); }
        }

        public EventArgs(WinKeys modifiers)
        {
            _modifiers = modifiers;
        }
    }
}
