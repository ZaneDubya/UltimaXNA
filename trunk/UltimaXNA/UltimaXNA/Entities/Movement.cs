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

        Position3D _currentPosition, _goalPosition;
        
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

        internal Position3D Position { get { return _currentPosition; } }
        
        MoveEvents _moveEvents;
        MoveEvents moveEvents
        {
            get
            {
                if (_moveEvents == null)
                    _moveEvents = new MoveEvents();
                return _moveEvents;
            }
        }

        private Entity _entity;
        private IWorld _world;

        public Movement(Entity entity, IWorld world)
        {
            _entity = entity;
            _world = world;
            _currentPosition = new Position3D();
        }

        public void NewFacingToServer(Direction nDirection)
        {
            _facing = nDirection;
            moveEvents.NewEvent(this._facing);
        }

        public bool GetMoveEvent(ref int direction, ref int sequence, ref int fastwalkkey)
        {
            if (!_entity.IsClientEntity)
                return false;
            return moveEvents.GetMoveEvent(ref direction, ref sequence, ref fastwalkkey);
        }

        public void MoveEventAck(int nSequence)
        {
            // do nothing
        }

        public void MoveEventRej(int sequenceID, int x, int y, int z, int direction)
        {
            // immediately return to the designated tile.
            MoveToInstant(x, y, z, direction);
            moveEvents.ResetMoveSequence();
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
                Vector3 next = MovementCheck.OffsetTile(_currentPosition.Tile_V3, facing);
                MoveTo(next, (int)facing);
            }
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

        public void MoveTo(Vector3 v, int facing)
        {
            MoveTo((int)v.X, (int)v.Y, (int)v.Z, facing);
        }

        public void MoveTo(int x, int y, int z, int facing)
        {
            Direction iFacing;
            _goalPosition = getNextTile(_currentPosition, new Position3D(x, y, z), out iFacing);

            // If we are the player, set our move event so that the game
            // knows to send a move request to the server ...
            if (_entity.IsClientEntity)
            {
                // Special exception for the player: if we are facing a new direction, we
                // need to pause for a brief moment and let the server know that.
                if ((_facing & Direction.FacingMask) != ((Direction)facing & Direction.FacingMask))
                {
                    moveEvents.NewEvent(((Direction)facing & Direction.FacingMask));
                }
                moveEvents.NewEvent((Direction)facing);
            }

            _facing = (Direction)facing;
        }

        public void MoveToInstant(int x, int y, int z, int facing)
        {
            mFlushDrawObjects();
            _facing = ((Direction)facing & Direction.FacingMask);
            _goalPosition = _currentPosition = new Position3D(x, y, z);
        }

        public void Update(GameTime gameTime)
        {
            mFlushDrawObjects();
            
            // Are we moving? (if our current location != our destination, then we are moving)
            if (IsMoving)
            {
                MoveSequence += ((float)(gameTime.ElapsedGameTime.TotalMilliseconds / 1000) / TimeToCompleteMove);

                if (MoveSequence < 1f)
                {
                    _currentPosition.Offset_V3 = (_goalPosition.Tile_V3 - _currentPosition.Tile_V3) * MoveSequence; // _currentTile.Point = _lastTile.Point + (_nextTile.Point - _lastTile.Point) * MoveSequence;
                }
                else
                {
                    _currentPosition = _goalPosition;
                    // we have reached our destination :)
                    if (_queuedFacing != Direction.Nothing)
                    {
                        _facing = _queuedFacing; // Occasionally we will have a queued facing for monsters.
                        _queuedFacing = Direction.Nothing;
                    }
                    MoveSequence = 0f;
                }
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
                    facing = _facing & Direction.FacingMask;
                }
            }

            return facing;
        }
    }

    public class Position3D
    {
        public static Vector3 NullPosition = new Vector3(-1);

        Vector3 _tile;
        Vector3 _offset;

        public Vector3 Tile_V3 { get { return _tile; } set { _tile = value; } }
        public Vector3 Offset_V3 { get { return _offset; } set { _offset = value; } }
        public Vector3 Point_V3 { get { return _tile + _offset; } }

        public bool IsOffset { get { return (X_offset != 0) || (Y_offset != 0) || (Z_offset != 0); } }
        public bool IsNullPosition { get { return _tile == NullPosition; } }

        public int X { get { return (int)_tile.X; } set { _tile.X = value; } }
        public int Y { get { return (int)_tile.Y; } set { _tile.Y = value; } }
        public int Z { get { return (int)_tile.Z; } set { _tile.Z = value; } }
        float X_offset { get { return _offset.X % 1.0f; } }
        float Y_offset { get { return _offset.Y % 1.0f; } }
        float Z_offset { get { return _offset.Z % 1.0f; } }

        public int Draw_TileX { get { return drawOffsetTile(X, X_offset); } }
        public int Draw_TileY { get { return drawOffsetTile(Y, Y_offset); } }
        public float Draw_Xoffset { get { return drawOffsetOffset(X_offset); } }
        public float Draw_Yoffset { get { return drawOffsetOffset(Y_offset); } }
        public float Draw_Zoffset { get { return Z_offset; } }

        int drawOffsetTile(int tile, float offset)
        {
            return (offset > 0) ? tile + 1 : tile;
        }

        float drawOffsetOffset(float offset)
        {
            if (offset > 0)
                return offset - 1f;
            else
                return offset;
            // return (offset == 0) ? 0 : offset - 1f; 
        }

        public Position3D()
        {
            _tile = NullPosition;
        }

        public Position3D(int x, int y, int z)
        {
            _tile = new Vector3(x, y, z);
        }

        public Position3D(Vector3 v)
        {
            _tile = v;
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

        public string ToStringComplex()
        {
            return
                "PT=" + ToString() + Environment.NewLine +
                "PO=" + string.Format("X:{0} Y:{1} Z:{2}", X_offset, Y_offset, Z_offset) + Environment.NewLine +
                "DT=" + string.Format("X:{0} Y:{1} Z:{2}", Draw_TileX, Draw_TileY, Z) + Environment.NewLine + 
                "D=" + string.Format("X:{0} Y:{1} Z:{2}", Draw_Xoffset, Draw_Yoffset, Draw_Zoffset);
        }
    }

    // This event handles all the move sequences
    public class MoveEvents
    {
        private int _NextSequence;
        private int _FastWalkKey;
        List<MoveEventSingle> _events;

        public MoveEvents()
        {
            _events = new List<MoveEventSingle>();
            _FastWalkKey = new Random().Next(int.MinValue, int.MaxValue);
            ResetMoveSequence();
        }

        public void ResetMoveSequence()
        {
            _NextSequence = 0;
        }

        public void NewEvent(Direction nDirection)
        {
            int sequence = _NextSequence++;
            if (_NextSequence > byte.MaxValue)
                ResetMoveSequence();
            int direction = (int)nDirection;
            int fastWalk = (_FastWalkKey == int.MaxValue) ? new Random().Next(int.MinValue, int.MaxValue) : _FastWalkKey++;

            _events.Add(new MoveEventSingle(sequence, direction, fastWalk));
        }

        public bool GetMoveEvent(ref int direction, ref int sequence, ref int fastwalkkey)
        {
            if (_events.Count == 0)
            {
                return false;
            }
            else
            {
                direction = _events[0].Direction;
                sequence = _events[0].Sequence;
                fastwalkkey = _events[0].Fastwalk;
                _events.RemoveAt(0);
                return true;
            }
        }
    }

    class MoveEventSingle
    {
        public readonly int Sequence;
        public readonly int Direction;
        public readonly int Fastwalk;

        public MoveEventSingle(int sequence, int direction, int fastwalk)
        {
            Sequence = sequence;
            Direction = direction;
            Fastwalk = fastwalk;
        }
    }
}
