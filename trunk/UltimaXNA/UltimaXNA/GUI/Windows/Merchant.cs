/***************************************************************************
 *   Merchant.cs
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

namespace UltimaXNA.GUI
{
    class Window_Merchant : Window
    {
        private Vector2 mWindowSize = new Vector2(345, 334);
        private Vector2 mBGOffset = new Vector2(-12, -13);
        private ContainerItem mContainerObject;
        private int mLastContainerUpdated = -1;

        private int mScrollY, mMaxScrollY = 0;

        public Serial serial { get { return mContainerObject.Serial; } }

        public Window_Merchant(Entity nContainerObject, FormCollection nFormCollection)
            : base()
        {
            mContainerObject = (ContainerItem)nContainerObject;

            //Create a new form
            string iFormName = "frmMerchant:" + mContainerObject.Serial;
            m_FormCollection.Add(new Form(iFormName, "", mWindowSize, new Vector2(200, 200), Form.BorderStyle.None));
            _MyForm = m_FormCollection[iFormName];
            _MyForm.BorderName = null;
            _MyForm.CustomDragArea = new Rectangle(1, 1, 308, 20);
            //_MyForm.MouseThrough = true;

            Controls.Add(new PictureBox("picBG", mBGOffset, @"GUI\MERCHFRAME\MERCHFRAME-BG.png", 512, 512, 0));

            Controls.Add(new Label("lblContainer", new Vector2(8f, 4f), "MerchantFrame", Color.TransparentBlack, Color.White, 128, Label.Align.Left));
            _MyForm["lblContainer"].FontName = "ArialNarrow10";

            Controls.Add(new CustomButton("btnClose", new Vector2(308, -5), new Rectangle(6, 7, 19, 18),
                @"GUI\COMMON\UI-Panel-MinimizeButton-Up.png", @"GUI\COMMON\UI-Panel-MinimizeButton-Down.png",
                @"GUI\COMMON\UI-Panel-MinimizeButton-Disabled.png", @"GUI\COMMON\UI-Panel-MinimizeButton-Highlight.png"));
            Controls["btnClose"].OnRelease = btnClose_OnRelease;

            Controls.Add(new CustomButton("btnScrollUp", new Vector2(307, 21), new Rectangle(6, 7, 19, 18),
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Up.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Down.png",
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Disabled.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Highlight.png"));
            Controls["btnScrollUp"].OnRelease = btnScrollUp_OnRelease;

            Controls.Add(new CustomButton("btnScrollDown", new Vector2(307, 273), new Rectangle(6, 7, 19, 18),
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Up.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Down.png",
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Disabled.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Highlight.png"));
            Controls["btnScrollDown"].OnRelease = btnScrollDown_OnRelease;

            for (int i = 0; i < 12; i++)
            {
                string iControlName = "bgItem" + i;
                string iLabelName = "lblItem" + i;
                Vector2 iPosition = new Vector2();
                iPosition.Y = (int)(i / 2) * 45 + 29;
                iPosition.X = (i - ((int)(i / 2)) * 2) * 152  + 5;
                Controls.Add(new PictureBox(iControlName, iPosition, @"GUI\MERCHFRAME\MERCHFRAME-ITEM-BG.png", 0));
                iPosition.X += 44;
                iPosition.Y += 3;
                Controls.Add(new Label(iLabelName, iPosition, "",  Color.TransparentBlack, Color.White, 100, Label.Align.Left));
                Controls[iLabelName].FontName = "ArialNarrow10";
            }

            for (int i = 0; i < 12; i++)
            {
                string iBtnName = "btnInv" + i;
                Vector2 iPosition = new Vector2();
                iPosition.Y = (int)(i / 2) * 45 + 30;
                iPosition.X = (i - ((int)(i / 2)) * 2) * 152 + 6;
                Controls.Add(new CustomButton(iBtnName, iPosition, new Rectangle(0, 0, 39, 39),
                    null, null, null, null, 1f));
                _MyForm[iBtnName].OnMouseOver += btnInv_OnOver;
                _MyForm[iBtnName].OnMouseOut += btnInv_OnOut;
                _MyForm[iBtnName].OnPress += btnInv_OnPress;
                _MyForm[iBtnName].OnRelease += btnInv_OnRelease;
            }

            this.Show();
        }

        private void btnInv_OnPress(object obj, EventArgs e)
        {
            // Buy this item!
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * 2;
            Item iItem = mContainerObject.Contents.GetContents(iIndex);
            if (iItem != null)
            {
                // pick the item up!
                GUIHelper.BuyItemFromVendor(mContainerObject.Wearer.Serial, iItem.Serial, 1);
            }
        }

        private void btnInv_OnRelease(object obj, EventArgs e)
        {
            // Can't drop items into a merchant frame!
            // if (GUIHelper.MouseHoldingItem != null)
            // {
            //     int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * 2;
            //     GUIHelper.DropItemIntoSlot(mContainerObject, iIndex);
            // }
        }

        private void btnInv_OnOver(object obj, EventArgs e)
        {
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * 2;
            Item iItem = mContainerObject.Contents.GetContents(iIndex);

            if (GUIHelper.MouseHoldingItem != null)
            {
                ((CustomButton)obj).RespondToAllReleaseEvents = true;
            }
            else
            {
                GUIHelper.ToolTipItem = iItem;
                GUIHelper.TooltipX = (int)_MyForm.X + (int)((CustomButton)obj).X + 42;
                GUIHelper.TooltipY = (int)_MyForm.Y + (int)((CustomButton)obj).Y;
            }
        }
        private void btnInv_OnOut(object obj, EventArgs e)
        {
            if (GUIHelper.MouseHoldingItem != null)
            {
                ((CustomButton)obj).RespondToAllReleaseEvents = false;
            }

            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * 2;
            Item iItem = mContainerObject.Contents.GetContents(iIndex);
            if (GUIHelper.ToolTipItem == iItem)
            {
                GUIHelper.ToolTipItem = null;
            }
        }

        private void btnClose_OnRelease(object obj, EventArgs e)
        {
            Close();
        }

        private void btnScrollUp_OnRelease(object obj, EventArgs e)
        {
            mScrollY--;
            if (mScrollY < 0)
                mScrollY = 0;
            mLastContainerUpdated = -1;
        }

        private void btnScrollDown_OnRelease(object obj, EventArgs e)
        {
            mScrollY++;
            if (mScrollY > mMaxScrollY)
                mScrollY = mMaxScrollY;
            mLastContainerUpdated = -1;
        }

        public override void Update()
        {
            base.Update();

            if (this.IsClosed)
                return;

            if (mContainerObject.Contents.UpdateTicker != mLastContainerUpdated)
            {
                mMaxScrollY = (int)(mContainerObject.Contents.LastSlotOccupied / 2) + 1 - 6;

                for (int i = 0; i < 12; i++)
                {
                    int iItemTypeID = 0;
                    Item iItem = mContainerObject.Contents.GetContents(i + mScrollY * 2);
                    if (iItem != null)
                        iItemTypeID = iItem.ItemID;
                    string iBtnName = "btnInv" + i;
                    string iLblName = "lblItem" + i;
                    ((CustomButton)_MyForm[iBtnName]).Texture = GUIHelper.ItemIcon(iItem);
                    if (iItemTypeID == 0)
                    {
                        ((CustomButton)_MyForm[iBtnName]).Disabled = false;
                    }
                    else
                    {
                        ((CustomButton)_MyForm[iBtnName]).Disabled = false;
                        ((Label)_MyForm[iLblName]).Text = iItem.ItemData.Name
                            + Environment.NewLine + iItem.ItemData.AnimID;
                    }

                }
                mLastContainerUpdated = mContainerObject.Contents.UpdateTicker;
            }

            if (mScrollY == 0)
                ((CustomButton)Controls["btnScrollUp"]).Disabled = true;
            else
                ((CustomButton)Controls["btnScrollUp"]).Disabled = false;

            if (mScrollY >= mMaxScrollY)
                ((CustomButton)Controls["btnScrollDown"]).Disabled = true;
            else
                ((CustomButton)Controls["btnScrollDown"]).Disabled = false;
        }
    }
}
