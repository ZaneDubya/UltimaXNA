/***************************************************************************
 *   Button.cs
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;

namespace UltimaXNA.Ultima.UI.Controls {
    public enum ButtonTypes {
        Default = 0,
        SwitchPage = 0,
        Activate = 1,
    }

    public class Button : AControl {
        const int Gump_Up = 0, Gump_Down = 1, Gump_Over = 2;

        Texture2D[] m_GumpTextures = { null, null, null };
        int[] m_GumpID = { 0, 0, 0 }; // 0 == up, 1 == down, 2 == additional over state, not sent by the server but can be added for clientside gumps.
        RenderedText m_Texture;

        public int GumpUpID {
            set {
                m_GumpID[Gump_Up] = value;
                m_GumpTextures[Gump_Up] = null;
            }
        }

        public int GumpDownID {
            set {
                m_GumpID[Gump_Down] = value;
                m_GumpTextures[Gump_Down] = null;
            }
        }

        public int GumpOverID {
            set {
                m_GumpID[Gump_Over] = value;
                m_GumpTextures[Gump_Over] = null;
            }
        }

        public ButtonTypes ButtonType = ButtonTypes.Default;
        public int ButtonParameter;
        public int ButtonID;
        public string Caption = string.Empty;

        public bool MouseDownOnThis => m_clicked;

        Button(AControl parent)
            : base(parent) {
            HandlesMouseInput = true;
        }

        public Button(AControl parent, string[] arguements)
            : this(parent) {
            int x, y, gumpID1, gumpID2, buttonType, param, buttonID;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            gumpID1 = Int32.Parse(arguements[3]);
            gumpID2 = Int32.Parse(arguements[4]);
            buttonType = Int32.Parse(arguements[5]);
            param = Int32.Parse(arguements[6]);
            buttonID = 0;
            if (arguements.Length > 7) {
                buttonID = Int32.Parse(arguements[7]);
            }
            BuildGumpling(x, y, gumpID1, gumpID2, (ButtonTypes)buttonType, param, buttonID);
        }

        public Button(AControl parent, int x, int y, int gumpID1, int gumpID2, ButtonTypes buttonType, int param, int buttonID)
            : this(parent) {
            BuildGumpling(x, y, gumpID1, gumpID2, buttonType, param, buttonID);
        }

        void BuildGumpling(int x, int y, int gumpID1, int gumpID2, ButtonTypes buttonType, int param, int buttonID) {
            Position = new Point(x, y);
            GumpUpID = gumpID1;
            GumpDownID = gumpID2;
            ButtonType = buttonType;
            ButtonParameter = param;
            ButtonID = buttonID;
            m_Texture = new RenderedText(string.Empty, 100, true);
        }

        public override void Update(double totalMS, double frameMS) {
            for (int i = Gump_Up; i <= Gump_Over; i++) {
                if (m_GumpID[i] != 0 && m_GumpTextures[i] == null) {
                    IResourceProvider provider = Service.Get<IResourceProvider>();
                    m_GumpTextures[i] = provider.GetUITexture(m_GumpID[i]);
                }
            }
            if (Width == 0 && Height == 0 && m_GumpTextures[Gump_Up] != null) {
                Size = new Point(m_GumpTextures[Gump_Up].Width, m_GumpTextures[Gump_Up].Height);
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS) {
            Texture2D texture = GetTextureFromMouseState();
            if (Caption != string.Empty) {
                m_Texture.Text = Caption;
            }
            spriteBatch.Draw2D(texture, new Rectangle(position.X, position.Y, Width, Height), Vector3.Zero);
            if (Caption != string.Empty) {
                int yoffset = MouseDownOnThis ? 1 : 0;
                m_Texture.Draw(spriteBatch, new Point(
                    position.X + (Width - m_Texture.Width) / 2, 
                    position.Y + yoffset + (Height - m_Texture.Height) / 2));
            }
            base.Draw(spriteBatch, position, frameMS);
        }

        Texture2D GetTextureFromMouseState()
        {
            if (MouseDownOnThis && m_GumpTextures[Gump_Down] != null)
            {
                return m_GumpTextures[Gump_Down];
            }
            if (UserInterface.MouseOverControl == this && m_GumpTextures[Gump_Over] != null)
            {
                return m_GumpTextures[Gump_Over];
            }
            return m_GumpTextures[Gump_Up];
        }

        int GetGumpIDFromMouseState()
        {
            if (MouseDownOnThis && m_GumpTextures[Gump_Down] != null)
            {
                return m_GumpID[Gump_Down];
            }
            if (UserInterface.MouseOverControl == this && m_GumpTextures[Gump_Over] != null)
            {
                return m_GumpID[Gump_Over];
            }
            return m_GumpID[Gump_Up];
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            int gumpID = GetGumpIDFromMouseState();
            IResourceProvider provider = Service.Get<IResourceProvider>();
            return provider.IsPointInUITexture(gumpID, x, y);
        }

        bool m_clicked;

        protected override void OnMouseDown(int x, int y, MouseButton button) {
            if (button == MouseButton.Left) {
                m_clicked = true;
            }
        }

        protected override void OnMouseUp(int x, int y, MouseButton button) {
            if (button == MouseButton.Left) {
                m_clicked = false;
            }
        }

        protected override void OnMouseClick(int x, int y, MouseButton button) {
            if (button == MouseButton.Left) {
                switch (ButtonType) {
                    case ButtonTypes.SwitchPage:
                        // switch page
                        ChangePage(ButtonParameter);
                        break;

                    case ButtonTypes.Activate:
                        // send response
                        OnButtonClick(ButtonID);
                        break;
                }
            }
        }
    }
}