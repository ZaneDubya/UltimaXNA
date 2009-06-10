#region File Description & Usings
//-----------------------------------------------------------------------------
// Corpse.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
#endregion

namespace UndeadClient.GameObjects
{
    class Corpse : UndeadClient.GameObjects.GameObject
    {
        public int OwnerGUID = 0;
        public int[] EquipSlotDisplayIDs = new int[12];
        public int bytes0 = 0, bytes1 = 0, bytes2 = 0;
        public int CorpseFlags = 0;

        public Corpse(int nGUID)
            : base(nGUID)
        {
            ObjectType = ObjectType.Corpse;
        }
    }
}
