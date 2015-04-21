/***************************************************************************
 *   MsgBox.cs
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
    public class MsgBox : Gump
    {
        string m_msg;
        HtmlGump m_text;
        MsgBoxTypes m_type;

        public Action OnClose;
        public Action OnCancel;

        public MsgBox(string msg, MsgBoxTypes msgBoxType)
            : base(0, 0)
        {
            m_msg = "<big color=000000>" + msg;
            m_type = msgBoxType;
            IsModal = true;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (IsInitialized && m_text == null)
            {
                ResizePic resize;

                m_text = new HtmlGump(this, 0, 10, 10, 200, 200, 0, 0, m_msg);
                int width = m_text.Width + 20;
                AddControl(resize = new ResizePic(this, 0, 0, 0, 9200, width, m_text.Height + 45));
                AddControl(m_text);
                // Add buttons
                switch (m_type)
                {
                    case MsgBoxTypes.OkOnly:
                        AddControl(new Button(this, 0, (m_text.Width + 20) / 2 - 23, m_text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0));
                        ((Button)LastControl).GumpOverID = 2076;
                        break;
                    case MsgBoxTypes.OkCancel:
                        AddControl(new Button(this, 0, (width / 2) - 46 - 10, m_text.Height + 15, 0x817, 0x818, ButtonTypes.Activate, 0, 1));
                        ((Button)LastControl).GumpOverID = 0x819;
                        AddControl(new Button(this, 0, (width / 2) + 10, m_text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0));
                        ((Button)LastControl).GumpOverID = 2076;
                        break;
                }
                
                base.Update(totalMS, frameMS);
                Center();
            }
            base.Update(totalMS, frameMS);
        }

        public override void ActivateByButton(int buttonID)
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
