/***************************************************************************
 *   LoginCharList.cs
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

namespace UltimaXNA.GUI
{
    class Window_CharacterList : Window
    {
        public Window_CharacterList(FormCollection nFormCollection)
            : base()
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmLoginCharList", "Character List", new Vector2(270, 180), new Vector2(250, 200), Form.BorderStyle.Fixed));
            _MyForm = m_FormCollection["frmLoginCharList"];

            Controls.Add(new Label("label1", new Vector2(10, 30), "Username:", Color.TransparentBlack, Color.Black, 80, Label.Align.Right));
            Controls.Add(new Textbox("txtUsername", new Vector2(100, 30), 140, ""));
            _MyForm["txtUsername"].Focus();

            Controls.Add(new Label("label2", new Vector2(10, 60), "Password:", Color.TransparentBlack, Color.Black, 80, Label.Align.Right));
            Controls.Add(new Textbox("txtPassword", new Vector2(100, 60), 140, ""));

            Controls.Add(new Label("label3", new Vector2(10, 90), "Server:", Color.TransparentBlack, Color.Black, 80, Label.Align.Right));
            Controls.Add(new Textbox("txtServer", new Vector2(100, 90), 140, "localhost"));

            Controls.Add(new Label("label4", new Vector2(10, 120), "Port:", Color.TransparentBlack, Color.Black, 80, Label.Align.Right));
            Controls.Add(new Textbox("txtPort", new Vector2(100, 120), 140, "2593"));

            Controls.Add(new Button("btnLogin", new Vector2(100, 150), 140, "Log in", Color.White, Color.Black));
            _MyForm["btnLogin"].OnPress = Button1_OnPress;
            _MyForm["btnLogin"].OnRelease = Button1_OnRelease;

            //Show the form
            this.Show();
            _MyForm.CloseButton.OnRelease += Form_OnClose; // Respond when we close the login window ...
        }

        private void Form_OnClose(object obj, EventArgs e)
        {
            // We closed the window before logging in.
            Events.QuitImmediate();
        }

        private void Button1_OnPress(object obj, EventArgs e)
        {
            //((Listbox)formCollection["form1"]["listbox1"]).Add(formCollection["form1"].Controls["textbox1"].Text);
        }
        private void Button1_OnRelease(object obj, EventArgs e)
        {
        }
    }
}
