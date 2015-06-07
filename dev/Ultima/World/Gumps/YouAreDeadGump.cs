/***************************************************************************
 *   YouAreDeadGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Entities;
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.Entities.Items.Containers;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    class YouAreDeadGump : Gump
    {
        public YouAreDeadGump()
            : base(0, 0)
        {
            AddControl(new HtmlGumpling(this, 0, 0, 0, 200, 40, 0, 0, "<big><center>You are dead.</center></big>"));
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            CenterThisControlOnScreen();
        }
    }
}
