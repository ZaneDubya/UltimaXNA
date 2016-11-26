/***************************************************************************
 *   ModelManager.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Core.Patterns.MVC
{
    class ModelManager
    {
        AModel m_Model;
        AModel m_QueuedModel;

        public AModel Next
        {
            get { return m_QueuedModel; }
            set
            {
                if (m_QueuedModel != null)
                {
                    m_QueuedModel.Dispose();
                    m_QueuedModel = null;
                }
                m_QueuedModel = value;
                if (m_QueuedModel != null)
                {
                    m_QueuedModel.Initialize();
                }
            }
        }

        public AModel Current
        {
            get { return m_Model; }
            set
            {
                if (m_Model != null)
                {
                    m_Model.Dispose();
                    m_Model = null;
                }
                m_Model = value;
                if (m_Model != null)
                {
                    m_Model.Initialize();
                }
            }
        }

        public void ActivateNext()
        {
            if (m_QueuedModel != null)
            {
                Current = Next;
                m_QueuedModel = null;
            }
        }
    }
}
