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
    public abstract class AEntity
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

        public Map Map
        {
            get;
            private set;
        }

        private MapTile m_Tile;
        protected MapTile Tile
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
            if (IsClientEntity && Map.Index >= 0)
                Map.CenterPosition = new Point(x, y);
            Tile = Map.GetMapTile(x, y);
        }

        public int Z
        {
            get { return Position.Z; }
        }

        private Position3D m_Position;
        public virtual Position3D Position { get { return m_Position; } }

        // ============================================================
        // Methods
        // ============================================================

        public AEntity(Serial serial, Map map)
        {
            Serial = serial;
            Map = map;

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

        // ============================================================
        // Overhead handling code
        // ============================================================

        private List<Overhead> m_overheads = new List<Overhead>();

        internal void InternalDrawOverheads(MapTile tile, Position3D position)
        {
            // base entities do not draw, but they can have overheads, so we draw those.
            foreach (Overhead overhead in m_overheads)
            {
                if (!overhead.IsDisposed)
                    overhead.Draw(tile, position);
            }
        }

        private void InternalUpdateOverheads(double frameMS)
        {
            // handle overheads.
            InternalClearDisposedOverheads();
            foreach (Overhead overhead in m_overheads)
            {
                overhead.Update(frameMS);
            }
        }

        public Overhead AddOverhead(MessageType msgType, string text, int fontID, int hue)
        {
            // Labels are exclusive
            if (msgType == MessageType.Label)
            {
                InternalDisposeOfAllLabels();
                InternalClearDisposedOverheads();
            }

            foreach (Overhead o in m_overheads)
            {
                if ((o.Text == text) && !(o.IsDisposed))
                {
                    o.ResetTimer();
                    return o;
                }
            }

            Overhead overhead = new Overhead(this, msgType, text);
            m_overheads.Add(overhead);
            return overhead;
        }

        private void InternalDisposeOfAllLabels()
        {
            foreach (Overhead o in m_overheads)
            {
                if (o.MessageType == MessageType.Label)
                {
                    o.Dispose();
                }
            }
        }

        private void InternalClearDisposedOverheads()
        {
            List<int> removeObjectsList = removeObjectsList = new List<int>();
            for (int i = 0; i < m_overheads.Count; i++)
            {
                if (m_overheads[i].IsDisposed)
                {
                    m_overheads.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}
