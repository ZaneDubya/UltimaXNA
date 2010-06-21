using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.UILegacy.Gumplings;

namespace UltimaXNA.UILegacy
{
    public class MsgBox : Gump
    {
        string _msg;
        HtmlGump _text;
        Button _okay;

        public PublicControlEvent OnClose;

        public MsgBox(string msg)
            : base(0, 0)
        {
            _msg = "<big>" + msg + "</big>";
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (IsInitialized && _text == null)
            {
                _text = new HtmlGump(this, 0, 10, 10, 200, 200, 0, 0, _msg);
                AddGumpling(_text);
                base.Update(gameTime);
                _controls.Clear();
                ResizePic resize = new ResizePic(this, 0, 0, 0, 9200, _text.Width + 20, _text.Height + 45);
                AddGumpling(resize);
                AddGumpling(_text);
                _okay = new Button(this, 0, (_text.Width + 20) / 2 - 23, _text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0);
                _okay.GumpOverID = 2076;
                AddGumpling(_okay);
                base.Update(gameTime);
                Center();
            }
            base.Update(gameTime);
        }

        public override void ActivateByButton(int buttonID)
        {
            if (OnClose != null)
                OnClose();
            this.Dispose();
        }
    }
}
