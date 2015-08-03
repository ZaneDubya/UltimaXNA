/***************************************************************************
 *   VendorBuyGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Network;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Ultima.World.Entities;
using UltimaXNA.Ultima.Network.Server;
#endregion


namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class VendorBuyGump : Gump
    {
        private ExpandableScroll m_ScrollBackground;

        public VendorBuyGump(AEntity vendorBackpack, VendorBuyListPacket packet)
            : base(0, 0)
        {
            IsMoveable = true;

            AddControl(m_ScrollBackground = new ExpandableScroll(this, 0, 0, 320, false));
        }
    }
}
