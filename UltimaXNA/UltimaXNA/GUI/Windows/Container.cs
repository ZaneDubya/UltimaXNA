using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;

namespace UltimaXNA.GUI
{
    class Window_Container : Window
    {
        private Vector2 mWindowSize = new Vector2(215, 233);
        private Vector2 mBGOffset = new Vector2(215 - 256 + 2, -1);
        private GameObjects.GameObject mContainerObject;
        private int mLastContainerUpdated = -1;

        private int mScrollY, mMaxScrollY = 0;

        int _SlotsWide, _SlotsHigh;
        int _SlotsTotal
        {
            get
            {
                int numberOfSlots = _SlotsWide * _SlotsHigh;
                if ((_SlotsItemIDs == null) || (_SlotsItemIDs.Length != numberOfSlots))
                {
                    _SlotsItemIDs = new int[numberOfSlots];
                    mLastContainerUpdated = -1;
                }
                return numberOfSlots;
            }
        }
        int[] _SlotsItemIDs;

        public Serial serial { get { return mContainerObject.Serial; } }

        public Window_Container(GameObjects.BaseObject nContainerObject, FormCollection nFormCollection)
            : base()
        {
            mContainerObject = (GameObjects.GameObject)nContainerObject;

            //Create a new form
            string iFormName = "frmContainer:" + mContainerObject.Serial;
            m_FormCollection.Add(new Form(iFormName, "", mWindowSize, new Vector2(800 - 256, 600 - 256), Form.BorderStyle.None));
            _MyForm = m_FormCollection[iFormName];
            _MyForm.BorderName = null;
            _MyForm.CustomDragArea = new Rectangle(44, 8, 143, 18);
            //_MyForm.MouseThrough = true;

            Controls.Add(new PictureBox("picBG", mBGOffset, @"GUI\CONTAINERFRAME\UI-Bag-4x4.png", 256, 256, 0));

            Controls.Add(new Label("lblContainer", new Vector2(16f, 7f), "ContainerFrame | " + GUIHelper.SerialHex(mContainerObject.Serial),
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
            GameObjects.GameObject iItem = mContainerObject.ContainerObject.GetContents(iIndex);
            if (GUIHelper.MouseHoldingItem != null)
            {
                GUIHelper.DropItemIntoSlot(mContainerObject, iIndex);
            }
            if (iItem != null)
            {
                // pick the item up!
                GUIHelper.PickUpItem(iItem);
            }
        }

        private void btnInv_OnRelease(object obj, EventArgs e)
        {
            if (GUIHelper.MouseHoldingItem != null)
            {
                int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * _SlotsWide;
                GUIHelper.DropItemIntoSlot(mContainerObject, iIndex);
            }
        }

        private void btnInv_OnOver(object obj, EventArgs e)
        {
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * _SlotsWide;
            GameObjects.GameObject iItem = mContainerObject.ContainerObject.GetContents(iIndex);

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
            GameObjects.GameObject iItem = mContainerObject.ContainerObject.GetContents(iIndex);
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

            if (mContainerObject.ContainerObject.UpdateTicker != mLastContainerUpdated)
            {
                mMaxScrollY = (int)(mContainerObject.ContainerObject.LastSlotOccupied / _SlotsWide) + 1 - _SlotsHigh;
                if (((mContainerObject.ContainerObject.LastSlotOccupied + 1) % _SlotsWide) == 0)
                    mMaxScrollY++;

                for (int i = 0; i < _SlotsTotal; i++)
                {
                    GameObjects.GameObject iItem = mContainerObject.ContainerObject.GetContents(i + mScrollY * _SlotsWide);
                    if (iItem == null)
                        _SlotsItemIDs[i] = 0;
                    else
                        _SlotsItemIDs[i] = iItem.ObjectTypeID;
                }
                mLastContainerUpdated = mContainerObject.ContainerObject.UpdateTicker;

                for (int i = 0; i < _SlotsTotal; i++)
                {
                    string iBtnName = "btnInv" + i;
                    ((CustomButton)_MyForm[iBtnName]).Texture = GUIHelper.ItemIcon(_SlotsItemIDs[i]);
                    if (_SlotsItemIDs[i] == 0)
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
