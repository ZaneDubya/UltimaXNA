/***************************************************************************
 *   MouseHandler.cs
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
using Microsoft.Xna.Framework.Input;
#endregion

namespace UltimaXNA.Input
{
    public class MouseHandler
    {
        public Vector2 Position;
        public MouseButton[] Buttons;
        private MouseState m_MouseState;
        private MouseState m_MouseStateLast;

        public MouseHandler()
        {
            Buttons = new MouseButton[2];
            for (int i = 0; i < 2; i++)
            {
                Buttons[i] = new MouseButton();
            }
        }

        public void NoInput()
        {
            Position = new Vector2(99999, 99999);
            Buttons[0].Update(ButtonState.Released, ButtonState.Released);
            Buttons[1].Update(ButtonState.Released, ButtonState.Released);
            m_MouseStateLast = m_MouseState;
        }

        public void Update(GameTime gameTime)
        {
            m_MouseStateLast = m_MouseState;
            m_MouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Position = new Vector2(m_MouseState.X, m_MouseState.Y);

            Buttons[0].Update(m_MouseState.LeftButton, m_MouseStateLast.LeftButton);
            Buttons[1].Update(m_MouseState.RightButton, m_MouseStateLast.RightButton);
        }

        public bool MovedSinceLastUpdate
        {
            get
            {
                return ((m_MouseState.X == m_MouseStateLast.X) && (m_MouseState.Y == m_MouseStateLast.Y)) ? false : true;
            }
        }
    }

    public class MouseButton
    {
        public bool IsDown;
        public bool Press;
        public bool Release;

        internal void Update(ButtonState nStateCurrent, ButtonState nStateLast)
        {
            Press = false;
            Release = false;

            if (nStateCurrent == ButtonState.Pressed)
            {
                IsDown = true;
                if (nStateLast == ButtonState.Released)
                    Press = true;
            }
            else
            {
                IsDown = false;
                if (nStateLast == ButtonState.Pressed)
                    Release = true;
            }
        }
    }
}
