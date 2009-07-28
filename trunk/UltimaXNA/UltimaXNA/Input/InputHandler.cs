/***************************************************************************
 *   InputHandler.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.Input
{
    public static class InputHandler
    {
        private static KeyboardHandler _keyboard;
        public static KeyboardHandler Keyboard { get { return _keyboard; } }

        private static MouseHandler _mouse;
        public static MouseHandler Mouse { get { return _mouse; } }

        static InputHandler()
        {
            _keyboard = new KeyboardHandler();
            _mouse = new MouseHandler();
        }

        public static void Update(GameTime gameTime)
        {
            if (GameState.EngineRunning)
            {
                _keyboard.Update(gameTime);
                _mouse.Update(gameTime);
            }
            else
            {
                _keyboard.NoInput();
                _mouse.NoInput();
            }
        }
    }
}
