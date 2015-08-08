/***************************************************************************
 *   Button.cs
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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Resources;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.Resources;

namespace UltimaXNA.Ultima.UI.Controls
{
    public enum ButtonTypes
    {
        Default = 0,
        SwitchPage = 0,
        Activate = 1
    }

    public class Button : AControl
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

        internal bool MouseDownOnThis { get { return (m_clicked); } }

        RenderedText m_Texture;

        UserInterfaceService m_UserInterface;

        public Button(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;

            m_UserInterface = ServiceRegistry.GetService<UserInterfaceService>();
        }

        public Button(AControl parent, string[] arguements)
            : this(parent)
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

        public Button(AControl parent, int x, int y, int gumpID1, int gumpID2, ButtonTypes buttonType, int param, int buttonID)
            : this(parent)
        {
            buildGumpling(x, y, gumpID1, gumpID2, buttonType, param, buttonID);
        }

        void buildGumpling(int x, int y, int gumpID1, int gumpID2, ButtonTypes buttonType, int param, int buttonID)
        {
            Position = new Point(x, y);
            GumpUpID = gumpID1;
            GumpDownID = gumpID2;
            ButtonType = buttonType;
            ButtonParameter = param;
            ButtonID = buttonID;
            m_Texture = new RenderedText(string.Empty, 100);
        }

        public override void Update(double totalMS, double frameMS)
        {
            for (int i = kGump_Up; i <= kGump_Over; i++)
            {
                if (m_gumpID[i] != 0 && m_gumpTextures[i] == null)
                {
                    IResourceProvider provider = ServiceRegistry.GetService<IResourceProvider>();
                    m_gumpTextures[i] = provider.GetUITexture(m_gumpID[i]);
                }
            }

            if (Width == 0 && Height == 0 && m_gumpTextures[kGump_Up] != null)
                Size = new Point(m_gumpTextures[kGump_Up].Width, m_gumpTextures[kGump_Up].Height);

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position)
        {
            Texture2D texture = getTextureFromMouseState();

            if (Caption != string.Empty)
                m_Texture.Text = Caption;

            spriteBatch.Draw2D(texture, new Rectangle(position.X, position.Y, Width, Height), Vector3.Zero);

            if (Caption != string.Empty)
            {
                int yoffset = MouseDownOnThis ? 1 : 0;
                m_Texture.Draw(spriteBatch,
                    new Point(position.X + (Width - m_Texture.Width) / 2,
                        position.Y + yoffset + (Height - m_Texture.Height) / 2));
            }
            base.Draw(spriteBatch, position);
        }

        private Texture2D getTextureFromMouseState()
        {
            if (MouseDownOnThis && m_gumpTextures[kGump_Down] != null)
                return m_gumpTextures[kGump_Down];
            else if (m_UserInterface.MouseOverControl == this && m_gumpTextures[kGump_Over] != null)
                return m_gumpTextures[kGump_Over];
            else
                return m_gumpTextures[kGump_Up];
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            ushort[] pixelData;
            pixelData = new ushort[1];
            getTextureFromMouseState().GetData<ushort>(0, new Rectangle(x, y, 1, 1), pixelData, 0, 1);
            if (pixelData[0] > 0)
                return true;
            else
                return false;
        }

        bool m_clicked = false;

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                m_clicked = true;
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                m_clicked = false;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                switch (ButtonType)
                {
                    case ButtonTypes.SwitchPage:
                        // switch page
                        ChangePage(ButtonParameter);
                        break;
                    case ButtonTypes.Activate:
                        // send response
                        ActivateByButton(ButtonID);
                        break;
                }
            }
        }
    }
}
