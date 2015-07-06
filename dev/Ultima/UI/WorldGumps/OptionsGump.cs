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

#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class OptionsGump : Gump
    {
        UserInterfaceService m_UserInterface;
        WorldModel m_World;

        public OptionsGump()
            : base(0, 0)
        {
            IsMovable = true;
            AddControl(new ResizePic(this, 40, 0, 2600, 550, 450));
            //left column
            AddControl(new Button(this, 0, 40, 218, 217, ButtonTypes.SwitchPage, 1, (int)Buttons.Sound));
            AddControl(new Button(this, 0, 110, 220, 219, ButtonTypes.SwitchPage, 2, (int)Buttons.Help));
            AddControl(new Button(this, 0, 250, 224, 223, ButtonTypes.SwitchPage, 3, (int)Buttons.Chat));
            AddControl(new Button(this, 0, 320, 237, 236, ButtonTypes.SwitchPage, 4, (int)Buttons.Macros));
            //right column
            AddControl(new Button(this, 576, 40, 226, 225, ButtonTypes.SwitchPage, 5, (int)Buttons.Interface));
            AddControl(new Button(this, 576, 110, 228, 227, ButtonTypes.SwitchPage, 6, (int)Buttons.Display));
            AddControl(new Button(this, 576, 180, 230, 229, ButtonTypes.SwitchPage, 7, (int)Buttons.Reputation));
            AddControl(new Button(this, 576, 250, 232, 231, ButtonTypes.SwitchPage, 8, (int)Buttons.Miscellaneous));
            AddControl(new Button(this, 576, 320, 235, 234, ButtonTypes.SwitchPage, 9, (int)Buttons.Filters));
            //bottom buttons
            AddControl(new Button(this, 140, 410, 243, 241, ButtonTypes.Activate, 0, (int)Buttons.Cancel));
            AddControl(new Button(this, 240, 410, 239, 240, ButtonTypes.Activate, 0, (int)Buttons.Apply));
            AddControl(new Button(this, 340, 410, 246, 244, ButtonTypes.Activate, 0, (int)Buttons.Default));
            AddControl(new Button(this, 440, 410, 249, 248, ButtonTypes.Activate, 0, (int)Buttons.Okay));

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_World = ServiceRegistry.GetService<WorldModel>();

            // page 1 Sound and Music
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Sound and Music"), 1);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting affect the sound and music you will hear while playing Ultima Online."), 1);

            // page 2 Pop-up Help
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Pop-up Help"), 2);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting configure the behavior of the pop-up help."), 2);

            // page 3 Chat
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Chat"), 3);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting affect the interface display for chat system."), 3);

            // page 4 Macro Options
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Macro Options"), 4);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @""), 4);

            // page 5 Interface
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Interface"), 5);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting affect your interface."), 5);

            // page 6 Display
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Display"), 6);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting affect your display, and adjusting some of them may improve your graphics performance."), 6);
            AddControl(new CheckBox(this, 60, 80, 210, 211, false, 61), 6);
            AddControl(new TextLabelAscii(this, 85, 80, 1, 9, @"Some option"), 6);
            AddControl(new CheckBox(this, 60, 100, 210, 211, false, 62), 6);
            AddControl(new TextLabelAscii(this, 85, 100, 1, 9, @"Another option"), 6);
            AddControl(new CheckBox(this, 60, 120, 210, 211, Settings.World.IsMaximized, (int)CheckBoxes.UseFullScreen), 6);
            AddControl(new TextLabelAscii(this, 85, 120, 1, 9, @"Use full screen display"), 6);

            AddControl(new TextLabelAscii(this, 60, 140, 1, 9, @"Full screen resolution"), 6);

            // page 7 Reputation system
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Reputation system"), 7);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting affect the reputation system, which is Ultima Online's system for controlling antisocial behavior."), 7);

            // page 8 Miscellaneous
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Miscellaneous"), 8);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"Miscellaneous options."), 8);

            // page 9 Filter Options
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Filter Options"), 9);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @""), 9);
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("options");
            base.OnInitialize();
        }

        public void SaveSettings()
        {

        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.Cancel:
                    {
                    break;
                    }
                case Buttons.Apply:
                    {
                    break;
                    }
                case Buttons.Default:
                    {
                    break;
                    }
                case Buttons.Okay:
                    {
                    break;
                    }
            }
        }

        enum Buttons
        {
            Sound,
            Help,
            Chat,
            Macros,
            Interface,
            Display,
            Reputation,
            Miscellaneous,
            Filters,
            Cancel,
            Apply,
            Default,
            Okay
        }

        enum CheckBoxes
        {
            UseFullScreen
        }
    }
}
