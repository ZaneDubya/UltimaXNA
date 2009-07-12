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
        public Window_Chat()
            : base()
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmChat", string.Empty, new Vector2(392, 128), new Vector2(4, 600 - 120 - 36), Form.BorderStyle.None));
            _MyForm = m_FormCollection["frmChat"];
            _MyForm.CanSnap = false;
            _MyForm.RespondToMinMaxEvents = false;
            _MyForm.BorderName = null;
            _MyForm.MouseThrough = true;

            _MyForm.Controls.Add(new Textbox("txtChat", new Vector2(0, 0), (int)_MyForm.Width, (int)_MyForm.Height, string.Empty));
            ((Textbox)_MyForm["txtChat"]).TextOffset = new Vector2(0, 0);
            ((Textbox)_MyForm["txtChat"]).DrawBackground = false;
            ((Textbox)_MyForm["txtChat"]).ScrollFromBottom = true;
            ((Textbox)_MyForm["txtChat"]).WrapText = true;
            _MyForm["txtChat"].ForeColor = Color.White;
            _MyForm["txtChat"].FontName = "ArialNarrow10";
            _MyForm["txtChat"].MouseThrough = true;

            // float iButtonLeft = 4f;
            // _MyForm.Controls.Add(new Button("btnChat", new Vector2(iButtonLeft, 0), @"GUI\CHATFRAME\UI-ChatIcon-Chat-Up.png", 1.0f, Color.White));
            // _MyForm.Controls.Add(new Button("btnScrollUp", new Vector2(iButtonLeft, 32), @"GUI\CHATFRAME\UI-ChatIcon-ScrollUp-Up.png", 1.0f, Color.White));
            // _MyForm.Controls.Add(new Button("btnScrollDown", new Vector2(iButtonLeft, 64), @"GUI\CHATFRAME\UI-ChatIcon-ScrollDown-Up.png", 1.0f, Color.White));
            // _MyForm.Controls.Add(new Button("btnScrollEnd", new Vector2(iButtonLeft, 96), @"GUI\CHATFRAME\UI-ChatIcon-ScrollEnd-Up.png", 1.0f, Color.White));
            // _MyForm["btnChat"].OnPress += Button1_OnPress;
            // _MyForm["btnChat"].OnRelease = Button1_OnRelease; 

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
