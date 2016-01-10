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
using System;
using System.Collections.Generic;
using System.Linq;
using UltimaXNA.Configuration.Properties;
using UltimaXNA.Core;
using UltimaXNA.Core.UI;
using UltimaXNA.Core.Windows;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.UI.Controls;
using UltimaXNA.Ultima.World;
#endregion

namespace UltimaXNA.Ultima.UI.WorldGumps
{
    public class OptionsGump : Gump
    {
        private UserInterfaceService m_UserInterface;
        private WorldModel m_World;
        private HSliderBar m_MusicVolume;
        private HSliderBar m_SoundVolume;
        private CheckBox m_MusicOn;
        private CheckBox m_SoundOn;
        private CheckBox m_FootStepSoundOn;
        private CheckBox m_AlwaysRun;
        private CheckBox m_MenuBarDisabled;

        private CheckBox m_IsVSyncEnabled;
        private CheckBox m_ShowFps;
        private CheckBox m_IsConsoleEnabled;
        private ColorPickerBox m_SpeechColor;
        private ColorPickerBox m_EmoteColor;
        private ColorPickerBox m_PartyMsgColor;
        private ColorPickerBox m_GuildMsgColor;
        private CheckBox m_IgnoreGuildMsg;
        private ColorPickerBox m_AllianceMsgColor;
        private CheckBox m_IgnoreAllianceMsg;

        private DropDownList m_DropDownFullScreenResolutions;
        private DropDownList m_DropDownPlayWindowResolutions;

        #region about macro options

        private KeyPressControl m_MacroKeyPress;

        //three variable have to same capacity (example: 10)
        private MacroDropDownList[] m_ActionTypeList = new MacroDropDownList[10];

        //three variable have to same capacity (example: 10)
        private TextEntry[] m_Action1Text = new TextEntry[10];

        //three variable have to same capacity (example: 10)
        private MacroDropDownList[] m_Action1List = new MacroDropDownList[10];

        private CheckBox m_chkShift;
        private CheckBox m_chkAlt;
        private CheckBox m_chkCtrl;
        private int currentMacro = 0;
        private List<XMacro> deletedMacros = new List<XMacro>();
        private List<XMacro> addedMacros = new List<XMacro>();

        #endregion

        private double m_RefreshTime = 0d;
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
            AddControl(new Button(this, 0, 40, 217, 217, ButtonTypes.SwitchPage, 1, (int)Buttons.Sound), 1);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Sound and Music"), 1);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting affect the sound and music you will hear while playing Ultima Online."), 1);

            AddControl(new TextLabelAscii(this, 85, 85, 9, 1, @"Sound on/off"), 1);
            m_SoundOn = AddControl<CheckBox>(new CheckBox(this, 60, 80, 210, 211, Settings.Audio.SoundOn, 61), 1);

            AddControl(new TextLabelAscii(this, 60, 110, 9, 1, @"Sound volume"), 1);
            m_SoundVolume = AddControl<HSliderBar>(new HSliderBar(this, 60, 130, 150, 0, 100, Settings.Audio.SoundVolume, HSliderBarStyle.MetalWidgetRecessedBar), 1);
            m_Labels[(int)Labels.SoundVolume] = AddControl<TextLabelAscii>(new TextLabelAscii(this, 220, 130, 9, 1, Settings.Audio.SoundVolume.ToString()), 1);

            AddControl(new TextLabelAscii(this, 85, 155, 9, 1, @"Music on/off"), 1);
            m_MusicOn = AddControl<CheckBox>(new CheckBox(this, 60, 150, 210, 211, Settings.Audio.MusicOn, 62), 1);

            AddControl(new TextLabelAscii(this, 60, 180, 9, 1, @"Music volume"), 1);
            m_MusicVolume = AddControl<HSliderBar>(new HSliderBar(this, 60, 200, 150, 0, 100, Settings.Audio.MusicVolume, HSliderBarStyle.MetalWidgetRecessedBar), 1);
            m_Labels[(int)Labels.MusicVolume] = AddControl<TextLabelAscii>(new TextLabelAscii(this, 220, 200, 9, 1, m_MusicVolume.Value.ToString()), 1);

            AddControl(new TextLabelAscii(this, 85, 225, 9, 1, @"Play footstep sound"), 1);
            m_FootStepSoundOn = AddControl<CheckBox>(new CheckBox(this, 60, 220, 210, 211, Settings.Audio.FootStepSoundOn, 62), 1);

            // page 2 Pop-up Help
            AddControl(new Button(this, 0, 110, 219, 219, ButtonTypes.SwitchPage, 2, (int)Buttons.Help), 2);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Pop-up Help"), 2);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting configure the behavior of the pop-up help."), 2);

            // page 3 Chat
            AddControl(new Button(this, 0, 250, 223, 223, ButtonTypes.SwitchPage, 3, (int)Buttons.Chat), 3);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Chat"), 3);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting affect the interface display for chat system."), 3);

            // page 4 Macro Options
            AddControl(new Button(this, 0, 320, 236, 236, ButtonTypes.SwitchPage, 4, (int)Buttons.Macros), 4);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Macro Options"), 4);
            AddControl(new TextLabelAscii(this, 60, 40, 9, 1, @""), 4);
            AddControl(new Button(this, 180, 60, 2460, 2461, ButtonTypes.Activate, 4, (int)Buttons.MAdd), 4); // add
            AddControl(new Button(this, 234, 60, 2463, 2464, ButtonTypes.Activate, 4, (int)Buttons.MDelete), 4); // delete
            AddControl(new Button(this, 302, 60, 2466, 2467, ButtonTypes.Activate, 4, (int)Buttons.MPrevious), 4); // previous
            AddControl(new Button(this, 386, 60, 2469, 2470, ButtonTypes.Activate, 4, (int)Buttons.MNext), 4); // next
            AddControl(new TextLabelAscii(this, 125, 85, 9, 1, @"Keystroke"), 4);
            //here is controlling key press event
            KeyPressControl myKeyPress = new KeyPressControl(this, 130, 100, 57, 14, 4000);
            AddControl(new ResizePic(this, myKeyPress), 4);
            m_MacroKeyPress = AddControl<KeyPressControl>(myKeyPress, 4);
            ///
            AddControl(new TextLabelAscii(this, 195, 100, 9, 1, @"Key"), 4);

            m_chkShift = AddControl<CheckBox>(new CheckBox(this, 260, 90, 2151, 2153, Settings.Audio.FootStepSoundOn, 62), 4); //shift
            m_chkAlt = AddControl<CheckBox>(new CheckBox(this, 330, 90, 2151, 2153, Settings.Audio.FootStepSoundOn, 62), 4); //alt
            m_chkCtrl = AddControl<CheckBox>(new CheckBox(this, 400, 90, 2151, 2153, Settings.Audio.FootStepSoundOn, 62), 4); //ctrl
            AddControl(new TextLabelAscii(this, 285, 90, 9, 1, @"Shift"), 4);
            AddControl(new TextLabelAscii(this, 355, 90, 9, 1, @"Alt"), 4);
            AddControl(new TextLabelAscii(this, 425, 90, 9, 1, @"Ctrl"), 4);

            AddControl(new TextLabelAscii(this, 180, 135, 9, 1, @"ACTION"), 4);
            AddControl(new TextLabelAscii(this, 420, 135, 9, 1, @"VALUE"), 4);
            //here is controlling the macro's action type and controlling another dropdown list for visual
            int y = 0;
            for (int i = 0; i < m_ActionTypeList.Count(); i++)
            {
                //number of action
                AddControl(new TextLabelAscii(this, 84, 155 + y, 9, 1, (i + 1).ToString()), 4);
                //here is action dropdown list (i need ID variable for find in controls)
                m_ActionTypeList[i] = AddControl<MacroDropDownList>(new MacroDropDownList(this, 100, 150 + y, 215, Settings.fromList<Macro>(Settings.Macro.actionTypes), 10, 0, false, (i + 1000), true), 4);
                //here is value dropdown list (i need ID variable for find in controls)
                m_Action1List[i] = AddControl<MacroDropDownList>(new MacroDropDownList(this, 330, 150 + y, 190, new string[] { }, 10, 0, false, (i + 2000), false), 4);
                //visual control about resizable picture
                m_Action1List[i].IsVisible = false;
                //here is textentry for example: Say,Emote,Yell (i need ID variable for find in controls)
                m_Action1Text[i] = AddControl<TextEntry>(new TextEntry(this, 340, 150 + y, 160, 20, 1, (3000 + i), 80, string.Empty), 4);
                //visual control about can write
                m_Action1Text[i].IsEditable = false;
                y += 25;
            }
            //

            // page 5 Interface
            AddControl(new Button(this, 576, 40, 225, 225, ButtonTypes.SwitchPage, 5, (int)Buttons.Interface), 5);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Interface"), 5);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting affect your interface."), 5);

            AddControl(new TextLabelAscii(this, 85, 85, 9, 1, @"Your character will always run if this is checked"), 5);
            m_AlwaysRun = AddControl<CheckBox>(new CheckBox(this, 60, 80, 210, 211, Settings.UserInterface.AlwaysRun, 61), 5);

            AddControl(new TextLabelAscii(this, 85, 115, 9, 1, @"Disable the Menu Bar"), 5);
            m_MenuBarDisabled = AddControl<CheckBox>(new CheckBox(this, 60, 110, 210, 211, Settings.UserInterface.MenuBarDisabled, 61), 5);

            // page 6 Display
            AddControl(new Button(this, 576, 110, 227, 227, ButtonTypes.SwitchPage, 6, (int)Buttons.Display), 6);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Display"), 6);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting affect your display, and adjusting some of them may improve your graphics performance.", 430), 6);

            AddControl(new TextLabelAscii(this, 85, 80, 9, 1, @"Enable vertical synchronization"), 6);
            m_IsVSyncEnabled = AddControl<CheckBox>(new CheckBox(this, 60, 80, 210, 211, Settings.Engine.IsVSyncEnabled, 61), 6);

            AddControl(new TextLabelAscii(this, 85, 100, 9, 1, @"Some unused option"), 6);
            AddControl(new CheckBox(this, 60, 100, 210, 211, false, 62), 6);

            AddControl(new TextLabelAscii(this, 85, 120, 9, 1, @"Use full screen display"), 6);
            AddControl(new CheckBox(this, 60, 120, 210, 211, Settings.UserInterface.IsMaximized, 61), 6);

            AddControl(new TextLabelAscii(this, 60, 150, 9, 1, @"Full Screen Resolution:"), 6);
            m_DropDownFullScreenResolutions = AddControl<DropDownList>(new DropDownList(this, 60, 165, 122, Settings.fromList<ResolutionProperty>(Resolutions.FullScreenResolutionsList), 10, GetCurrentFullScreenIndex(), false), 6);

            AddControl(new TextLabelAscii(this, 60, 190, 9, 1, @"Play Window Resolution:"), 6);
            m_DropDownPlayWindowResolutions = AddControl<DropDownList>(new DropDownList(this, 60, 205, 122, Settings.fromList<ResolutionProperty>(Resolutions.FullScreenResolutionsList), 10, GetCurrentPlayWindowIndex(), false), 6);

            AddControl(new TextLabelAscii(this, 85, 235, 9, 1, @"Speech color"), 6);
            m_SpeechColor = AddControl<ColorPickerBox>(new ColorPickerBox(this, new Rectangle(60, 235, 15, 15), new Rectangle(60, 235, 450, 150), 30, 10, Hues.TextTones, Settings.UserInterface.SpeechColor), 6);

            AddControl(new TextLabelAscii(this, 85, 255, 9, 1, @"Emote color"), 6);
            m_EmoteColor = AddControl<ColorPickerBox>(new ColorPickerBox(this, new Rectangle(60, 255, 15, 15), new Rectangle(60, 255, 450, 150), 30, 10, Hues.TextTones, Settings.UserInterface.EmoteColor), 6);

            AddControl(new TextLabelAscii(this, 85, 275, 9, 1, @"Party message color"), 6);
            m_PartyMsgColor = AddControl<ColorPickerBox>(new ColorPickerBox(this, new Rectangle(60, 275, 15, 15), new Rectangle(60, 275, 450, 150), 30, 10, Hues.TextTones, Settings.UserInterface.PartyMsgColor), 6);

            AddControl(new TextLabelAscii(this, 85, 295, 9, 1, @"Guild message color"), 6);
            m_GuildMsgColor = AddControl<ColorPickerBox>(new ColorPickerBox(this, new Rectangle(60, 295, 15, 15), new Rectangle(60, 295, 450, 150), 30, 10, Hues.TextTones, Settings.UserInterface.GuildMsgColor), 6);

            AddControl(new TextLabelAscii(this, 85, 315, 9, 1, @"Ignore guild messages"), 6);
            m_IgnoreGuildMsg = AddControl<CheckBox>(new CheckBox(this, 60, 315, 210, 211, Settings.UserInterface.IgnoreGuildMsg, 62), 6);

            AddControl(new TextLabelAscii(this, 85, 335, 9, 1, @"Alliance message color"), 6);
            m_AllianceMsgColor = AddControl<ColorPickerBox>(new ColorPickerBox(this, new Rectangle(60, 335, 15, 15), new Rectangle(60, 335, 450, 150), 30, 10, Hues.TextTones, Settings.UserInterface.AllianceMsgColor), 6);

            AddControl(new TextLabelAscii(this, 85, 355, 9, 1, @"Ignore alliance messages"), 6);
            m_IgnoreAllianceMsg = AddControl<CheckBox>(new CheckBox(this, 60, 355, 210, 211, Settings.UserInterface.IgnoreAllianceMsg, 62), 6);

            // page 7 Reputation system
            AddControl(new Button(this, 576, 180, 229, 229, ButtonTypes.SwitchPage, 7, (int)Buttons.Reputation), 7);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Reputation system"), 7);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settting affect the reputation system, which is Ultima Online's system for controlling antisocial behavior."), 7);

            // page 8 Miscellaneous
            AddControl(new Button(this, 576, 250, 231, 231, ButtonTypes.SwitchPage, 8, (int)Buttons.Miscellaneous), 8);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Miscellaneous"), 8);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"Miscellaneous options."), 8);

            AddControl(new TextLabelAscii(this, 85, 80, 9, 1, @"Enable debug console"), 8);
            m_IsConsoleEnabled = AddControl<CheckBox>(new CheckBox(this, 60, 80, 210, 211, Settings.Debug.IsConsoleEnabled, 61), 8);

            AddControl(new TextLabelAscii(this, 85, 100, 9, 1, @"Show FPS"), 8);
            m_ShowFps = AddControl<CheckBox>(new CheckBox(this, 60, 100, 210, 211, Settings.Debug.ShowFps, 61), 8);

            // page 9 Filter Options
            AddControl(new Button(this, 576, 320, 234, 234, ButtonTypes.SwitchPage, 9, (int)Buttons.Filters), 9);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Filter Options"), 9);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @""), 9);

            //ALL CONTROLS ADDED. NOW I INCLUDE THE MACROS FROM MEMORY (AFTER LOAD FROM FILE(Characher Login Packet))....
            ChangeCurrentMacro(Settings.Macro.UserMacros.getMacros().Count - 1);//SELECTED LAST MACRO HAVE FUNN
        }

        public void ChangeCurrentMacro(int index)
        {
            setDefaultDropdownList(false);

            if (Settings.Macro.UserMacros.Count == 0)
                return;

            XMacro MCR = Settings.Macro.UserMacros.getMacro(index);
            currentMacro = index;
            int IntValeTest = 0;
            if (MCR.Keystroke.ToString()[0] == 'D' && int.TryParse(MCR.Keystroke.ToString()[1].ToString(), out IntValeTest) && MCR.Keystroke.ToString().Length == 2)
            {
                m_MacroKeyPress.Text = IntValeTest.ToString();
            }
            else
            {
                m_MacroKeyPress.Text = MCR.Keystroke.ToString();
            }
            m_chkShift.IsChecked = MCR.Shift;
            m_chkAlt.IsChecked = MCR.Alt;
            m_chkCtrl.IsChecked = MCR.Ctrl;
            for (int i = 0; i < MCR.actionList.Count; i++)
            {
                m_ActionTypeList[i].Index = MCR.actionList[i].actionID;//SELECTED ACTION
                if (MCR.actionList[i].valueID != -1)
                {
                    if (!m_Action1List[i].m_firstvisible)
                    {
                        m_Action1List[i].CreateVisual();//ACTIVATED VISUAL
                    }
                    m_Action1List[i].setIndex(MCR.actionList[i].actionID, MCR.actionList[i].valueID);//visual changing
                    m_Action1Text[i].IsEditable = false;
                }
                else if (MCR.actionList[i].valueText != "N")
                {
                    if (!m_Action1List[i].m_firstvisible)
                    {
                        m_Action1List[i].CreateVisual();//ACTIVATED VISUAL
                    }
                    //m_Action1List[i].m_scrollButton.IsVisible = false;//SCROLL İCON İT'S REALLY PROBLEM FOR ME :( İ CAN'T TO MYSELF SO I USED SELF METHOD
                    m_Action1List[i].m_scrollButton.IsVisible = true;
                    m_Action1List[i].m_items.Clear();
                    m_Action1List[i].IsVisible = true;
                    m_Action1Text[i].IsEditable = true;
                    m_Action1Text[i].IsVisible = true;
                    m_Action1Text[i].Text = MCR.actionList[i].valueText;
                }
                else
                {
                    //NONE VALUE
                }
            }
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
            string res = string.Format("{0}x{1}", Settings.UserInterface.FullScreenResolution.Width, Settings.UserInterface.FullScreenResolution.Height);
            for (int i = 0; i < Resolutions.FullScreenResolutionsList.Count; i++)
            {
                if (Resolutions.FullScreenResolutionsList[i].Width == Settings.UserInterface.FullScreenResolution.Width && Resolutions.FullScreenResolutionsList[i].Height == Settings.UserInterface.FullScreenResolution.Height)
                    return i;
            }
            return -1;
        }

        public int GetCurrentPlayWindowIndex()
        {
            string res = string.Format("{0}x{1}", Settings.UserInterface.PlayWindowGumpResolution.Width, Settings.UserInterface.PlayWindowGumpResolution.Height);
            for (int i = 0; i < Resolutions.PlayWindowResolutionsList.Count; i++)
            {
                if (Resolutions.PlayWindowResolutionsList[i].Width == Settings.UserInterface.PlayWindowGumpResolution.Width && Resolutions.PlayWindowResolutionsList[i].Height == Settings.UserInterface.PlayWindowGumpResolution.Height)
                    return i;
            }
            return -1;
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
            Settings.UserInterface.AlwaysRun = m_AlwaysRun.IsChecked;
            Settings.UserInterface.MenuBarDisabled = m_MenuBarDisabled.IsChecked;
            Settings.UserInterface.FullScreenResolution = new ResolutionProperty(Resolutions.FullScreenResolutionsList[m_DropDownFullScreenResolutions.Index].Width, Resolutions.FullScreenResolutionsList[m_DropDownFullScreenResolutions.Index].Height);
            Settings.UserInterface.PlayWindowGumpResolution = new ResolutionProperty(Resolutions.PlayWindowResolutionsList[m_DropDownPlayWindowResolutions.Index].Width, Resolutions.PlayWindowResolutionsList[m_DropDownPlayWindowResolutions.Index].Height);
            Settings.Engine.IsVSyncEnabled = m_IsVSyncEnabled.IsChecked;
            Settings.Debug.IsConsoleEnabled = m_IsConsoleEnabled.IsChecked;
            Settings.Debug.ShowFps = m_ShowFps.IsChecked;

            Settings.UserInterface.SpeechColor = m_SpeechColor.Index;
            Settings.UserInterface.EmoteColor = m_EmoteColor.Index;
            Settings.UserInterface.PartyMsgColor = m_PartyMsgColor.Index;
            Settings.UserInterface.GuildMsgColor = m_GuildMsgColor.Index;
            Settings.UserInterface.IgnoreGuildMsg = m_IgnoreGuildMsg.IsChecked;
            Settings.UserInterface.AllianceMsgColor = m_AllianceMsgColor.Index;
            Settings.UserInterface.IgnoreAllianceMsg = m_IgnoreAllianceMsg.IsChecked;

            //deleting selected macros
            for (int i = 0; i < deletedMacros.Count; i++)
            {
                Settings.Macro.UserMacros.removeMacro(deletedMacros[i]);
            }
            deletedMacros.Clear();
            ////
            //adding new macros
            for (int i = 0; i < addedMacros.Count; i++)
            {
                Settings.Macro.UserMacros.addKey(addedMacros[i]);
            }
            addedMacros.Clear();
            //CURRENT MACRO ADDING MAYBE NOT SET
            AddingCurrentMacro();
            ///
            Settings.Macro.UserMacros.saveMacros();//saved macros
            //
            SwitchTopMenuGump();
        }

        public void setDefaultDropdownList(bool isEditable)
        {
            m_MacroKeyPress.Text = "Any Press";
            m_chkShift.IsChecked = false;
            m_chkAlt.IsChecked = false;
            m_chkCtrl.IsChecked = false;
            for (int i = 0; i < m_ActionTypeList.Count(); i++)
            {
                m_ActionTypeList[i].Index = 0;
                m_Action1List[i].IsVisible = false;
                if (m_Action1List[i].m_firstvisible)
                    m_Action1List[i].m_scrollButton.IsVisible = false;

                m_Action1Text[i].Text = "";
                m_Action1Text[i].IsVisible = isEditable;
            }
        }

        public void AddingCurrentMacro()
        {
            if (m_MacroKeyPress.Text != "Any Press")
            {
                XMacro xmcr = new XMacro();

                #region baddd

                bool isNumber = false;
                if (m_MacroKeyPress.Text.Length == 1)
                {
                    int IntValeTest = 0;
                    isNumber = int.TryParse(m_MacroKeyPress.Text, out IntValeTest);
                }
                if (isNumber)
                {
                    xmcr.Keystroke = (WinKeys)Enum.Parse(typeof(WinKeys), "D" + m_MacroKeyPress.Text, true);
                }
                else
                {
                    xmcr.Keystroke = (WinKeys)Enum.Parse(typeof(WinKeys), m_MacroKeyPress.Text, true);
                }

                #endregion

                //xmcr.Keystroke = (WinKeys)Enum.Parse(typeof(WinKeys), m_MacroKeyPress.Text, true);
                xmcr.Shift = m_chkShift.IsChecked;
                xmcr.Alt = m_chkAlt.IsChecked;
                xmcr.Ctrl = m_chkCtrl.IsChecked;
                for (int i2 = 0; i2 < m_ActionTypeList.Length; i2++)
                {
                    if (m_ActionTypeList[i2].Index == 0)
                    {
                        break;
                    }
                    XAction action = new XAction();
                    action.actionID = m_ActionTypeList[i2].Index;
                    if (m_Action1List[i2].Index != -1)
                        action.valueID = m_Action1List[i2].Index;
                    else
                        action.valueText = m_Action1Text[i2].Text;

                    xmcr.actionList.Add(action);
                }
                if (xmcr.actionList.Count > 0)
                {
                    int inx = addedMacros.FindIndex(p => p == xmcr);
                    if (inx != -1)
                        addedMacros.RemoveAt(inx);
                    addedMacros.Add(xmcr);
                }
            }
        }

        public void SwitchTopMenuGump()
        {
            if (!Settings.UserInterface.MenuBarDisabled && m_UserInterface.GetControl<TopMenuGump>() == null)
            {
                m_UserInterface.AddControl(new TopMenuGump(), 0, 0); // by default at the top of the screen.
            }
            else if (Settings.UserInterface.MenuBarDisabled && m_UserInterface.GetControl<TopMenuGump>() != null)
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
                case Buttons.MAdd:
                    for (int i = 0; i < addedMacros.Count; i++)
                    {
                        Settings.Macro.UserMacros.addKey(addedMacros[i]);
                    }
                    AddingCurrentMacro();
                    setDefaultDropdownList(true);
                    break;

                case Buttons.MDelete:
                    if (Settings.Macro.UserMacros.Count == 0)
                        return;

                    deletedMacros.Add(Settings.Macro.UserMacros.getMacro(currentMacro));
                    Settings.Macro.UserMacros.removeMacro(currentMacro);
                    currentMacro--;
                    if (currentMacro < 0)
                        currentMacro = 0;
                    ChangeCurrentMacro(currentMacro);
                    break;

                case Buttons.MPrevious:
                    currentMacro--;
                    if (currentMacro < 0)
                        currentMacro = 0;

                    ChangeCurrentMacro(currentMacro);
                    break;

                case Buttons.MNext:
                    currentMacro++;
                    if (currentMacro >= Settings.Macro.UserMacros.getMacros().Count)
                        currentMacro = Settings.Macro.UserMacros.getMacros().Count - 1;

                    ChangeCurrentMacro(currentMacro);
                    break;
            }
        }

        private enum Buttons
        {
            Sound,
            Help,
            Chat,
            Macros,
            MAdd,
            MDelete,
            MPrevious,
            MNext,
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

        protected override void CloseWithRightMouseButton()
        {
            for (int i = 0; i < addedMacros.Count; i++)
            {
                Settings.Macro.UserMacros.removeMacro(addedMacros[i]);
            }
            addedMacros.Clear();
            for (int i = 0; i < deletedMacros.Count; i++)
            {
                Settings.Macro.UserMacros.addKey(deletedMacros[i]);
            }
            deletedMacros.Clear();
            Settings.Macro.UserMacros.saveMacros();
            base.CloseWithRightMouseButton();
        }
    }
}