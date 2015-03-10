/***************************************************************************
 *   Button.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Rendering;
using InterXLib.Input.Windows;
using UltimaXNA.UltimaGUI;

namespace UltimaXNA.UltimaGUI.Controls
{
    public enum ButtonTypes
    {
        Default = 0,
        SwitchPage = 0,
        Activate = 1
    }

    public class Button : Control
    {
        private const int kGump_Up = 0, kGump_Down = 1, kGump_Over = 2;
        Texture2D[] m_gumpTextures = new Texture2D[3] { null, null, null };
        int[] m_gumpID = new int[3] { 0, 0, 0 }; // 0 == up, 1 == down, 2 == additional over state, not sent by the server but can be added for clientside gumps.

        public int GumpUpID
        {
            set
            {
                m_gumpID[kGump_Up] = value;
                m_gumpTextures[kGump_Up] = null;
            }
        }

        public int GumpDownID
        {
            set
            {
                m_gumpID[kGump_Down] = value;
                m_gumpTextures[kGump_Down] = null;
            }
        }

        public int GumpOverID
        {
            set
            {
                m_gumpID[kGump_Over] = value;
                m_gumpTextures[kGump_Over] = null;
            }
        }

        public ButtonTypes ButtonType = ButtonTypes.Default;
        public int ButtonParameter = 0;
        public int ButtonID = 0;
        public string Caption = string.Empty;
        public bool DoDrawBounds = false;

        internal bool MouseDownOnThis { get { return (m_clicked); } }

        UltimaGUI.TextRenderer m_textRenderer;

        public Button(Control owner, int page)
            : base(owner, page)
        {
            HandlesMouseInput = true;
        }

        public Button(Control owner, int page, string[] arguements)
            : this(owner, page)
        {
            int x, y, gumpID1, gumpID2, buttonType, param, buttonID;
            x = Int32.Parse(arguements[1]);
            y = Int32.Parse(arguements[2]);
            gumpID1 = Int32.Parse(arguements[3]);
            gumpID2 = Int32.Parse(arguements[4]);
            buttonType = Int32.Parse(arguements[5]);
            param = Int32.Parse(arguements[6]);
            buttonID = Int32.Parse(arguements[7]);
            buildGumpling(x, y, gumpID1, gumpID2, (ButtonTypes)buttonType, param, buttonID);
        }

        public Button(Control owner, int page, int x, int y, int gumpID1, int gumpID2, ButtonTypes buttonType, int param, int buttonID)
            : this(owner, page)
        {
            buildGumpling(x, y, gumpID1, gumpID2, buttonType, param, buttonID);
        }

        void buildGumpling(int x, int y, int gumpID1, int gumpID2, ButtonTypes buttonType, int param, int buttonID)
        {
            Position = new Point(x, y);
            GumpUpID = gumpID1;
            ButtonType = buttonType;
            ButtonParameter = param;
            ButtonID = buttonID;
            m_textRenderer = new UltimaGUI.TextRenderer("", 100, true);
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = kGump_Up; i <= kGump_Over; i++)
            {
                if (m_gumpID[i] != 0 && m_gumpTextures[i] == null)
                {
                    m_gumpTextures[i] = UltimaData.GumpData.GetGumpXNA(m_gumpID[i]);
                }
            }

            if (Width == 0 && Height == 0 && m_gumpTextures[kGump_Up] != null)
                Size = new Point(m_gumpTextures[kGump_Up].Width, m_gumpTextures[kGump_Up].Height);

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            Texture2D texture = getTextureFromMouseState();

            if (Caption != string.Empty)
                m_textRenderer.Text = Caption;

            spriteBatch.Draw2D(texture, new Rectangle(X, Y, Width, Height), 0, false, false);
            if (DoDrawBounds)
                DrawBounds(spriteBatch, Color.Black);

            if (Caption != string.Empty)
            {
                int yoffset = MouseDownOnThis ? 1 : 0;
                m_textRenderer.Draw(spriteBatch, 
                    new Point(X + (Width - m_textRenderer.Width) / 2,
                        Y + yoffset + (Height - m_textRenderer.Height) / 2));
            }
            base.Draw(spriteBatch);
        }

        private Texture2D getTextureFromMouseState()
        {
            if (MouseDownOnThis && m_gumpTextures[kGump_Down] != null)
                return m_gumpTextures[kGump_Down];
            else if (UserInterface.MouseOverControl == this && m_gumpTextures[kGump_Over] != null)
                return m_gumpTextures[kGump_Over];
            else
                return m_gumpTextures[kGump_Up];
        }

        protected override bool m_hitTest(int x, int y)
        {
            Color[] pixelData;
            pixelData = new Color[1];
            getTextureFromMouseState().GetData<Color>(0, new Rectangle(x, y, 1, 1), pixelData, 0, 1);
            if (pixelData[0].A > 0)
                return true;
            else
                return false;
        }

        bool m_clicked = false;

        protected override void mouseDown(int x, int y, MouseButton button)
        {
            m_clicked = true;
        }

        protected override void mouseUp(int x, int y, MouseButton button)
        {
            m_clicked = false;
        }

        protected override void mouseClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                switch (this.ButtonType)
                {
                    case ButtonTypes.SwitchPage:
                        // switch page
                        ChangePage(this.ButtonParameter);
                        break;
                    case ButtonTypes.Activate:
                        // send response
                        ActivateByButton(this.ButtonID);
                        break;
                }
            }
        }
    }
}
