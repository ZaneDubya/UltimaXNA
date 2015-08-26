/***************************************************************************
 *   MsgBoxGump.cs
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
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI
{
    public enum MsgBoxTypes
    {
        OkOnly,
        OkCancel
    }

    public class MsgBoxGump : Gump
    {
        /// <summary>
        /// Opens a modal message box with either 'OK' or 'OK and Cancel' buttons.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        public static MsgBoxGump Show(string msg, MsgBoxTypes type)
        {
            MsgBoxGump gump = new MsgBoxGump(msg, type);
            return gump;
        }

        private string m_msg;
        private HtmlGumpling m_text;
        private MsgBoxTypes m_type;

        public Action OnClose;
        public Action OnCancel;

        private MsgBoxGump(string msg, MsgBoxTypes msgBoxType)
            : base(0, 0)
        {
            m_msg = "<big color=000000>" + msg;
            m_type = msgBoxType;
            UserInterface.AddControl(this, 0, 0);
            MetaData.IsModal = true;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (IsInitialized && m_text == null)
            {
                ResizePic resize;

                m_text = new HtmlGumpling(this, 10, 10, 200, 200, 0, 0, m_msg);
                int width = m_text.Width + 20;
                AddControl(resize = new ResizePic(this, 0, 0, 9200, width, m_text.Height + 45));
                AddControl(m_text);
                // Add buttons
                switch (m_type)
                {
                    case MsgBoxTypes.OkOnly:
                        AddControl(new Button(this, (m_text.Width + 20) / 2 - 23, m_text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0));
                        ((Button)LastControl).GumpOverID = 2076;
                        break;
                    case MsgBoxTypes.OkCancel:
                        AddControl(new Button(this, (width / 2) - 46 - 10, m_text.Height + 15, 0x817, 0x818, ButtonTypes.Activate, 0, 1));
                        ((Button)LastControl).GumpOverID = 0x819;
                        AddControl(new Button(this, (width / 2) + 10, m_text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0));
                        ((Button)LastControl).GumpOverID = 2076;
                        break;
                }
                
                base.Update(totalMS, frameMS);
                CenterThisControlOnScreen();
            }
            base.Update(totalMS, frameMS);
        }

        public override void OnButtonClick(int buttonID)
        {
            switch (buttonID)
            {
                case 0:
                    if (OnClose != null)
                        OnClose();
                    break;
                case 1:
                    if (OnCancel != null)
                        OnCancel();
                    break;
            }
            Dispose();
        }
    }
}
