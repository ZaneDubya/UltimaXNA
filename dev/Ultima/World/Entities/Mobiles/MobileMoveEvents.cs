using System;

namespace UltimaXNA.Ultima.World.Entities.Mobiles
{
    /// <summary>
    /// Queues moves and maintains the fastwalk key and current sequence value.
    /// </summary>
    class MobileMoveEvents
    {
        private int m_LastSequenceAck;
        private int m_SequenceQueued;
        private int m_SequenceNextSend;
        private int m_FastWalkKey;
        MobileMoveEvent[] m_History;

        public bool SlowSync
        {
            get
            {
                if (m_SequenceNextSend > m_LastSequenceAck + 4)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public MobileMoveEvents()
        {
            ResetMoveSequence();
        }

        public void ResetMoveSequence()
        {
            m_SequenceQueued = 0;
            m_LastSequenceAck = -1;
            m_SequenceNextSend = 0;
            m_FastWalkKey = new Random().Next(int.MinValue, int.MaxValue);
            m_History = new MobileMoveEvent[256];
        }

        public void AddMoveEvent(int x, int y, int z, int facing, bool createdByPlayerInput)
        {
            MobileMoveEvent moveEvent = new MobileMoveEvent(x, y, z, facing, m_FastWalkKey);
            moveEvent.CreatedByPlayerInput = createdByPlayerInput;

            m_History[m_SequenceQueued] = moveEvent;

            m_SequenceQueued += 1;
            if (m_SequenceQueued > byte.MaxValue)
                m_SequenceQueued = 1;
        }

        public MobileMoveEvent GetMoveEvent(out int sequence)
        {
            if (m_History[m_SequenceNextSend] != null)
            {
                MobileMoveEvent m = m_History[m_SequenceNextSend];
                m_History[m_SequenceNextSend] = null;
                sequence = m_SequenceNextSend;
                m_SequenceNextSend++;
                if (m_SequenceNextSend > byte.MaxValue)
                    m_SequenceNextSend = 1;
                return m;
            }
            else
            {
                sequence = 0;
                return null;
            }
        }

        public MobileMoveEvent GetFinalMoveEvent(out int sequence)
        {
            MobileMoveEvent moveEvent = null, moveEventNext;

            while ((moveEventNext = GetMoveEvent(out sequence)) != null)
            {
                moveEvent = moveEventNext;
            }
            return moveEvent;
        }

        public void MoveRequestAcknowledge(int sequence)
        {
            m_History[sequence] = null;
            m_LastSequenceAck = sequence;
        }

        public void MoveRequestReject(int sequence, out int x, out int y, out int z, out int facing)
        {
            if (m_History[sequence] != null)
            {
                MobileMoveEvent e = m_History[sequence];
                x = e.X;
                y = e.Y;
                z = e.Z;
                facing = e.Facing;
            }
            else
            {
                x = y = z = facing = -1;
            }
            ResetMoveSequence();
        }
    }

    class MobileMoveEvent
    {
        public bool CreatedByPlayerInput = false;
        public readonly int X, Y, Z, Facing, Fastwalk;
        public MobileMoveEvent(int x, int y, int z, int facing, int fastwalk)
        {
            X = x;
            Y = y;
            Z = z;
            Facing = facing;
            Fastwalk = fastwalk;
        }
    }
}
