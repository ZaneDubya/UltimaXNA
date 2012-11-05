﻿/***************************************************************************
 *   Gump.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Interface.Graphics;
using UltimaXNA.Interface.GUI;

namespace UltimaXNA.UltimaGUI
{
    public class Gump : Control
    {
        public Serial Serial;
        Serial GumpID;
        string[] _gumpPieces, _gumpLines;

        public bool IsServerGump { get; set; }

        public Gump(Serial serial, Serial gumpID)
            : base(null, 0)
        {
            Serial = serial;
            GumpID = gumpID;
        }

        public Gump(Serial serial, Serial gumpID, String[] pieces, String[] textlines)
            : this(serial, gumpID)
        {
            _gumpPieces = pieces;
            _gumpLines = textlines;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // don't process any server gumps until we're in the world.
            if (IsServerGump && !UltimaVars.EngineVars.InWorld)
                return;

            // Add any gump pieces that have been given to the gump...
            if (_gumpPieces != null)
            {
                buildGump(_gumpPieces, _gumpLines);
                _gumpPieces = null;
                _gumpLines = null;
            }

            // If page = 0, then we've just created this page. We initialize activepage to 1.
            // This triggers the additional functionality in Control.ActivePage.Set().
            if (ActivePage == 0)
                ActivePage = 1;

            // Update the gumplings...
            base.Update(gameTime);

            // Do we need to resize?
            if (checkResize())
            {
                if (_gumpTexture != null)
                {
                    _gumpTexture.Dispose();
                }
            }
        }

        RenderTarget2D _gumpTexture = null;

        public override void Draw(SpriteBatchUI spriteBatch)
        {
            if (!Visible)
                return;
            // don't draw any server gumps until we're in the world.
            if (IsServerGump && !UltimaVars.EngineVars.InWorld)
                return;

            if (_renderFullScreen)
            {
                InputMultiplier = (float)spriteBatch.GraphicsDevice.Viewport.Width / (float)Width;

                if (_gumpTexture == null)
                {
                    // the render target CANNOT be larger than the viewport.
                    int w = Width < UserInterface.Width ? Width : UserInterface.Width;
                    int h = Height < UserInterface.Height ? Height : UserInterface.Height;
                    _gumpTexture = new RenderTarget2D(spriteBatch.GraphicsDevice, w, h, false, SurfaceFormat.Color, DepthFormat.Depth16);
                }

                spriteBatch.GraphicsDevice.SetRenderTarget(_gumpTexture);
                spriteBatch.GraphicsDevice.Clear(Color.Transparent);

                base.Draw(spriteBatch);
                spriteBatch.Flush();

                spriteBatch.GraphicsDevice.SetRenderTarget(null);

                if (_renderFullScreen)
                {
                    spriteBatch.Draw2D(_gumpTexture, new Rectangle(0, 0, (int)(Width * InputMultiplier), (int)(Height * InputMultiplier)), 0, false, false);
                }
                else
                    spriteBatch.Draw2D(_gumpTexture, Position, 0, false, false);
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
            Pair<short, string>[] textEntries = new Pair<short,string>[0];
            UltimaClient.Send(new Network.Packets.Client.GumpMenuSelectPacket(
                this.Serial, this.GumpID, buttonID, switchIDs, textEntries));
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
                        AddControl(new Gumplings.CheckerTrans(this, currentGUMPPage, arguements));
                        break;
                    case "resizepic":
                        AddControl(new Gumplings.ResizePic(this, currentGUMPPage, arguements));
                        ((Gumplings.ResizePic)LastControl).CloseOnRightClick = true;
                        break;
                    case "button":
                        AddControl(new Gumplings.Button(this, currentGUMPPage, arguements));
                        break;
                    case "croppedtext":
                        AddControl(new Gumplings.CroppedText(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "htmlgump":
                        AddControl(new Gumplings.HtmlGump(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "gumppictiled":
                        AddControl(new Gumplings.GumpPicTiled(this, currentGUMPPage, arguements));
                        break;
                    case "gumppic":
                        AddControl(new Gumplings.GumpPic(this, currentGUMPPage, arguements));
                        break;
                    case "text":
                        AddControl(new Gumplings.TextLabel(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "tilepic":
                        AddControl(new Gumplings.TilePic(this, currentGUMPPage, arguements));
                        break;
                    case "tilepichue":
                        AddControl(new Gumplings.TilePic(this, currentGUMPPage, arguements));
                        break;
                    case "textentry":
                        AddControl(new Gumplings.TextEntry(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "textentrylimited":
                        AddControl(new Gumplings.TextEntry(this, currentGUMPPage, arguements, gumpLines));
                        break;

                    case "checkbox":
                        UltimaInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "group":
                        UltimaInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmlgump":
                        UltimaInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmlgumpcolor":
                        UltimaInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmltok":
                        UltimaInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "buttontileart":
                        UltimaInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "tooltip":
                        UltimaInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "radio":
                        UltimaInteraction.ChatMessage("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    default:
                        UltimaInteraction.ChatMessage("GUMP: Unknown piece '" + arguements[0] + "'.");
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
                if (c.GetType() == typeof(UltimaGUI.Gumplings.TextEntry))
                {
                    UltimaGUI.Gumplings.TextEntry g = (UltimaGUI.Gumplings.TextEntry)c;
                    if (g.EntryID == entryID)
                        return g.Text;
                }
            }
            return string.Empty;
        }
    }
}
