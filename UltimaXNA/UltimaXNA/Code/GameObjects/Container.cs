#region File Description & Usings
//-----------------------------------------------------------------------------
// Container.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using System.Collections.Generic;
#endregion

namespace UltimaXNA.GameObjects
{
    class Container : UltimaXNA.GameObjects.Item
    {
        // public int Container_NumSlots = 0;
        // public int[] Container_Contents = new int[20];
        private List<Item> m_Contents;

        // When Updated is different from the last time you checked it, you should update your representation of this Container.
        public int Updated = 0;

        public Container(int nGUID)
            : base(nGUID)
        {
            ObjectType = ObjectType.Container;
            m_Contents = new List<Item>();
        }

        public void AddToContents(Item nObject)
        {
            m_Contents.Add(nObject);
            Updated++;
        }

        public Item GetContents(int nIndex)
        {
            if (nIndex < m_Contents.Count)
                return m_Contents[nIndex];
            else
                return null;
        }

        public int NumContents
        {
            get { return m_Contents.Count; }
        }
    }
}
