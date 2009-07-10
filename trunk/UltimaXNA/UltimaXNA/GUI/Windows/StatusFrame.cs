#region File Description & Usings
//-----------------------------------------------------------------------------
// GUIWindow_Chat.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;
#endregion

namespace UltimaXNA.GUI
{
    class Window_StatusFrame : Window
    {
        public string CharacterName
        {
            set
            {
                ((Label)_MyForm["lblCharName"]).Text = value;
            }
        }

        private int m_CurrentHealth, m_MaxHealth;
        public int CurrentHealth { set { m_CurrentHealth = value; m_UpdateBar(0); } }
        public int MaxHealth { set { m_MaxHealth = value; m_UpdateBar(0); } }

        private int m_CurrentMana, m_MaxMana;
        public int CurrentMana { set { m_CurrentMana = value; m_UpdateBar(1); } }
        public int MaxMana { set { m_MaxMana = value; m_UpdateBar(1); } }

        private int m_CurrentStamina, m_MaxStamina;
        public int CurrentStamina { set { m_CurrentStamina = value; m_UpdateBar(2); } }
        public int MaxStamina { set { m_MaxStamina = value; m_UpdateBar(2); } }

        public Window_StatusFrame(FormCollection nFormCollection)
            : base(nFormCollection)
        {
            //Create a new form
            m_FormCollection.Add(new Form("frmStatusFrameMain", "", new Vector2(128, 64), new Vector2(0, 0), Form.BorderStyle.None));
            _MyForm = m_FormCollection["frmStatusFrameMain"];
            _MyForm.BorderName = null;

            _MyForm.Controls.Add(new PictureBox("picBG", new Vector2(0,0), @"GUI\STATFRAME\UI-STATFRAME-MAINNEW.png", 128, 64, 0));
            _MyForm.Controls.Add(new Label("lblCharName", new Vector2(4, -1), "Poplicola", Color.TransparentBlack, Color.White, 100, Label.Align.Left));
            _MyForm["lblCharName"].FontName = "Pericles9";

            _MyForm.Controls.Add(new PictureBox("barStat0",   new Vector2(0, 16), @"GUI\STATFRAME\UI-STATFRAME-MAINNEW-HITS.png", 128, 16, 0));
            _MyForm.Controls.Add(new PictureBox("barStat1", new Vector2(0, 29), @"GUI\STATFRAME\UI-StatFrame-MAINNEW-MANA.png", 128, 16, 0));
            _MyForm.Controls.Add(new PictureBox("barStat2", new Vector2(0, 42), @"GUI\STATFRAME\UI-StatFrame-MAINNEW-STAM.png", 128, 16, 0));



            _MyForm.Controls.Add(new Label("lblStat%0", new Vector2(85, 15), string.Empty, Color.TransparentBlack, Color.White, 100, Label.Align.Left));
            _MyForm.Controls.Add(new Label("lblStat%1", new Vector2(85, 28), string.Empty, Color.TransparentBlack, Color.White, 100, Label.Align.Left));
            _MyForm.Controls.Add(new Label("lblStat%2", new Vector2(85, 41), string.Empty, Color.TransparentBlack, Color.White, 100, Label.Align.Left));
            _MyForm.Controls.Add(new Label("lblStatAmt0", new Vector2(4, 15), string.Empty, Color.TransparentBlack, Color.White, 100, Label.Align.Left));
            _MyForm.Controls.Add(new Label("lblStatAmt1", new Vector2(4, 28), string.Empty, Color.TransparentBlack, Color.White, 100, Label.Align.Left));
            _MyForm.Controls.Add(new Label("lblStatAmt2", new Vector2(4, 41), string.Empty, Color.TransparentBlack, Color.White, 100, Label.Align.Left));
            for (int i = 0; i < 3; i++)
            {
                _MyForm["lblStat%" + i].FontName = "MiramontBold7";
                _MyForm["lblStatAmt" + i].FontName = "MiramontBold7";
            }
            
            //Show the form
            this.Show();
        }

        public override void Update()
        {
            CharacterName = "Poplicola";
        }

        private void m_UpdateBar(int nBarIndex)
        {
            const int iMaxWidth = 128 ;
            int iActualWidth;
            int iCurrent, iMax;
            switch (nBarIndex)
            {
                case 0:
                    iCurrent = m_CurrentHealth;
                    iMax = m_MaxHealth;
                    break;
                case 1:
                    iCurrent = m_CurrentMana;
                    iMax = m_MaxMana;
                    break;
                case 2:
                    iCurrent = m_CurrentStamina;
                    iMax = m_MaxStamina;
                    break;
                default:
                    throw (new Exception("Bar index should be 0-2"));
            }

            if (iMax == 0)
            {
                iActualWidth = iMaxWidth;
            }
            else if (iCurrent == 0)
            {
                iActualWidth = 0;
            }
            else
            {
                iActualWidth = (int)((float)iMaxWidth * ((float)iCurrent / (float)iMax));
            }

            _MyForm["barStat" + nBarIndex].Width = iActualWidth;
            _MyForm["lblStat%" + nBarIndex].Text = ((int)(((float)iCurrent / (float)iMax) * 100f)).ToString() + "%";
            _MyForm["lblStatAmt" + nBarIndex].Text = iCurrent.ToString();
        }
    }
}
