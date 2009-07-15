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
    public class PlayerMobile : Mobile
    {
        public PlayerMobile(Serial serial)
            : base(serial)
        {
            animation.HoldAnimationMS = 0;
        }

        public override string ToString()
        {
            return base.ToString() + " | " + Name;
        }
    }
}
