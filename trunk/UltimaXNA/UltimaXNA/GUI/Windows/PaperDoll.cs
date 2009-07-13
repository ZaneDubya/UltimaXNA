using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;

namespace UltimaXNA.GUI
{
    class Window_PaperDoll : Window
    {
        private Vector2 mWindowSize = new Vector2(228, 400);
        private Vector2 mBGOffset = new Vector2(0, 0);
        private GameObjects.Unit mMobileObject;
        private int mLastContainerUpdated = -1;
        private const int m_MaxButtons = 0x18;

        public Serial serial { get { return mMobileObject.Serial; } }

        public Window_PaperDoll(GameObjects.BaseObject nMobileObject, FormCollection nFormCollection)
            : base()
        {
            mMobileObject = (GameObjects.Unit)nMobileObject;

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

            for (int i = 0x00; i <= 0x18; i++)
                m_CreateGumpTexture(i, 20, 85);

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
            // skip 0x0B: Hair
            m_CreateEquipButton(0x0C, 172, 155); // belt
            m_CreateEquipButton(0x0D, 12, 235); // chest
            m_CreateEquipButton(0x0E, 172, 35); // bracelet
            // skip 0x0F: unused.
            // skip 0x10: facial hair
            m_CreateEquipButton(0x11, 92, 35); // sash
            m_CreateEquipButton(0x12, 52, 35); // earring
            m_CreateEquipButton(0x13, 172, 75); // sleeves
            m_CreateEquipButton(0x14, 12, 195); // back
            // skip 0x15: backpack
            m_CreateEquipButton(0x16, 12, 155); // robe
            m_CreateEquipButton(0x17, 172, 235); // skirt
            // skip 0x18: inner legs (?)

            m_FormCollection.Add(_MyForm);
            this.Show();
            
        }

        private void m_CreateGumpTexture(int nEquipIndex, int nX, int nY)
        {
            string iPicName = "picEquip" + nEquipIndex;
            Vector2 iPosition = new Vector2();
            iPosition.Y = nY;
            iPosition.X = nX;
            Controls.Add(new PictureBox(iPicName, iPosition, string.Empty, 0));
        }

        private void m_CreateEquipButton(int nEquipIndex, int nX, int nY)
        {
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

        private void btnEquip_OnPress(object obj, EventArgs e)
        {
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(8));
            GameObjects.GameObject iItem = mMobileObject.Equipment[iIndex];
            if (iItem != null)
            {
                // pick the item up!
                GUIHelper.PickUpItem(iItem);
            }
        }

        private void btnEquip_OnRelease(object obj, EventArgs e)
        {
            if (GUIHelper.MouseHoldingItem != null)
            {
                int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(8));
                GUIHelper.WearItem(mMobileObject, iIndex);
            }
        }

        private void btnEquip_OnOver(object obj, EventArgs e)
        {
            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(8));
            GameObjects.GameObject iItem = mMobileObject.Equipment[iIndex];

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
        private void btnEquip_OnOut(object obj, EventArgs e)
        {
            if (GUIHelper.MouseHoldingItem != null)
            {
                ((CustomButton)obj).RespondToAllReleaseEvents = false;
            }

            int iIndex = Int32.Parse(((CustomButton)obj).Name.Substring(8));
            GameObjects.GameObject iItem = mMobileObject.Equipment[iIndex];
            if (GUIHelper.ToolTipItem == iItem)
            {
                GUIHelper.ToolTipItem = null;
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

            if (mMobileObject.Equipment.UpdateTicker != mLastContainerUpdated)
            {

                ((PictureBox)_MyForm["picEquip0"]).Texture = Data.Gumps.GetGumpXNA(0x000C);

                // Buttons index starting at 1.
                for (int i = 1; i <= m_MaxButtons; i++)
                {
                    string iPicName = "picEquip" + i;
                    string iBtnName = "btnEquip" + i;
                    // Check to make sure this button exists. If not, skip it.
                    if (_MyForm[iBtnName] == null)
                        continue;

                    int iItemTypeID = 0;
                    GameObjects.GameObject iItem = mMobileObject.Equipment[i];
                    if (iItem != null)
                        iItemTypeID = iItem.ObjectTypeID;
                    ((CustomButton)_MyForm[iBtnName]).Texture = GUIHelper.ItemIcon(iItemTypeID);
                    if (iItemTypeID == 0)
                    {
                        ((PictureBox)_MyForm[iPicName]).Texture = null;
                        ((CustomButton)_MyForm[iBtnName]).Disabled = false;
                    }
                    else
                    {
                        ((PictureBox)_MyForm[iPicName]).Texture = Data.Gumps.GetGumpXNA(iItem.AnimationDisplayID + 50000);
                        ((CustomButton)_MyForm[iBtnName]).Disabled = false;
                    }
                }
                mLastContainerUpdated = mMobileObject.Equipment.UpdateTicker;
            }
        }
    }
}
