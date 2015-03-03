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
using UltimaXNA.UltimaWorld;
#endregion

namespace UltimaXNA.Entity
{
    public class BaseEntity
    {
        public Serial Serial;
        public PropertyList PropertyList = new PropertyList();

        private Dictionary<int, Overhead> m_overheads = new Dictionary<int, Overhead>();
        int m_lastOverheadIndex = 0;

        protected bool m_drawn = false; // if this is false this object will redraw itself in the tileengine.}
        protected bool HasBeenDrawn
        {
            get { return m_drawn; }
            set
            {
                if ((value == false) && (m_drawn == true))
                {
                    flushDrawObjects();
                }
                m_drawn = value;
            }
        }

        bool m_Disposed = false; // set this to true to have the object deleted.
        public bool IsDisposed { get { return m_Disposed; } protected set { m_Disposed = value; } }

        protected Movement m_movement;
        public int X { get { return m_movement.Position.X; } set { m_movement.Position.X = value; HasBeenDrawn = false; } }
        public int Y { get { return m_movement.Position.Y; } set { m_movement.Position.Y = value; HasBeenDrawn = false; } }
        public int Z { get { return m_movement.Position.Z; } set { m_movement.Position.Z = value; HasBeenDrawn = false; } }
        public Position3D Position { get { return m_movement.Position; } }
        public virtual Position3D WorldPosition { get { return m_movement.Position; } }
        public Direction Facing { get { return m_movement.Facing & Direction.FacingMask; } set { m_movement.Facing = value; HasBeenDrawn = false; } }

        public bool IsClientEntity = false;

        public int DrawFacing
        {
            get
            {
                int iFacing = (int)(m_movement.Facing & Direction.FacingMask);
                if (iFacing >= 3)
                    return iFacing - 3;
                else
                    return iFacing + 5;
            }
        }

        public BaseEntity(Serial serial)
        {
            Serial = serial;
            m_movement = new Movement(this);
        }

        public virtual void Update(double frameMS)
        {
            if (IsDisposed)
                return;

            if (!m_movement.Position.IsNullPosition)
            {
                m_movement.Update(frameMS);

                if (IsometricRenderer.Map == null)
                    return;

                MapTile t = IsometricRenderer.Map.GetMapTile(m_movement.Position.X, m_movement.Position.Y, false);
                if (t != null)
                {
                    this.Draw(t, m_movement.Position);
                    HasBeenDrawn = true;
                }
                else
                    HasBeenDrawn = false;
            }

            // handle overheads.
            clearDisposedOverheads();
            foreach (KeyValuePair<int, Overhead> overhead in m_overheads)
            {
                overhead.Value.Update(frameMS);
            }
        }

        internal virtual void Draw(MapTile tile, Position3D position)
        {

        }

        internal void drawOverheads(MapTile tile, Position3D position)
        {
            // base entities do not draw, but they can have overheads, so we draw those.
            foreach (KeyValuePair<int, Overhead> overhead in m_overheads)
            {
                if (!overhead.Value.IsDisposed)
                    overhead.Value.Draw(tile, position);
            }
        }

        void flushDrawObjects()
        {
            if (!Position.IsNullPosition)
            {
                MapTile lastTile = IsometricRenderer.Map.GetMapTile(Position.X, Position.Y, false);
                if (lastTile != null)
                    lastTile.FlushObjectsBySerial(Serial);
            }
        }

        public virtual void Dispose()
        {
            m_Disposed = true;
            m_movement.ClearImmediate();
        }

        public Overhead AddOverhead(MessageType msgType, string text, int fontID, int hue)
        {
            // Only one label allowed at a time.
            if (msgType == MessageType.Label)
            {
                disposeLabels();
                clearDisposedOverheads();
            }

            foreach (Overhead o in m_overheads.Values)
            {
                if ((o.Text == text) && !(o.IsDisposed))
                {
                    o.RefreshTimer();
                    return o;
                }
            }

            Overhead overhead = new Overhead(this, msgType, text, fontID, hue);
            m_overheads.Add(m_lastOverheadIndex++, overhead);
            return overhead;
        }

        private void disposeLabels()
        {
            foreach (Overhead o in m_overheads.Values)
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
            foreach (KeyValuePair<int, Overhead> overhead in m_overheads)
            {
                if (overhead.Value.IsDisposed)
                {
                    removeObjectsList.Add(overhead.Key);
                    continue;
                }
            }
            foreach (int i in removeObjectsList)
            {
                m_overheads.Remove(i);
            }
        }

        public override string ToString()
        {
            return Serial.ToString();
        }
    }
}
