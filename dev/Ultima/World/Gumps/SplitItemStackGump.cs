/***************************************************************************
 *   SplitItemStackGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    class SplitItemStackGump : Gump
    {
        public Item Item
        {
            get;
            private set;
        }

        public SplitItemStackGump(Item item)
            : base(0, 0)
        {
            IsMovable = true;

            Item = item;

            AddControl(new GumpPic(this, 0, 0, 0, 0x085c, 0));
            AddControl(new Button(this, 1, 102, 38, 0x085d, 0x085e, ButtonTypes.Default, 0, 0));
            ((Button)LastControl).GumpOverID = 0x085f;
        }
    }
}
