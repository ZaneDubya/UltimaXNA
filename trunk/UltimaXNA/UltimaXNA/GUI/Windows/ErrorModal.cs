using System;
using xWinFormsLib;
using Microsoft.Xna.Framework;

namespace UltimaXNA.GUI
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
