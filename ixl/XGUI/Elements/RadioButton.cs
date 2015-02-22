using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.XGUI.Support;

namespace InterXLib.XGUI.Elements
{
    public class RadioButton : Button
    {
        protected override Patterns.MVC.AView CreateView()
        {
            return new Views.RadioButtonView(this, Manager);
        }

        protected override Patterns.MVC.AController CreateController()
        {
            return new Controllers.RadioButtonController(this, Manager);
        }

        public RadioButton(AElement parent, int page, RadioButtonGroup group)
            : base(parent, page)
        {
            IsDown = false;
            Group = group;
        }

        public bool IsDown { get; set; }

        private RadioButtonGroup m_Group = null;
        public RadioButtonGroup Group
        {
            get
            {
                return m_Group;
            }
            private set
            {
                if (value != m_Group)
                {
                    if (m_Group != null)
                        m_Group.RemoveButton(this);
                    if (value != null)
                    {
                        m_Group = value;
                        m_Group.AddButton(this);
                    }
                }
            }
        }
    }
}
