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
    class Window
    {
        internal FormCollection m_FormCollection;
        internal Form m_MyForm;

        public Window(FormCollection nFormCollection)
        {
            m_FormCollection = nFormCollection;
        }

        public void Unload()
        {
            if (m_MyForm != null)
            {
                m_MyForm.Dispose();
                m_MyForm = null;
            }
        }
    }
}
