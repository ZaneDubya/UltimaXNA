/***************************************************************************
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
        public static float WalkFoot { get { return 400f / 1000f; } }
        public static float RunFoot { get { return 200f / 1000f; } }
        public static float WalkMount { get { return 200f / 1000f; } }
        public static float RunMount { get { return 100f / 1000f; } }
        private float TimeToCompleteMove
        {
            get
            {
                if (IsMounted)
                    return (_facing & Direction.Running) == Direction.Running ? RunMount : WalkMount;
                else
                    return (_facing & Direction.Running) == Direction.Running ? RunFoot : WalkFoot;
            }
        }
        #endregion

        public bool RequiresUpdate = false;
        // booleans
        public bool IsMounted;
        public bool IsRunning { get { return ((_facing & Direction.Running) == Direction.Running); } }

        Position3D _lastTile, _currentTile, _nextTile, _goalPosition;
        Direction _facing = Direction.Up;
        Direction _queuedFacing = Direction.Nothing;
        public Direction Facing
        {
            get { return _facing; }
            set
            {
                if (IsMoving)
                {
                    _queuedFacing = value;
                }
                else
                {
                    _facing = value;
                }
            }
        }

        float MoveSequence = 0f;

        internal Position3D Position { get { return _currentTile; } }
        
        MoveEvent _moveEvent;
        MoveEvent moveEvent
        {
            get
            {
                if (_moveEvent == null)
                    _moveEvent = new MoveEvent();
                return _moveEvent;
            }
        }

        private Entity _entity;
        private IWorld _world;

        public Movement(Entity entity, IWorld world)
        {
            _entity = entity;
            _world = world;
            _currentTile = new Position3D();
        }

        public void NewFacingToServer(Direction nDirection)
        {
            _facing = nDirection;
            moveEvent.NewEvent(this._facing);
        }

        public bool GetMoveEvent(ref int direction, ref int sequence, ref int fastwalkkey)
        {
            if (!_entity.IsClientEntity)
                return false;
            return moveEvent.GetMoveEvent(ref direction, ref sequence, ref fastwalkkey);
        }

        public void MoveEventAck(int nSequence)
        {
            // do nothing
        }

        public void MoveEventRej(int sequenceID, int x, int y, int z, int direction)
        {
            // immediately return to the designated tile.
            MoveToInstant(x, y, z, direction);
            moveEvent.ResetMoveSequence();
        }

        public void Move(Direction facing)
        {
            if (!IsMoving)
            {
                // we need to transfer over the running flag to our local copy.
                // copy over running flag if it exists, zero it out if it doesn't
                if ((facing & Direction.Running) != 0)
                    this.Facing |= Direction.Running;
                else
                    this.Facing &= Direction.FacingMask;
                // now get the goal tile.
                Vector3 position = MovementCheck.OffsetTile(_currentTile.Point, facing);
                MoveTo(MovementCheck.OffsetTile(_currentTile.Point, facing));
            }
        }

        public bool IsMoving
        {
            get
            {
                if (_goalPosition == null)
                    return false;
                if ((_currentTile.X == _goalPosition.X) &&
                    (_currentTile.Y == _goalPosition.Y) &&
                    !_currentTile.IsOffset)
                    return false;
                return true;
            }
        }

        public void MoveTo(int x, int y, int z, int facing)
        {
            if (facing != -1)
            {
                mFlushDrawObjects();
                _facing = (Direction)facing;
            }
            _goalPosition = new Position3D(x, y, z);
        }

        public void MoveTo(Vector3 v)
        {
            _goalPosition = new Position3D(v);
        }

        public void MoveToInstant(int x, int y, int z, int facing)
        {
            if (facing != -1)
            {
                mFlushDrawObjects();
                _facing = (Direction)facing;
            }
            _goalPosition = _lastTile = _nextTile = _currentTile = new Position3D(x, y, z);
        }

        public void Update(GameTime gameTime)
        {
            mFlushDrawObjects();
            
            // Are we moving? (if our current location != our destination, then we are moving)
            if (IsMoving)
            {
                // UO movement is tile based. Have we reached the next tile yet?
                // If we have, then get the next tile in the move sequence and move to that one.
                if (_currentTile.Point == _nextTile.Point)
                {
                    Direction iFacing;
                    _nextTile = getNextTile(_currentTile, _goalPosition, out iFacing);
                    // Is the next tile on-screen?
                    if (_nextTile != null)
                    {
                        // If we are the player, set our move event so that the game
                        // knows to send a move request to the server ...
                        if (_entity.IsClientEntity)
                        {
                            // Special exception for the player: if we are facing a new direction, we
                            // need to pause for a brief moment and let the server know that.
                            if ((_facing & Direction.FacingMask) != (iFacing & Direction.FacingMask))
                            {
                                moveEvent.NewEvent((iFacing & Direction.FacingMask));
                                _facing = iFacing;
                                _nextTile.Point = _currentTile.Point;
                                return;
                            }
                            else
                            {
                                moveEvent.NewEvent(iFacing);
                            }
                        }
                        else
                        {
                            // if we are not the player, then we can just change the facing.
                            _facing = iFacing;
                        }
                        _lastTile = new Position3D(_currentTile.X, _currentTile.Y, _currentTile.Z);
                    }
                    else
                    {
                        // This next tile is no longer in our map in memory.
                        // Ideally, we should remove them from the map entirely.
                        // Right now, we just set their location to the current tile
                        // and refuse to move them further.
                        MoveToInstant(_currentTile.X, _currentTile.Y, _currentTile.Z, (int)_facing);
                        return;
                    }
                }

                MoveSequence += ((float)(gameTime.ElapsedRealTime.TotalMilliseconds / 1000) / TimeToCompleteMove);
                if (MoveSequence >= 1f)
                {
                    MoveSequence -= 1f;
                    _currentTile.Point = _nextTile.Point;
                }
                else
                {
                    _currentTile.Point = _lastTile.Point + (_nextTile.Point - _lastTile.Point) * MoveSequence;
                }
            }
            else
            {
                // we have reached our destination :)
                if (_queuedFacing != Direction.Nothing)
                {
                    _facing = _queuedFacing; // Occasionally we will have a queued facing for monsters.
                    _queuedFacing = Direction.Nothing;
                }
                MoveSequence = 0f;
            }
        }

        public void ClearImmediate()
        {
            mFlushDrawObjects();
        }

        private void mFlushDrawObjects()
        {
            if (Position.IsNullPosition)
                return;
            if (_world.Map == null)
                return;
            TileEngine.MapTile lastTile = _world.Map.GetMapTile(Position.TileX, Position.TileY, false);
            if (lastTile != null)
                lastTile.FlushObjectsBySerial(_entity.Serial);
        }

        private Position3D getNextTile(Position3D current, Position3D goal, out Direction facing)
        {
            facing = getNextFacing(current, goal);
            Vector3 nextTile = MovementCheck.OffsetTile(current.Point, facing);

            int nextAltitude;
            bool moveIsOkay = MovementCheck.CheckMovement((Mobile)_entity, _world.Map, current.Point, facing, out nextAltitude);
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
                    facing = (Direction)0xFF;
                }
            }

            return facing;
        }
    }

    public class Position3D
    {
        public static Vector3 NullPosition = new Vector3(-1);

        Vector3 _position;
        public Vector3 Point { get { return _position; } set { _position = value; } }
        public int X { get { return (int)_position.X; } set { _position.X = value; } }
        public int Y { get { return (int)_position.Y; } set { _position.Y = value; } }
        public int Z {
            get
            {
                return (int)_position.Z;
            } 
            set
            {
                _position.Z = value;
            }
        }

        public int TileX { get { return (Xoffset != 0) ? X + 1 : X; } }
        public int TileY { get { return (Yoffset != 0) ? Y + 1 : Y; } }
        public float Xoffset { get { return _position.X % 1.0f; } }
        public float Yoffset { get { return _position.Y % 1.0f; } }
        public float Zoffset { get { return _position.Z % 1.0f; } }
        public float Xoffset_Draw { get { return (Xoffset == 0) ? 0 : Xoffset - 1f; } }
        public float Yoffset_Draw { get { return (Yoffset == 0) ? 0 : Yoffset - 1f; } }
        public float Zoffset_Draw { get { return Zoffset; } }

        public bool IsOffset { get { return (Xoffset != 0) || (Yoffset != 0) || (Zoffset != 0); } }
        public bool IsNullPosition { get { return _position == NullPosition; } }


        public Position3D()
        {
            _position = NullPosition;
        }

        public Position3D(int x, int y, int z)
        {
            _position = new Vector3(x, y, z);
        }

        public Position3D(Vector3 v)
        {
            _position = v;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(Position3D)) return false;
            if (this.X != ((Position3D)o).X) return false;
            if (this.Y != ((Position3D)o).Y) return false;
            if (this.Z != ((Position3D)o).Z) return false;
            return true;
        }

        // Equality operator. Returns dbNull if either operand is dbNull, 
        // otherwise returns dbTrue or dbFalse:
        public static bool operator ==(Position3D x, Position3D y)
        {
            if ((object)x == null)
                return ((object)y == null);
            return x.Equals(y);
        }

        // Inequality operator. Returns dbNull if either operand is
        // dbNull, otherwise returns dbTrue or dbFalse:
        public static bool operator !=(Position3D x, Position3D y)
        {
            if ((object)x == null)
                return ((object)y != null);
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
            return X ^ Y ^ Z;
        }

        public override string ToString()
        {
            return string.Format("X:{0} Y:{1} Z:{2}", X, Y, Z);
        }
    }

    // This event handles all the move sequences
    public class MoveEvent
    {
        private bool _Event;
        private int _Sequence;
        private Direction _Direction;
        private int _FastWalkKey;

        public MoveEvent()
        {
            _Event = false;
            _Direction = Direction.Up;
            _FastWalkKey = new Random().Next(int.MinValue, int.MaxValue);
            this.ResetMoveSequence();
        }

        public void ResetMoveSequence()
        {
            _Sequence = -1;
        }

        public void NewEvent(Direction nDirection)
        {
            _Event = true;
            _Sequence++;
            if (_Sequence > byte.MaxValue)
                _Sequence = 1;
            _Direction = nDirection;

            if (_FastWalkKey == int.MaxValue)
                _FastWalkKey = new Random().Next(int.MinValue, int.MaxValue);
            else
                _FastWalkKey++;
            
        }

        public bool GetMoveEvent(ref int direction, ref int sequence, ref int fastwalkkey)
        {
            if (_Event == false)
            {
                return false;
            }
            else
            {
                _Event = false;
                sequence = _Sequence;
                fastwalkkey = _FastWalkKey;
                direction = (int)_Direction;
                return true;
            }
        }
    }
}
