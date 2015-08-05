/***************************************************************************
 *   Gump.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.UI;
using UltimaXNA.Ultima.UI.Controls;
#endregion

namespace UltimaXNA.Ultima.UI
{
    /// <summary>
    /// The base class that encapsulates Gump functionality. All Gumps should inherit from this class or a child thereof.
    /// </summary>
    public class Gump : AControl
    {
        /// <summary>
        /// A unique identifier, assigned by the server, that is sent by the client when a button is pressed.
        /// </summary>
        public int GumpServerTypeID
        {
            get;
            set;
        }

        /// <summary>
        /// If this is true, SaveGump() will be called when the World stops, and LoadGump() will be called when the World starts.
        /// </summary>
        public bool SaveOnWorldStop
        {
            get;
            protected set;
        }

        /// <summary>
        /// If true, gump will not be moved by mouse movement, even if IsMoveable is true.
        /// </summary>
        public bool BlockMovement
        {
            get;
            set;
        }

        public override bool IsMoveable
        {
            get
            {
                return !BlockMovement && base.IsMoveable;
            }
            set
            {
                base.IsMoveable = value;
            }
        }

        public Gump(int localID, int gumpTypeID)
            : base(null)
        {
            GumpLocalID = localID;
            GumpServerTypeID = gumpTypeID;
        }

        public Gump(int localID, int gumpTypeID, String[] pieces, String[] textlines)
            : this(localID, gumpTypeID)
        {
            // Add any gump pieces that have been given to the gump...
            GumpBuilder.BuildGump(this, pieces, textlines);
        }

        public override void Dispose()
        {
            SavePosition();
            base.Dispose();
        }

        public override string ToString()
        {
            return GetType().ToString();
        }

        public override void Update(double totalMS, double frameMS)
        {
            // If page = 0, then we've just created this page. We initialize activepage to 1.
            // This triggers the additional functionality in Control.ActivePage.Set().
            if (ActivePage == 0)
                ActivePage = 1;

            // Update the Controls...
            base.Update(totalMS, frameMS);

            CheckRestoreSavedPosition();
        }

        public override void ActivateByButton(int buttonID)
        {
            if (GumpLocalID != 0)
            {
                if (buttonID == 0) // cancel
                {
                    WorldModel world = ServiceRegistry.GetService<WorldModel>();
                    world.Client.SendGumpMenuSelect(GumpLocalID, GumpServerTypeID, buttonID, null, null);
                }
                else
                {
                    List<int> switchIDs = new List<int>();
                    foreach (AControl control in Children)
                    {
                        if (control is CheckBox && (control as CheckBox).IsChecked)
                            switchIDs.Add(control.GumpLocalID);
                        else if (control is RadioButton && (control as RadioButton).IsChecked)
                            switchIDs.Add(control.GumpLocalID);
                    }
                    List<Tuple<short, string>> textEntries = new List<Tuple<short, string>>();
                    foreach (AControl control in Children)
                    {
                        if (control is TextEntry)
                        {
                            textEntries.Add(new Tuple<short, string>((short)control.GumpLocalID, (control as TextEntry).Text));
                        }
                    }
                    WorldModel world = ServiceRegistry.GetService<WorldModel>();
                    world.Client.SendGumpMenuSelect(GumpLocalID, GumpServerTypeID, buttonID, switchIDs.ToArray(), textEntries.ToArray());
                }
                Dispose();
            }
        }

        protected override void CloseWithRightMouseButton()
        {
            if (IsUncloseableWithRMB)
                return;
            // send cancel message for server gump
            if (GumpServerTypeID != 0)
                ActivateByButton(0);
            base.CloseWithRightMouseButton();
        }

        public override void ChangePage(int pageIndex)
        {
            // For a gump, Page is the page that is drawing.
            ActivePage = pageIndex;
        }

        protected string GetTextEntry(int entryID)
        {
            foreach (AControl c in Children)
            {
                if (c.GetType() == typeof(TextEntry))
                {
                    TextEntry g = (TextEntry)c;
                    if (g.EntryID == entryID)
                        return g.Text;
                }
            }
            return string.Empty;
        }

        protected override void OnMove()
        {
            SpriteBatchUI sb = ServiceRegistry.GetService<SpriteBatchUI>();
            Point position = Position;

            int halfWidth = Width / 2;
            int halfHeight = Height / 2;

            if (X < -halfWidth)
                position.X = -halfWidth;
            if (Y < -halfHeight)
                position.Y = -halfHeight;
            if (X > sb.GraphicsDevice.Viewport.Width - halfWidth)
                position.X = sb.GraphicsDevice.Viewport.Width - halfWidth;
            if (Y > sb.GraphicsDevice.Viewport.Height - halfHeight)
                position.Y = sb.GraphicsDevice.Viewport.Height - halfHeight;

            Position = position;
        }

        #region Position Save and Restore
        private bool m_WillSavePosition = false, m_WillOffsetNextPosition = false;
        private string m_SavePositionName = null;
        private bool m_HasRestoredPosition = false;

        private static Point s_SavePositionOffsetAmount = new Point(24, 24);

        protected void SetSavePositionName(string positionName, bool offsetNext = false)
        {
            if (positionName != null)
            {
                m_WillSavePosition = true;
                m_SavePositionName = positionName;
                m_WillOffsetNextPosition = offsetNext;
            }
        }

        private void CheckRestoreSavedPosition()
        {
            if (!m_HasRestoredPosition && m_WillSavePosition && m_SavePositionName != null)
            {
                Point gumpPosition = Settings.Gumps.GetLastPosition(m_SavePositionName, Position);
                Position = gumpPosition;
                if (m_WillOffsetNextPosition)
                {
                    SavePosition();
                }
            }
            m_HasRestoredPosition = true;
        }

        private void SavePosition()
        {
            if (m_WillSavePosition && m_SavePositionName != null)
            {
                Point savePosition = Position;
                if (m_WillOffsetNextPosition)
                {
                    savePosition.X += s_SavePositionOffsetAmount.X;
                    savePosition.Y += s_SavePositionOffsetAmount.Y;
                }
                Settings.Gumps.SetLastPosition(m_SavePositionName, savePosition);

            }
        }
        #endregion

        /// <summary>
        /// Called when a gump asks to be restored on login. Should return a dictionary of data needed to restore the gump. Return false to not save this gump.
        /// </summary>
        /// <returns></returns>
        public virtual bool SaveGump(out Dictionary<string, object> data)
        {
            data = null;
            return false;
        }

        /// <summary>
        /// Called to restore a gump that asked to be restored on login.
        /// </summary>
        /// <param name="data">A dictionary of data needed to restore the gump.</param>
        /// <returns>Return false to cancel restoring this gump.</returns>
        public virtual bool RestoreGump(Dictionary<string, object> data)
        {
            return false;
        }
    }
}
