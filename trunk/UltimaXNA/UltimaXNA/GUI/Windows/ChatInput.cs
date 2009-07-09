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
        public Window_ChatInput(FormCollection nFormCollection)
            : base(nFormCollection)
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmChatInput", string.Empty, new Vector2(400, 24), new Vector2(4, 600 - 24), Form.BorderStyle.None));
            m_MyForm = m_FormCollection["frmChatInput"];
            m_MyForm.CanSnap = false;
            m_MyForm.RespondToMinMaxEvents = false;
            m_MyForm.BorderName = null;
            m_MyForm.MouseThrough = false;
            m_MyForm.FocusAlways = true;

            m_MyForm.Controls.Add(new Textbox("txtInput", new Vector2(0, 0), (int)m_MyForm.Width));
            ((Textbox)m_MyForm["txtInput"]).TexturePath = @"xwinforms\textures\controls\textbox-dark.png";
            ((Textbox)m_MyForm["txtInput"]).TextureFocusPath = @"xwinforms\textures\controls\textbox-dark.png";
            ((Textbox)m_MyForm["txtInput"]).DrawBackground = true;
            ((Textbox)m_MyForm["txtInput"]).OnEnter += ChatInput_OnEnter;
            m_MyForm["txtInput"].ForeColor = Color.White;
            m_MyForm["txtInput"].FontName = "ArialNarrow10";
            m_MyForm["txtInput"].MouseThrough = false;
            ((Textbox)m_MyForm["txtInput"]).FocusAlways = true;

            //Show the form
            this.Show();
        }

        private void ChatInput_OnEnter(object obj, EventArgs e)
        {
            GUIHelper.Network_SendChat(((Textbox)m_MyForm["txtInput"]).Text);
            ((Textbox)m_MyForm["txtInput"]).Clear();
        }
    }
}
