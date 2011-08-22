using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Client;
using UltimaXNA.UILegacy.Gumplings;
using UltimaXNA.Graphics;

namespace UltimaXNA.UILegacy
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
            if (IsServerGump && !ClientVars.InWorld)
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
            if (IsServerGump && !ClientVars.InWorld)
                return;

            if (_renderFullScreen)
            {
                InputMultiplier = (float)spriteBatch.GraphicsDevice.Viewport.Width / (float)Width;

                if (_gumpTexture == null)
                {
                    // the render target CANNOT be larger than the viewport.
                    int w = Width < _manager.Width ? Width : _manager.Width;
                    int h = Height < _manager.Height ? Height : _manager.Height;
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
            UltimaClient.Send(new Client.Packets.Client.GumpMenuSelectPacket(
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
                        _manager.AddMessage_Chat("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "group":
                        _manager.AddMessage_Chat("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmlgump":
                        _manager.AddMessage_Chat("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmlgumpcolor":
                        _manager.AddMessage_Chat("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmltok":
                        _manager.AddMessage_Chat("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "buttontileart":
                        _manager.AddMessage_Chat("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "tooltip":
                        _manager.AddMessage_Chat("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "radio":
                        _manager.AddMessage_Chat("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    default:
                        _manager.AddMessage_Chat("GUMP: Unknown piece '" + arguements[0] + "'.");
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
            ClientVars.EngineRunning = false;
        }
    }
}
