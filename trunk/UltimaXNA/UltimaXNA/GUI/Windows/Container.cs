/***************************************************************************
 *   Container.cs
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
    class Window_Container : Window
    {
        private Vector2 mWindowSize = new Vector2(215, 233);
        private Vector2 mBGOffset = new Vector2(215 - 256 + 2, -1);
        private ContainerItem _containerEntity;
        private int _lastContainerUpdated = -1;

        private int mScrollY, mMaxScrollY = 0;

        int _SlotsWide = 0, _SlotsHigh = 0;
        Item[] _SlotsItems;
        int _SlotsTotal
        {
            get
            {
                int numberOfSlots = _SlotsWide * _SlotsHigh;
                if ((_SlotsItems == null) || (_SlotsItems.Length != numberOfSlots))
                {
                    _SlotsItems = new Item[numberOfSlots];
                    _lastContainerUpdated = -1;
                }
                return numberOfSlots;
            }
        }

        public Serial serial { get { return _containerEntity.Serial; } }

        public Window_Container(Entity nContainerObject, FormCollection nFormCollection)
            : base()
        {
            _containerEntity = (ContainerItem)nContainerObject;

            //Create a new form
            string iFormName = "frmContainer:" + _containerEntity.Serial;
            m_FormCollection.Add(new Form(iFormName, "", mWindowSize, new Vector2(800 - 256, 600 - 256), Form.BorderStyle.None));
            _MyForm = m_FormCollection[iFormName];
            _MyForm.BorderName = null;
            _MyForm.CustomDragArea = new Rectangle(44, 8, 143, 18);
            //_MyForm.MouseThrough = true;

            Controls.Add(new PictureBox("picBG", mBGOffset, @"GUI\CONTAINERFRAME\UI-Bag-4x4.png", 256, 256, 0));

            Controls.Add(new Label("lblContainer", new Vector2(16f, 7f), "ContainerFrame | " + _containerEntity.Serial,
                Color.TransparentBlack, Color.White, 160, Label.Align.Left));
            _MyForm["lblContainer"].FontName = "ArialNarrow10";

            Controls.Add(new CustomButton("btnClose", new Vector2(184, -1), new Rectangle(6, 7, 19, 18),
                @"GUI\COMMON\UI-Panel-MinimizeButton-Up.png", @"GUI\COMMON\UI-Panel-MinimizeButton-Down.png",
                @"GUI\COMMON\UI-Panel-MinimizeButton-Disabled.png", @"GUI\COMMON\UI-Panel-MinimizeButton-Highlight.png"));
            Controls["btnClose"].OnRelease = btnClose_OnRelease;

            Controls.Add(new CustomButton("btnScrollUp", new Vector2(183, 25), new Rectangle(6, 7, 19, 18),
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Up.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Down.png",
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Disabled.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Highlight.png"));
            Controls["btnScrollUp"].OnRelease = btnScrollUp_OnRelease;

            Controls.Add(new CustomButton("btnScrollDown", new Vector2(183, 174), new Rectangle(6, 7, 19, 18),
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Up.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Down.png",
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Disabled.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Highlight.png"));
            Controls["btnScrollDown"].OnRelease = btnScrollDown_OnRelease;

            _SlotsWide = 4;
            _SlotsHigh = 4;

            for (int i = 0; i < _SlotsTotal; i++)
            {
                string iBtnName = "btnInv" + i;
                Vector2 iPosition = new Vector2();
                iPosition.Y = (int)(i / _SlotsWide) * 41 + 34;
                iPosition.X = (i - ((int)(i / _SlotsWide)) * _SlotsWide) * 42 + 17;
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
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * _SlotsWide;
            Item item = _containerEntity.Contents.GetContents(iIndex);

            int buttonindex = 0;
            if (buttonindex == 0)
            {
                if (GUIHelper.MouseHoldingItem != null)
                {
                    GUIHelper.DropItemIntoSlot(_containerEntity, iIndex);
                }
                if (item != null)
                {
                    GUIHelper.PickUpItem(item);
                }
            }
            else if (buttonindex == 1)
            {
                Events.UseItem(item);
            }


        }

        private void btnInv_OnRelease(object obj, EventArgs e)
        {
            if (GUIHelper.MouseHoldingItem != null)
            {
                int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * _SlotsWide;
                GUIHelper.DropItemIntoSlot(_containerEntity, iIndex);
            }
        }

        private void btnInv_OnOver(object obj, EventArgs e)
        {
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * _SlotsWide;
            Item iItem = _containerEntity.Contents.GetContents(iIndex);

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

            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * _SlotsWide;
            Item iItem = _containerEntity.Contents.GetContents(iIndex);
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
            _lastContainerUpdated = -1;
        }

        private void btnScrollDown_OnRelease(object obj, EventArgs e)
        {
            mScrollY++;
            if (mScrollY > mMaxScrollY)
                mScrollY = mMaxScrollY;
            _lastContainerUpdated = -1;
        }

        public override void Update()
        {
            base.Update();

            if (this.IsClosed)
                return;

            if (_containerEntity.Contents.UpdateTicker != _lastContainerUpdated)
            {
                mMaxScrollY = (int)(_containerEntity.Contents.LastSlotOccupied / _SlotsWide) + 1 - _SlotsHigh;
                if (((_containerEntity.Contents.LastSlotOccupied + 1) % _SlotsWide) == 0)
                    mMaxScrollY++;

                for (int i = 0; i < _SlotsTotal; i++)
                {
                    _SlotsItems[i] = _containerEntity.Contents.GetContents(i + mScrollY * _SlotsWide);
                }
                _lastContainerUpdated = _containerEntity.Contents.UpdateTicker;

                for (int i = 0; i < _SlotsTotal; i++)
                {
                    string iBtnName = "btnInv" + i;
                    ((CustomButton)_MyForm[iBtnName]).Texture = GUIHelper.ItemIcon(_SlotsItems[i]);
                    if (_SlotsItems[i] == null)
                        ((CustomButton)_MyForm[iBtnName]).Disabled = false;
                    else
                        ((CustomButton)_MyForm[iBtnName]).Disabled = false;
                }
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
