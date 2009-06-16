#region File Description & Usings
//-----------------------------------------------------------------------------
// GUIWindow.cs
//
// This is the container file for a scripted window.
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using xWinFormsLib;
#endregion

namespace UltimaXNA.GUI
{
    public class Window
    {
        internal FormCollection m_FormCollection;
        internal Form m_MyForm;
        public bool IsClosed = false;

        public Window(FormCollection nFormCollection)
        {
            m_FormCollection = nFormCollection;
        }

        public void Close()
        {
            if (m_MyForm != null)
            {
                m_MyForm.Dispose();
                m_MyForm = null;
                IsClosed = true;
            }
        }

        public virtual void Update()
        {

        }

        public void Show()
        {
            m_MyForm.Show();
        }
    }
}
