using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
