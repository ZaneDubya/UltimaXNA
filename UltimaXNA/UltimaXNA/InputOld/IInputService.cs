using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UltimaXNA.InputOld
{
    public interface IInputService : IGameComponent
    {
        Vector2 MousePosition { get; }

        event EventHandler<KeyboardEventArgs> KeyDown;
        event EventHandler<KeyboardEventArgs> KeyUp;
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<MouseEventArgs> MouseMove;

        bool IsKeyDown(Keys key);
        bool IsKeyPress(Keys key);
        bool IsKeyRelease(Keys key);
        bool IsKeyUp(Keys key);

        bool IsMouseButtonDown(MouseButton mb);
        bool IsMouseButtonPress(MouseButton mb);
        bool IsMouseButtonRelease(MouseButton mb);
        bool IsMouseButtonUp(MouseButton mb);
        bool IsSpecialKey(Keys key);
        bool IsCursorMovedSinceLastUpdate();

        float TimePressed(MouseButton mb);
        float TimePressed(Keys key);

        void GetKeyboardInput(out string inputText, out List<Keys> specialKeys);
        char KeyToChar(Keys key, bool shiftPressed);
    }
}
