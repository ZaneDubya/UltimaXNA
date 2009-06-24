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
    class Window_Chat : Window
    {
        public Window_Chat(FormCollection nFormCollection)
            : base(nFormCollection)
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmChat", string.Empty, new Vector2(392, 128), new Vector2(4, 600 - 120 - 36), Form.BorderStyle.None));
            m_MyForm = m_FormCollection["frmChat"];
            m_MyForm.CanSnap = false;
            m_MyForm.RespondToMinMaxEvents = false;
            m_MyForm.BorderName = null;
            m_MyForm.MouseThrough = true;

            m_MyForm.Controls.Add(new Textbox("txtChat", new Vector2(0, 0), (int)m_MyForm.Width, (int)m_MyForm.Height, string.Empty));
            ((Textbox)m_MyForm["txtChat"]).TextOffset = new Vector2(0, 0);
            ((Textbox)m_MyForm["txtChat"]).DrawBackground = false;
            ((Textbox)m_MyForm["txtChat"]).ScrollFromBottom = true;
            m_MyForm["txtChat"].ForeColor = Color.White;
            m_MyForm["txtChat"].FontName = "ArialNarrow10";
            m_MyForm["txtChat"].MouseThrough = true;

            // float iButtonLeft = 4f;
            // m_MyForm.Controls.Add(new Button("btnChat", new Vector2(iButtonLeft, 0), @"GUI\CHATFRAME\UI-ChatIcon-Chat-Up.png", 1.0f, Color.White));
            // m_MyForm.Controls.Add(new Button("btnScrollUp", new Vector2(iButtonLeft, 32), @"GUI\CHATFRAME\UI-ChatIcon-ScrollUp-Up.png", 1.0f, Color.White));
            // m_MyForm.Controls.Add(new Button("btnScrollDown", new Vector2(iButtonLeft, 64), @"GUI\CHATFRAME\UI-ChatIcon-ScrollDown-Up.png", 1.0f, Color.White));
            // m_MyForm.Controls.Add(new Button("btnScrollEnd", new Vector2(iButtonLeft, 96), @"GUI\CHATFRAME\UI-ChatIcon-ScrollEnd-Up.png", 1.0f, Color.White));
            // m_MyForm["btnChat"].OnPress += Button1_OnPress;
            // m_MyForm["btnChat"].OnRelease = Button1_OnRelease; 

            //Show the form
            this.Show();
        }

        internal void AddText(string nText)
        {
            ((Textbox)m_MyForm["txtChat"]).AddLine(nText);
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
