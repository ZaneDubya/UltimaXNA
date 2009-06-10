#region File Description & Usings
//-----------------------------------------------------------------------------
// InputHandler.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
#endregion

namespace UndeadClient.Input
{
    public interface IInputHandler
    {
        MouseHandler Mouse { get; }
        KeyboardHandler Keyboard { get; }
    }

    public class InputHandler : GameComponent, IInputHandler
    {
        private KeyboardHandler m_Keyboard;
        public KeyboardHandler Keyboard { get { return m_Keyboard; } }

        private MouseHandler m_Mouse;
        public MouseHandler Mouse { get { return m_Mouse; } }

        public InputHandler(Game game)
            : base(game)
        {
            game.Services.AddService(typeof(IInputHandler), this);
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
        }
    }
}
