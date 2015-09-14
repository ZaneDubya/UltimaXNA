/***************************************************************************
 *   CreditsGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.Resources;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.Input;
using System.IO;
#endregion

namespace UltimaXNA.Ultima.UI.LoginGumps
{
    public class CreditsGump : Gump
    {
        public CreditsGump()
            : base(0, 0)
        {
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 0x0588));
            AddControl(new HtmlGumpling(this, 96, 64, 400, 400, 1, 1, ReadCreditsFile()));
            HandlesMouseInput = true;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            Dispose();
        }

        private string ReadCreditsFile()
        {
            string path = @"Data\credits.txt";
            if (!File.Exists(path))
                return "<span color='#000'>Credits file not found.";
            try
            {
                string text = File.ReadAllText(@"Data\credits.txt");
                return text;
            }
            catch
            {
                return "<span color='#000'>Could not read credits file.";
            }
            
        }
    }
}
