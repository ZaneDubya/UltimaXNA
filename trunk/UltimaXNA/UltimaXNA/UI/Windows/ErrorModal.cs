/***************************************************************************
 *   ErrorModal.cs
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
using System;
using xWinFormsLib;
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.UI
{
    class ErrorModal : Window
    {
        public ErrorModal(FormCollection nFormCollection, string nText)
            : base()
        {
            MessageBox msgbox0 = new MessageBox(new Vector2(300, 100), "Error", nText, MessageBox.Type.MB_OK);
            msgbox0.OnOk = msgbox0_OnOk;
        }

        private void msgbox0_OnOk(object obj, EventArgs e)
        {

        }
    }


}
