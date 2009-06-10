#region File Description & Usings
//-----------------------------------------------------------------------------
// DynamicObject.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
#endregion

namespace UndeadClient.GameObjects
{
    class DynamicObject : UndeadClient.GameObjects.GameObject
    {
        public int CasterGUID = 0;
        public int Bytes0 = 0;
        public int SpellID = 0;

        public DynamicObject(int nGUID)
            : base(nGUID)
        {
            ObjectType = ObjectType.DynamicObject;
        }
    }
}
