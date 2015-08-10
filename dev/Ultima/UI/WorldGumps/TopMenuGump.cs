/***************************************************************************
 *   TopMenu.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Core.Network;
using UltimaXNA.Ultima.Network.Client;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class TopMenuGump : Gump
    {
        UserInterfaceService m_UserInterface;
        WorldModel m_World;
        INetworkClient m_Client;

        public TopMenuGump()
            : base(0, 0)
        {
            IsUncloseableWithRMB = true;
            IsMoveable = true;
            
            // maximized view
            AddControl(new ResizePic(this, 0, 0, 9200, 610, 27), 1);
            AddControl(new Button(this, 5, 3, 5540, 5542, 0, 2, 0), 1);
            ((Button)LastControl).GumpOverID = 5541;
            // buttons are 2443 small, 2445 big
            // 30, 93, 201, 309, 417, 480, 543
            // map, paperdollB, inventoryB, journalB, chat, help, < ? >
            AddControl(new Button(this, 30, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Map), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Map";
            AddControl(new Button(this, 93, 3, 2445, 2445, ButtonTypes.Activate, 0, (int)Buttons.Paperdoll), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Paperdoll";
            AddControl(new Button(this, 201, 3, 2445, 2445, ButtonTypes.Activate, 0, (int)Buttons.Inventory), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Inventory";
            AddControl(new Button(this, 309, 3, 2445, 2445, ButtonTypes.Activate, 0, (int)Buttons.Journal), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Journal";
            AddControl(new Button(this, 417, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Chat), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Chat";
            AddControl(new Button(this, 480, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Help), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Help";
            AddControl(new Button(this, 543, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Question), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Debug";
            // minimized view
            AddControl(new ResizePic(this, 0, 0, 9200, 30, 27), 2);
            AddControl(new Button(this, 5, 3, 5537, 5539, 0, 1, 0), 2);
            ((Button)LastControl).GumpOverID = 5538;

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_World = ServiceRegistry.GetService<WorldModel>();
            m_Client = ServiceRegistry.GetService<INetworkClient>();

            MetaData.Layer = UILayer.Over;
        }
        
        protected override void OnInitialize()
        {
            SetSavePositionName("topmenu");
            base.OnInitialize();
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.Map:
                    if (m_UserInterface.GetControl<MiniMapGump>() == null)
                        m_UserInterface.AddControl(new MiniMapGump(), 566, 25);
                    else
                        m_UserInterface.RemoveControl<MiniMapGump>();
                    break;
                case Buttons.Paperdoll:
                    Mobile player = (Mobile)WorldModel.Entities.GetPlayerEntity();
                    if (m_UserInterface.GetControl<PaperDollGump>(player.Serial) == null)
                        m_Client.Send(new DoubleClickPacket(player.Serial | Serial.ProtectedAction)); // additional flag keeps us from being dismounted.
                    else
                        m_UserInterface.RemoveControl<PaperDollGump>(player.Serial);
                    break;
                case Buttons.Inventory:
                    // opens the player's backpack.
                    Mobile mobile = WorldModel.Entities.GetPlayerEntity();
                    Container backpack = mobile.Backpack;
                    m_World.Interaction.DoubleClick(backpack);
                    break;
                case Buttons.Journal:
                    if (m_UserInterface.GetControl<JournalGump>() == null)
                        m_UserInterface.AddControl(new JournalGump(), 80, 80);
                    else
                        m_UserInterface.RemoveControl<JournalGump>();
                    break;
                case Buttons.Chat:
                    break;
                case Buttons.Help:
                    break;
                case Buttons.Question:
                    if (m_UserInterface.GetControl<DebugGump>() == null)
                        m_UserInterface.AddControl(new DebugGump(), 50, 50);
                    else
                        m_UserInterface.RemoveControl<DebugGump>();
                    break;
            }
        }

        enum Buttons
        {
            Map,
            Paperdoll,
            Inventory,
            Journal,
            Chat,
            Help,
            Question
        }
    }
}
