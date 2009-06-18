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
                ((Label)m_MyForm["lblCharName"]).Text = value;
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
            m_FormCollection.Add(new Form("frmStatusFrameMain", "", new Vector2(128, 128), new Vector2(0, 0), Form.BorderStyle.None));
            m_MyForm = m_FormCollection["frmStatusFrameMain"];
            m_MyForm.BorderName = null;

            m_MyForm.Controls.Add(new PictureBox("picBG", new Vector2(0,0), @"GUI\STATFRAME\UI-STATFRAME-MAIN.png", 128, 128, 0));

            m_MyForm.Controls.Add(new PictureBox("barHealth", new Vector2(2, 22), @"GUI\STATFRAME\UI-StatFrame-Main-Bar.png", 128, 8, 0));
            m_MyForm.Controls.Add(new PictureBox("barMana", new Vector2(2, 33), @"GUI\STATFRAME\UI-StatFrame-Main-Bar.png", 128, 8, 0));
            m_MyForm.Controls.Add(new PictureBox("barStamina", new Vector2(2, 44), @"GUI\STATFRAME\UI-StatFrame-Main-Bar.png", 128, 8, 0));

            m_MyForm["barHealth"].BackColor = Color.Red;
            m_MyForm["barMana"].BackColor = Color.Blue;
            m_MyForm["barStamina"].BackColor = Color.Yellow;

            m_MyForm.Controls.Add(new Label("lblCharName", new Vector2(2, 3), "Poplicola", Color.TransparentBlack, Color.White, 126, Label.Align.Center));

            //Show the form
            this.Show();
        }

        public override void Update()
        {
            CharacterName = "Poplicola";
        }

        private void m_UpdateBar(int nBarIndex)
        {
            const int iMaxWidth = 117;
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

            switch (nBarIndex)
            {
                case 0:
                    m_MyForm["barHealth"].Width = iActualWidth;
                    break;
                case 1:
                    m_MyForm["barMana"].Width = iActualWidth;
                    break;
                case 2:
                    m_MyForm["barStamina"].Width = iActualWidth;
                    break;
                default:
                    throw (new Exception("Bar index should be 0-2"));
            }
        }
    }
}
