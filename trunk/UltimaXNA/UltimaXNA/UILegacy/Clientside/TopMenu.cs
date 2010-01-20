using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Entities;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy.Clientside
{
    class TopMenu : Gump
    {
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

        public TopMenu(Serial serial)
            : base(serial, 0)
        {
            // maximized view
            AddGumpling(new ResizePic(this, 1, 0, 0, 9200, 610, 27));
            AddGumpling(new Button(this, 1, 5, 3, 5540, 5542, 0, 2, 0));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5541;
            // buttons are 2443 small, 2445 big
            // 30, 93, 201, 309, 417, 480, 543
            // map, paperdollB, inventoryB, journalB, chat, help, < ? >
            AddGumpling(new Button(this, 1, 30, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Map));
            ((Button)_controls[_controls.Count - 1]).Caption = "<basefont color=#000000>Map";
            AddGumpling(new Button(this, 1, 93, 3, 2445, 2445, ButtonTypes.Activate, 0, (int)Buttons.Paperdoll));
            ((Button)_controls[_controls.Count - 1]).Caption = "<basefont color=#000000>Paperdoll";
            AddGumpling(new Button(this, 1, 201, 3, 2445, 2445, ButtonTypes.Activate, 0, (int)Buttons.Inventory));
            ((Button)_controls[_controls.Count - 1]).Caption = "<basefont color=#000000>Inventory";
            AddGumpling(new Button(this, 1, 309, 3, 2445, 2445, ButtonTypes.Activate, 0, (int)Buttons.Journal));
            ((Button)_controls[_controls.Count - 1]).Caption = "<basefont color=#000000>Journal";
            AddGumpling(new Button(this, 1, 417, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Chat));
            ((Button)_controls[_controls.Count - 1]).Caption = "<basefont color=#000000>Chat";
            AddGumpling(new Button(this, 1, 480, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Help));
            ((Button)_controls[_controls.Count - 1]).Caption = "<basefont color=#000000>Help";
            AddGumpling(new Button(this, 1, 543, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Question));
            ((Button)_controls[_controls.Count - 1]).Caption = "<basefont color=#000000>< ? >";
            // minimized view
            AddGumpling(new ResizePic(this, 2, 0, 0, 9200, 30, 27));
            AddGumpling(new Button(this, 2, 5, 3, 5537, 5539, 0, 1, 0));
            ((Button)_controls[_controls.Count - 1]).GumpOverID = 5538;
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.Map:
                    _manager.ToggleGump_Local(new MiniMap(), 566, 25);
                    break;
                case Buttons.Paperdoll:
                    _manager.ToggleGump_Local(new DebugGump(), 50, 50);
                    break;
                case Buttons.Inventory:
                    PlayerMobile m = (PlayerMobile)EntitiesCollection.GetPlayerObject();
                    Container i = (Container)m.equipment[(int)EquipLayer.Backpack];
                    Client.UltimaClient.Send(new Network.Packets.Client.DoubleClickPacket(i.Serial));
                    // _manager.AddContainerGump(i, 0);
                    break;
                case Buttons.Journal:
                    break;
                case Buttons.Chat:
                    break;
                case Buttons.Help:
                    break;
                case Buttons.Question:
                    break;
            }
        }
    }
}
