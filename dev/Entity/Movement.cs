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
        #region MovementSpeed
        private static TimeSpan _TimeWalkFoot = TimeSpan.FromSeconds(0.4);
        private static TimeSpan _TimeRunFoot = TimeSpan.FromSeconds(0.2);
        private static TimeSpan _TimeWalkMount = TimeSpan.FromSeconds(0.3);
        private static TimeSpan _TimeRunMount = TimeSpan.FromSeconds(0.1);
        private TimeSpan TimeToCompleteMove(Direction facing)
        {
            if (_entity is Mobile && ((Mobile)_entity).IsMounted)
                return (facing & Direction.Running) == Direction.Running ? _TimeRunMount : _TimeWalkMount;
            else
                return (facing & Direction.Running) == Direction.Running ? _TimeRunFoot : _TimeWalkFoot;
        }
        #endregion

        public bool RequiresUpdate = false;
        public bool IsRunning { get { return ((Facing & Direction.Running) == Direction.Running); } }

        Position3D _currentPosition, _goalPosition;
        
        Direction _playerMobile_NextMove = Direction.Nothing;
        DateTime _playerMobile_NextMoveTime;
        Direction __facing = Direction.Up;
        public Direction Facing
        {
            get { return __facing; }
            set { __facing = value; }
        }

        public bool IsMoving
        {
            get
            {
                if (_goalPosition == null)
                    return false;
                if ((_currentPosition.Tile_V3 == _goalPosition.Tile_V3) &&
                    !_currentPosition.IsOffset)
                    return false;
                return true;
            }
        }

        float MoveSequence = 0f;

        internal Position3D Position { get { return _currentPosition; } }

        MoveEvents _moveEvents;
        BaseEntity _entity;

        public Movement(BaseEntity entity)
        {
            _entity = entity;
            _currentPosition = new Position3D();
            _moveEvents = new MoveEvents();
        }

        public void PlayerMobile_MoveEventAck(int nSequence)
        {
            _moveEvents.MoveRequestAcknowledge(nSequence);
        }

        public void PlayerMobile_MoveEventRej(int sequenceID, int x, int y, int z, int direction)
        {
            // immediately return to the designated tile.
            int ax, ay, az, af;
            _moveEvents.MoveRequestReject(sequenceID, out ax, out ay, out az, out af);
            Move_Instant(x, y, z, direction);
            _moveEvents.ResetMoveSequence();
        }

        public void PlayerMobile_Move(Direction facing)
        {
            if (!IsMoving)
            {
                _playerMobile_NextMove = facing;
            }
        }

        public void PlayerMobile_CheckForMoveEvent()
        {
            if (!_entity.IsClientEntity)
                return;

            if ((DateTime.Now > _playerMobile_NextMoveTime) && (_playerMobile_NextMove != Direction.Nothing))
            {
                Direction nextMove = _playerMobile_NextMove;
                _playerMobile_NextMove = Direction.Nothing;

                // _nextMove = the time we will next accept a move request from GameState
                _playerMobile_NextMoveTime = DateTime.Now + TimeToCompleteMove(nextMove);

                // get the next tile and the facing necessary to reach it.
                Direction facing;
                Vector3 nextTile = MovementCheck.OffsetTile(_currentPosition.Tile_V3, nextMove);
                Position3D nextPosition = getNextTile(_currentPosition, new Position3D(nextTile), out facing);

                // Check facing and about face if necessary.
                if ((Facing & Direction.FacingMask) != (facing & Direction.FacingMask))
                    _moveEvents.AddMoveEvent(
                        _currentPosition.X,
                        _currentPosition.Y,
                        _currentPosition.Z,
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

                    if ((_currentPosition.X != nextPosition.X) ||
                        (_currentPosition.Y != nextPosition.Y) ||
                        (_currentPosition.Z != nextPosition.Z))
                    {
                        _moveEvents.AddMoveEvent(
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
            _moveEvents.AddMoveEvent(x, y, z, facing);
        }

        public void Move_Instant(int x, int y, int z, int facing)
        {
            _moveEvents.ResetMoveSequence();
            flushDrawObjects();
            Facing = ((Direction)facing & Direction.FacingMask);
            _goalPosition = _currentPosition = new Position3D(x, y, z);
        }

        public void Update(GameTime gameTime)
        {
            flushDrawObjects();
            // Are we moving? (if our current location != our destination, then we are moving)
            if (IsMoving)
            {
                MoveSequence += ((float)(gameTime.ElapsedGameTime.TotalMilliseconds) / TimeToCompleteMove(Facing).Milliseconds);

                if (MoveSequence < 1f)
                {
                    _currentPosition.Offset_V3 = (_goalPosition.Tile_V3 - _currentPosition.Tile_V3) * MoveSequence;
                }
                else
                {
                    _currentPosition = _goalPosition;
                    MoveSequence = 0f;
                }
            }
            else
            {
                MoveEvent moveEvent;
                int sequence;
                while ((moveEvent = _moveEvents.GetMoveEvent(out sequence)) != null)
                {
                    if (_entity.IsClientEntity)
                        UltimaClient.Send(new MoveRequestPacket((byte)moveEvent.Facing, (byte)sequence, moveEvent.Fastwalk));
                    Facing = (Direction)moveEvent.Facing;
                    Position3D p = new Position3D(
                        moveEvent.X, moveEvent.Y, moveEvent.Z);
                    if (p != _currentPosition)
                    {
                        _goalPosition = p;
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
                lastTile.FlushObjectsBySerial(_entity.Serial);
        }

        private Position3D getNextTile(Position3D current, Position3D goal, out Direction facing)
        {
            facing = getNextFacing(current, goal);
            Vector3 nextTile = MovementCheck.OffsetTile(current.Tile_V3, facing);

            int nextAltitude;
            bool moveIsOkay = MovementCheck.CheckMovement((Mobile)_entity, IsometricRenderer.Map, current.Tile_V3, facing, out nextAltitude);
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
        private int _lastSequenceAck;
        private int _sequenceQueued;
        private int _sequenceNextSend;
        private int _FastWalkKey;
        MoveEvent[] _history;

        public bool SlowSync
        {
            get
            {
                if (_sequenceNextSend > _lastSequenceAck + 4)
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
            _sequenceQueued = 0;
            _lastSequenceAck = -1;
            _sequenceNextSend = 0;
            _FastWalkKey = new Random().Next(int.MinValue, int.MaxValue);
            _history = new MoveEvent[256];
        }

        public void AddMoveEvent(int x, int y, int z, int facing)
        {
            _history[_sequenceQueued] = new MoveEvent(x, y, z, facing, _FastWalkKey);
            _sequenceQueued += 1;
            if (_sequenceQueued > byte.MaxValue)
                _sequenceQueued = 1;
        }

        public MoveEvent GetMoveEvent(out int sequence)
        {
            if (_history[_sequenceNextSend] != null)
            {
                MoveEvent m = _history[_sequenceNextSend];
                _history[_sequenceNextSend] = null;
                sequence = _sequenceNextSend;
                _sequenceNextSend++;
                if (_sequenceNextSend > byte.MaxValue)
                    _sequenceNextSend = 1;
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
            _history[sequence] = null;
            _lastSequenceAck = sequence;
        }

        public void MoveRequestReject(int sequence, out int x, out int y, out int z, out int facing)
        {
            if (_history[sequence] != null)
            {
                MoveEvent e = _history[sequence];
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
