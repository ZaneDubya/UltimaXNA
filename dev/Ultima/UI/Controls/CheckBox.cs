using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.IO;

namespace UltimaXNA.Ultima.UI.Controls
{
    /// <summary>
    /// A checkbox control.
    /// </summary>
    class CheckBox : AControl
    {
        private Texture2D m_Inactive, m_Active;

        public bool IsChecked
        {
            get;
            protected set;
        }

        public CheckBox(AControl owner)
            : base(owner)
        {
            HandlesMouseInput = true;
        }

        public CheckBox(AControl owner, string[] arguements, string[] lines)
            : this(owner)
        {
            int x, y, inactiveID, activeID, switchID;
            bool initialState;

            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            inactiveID = Int32.Parse(arguements[3]);
            activeID = Int32.Parse(arguements[4]);
            initialState = Int32.Parse(arguements[5]) == 1;
            switchID = Int32.Parse(arguements[6]);

            buildGumpling(x, y, inactiveID, activeID, initialState, switchID);
        }

        public CheckBox(AControl owner, int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
            : this(owner)
        {
            buildGumpling(x, y, inactiveID, activeID, initialState, switchID);
        }

        void buildGumpling(int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
        {
            m_Inactive = GumpData.GetGumpXNA(inactiveID);
            m_Active = GumpData.GetGumpXNA(activeID);

            Position = new Point(x, y);
            Size = new Point(m_Inactive.Width, m_Inactive.Height);
            IsChecked = initialState;
            GumpLocalID = switchID;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            base.Draw(spriteBatch, position);
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
    }
}
