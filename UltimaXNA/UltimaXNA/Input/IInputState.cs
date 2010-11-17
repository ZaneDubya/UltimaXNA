using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Input.Events;

namespace UltimaXNA.Input
{
    public interface IInputState
    {
        List<InputEventKeyboard> GetKeyboardEvents();
        List<InputEventMouse> GetMouseEvents();

        bool IsKeyDown(WinKeys key);
        bool IsKeyUp(WinKeys key);
        bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl);
        bool HandleMouseEvent(MouseEvent type, MouseButton mb);

        Point2D MousePosition { get; }
        // bool IsMouseButtonDown(MouseButtonInternal mb);
        // bool IsMouseButtonUp(MouseButtonInternal mb);
        bool IsMouseStationarySinceLastUpdate();
    }
}
