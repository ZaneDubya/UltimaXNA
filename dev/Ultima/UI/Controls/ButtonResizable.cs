/***************************************************************************
 *   ButtonResizable.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using System;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls {
    public class ButtonResizable : AControl {
        const int GumpUp = 9400, GumpDown = 9500, GumpOver = 9450;
        ResizePic[] m_Gumps = new ResizePic[3];
        bool m_IsMouseDown;
        RenderedText m_Caption;
        Action m_OnClickRight;
        readonly Action m_OnClickLeft;

        internal bool IsMouseDownOnThis => m_IsMouseDown;

        public string Caption {
            get {
                return m_Caption.Text;
            }
            set {
                m_Caption.Text = $"<outline><span style='font-family: uni1;' color='#ddd'>{value}";
            }
        }

        public ButtonResizable(AControl parent, int x, int y, int width, int height, string caption, Action onClickLeft = null, Action onClickRight = null)
            : base(parent) {
            HandlesMouseInput = true;
            Position = new Point(x, y);
            Size = new Point(width, height);
            m_Caption = new RenderedText(string.Empty, width, true);
            Caption = caption;
            m_Gumps[0] = AddControl(new ResizePic(null, 0, 0, GumpUp, Width, Height), 1) as ResizePic;
            m_Gumps[1] = AddControl(new ResizePic(null, 0, 0, GumpDown, Width, Height), 2) as ResizePic;
            m_Gumps[2] = AddControl(new ResizePic(null, 0, 0, GumpOver, Width, Height), 3) as ResizePic;
            m_OnClickLeft = onClickLeft;
            m_OnClickRight = onClickRight;
        }

        public override void Update(double totalMS, double frameMS) {
            if (IsMouseDownOnThis)
                ActivePage = 2;
            else if (IsMouseOver)
                ActivePage = 3;
            else
                ActivePage = 1;
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS) {
            base.Draw(spriteBatch, position, frameMS);
            if (Caption != string.Empty) {
                int yoffset = IsMouseDownOnThis ? 2 : 1;
                m_Caption.Draw(spriteBatch, new Point(
                    position.X + (Width - m_Caption.Width) / 2,
                    position.Y + yoffset + (Height - m_Caption.Height) / 2));
            }
        }

        protected override bool IsPointWithinControl(int x, int y) {
            return true;
        }

        protected override void OnMouseDown(int x, int y, MouseButton button) {
            if (button == MouseButton.Left) {
                m_IsMouseDown = true;
            }
        }

        protected override void OnMouseUp(int x, int y, MouseButton button) {
            if (button == MouseButton.Left) {
                m_IsMouseDown = false;
            }
        }

        protected override void OnMouseClick(int x, int y, MouseButton button) {
            switch (button) {
                case MouseButton.Left:
                    m_OnClickLeft?.Invoke();
                    break;
                case MouseButton.Right:
                    m_OnClickRight?.Invoke();
                    break;
            }
        }
    }
}