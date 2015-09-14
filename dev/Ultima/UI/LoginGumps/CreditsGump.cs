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
#endregion

namespace UltimaXNA.Ultima.UI.LoginGumps
{
    public class CreditsGump : Gump
    {
        private const string CreditsString =
            "<span color='#000'>UltimaXNA\n" +
            "Copyright (c) 2009, 2015 UltimaXNA Development Team\n\n" +

            "This program is free software; you can redistribute it and/or modify " +
            "it under the terms of the GNU General Public License as published by " +
            "the Free Software Foundation; either version 3 of the License, or " +
            "(at your option) any later version.\n\n" +

            "This program is distributed in the hope that it will be useful, " +
            "but WITHOUT ANY WARRANTY; without even the implied warranty of " +
            "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the " +
            "GNU General Public License for more details.\n\n" +

            "You should have received a copy of the GNU General Public License " +
            "along with this program. If not, see <u>http://www.gnu.org/licenses/</u>.\n\n" +

            "Developers:\nZane Wagner\nJeff Boulanger\nChase Mosher\nPatrik Samuel Tauchim\n" +
            "Tolga Basol\nSmjert\nScott Gorman\nDeniz Sokmen\nAatu Riuttamäki\nGabriele Tozzi\nslayde1970\n\n" +

            "UltimaXNA contains code from:\n" +
            "RunUO <u>https://github.com/runuo/runuo</u>\n" +
            "UltimaSDK <u>https://github.com/jeffboulanger/OpenUO</u>\n" +
            "OpenUO <u>https://ultimasdk.codeplex.com</u>";

        public CreditsGump()
            : base(0, 0)
        {
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 0x0588));
            AddControl(new HtmlGumpling(this, 96, 64, 400, 400, 1, 1, CreditsString));
            HandlesMouseInput = true;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            Dispose();
        }
    }
}
