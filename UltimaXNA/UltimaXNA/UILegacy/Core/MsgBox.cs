using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy
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
            _msg = "<big>" + msg + "</big>";
            _type = msgBoxType;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (IsInitialized && _text == null)
            {
                _text = new HtmlGump(this, 0, 10, 10, 200, 200, 0, 0, _msg);
                AddGumpling(_text);
                base.Update(gameTime);
                _controls.Clear();
                int width = _text.Width + 20;
                ResizePic resize = new ResizePic(this, 0, 0, 0, 9200, _text.Width + 20, _text.Height + 45);
                AddGumpling(resize);
                AddGumpling(_text);
                // Add buttons
                switch (_type)
                {
                    case MsgBoxTypes.OkOnly:
                        AddGumpling(new Button(this, 0, (_text.Width + 20) / 2 - 23, _text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0));
                        ((Button)LastGumpling).GumpOverID = 2076;
                        break;
                    case MsgBoxTypes.OkCancel:
                        AddGumpling(new Button(this, 0, (width / 2) - 46 - 10, _text.Height + 15, 0x817, 0x818, ButtonTypes.Activate, 0, 1));
                        ((Button)LastGumpling).GumpOverID = 0x819;
                        AddGumpling(new Button(this, 0, (width / 2) + 10, _text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0));
                        ((Button)LastGumpling).GumpOverID = 2076;
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
