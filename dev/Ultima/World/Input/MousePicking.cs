/***************************************************************************
 *   MousePicking.cs
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
    public class MousePicking
    {
        MouseOverItem m_OverObject;
        MouseOverItem m_OverGround;

        public AEntity MouseOverObject => m_OverObject?.Entity;

        public AEntity MouseOverGround => m_OverGround?.Entity;

        public Point MouseOverObjectPoint
        {
            get { return (m_OverObject == null) ? Point.Zero : m_OverObject.InTexturePoint; }
        }

        public const PickType DefaultPickType = PickType.PickStatics | PickType.PickObjects;

        public PickType PickOnly
        {
            get; 
            set; 
        }

        public Point Position
        {
            get;
            set;
        }

        public MousePicking()
        {
            PickOnly = PickType.PickNothing;
        }

        public void UpdateOverEntities(MouseOverList list, Point mousePosition)
        {
            m_OverObject = list.GetForemostMouseOverItem(mousePosition);
            m_OverGround = list.GetForemostMouseOverItem<Ground>(mousePosition);
        }
    }
}
