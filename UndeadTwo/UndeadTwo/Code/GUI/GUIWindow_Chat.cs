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

namespace UndeadClient.GUI
{
    class GUIWindow_Chat : GUIWindow
    {
        Form m_MyForm;

        public GUIWindow_Chat(FormCollection nFormCollection)
            : base(nFormCollection)
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmChat", "ChatFrame", new Vector2(400, 128), new Vector2(0, 600 - 128), Form.BorderStyle.None));
            m_MyForm = m_FormCollection["frmChat"];
            m_MyForm.BorderName = null;
            m_MyForm.MouseThrough = true;

            float iButtonLeft = 0f;
            m_MyForm.Controls.Add(new Button("btnChat", new Vector2(iButtonLeft, 0), @"GUI\CHATFRAME\UI-ChatIcon-Chat-Up.png", 1.0f, Color.White));
            m_MyForm.Controls.Add(new Button("btnScrollUp", new Vector2(iButtonLeft, 32), @"GUI\CHATFRAME\UI-ChatIcon-ScrollUp-Up.png", 1.0f, Color.White));
            m_MyForm.Controls.Add(new Button("btnScrollDown", new Vector2(iButtonLeft, 64), @"GUI\CHATFRAME\UI-ChatIcon-ScrollDown-Up.png", 1.0f, Color.White));
            m_MyForm.Controls.Add(new Button("btnScrollEnd", new Vector2(iButtonLeft, 96), @"GUI\CHATFRAME\UI-ChatIcon-ScrollEnd-Up.png", 1.0f, Color.White));

            m_MyForm["btnChat"].OnPress += Button1_OnPress;
            m_MyForm["btnChat"].OnRelease = Button1_OnRelease;

            m_MyForm.Controls.Add(new Textbox("txtChat", new Vector2(iButtonLeft + 32f, 0f), 400 - 32 - 4, 128 - 4, ""));
            ((Textbox)m_MyForm["txtChat"]).DrawBackground = false;
            m_MyForm["txtChat"].ForeColor = Color.White;
            m_MyForm["txtChat"].FontName = "ArialNarrow10";
            m_MyForm["txtChat"].MouseThrough = true;

            //Show the form
            m_FormCollection["frmChat"].Show();
        }

        public void AddText(string nText)
        {
            ((Textbox)m_MyForm["txtChat"]).AddLine(nText);
        }

        private void Button1_OnPress(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button Pressed!");
            //((Listbox)formCollection["form1"]["listbox1"]).Add(formCollection["form1"].Controls["textbox1"].Text);
        }
        private void Button1_OnRelease(object obj, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Button Released!");
        }
    }
}
