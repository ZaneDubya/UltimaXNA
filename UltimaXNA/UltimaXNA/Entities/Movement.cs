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
        private bool _IsClientPlayer = false;
        // booleans
        public bool IsMounted;
        public bool IsRunning { get { return ((_facing & Direction.Running) == Direction.Running); } }

        private TilePosition _LastTile, _CurrentTile, _NextTile, _GoalTile;
        private Direction _facing = Direction.Up;
        private Direction _queuedFacing = Direction.Nothing;
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
        
        public float MoveSequence = 0f;

        public int TileX { get { return (int)_CurrentTile.Location.X; } }
        public int TileY { get { return (int)_CurrentTile.Location.Y; } }

        public DrawPosition DrawPosition { get; protected set; }
        
        private MoveEvent _MoveEvent;

        private Entity _entity;
        private IWorld _world;

        public int DrawFacing
        {
            get
            {
                int iFacing = (int)(_facing & Direction.FacingMask);
                if (iFacing >= 3)
                    return iFacing - 3;
                else
                    return iFacing + 5;
            }
        }

        public Movement(Entity entity, IWorld world)
        {
            _entity = entity;
            _world = world;
        }

        public void DesignateClientPlayer()
        {
            _IsClientPlayer = true;
            _MoveEvent = new MoveEvent();
        }

        public void NewFacingToServer(Direction nDirection)
        {
            _facing = nDirection;
            _MoveEvent.NewEvent(this._facing);
        }

        public bool GetMoveEvent(ref int direction, ref int sequence, ref int fastwalkkey)
        {
            if (_IsClientPlayer == false)
                return false;
            return _MoveEvent.GetMoveEvent(ref direction, ref sequence, ref fastwalkkey);
        }

        public void MoveEventAck(int nSequence)
        {
            // do nothing
        }

        public void MoveEventRej(int nSequence, int nX, int nY, int nZ, int nDirection)
        {
            // immediately return to the designated tile.
            SetPositionInstant(nX, nY, nZ, nDirection);
            _MoveEvent.ResetMoveSequence();
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
                Vector3 position = MovementCheck.OffsetTile(_CurrentTile.Location, facing);
                SetGoalTile((int)position.X, (int)position.Y, (int)position.Z);
            }
        }

        public bool IsMoving
        {
            get
            {
                if (((int)_CurrentTile.Location.X == (int)_GoalTile.Location.X) &&
                    ((int)_CurrentTile.Location.Y == (int)_GoalTile.Location.Y) &&
                    DrawPosition.OffsetV3 == new Vector3())
                    return false;
                return true;
            }
        }

        public void SetGoalTile(float nX, float nY, float nZ)
        {
            _GoalTile = new TilePosition(nX, nY, nZ);
        }

        public void SetPositionInstant(int nX, int nY, int nZ, int nFacing)
        {
            _facing = (Direction)nFacing;
            mFlushDrawObjects();
            _GoalTile = _LastTile = _NextTile = _CurrentTile = new TilePosition(nX, nY, nZ);
            DrawPosition = new DrawPosition(_CurrentTile);
        }

        public void Update(GameTime gameTime)
        {
            mFlushDrawObjects();
            
            // Are we moving? (if our current location != our destination, then we are moving)
            if (IsMoving)
            {
                // UO movement is tile based. Have we reached the next tile yet?
                // If we have, then get the next tile in the move sequence and move to that one.
                if (_CurrentTile.Location == _NextTile.Location)
                {
                    Direction iFacing;
                    _NextTile = getNextTile(_CurrentTile.Location, _GoalTile.Location, out iFacing);
                    // Is the next tile on-screen?
                    if (_NextTile != null)
                    {
                        // If we are the player, set our move event so that the game
                        // knows to send a move request to the server ...
                        if (_IsClientPlayer)
                        {
                            // Special exception for the player: if we are facing a new direction, we
                            // need to pause for a brief moment and let the server know that.
                            if ((_facing & Direction.FacingMask) != (iFacing & Direction.FacingMask))
                            {
                                _MoveEvent.NewEvent((iFacing & Direction.FacingMask));
                                _facing = iFacing;
                                _NextTile.Location = _CurrentTile.Location;
                                return;
                            }
                            else
                            {
                                _MoveEvent.NewEvent(iFacing);
                            }
                        }
                        else
                        {
                            // if we are not the player, then we can just change the facing.
                            _facing = iFacing;
                        }
                        _LastTile = new TilePosition(_CurrentTile);
                    }
                    else
                    {
                        // This next tile is no longer in our map in memory.
                        // Ideally, we should remove them from the map entirely.
                        // Right now, we just set their location to the current tile
                        // and refuse to move them further.
                        SetPositionInstant(
                            (int)_CurrentTile.Location.X,
                            (int)_CurrentTile.Location.Y,
                            (int)_CurrentTile.Location.Z,
                            (int)_facing);
                        return;
                    }
                }

                MoveSequence += ((float)(gameTime.ElapsedRealTime.TotalMilliseconds / 1000) / TimeToCompleteMove);
                if (MoveSequence >= 1f)
                {
                    MoveSequence -= 1f;
                    _CurrentTile.Location = _NextTile.Location;
                }
                else
                {
                    _CurrentTile.Location = _LastTile.Location + (_NextTile.Location - _LastTile.Location) * MoveSequence;
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

            DrawPosition = new DrawPosition(_CurrentTile);
        }

        public void ClearImmediate()
        {
            mFlushDrawObjects();
        }

        private void mFlushDrawObjects()
        {
            if (DrawPosition == null)
                return;
            TileEngine.MapTile lastTile = _world.Map.GetMapTile(DrawPosition.TileX, DrawPosition.TileY);
            if (lastTile != null)
            {
                lastTile.FlushObjectsBySerial(_entity.Serial);
            }
        } 

        private TilePosition getNextTile(Vector3 currentLocation, Vector3 goalLocation, out Direction facing)
        {
            facing = getNextFacing(currentLocation, goalLocation);
            Vector3 nextTile = MovementCheck.OffsetTile(currentLocation, facing);

            int nextAltitude;
            bool moveIsOkay = MovementCheck.CheckMovement((Mobile)_entity, _world.Map, currentLocation, facing, out nextAltitude);
            if (moveIsOkay)
            {
                if (IsRunning)
                    facing |= Direction.Running;
                return new TilePosition((int)nextTile.X, (int)nextTile.Y, nextAltitude);
            }
            else
            {
                return null;
            }
        }

        private Direction getNextFacing(Vector3 currentTile, Vector3 goalTile)
        {
            Direction facing;

            if (goalTile.X < currentTile.X)
            {
                if (goalTile.Y < currentTile.Y)
                {
                    facing = Direction.Up;
                }
                else if (goalTile.Y > currentTile.Y)
                {
                    facing = Direction.Left;
                }
                else
                {
                    facing = Direction.West;
                }
            }
            else if (goalTile.X > currentTile.X)
            {
                if (goalTile.Y < currentTile.Y)
                {
                    facing = Direction.Right;
                }
                else if (goalTile.Y > currentTile.Y)
                {
                    facing = Direction.Down;
                }
                else
                {
                    facing = Direction.East;
                }
            }
            else
            {
                if (goalTile.Y < currentTile.Y)
                {
                    facing = Direction.North;
                }
                else if (goalTile.Y > currentTile.Y)
                {
                    facing = Direction.South;
                }
                else
                {
                    // We should never reach this.
                    facing = (Direction)0xFF;
                }
            }

            return facing;
        }
    }

    public class DrawPosition
    {
        public int TileX, TileY, TileZ;
        public float OffsetX, OffsetY, OffsetZ;

        public Vector3 PositionV3
        {
            get
            {
                return new Vector3(TileX, TileY, TileZ);
            }
            set
            {
                TileX = (int)value.X;
                TileY = (int)value.Y;
                TileZ = (int)value.Z;
            }
        }

        public Vector3 OffsetV3
        {
            get
            {
                return new Vector3(OffsetX, OffsetY, OffsetZ);
            }
            set
            {
                OffsetX = value.X;
                OffsetY = value.Y;
                OffsetZ = value.Z;
            }
        }

        public DrawPosition()
        {

        }

        public DrawPosition(TilePosition p)
        {
            TileX = (int)Math.Ceiling(p.Location.X);
            TileY = (int)Math.Ceiling(p.Location.Y);
            TileZ = (int)Math.Ceiling(p.Location.Z);
            OffsetX = ((p.Location.X - (float)TileX));
            OffsetY = ((p.Location.Y - (float)TileY));
            OffsetZ = ((p.Location.Z - (float)TileZ));
        }

        public DrawPosition(DrawPosition p)
        {
            PositionV3 = p.PositionV3;
            OffsetV3 = p.OffsetV3;
        }

        public override string ToString()
        {
            return "X:" + TileX.ToString() + " Y:" + TileY.ToString();
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (o.GetType() != typeof(DrawPosition)) return false;
            if (this.TileX != ((DrawPosition)o).TileX) return false;
            if (this.TileY != ((DrawPosition)o).TileY) return false;
            if (this.TileZ != ((DrawPosition)o).TileZ) return false;
            return true;
        }

        // Equality operator. Returns dbNull if either operand is dbNull, 
        // otherwise returns dbTrue or dbFalse:
        public static bool operator ==(DrawPosition x, DrawPosition y)
        {
            if ((object)x == null)
                return ((object)y == null);
            return x.Equals(y);
        }

        // Inequality operator. Returns dbNull if either operand is
        // dbNull, otherwise returns dbTrue or dbFalse:
        public static bool operator !=(DrawPosition x, DrawPosition y)
        {
            if ((object)x == null)
                return ((object)y != null);
            return !x.Equals(y);
        }

        public override int GetHashCode()
        {
           return 0;
        }

    }

    public class TilePosition
    {
        // public int Map = 0;
        internal Vector3 Location;

        public TilePosition(float nTileX, float nTileY, float nTileZ)
        {
            // Map = 0;
            Location = new Vector3(nTileX, nTileY, nTileZ);
        }

        public TilePosition(TilePosition nCopyPosition)
        {
            // Map = nCopyPosition.Map;
            Location = new Vector3(nCopyPosition.Location.X, nCopyPosition.Location.Y, nCopyPosition.Location.Z);
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
