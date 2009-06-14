using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xWinFormsLib;

namespace UltimaXNA.GUI
{
    class Window_Container : Window
    {
        private Vector2 m_Size = new Vector2(215, 217);
        private Vector2 m_BGOffset = new Vector2(215 - 256 + 2, -1);
        private GameObjects.Container m_Container;
        private int m_LastContainerUpdated;

        private int m_ScrollY, m_MaxScrollY = 0;

        public int GUID { get { return m_Container.GUID; } }

        public Window_Container(GameObjects.BaseObject nContainerObject, FormCollection nFormCollection)
            : base(nFormCollection)
        {
            m_Container = (GameObjects.Container)nContainerObject;

            //Create a new form
            string iFormName = "frmContainer:" + m_Container.GUID;
            m_FormCollection.Add(new Form(iFormName, "", m_Size, new Vector2(800 - 256, 600 - 256), Form.BorderStyle.None));
            m_MyForm = m_FormCollection[iFormName];
            m_MyForm.BorderName = null;
            m_MyForm.CustomDragArea = new Rectangle(44, 8, 143, 18);
            //m_MyForm.MouseThrough = true;

            m_MyForm.Controls.Add(new PictureBox("picBG", m_BGOffset, @"GUI\CONTAINERFRAME\UI-Bag-4x4.png", 256, 256, 0));

            m_MyForm.Controls.Add(new Label("lblContainer", new Vector2(50, 7f), "ContainerFrame", Color.TransparentBlack, Color.White, 128, Label.Align.Left));
            m_MyForm["lblContainer"].FontName = "ArialNarrow10";

            m_MyForm.Controls.Add(new CustomButton("btnClose", new Vector2(185, 0), new Rectangle(6, 7, 19, 18),
                @"GUI\COMMON\UI-Panel-MinimizeButton-Up.png", @"GUI\COMMON\UI-Panel-MinimizeButton-Down.png",
                @"GUI\COMMON\UI-Panel-MinimizeButton-Disabled.png", @"GUI\COMMON\UI-Panel-MinimizeButton-Highlight.png"));
            m_MyForm.Controls["btnClose"].OnRelease = btnClose_OnRelease;

            m_MyForm.Controls.Add(new CustomButton("btnScrollUp", new Vector2(181, 42), new Rectangle(6, 7, 19, 18),
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Up.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Down.png",
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Disabled.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollUpButton-Highlight.png"));
            m_MyForm.Controls["btnScrollUp"].OnRelease = btnScrollUp_OnRelease;

            m_MyForm.Controls.Add(new CustomButton("btnScrollDown", new Vector2(181, 186), new Rectangle(6, 7, 19, 18),
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Up.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Down.png",
                @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Disabled.png", @"GUI\SCROLLBAR\UI-ScrollBar-ScrollDownButton-Highlight.png"));
            m_MyForm.Controls["btnScrollDown"].OnRelease = btnScrollDown_OnRelease;

            //Show the form
            this.Show();
        }

        private void btnClose_OnRelease(object obj, EventArgs e)
        {
            Close();
        }

        private void btnScrollUp_OnRelease(object obj, EventArgs e)
        {
            m_ScrollY--;
            if (m_ScrollY < 0)
                m_ScrollY = 0;
            m_LastContainerUpdated = 0;
        }

        private void btnScrollDown_OnRelease(object obj, EventArgs e)
        {
            m_ScrollY++;
            if (m_ScrollY > m_MaxScrollY)
                m_ScrollY = m_MaxScrollY;
            m_LastContainerUpdated = 0;
        }

        public override void Update()
        {
            base.Update();

            if (this.IsClosed)
                return;

            if (m_Container.Updated != m_LastContainerUpdated)
            {
                m_MaxScrollY = (int)(m_Container.NumContents / 4) + 1 - 4;

                float iScale = 40f / 64f;
                for (int i = 0; i < 16; i++)
                {
                    int iItemTypeID = 0;
                    GameObjects.Item iItem = m_Container.GetContents(i + m_ScrollY * 4);
                    if (iItem != null)
                        iItemTypeID = iItem.ItemTypeID;

                    string iBtnName = "btnInv" + i; 
                    m_MyForm.Remove(iBtnName);
                    Vector2 iPosition = new Vector2();
                    iPosition.Y = (int)(i / 4) * 40 + 49;
                    iPosition.X = (i - ((int)(i / 4)) * 4) * 42 + 17;
                    m_MyForm.Controls.Add(new CustomButton(iBtnName, iPosition, new Rectangle(0, 0, 32, 32),
                    null, null, null, null, iScale));

                    ((CustomButton)m_MyForm[iBtnName]).Texture = m_GetItemIcon(iItemTypeID);
                }
                m_LastContainerUpdated = m_Container.Updated;
            }

            

            if (m_ScrollY == 0)
                ((CustomButton)m_MyForm.Controls["btnScrollUp"]).Disabled = true;
            else
                ((CustomButton)m_MyForm.Controls["btnScrollUp"]).Disabled = false;

            if (m_ScrollY == m_MaxScrollY)
                ((CustomButton)m_MyForm.Controls["btnScrollDown"]).Disabled = true;
            else
                ((CustomButton)m_MyForm.Controls["btnScrollDown"]).Disabled = false;
        }

        private Texture2D m_GetItemIcon(int nItemID)
        {
            // This is an awful hack to get item icons. Ideally this should be put in a static class,
            // that will cache the item icons. I'll fix it later...
            GraphicsDevice iDevice = FormCollection.Graphics.GraphicsDevice;

            // Texture2D iTextureArt = Texture2D.FromFile(iDevice, FormCollection.ContentManager.RootDirectory + @"GUI\ICONS\INV_Drink_05.png");
            Texture2D iTextureArt = null;
            if (nItemID != 0)
            {
                iTextureArt = DataLocal.Art.GetStaticTexture(nItemID, FormCollection.Graphics.GraphicsDevice);
            }
            Texture2D iTextureFrame = Texture2D.FromFile(iDevice, FormCollection.ContentManager.RootDirectory + @"GUI\COMMON\UI-Quickslot2.png");
            int iWidth, iHeight;
            DataLocal.Art.GetStaticDimensions(nItemID, out iWidth, out iHeight);


            float iScaleUp = 1f;
            if (iTextureArt != null)
            {
                if (iTextureArt.Width > iTextureArt.Height)
                    iScaleUp = 64f / iTextureArt.Height;
                else
                    iScaleUp = 64f / iTextureArt.Width;
            }

            
            RenderTarget2D renderTarget = new RenderTarget2D(iDevice, 64, 64, 1, iDevice.DisplayMode.Format);
            iDevice.SetRenderTarget(0, renderTarget);

            iDevice.Clear(Color.TransparentBlack);
            using (SpriteBatch sprite = new SpriteBatch(iDevice))
            {
                sprite.Begin();

                if (iTextureArt != null)
                {
                    sprite.Draw(iTextureArt,
                        new Rectangle(
                            -(int)((iTextureArt.Width * iScaleUp) - 64) / 2,
                            -(int)(((iTextureArt.Height - 6) * iScaleUp) - 64) / 2,
                            (int)(iTextureArt.Width * iScaleUp),
                            (int)(iTextureArt.Height * iScaleUp)),
                        Color.White);
                }
                sprite.Draw(iTextureFrame, new Rectangle(0, 0, 64, 64), new Rectangle(12, 12, 39, 39), Color.White);
                
                sprite.End();
            }

            iDevice.SetRenderTarget(0, null);
            Texture2D iTexture = renderTarget.GetTexture();

            return iTexture;
        }
    }
}
