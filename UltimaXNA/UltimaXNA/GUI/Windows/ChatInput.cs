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
    class Window_ChatInput : Window
    {
        public Window_ChatInput()
            : base()
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmChatInput", string.Empty, new Vector2(400, 24), new Vector2(4, 600 - 24), Form.BorderStyle.None));
            _MyForm = m_FormCollection["frmChatInput"];
            _MyForm.CanSnap = false;
            _MyForm.RespondToMinMaxEvents = false;
            _MyForm.BorderName = null;
            _MyForm.MouseThrough = false;
            _MyForm.FocusAlways = true;

            _MyForm.Controls.Add(new Textbox("txtInput", new Vector2(0, 0), (int)_MyForm.Width));
            ((Textbox)_MyForm["txtInput"]).TexturePath = @"xwinforms\textures\controls\textbox-dark.png";
            ((Textbox)_MyForm["txtInput"]).TextureFocusPath = @"xwinforms\textures\controls\textbox-dark.png";
            ((Textbox)_MyForm["txtInput"]).DrawBackground = true;
            ((Textbox)_MyForm["txtInput"]).OnEnter += ChatInput_OnEnter;
            _MyForm["txtInput"].ForeColor = Color.White;
            _MyForm["txtInput"].FontName = "ArialNarrow10";
            _MyForm["txtInput"].MouseThrough = false;
            ((Textbox)_MyForm["txtInput"]).FocusAlways = true;

            //Show the form
            this.Show();
        }

        private void ChatInput_OnEnter(object obj, EventArgs e)
        {
            GUIHelper.Network_SendChat(((Textbox)_MyForm["txtInput"]).Text);
            ((Textbox)_MyForm["txtInput"]).Clear();
        }
    }
}
