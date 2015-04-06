/***************************************************************************
 *   Gump.cs
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Rendering;
#endregion

namespace UltimaXNA.UltimaGUI
{
    /// <summary>
    /// The base class that encapsulates Gump functionality. All Gumps should inherit from this class or a child thereof.
    /// </summary>
    public class Gump : AControl
    {
        Serial m_GumpID;
        string[] m_gumpPieces, m_gumpLines;

        public Gump(Serial serial, Serial gumpID)
            : base(null, 0)
        {
            Serial = serial;
            m_GumpID = gumpID;
        }

        public Gump(Serial serial, Serial gumpID, String[] pieces, String[] textlines)
            : this(serial, gumpID)
        {
            m_gumpPieces = pieces;
            m_gumpLines = textlines;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            // Add any gump pieces that have been given to the gump...
            if (m_gumpPieces != null)
            {
                BuildGump(m_gumpPieces, m_gumpLines);
                m_gumpPieces = null;
                m_gumpLines = null;
            }

            // If page = 0, then we've just created this page. We initialize activepage to 1.
            // This triggers the additional functionality in Control.ActivePage.Set().
            if (ActivePage == 0)
                ActivePage = 1;

            // Update the Controls...
            base.Update(totalMS, frameMS);

            // Do we need to resize?
            if (CheckResize())
            {

            }
        }

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (!Visible)
                return;

            base.Draw(spriteBatch);
        }

        public override void ActivateByButton(int buttonID)
        {
            int[] switchIDs = new int[0];
            Tuple<short, string>[] textEntries = new Tuple<short,string>[0];
            Engine.UserInterface.GumpMenuSelect(Serial, m_GumpID, buttonID, switchIDs, textEntries);
            Dispose();
        }

        public override void ChangePage(int pageIndex)
        {
            // For a gump, Page is the page that is drawing.
            ActivePage = pageIndex;
        }

        private void BuildGump(string[] gumpPieces, string[] gumpLines)
        {
            int currentGUMPPage = 0;

            for (int i = 0; i < gumpPieces.Length; i++)
            {
                string[] gumpParams = gumpPieces[i].Split(' ');
                switch (gumpParams[0])
                {
                    case "page":
                        currentGUMPPage = Int32.Parse(gumpParams[1]);
                        break;
                    case "checkertrans":
                        AddControl(new Controls.CheckerTrans(this, currentGUMPPage, gumpParams));
                        break;
                    case "resizepic":
                        AddControl(new Controls.ResizePic(this, currentGUMPPage, gumpParams));
                        ((Controls.ResizePic)LastControl).CloseOnRightClick = true;
                        break;
                    case "button":
                        AddControl(new Controls.Button(this, currentGUMPPage, gumpParams));
                        break;
                    case "croppedtext":
                        AddControl(new Controls.CroppedText(this, currentGUMPPage, gumpParams, gumpLines));
                        break;
                    case "htmlgump":
                        AddControl(new Controls.HtmlGump(this, currentGUMPPage, gumpParams, gumpLines));
                        break;
                    case "gumppictiled":
                        AddControl(new Controls.GumpPicTiled(this, currentGUMPPage, gumpParams));
                        break;
                    case "gumppic":
                        AddControl(new Controls.GumpPic(this, currentGUMPPage, gumpParams));
                        break;
                    case "text":
                        AddControl(new Controls.TextLabel(this, currentGUMPPage, gumpParams, gumpLines));
                        break;
                    case "tilepic":
                        AddControl(new Controls.TilePic(this, currentGUMPPage, gumpParams));
                        break;
                    case "tilepichue":
                        AddControl(new Controls.TilePic(this, currentGUMPPage, gumpParams));
                        break;
                    case "textentry":
                        AddControl(new Controls.TextEntry(this, currentGUMPPage, gumpParams, gumpLines));
                        break;
                    case "textentrylimited":
                        AddControl(new Controls.TextEntry(this, currentGUMPPage, gumpParams, gumpLines));
                        break;

                    case "checkbox":
                        Logger.Warn("GUMP: Unhandled '" + gumpParams[0] + "'.");
                        break;
                    case "group":
                        Logger.Warn("GUMP: Unhandled '" + gumpParams[0] + "'.");
                        break;
                    case "xmfhtmlgump":
                        Logger.Warn("GUMP: Unhandled '" + gumpParams[0] + "'.");
                        break;
                    case "xmfhtmlgumpcolor":
                        Logger.Warn("GUMP: Unhandled '" + gumpParams[0] + "'.");
                        break;
                    case "xmfhtmltok":
                        Logger.Warn("GUMP: Unhandled '" + gumpParams[0] + "'.");
                        break;
                    case "buttontileart":
                        Logger.Warn("GUMP: Unhandled '" + gumpParams[0] + "'.");
                        break;
                    case "tooltip":
                        Logger.Warn("GUMP: Unhandled '" + gumpParams[0] + "'.");
                        break;
                    case "radio":
                        Logger.Warn("GUMP: Unhandled '" + gumpParams[0] + "'.");
                        break;
                    default:
                        Logger.Warn("GUMP: Unknown piece '" + gumpParams[0] + "'.");
                        break;
                }
            }
        }

        private bool CheckResize()
        {
            bool changedDimensions = false;
            if (Controls.Count > 0)
            {
                int w = 0, h = 0;
                foreach (AControl c in Controls)
                {
                    if (c.Page == 0 || c.Page == ActivePage)
                    {
                        if (w < c.X + c.Width)
                        {
                            w = c.X + c.Width;
                        }
                        if (h < c.Y + c.Height)
                        {
                            h = c.Y + c.Height;
                        }
                    }
                }

                if (w != Width || h != Height)
                {
                    Width = w;
                    Height = h;
                    changedDimensions = true;
                }
            }
            return changedDimensions;
        }

        protected string GetTextEntry(int entryID)
        {
            foreach (AControl c in Controls)
            {
                if (c.GetType() == typeof(UltimaGUI.Controls.TextEntry))
                {
                    UltimaGUI.Controls.TextEntry g = (UltimaGUI.Controls.TextEntry)c;
                    if (g.EntryID == entryID)
                        return g.Text;
                }
            }
            return string.Empty;
        }

        public override bool Equals(object obj)
        {
            // We override gump equality to provide the ability to NOT add a gump if only one should be active.

            // if parameter is null or cannot be cast to Control, return false.
            if (obj == null || (obj as AControl) == null)
            {
                return false;
            }

            // by default, Gumps are equal to each other if they are of the same type.
            // Inheriting Gumps should override this to base equality on their Parent object's serial, if appropriate.
            if (GetType() == obj.GetType())
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
