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

        Point2D MousePosition { get; }
        bool IsMouseButtonDown(MouseButton mb);
        bool IsMouseButtonUp(MouseButton mb);
        bool IsMouseStationarySinceLastUpdate();
    }
}
