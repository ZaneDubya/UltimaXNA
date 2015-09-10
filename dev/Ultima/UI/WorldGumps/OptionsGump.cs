/***************************************************************************
 *   OptionsGump.cs
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
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using UltimaXNA.Configuration;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;
using UltimaXNA.Core;
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

        CheckBox m_IsVSyncEnabled;
        CheckBox m_ShowFps;
        CheckBox m_IsConsoleEnabled;
        ColorPickerBox m_SpeechColor;
        ColorPickerBox m_EmoteColor;
        ColorPickerBox m_PartyMsgColor;
        ColorPickerBox m_GuildMsgColor;
        CheckBox m_IgnoreGuildMsg;
        ColorPickerBox m_AllianceMsgColor;
        CheckBox m_IgnoreAllianceMsg;

        DropDownList m_DropDownFullScreenResolutions;
        DropDownList m_DropDownPlayWindowResolutions;

        double m_RefreshTime = 0d;
        private TextLabelAscii[] m_Labels = new TextLabelAscii[2];

        private enum Labels
        {
            SoundVolume,
            MusicVolume
        }

        public OptionsGump()
            : base(0, 0)
        {
            IsMoveable = true;

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
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Sound and Music"), 1);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting affect the sound and music you will hear while playing Ultima Online."), 1);

            AddControl(new TextLabelAscii(this, 85, 85, 9, 1, @"Sound on/off"), 1);
            m_SoundOn = (CheckBox)AddControl(new CheckBox(this, 60, 80, 210, 211, Settings.Audio.SoundOn, 61), 1);

            AddControl(new TextLabelAscii(this, 60, 110, 9, 1, @"Sound volume"), 1);
            m_SoundVolume = (HSliderBar)AddControl(new HSliderBar(this, 60, 130, 150, 0, 100, Settings.Audio.SoundVolume, HSliderBarStyle.MetalWidgetRecessedBar), 1);
            m_Labels[(int)Labels.SoundVolume] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 220, 130, 9, 1, Settings.Audio.SoundVolume.ToString()), 1);

            AddControl(new TextLabelAscii(this, 85, 155, 9, 1, @"Music on/off"), 1);
            m_MusicOn = (CheckBox)AddControl(new CheckBox(this, 60, 150, 210, 211, Settings.Audio.MusicOn, 62), 1);

            AddControl(new TextLabelAscii(this, 60, 180, 9, 1, @"Music volume"), 1);
            m_MusicVolume = (HSliderBar)AddControl(new HSliderBar(this, 60, 200, 150, 0, 100, Settings.Audio.MusicVolume, HSliderBarStyle.MetalWidgetRecessedBar), 1);
            m_Labels[(int)Labels.MusicVolume] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 220, 200, 9, 1, m_MusicVolume.Value.ToString()), 1);

            AddControl(new TextLabelAscii(this, 85, 225, 9, 1, @"Play footstep sound"), 1);
            m_FootStepSoundOn = (CheckBox)AddControl(new CheckBox(this, 60, 220, 210, 211, Settings.Audio.FootStepSoundOn, 62), 1);
            
            // page 2 Pop-up Help
            AddControl(new Button(this, 0, 110, 219, 219, ButtonTypes.SwitchPage, 2, (int)Buttons.Help),2);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Pop-up Help"), 2);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting configure the behavior of the pop-up help."), 2);

            // page 3 Chat
            AddControl(new Button(this, 0, 250, 223, 223, ButtonTypes.SwitchPage, 3, (int)Buttons.Chat),3);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Chat"), 3);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting affect the interface display for chat system."), 3);

            // page 4 Macro Options
            AddControl(new Button(this, 0, 320, 236, 236, ButtonTypes.SwitchPage, 4, (int)Buttons.Macros),4);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Macro Options"), 4);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @""), 4);
            AddControl(new Button(this, 180, 110, 2460, 2461, ButtonTypes.SwitchPage, 4, (int)Buttons.Macros), 4); // add
            AddControl(new Button(this, 234, 110, 2463, 2464, ButtonTypes.SwitchPage, 4, (int)Buttons.Macros), 4); // delete
            AddControl(new Button(this, 302, 110, 2466, 2467, ButtonTypes.SwitchPage, 4, (int)Buttons.Macros), 4); // previous
            AddControl(new Button(this, 386, 110, 2469, 2470, ButtonTypes.SwitchPage, 4, (int)Buttons.Macros), 4); // next
            AddControl(new TextLabelAscii(this, 160, 150, 9, 1, @"Keystroke"), 4);
            AddControl(new GumpPic(this, 155, 175, 2444, 0), 4); // macro key input
            AddControl(new TextLabelAscii(this, 225, 180, 9, 1, @"Key"), 4);

            AddControl(new CheckBox(this, 270, 140, 2151, 2153, Settings.Audio.FootStepSoundOn, 62), 4); //shift
            AddControl(new CheckBox(this, 270, 170, 2151, 2153, Settings.Audio.FootStepSoundOn, 62), 4); //alt
            AddControl(new CheckBox(this, 270, 200, 2151, 2153, Settings.Audio.FootStepSoundOn, 62), 4); //ctrl
            AddControl(new TextLabelAscii(this, 307, 150, 9, 1, @"Shift"), 4);
            AddControl(new TextLabelAscii(this, 307, 180, 9, 1, @"Alt"), 4);
            AddControl(new TextLabelAscii(this, 307, 210, 9, 1, @"Ctrl"), 4);


            AddControl(new TextLabelAscii(this, 160, 225, 9, 1, @"Actions"), 4);
            AddControl(new GumpPic(this, 155, 245, 2445, 0), 4); // action type
            AddControl(new GumpPic(this, 267, 245, 2446, 0), 4); // action

            AddControl(new GumpPic(this, 155, 272, 2445, 0), 4); // action type


            // page 5 Interface
            AddControl(new Button(this, 576, 40, 225, 225, ButtonTypes.SwitchPage, 5, (int)Buttons.Interface),5);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Interface"), 5);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting affect your interface."), 5);

            AddControl(new TextLabelAscii(this, 85, 85, 9, 1, @"Your character will always run if this is checked"), 5);
            m_AlwaysRun = (CheckBox)AddControl(new CheckBox(this, 60, 80, 210, 211, Settings.World.AlwaysRun, 61), 5);

            AddControl(new TextLabelAscii(this, 85, 115, 9, 1, @"Disable the Menu Bar"), 5);
            m_MenuBarDisabled = (CheckBox)AddControl(new CheckBox(this, 60, 110, 210, 211, Settings.World.MenuBarDisabled, 61), 5);

            // page 6 Display
            AddControl(new Button(this, 576, 110, 227, 227, ButtonTypes.SwitchPage, 6, (int)Buttons.Display),6);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Display"), 6);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting affect your display, and adjusting some of them may improve your graphics performance.", 430), 6);

            AddControl(new TextLabelAscii(this, 85, 80, 9, 1, @"Enable vertical synchronization"), 6);
            m_IsVSyncEnabled = (CheckBox)AddControl(new CheckBox(this, 60, 80, 210, 211, Settings.Game.IsVSyncEnabled, 61), 6);

            AddControl(new TextLabelAscii(this, 85, 100, 9, 1, @"Some unused option"), 6);
            AddControl(new CheckBox(this, 60, 100, 210, 211, false, 62), 6);

            AddControl(new TextLabelAscii(this, 85, 120, 9, 1, @"Use full screen display"), 6);
            AddControl(new CheckBox(this, 60, 120, 210, 211, Settings.World.IsMaximized, 61), 6);
            
            AddControl(new TextLabelAscii(this, 60, 150, 9, 1, @"Full Screen Resolution:"), 6);
            m_DropDownFullScreenResolutions = (DropDownList)AddControl(new DropDownList(this, 60, 165, 122, CreateResolutionsStringArrayFromList(Resolutions.FullScreenResolutionsList), 10, GetCurrentFullScreenIndex(), false), 6);

            AddControl(new TextLabelAscii(this, 60, 190, 9, 1, @"Play Window Resolution:"), 6);
            m_DropDownPlayWindowResolutions = (DropDownList)AddControl(new DropDownList(this, 60, 205, 122, CreateResolutionsStringArrayFromList(Resolutions.PlayWindowResolutionsList), 10, GetCurrentPlayWindowIndex(), false), 6);

            AddControl(new TextLabelAscii(this, 85, 235, 9, 1, @"Speech color"), 6);
            m_SpeechColor = (ColorPickerBox)AddControl(new ColorPickerBox(this, new Rectangle(60, 235, 15, 15), new Rectangle(60, 235, 450, 150), 30, 10, Hues.TextTones, Settings.Game.SpeechColor), 6);

            AddControl(new TextLabelAscii(this, 85, 255, 9, 1, @"Emote color"), 6);
            m_EmoteColor = (ColorPickerBox)AddControl(new ColorPickerBox(this, new Rectangle(60, 255, 15, 15), new Rectangle(60, 255, 450, 150), 30, 10, Hues.TextTones, Settings.Game.EmoteColor), 6);

            AddControl(new TextLabelAscii(this, 85, 275, 9, 1, @"Party message color"), 6);
            m_PartyMsgColor = (ColorPickerBox)AddControl(new ColorPickerBox(this, new Rectangle(60, 275, 15, 15), new Rectangle(60, 275, 450, 150), 30, 10, Hues.TextTones, Settings.Game.PartyMsgColor), 6);

            AddControl(new TextLabelAscii(this, 85, 295, 9, 1, @"Guild message color"), 6);
            m_GuildMsgColor = (ColorPickerBox)AddControl(new ColorPickerBox(this, new Rectangle(60, 295, 15, 15), new Rectangle(60, 295, 450, 150), 30, 10, Hues.TextTones, Settings.Game.GuildMsgColor), 6);

            AddControl(new TextLabelAscii(this, 85, 315, 9, 1, @"Ignore guild messages"), 6);
            m_IgnoreGuildMsg = (CheckBox)AddControl(new CheckBox(this, 60, 315, 210, 211, Settings.Game.IgnoreGuildMsg, 62), 6);

            AddControl(new TextLabelAscii(this, 85, 335, 9, 1, @"Alliance message color"), 6);
            m_AllianceMsgColor = (ColorPickerBox)AddControl(new ColorPickerBox(this, new Rectangle(60, 335, 15, 15), new Rectangle(60, 335, 450, 150), 30, 10, Hues.TextTones, Settings.Game.AllianceMsgColor), 6);

            AddControl(new TextLabelAscii(this, 85, 355, 9, 1, @"Ignore alliance messages"), 6);
            m_IgnoreAllianceMsg = (CheckBox)AddControl(new CheckBox(this, 60, 355, 210, 211, Settings.Game.IgnoreAllianceMsg, 62), 6);

            // page 7 Reputation system
            AddControl(new Button(this, 576, 180, 229, 229, ButtonTypes.SwitchPage, 7, (int)Buttons.Reputation),7);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Reputation system"), 7);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting affect the reputation system, which is Ultima Online's system for controlling antisocial behavior."), 7);

            // page 8 Miscellaneous
            AddControl(new Button(this, 576, 250, 231, 231, ButtonTypes.SwitchPage, 8, (int)Buttons.Miscellaneous),8);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Miscellaneous"), 8);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"Miscellaneous options."), 8);

            AddControl(new TextLabelAscii(this, 85, 80, 9, 1, @"Enable debug console"), 8);
            m_IsConsoleEnabled = (CheckBox)AddControl(new CheckBox(this, 60, 80, 210, 211, Settings.Debug.IsConsoleEnabled, 61), 8);

            AddControl(new TextLabelAscii(this, 85, 100, 9, 1, @"Show FPS"), 8);
            m_ShowFps = (CheckBox)AddControl(new CheckBox(this, 60, 100, 210, 211, Settings.Debug.ShowFps, 61), 8);

            // page 9 Filter Options
            AddControl(new Button(this, 576, 320, 234, 234, ButtonTypes.SwitchPage, 9, (int)Buttons.Filters),9);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Filter Options"), 9);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @""), 9);
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("options");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_RefreshTime + 0.5d < totalMS) //need to update
            {
                m_RefreshTime = totalMS;
                m_Labels[(int)Labels.MusicVolume].Text = m_MusicVolume.Value.ToString();
                m_Labels[(int)Labels.SoundVolume].Text = m_SoundVolume.Value.ToString();
            }

            base.Update(totalMS, frameMS);
        }

        public int GetCurrentFullScreenIndex()
        {
            string res = string.Format("{0}x{1}", Settings.World.FullScreenResolution.Width, Settings.World.FullScreenResolution.Height);
            for (int i = 0; i < Resolutions.FullScreenResolutionsList.Count; i++)
            {
                if (Resolutions.FullScreenResolutionsList[i].Width == Settings.World.FullScreenResolution.Width && Resolutions.FullScreenResolutionsList[i].Height == Settings.World.FullScreenResolution.Height)
                    return i;
            }
            return -1;
        }

        public int GetCurrentPlayWindowIndex()
        {
            string res = string.Format("{0}x{1}", Settings.World.PlayWindowGumpResolution.Width, Settings.World.PlayWindowGumpResolution.Height);
            for (int i = 0; i < Resolutions.PlayWindowResolutionsList.Count; i++)
            {
                if (Resolutions.PlayWindowResolutionsList[i].Width == Settings.World.PlayWindowGumpResolution.Width && Resolutions.PlayWindowResolutionsList[i].Height == Settings.World.PlayWindowGumpResolution.Height)
                    return i;
            }
            return -1;
        }

        public string[] CreateResolutionsStringArrayFromList(List<ResolutionConfig> resolutions)
        {
            string[] array = new string[resolutions.Count];
            for (int i = 0; i < resolutions.Count; i++)
                array[i] = resolutions[i].ToString();
            return array;
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
            Settings.World.FullScreenResolution = new ResolutionConfig(Resolutions.FullScreenResolutionsList[m_DropDownFullScreenResolutions.Index].Width, Resolutions.FullScreenResolutionsList[m_DropDownFullScreenResolutions.Index].Height);
            Settings.World.PlayWindowGumpResolution = new ResolutionConfig(Resolutions.PlayWindowResolutionsList[m_DropDownPlayWindowResolutions.Index].Width, Resolutions.PlayWindowResolutionsList[m_DropDownPlayWindowResolutions.Index].Height);
            Settings.Game.IsVSyncEnabled = m_IsVSyncEnabled.IsChecked;
            Settings.Debug.IsConsoleEnabled = m_IsConsoleEnabled.IsChecked;
            Settings.Debug.ShowFps = m_ShowFps.IsChecked;

            Settings.Game.SpeechColor = m_SpeechColor.Index;
            Settings.Game.EmoteColor = m_EmoteColor.Index;
            Settings.Game.PartyMsgColor = m_PartyMsgColor.Index;
            Settings.Game.GuildMsgColor = m_GuildMsgColor.Index;
            Settings.Game.IgnoreGuildMsg = m_IgnoreGuildMsg.IsChecked;
            Settings.Game.AllianceMsgColor = m_AllianceMsgColor.Index;
            Settings.Game.IgnoreAllianceMsg = m_IgnoreAllianceMsg.IsChecked;
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

        public override void OnButtonClick(int buttonID)
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
