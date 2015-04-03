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
using UltimaXNA.Core.Rendering;
using UltimaXNA.UltimaWorld;
#endregion

namespace UltimaXNA.UltimaGUI
{
    /// <summary>
    /// The base class that encapsulates Gump functionality. All Gumps should inherit from this class or a child thereof.
    /// </summary>
    public class Gump : Control
    {
        
        Serial GumpID;
        string[] m_gumpPieces, m_gumpLines;

        public Gump(Serial serial, Serial gumpID)
            : base(null, 0)
        {
            Serial = serial;
            GumpID = gumpID;
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
                buildGump(m_gumpPieces, m_gumpLines);
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
            if (checkResize())
            {
                if (m_gumpTexture != null)
                {
                    m_gumpTexture.Dispose();
                }
            }
        }

        RenderTarget2D m_gumpTexture = null;

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (!Visible)
                return;

            if (m_renderFullScreen)
            {
                InputMultiplier = (float)spriteBatch.GraphicsDevice.Viewport.Width / (float)Width;

                if (m_gumpTexture == null)
                {
                    // the render target CANNOT be larger than the viewport.
                    int w = Width < UserInterface.Width ? Width : UserInterface.Width;
                    int h = Height < UserInterface.Height ? Height : UserInterface.Height;
                    m_gumpTexture = new RenderTarget2D(spriteBatch.GraphicsDevice, w, h, false, SurfaceFormat.Color, DepthFormat.Depth16);
                }

                spriteBatch.GraphicsDevice.SetRenderTarget(m_gumpTexture);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                base.Draw(spriteBatch);
                spriteBatch.Flush();

                spriteBatch.GraphicsDevice.SetRenderTarget(null);

                if (m_renderFullScreen)
                {
                    spriteBatch.Draw2D(m_gumpTexture, new Rectangle(0, 0, (int)(Width * InputMultiplier), (int)(Height * InputMultiplier)), 0, false, false);
                }
                else
                    spriteBatch.Draw2D(m_gumpTexture, Position, 0, false, false);
            }
            else
            {
                spriteBatch.Flush();
                base.Draw(spriteBatch);
            }
        }

        public override void ActivateByButton(int buttonID)
        {
            int[] switchIDs = new int[0];
            Tuple<short, string>[] textEntries = new Tuple<short,string>[0];
            WorldInteraction.GumpMenuSelect(this.Serial, this.GumpID, buttonID, switchIDs, textEntries);
            this.Dispose();
        }

        public override void ChangePage(int pageIndex)
        {
            // For a gump, Page is the page that is drawing.
            ActivePage = pageIndex;
        }

        private void buildGump(string[] gumpPieces, string[] gumpLines)
        {
            int currentGUMPPage = 0;

            for (int i = 0; i < gumpPieces.Length; i++)
            {
                string[] arguements = gumpPieces[i].Split(' ');
                switch (arguements[0])
                {
                    case "page":
                        currentGUMPPage = interpret_page(arguements);
                        break;
                    case "checkertrans":
                        AddControl(new Controls.CheckerTrans(this, currentGUMPPage, arguements));
                        break;
                    case "resizepic":
                        AddControl(new Controls.ResizePic(this, currentGUMPPage, arguements));
                        ((Controls.ResizePic)LastControl).CloseOnRightClick = true;
                        break;
                    case "button":
                        AddControl(new Controls.Button(this, currentGUMPPage, arguements));
                        break;
                    case "croppedtext":
                        AddControl(new Controls.CroppedText(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "htmlgump":
                        AddControl(new Controls.HtmlGump(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "gumppictiled":
                        AddControl(new Controls.GumpPicTiled(this, currentGUMPPage, arguements));
                        break;
                    case "gumppic":
                        AddControl(new Controls.GumpPic(this, currentGUMPPage, arguements));
                        break;
                    case "text":
                        AddControl(new Controls.TextLabel(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "tilepic":
                        AddControl(new Controls.TilePic(this, currentGUMPPage, arguements));
                        break;
                    case "tilepichue":
                        AddControl(new Controls.TilePic(this, currentGUMPPage, arguements));
                        break;
                    case "textentry":
                        AddControl(new Controls.TextEntry(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "textentrylimited":
                        AddControl(new Controls.TextEntry(this, currentGUMPPage, arguements, gumpLines));
                        break;

                    case "checkbox":
                        WorldInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "group":
                        WorldInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmlgump":
                        WorldInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmlgumpcolor":
                        WorldInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmltok":
                        WorldInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "buttontileart":
                        WorldInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "tooltip":
                        WorldInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "radio":
                        WorldInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    default:
                        WorldInteraction.ChatMessage("GUMP: Unknown piece '" + arguements[0] + "'.");
                        break;
                }
            }
        }

        private int interpret_page(string[] arguements)
        {
            int page = Int32.Parse(arguements[1]);
            return page;
        }

        private bool checkResize()
        {
            bool changedDimensions = false;
            if (Controls.Count > 0)
            {
                int w = 0, h = 0;
                foreach (Control c in Controls)
                {
                    if (c.Page == 0 || c.Page == this.ActivePage)
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

        protected void Quit()
        {
            UltimaVars.EngineVars.EngineRunning = false;
        }

        protected string getTextEntry(int entryID)
        {
            foreach (Control c in Controls)
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
            if (obj == null || (obj as Control) == null)
            {
                return false;
            }

            // by default, Gumps are equal to each other if they are of the same type.
            // Inheriting Gumps should override this to base equality on their Parent object's serial, if appropriate.
            if (this.GetType() == obj.GetType())
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
