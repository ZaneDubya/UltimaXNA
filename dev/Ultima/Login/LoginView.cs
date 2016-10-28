/***************************************************************************
 *   LoginView.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
 
 using UltimaXNA.Core.Patterns.MVC;

namespace UltimaXNA.Ultima.Login
{
    class LoginView : AView
    {
        protected new LoginModel Model {
            get { return (LoginModel)base.Model; }
        }

        public LoginView(LoginModel model)
            : base(model) {

        }
    }
}
