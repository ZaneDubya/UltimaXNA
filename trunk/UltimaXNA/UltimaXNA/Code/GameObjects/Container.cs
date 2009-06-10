#region File Description & Usings
//-----------------------------------------------------------------------------
// Container.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
#endregion

namespace UltimaXNA.GameObjects
{
    class Container : UltimaXNA.GameObjects.Item
    {
        public int Container_NumSlots = 0;
        public int[] Container_Contents = new int[20];

        public Container(int nGUID)
            : base(nGUID)
        {
            ObjectType = ObjectType.Container;
        }
    }
}
