/***************************************************************************
 *   Window.cs
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

        internal ControlCollection Controls
        {
            get { return _MyForm.Controls; }
        }

        public void Show()
        {
            _MyForm.Show();
        }
    }
}
