using UltimaXNA.UltimaGUI;
/***************************************************************************
 *   MsgBox.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using UltimaXNA.UltimaGUI.Controls;

namespace UltimaXNA.UltimaGUI
{
    public enum MsgBoxTypes
    {
        OkOnly,
        OkCancel
    }
    public class MsgBox : Gump
    {
        string _msg;
        HtmlGump _text;
        MsgBoxTypes _type;

        public PublicControlEvent OnClose;
        public PublicControlEvent OnCancel;

        public MsgBox(string msg, MsgBoxTypes msgBoxType)
            : base(0, 0)
        {
            _msg = "<big color=000000>" + msg;
            _type = msgBoxType;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (IsInitialized && _text == null)
            {
                ResizePic resize;

                _text = new HtmlGump(this, 0, 10, 10, 200, 200, 0, 0, _msg);
                int width = _text.Width + 20;
                AddControl(resize = new ResizePic(this, 0, 0, 0, 9200, width, _text.Height + 45));
                AddControl(_text);
                // Add buttons
                switch (_type)
                {
                    case MsgBoxTypes.OkOnly:
                        AddControl(new Button(this, 0, (_text.Width + 20) / 2 - 23, _text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0));
                        ((Button)LastControl).GumpOverID = 2076;
                        break;
                    case MsgBoxTypes.OkCancel:
                        AddControl(new Button(this, 0, (width / 2) - 46 - 10, _text.Height + 15, 0x817, 0x818, ButtonTypes.Activate, 0, 1));
                        ((Button)LastControl).GumpOverID = 0x819;
                        AddControl(new Button(this, 0, (width / 2) + 10, _text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0));
                        ((Button)LastControl).GumpOverID = 2076;
                        break;
                }
                
                base.Update(gameTime);
                Center();
            }
            base.Update(gameTime);
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
            this.Dispose();
        }
    }
}
