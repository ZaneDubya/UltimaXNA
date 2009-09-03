using System;
using UltimaXNA.EventSystem;
using Microsoft.Xna.Framework;
namespace UltimaXNA.Graphics.UI
{
    interface IUINode : IDisposable, IEventReceiver
    {
        bool HitTest(float x, float y);
        void HitTest(ref float x, ref float y, out bool value);
        bool HitTest(Vector2 position);

        event ClickHandler Click;
        event MouseEnterHandler MouseEnter;
        event MouseLeaveHandler MouseLeave;
        event KeyCharHandler KeyChar;
        event EventEngineHandler Event;
    }
}
