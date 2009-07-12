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
        internal Form _MyForm;
        public bool IsClosed = false;

        public Window()
        {
            m_FormCollection = GUIHelper.FormCollection;
        }

        public void Close()
        {
            if (_MyForm != null)
            {
                _MyForm.Dispose();
                _MyForm = null;
                IsClosed = true;
            }
        }

        public virtual void Update()
        {

        }

        public void Show()
        {
            _MyForm.Show();
        }
    }
}
