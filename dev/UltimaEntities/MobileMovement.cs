/***************************************************************************
 *   Movement.cs
 *   Based on code from RunUO: http://www.runuo.com
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using System;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.View;
#endregion

namespace UltimaXNA.UltimaEntities
{
    public class MobileMovement
    {
        public static Action<MoveRequestPacket> SendMoveRequestPacket;

        #region MovementSpeed
        private static TimeSpan m_TimeWalkFoot = TimeSpan.FromSeconds(0.4);
        private static TimeSpan m_TimeRunFoot = TimeSpan.FromSeconds(0.2);
        private static TimeSpan m_TimeWalkMount = TimeSpan.FromSeconds(0.3);
        private static TimeSpan m_TimeRunMount = TimeSpan.FromSeconds(0.1);
        private TimeSpan TimeToCompleteMove(Direction facing)
        {
            if (m_entity is Mobile && ((Mobile)m_entity).IsMounted)
                return (facing & Direction.Running) == Direction.Running ? m_TimeRunMount : m_TimeWalkMount;
            else
                return (facing & Direction.Running) == Direction.Running ? m_TimeRunFoot : m_TimeWalkFoot;
        }
        #endregion

        public bool RequiresUpdate = false;
        public bool IsRunning { get { return ((Facing & Direction.Running) == Direction.Running); } }

        protected Position3D CurrentPosition
        {
            get { return m_entity.Position; }
        }

        private Position3D m_goalPosition;
        
        Direction m_playerMobile_NextMove = Direction.Nothing;
        DateTime m_playerMobile_NextMoveTime;
        Direction m_Facing = Direction.Up;
        public Direction Facing
        {
            get { return m_Facing; }
            set { m_Facing = value; }
        }

        public bool IsMoving
        {
            get
            {
                if (m_goalPosition == null)
                    return false;
                if ((CurrentPosition.Tile == m_goalPosition.Tile) &&
                    !CurrentPosition.IsOffset)
                    return false;
                return true;
            }
        }

        float MoveSequence = 0f;

        internal Position3D Position { get { return CurrentPosition; } }

        MoveEvents m_moveEvents;
        AEntity m_entity;

        public MobileMovement(AEntity entity)
        {
            m_entity = entity;
            m_moveEvents = new MoveEvents();
        }

        public void PlayerMobile_MoveEventAck(int nSequence)
        {
            m_moveEvents.MoveRequestAcknowledge(nSequence);
        }

        public void PlayerMobile_MoveEventRej(int sequenceID, int x, int y, int z, int direction)
        {
            // immediately return to the designated tile.
            int ax, ay, az, af;
            m_moveEvents.MoveRequestReject(sequenceID, out ax, out ay, out az, out af);
            Move_Instant(x, y, z, direction);
            m_moveEvents.ResetMoveSequence();
        }

        public void PlayerMobile_Move(Direction facing)
        {
            if (!IsMoving)
            {
                m_playerMobile_NextMove = facing;
            }
        }

        public void PlayerMobile_CheckForMoveEvent()
        {
            if (!m_entity.IsClientEntity)
                return;

            if ((DateTime.Now > m_playerMobile_NextMoveTime) && (m_playerMobile_NextMove != Direction.Nothing))
            {
                Direction nextMove = m_playerMobile_NextMove;
                m_playerMobile_NextMove = Direction.Nothing;

                // m_nextMove = the time we will next accept a move request from GameState
                m_playerMobile_NextMoveTime = DateTime.Now + TimeToCompleteMove(nextMove);

                // get the next tile and the facing necessary to reach it.
                Direction facing;
                Point targetTile = MobileMovementCheck.OffsetTile(CurrentPosition, nextMove);

                int nextZ;
                Point nextTile;
                if (PlayerMobile_GetNextTile(CurrentPosition, targetTile, out facing, out nextTile, out nextZ))
                {

                    // Check facing and about face if necessary.
                    if ((Facing & Direction.FacingMask) != (facing & Direction.FacingMask))
                    {
                        m_moveEvents.AddMoveEvent(
                            CurrentPosition.X,
                            CurrentPosition.Y,
                            CurrentPosition.Z,
                            (int)(facing & Direction.FacingMask),
                            true);
                    }

                    // copy the running flag to our local facing if we are running,
                    // zero it out if we are not.
                    if ((nextMove & Direction.Running) != 0)
                        facing |= Direction.Running;
                    else
                        facing &= Direction.FacingMask;

                    if ((CurrentPosition.X != nextTile.X) ||
                        (CurrentPosition.Y != nextTile.Y) ||
                        (CurrentPosition.Z != nextZ))
                    {
                        m_moveEvents.AddMoveEvent(
                            nextTile.X,
                            nextTile.Y,
                            nextZ,
                            (int)(facing),
                            true);
                    }
                }
                else
                {
                    // blocked
                }
            }
        }

        public void Mobile_ServerAddMoveEvent(int x, int y, int z, int facing)
        {
            m_moveEvents.AddMoveEvent(x, y, z, facing, false);
        }

        public void Move_Instant(int x, int y, int z, int facing)
        {
            m_moveEvents.ResetMoveSequence();
            Facing = (Direction)facing;
            CurrentPosition.Set(x, y, z);
            m_goalPosition = null;
        }

        public void Update(double frameMS)
        {
            // Are we moving? (if our current location != our destination, then we are moving)
            if (IsMoving)
            {
                MoveSequence += ((float)(frameMS) / TimeToCompleteMove(Facing).Milliseconds);

                if (MoveSequence < 1f)
                {
                    CurrentPosition.Offset = new Vector3(
                        m_goalPosition.X - CurrentPosition.X,
                        m_goalPosition.Y - CurrentPosition.Y,
                        m_goalPosition.Z - CurrentPosition.Z) * MoveSequence;
                }
                else
                {
                    CurrentPosition.Set(m_goalPosition.X, m_goalPosition.Y, m_goalPosition.Z);
                    CurrentPosition.Offset = Vector3.Zero;
                    MoveSequence = 0f;
                }
            }
            else
            {
                MoveEvent moveEvent;
                int sequence;
                while ((moveEvent = m_moveEvents.GetMoveEvent(out sequence)) != null)
                {
                    if (m_entity.IsClientEntity && moveEvent.CreatedByPlayerInput)
                        SendMoveRequestPacket(new MoveRequestPacket((byte)moveEvent.Facing, (byte)sequence, moveEvent.Fastwalk));
                    Facing = (Direction)moveEvent.Facing;
                    Position3D p = new Position3D(
                        moveEvent.X, moveEvent.Y, moveEvent.Z);
                    if (p != CurrentPosition)
                    {
                        m_goalPosition = p;
                        return;
                    }
                }

            }
        }

        private bool PlayerMobile_GetNextTile(Position3D current, Point goal, out Direction facing, out Point nextPosition, out int nextZ)
        {
            bool moveIsOkay;

            // attempt to move in the direction specified.
            facing = getNextFacing(current, goal);
            nextPosition = MobileMovementCheck.OffsetTile(current, facing);
            moveIsOkay = MobileMovementCheck.CheckMovement((Mobile)m_entity, current, facing, out nextZ);

            // if blocked, attempt moving in the direction 1/8 counterclockwise to the direction specified.
            if (!moveIsOkay)
            {
                facing = (Direction)((facing - 1) & Direction.ValueMask);
                nextPosition = MobileMovementCheck.OffsetTile(current, facing);
                moveIsOkay = MobileMovementCheck.CheckMovement((Mobile)m_entity, current, facing, out nextZ);
            }

            // if blocked again, attempt moving in the direction 1/8 clockwise to the direction specified.
            if (!moveIsOkay)
            {
                facing = (Direction)((facing + 2) & Direction.ValueMask);
                nextPosition = MobileMovementCheck.OffsetTile(current, facing);
                moveIsOkay = MobileMovementCheck.CheckMovement((Mobile)m_entity, current, facing, out nextZ);
            }

            // if we were able to move, then set the running flag (if necessary) and return true.
            if (moveIsOkay)
            {
                if (IsRunning)
                    facing |= Direction.Running;
                return true;
            }
            else
            {
                // otherwise return false, indicating that the player is blocked.
                return false;
            }
        }

        private Direction getNextFacing(Position3D current, Point goal)
        {
            Direction facing;

            if (goal.X < current.X)
            {
                if (goal.Y < current.Y)
                    facing = Direction.Up;
                else if (goal.Y > current.Y)
                    facing = Direction.Left;
                else
                    facing = Direction.West;
            }
            else if (goal.X > current.X)
            {
                if (goal.Y < current.Y)
                    facing = Direction.Right;
                else if (goal.Y > current.Y)
                    facing = Direction.Down;
                else
                    facing = Direction.East;
            }
            else
            {
                if (goal.Y < current.Y)
                    facing = Direction.North;
                else if (goal.Y > current.Y)
                    facing = Direction.South;
                else
                {
                    // We should never reach this.
                    facing = Facing & Direction.FacingMask;
                }
            }

            return facing;
        }
    }

    

    // This class queues moves and maintains the fastwalk key and current sequence value.
    class MoveEvents
    {
        private int m_lastSequenceAck;
        private int m_sequenceQueued;
        private int m_sequenceNextSend;
        private int m_FastWalkKey;
        MoveEvent[] m_history;

        public bool SlowSync
        {
            get
            {
                if (m_sequenceNextSend > m_lastSequenceAck + 4)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public MoveEvents()
        {
            ResetMoveSequence();
        }

        public void ResetMoveSequence()
        {
            m_sequenceQueued = 0;
            m_lastSequenceAck = -1;
            m_sequenceNextSend = 0;
            m_FastWalkKey = new Random().Next(int.MinValue, int.MaxValue);
            m_history = new MoveEvent[256];
        }

        public void AddMoveEvent(int x, int y, int z, int facing, bool createdByPlayerInput)
        {
            MoveEvent moveEvent = new MoveEvent(x, y, z, facing, m_FastWalkKey);
            moveEvent.CreatedByPlayerInput = createdByPlayerInput;

            m_history[m_sequenceQueued] = moveEvent;

            m_sequenceQueued += 1;
            if (m_sequenceQueued > byte.MaxValue)
                m_sequenceQueued = 1;
        }

        public MoveEvent GetMoveEvent(out int sequence)
        {
            if (m_history[m_sequenceNextSend] != null)
            {
                MoveEvent m = m_history[m_sequenceNextSend];
                m_history[m_sequenceNextSend] = null;
                sequence = m_sequenceNextSend;
                m_sequenceNextSend++;
                if (m_sequenceNextSend > byte.MaxValue)
                    m_sequenceNextSend = 1;
                return m;
            }
            else
            {
                sequence = 0;
                return null;
            }
        }

        public void MoveRequestAcknowledge(int sequence)
        {
            m_history[sequence] = null;
            m_lastSequenceAck = sequence;
        }

        public void MoveRequestReject(int sequence, out int x, out int y, out int z, out int facing)
        {
            if (m_history[sequence] != null)
            {
                MoveEvent e = m_history[sequence];
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

    class MoveEvent
    {
        public bool CreatedByPlayerInput = false;
        public readonly int X, Y, Z, Facing, Fastwalk;
        public MoveEvent(int x, int y, int z, int facing, int fastwalk)
        {
            X = x;
            Y = y;
            Z = z;
            Facing = facing;
            Fastwalk = fastwalk;
        }
    }
}
