/***************************************************************************
 *   CheckBox.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
#endregion

namespace UltimaXNA.Ultima.UI.Controls
{
    /// <summary>
    /// A checkbox control.
    /// </summary>
    class CheckBox : AControl
    {
        Texture2D m_Inactive, m_Active;
        bool m_ischecked = false;

        public bool IsChecked
        {
            get { return m_ischecked; }
            set
            {
                m_ischecked = value;
            }
        }

        CheckBox(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
        }

        public CheckBox(AControl parent, string[] arguements, string[] lines)
            : this(parent)
        {
            int x, y, inactiveID, activeID, switchID;
            bool initialState;

            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            inactiveID = Int32.Parse(arguements[3]);
            activeID = Int32.Parse(arguements[4]);
            initialState = Int32.Parse(arguements[5]) == 1;
            switchID = Int32.Parse(arguements[6]);

            BuildGumpling(x, y, inactiveID, activeID, initialState, switchID);
        }

        public CheckBox(AControl parent, int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
            : this(parent)
        {
            BuildGumpling(x, y, inactiveID, activeID, initialState, switchID);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
            if (IsChecked && m_Active != null)
            {
                spriteBatch.Draw2D(m_Active, new Vector3(position.X, position.Y, 0), Vector3.Zero);
            }
            else if (!IsChecked && m_Inactive != null)
            {
                spriteBatch.Draw2D(m_Inactive, new Vector3(position.X, position.Y, 0), Vector3.Zero);
            }
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            IsChecked = !IsChecked;
        }

        void BuildGumpling(int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
        {
            IResourceProvider provider = Services.Get<IResourceProvider>();
            m_Inactive = provider.GetUITexture(inactiveID);
            m_Active = provider.GetUITexture(activeID);

            Position = new Point(x, y);
            Size = new Point(m_Inactive.Width, m_Inactive.Height);
            IsChecked = initialState;
            GumpLocalID = switchID;
        }
    }
}