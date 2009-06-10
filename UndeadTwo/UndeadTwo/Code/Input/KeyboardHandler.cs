#region File Description & Usings
//-----------------------------------------------------------------------------
// KeyboardHandler.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace UndeadClient.Input
{
    public class KeyboardHandler
    {
        private List<Keys> m_PressedKeys;

        public KeyboardHandler()
        {
            m_PressedKeys = new List<Keys>();
        }

        public bool IsKeyDown(Keys key)
        {
            if (m_PressedKeys.Contains(key))
                return true;
            return false;
        }

        public void Update(GameTime gameTime)
        {
            KeyboardState iKeyboardState;
            iKeyboardState = Keyboard.GetState();

            m_PressedKeys.Clear();

            foreach (Keys key in iKeyboardState.GetPressedKeys())
            {
                m_PressedKeys.Add(key);
            }
        }
        /*
        public uint frame_Number = 0;
        public Dictionary key2KeyPress = new Dictionary<();

        Keys[] pressed_Key = Keyboard.GetState().GetPressedKeys();
        for (int i = 0; i < pressed_Key.Length; i++)
        {
              //if key isnt supportedsupport it.
              if (!key2KeyPress.ContainsKey(pressed_Key[i])) 
                   key2KeyPress.Add(pressed_Key[i], new Key_Press(frame_Number));
                   
              Key_Press pk = key2KeyPress[pressed_Key[i]];
              
              if (pk.frame_Pressed + 1 == frame_Number) //key down.
              {
                   pk.time += gameTime.ElapsedRealTime;
                   pk.frame_Pressed = frame_Number;
                   if (pk.time.Milliseconds > 400)
                        I_WAS_PRESSED(pressed_Key[i]);
              }
              else //key press
              {
                   pk.time = TimeSpan.Zero;
                   pk.frame_Pressed = frame_Number;
                   I_WAS_PRESSED(pressed_Key[i]);
              }
        }
         */
    }
}
