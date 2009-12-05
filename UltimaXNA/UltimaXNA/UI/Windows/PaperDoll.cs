/***************************************************************************
 *   PaperDoll.cs
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
    class Window_PaperDoll : Window
    {
        private Vector2 mWindowSize = new Vector2(228, 400);
        private Vector2 mBGOffset = new Vector2(0, 0);
        private Mobile mMobileObject;
        private int mLastContainerUpdated = -1;
        private const int m_MaxButtons = 0x18;

        public Serial serial { get { return mMobileObject.Serial; } }

        public Window_PaperDoll(Entity nMobileObject, FormCollection nFormCollection)
            : base()
        {
            mMobileObject = (Mobile)nMobileObject;

            //Create a new form
            string iFormName = "frmPaperDoll:" + mMobileObject.Serial;
            _MyForm = new Form(iFormName, "", mWindowSize, new Vector2(64, 64), Form.BorderStyle.None);
            _MyForm.BorderName = null;
            _MyForm.CustomDragArea = new Rectangle(2, 2, 192, 20);
            //_MyForm.MouseThrough = true;

            Controls.Add(new PictureBox("picBG", mBGOffset, @"GUI\PAPERDOLL\PAPERDOLL-FRAME.png", 256, 512, 0));

            Controls.Add(new Label("lblCaption", new Vector2(8f, 4f), "PaperDoll | " + mMobileObject.Serial,
                Color.TransparentBlack, Color.White, 160, Label.Align.Left));
            _MyForm["lblCaption"].FontName = "ArialNarrow10";

            Controls.Add(new CustomButton("btnClose", new Vector2(193, -4), new Rectangle(6, 7, 19, 18),
                @"GUI\COMMON\UI-Panel-MinimizeButton-Up.png", @"GUI\COMMON\UI-Panel-MinimizeButton-Down.png",
                @"GUI\COMMON\UI-Panel-MinimizeButton-Disabled.png", @"GUI\COMMON\UI-Panel-MinimizeButton-Highlight.png"));
            Controls["btnClose"].OnRelease = btnClose_OnRelease;

            m_CreateEquipButton(0x00); // body
            m_CreateEquipButton(0x01, 52, 315); // Right hand (one handed)
            m_CreateEquipButton(0x02, 132, 315); // Left hand (also set to 'two handed')
            m_CreateEquipButton(0x03, 172, 275); // Footwear
            m_CreateEquipButton(0x04, 172, 195); // Legging
            m_CreateEquipButton(0x05, 12, 275); // Shirt
            m_CreateEquipButton(0x06, 12, 75); // Head
            m_CreateEquipButton(0x07, 172, 115); // Gloves
            m_CreateEquipButton(0x08, 12, 35); // Ring
            m_CreateEquipButton(0x09, 132, 35); // Talisman
            m_CreateEquipButton(0x0A, 12, 115); // neck
            m_CreateEquipButton(0x0B); // hair
            m_CreateEquipButton(0x0C, 172, 155); // belt
            m_CreateEquipButton(0x0D, 12, 235); // chest
            m_CreateEquipButton(0x0E, 172, 35); // bracelet
            // skip 0x0F: unused.
            m_CreateEquipButton(0x10); // facial hair
            m_CreateEquipButton(0x11, 92, 35); // sash
            m_CreateEquipButton(0x12, 52, 35); // earring
            m_CreateEquipButton(0x13, 172, 75); // sleeves
            m_CreateEquipButton(0x14, 12, 195); // back
            // skip 0x15: backpack
            m_CreateEquipButton(0x16, 12, 155); // robe
            m_CreateEquipButton(0x17, 172, 235); // skirt
            // skip 0x18: inner legs (?)

            _formCollection.Add(_MyForm);
            this.Show();
            
        }

        private void m_CreateEquipButton(int nEquipIndex, int nX, int nY)
        {
            m_CreateGumpTexture(nEquipIndex, 20, 85);
            string iBtnName = "btnEquip" + nEquipIndex;
            Vector2 iPosition = new Vector2();
            iPosition.Y = nY;
            iPosition.X = nX;
            Controls.Add(new CustomButton(iBtnName, iPosition, new Rectangle(0, 0, 39, 39),
                null, null, null, null, 1f));
            _MyForm[iBtnName].OnMouseOver += btnEquip_OnOver;
            _MyForm[iBtnName].OnMouseOut += btnEquip_OnOut;
            _MyForm[iBtnName].OnPress += btnEquip_OnPress;
            _MyForm[iBtnName].OnRelease += btnEquip_OnRelease;
        }

        private void m_CreateEquipButton(int nEquipIndex)
        {
            m_CreateGumpTexture(nEquipIndex, 20, 85);
        }

        private void m_CreateGumpTexture(int nEquipIndex, int nX, int nY)
        {
            string iPicName = "picEquip" + nEquipIndex;
            Vector2 iPosition = new Vector2();
            iPosition.Y = nY;
            iPosition.X = nX;
            Controls.Add(new PictureBox(iPicName, iPosition, string.Empty, 0));
        }

        private void btnEquip_OnPress(object obj, EventArgs e)
        {
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(8));
            Item iItem = mMobileObject.equipment[iIndex];
            if (iItem != null)
            {
                // pick the item up!
                UIHelper.PickUpItem(iItem);
            }
        }

        private void btnEquip_OnRelease(object obj, EventArgs e)
        {
            if (UIHelper.MouseHoldingItem != null)
            {
                int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(8));
                UIHelper.WearItem(mMobileObject, iIndex);
            }
        }

        private void btnEquip_OnOver(object obj, EventArgs e)
        {
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(8));
            Item iItem = mMobileObject.equipment[iIndex];

            if (UIHelper.MouseHoldingItem != null)
            {
                ((CustomButton)obj).RespondToAllReleaseEvents = true;
            }
            else
            {
                UIHelper.ToolTipItem = iItem;
                UIHelper.TooltipX = (int)_MyForm.X + (int)((CustomButton)obj).X + 42;
                UIHelper.TooltipY = (int)_MyForm.Y + (int)((CustomButton)obj).Y;
            }
        }
        private void btnEquip_OnOut(object obj, EventArgs e)
        {
            if (UIHelper.MouseHoldingItem != null)
            {
                ((CustomButton)obj).RespondToAllReleaseEvents = false;
            }

            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(8));
            Item iItem = mMobileObject.equipment[iIndex];
            if (UIHelper.ToolTipItem == iItem)
            {
                UIHelper.ToolTipItem = null;
            }
        }

        private void btnClose_OnRelease(object obj, EventArgs e)
        {
            Close();
        }

        public override void Update()
        {
            base.Update();

            if (this.IsClosed)
                return;

            if (mMobileObject.equipment.UpdateTicker != mLastContainerUpdated)
            {

                ((PictureBox)_MyForm["picEquip0"]).Texture = Data.Gumps.GetGumpXNA(0x000C, mMobileObject.Hue, true);

				bool hasOuterTorso = mMobileObject.equipment[(int)EquipLayer.OuterTorso] != null && mMobileObject.equipment[(int)EquipLayer.OuterTorso].AnimationDisplayID != 0;
				// if there is something we need to make sure there is no gump texture left for the items underneath
				if (hasOuterTorso)
				{
					((PictureBox)_MyForm["picEquip13"]).Texture = null;
					((PictureBox)_MyForm["picEquip17"]).Texture = null;
				}

                // Buttons index starting at 1.
                for (int i = 1; i <= m_MaxButtons; i++)
                {
                    string iPicName = "picEquip" + i;
                    string iBtnName = "btnEquip" + i;
                    // Check to make sure this button exists. If not, skip it.
                    if (_MyForm[iBtnName] != null)
                    {
                        ((CustomButton)_MyForm[iBtnName]).Texture = UIHelper.ItemIcon(mMobileObject.equipment[i]);
						if (mMobileObject.equipment[i] == null)
						{
							((PictureBox)_MyForm[iPicName]).Texture = null;
							((CustomButton)_MyForm[iBtnName]).Disabled = false;
						}
						else
						{
							// if we have something on the outertorso (e.g. a robe) the two inner torso layers are not drawn
							if (hasOuterTorso && (i == (int)EquipLayer.MiddleTorso || i == (int)EquipLayer.InnerTorso))
							{
								continue;
							}
							((PictureBox)_MyForm[iPicName]).Texture = Data.Gumps.GetGumpXNA (
								mMobileObject.equipment[i].AnimationDisplayID + 50000,
								mMobileObject.equipment[i].Hue, true );
							((CustomButton)_MyForm[iBtnName]).Disabled = false;
						}
                    }
                    else if (_MyForm[iPicName] != null)
                    {
                        int bodyID, hue;

                        switch (i)
                        {
                            case 0x0B:
                                if (mMobileObject.HairBodyID == 0)
                                    continue;
                                bodyID = mMobileObject.HairBodyID + 50000;
                                hue = mMobileObject.HairHue;
                                break;
                            case 0x10:
                                if (mMobileObject.FacialHairBodyID == 0)
                                    continue;
                                bodyID = mMobileObject.FacialHairBodyID + 50000;
                                hue = mMobileObject.FacialHairHue;
                                break;
                            default:
                                continue;
                        }

                        if (bodyID == 0)
                        {
                            ((PictureBox)_MyForm[iPicName]).Texture = null;
                        }
                        else
                        {
                            ((PictureBox)_MyForm[iPicName]).Texture = Data.Gumps.GetGumpXNA(bodyID, hue, true);
                        }
                    }
                }
                mLastContainerUpdated = mMobileObject.equipment.UpdateTicker;
            }
        }
    }
}
