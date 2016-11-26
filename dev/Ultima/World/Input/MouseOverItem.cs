/***************************************************************************
 *   MouseOverItem.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using UltimaXNA.Ultima.World.Entities;
#endregion

namespace UltimaXNA.Ultima.World.Input
{
    public class MouseOverItem
    {
        public Point InTexturePoint;
        public AEntity Entity;

        internal MouseOverItem(Point inTexturePoint, AEntity entity)
        {
            InTexturePoint = inTexturePoint;
            Entity = entity;
        }
    }
}
