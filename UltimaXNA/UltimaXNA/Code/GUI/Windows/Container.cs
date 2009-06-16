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
        private GameObjects.Container mContainerObject;
        private int mLastContainerUpdated = -1;

        private int mScrollY, mMaxScrollY = 0;

        public int GUID { get { return mContainerObject.GUID; } }

        public Window_Container(GameObjects.BaseObject nContainerObject, FormCollection nFormCollection)
            : base(nFormCollection)
        {
            mContainerObject = (GameObjects.Container)nContainerObject;

            //Create a new form
            string iFormName = "frmContainer:" + mContainerObject.GUID;
            m_FormCollection.Add(new Form(iFormName, "", mWindowSize, new Vector2(800 - 256, 600 - 256), Form.BorderStyle.None));
            m_MyForm = m_FormCollection[iFormName];
            m_MyForm.BorderName = null;
            m_MyForm.CustomDragArea = new Rectangle(44, 8, 143, 18);
            //m_MyForm.MouseThrough = true;

            m_MyForm.Controls.Add(new PictureBox("picBG", mBGOffset, @"GUI\CONTAINERFRAME\UI-Bag-4x4.png", 256, 256, 0));

            m_MyForm.Controls.Add(new Label("lblContainer", new Vector2(16f, 7f), "ContainerFrame", Color.TransparentBlack, Color.White, 128, Label.Align.Left));
            m_MyForm["lblContainer"].FontName = "ArialNarrow10";

            m_MyForm.Controls.Add(new CustomButton("btnClose", new Vector2(184, -1), new Rectangle(6, 7, 19, 18),
                @"GUI\COMMON\UI-Panel-MinimizeButton-Up.png", @"GUI\COMMON\UI-Panel-MinimizeButton-Down.png",
                @"GUI\COMMON\UI-Panel-MinimizeButton-Disabled.png", @"GUI\COMMON\UI-Panel-MinimizeButton-Highlight.png"));
            m_MyForm.Controls["btnClose"].OnRelease = btnClose_OnRelease;

            m_MyForm.Controls.Add(new CustomButton("btnScrollUp", new Vector2(183, 25), new Rectangle(6, 7, 19, 18),
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Up.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Down.png",
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Disabled.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Highlight.png"));
            m_MyForm.Controls["btnScrollUp"].OnRelease = btnScrollUp_OnRelease;

            m_MyForm.Controls.Add(new CustomButton("btnScrollDown", new Vector2(183, 174), new Rectangle(6, 7, 19, 18),
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Up.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Down.png",
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Disabled.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Highlight.png"));
            m_MyForm.Controls["btnScrollDown"].OnRelease = btnScrollDown_OnRelease;

            for (int i = 0; i < 16; i++)
            {
                string iBtnName = "btnInv" + i;
                Vector2 iPosition = new Vector2();
                iPosition.Y = (int)(i / 4) * 41 + 34;
                iPosition.X = (i - ((int)(i / 4)) * 4) * 42 + 17;
                m_MyForm.Controls.Add(new CustomButton(iBtnName, iPosition, new Rectangle(0, 0, 39, 39),
                    null, null, null, null, 1f));
                m_MyForm[iBtnName].OnMouseOver += btnInv_OnOver;
                m_MyForm[iBtnName].OnMouseOut += btnInv_OnOut;
                m_MyForm[iBtnName].OnPress += btnInv_OnPress;
                m_MyForm[iBtnName].OnRelease += btnInv_OnRelease;
            }
            this.Show();
        }

        private void btnInv_OnPress(object obj, EventArgs e)
        {
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * 4;
            GameObjects.GameObject iItem = mContainerObject.GetContents(iIndex);
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
                int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * 4;
                GUIHelper.DropItemIntoSlot(mContainerObject, iIndex);
            }
        }

        private void btnInv_OnOver(object obj, EventArgs e)
        {
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * 4;
            GameObjects.GameObject iItem = mContainerObject.GetContents(iIndex);

            if (GUIHelper.MouseHoldingItem != null)
            {
                ((CustomButton)obj).RespondToAllReleaseEvents = true;
            }
            else
            {
                GUIHelper.ToolTipItem = iItem;
                GUIHelper.TooltipX = (int)m_MyForm.X + (int)((CustomButton)obj).X + 42;
                GUIHelper.TooltipY = (int)m_MyForm.Y + (int)((CustomButton)obj).Y;
            }
        }
        private void btnInv_OnOut(object obj, EventArgs e)
        {
            if (GUIHelper.MouseHoldingItem != null)
            {
                ((CustomButton)obj).RespondToAllReleaseEvents = false;
            }

            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(6)) + mScrollY * 4;
            GameObjects.GameObject iItem = mContainerObject.GetContents(iIndex);
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

            if (mContainerObject.UpdateTicker != mLastContainerUpdated)
            {
                mMaxScrollY = (int)(mContainerObject.LastSlotOccupied / 4) + 1 - 4;

                for (int i = 0; i < 16; i++)
                {
                    int iItemTypeID = 0;
                    GameObjects.GameObject iItem = mContainerObject.GetContents(i + mScrollY * 4);
                    if (iItem != null)
                        iItemTypeID = iItem.ObjectTypeID;
                    string iBtnName = "btnInv" + i; 
                    ((CustomButton)m_MyForm[iBtnName]).Texture = GUIHelper.GetItemIcon(iItemTypeID);
                    if (iItemTypeID == 0)
                        ((CustomButton)m_MyForm[iBtnName]).Disabled = false;
                    else
                        ((CustomButton)m_MyForm[iBtnName]).Disabled = false;
                }
                mLastContainerUpdated = mContainerObject.UpdateTicker;
            }

            if (mScrollY == 0)
                ((CustomButton)m_MyForm.Controls["btnScrollUp"]).Disabled = true;
            else
                ((CustomButton)m_MyForm.Controls["btnScrollUp"]).Disabled = false;

            if (mScrollY >= mMaxScrollY)
                ((CustomButton)m_MyForm.Controls["btnScrollDown"]).Disabled = true;
            else
                ((CustomButton)m_MyForm.Controls["btnScrollDown"]).Disabled = false;
        }
    }
}
