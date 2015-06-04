/***************************************************************************
 *   UseSkillButtonGump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Ultima.Player;
using UltimaXNA.Ultima.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.World.Gumps
{
    public class UseSkillButtonGump : Gump
    {
        // private variables
        private SkillEntry m_Skill;
        private ResizePic[] m_BG;
        private HtmlGumpling m_Caption;
        private bool m_IsMouseDown;
        // services
        private WorldModel m_World;

        public UseSkillButtonGump(SkillEntry skill)
            : base(skill.ID, 0)
        {
            while (UserInterface.GetControl<UseSkillButtonGump>(skill.ID) != null)
            {
                UserInterface.GetControl<UseSkillButtonGump>(skill.ID).Dispose();
            }

            m_Skill = skill;
            m_World = UltimaServices.GetService<WorldModel>();

            IsMovable = true;
            HandlesMouseInput = true;

            m_BG = new ResizePic[3];
            m_BG[0] = (ResizePic)AddControl(new ResizePic(this, 0, 0, 0, 0x24B8, 120, 40));
            m_BG[1] = (ResizePic)AddControl(new ResizePic(this, 0, 0, 0, 0x24EA, 120, 40));
            m_BG[2] = (ResizePic)AddControl(new ResizePic(this, 0, 0, 0, 0x251C, 120, 40));
            m_Caption = (HtmlGumpling)AddControl(new HtmlGumpling(this, 0, 0, 10, 120, 20, 0, 0, "<center>" + m_Skill.Name));

            for (int i = 0; i < 3; i++)
            {
                m_BG[i].MouseDownEvent += OnMouseDown;
                m_BG[i].MouseUpEvent += OnMouseUp;
                m_BG[i].MouseClickEvent += OnMouseClick;
            }
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            bool isMouseOver = (m_BG[0].IsMouseOver || m_BG[1].IsMouseOver || m_BG[2].IsMouseOver);
            if (m_IsMouseDown)
            {
                m_BG[0].IsVisible = false;
                m_BG[1].IsVisible = false;
                m_BG[2].IsVisible = true;

            }
            else if (isMouseOver)
            {
                m_BG[0].IsVisible = false;
                m_BG[1].IsVisible = true;
                m_BG[2].IsVisible = false;
            }
            else
            {
                m_BG[0].IsVisible = true;
                m_BG[1].IsVisible = false;
                m_BG[2].IsVisible = false;
            }

            if (m_IsMouseDown)
                m_Caption.Position = new Point(m_Caption.Position.X, m_Caption.Position.Y + 1);

            base.Draw(spriteBatch, position);

            if (m_IsMouseDown)
                m_Caption.Position = new Point(m_Caption.Position.X, m_Caption.Position.Y - 1);
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            m_IsMouseDown = true;
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            m_IsMouseDown = false;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            m_World.Interaction.UseSkill(m_Skill.Index);
        }
    }
}