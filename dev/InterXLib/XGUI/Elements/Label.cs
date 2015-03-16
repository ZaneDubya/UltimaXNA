using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.Display;
using Microsoft.Xna.Framework;

namespace InterXLib.XGUI.Elements
{
    public class Label : AElement
    {
        public FontJustification Justification = FontJustification.Left;


        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.LabelView(this, Manager);
        }

        public Label(AElement parent, int page, string caption)
            : base(parent, page)
        {
            Caption = caption;
        }

        private string m_Caption = null;
        public string Caption
        {
            get
            {
                return m_Caption;
            }
            set
            {
                m_Caption = value;
            }
        }

        private Color m_Color = Color.White;
        public Color Color
        {
            get { return m_Color; }
            set
            {
                m_Color = value;
            }
        }
    }
}
