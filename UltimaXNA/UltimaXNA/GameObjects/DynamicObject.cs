#region File Description & Usings
//-----------------------------------------------------------------------------
// DynamicObject.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
#endregion

namespace UltimaXNA.GameObjects
{
    class DynamicObject : Item
    {
        public int CasterSerial = 0;
        public int Bytes0 = 0;
        public int SpellID = 0;

        public DynamicObject(Serial serial)
            : base(serial)
        {

        }
    }
}
