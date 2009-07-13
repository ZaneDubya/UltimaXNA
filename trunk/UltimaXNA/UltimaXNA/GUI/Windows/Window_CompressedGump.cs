using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;

namespace UltimaXNA.GUI
{
    class Window_CompressedGump : Window
    {
        int _TextIndex = 0;
        string[] _gumpText;

        public Window_CompressedGump(Serial gumpID, string[] gumpPieces, string[] gumpText, int x, int y)
            : base()
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmCompressedGump:" + gumpID, string.Empty, new Vector2(1, 1), new Vector2(x, y), Form.BorderStyle.None));
            _MyForm = m_FormCollection["frmCompressedGump:" + gumpID];
            _MyForm.BorderName = null;

            _gumpText = gumpText;
            buildGump(gumpPieces);

            //Show the form
            this.Show();
        }

        private void OnPress(object obj, EventArgs e)
        {
            this.Close();
        }

        private void Button_OnPress(object obj, EventArgs e)
        {

        }

        private void buildGump(string[] gumpPieces)
        {
            for (int i = 0; i < gumpPieces.Length; i++)
            {
                string[] arguements = gumpPieces[i].Split(' ');
                switch (arguements[0])
                {
                    case "checkertrans":
                        interpret_checkertrans(arguements);
                        break;
                    case "resizepic":
                        interpret_resizepic(arguements);
                        break;
                    case "button":
                        interpret_button(arguements);
                        break;
                    case "checkbox":
                        interpret_checkbox(arguements);
                        break;
                    case "group":
                        interpret_group(arguements);
                        break;
                    case "gumphtml":
                        interpret_gumphtml(arguements);
                        break;
                    case "xmfhtmlgump" :
                        interpret_xmfhtmlgump(arguements);
                        break;
                    case "xmfhtmlgumpcolor":
                        interpret_xmfhtmlgumpcolor(arguements);
                        break;
                    case "xmfhtmltok":
                        interpret_xmfhtmltok(arguements);
                        break;
                    case "gumppic":
                        interpret_gumppic(arguements);
                        break;
                    case "buttontileart":
                        interpret_buttontileart(arguements);
                        break;
                    case "tooltip":
                        interpret_tooltip(arguements);
                        break;
                    case "gumppictiled":
                        interpret_gumppictiled(arguements);
                        break;
                    case "tilepic":
                        interpret_tilepic(arguements);
                        break;
                    case "tilepichue":
                        interpret_tilepichue(arguements);
                        break;
                    case "text":
                        interpret_text(arguements);
                        break;
                    case "croppedtext":
                        interpret_croppedtext(arguements);
                        break;
                    case "radio":
                        interpret_radio(arguements);
                        break;
                    case "textentry":
                        interpret_textentry(arguements);
                        break;
                    case "textentrylimited":
                        interpret_textentrylimited(arguements);
                        break;
                    case "page":
                        interpret_page(arguements);
                        break;
                    default:
                        GUIHelper.Chat_AddLine("GUMP: Unknown piece '" + arguements[0] + "'.");
                        break;
                }

            }
        }

        private void interpret_button(string[] arguements)
        {
            int m_X, m_Y, m_ID1, m_ID2, m_Type, m_Param, m_ButtonID;
            m_X = Int32.Parse(arguements[1]);
            m_Y = Int32.Parse(arguements[2]);
            m_ID1 = Int32.Parse(arguements[3]);
            m_ID2 = Int32.Parse(arguements[4]);
            m_Type = Int32.Parse(arguements[5]);
            m_Param = Int32.Parse(arguements[6]);
            m_ButtonID = Int32.Parse(arguements[7]);

            Texture2D upTexture = Data.Gumps.GetGumpXNA(m_ID1);
            Texture2D downTexture = Data.Gumps.GetGumpXNA(m_ID2);

            Controls.Add(new CustomButton("btn" + m_ButtonID + ":" + m_Param, new Vector2(m_X, m_Y), new Rectangle(0, 0, upTexture.Width, upTexture.Height),
                null, null, null, null));
            ((CustomButton)Controls["btn" + m_ButtonID + ":" + m_Param]).TextureUp = upTexture;
            ((CustomButton)Controls["btn" + m_ButtonID + ":" + m_Param]).TextureDown = downTexture;
            Controls["btn" + m_ButtonID + ":" + m_Param].OnPress += Button_OnPress;
        }

        private void interpret_buttontileart(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_checkbox(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_checkertrans(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_croppedtext(string[] arguements)
        {
            int m_X, m_Y, m_Width, m_Height, m_Hue, m_Parent;
            m_X = Int32.Parse(arguements[1]);
            m_Y = Int32.Parse(arguements[2]);
            m_Width = Int32.Parse(arguements[3]);
            m_Height = Int32.Parse(arguements[4]);
            m_Hue = Int32.Parse(arguements[5]);
            m_Parent = Int32.Parse(arguements[6]);
            Controls.Add(new Label("lbl" + (Controls.Count + 1), new Vector2(m_X, m_Y), string.Empty, Color.TransparentBlack, Color.Black, m_Width, Label.Align.Left));
            Controls["lbl" + Controls.Count].Text = _gumpText[m_Parent];
            checkResize(m_X, m_Y, m_Width, m_Height);
        }

        private void interpret_group(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_gumppic(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_gumppictiled(string[] arguements)
        {
            int m_X, m_Y, m_GumpID, m_Width, m_Height;
            m_X = Int32.Parse(arguements[1]);
            m_Y = Int32.Parse(arguements[2]);
            m_Width = Int32.Parse(arguements[3]);
            m_Height = Int32.Parse(arguements[4]);
            m_GumpID = Int32.Parse(arguements[5]);
            Controls.Add(new PictureBox("pic" + (Controls.Count + 1), new Vector2(m_X, m_Y), string.Empty, m_Width, m_Height, 0));
            ((PictureBox)Controls["pic" + Controls.Count]).Texture = Data.Gumps.GetGumpXNA(m_GumpID);
            checkResize(m_X, m_Y, m_Width, m_Height);
        }

        private void interpret_gumphtml(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_page(string[] arguements)
        {
            if (arguements[1] != "0")
                throw new Exception("Unknown page #");
            return;
        }

        private void interpret_radio(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_resizepic(string[] arguements)
        {
            int m_X, m_Y, m_GumpID, m_Width, m_Height;
            m_X = Int32.Parse(arguements[1]);
            m_Y = Int32.Parse(arguements[2]);
            m_GumpID = Int32.Parse(arguements[3]);
            m_Width = Int32.Parse(arguements[4]);
            m_Height = Int32.Parse(arguements[5]);

            Texture2D[] iGumps = new Texture2D[9];
            for (int i = 0; i < 9; i++)
            {
                iGumps[i] = Data.Gumps.GetGumpXNA(m_GumpID + i);
            }

            Controls.Add(new PictureBox("pic" + (Controls.Count + 1), new Vector2(m_X, m_Y), string.Empty, iGumps[0].Width, iGumps[0].Height, 0));
            ((PictureBox)Controls["pic" + Controls.Count]).Texture = iGumps[0];
            Controls.Add(new PictureBox("pic" + (Controls.Count + 1), new Vector2(m_X + iGumps[0].Width, m_Y), string.Empty, m_Width - (iGumps[0].Width + iGumps[2].Width), iGumps[1].Height, 0));
            ((PictureBox)Controls["pic" + Controls.Count]).Texture = iGumps[1];
            Controls.Add(new PictureBox("pic" + (Controls.Count + 1), new Vector2(m_Width - iGumps[2].Width, m_Y), string.Empty, iGumps[2].Width, iGumps[2].Height, 0));
            ((PictureBox)Controls["pic" + Controls.Count]).Texture = iGumps[2];

            Controls.Add(new PictureBox("pic" + (Controls.Count + 1), new Vector2(m_X, m_Y + iGumps[0].Height), string.Empty, iGumps[3].Width, m_Height - (iGumps[0].Height + iGumps[6].Height), 0));
            ((PictureBox)Controls["pic" + Controls.Count]).Texture = iGumps[3];
            Controls.Add(new PictureBox("pic" + (Controls.Count + 1), new Vector2(m_X + iGumps[3].Width, m_Y + iGumps[1].Height), string.Empty, m_Width - (iGumps[3].Width + iGumps[5].Width), m_Height - (iGumps[1].Height + iGumps[7].Height), 0));
            ((PictureBox)Controls["pic" + Controls.Count]).Texture = iGumps[4];
            Controls.Add(new PictureBox("pic" + (Controls.Count + 1), new Vector2(m_Width - iGumps[2].Width, m_Y + iGumps[2].Height), string.Empty, iGumps[5].Width, m_Height - (iGumps[2].Height + iGumps[8].Height), 0));
            ((PictureBox)Controls["pic" + Controls.Count]).Texture = iGumps[5];

            Controls.Add(new PictureBox("pic" + (Controls.Count + 1), new Vector2(m_X, m_Height - iGumps[6].Height), string.Empty, iGumps[6].Width, iGumps[6].Height, 0));
            ((PictureBox)Controls["pic" + Controls.Count]).Texture = iGumps[6];
            Controls.Add(new PictureBox("pic" + (Controls.Count + 1), new Vector2(m_X + iGumps[6].Width, m_Height - iGumps[7].Height), string.Empty, m_Width - (iGumps[6].Width + iGumps[8].Width), iGumps[7].Height, 0));
            ((PictureBox)Controls["pic" + Controls.Count]).Texture = iGumps[7];
            Controls.Add(new PictureBox("pic" + (Controls.Count + 1), new Vector2(m_Width - iGumps[8].Width, m_Height - iGumps[8].Height), string.Empty, iGumps[8].Width, iGumps[8].Height, 0));
            ((PictureBox)Controls["pic" + Controls.Count]).Texture = iGumps[8];

            for (int i = 0; i < 9; i++)
            {
                Controls["pic" + (Controls.Count - i)].OnPress += OnPress;
            }

            

            checkResize(m_X, m_Y, m_Width, m_Height);
        }

        private void interpret_text(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_textentry(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_textentrylimited(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_tilepic(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_tilepichue(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_tooltip(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_xmfhtmlgump(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_xmfhtmlgumpcolor(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void interpret_xmfhtmltok(string[] arguements)
        {
            GUIHelper.Chat_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
        }

        private void checkResize(int x, int y, int width, int height)
        {
            if (_MyForm.Width < x + width)
                _MyForm.Width = x + width;
            if (_MyForm.Height < y + height)
                _MyForm.Height = y + height;
        }
    }
}
