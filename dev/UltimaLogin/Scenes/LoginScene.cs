/***************************************************************************
 *   LoginScene.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.UltimaGUI.LoginGumps;
#endregion

namespace UltimaXNA.UltimaLogin.Scenes
{
    public class LoginScene : AScene
    {
        LoginGump m_LoginGump;

        public LoginScene()
        {

        }

        public override void Intitialize(UltimaEngine engine)
        {
            base.Intitialize(engine);
            m_LoginGump = (LoginGump)Engine.UserInterface.AddControl(new LoginGump(), 0, 0);
            m_LoginGump.OnLogin += OnLogin;
        }

        public override void Update(double totalTime, double frameTime)
        {
            base.Update(totalTime, frameTime);

            if (Engine.Input.HandleKeyboardEvent(UltimaXNA.Input.Windows.KeyboardEventType.Down, UltimaXNA.Input.Windows.WinKeys.D, false, false, true))
            {
                Manager.CurrentScene = new HueTestScene();
            }
        }

        public override void Dispose()
        {
            m_LoginGump.Dispose();
            base.Dispose();
        }

        public void OnLogin(string server, int port, string account, string password)
        {
            Manager.CurrentScene = new LoggingInScene(server, port, account, password);
        }
    }
}
