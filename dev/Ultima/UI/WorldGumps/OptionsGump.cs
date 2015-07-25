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
using System;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
using System.Collections.Generic;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Configuration;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    class OptionsGump : Gump
    {
        UserInterfaceService m_UserInterface;
        WorldModel m_World;
        HSliderBar m_MusicVolume;
        HSliderBar m_SoundVolume;
        CheckBox m_MusicOn;
        CheckBox m_SoundOn;
        CheckBox m_FootStepSoundOn;
        CheckBox m_AlwaysRun;
        CheckBox m_MenuBarDisabled;
        DropDownList m_Resolution;

        Resolution[] ResolutionAsResolution;
        string[] RS;


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
            AddControl(new Button(this, 0, 40, 217, 217, ButtonTypes.SwitchPage, 1, (int)Buttons.Sound),1);
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Sound and Music"), 1);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting affect the sound and music you will hear while playing Ultima Online."), 1);

            AddControl(new TextLabelAscii(this, 85, 85, 1, 9, @"Sound on/off"), 1);
            m_SoundOn = (CheckBox)AddControl(new CheckBox(this, 60, 80, 210, 211, Settings.Audio.SoundOn, 61), 1);

            AddControl(new TextLabelAscii(this, 60, 110, 1, 9, @"Sound volume"), 1);
            m_SoundVolume = (HSliderBar)AddControl(new HSliderBar(this, 60, 130, 150, 0, 100, Settings.Audio.SoundVolume, HSliderBarStyle.MetalWidgetRecessedBar), 1);
            AddControl(new TextLabelAscii(this, 220, 130, 1, 9, Settings.Audio.SoundVolume.ToString()), 1);

            AddControl(new TextLabelAscii(this, 85, 155, 1, 9, @"Music on/off"), 1);
            m_MusicOn = (CheckBox)AddControl(new CheckBox(this, 60, 150, 210, 211, Settings.Audio.MusicOn, 62), 1);

            AddControl(new TextLabelAscii(this, 60, 180, 1, 9, @"Music volume"), 1);
            m_MusicVolume = (HSliderBar)AddControl(new HSliderBar(this, 60, 200, 150, 0, 100, Settings.Audio.MusicVolume, HSliderBarStyle.MetalWidgetRecessedBar), 1);

            AddControl(new TextLabelAscii(this, 220, 200, 1, 9, m_MusicVolume.Value.ToString()), 1);

            AddControl(new TextLabelAscii(this, 85, 225, 1, 9, @"Play footstep sound"), 1);
            m_FootStepSoundOn = (CheckBox)AddControl(new CheckBox(this, 60, 220, 210, 211, Settings.Audio.FootStepSoundOn, 62), 1);
            
            // page 2 Pop-up Help
            AddControl(new Button(this, 0, 110, 219, 219, ButtonTypes.SwitchPage, 2, (int)Buttons.Help),2);
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Pop-up Help"), 2);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting configure the behavior of the pop-up help."), 2);

            // page 3 Chat
            AddControl(new Button(this, 0, 250, 223, 223, ButtonTypes.SwitchPage, 3, (int)Buttons.Chat),3);
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Chat"), 3);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting affect the interface display for chat system."), 3);

            // page 4 Macro Options
            AddControl(new Button(this, 0, 320, 236, 236, ButtonTypes.SwitchPage, 4, (int)Buttons.Macros),4);
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Macro Options"), 4);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @""), 4);

            // page 5 Interface
            AddControl(new Button(this, 576, 40, 225, 225, ButtonTypes.SwitchPage, 5, (int)Buttons.Interface),5);
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Interface"), 5);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting affect your interface."), 5);

            AddControl(new TextLabelAscii(this, 85, 85, 1, 9, @"Your character will always run if this is checked"), 5);
            m_AlwaysRun = (CheckBox)AddControl(new CheckBox(this, 60, 80, 210, 211, Settings.World.AlwaysRun, 61), 5);

            AddControl(new TextLabelAscii(this, 85, 115, 1, 9, @"Disable the Menu Bar"), 5);
            m_MenuBarDisabled = (CheckBox)AddControl(new CheckBox(this, 60, 110, 210, 211, Settings.World.MenuBarDisabled, 61), 5);

            // page 6 Display
            AddControl(new Button(this, 576, 110, 227, 227, ButtonTypes.SwitchPage, 6, (int)Buttons.Display),6);
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Display"), 6);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting affect your display, and adjusting some of them may improve your graphics performance."), 6);
            AddControl(new CheckBox(this, 60, 80, 210, 211, false, 61), 6);
            AddControl(new TextLabelAscii(this, 85, 80, 1, 9, @"Some option"), 6);
            AddControl(new CheckBox(this, 60, 100, 210, 211, false, 62), 6);
            AddControl(new TextLabelAscii(this, 85, 100, 1, 9, @"Another option"), 6);
            AddControl(new CheckBox(this, 60, 120, 210, 211, Settings.World.IsMaximized, 61), 6);
            AddControl(new TextLabelAscii(this, 85, 120, 1, 9, @"Use full screen display"), 6);
            
            AddControl(new TextLabelAscii(this, 60, 150, 1, 9, @"Full screen resolution"), 6);
            m_Resolution = (DropDownList)AddControl(new DropDownList(this, 60, 170, 122, ResolutionsInString(), 10, ActualResolution(), false), 6);

            // page 7 Reputation system
            AddControl(new Button(this, 576, 180, 229, 229, ButtonTypes.SwitchPage, 7, (int)Buttons.Reputation),7);
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Reputation system"), 7);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"These settting affect the reputation system, which is Ultima Online's system for controlling antisocial behavior."), 7);

            // page 8 Miscellaneous
            AddControl(new Button(this, 576, 250, 231, 231, ButtonTypes.SwitchPage, 8, (int)Buttons.Miscellaneous),8);
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Miscellaneous"), 8);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @"Miscellaneous options."), 8);

            // page 9 Filter Options
            AddControl(new Button(this, 576, 320, 234, 234, ButtonTypes.SwitchPage, 9, (int)Buttons.Filters),9);
            AddControl(new TextLabelAscii(this, 250, 20, 1, 2, @"Filter Options"), 9);
            AddControl(new TextLabelAscii(this, 60, 45, 1, 9, @""), 9);
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("options");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            /*AddControl(new TextLabelAscii(this, 220, 130, 1, 9, m_SoundVolume.Value.ToString()), 1);
            ControlsToUpdate.Add(LastControl);
            AddControl(new TextLabelAscii(this, 220, 200, 1, 9, m_MusicVolume.Value.ToString()), 1);
            ControlsToUpdate.Add(LastControl);*/

            base.Update(totalMS, frameMS);
        }

        public int ActualResolution()
        {
            string res = Settings.World.GumpResolution.Width + "x" + Settings.World.GumpResolution.Height;
            int index = Array.IndexOf(RS, res);
            return index;
        }

        public string[] ResolutionsInString()
        {
            List<Resolution> ResolutionsR = new List<Resolution>();
            List<string> ResolutionsS = new List<string>();

            foreach (Microsoft.Xna.Framework.Graphics.DisplayMode mode in Microsoft.Xna.Framework.Graphics.GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                string resS = mode.Width + "x" + mode.Height;
                Resolution resR = new Resolution(mode.Width, mode.Height);

                if (!ResolutionsS.Contains(resS))
                {
                    ResolutionsS.Add(resS);
                    ResolutionsR.Add(resR);
                }
            }
            RS = ResolutionsS.ToArray();
            ResolutionAsResolution = ResolutionsR.ToArray();
            return RS;
        }

        public void SaveSettings()
        {
            //audio
            Settings.Audio.MusicVolume = m_MusicVolume.Value;
            Settings.Audio.SoundVolume = m_SoundVolume.Value;
            Settings.Audio.MusicOn = m_MusicOn.IsChecked;
            Settings.Audio.SoundOn = m_SoundOn.IsChecked;
            Settings.Audio.FootStepSoundOn = m_FootStepSoundOn.IsChecked;

            //interface
            Settings.World.AlwaysRun = m_AlwaysRun.IsChecked;
            Settings.World.MenuBarDisabled = m_MenuBarDisabled.IsChecked;
            Settings.World.WindowResolution = new Resolution(ResolutionAsResolution[m_Resolution.Index].Width, ResolutionAsResolution[m_Resolution.Index].Height);
            Settings.World.GumpResolution = new Resolution(ResolutionAsResolution[m_Resolution.Index].Width, ResolutionAsResolution[m_Resolution.Index].Height);
            SwitchTopMenuGump();
        }

        public void SwitchTopMenuGump()
        {
            if (!Settings.World.MenuBarDisabled && m_UserInterface.GetControl<TopMenuGump>() == null)
            {
                m_UserInterface.AddControl(new TopMenuGump(), 0, 0); // by default at the top of the screen.
            }
            else if (Settings.World.MenuBarDisabled && m_UserInterface.GetControl<TopMenuGump>() != null)
            {
                m_UserInterface.GetControl<TopMenuGump>().Dispose();
            }
        }

        public override void ActivateByButton(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.Cancel:
                    {
                        Dispose();
                        break;
                    }
                case Buttons.Apply:
                    {
                        SaveSettings();
                        break;
                    }
                case Buttons.Default:
                    {
                        break;
                    }
                case Buttons.Okay:
                    {
                        SaveSettings();
                        Dispose();
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
    }
}
