/***************************************************************************
 *   LoginBackdrop.cs
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
    class Window_LoginBG : Window
    {
        int logoWidth = (int)(1024 * .7f);
        int logoHeight = (int)(256 * .7f);

        public Window_LoginBG()
            : base()
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmLoginBG", "Login to UO", new Vector2(800, 600), new Vector2(0, 0), Form.BorderStyle.None));
            _MyForm = m_FormCollection["frmLoginBG"];
            _MyForm.BorderName = null;
            _MyForm.MouseThrough = true;

            // Controls.Add(new PictureBox("picturebox1", new Vector2(0, 0), @"", 800, 600, 0));
            // ((PictureBox)_MyForm["picturebox1"]).Texture = Data.Gumps.GetGumpXNA(0x500);

            Controls.Add(new PictureBox("picLogo", new Vector2((800 - logoWidth) / 2, 64), @"Logo\UltimaXNALogo1024.png", logoWidth, logoHeight, 0));
            //Show the form
            this.Show();
        }
    }
}
