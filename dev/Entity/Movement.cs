/***************************************************************************
 *   Movement.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from RunUO: http://www.runuo.com
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld;
using UltimaXNA.Core.Network;
using UltimaXNA.UltimaPackets.Client;
using UltimaXNA.UltimaPackets;
#endregion

namespace UltimaXNA.Entity
{
    public class Movement
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

        Position3D m_currentPosition, m_goalPosition;
        
        Direction m_playerMobile_NextMove = Direction.Nothing;
        DateTime m_playerMobile_NextMoveTime;
        Direction m__facing = Direction.Up;
        public Direction Facing
        {
            get { return m__facing; }
            set { m__facing = value; }
        }

        public bool IsMoving
        {
            get
            {
                if (m_goalPosition == null)
                    return false;
                if ((m_currentPosition.Tile_V3 == m_goalPosition.Tile_V3) &&
                    !m_currentPosition.IsOffset)
                    return false;
                return true;
            }
        }

        float MoveSequence = 0f;

        internal Position3D Position { get { return m_currentPosition; } }

        MoveEvents m_moveEvents;
        BaseEntity m_entity;

        public Movement(BaseEntity entity)
        {
            m_entity = entity;
            m_currentPosition = new Position3D();
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
                Vector3 nextTile = MovementCheck.OffsetTile(m_currentPosition.Tile_V3, nextMove);
                Position3D nextPosition = getNextTile(m_currentPosition, new Position3D(nextTile), out facing);

                // Check facing and about face if necessary.
                if ((Facing & Direction.FacingMask) != (facing & Direction.FacingMask))
                    m_moveEvents.AddMoveEvent(
                        m_currentPosition.X,
                        m_currentPosition.Y,
                        m_currentPosition.Z,
                        (int)(facing & Direction.FacingMask));

                // if nextPosition is false, then we are blocked
                if (nextPosition != null)
                {
                    // copy the running flag to our local facing if we are running,
                    // zero it out if we are not.
                    if ((nextMove & Direction.Running) != 0)
                        facing |= Direction.Running;
                    else
                        facing &= Direction.FacingMask;

                    if ((m_currentPosition.X != nextPosition.X) ||
                        (m_currentPosition.Y != nextPosition.Y) ||
                        (m_currentPosition.Z != nextPosition.Z))
                    {
                        m_moveEvents.AddMoveEvent(
                            nextPosition.X, 
                            nextPosition.Y, 
                            nextPosition.Z, 
                            (int)(facing));
                    }
                }
            }
        }

        public void Mobile_AddMoveEvent(int x, int y, int z, int facing)
        {
            m_moveEvents.AddMoveEvent(x, y, z, facing);
        }

        public void Move_Instant(int x, int y, int z, int facing)
        {
            m_moveEvents.ResetMoveSequence();
            flushDrawObjects();
            Facing = ((Direction)facing & Direction.FacingMask);
            m_goalPosition = m_currentPosition = new Position3D(x, y, z);
        }

        public void Update(double frameMS)
        {
            flushDrawObjects();
            // Are we moving? (if our current location != our destination, then we are moving)
            if (IsMoving)
            {
                MoveSequence += ((float)(frameMS) / TimeToCompleteMove(Facing).Milliseconds);

                if (MoveSequence < 1f)
                {
                    m_currentPosition.Offset_V3 = (m_goalPosition.Tile_V3 - m_currentPosition.Tile_V3) * MoveSequence;
                }
                else
                {
                    m_currentPosition = m_goalPosition;
                    MoveSequence = 0f;
                }
            }
            else
            {
                MoveEvent moveEvent;
                int sequence;
                while ((moveEvent = m_moveEvents.GetMoveEvent(out sequence)) != null)
                {
                    if (m_entity.IsClientEntity)
                        SendMoveRequestPacket(new MoveRequestPacket((byte)moveEvent.Facing, (byte)sequence, moveEvent.Fastwalk));
                    Facing = (Direction)moveEvent.Facing;
                    Position3D p = new Position3D(
                        moveEvent.X, moveEvent.Y, moveEvent.Z);
                    if (p != m_currentPosition)
                    {
                        m_goalPosition = p;
                        return;
                    }
                }
                
            }
        }

        public void ClearImmediate()
        {
            flushDrawObjects();
        }

        private void flushDrawObjects()
        {
            if (Position.IsNullPosition)
                return;
            if (IsometricRenderer.Map == null)
                return;
            MapTile lastTile = IsometricRenderer.Map.GetMapTile(Position.X, Position.Y, false);
            if (lastTile != null)
                lastTile.FlushObjectsBySerial(m_entity.Serial);
        }

        private Position3D getNextTile(Position3D current, Position3D goal, out Direction facing)
        {
            facing = getNextFacing(current, goal);
            Vector3 nextTile = MovementCheck.OffsetTile(current.Tile_V3, facing);

            int nextAltitude;
            bool moveIsOkay = MovementCheck.CheckMovement((Mobile)m_entity, IsometricRenderer.Map, current.Tile_V3, facing, out nextAltitude);
            nextTile.Z = nextAltitude;
            if (moveIsOkay)
            {
                if (IsRunning)
                    facing |= Direction.Running;
                return new Position3D(nextTile);
            }
            else
            {
                return null;
            }
        }

        private Direction getNextFacing(Position3D current, Position3D goal)
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

        public void AddMoveEvent(int x, int y, int z, int facing)
        {
            m_history[m_sequenceQueued] = new MoveEvent(x, y, z, facing, m_FastWalkKey);
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
