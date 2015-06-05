/***************************************************************************
 *   MovementSpeed.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

namespace UltimaXNA.Ultima.Entities.Mobiles
{
    public static class MovementSpeed
    {
        private static double m_TimeWalkFoot = (8d / 20d) * 1000d;
        private static double m_TimeRunFoot = (4d / 20d) * 1000d;
        private static double m_TimeWalkMount = (4d / 20d) * 1000d;
        private static double m_TimeRunMount = (2d / 20d) * 1000d;

        public static double TimeToCompleteMove(AEntity entity, Direction facing)
        {
            if (entity is Mobile && (entity as Mobile).IsMounted)
                return (facing & Direction.Running) == Direction.Running ? m_TimeRunMount : m_TimeWalkMount;
            else
                return (facing & Direction.Running) == Direction.Running ? m_TimeRunFoot : m_TimeWalkFoot;
        }
    }
}
