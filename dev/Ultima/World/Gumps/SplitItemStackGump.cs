/***************************************************************************
 *   SplitItemStackGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using UltimaXNA.Ultima.Entities.Items;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    class SplitItemStackGump : Gump
    {
        public Item Item
        {
            get;
            private set;
        }

        private HSliderBar m_Slider;
        private TextEntry m_AmountEntry;
        private int m_LastValue = 0;

        public SplitItemStackGump(Item item)
            : base(0, 0)
        {
            IsMovable = true;

            Item = item;
            // Background
            AddControl(new GumpPic(this, 0, 0, 0, 0x085c, 0));
            // Slider
            m_Slider = (HSliderBar)AddControl(new HSliderBar(this, 0, 30, 16, 104, 1, item.Amount, item.Amount, HSliderBarStyle.BlueWidgetNoBar));
            m_LastValue = m_Slider.Value;
            // Ok button
            AddControl(new Button(this, 1, 102, 38, 0x085d, 0x085e, ButtonTypes.Default, 0, 0));
            ((Button)LastControl).GumpOverID = 0x085f;
            // Text entry field
            m_AmountEntry = (TextEntry)AddControl(new TextEntry(this, 0, 30, 39, 60, 16, 0, 0, 5, item.Amount.ToString()));
            m_AmountEntry.HtmlTag = "<big>";
            m_AmountEntry.LegacyCarat = true;
            m_AmountEntry.Hue = 1001;
        }

        public override void Update(double totalMS, double frameMS)
        {
            // update children controls first
            base.Update(totalMS, frameMS);

            // update strategy: if slider != last value, then set text equal to slider value. else try parsing text.
            //                  if text is empty, value = minvalue.
            //                  if can't parse text, then set text equal to slider value.
            //                  if can parse text, and text != slider, then set slider = text.
            if (m_Slider.Value != m_LastValue)
            {
                m_AmountEntry.Text = m_Slider.Value.ToString();
            }
            else
            {
                int textValue;
                if (m_AmountEntry.Text.Length == 0)
                {
                    m_Slider.Value = m_Slider.MinValue;
                    m_AmountEntry.Text = m_Slider.Value.ToString();
                }
                else if (!int.TryParse(m_AmountEntry.Text, out textValue))
                {
                    m_AmountEntry.Text = m_Slider.Value.ToString();
                }
                else
                {
                    if (textValue != m_Slider.Value)
                    {
                        if (textValue <= m_Slider.MaxValue)
                            m_Slider.Value = textValue;
                        else
                        {
                            m_Slider.Value = m_Slider.MaxValue;
                            m_AmountEntry.Text = m_Slider.Value.ToString();
                        }
                    }
                }
            }
            m_LastValue = m_Slider.Value;
        }
    }
}
