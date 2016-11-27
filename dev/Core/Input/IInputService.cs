/***************************************************************************
 *   IInputService.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Core.Windows;

namespace UltimaXNA.Core.Input
{
    interface IInputService
    {
        bool IsCtrlDown { get; }
        bool IsShiftDown { get; }
        bool IsKeyDown(WinKeys key);
        Point MousePosition { get; }
        bool IsMouseButtonDown(MouseButton btn);
        List<InputEventKeyboard> GetKeyboardEvents();
        List<InputEventMouse> GetMouseEvents();
        bool HandleKeyboardEvent(KeyboardEvent type, WinKeys key, bool shift, bool alt, bool ctrl);
        bool HandleMouseEvent(MouseEvent type, MouseButton mb);
        void Update(double totalTime, double frameTime);
    }
}
