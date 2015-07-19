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

        public RadioButton(AControl owner, int groupIndex, string[] arguements, string[] lines)
            : base(owner, arguements, lines)
        {
            GroupIndex = groupIndex;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
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
