/***************************************************************************
 *   Entity.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
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
#endregion

namespace UltimaXNA.Entities
{
    public class Entity
    {
        public Serial Serial;
        public Movement Movement;
        public PropertyList PropertyList = new PropertyList();

        private Dictionary<int, Overhead> _overheads = new Dictionary<int, Overhead>();
        private int _lastOverheadIndex = 0;

        internal bool _hasBeenDrawn = false; // if this is false this object will redraw itself in the tileengine.}
        internal bool _Disposed = false; // set this to true to have the object deleted.
        public bool IsDisposed { get { return _Disposed; } }

        public int X { get { return Movement.DrawPosition.TileX; } }
        public int Y { get { return Movement.DrawPosition.TileY; } }
        public int Z { get { return Movement.DrawPosition.TileZ; } }

        public Entity(Serial serial)
        {
            Serial = serial;
            Movement = new Movement(this);
            _hasBeenDrawn = false;
        }

        public virtual void Update(GameTime gameTime)
        {
            if ((Movement.RequiresUpdate || _hasBeenDrawn == false) && Movement.DrawPosition != null)
            {
                Movement.Update(gameTime);

                TileEngine.MapCell iThisMapCell = TileEngine.World.Map.GetMapCell(Movement.DrawPosition.TileX, Movement.DrawPosition.TileY);
                if (iThisMapCell != null)
                {
                    this.Draw(iThisMapCell, Movement.DrawPosition.PositionV3, Movement.DrawPosition.OffsetV3);
                    _hasBeenDrawn = true;
                }
                else
                {
                    _hasBeenDrawn = false;
                }
            }

            // handle overheads.
            clearDisposedOverheads();
            foreach (KeyValuePair<int, Overhead> overhead in _overheads)
            {
                overhead.Value.Update(gameTime);
            }
        }

        internal virtual void Draw(TileEngine.MapCell cell, Vector3 position, Vector3 positionOffset)
        {
            // base entities do not draw.
        }

        internal void drawOverheads(TileEngine.MapCell cell, Vector3 position, Vector3 positionOffset)
        {
            // base entities do not draw, but they can have overheads, so we draw those.
            position.Z += 20;
            foreach (KeyValuePair<int, Overhead> overhead in _overheads)
            {
                if (!overhead.Value.IsDisposed)
                    overhead.Value.Draw(cell, position, positionOffset);
            }
        }

        public virtual void Dispose()
        {
            _Disposed = true;
            Movement.ClearImmediate();
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

            Overhead overhead = new Overhead(msgType, this, text, fontID, hue);
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
