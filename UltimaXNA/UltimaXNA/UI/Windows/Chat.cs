/***************************************************************************
 *   Window_Chat.cs
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;
#endregion

namespace UltimaXNA.UI
{
    class Window_Chat : Window
    {
        public Window_Chat()
            : base()
        {
            //Create a new form
            _formCollection.Add(new Form("frmChat", string.Empty, new Vector2(392, 128), new Vector2(4, 600 - 120 - 36), Form.BorderStyle.None));
            _MyForm = _formCollection["frmChat"];
            _MyForm.CanSnap = false;
            _MyForm.RespondToMinMaxEvents = false;
            _MyForm.BorderName = null;
            _MyForm.MouseThrough = true;

            Controls.Add(new Textbox("txtChat", new Vector2(0, 0), (int)_MyForm.Width, (int)_MyForm.Height, string.Empty));
            ((Textbox)_MyForm["txtChat"]).TextOffset = new Vector2(0, 0);
            ((Textbox)_MyForm["txtChat"]).DrawBackground = false;
            ((Textbox)_MyForm["txtChat"]).ScrollFromBottom = true;
            ((Textbox)_MyForm["txtChat"]).WrapText = true;
            _MyForm["txtChat"].ForeColor = Color.White;
            _MyForm["txtChat"].FontName = "ArialNarrow10";
            _MyForm["txtChat"].MouseThrough = true;

            //Show the form
            this.Show();
        }

        internal void AddText(string nText)
        {
            ((Textbox)_MyForm["txtChat"]).AddLine(nText);
        }

        private void Button1_OnPress(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button Pressed!");
        }
        private void Button1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button Released!");
        }
    }
}
