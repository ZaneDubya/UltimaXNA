/***************************************************************************
 *   LoginState.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Security;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.LoginGumps;
#endregion

namespace UltimaXNA.Ultima.Login.States {
    public class LoginState : AState {
        LoginGump m_LoginGump;
        UserInterfaceService m_UserInterface;
        InputManager m_Input;
        LoginModel m_Login;

        public LoginState() {
            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_Login = ServiceRegistry.GetService<LoginModel>();
            m_Input = ServiceRegistry.GetService<InputManager>();
        }

        public override void Intitialize() {
            base.Intitialize();
            m_LoginGump = (LoginGump)m_UserInterface.AddControl(new LoginGump(OnLogin), 0, 0);
        }

        public override void Dispose() {
            m_LoginGump.Dispose();
            base.Dispose();
        }

        void OnLogin(string server, int port, string account, SecureString password) {
            m_Login.Client.UserName = account;
            m_Login.Client.Password = password;
            Manager.CurrentState = new LoggingInState();
        }
    }
}
