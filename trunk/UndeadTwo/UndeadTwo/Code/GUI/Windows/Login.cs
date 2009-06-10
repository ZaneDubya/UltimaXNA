#region File Description & Usings
//-----------------------------------------------------------------------------
// GUIWindow_Chat.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;
#endregion

namespace UltimaXNA.GUI
{
    class Window_Login : Window
    {
        public Window_Login(FormCollection nFormCollection)
            : base(nFormCollection)
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmLogin", "Login to UO", new Vector2(270, 180), new Vector2(250, 200), Form.BorderStyle.Fixed));
            m_MyForm = m_FormCollection["frmLogin"];

            m_MyForm.Controls.Add(new Button("btnLogin", new Vector2(100, 120), 140, "Log in", Color.White, Color.Black));
            m_MyForm["btnLogin"].OnPress += Button1_OnPress;
            m_MyForm["btnLogin"].OnRelease = Button1_OnRelease;

            m_MyForm.Controls.Add(new Label("label1", new Vector2(10, 60), "Username:", Color.TransparentBlack, Color.Black, 80, Label.Align.Right));
            m_MyForm.Controls.Add(new Label("label2", new Vector2(10, 90), "Password:", Color.TransparentBlack, Color.Black, 80, Label.Align.Right));

            m_MyForm.Controls.Add(new Textbox("txtUsername", new Vector2(100, 60), 140, ""));
            m_MyForm.Controls.Add(new Textbox("txtPassword", new Vector2(100, 90), 140, ""));
            //Show the form
            m_FormCollection["frmLogin"].Show();
            m_MyForm.CloseButton.OnRelease += Form_OnClose; // Respond when we close the login window ...
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
            Events.Login(m_MyForm["txtUsername"].Text, m_MyForm["txtPassword"].Text);
            m_MyForm["txtUsername"].Enabled = false;
            m_MyForm["txtPassword"].Enabled = false;
            m_MyForm["btnLogin"].Enabled = false;
        }
    }
}
