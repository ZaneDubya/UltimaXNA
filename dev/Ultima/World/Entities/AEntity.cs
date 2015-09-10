/***************************************************************************
 *   AEntity.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Ultima.Data;
using UltimaXNA.Ultima.World.EntityViews;
using UltimaXNA.Ultima.World.Maps;
#endregion

public delegate void OnEvent();

namespace UltimaXNA.Ultima.World.Entities
{
    /// <summary>
    /// Base class for all entities which exist in the world model.
    /// </summary>
    public abstract class AEntity
    {
        public OnEvent OnEntityUpdated;

        // ============================================================
        // Properties
        // ============================================================

        public Serial Serial;

        public PropertyList PropertyList = new PropertyList();

        public bool IsDisposed = false;

        public bool IsClientEntity = false;

        public virtual int Hue
        {
            set;
            get;
        }

        public virtual string Name
        {
            get
            {
                return "AEntity";
            }
            set
            {
                // do nothing. This exists so that inheriting classes can override the set accessor.
            }
        }

        // ============================================================
        // Position
        // ============================================================

        public Map Map
        {
            get;
            private set;
        }

        public void SetMap(Map map)
        {
            if (map != Map)
            {
                Map = map;
                Position.Tile = Position3D.NullTile;
            }
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
                    if (!IsClientEntity)
                        if (!IsDisposed)
                            Dispose();
                }
            }
            get
            {
                return m_Tile;
            }
        }

        protected virtual void OnTileChanged(int x, int y)
        {
            if (Map != null)
            {
                if (IsClientEntity && Map.Index >= 0)
                    Map.CenterPosition = new Point(x, y);
                Tile = Map.GetMapTile(x, y);
            }
            else
            {
                if (!IsClientEntity)
                    Dispose();
            }
        }

        public int X
        {
            get { return Position.X; }
        }

        public int Y
        {
            get { return Position.Y; }
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
        // Draw and View handling code
        // ============================================================

        private AEntityView m_View = null;

        protected virtual AEntityView CreateView()
        {
            return null;
        }

        public AEntityView GetView()
        {
            if (m_View == null)
                m_View = CreateView();
            return m_View;
        }

        internal virtual void Draw(MapTile tile, Position3D position)
        {

        }

        // ============================================================
        // Overhead handling code (labels, chat, etc.)
        // ============================================================

        private List<Overhead> m_Overheads = new List<Overhead>();
        public List<Overhead> Overheads
        {
            get { return m_Overheads; }
        }

        public Overhead AddOverhead(MessageTypes msgType, string text, int fontID, int hue, bool asUnicode)
        {
            Overhead overhead;
            text = string.Format("<outline style='font-family: {2}{0};'>{1}", fontID, text, asUnicode ? "uni" : "ascii");

            for (int i = 0; i < m_Overheads.Count; i++)
            {
                overhead = m_Overheads[i];
                // is this overhead an already active label?
                if ((msgType  == MessageTypes.Label) && (overhead.Text == text) && (overhead.MessageType == msgType) && !(overhead.IsDisposed))
                {
                    // reset the timer for the object so it lasts longer.
                    overhead.ResetTimer();
                    // update hue?
                    overhead.Hue = hue;
                    // insert it at the bottom of the queue so it displays closest to the player.
                    m_Overheads.RemoveAt(i);
                    InternalInsertOverhead(overhead);
                    return overhead;
                }
            }

            overhead = new Overhead(this, msgType, text);
            overhead.Hue = hue;
            InternalInsertOverhead(overhead);
            return overhead;
        }

        private void InternalInsertOverhead(Overhead overhead)
        {
            if (m_Overheads.Count == 0 || (m_Overheads[0].MessageType != MessageTypes.Label))
                m_Overheads.Insert(0, overhead);
            else
                m_Overheads.Insert(1, overhead);
        }

        internal void InternalDrawOverheads(MapTile tile, Position3D position)
        {
            // base entities do not draw, but they can have overheads, so we draw those.
            foreach (Overhead overhead in m_Overheads)
            {
                if (!overhead.IsDisposed)
                    overhead.Draw(tile, position);
            }
        }

        private void InternalUpdateOverheads(double frameMS)
        {
            // update overheads
            foreach (Overhead overhead in m_Overheads)
            {
                overhead.Update(frameMS);
            }
            // remove disposed of overheads.
            for (int i = 0; i < m_Overheads.Count; i++)
            {
                if (m_Overheads[i].IsDisposed)
                {
                    m_Overheads.RemoveAt(i);
                    i--;
                }
            }
        }

        // Update range
        public virtual int GetMaxUpdateRange()
        {
            return 18;
        }
    }
}
