#region File Description & Usings
//-----------------------------------------------------------------------------
// Player.cs
//
// Created by Poplicola
//-----------------------------------------------------------------------------
using Microsoft.Xna.Framework;
#endregion

namespace UltimaXNA.GameObjects
{
    public class Player : UltimaXNA.GameObjects.Unit
    {
        public Player(Serial serial)
            : base(serial)
        {
            this.ObjectType = ObjectType.Player;
            m_Animation.HoldAnimationMS = 0;
        }

        public override string ToString()
        {
            return base.ToString() + " | " + Name;
        }
    }
}
