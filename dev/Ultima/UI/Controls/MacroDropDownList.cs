using Microsoft.Xna.Framework;
using System.Collections.Generic;
using UltimaXNA.Configuration.Properties;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls
{
    internal class MacroDropDownList : AControl
    {
        public int Index;

        private int m_width;
        public List<string> m_items;
        private int m_visibleItems;
        private bool m_canBeNull;
        public bool m_firstvisible = true;
        public ResizePic m_resize;
        private TextLabelAscii m_label;
        public GumpPic m_scrollButton;

        private bool m_listOpen = false;

        private ResizePic m_openResizePic;
        private ScrollBar m_openScrollBar;
        private TextLabelAscii[] m_openLabels;

        private const int hue_Text = 1107;
        private const int hue_TextSelected = 588;

        private IFont m_Font;

        public MacroDropDownList(AControl parent)
                : base(parent)
        {
            HandlesMouseInput = true;

            m_Font = ServiceRegistry.GetService<IResourceProvider>().GetAsciiFont(1);
        }

        public MacroDropDownList(AControl parent, int x, int y, int width, string[] items, int itemsVisible, int index, bool canBeNull, bool firstVisible)
                : this(parent)
        {
            buildGumpling(x, y, width, items, itemsVisible, index, canBeNull, firstVisible);
        }

        public MacroDropDownList(AControl parent, int x, int y, int width, string[] items, int itemsVisible, int index, bool canBeNull, int ID, bool firstVisible)
                : this(parent, x, y, width, items, itemsVisible, index, canBeNull, firstVisible)
        {
            base.IDs = ID;
        }

        private void buildGumpling(int x, int y, int width, string[] items, int itemsVisible, int index, bool canBeNull, bool firstVisible)
        {
            Position = new Point(x, y);
            m_items = new List<string>(items);
            m_width = width;
            Index = index;
            m_visibleItems = itemsVisible;
            m_canBeNull = canBeNull;
            m_firstvisible = firstVisible;//hide creating control

            if (m_firstvisible)//for fill action dropdownlist
                CreateVisual();
        }

        public void CreateVisual()
        {
            if (m_resize != null || m_label != null || m_scrollButton != null)
                return;

            m_resize = (ResizePic)AddControl(new ResizePic(this, 0, 0, 3000, m_width, m_Font.Height + 8), 0);
            m_resize.IDs = this.IDs;
            m_resize.MouseClickEvent += onClickClosedList;
            m_resize.MouseOverEvent += onMouseOverClosedList;
            m_resize.MouseOutEvent += onMouseOutClosedList;
            m_resize.IsEnabled = false;
            m_label = (TextLabelAscii)AddControl(new TextLabelAscii(this, 4, 5, 1, hue_Text, string.Empty), 0);
            m_label.IDs = this.IDs;
            m_scrollButton = (GumpPic)AddControl(new GumpPic(this, m_width - 22, 5, 2086, 0), 0);

            m_firstvisible = true;//for invisible create control
        }

        public override void Dispose()
        {
            if (m_resize != null)
            {
                m_resize.MouseClickEvent -= onClickClosedList;
                m_resize.MouseOverEvent -= onMouseOverClosedList;
                m_resize.MouseOutEvent -= onMouseOutClosedList;
            }
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Index < 0 || Index >= m_items.Count)
                Index = -1;

            if (m_listOpen)
            {
                // if we have moused off the open list, close it. We check to see if the mouse is over:
                // the resizepic for the closed list (because it takes one update cycle to open the list)
                // the resizepic for the open list, and the scroll bar if it is loaded.
                if (UserInterface.MouseOverControl != m_openResizePic &&
                    UserInterface.MouseOverControl != m_resize &&
                    (m_openScrollBar == null ? false : UserInterface.MouseOverControl != m_openScrollBar))
                {
                    closeOpenList();
                }
                else
                {
                    // update the visible items
                    int itemOffset = (m_openScrollBar == null ? 0 : m_openScrollBar.Value);
                    if (m_items.Count != 0)
                    {
                        for (int i = 0; i < m_visibleItems; i++)
                        {
                            m_openLabels[i].Text = (i + itemOffset < 0) ? string.Empty : m_items[i + itemOffset];
                        }
                    }
                }
            }
            else if (m_firstvisible)//for create hide control
            {
                if (Index == -1)
                {
                    if (m_items.Count > 0)
                        m_label.Text = m_items[0];
                    else
                        m_label.Text = "";
                }
                else
                {
                    m_label.Text = m_items[Index];
                }
            }
            base.Update(totalMS, frameMS);
        }

        private void closeOpenList()
        {
            m_listOpen = false;
            if (m_openResizePic != null)
            {
                m_openResizePic.MouseClickEvent -= onClickOpenList;
                m_openResizePic.MouseOverEvent -= onMouseOverOpenList;
                m_openResizePic.MouseOutEvent -= onMouseOutOpenList;
                m_openResizePic.Dispose();
                m_openResizePic = null;
            }
            if (m_openScrollBar != null)
                m_openScrollBar.Dispose();
            for (int i = 0; i < m_visibleItems; i++)
                m_openLabels[i].Dispose();
        }

        private void onClickClosedList(AControl control, int x, int y, MouseButton button)
        {
            if (m_items.Count > 0)
            {
                m_listOpen = true;
                m_openResizePic = new ResizePic(Parent, X, Y, 3000, m_width, m_Font.Height * m_visibleItems + 8);
                m_openResizePic.IDs = IDs;
                m_openResizePic.MouseClickEvent += onClickOpenList;
                m_openResizePic.MouseOverEvent += onMouseOverOpenList;
                m_openResizePic.MouseOutEvent += onMouseOutOpenList;
                ((Gump)Parent).AddControl(m_openResizePic, this.Page);

                if (m_visibleItems > m_items.Count)
                {
                    m_visibleItems = m_items.Count;
                }

                // only show the scrollbar if we need to scroll
                if (m_visibleItems < m_items.Count)
                {
                    m_openScrollBar = new ScrollBar(Parent, X + m_width - 20, Y + 4, m_Font.Height * m_visibleItems, (m_canBeNull ? -1 : 0), m_items.Count - m_visibleItems, Index);
                    ((Gump)Parent).AddControl(m_openScrollBar, this.Page);
                }
                m_openLabels = new TextLabelAscii[m_visibleItems];
                for (int i = 0; i < m_visibleItems; i++)
                {
                    m_openLabels[i] = new TextLabelAscii(Parent, X + 4, Y + 5 + m_Font.Height * i, 1, 1106, string.Empty);
                    ((Gump)Parent).AddControl(m_openLabels[i], this.Page);
                }
            }
        }

        private void onMouseOverClosedList(AControl control, int x, int y)
        {
            m_label.Hue = hue_TextSelected;
        }

        private void onMouseOutClosedList(AControl control, int x, int y)
        {
            m_label.Hue = hue_Text;
        }

        public void setIndex(int actionID, int valueID)
        {
            this.m_scrollButton.IsVisible = true;
            this.IsVisible = true;
            this.m_items.Clear();
            this.Index = -1;
            MacroType mType = Settings.Macro.actionTypes[actionID].Type;
            switch (mType)
            {
                case MacroType.Skill:
                    this.m_items.AddRange(Settings.fromList(Settings.Macro.useSkills, true));
                    break;

                case MacroType.Spell:
                    this.m_items.AddRange(Settings.fromList(Settings.Macro.castSpell, true));
                    break;

                case MacroType.Display:
                    this.m_items.AddRange(Settings.fromList(Settings.Macro.Displays, true));
                    break;

                case MacroType.Text:
                    //its not for this method DELETEEEEEEEE
                    break;

                case MacroType.Move:
                    this.m_items.AddRange(Settings.fromList(Settings.Macro.Moves, true));
                    break;

                case MacroType.ArmDisarm:
                    this.m_items.AddRange(Settings.fromList(Settings.Macro.armDisarm, true));
                    break;

                case MacroType.None:
                    //its not for this method DELETEEEEEEEE
                    break;

                default:
                    break;
            }
            this.Index = valueID;
        }

        private void onClickOpenList(AControl control, int x, int y, MouseButton button)
        {
            //for macro options
            int id = control.IDs;
            int controlValueIndex = -1;
            for (int i = 0; i < Parent.Children.Count; i++)
            {
                if (Parent.Children[i].IDs == (id + 1000))
                {
                    controlValueIndex = i;
                    break;
                }
            }
            //////

            int indexOver = getOpenListIndexFromPoint(x, y);
            if (indexOver != -1)
                Index = indexOver + (m_openScrollBar == null ? 0 : m_openScrollBar.Value);
            closeOpenList();

            //for macro options
            if (controlValueIndex != -1)
            {
                if (indexOver == -1)
                    return;

                MacroType mType = Settings.Macro.actionTypes.Find(p => p.Name == m_openLabels[indexOver].Text).Type;
                //here is controlling the background's image visual
                if (!(Parent.Children[controlValueIndex] as MacroDropDownList).m_firstvisible)
                    (Parent.Children[controlValueIndex] as MacroDropDownList).CreateVisual();

                (Parent.Children[controlValueIndex] as MacroDropDownList).m_items.Clear();//cleaning dropdownlist
                (Parent.Children[controlValueIndex + 1] as TextEntry).Text = "";//cleaning text

                (Parent.Children[controlValueIndex] as MacroDropDownList).m_scrollButton.IsVisible = true;//easy way for visible dropdown list
                (Parent.Children[controlValueIndex] as MacroDropDownList).IsVisible = true;//easy way for visible dropdown list

                ///MAYBE THIS (SWITCH) WİLL CHANGİNG TO  "public void setIndex(int actionID, int valueID)" NOT NOW
                switch (mType)
                {
                    case MacroType.Skill:
                        (Parent.Children[controlValueIndex] as MacroDropDownList).m_items.AddRange(Settings.fromList(Settings.Macro.useSkills, true));//adding to dropdownlist
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//textentry disabled because i need dropdownlist
                        break;

                    case MacroType.Spell:
                        (Parent.Children[controlValueIndex] as MacroDropDownList).m_items.AddRange(Settings.fromList(Settings.Macro.castSpell, true));//adding to dropdownlist
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//textentry disabled because i need dropdownlist
                        break;

                    case MacroType.Display:
                        (Parent.Children[controlValueIndex] as MacroDropDownList).m_items.AddRange(Settings.fromList(Settings.Macro.Displays, true));//adding to dropdownlist
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//textentry disabled because i need dropdownlist
                        break;

                    case MacroType.Text:
                        //(Parent.Children[controlValueIndex] as MacroDropDownList).m_scrollButton.IsVisible = false; //as you wish
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = true;//textentry activated
                        break;

                    case MacroType.Move:
                        (Parent.Children[controlValueIndex] as MacroDropDownList).m_items.AddRange(Settings.fromList(Settings.Macro.Moves, true));//adding to dropdownlist
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//textentry disabled because i need dropdownlist
                        break;

                    case MacroType.ArmDisarm:
                        (Parent.Children[controlValueIndex] as MacroDropDownList).m_items.AddRange(Settings.fromList(Settings.Macro.armDisarm, true));//adding to dropdownlist
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//textentry disabled because i need dropdownlist
                        break;

                    case MacroType.None:
                        (Parent.Children[controlValueIndex] as MacroDropDownList).m_scrollButton.IsVisible = false;//i dont need any control :)
                        (Parent.Children[controlValueIndex + 1] as TextEntry).IsEditable = false;//i dont need any control :)
                        (Parent.Children[controlValueIndex] as MacroDropDownList).IsVisible = false;//i dont need any control :)
                        break;

                    default:
                        //unnecessary
                        break;
                }
            }
            /////
        }

        private void onMouseOverOpenList(AControl control, int x, int y)
        {
            int indexOver = getOpenListIndexFromPoint(x, y);
            for (int i = 0; i < m_openLabels.Length; i++)
            {
                if (i == indexOver)
                    m_openLabels[i].Hue = hue_TextSelected;
                else
                    m_openLabels[i].Hue = hue_Text;
            }
        }

        private void onMouseOutOpenList(AControl control, int x, int y)
        {
            for (int i = 0; i < m_openLabels.Length; i++)
                m_openLabels[i].Hue = hue_Text;
        }

        private int getOpenListIndexFromPoint(int x, int y)
        {
            Rectangle r = new Rectangle(4, 5, m_width - 20, m_Font.Height);
            for (int i = 0; i < m_openLabels.Length; i++)
            {
                if (r.Contains(new Point(x, y)))
                    return i;
                r.Y += m_Font.Height;
            }
            return -1;
        }
    }
}