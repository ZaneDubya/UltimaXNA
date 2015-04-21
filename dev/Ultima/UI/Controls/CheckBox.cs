using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltimaXNA.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimaXNA.Ultima.UI.Controls
{
    class CheckBox : AControl
    {
        private Texture2D m_Inactive, m_Active;

        public bool IsChecked
        {
            get;
            protected set;
        }

        public CheckBox(AControl owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
        }

        public CheckBox(AControl owner, int page, string[] arguements, string[] lines)
            : this(owner, page)
        {
            int x, y, inactiveID, activeID, switchID;
            bool initialState;

            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            inactiveID = Int32.Parse(arguements[3]);
            activeID = Int32.Parse(arguements[4]);
            initialState = Int32.Parse(arguements[5]) == 1;
            switchID = Int32.Parse(arguements[6]);

            MemberBuildGumpling(x, y, inactiveID, activeID, initialState, switchID);
        }

        public CheckBox(AControl owner, int page, int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
            : this(owner, page)
        {
            MemberBuildGumpling(x, y, inactiveID, activeID, initialState, switchID);
        }

        void MemberBuildGumpling(int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
        {
            m_Inactive = IO.GumpData.GetGumpXNA(inactiveID);
            m_Active = IO.GumpData.GetGumpXNA(activeID);

            Position = new Point(x, y);
            Size = new Point(m_Inactive.Width, m_Inactive.Height);
            IsChecked = initialState;
            Serial = switchID;
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            base.Draw(spriteBatch);
            if (IsChecked && m_Active != null)
            {
                spriteBatch.Draw2D(m_Active, new Vector3(X, Y, 0), Vector3.Zero);
            }
            else if (!IsChecked && m_Inactive != null)
            {
                spriteBatch.Draw2D(m_Inactive, new Vector3(X, Y, 0), Vector3.Zero);
            }
        }

        protected override void OnMouseClick(int x, int y, Core.Input.Windows.MouseButton button)
        {
            IsChecked = !IsChecked;
        }
    }
}
