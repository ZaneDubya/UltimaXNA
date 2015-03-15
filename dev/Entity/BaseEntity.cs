/***************************************************************************
 *   BaseEntity.cs
 *      
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using UltimaXNA.UltimaWorld;
using UltimaXNA.UltimaWorld.View;
using Microsoft.Xna.Framework;
using UltimaXNA.UltimaWorld.Model;
#endregion

namespace UltimaXNA.Entity
{
    public abstract class BaseEntity
    {
        // ============================================================
        // Properties
        // ============================================================

        public Serial Serial;

        public PropertyList PropertyList = new PropertyList();

        public bool IsDisposed = false;

        public bool IsClientEntity = false;

        public int Hue = 0;

        // ============================================================
        // Position
        // ============================================================

        private MapTile m_Tile;
        private MapTile Tile
        {
            set
            {
                if (m_Tile != null)
                    m_Tile.OnExit(this);
                m_Tile = value;

                if (m_Tile != null)
                    m_Tile.OnEnter(this);
                else
                {
                    if (!IsDisposed)
                        Dispose();
                }
            }
        }

        private void OnTileChanged(int x, int y)
        {
            if (IsClientEntity && EntityManager.Model.MapIndex >= 0)
                EntityManager.Model.Map.CenterPosition = new Point(x, y);
            Tile = EntityManager.Model.Map.GetMapTile(x, y);
        }

        public int X { get { return Position.X; } }
        public int Y { get { return Position.Y; } }

        public int Z
        {
            get { return Position.Z; }
        }

        private Position3D m_Position;
        public virtual Position3D Position { get { return m_Position; } }

        // ============================================================
        // Methods
        // ============================================================

        public BaseEntity(Serial serial)
        {
            Serial = serial;
            m_Position = new Position3D(OnTileChanged);
        }

        public virtual void Update(double frameMS)
        {
            if (IsDisposed)
                return;

            InternalUpdateOverheads(frameMS);
        }

        public virtual void Dispose()
        {
            IsDisposed = true;
            Tile = null;
        }

        public override string ToString()
        {
            return Serial.ToString();
        }

        // ============================================================
        // Draw handling code
        // ============================================================

        private EntityViews.AEntityView m_View = null;

        protected virtual EntityViews.AEntityView CreateView()
        {
            return null;
        }

        public EntityViews.AEntityView GetView()
        {
            if (m_View == null)
                m_View = CreateView();
            return m_View;
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

        // ============================================================
        // Overhead handling code
        // ============================================================

        private Dictionary<int, Overhead> m_overheads = new Dictionary<int, Overhead>();
        int m_lastOverheadIndex = 0;

        private void InternalUpdateOverheads(double frameMS)
        {
            // handle overheads.
            clearDisposedOverheads();
            foreach (KeyValuePair<int, Overhead> overhead in m_overheads)
            {
                overhead.Value.Update(frameMS);
            }
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
    }
}
