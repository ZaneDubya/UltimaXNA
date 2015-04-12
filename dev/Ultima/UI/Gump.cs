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
using UltimaXNA.Core.Graphics;
using UltimaXNA.Core.Diagnostics.Tracing;

#endregion

namespace UltimaXNA.Ultima.UI
{
    /// <summary>
    /// The base class that encapsulates Gump functionality. All Gumps should inherit from this class or a child thereof.
    /// </summary>
    public class Gump : AControl
    {
        Serial m_GumpID;
        string[] m_gumpPieces, m_gumpLines;

        GUIManager m_UserInterface;

        public Gump(Serial serial, Serial gumpID)
            : base(null, 0)
        {
            Serial = serial;
            m_GumpID = gumpID;

            m_UserInterface = UltimaServices.GetService<GUIManager>();
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

        public override void ActivateByButton(int buttonID)
        {
            int[] switchIDs = new int[0];
            Tuple<short, string>[] textEntries = new Tuple<short,string>[0];
            m_UserInterface.GumpMenuSelect(Serial, m_GumpID, buttonID, switchIDs, textEntries);
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
                    case "button":
                        // Button [x] [y] [released-id] [pressed-id] [quit] [page-id] [return-value]
                        // [released-id] and [pressed-id] specify the buttongraphic.
                        // If pressed check for [return-value].
                        // Use [page-id] to switch between pages and [quit]=1/0 to close the gump.
                        AddControl(new Controls.Button(this, currentGUMPPage, gumpParams));
                        break;
                    case "buttontileart":
                        // ButtonTileArt [x] [y] [released-id] [pressed-id] [quit] [page-id] [return-value] [tilepic-id] [hue] [tile-x] [tile-y]
                        //  Adds a button to the gump with the specified coordinates and tilepic as graphic.
                        // [tile-x] and [tile-y] define the coordinates of the tile graphic and are relative to [x] and [y]. 
                    case "checkertrans":
                        // CheckerTrans [x] [y] [width] [height]
                        // Creates a transparent rectangle on position [x,y] using [width] and [height].
                        AddControl(new Controls.CheckerTrans(this, currentGUMPPage, gumpParams));
                        break;
                    case "croppedtext":
                        // CroppedText [x] [y] [width] [height] [color] [text-id]
                        // Adds a text field to the gump. This is similar to the text command, but the text is cropped to the defined area.
                        AddControl(new Controls.CroppedText(this, currentGUMPPage, gumpParams, gumpLines));
                        break;
                    case "gumppic":
                        // GumpPic [x] [y] [id] hue=[color]
                        // Adds a graphic to the gump, where [id] ist the graphic id of an item. For example use InsideUO to get them. Optionaly there is a color parameter.
                        AddControl(new Controls.GumpPic(this, currentGUMPPage, gumpParams));
                        break;
                    case "gumppictiled":
                        // GumpPicTiled [x] [y] [width] [height] [id]
                        // Similar to GumpPic, but the gumppic is tiled to the given [height] and [width].
                        AddControl(new Controls.GumpPicTiled(this, currentGUMPPage, gumpParams));
                        break;
                    case "htmlgump":
                        // HtmlGump [x] [y] [width] [height] [text-id] [background] [scrollbar]
                        // Defines a text-area where Html-commands are allowed.
                        // [background] and [scrollbar] can be 0 or 1 and define whether the background is transparent and a scrollbar is displayed.
                        AddControl(new Controls.HtmlGump(this, currentGUMPPage, gumpParams, gumpLines));
                        break;
                    case "page":
                        // Page [Number]
                        // Specifies which page to define. Page 0 is the background thus always visible.
                        currentGUMPPage = Int32.Parse(gumpParams[1]);
                        break;
                    case "resizepic":
                        // ResizePic [x] [y] [gump-id] [width] [height]
                        // Similar to GumpPic but the pic is automatically resized to the given [width] and [height].
                        AddControl(new Controls.ResizePic(this, currentGUMPPage, gumpParams));
                        ((Controls.ResizePic)LastControl).CloseOnRightClick = true;
                        break;
                    case "text":
                        // Text [x] [y] [color] [text-id]
                        // Defines the position and color of a text (data) entry.
                        AddControl(new Controls.TextLabel(this, currentGUMPPage, gumpParams, gumpLines));
                        break;
                    case "textentry":
                        // TextEntry [x] [y] [width] [height] [color] [return-value] [default-text-id]
                        // Defines an area where the [default-text-id] is displayed.
                        // The player can modify this data. To get this data check the [return-value].
                        AddControl(new Controls.TextEntry(this, currentGUMPPage, gumpParams, gumpLines));
                        break;
                    case "textentrylimited":
                        // TextEntryLimited [x] [y] [width] [height] [color] [return-value] [default-text-id] [textlen]
                        // Similar to TextEntry but you can specify via [textlen] the maximum of characters the player can type in.
                        AddControl(new Controls.TextEntry(this, currentGUMPPage, gumpParams, gumpLines));
                        break;
                    case "tilepic":
                        // TilePic [x] [y] [id]
                        // Adds a Tilepicture to the gump. [id] defines the tile graphic-id. For example use InsideUO to get them.
                        AddControl(new Controls.TilePic(this, currentGUMPPage, gumpParams));
                        break;
                    case "tilepichue":
                        // TilePicHue [x] [y] [id] [hue]
                        // Similar to the tilepic command, but with an additional hue parameter.
                        AddControl(new Controls.TilePic(this, currentGUMPPage, gumpParams));
                        break;
                    case "noclose":
                        // NoClose 
                        // Prevents that the gump can be closed by right clicking.
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    case "nodispose":
                        // NoDispose 
                        //Prevents that the gump can be closed by hitting Esc.
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    case "nomove":
                        // NoMove
                        // Locks the gump in his position. 
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    case "group":
                        // Group [Number]
                        // Links radio buttons to a group. Add this before radiobuttons to do so. See also endgroup.
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    case "endgroup":
                        // EndGroup
                        //  Links radio buttons to a group. Add this before radiobuttons to do so. See also endgroup. 
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    case "radio":
                        // Radio [x] [y] [released-id] [pressed-id] [status] [return-value]
                        // Same as Checkbox, but only one Radiobutton can be pressed at the same time, and they are linked via the 'Group' command.
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    case "checkbox":
                        // CheckBox [x] [y] [released-id] [pressed-id] [status] [return-value]
                        // Adds a CheckBox to the gump. Multiple CheckBoxes can be pressed at the same time.
                        // Check the [return-value] if you want to know which CheckBoxes were selected.
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    case "xmfhtmlgump":
                        // XmfHtmlGump [x] [y] [width] [height] [cliloc-nr] [background] [scrollbar]
                        // Similar to the htmlgump command, but in place of the [text-id] a CliLoc entry is used.
                        AddControl(new Controls.HtmlGump(this, currentGUMPPage, int.Parse(gumpParams[1]), int.Parse(gumpParams[2]), int.Parse(gumpParams[3]), int.Parse(gumpParams[4]),
                            int.Parse(gumpParams[6]), int.Parse(gumpParams[7]), IO.StringData.Entry(int.Parse(gumpParams[5]))));
                        break;
                    case "xmfhtmlgumpcolor":
                        // XmfHtmlGumpColor [x] [y] [width] [height] [cliloc-nr] [background] [scrollbar] [color]
                        // Similar to the xmfhtmlgump command, but additionally a [color] can be specified.
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    case "xmfhtmltok":
                        // XmfHtmlTok [x] [y] [width] [height] [background] [scrollbar] [color] [cliloc-nr] @[arguments]@
                        // Similar to xmfhtmlgumpcolor command, but the parameter order is different and an additionally
                        // [argument] entry enclosed with @'s can be used. With this you can specify texts that will be
                        // added to the CliLoc entry. 
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    case "tooltip":
                        // Tooltip [cliloc-nr]
                        // Adds to the previous layoutarray entry a Tooltip with the in [cliloc-nr] defined CliLoc entry.
                        Tracer.Warn(string.Format("GUMP: Unhandled {0}.", gumpParams[0]));
                        break;
                    default:
                        Tracer.Critical("GUMP: Unknown piece '" + gumpParams[0] + "'.");
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
                if (c.GetType() == typeof(UI.Controls.TextEntry))
                {
                    UI.Controls.TextEntry g = (UI.Controls.TextEntry)c;
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
