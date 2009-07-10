#region File Description & Usings
//-----------------------------------------------------------------------------
// Corpse.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
#endregion

namespace UltimaXNA.GameObjects
{
    class Corpse : UltimaXNA.GameObjects.GameObject
    {
        public int OwnerSerial = 0;
        public int[] EquipSlotDisplayIDs = new int[12];
        public int bytes0 = 0, bytes1 = 0, bytes2 = 0;
        public int CorpseFlags = 0;

        public Corpse(Serial serial)
            : base(serial)
        {
            ObjectType = ObjectType.Corpse;
        }
    }
}
