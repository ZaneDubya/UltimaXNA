/***************************************************************************
 *   StatusFrame.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;
using xWinFormsLib;
#endregion

namespace UltimaXNA.UI
{
    class Window_StatusFrame : Window
    {
        Mobile _myEntity;
        Mobile _targetEntity;
        public Mobile MyEntity { set { _myEntity = value; } }
        public Mobile TargetEntity { set { _targetEntity = value; } }

        private int width = 400;

        public Window_StatusFrame()
            : base()
        {
            //Create a new form
            _formCollection.Add(new Form("frmStatusFrameMain", "", new Vector2(400, 75), new Vector2(200, 0), Form.BorderStyle.None));
            _MyForm = _formCollection["frmStatusFrameMain"];
            _MyForm.BorderName = null;

            Controls.Add(new PictureBox("picCenterFrame", new Vector2(width / 2 - 64, 0), @"GUI\STATFRAME\UI-STATFRAME-CENTER.png", 128, 128, 0));
            Controls.Add(new PictureBox("picMiniMap", new Vector2(136, -34), string.Empty, 128, 128, 0));
            // ((PictureBox)Controls["picMiniMap"]).MaskTexture = UIHelper.LoadTexture(@"GUI\STATFRAME\MINIMAP-MASK.png");
            // ((PictureBox)Controls["picMiniMap"]).MyEffect = UIHelper.LoadEffect(@"MaskEffect.fx");

            Controls.Add(new PictureBox("picFrameLeft", new Vector2(13, 0), @"GUI\STATFRAME\UI-STATFRAME-LEFT.png", 256, 64, 0));
            Controls.Add(new PictureBox("picFrameRight", new Vector2(width - 14 - 256, 0), @"GUI\STATFRAME\UI-STATFRAME-RIGHT.png", 256, 64, 0));

            // Controls.Add(new PictureBox("picBG", new Vector2(0,0), @"GUI\STATFRAME\UI-STATFRAME-MAINNEW.png", 128, 64, 0));
            Controls.Add(new Label("lblNameLeft", new Vector2(20, -1), "Me", Color.TransparentBlack, Color.White, 144, Label.Align.Right));
            Controls.Add(new Label("lblNameRight", new Vector2(232, -1), "Target", Color.TransparentBlack, Color.White, 144, Label.Align.Left));
            _MyForm["lblNameLeft"].FontName = "Pericles9";
            _MyForm["lblNameRight"].FontName = "Pericles9";

            Controls.Add(new PictureBox("barStat0Left", new Vector2(58, 16), @"GUI\STATFRAME\UI-STATFRAME-HITS.png", 128, 16, 0));
            Controls.Add(new PictureBox("barStat1Left", new Vector2(58, 29), @"GUI\STATFRAME\UI-STATFRAME-MANA.png", 128, 16, 0));
            Controls.Add(new PictureBox("barStat2Left", new Vector2(58, 42), @"GUI\STATFRAME\UI-STATFRAME-STAMINA.png", 128, 16, 0));

            Controls.Add(new PictureBox("barStat0Right", new Vector2(58 + 170, 16), @"GUI\STATFRAME\UI-STATFRAME-HITS.png", 128, 16, 0));
            Controls.Add(new PictureBox("barStat1Right", new Vector2(58 + 170, 29), @"GUI\STATFRAME\UI-STATFRAME-MANA.png", 128, 16, 0));
            Controls.Add(new PictureBox("barStat2Right", new Vector2(58 + 170, 42), @"GUI\STATFRAME\UI-STATFRAME-STAMINA.png", 128, 16, 0));
            
            for (int i = 0; i < 3; i++)
            {
                string iCtrlName;
                iCtrlName = "lblStat%" + i + "Left";
                Controls.Add(new Label(iCtrlName, new Vector2(30, 15 + i * 13), String.Empty, Color.TransparentBlack, Color.White, 100, Label.Align.Left));
                _MyForm[iCtrlName].FontName = "MiramontBold7";

                iCtrlName = "lblStatAmt" + i + "Left";
                Controls.Add(new Label(iCtrlName, new Vector2(150, 15 + i * 13), String.Empty, Color.TransparentBlack, Color.White, 100, Label.Align.Left));
                _MyForm[iCtrlName].FontName = "MiramontBold7";

                iCtrlName = "lblStat%" + i + "Right";
                Controls.Add(new Label(iCtrlName, new Vector2(344, 15 + i * 13), String.Empty, Color.TransparentBlack, Color.White, 100, Label.Align.Left));
                _MyForm[iCtrlName].FontName = "MiramontBold7";

                iCtrlName = "lblStatAmt" + i + "Right";
                Controls.Add(new Label(iCtrlName, new Vector2(234, 15 + i * 13), String.Empty, Color.TransparentBlack, Color.White, 100, Label.Align.Left));
                _MyForm[iCtrlName].FontName = "MiramontBold7";
            }
            
            //Show the form
            this.Show();
        }

        public override void Update()
        {
            for (int i = 0; i < 6; i++)
            {
                m_UpdateBar(i);
            }
            // ((PictureBox)_MyForm["picMiniMap"]).Texture = UIHelper.MiniMapTexture;
        }

        private void setTargetFrameVisibility(string leftOrRight, bool visible)
        {
            string iCtrlNameAppend = String.Empty;
            if (leftOrRight == "Left")
                iCtrlNameAppend = "Left";
            else if (leftOrRight == "Right")
                iCtrlNameAppend = "Right";
            else
                return;

            _MyForm["picFrame" + iCtrlNameAppend].Visible = visible;
            _MyForm["lblName" + iCtrlNameAppend].Visible = visible;
            for (int i = 0; i < 3; i++)
            {
                _MyForm["barStat" + i + iCtrlNameAppend].Visible = visible;
                _MyForm["lblStat%" + i + iCtrlNameAppend].Visible = visible;
                _MyForm["lblStatAmt" + i + iCtrlNameAppend].Visible = visible;
            }
        }

        private void m_UpdateBar(int nBarIndex)
        {
            const int iMaxWidth = 128;
            int iActualWidth;
            int iCurrent, iMax;
            int iPercent;
            Mobile entity;
            string iControlNameAppend;
            if (nBarIndex > 2)
            {
                entity = _targetEntity;
                nBarIndex -= 3;
                iControlNameAppend = "Right";
            }
            else
            {
                entity = _myEntity;
                iControlNameAppend = "Left";
            }

            if (entity == null)
            {
                setTargetFrameVisibility(iControlNameAppend, false);
                return;
            }
            else
            {
                setTargetFrameVisibility(iControlNameAppend, true);
            }

            switch (nBarIndex)
            {
                case 0:
                    iCurrent = entity.Health.Current;
                    iMax = entity.Health.Max;
                    break;
                case 1:
                    iCurrent = entity.Mana.Current;
                    iMax = entity.Mana.Max;
                    break;
                case 2:
                    iCurrent = entity.Stamina.Current;
                    iMax = entity.Stamina.Max;
                    break;
                default:
                    throw (new Exception("Bar index should be 0-2"));
            }

            if (iMax == 0)
            {
                iActualWidth = 0;
                iPercent = 0;
            }
            else if (iCurrent == 0)
            {
                iActualWidth = 0;
                iPercent = 0;
            }
            else
            {
                iActualWidth = (int)((float)iMaxWidth * ((float)iCurrent / (float)iMax));
                iPercent = ((int)(((float)iCurrent / (float)iMax) * 100f));
            }

            _MyForm["barStat" + nBarIndex + iControlNameAppend].Width = iActualWidth;
            _MyForm["lblStat%" + nBarIndex + iControlNameAppend].Text = iPercent + "%";
            _MyForm["lblStatAmt" + nBarIndex + iControlNameAppend].Text = iCurrent.ToString();
            _MyForm["lblName" + iControlNameAppend].Text = entity.Name;
        }
    }
}
