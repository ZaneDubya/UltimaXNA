using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InterXLib.XGUI.Rendering;
using Microsoft.Xna.Framework;
using InterXLib.Display;


namespace InterXLib.XGUI.Elements.Views
{
    class ListBoxView<T> : AElementView where T : class
    {
        new ListBox<T> Model
        {
            get
            {
                return (ListBox<T>)base.Model;
            }
        }

        private int m_BorderWidth = 1;

        public int LineSpacing
        {
            get
            {
                return (int)Model.FontSize + 2;
            }
        }

        public Rectangle SafeArea
        {
            get
            {
                return new Rectangle(
                    Model.ScreenArea.X + m_BorderWidth,
                    Model.ScreenArea.Y + m_BorderWidth,
                    Model.ScreenArea.Width - (m_BorderWidth * 2),
                    Model.ScreenArea.Height - (m_BorderWidth * 2));
            }
        }

        public ListBoxView(ListBox<T> listbox, GUIManager manager)
            : base(listbox, manager)
        {

        }

        protected override void LoadRenderers()
        {

        }

        protected override void InternalDraw(YSpriteBatch spritebatch, double frameTime)
        {
            spritebatch.GUIClipRect_Push(Model.ScreenArea);

            Color background = new Color(96, 96, 96, 255);
            Color background_list_alternate = background * 1.2f;
            Color selected = Color.DarkGray;
            Color border = Color.LightGray;

            int linecount = (Model.LocalArea.Height / LineSpacing) + 1;
            int y = Model.ScreenArea.Y + m_BorderWidth;
            int width = Model.ScreenArea.Width - ((linecount <= Model.Items.Count) ? 11 : 0);

            DrawCommon_FillBackground(spritebatch, background); // width is different here because we might be drawing a scrollbar. Does that matter?

            int first_item = (Model.ScrollPosition / LineSpacing);
            int item_offset = (Model.ScrollPosition % LineSpacing);

            for (int i = 0; i < linecount; i++)
            {
                if (Model.Items.Count > (i + first_item))
                {
                    if (i == (Model.SelectedIndex - first_item))
                    {
                        spritebatch.DrawRectangleFilled(
                            new Vector3(Model.ScreenArea.X, y + i * LineSpacing - item_offset, 0),
                            new Vector2(width, LineSpacing), selected);
                    }
                    else if (i % 2 == 1)
                    {
                        spritebatch.DrawRectangleFilled(
                            new Vector3(Model.ScreenArea.X, y + i * LineSpacing - item_offset, 0),
                            new Vector2(width, LineSpacing), background_list_alternate);
                    }
                    spritebatch.DrawString(Font, Model.Items[i + first_item].ToString(), new Vector2(Model.ScreenArea.X + 4, y + i * LineSpacing + m_BorderWidth - item_offset), Model.FontSize);
                }
            }

            DrawCommon_Border(spritebatch, border);

            spritebatch.GUIClipRect_Pop();
        }
    }
}
