/***************************************************************************
 *   SpellbookGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Core.Input;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class SpellbookGump : Gump
    {
        // Private variables

        // Services
        private WorldModel m_World;

        public SpellbookGump()
            : base(0, 0)
        {

        }
    }
}
