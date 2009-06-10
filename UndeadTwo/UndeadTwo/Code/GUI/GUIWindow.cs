#region File Description & Usings
//-----------------------------------------------------------------------------
// GUIWindow.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using xWinFormsLib;
#endregion

namespace UndeadClient.GUI
{
    class GUIWindow
    {
        internal FormCollection m_FormCollection;

        public GUIWindow(FormCollection nFormCollection)
        {
            m_FormCollection = nFormCollection;
        }
    }
}
