using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Ultima.UI.Controls
{
    class RadioButton : CheckBox
    {
        public int GroupIndex
        {
            get;
            protected set;
        }

        public RadioButton(AControl owner, int page, int groupIndex, string[] arguements, string[] lines)
            : base(owner, page, arguements, lines)
        {
            GroupIndex = groupIndex;
        }

        protected override void OnMouseClick(int x, int y, Core.Input.Windows.MouseButton button)
        {
            base.OnMouseClick(x, y, button);
            if (Owner != null)
            {
                foreach (AControl control in Owner.Children)
                {
                    if (control is RadioButton && (control as RadioButton).GroupIndex == GroupIndex)
                        (control as RadioButton).IsChecked = false;
                }
            }
        }
    }
}
