/***************************************************************************
 *   Login.cs
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
    class Window_Legacy_Menu : Window
    {
        public Window_Legacy_Menu()
            : base()
        {
            //Create a new form
            _formCollection.Add(new Form("frmLegacyMenu", string.Empty, new Vector2(610, 27), new Vector2(0, 0), Form.BorderStyle.None));
            _MyForm = _formCollection["frmLegacyMenu"];
            _MyForm.BorderName = null;

            Controls.Add(new PictureBox("picBG", Vector2.Zero, @"GUI\LEGACYFRAME_Menu\bg.png", 0));

            Controls.Add(new Button("btnMap", new Vector2(30, 3), 60, "Map", Color.White, Color.Black));
            Controls["btnMap"].OnPress = btnMap_OnPress;

            Controls.Add(new CustomButton("btnToggle", new Vector2(5, 3), new Rectangle(0, 0, 19, 17),
                @"GUI\LEGACYFRAME_Menu\toggle_open.png", null,
                null, null, 1.0f));

            //Controls.Add(new CustomButton("btnMap", new Vector2(30, 3), new Rectangle(0, 0, 60, 21), 
            //    @"GUI\LEGACYFRAME_Menu\button_up.png", @"GUI\LEGACYFRAME_Menu\button_down.png",
            //    null, null, 1.0f));
            

            Controls.Add(new CustomButton("btnMap", new Vector2(417, 3), new Rectangle(0, 0, 60, 21),
                @"GUI\LEGACYFRAME_Menu\button_up.png", @"GUI\LEGACYFRAME_Menu\button_down.png",
                null, null, 1.0f));
            Controls["btnMap"].OnPress = btnMap_OnPress;

            Controls.Add(new CustomButton("btnMap", new Vector2(480, 3), new Rectangle(0, 0, 60, 21),
                @"GUI\LEGACYFRAME_Menu\button_up.png", @"GUI\LEGACYFRAME_Menu\button_down.png",
                null, null, 1.0f));
            Controls["btnMap"].OnPress = btnMap_OnPress;

            Controls.Add(new CustomButton("btnMap", new Vector2(543, 3), new Rectangle(0, 0, 60, 21),
                @"GUI\LEGACYFRAME_Menu\button_up.png", @"GUI\LEGACYFRAME_Menu\button_down.png",
                null, null, 1.0f));
            Controls["btnMap"].OnPress = btnMap_OnPress;

            //Show the form
            this.Show();
        }

        private void btnMap_OnPress(object obj, EventArgs e)
        {
        }
        /*private void btnMap_OnPress(object obj, EventArgs e)
        {
        }
        private void btnMap_OnPress(object obj, EventArgs e)
        {
        }
        private void btnMap_OnPress(object obj, EventArgs e)
        {
        }*/
    }
}
