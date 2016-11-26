/***************************************************************************
 *   CrimeQueryGump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Network.Client;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities.Mobiles;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class CrimeQueryGump : Gump
    {
        public Mobile Mobile;

        public CrimeQueryGump(Mobile mobile)
            : base(0, 0)
        {
            Mobile = mobile;

            AddControl(new GumpPic(this, 0, 0, 0x0816, 0));
            AddControl(new TextLabelAscii(this, 40, 30, 1, 118, "This may flag you criminal!", 120));
            ((TextLabelAscii)LastControl).Hue = 997;
            AddControl(new Button(this, 40, 77, 0x817, 0x818, ButtonTypes.Activate, 0, 0));
            AddControl(new Button(this, 100, 77, 0x81A, 0x81B, ButtonTypes.Activate, 1, 1));

            IsMoveable = false;
            MetaData.IsModal = true;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            CenterThisControlOnScreen();
        }

        public override void OnButtonClick(int buttonID)
        {
            switch (buttonID)
            {
                case 0:
                    Dispose();
                    break;
                case 1:
                    INetworkClient m_Network = Services.Get<INetworkClient>();
                    m_Network.Send(new AttackRequestPacket(Mobile.Serial));
                    Dispose();
                    break;
            }
        }
    }
}
