using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    class RadioButton : CheckBox
    {
        public int GroupIndex
        {
            get;
            protected set;
        }

        public RadioButton(AControl parent, int groupIndex, string[] arguements, string[] lines)
            : base(parent, arguements, lines)
        {
            GroupIndex = groupIndex;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            base.OnMouseClick(x, y, button);
            if (Parent != null)
            {
                foreach (AControl control in Parent.Children)
                {
                    if (control is RadioButton && (control as RadioButton).GroupIndex == GroupIndex)
                        (control as RadioButton).IsChecked = false;
                }
            }
        }
    }
}
