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
using UltimaXNA.Ultima.World.Entities.Items.Containers;
using UltimaXNA.Ultima.World.Entities.Mobiles;
using UltimaXNA.Ultima.UI;
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

            // maximized view
            AddControl(new ResizePic(this, 40, 0, 2600, 550, 450), 1);
            //left column
            AddControl(new Button(this, 0, 40, 218, 217, ButtonTypes.Activate, 0, (int)Buttons.Sound), 1);
            AddControl(new Button(this, 0, 110, 220, 219, ButtonTypes.Activate, 0, (int)Buttons.Help), 1);
            AddControl(new Button(this, 0, 250, 224, 223, ButtonTypes.Activate, 0, (int)Buttons.Chat), 1);
            AddControl(new Button(this, 0, 320, 237, 236, ButtonTypes.Activate, 0, (int)Buttons.Macros), 1);
            //right column
            AddControl(new Button(this, 576, 40, 226, 225, ButtonTypes.Activate, 0, (int)Buttons.Interface), 1);
            AddControl(new Button(this, 576, 110, 228, 227, ButtonTypes.Activate, 0, (int)Buttons.Display), 1);
            AddControl(new Button(this, 576, 180, 230, 229, ButtonTypes.Activate, 0, (int)Buttons.Reputation), 1);
            AddControl(new Button(this, 576, 250, 232, 231, ButtonTypes.Activate, 0, (int)Buttons.Miscellaneous), 1);
            AddControl(new Button(this, 576, 320, 235, 234, ButtonTypes.Activate, 0, (int)Buttons.Filters), 1);

            AddControl(new Button(this, 140, 410, 243, 241, ButtonTypes.Activate, 0, (int)Buttons.Cancel), 1);
            AddControl(new Button(this, 240, 410, 239, 240, ButtonTypes.Activate, 0, (int)Buttons.Apply), 1);
            AddControl(new Button(this, 340, 410, 246, 244, ButtonTypes.Activate, 0, (int)Buttons.Default), 1);
            AddControl(new Button(this, 440, 410, 249, 248, ButtonTypes.Activate, 0, (int)Buttons.Okay), 1);

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
            m_World = ServiceRegistry.GetService<WorldModel>();

            MetaData.Layer = UILayer.Over;
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("options");
            base.OnInitialize();
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.Sound:
                    break;

                case Buttons.Help:
                    break;

                case Buttons.Chat:
                    break;

                case Buttons.Macros:
                    break;

                case Buttons.Interface:
                    break;

                case Buttons.Display:
                    break;

                case Buttons.Reputation:
                    break;

                case Buttons.Miscellaneous:
                    break;

                case Buttons.Filters:
                    break;

                case Buttons.Cancel:
                    break;

                case Buttons.Apply:
                    break;

                case Buttons.Default:
                    break;

                case Buttons.Okay:
                    break;


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
    }
}
