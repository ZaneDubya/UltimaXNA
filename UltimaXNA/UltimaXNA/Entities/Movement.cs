﻿/***************************************************************************
 *   Movement.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   Based on code from RunUO: http://www.runuo.com
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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
using UltimaXNA.TileEngine;
#endregion

namespace UltimaXNA.Entities
{
    public class Movement
    {
        #region MovementSpeed
        private static TimeSpan m_WalkFoot = TimeSpan.FromSeconds(0.4);
        private static TimeSpan m_RunFoot = TimeSpan.FromSeconds(0.2);
        private static TimeSpan m_WalkMount = TimeSpan.FromSeconds(0.2);
        private static TimeSpan m_RunMount = TimeSpan.FromSeconds(0.1);
        private TimeSpan TimeToCompleteMove(Direction facing)
        {
            if (IsMounted)
                return (facing & Direction.Running) == Direction.Running ? m_RunMount : m_WalkMount;
            else
                return (facing & Direction.Running) == Direction.Running ? m_RunFoot : m_WalkFoot;
        }
        #endregion

        public bool RequiresUpdate = false;
        public bool IsMounted;
        public bool IsRunning { get { return ((_facing & Direction.Running) == Direction.Running); } }

        Position3D _currentPosition, _goalPosition;

        public static Diagnostics.Logger _log = new Diagnostics.Logger("Movement");

        
        Direction _playerMobile_NextMove = Direction.Nothing;
        Direction _facing = Direction.Up;
        public Direction Facing
        {
            get { return _facing; }
            set { _facing = value; }
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

        moveEventsQueue _moveEvents;
        DateTime _nextMove;
        Entity _entity;
        IWorld _world;

        public Movement(Entity entity, IWorld world)
        {
            _entity = entity;
            _world = world;
            _currentPosition = new Position3D();
            _moveEvents = new moveEventsQueue();
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

        public bool PlayerMobile_GetMoveEvent(ref int direction, ref int sequence, ref int fastwalkkey)
        {
            if (!_entity.IsClientEntity)
                return false;

            if ((DateTime.Now > _nextMove) && (_playerMobile_NextMove != Direction.Nothing))
            {
                // log some debug information ...
                _log.Debug("Move: " + DateTime.Now.Millisecond.ToString());
                if (_moveEvents.SlowSync)
                    _log.Debug("Move:Slow!");

                // _nextMove = the time we will next accept a move request from GameState
                _nextMove = DateTime.Now + TimeToCompleteMove(_playerMobile_NextMove);
                
                // copy the running flag to our local facing if we are running, zero it out if we are not.
                if ((_playerMobile_NextMove & Direction.Running) != 0)
                    this.Facing |= Direction.Running;
                else
                    this.Facing &= Direction.FacingMask;
                // now get the goal tile.
                Vector3 next = MovementCheck.OffsetTile(_currentPosition.Tile_V3, _playerMobile_NextMove);
                MoveToGoalTile((int)next.X, (int)next.Y, (int)next.Z);
                _playerMobile_NextMove = Direction.Nothing;
            }

            bool isMoveEvent = _moveEvents.GetMoveEvent(ref direction, ref sequence, ref fastwalkkey);
            return isMoveEvent;
        }

        public void MoveToGoalTile(int x, int y, int z)
        {
            Direction facing;
            _goalPosition = getNextTile(_currentPosition, new Position3D(x, y, z), out facing);

            // If we are the player, set our move event - the game will send a move msg to the server.
            // If _goalPosition is null, then the requested move is blocked and we will not send a move msg.
            if (_goalPosition != null)
            {
                // if we are facing a new direction, we need to pause for a brief moment and let the server know that.
                if ((this.Facing & Direction.FacingMask) != ((Direction)facing & Direction.FacingMask))
                {
                    if (_entity.IsClientEntity)
                        _moveEvents.AddMoveEvent(_currentPosition.X, _currentPosition.Y, _currentPosition.Z, (int)((Direction)facing & Direction.FacingMask));
                    this.Facing = (Direction)facing;
                }
                if (_entity.IsClientEntity)
                    _moveEvents.AddMoveEvent(_currentPosition.X, _currentPosition.Y, _currentPosition.Z, (int)((Direction)facing));
            }
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
        }

        public void ClearImmediate()
        {
            flushDrawObjects();
        }

        private void flushDrawObjects()
        {
            if (Position.IsNullPosition)
                return;
            if (_world.Map == null)
                return;
            TileEngine.MapTile lastTile = _world.Map.GetMapTile(Position.Draw_TileX, Position.Draw_TileY, false);
            if (lastTile != null)
                lastTile.FlushObjectsBySerial(_entity.Serial);
        }

        private Position3D getNextTile(Position3D current, Position3D goal, out Direction facing)
        {
            facing = getNextFacing(current, goal);
            Vector3 nextTile = MovementCheck.OffsetTile(current.Tile_V3, facing);

            int nextAltitude;
            bool moveIsOkay = MovementCheck.CheckMovement((Mobile)_entity, _world.Map, current.Tile_V3, facing, out nextAltitude);
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
    class moveEventsQueue
    {
        private int _lastSequenceAck;
        private int _sequenceQueued;
        private int _sequenceNextSend;
        private int _FastWalkKey;
        moveEventsHistory_Entry[] _history;

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

        public moveEventsQueue()
        {
            ResetMoveSequence();
        }

        public void ResetMoveSequence()
        {
            _sequenceQueued = 0;
            _lastSequenceAck = -1;
            _sequenceNextSend = 0;
            _FastWalkKey = new Random().Next(int.MinValue, int.MaxValue);
            _history = new moveEventsHistory_Entry[256];
        }

        public void AddMoveEvent(int x, int y, int z, int direction)
        {
            _history[_sequenceQueued] = new moveEventsHistory_Entry(x, y, z, direction, _FastWalkKey);
            _sequenceQueued += 1;
            if (_sequenceQueued > byte.MaxValue)
                _sequenceQueued = 1;
        }

        public bool GetMoveEvent(ref int direction, ref int sequence, ref int fastwalkkey)
        {
            if (_history[_sequenceNextSend] != null)
            {
                Movement._log.Debug("M->S: " + DateTime.Now.Millisecond.ToString());
                direction = _history[_sequenceNextSend].Facing;
                sequence = _sequenceNextSend;
                fastwalkkey = _history[_sequenceNextSend].Fastwalk;
                _sequenceNextSend++;
                if (_sequenceNextSend > byte.MaxValue)
                    _sequenceNextSend = 1;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MoveRequestAcknowledge(int sequence)
        {
            Movement._log.Debug("@Ack: " + DateTime.Now.Millisecond.ToString());
            _history[sequence] = null;
            _lastSequenceAck = sequence;
        }

        public void MoveRequestReject(int sequence, out int x, out int y, out int z, out int facing)
        {
            Movement._log.Debug("@Rej: " + DateTime.Now.Millisecond.ToString());
            if (_history[sequence] != null)
            {
                moveEventsHistory_Entry e = _history[sequence];
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

        class moveEventsHistory_Entry
        {
            public readonly int X, Y, Z, Facing, Fastwalk;
            public moveEventsHistory_Entry(int x, int y, int z, int facing, int fastwalk)
            {
                X = x;
                Y = y;
                Z = z;
                Facing = facing;
                Fastwalk = fastwalk;
            }
        }
    }
}