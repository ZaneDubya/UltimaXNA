using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;

namespace UltimaXNA.GUI
{
    class Window_LoginBG : Window
    {
        public Window_LoginBG(FormCollection nFormCollection)
            : base(nFormCollection)
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmLoginBG", "Login to UO", new Vector2(800, 600), new Vector2(0, 0), Form.BorderStyle.None));
            m_MyForm = m_FormCollection["frmLoginBG"];
            m_MyForm.BorderName = null;
            m_MyForm.MouseThrough = true;

            m_MyForm.Controls.Add(new PictureBox("picturebox1", new Vector2(0, 0), @"", 800, 600, 0));
            ((PictureBox)m_MyForm["picturebox1"]).Texture = DataLocal.Gumps.GetGumpXNA(0x500);

            //Show the form
            this.Show();
        }
    }
}
