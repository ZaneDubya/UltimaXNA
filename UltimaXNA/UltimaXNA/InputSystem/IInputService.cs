using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UltimaXNA.Input
{
    public interface IInputService : IGameComponent
    {
        Vector2 CurrentMousePosition { get; }

        event EventHandler<KeyboardEventArgs> KeyDown;
        event EventHandler<KeyboardEventArgs> KeyUp;
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<MouseEventArgs> MouseMove;

        bool IsKeyDown(Keys key);
        bool IsKeyPress(Keys key);
        bool IsKeyRelease(Keys key);
        bool IsKeyUp(Keys key);
        bool IsMouseButtonDown(MouseButtons mb);
        bool IsMouseButtonPress(MouseButtons mb);
        bool IsMouseButtonRelease(MouseButtons mb);
        bool IsMouseButtonUp(MouseButtons mb);
        bool IsSpecialKey(Keys key);
        bool IsCursorMovedSinceLastUpdate();

        float TimePressed(MouseButtons mb);
        float TimePressed(Keys key);

        void GetKeyboardInput(out string inputText, out List<Keys> specialKeys);
        char KeyToChar(Keys key, bool shiftPressed);
    }
}
