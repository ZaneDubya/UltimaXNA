/***************************************************************************
 *   Entity.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *      
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
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
    public class Entity
    {
        public Serial Serial;
        public PropertyList PropertyList = new PropertyList();

        private Dictionary<int, Overhead> _overheads = new Dictionary<int, Overhead>();
        int _lastOverheadIndex = 0;

        protected bool _drawn = false; // if this is false this object will redraw itself in the tileengine.}
        protected bool HasBeenDrawn
        {
            get { return _drawn; }
            set
            {
                if ((value == false) && (_drawn == true))
                {
                    flushDrawObjects();
                }
                _drawn = value;
            }
        }

        bool _Disposed = false; // set this to true to have the object deleted.
        public bool IsDisposed { get { return _Disposed; } protected set { _Disposed = value; } }

        IIsometricRenderer _world;
        protected IIsometricRenderer World { get { return _world; } }
        protected Movement _movement;
        public int X { get { return _movement.Position.X; } set { _movement.Position.X = value; HasBeenDrawn = false; } }
        public int Y { get { return _movement.Position.Y; } set { _movement.Position.Y = value; HasBeenDrawn = false; } }
        public int Z { get { return _movement.Position.Z; } set { _movement.Position.Z = value; HasBeenDrawn = false; } }
        public Position3D Position { get { return _movement.Position; } }
        public virtual Position3D WorldPosition { get { return _movement.Position; } }
        public Direction Facing { get { return _movement.Facing & Direction.FacingMask; } set { _movement.Facing = value; HasBeenDrawn = false; } }

        public bool IsClientEntity = false;

        public int DrawFacing
        {
            get
            {
                int iFacing = (int)(_movement.Facing & Direction.FacingMask);
                if (iFacing >= 3)
                    return iFacing - 3;
                else
                    return iFacing + 5;
            }
        }

        public Entity(Serial serial, IIsometricRenderer world)
        {
            Serial = serial;
            _movement = new Movement(this, world);
            _world = world;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (IsDisposed)
                return;

            if (!_movement.Position.IsNullPosition)
            {
                _movement.Update(gameTime);

                if (_world.Map == null)
                    return;

                TileEngine.MapTile t = _world.Map.GetMapTile(_movement.Position.X, _movement.Position.Y, false);
                if (t != null)
                {
                    this.Draw(t, _movement.Position);
                    HasBeenDrawn = true;
                }
                else
                    HasBeenDrawn = false;
            }

            // handle overheads.
            clearDisposedOverheads();
            foreach (KeyValuePair<int, Overhead> overhead in _overheads)
            {
                overhead.Value.Update(gameTime);
            }
        }

        internal virtual void Draw(MapTile tile, Position3D position)
        {

        }

        internal void drawOverheads(TileEngine.MapTile tile, Position3D position)
        {
            // base entities do not draw, but they can have overheads, so we draw those.
            foreach (KeyValuePair<int, Overhead> overhead in _overheads)
            {
                if (!overhead.Value.IsDisposed)
                    overhead.Value.Draw(tile, position);
            }
        }

        void flushDrawObjects()
        {
            if (!Position.IsNullPosition)
            {
                TileEngine.MapTile lastTile = _world.Map.GetMapTile(Position.X, Position.Y, false);
                if (lastTile != null)
                    lastTile.FlushObjectsBySerial(Serial);
            }
        }

        public virtual void Dispose()
        {
            _Disposed = true;
            _movement.ClearImmediate();
        }

        public Overhead AddOverhead(MessageType msgType, string text, int fontID, int hue)
        {
            // Only one label allowed at a time.
            if (msgType == MessageType.Label)
            {
                disposeLabels();
                clearDisposedOverheads();
            }

            foreach (Overhead o in _overheads.Values)
            {
                if ((o.Text == text) && !(o.IsDisposed))
                {
                    o.RefreshTimer();
                    return o;
                }
            }

            Overhead overhead = new Overhead(this, _world, msgType, text, fontID, hue);
            _overheads.Add(_lastOverheadIndex++, overhead);
            return overhead;
        }

        private void disposeLabels()
        {
            foreach (Overhead o in _overheads.Values)
            {
                if (o.MsgType == MessageType.Label)
                {
                    o.Dispose();
                }
            }
        }

        private void clearDisposedOverheads()
        {
            List<int> removeObjectsList = removeObjectsList = new List<int>();
            foreach (KeyValuePair<int, Overhead> overhead in _overheads)
            {
                if (overhead.Value.IsDisposed)
                {
                    removeObjectsList.Add(overhead.Key);
                    continue;
                }
            }
            foreach (int i in removeObjectsList)
            {
                _overheads.Remove(i);
            }
        }

        public override string ToString()
        {
            return Serial.ToString();
        }
    }
}
