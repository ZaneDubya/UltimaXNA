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
using System;
using UltimaXNA.Ultima.Entities;
#endregion

namespace UltimaXNA.Ultima.World.Controllers
{
    public class MousePicking
    {
        private MouseOverItem m_overObject, m_overGround;

        public AEntity MouseOverObject
        {
            get { return (m_overObject == null) ? null : m_overObject.Entity; }
        }

        public AEntity MouseOverGround
        {
            get { return (m_overGround == null) ? null : m_overGround.Entity; }
        }

        public Vector2 MouseOverObjectPoint
        {
            get { return (m_overObject == null) ? new Vector2(0, 0) : m_overObject.InTexturePosition; }
        }

        public const PickType DefaultPickType = PickType.PickStatics | PickType.PickObjects;

        public PickType PickOnly
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
            m_overObject = list.GetForemostMouseOverItem(mousePosition);
            m_overGround = list.GetForemostMouseOverItem<Ground>(mousePosition);
        }
    }
}
