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
    public interface IInputService
    {
        MouseHandler Mouse { get; }
        KeyboardHandler Keyboard { get; }
    }

    public class InputHandler : GameComponent, IInputService
    {
        private KeyboardHandler m_Keyboard;
        public KeyboardHandler Keyboard { get { return m_Keyboard; } }

        private MouseHandler m_Mouse;
        public MouseHandler Mouse { get { return m_Mouse; } }

        public InputHandler(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IInputService), this);
        }

        public override void Initialize()
        {
            m_Keyboard = new KeyboardHandler();
            m_Mouse = new MouseHandler();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Game.IsActive)
            {
                m_Keyboard.Update(gameTime);
                m_Mouse.Update(gameTime);
            }
            else
            {
                m_Keyboard.NoInput();
                m_Mouse.NoInput();
            }
        }
    }
}
