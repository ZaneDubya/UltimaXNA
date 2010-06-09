using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.UILegacy.Gumplings;

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
            _controls = new List<Control>();
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
                if (_gumpTarget != null)
                {
                    _gumpTarget.Dispose();
                    _gumpTexture.Dispose();
                    _gumpTarget = null;
                }
            }
        }

        RenderTarget2D _gumpTarget = null;
        Texture2D _gumpTexture = null;

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            // don't draw any server gumps until we're in the world.
            if (IsServerGump && !ClientVars.InWorld)
                return;
            if (_controls.Count == 0)
                return;

            spriteBatch.Flush();

            if (_renderFullScreen)
            {
                InputMultiplier = (float)spriteBatch.GraphicsDevice.Viewport.Width / (float)Width;

                if (_gumpTarget == null)
                {
                    // the render target CANNOT be larger than the viewport.
                    int w = Width < _manager.Width ? Width : _manager.Width;
                    int h = Height < _manager.Height ? Height : _manager.Height;
                    _gumpTarget = new RenderTarget2D(spriteBatch.GraphicsDevice, w, h, 1, SurfaceFormat.Color);
                }

                spriteBatch.GraphicsDevice.SetRenderTarget(0, _gumpTarget);
                spriteBatch.GraphicsDevice.Clear(Color.TransparentBlack);

                base.Draw(spriteBatch);
                spriteBatch.Flush();

                spriteBatch.GraphicsDevice.SetRenderTarget(0, null);
                _gumpTexture = _gumpTarget.GetTexture();

                if (_renderFullScreen)
                {
                    spriteBatch.Draw(_gumpTexture, new Rectangle(0, 0, (int)(Width * InputMultiplier), (int)(Height * InputMultiplier)), 0, false);
                }
                else
                    spriteBatch.Draw(_gumpTexture, Position, 0, false);
            }
            else
            {
                base.Draw(spriteBatch);
            }
        }

        public override void ActivateByButton(int buttonID)
        {
            int[] switchIDs = new int[0];
            Network.Pair<short, string>[] textEntries = new UltimaXNA.Network.Pair<short,string>[0];
            Client.UltimaClient.Send(new Network.Packets.Client.GumpMenuSelectPacket(
                this.Serial, this.GumpID, buttonID, switchIDs, textEntries));
            this.Dispose();
        }

        public override void ChangePage(int pageIndex)
        {
            // For a gump, Page is the page that is drawing.
            ActivePage = pageIndex;
        }

        public Control AddGumpling(Control c)
        {
            _controls.Add(c);
            return this.LastGumpling;
        }

        public Control LastGumpling
        {
            get { return _controls[_controls.Count - 1]; }
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
                        _controls.Add(new Gumplings.CheckerTrans(this, currentGUMPPage, arguements));
                        break;
                    case "resizepic":
                        _controls.Add(new Gumplings.ResizePic(this, currentGUMPPage, arguements));
                        ((Gumplings.ResizePic)this.LastGumpling).CloseOnRightClick = true;
                        break;
                    case "button":
                        _controls.Add(new Gumplings.Button(this, currentGUMPPage, arguements));
                        break;
                    case "croppedtext":
                        _controls.Add(new Gumplings.CroppedText(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "htmlgump":
                        _controls.Add(new Gumplings.HtmlGump(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "gumppictiled":
                        _controls.Add(new Gumplings.GumpPicTiled(this, currentGUMPPage, arguements));
                        break;
                    case "gumppic":
                        _controls.Add(new Gumplings.GumpPic(this, currentGUMPPage, arguements));
                        break;
                    case "text":
                        _controls.Add(new Gumplings.TextLabel(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "tilepic":
                        _controls.Add(new Gumplings.TilePic(this, currentGUMPPage, arguements));
                        break;
                    case "tilepichue":
                        _controls.Add(new Gumplings.TilePic(this, currentGUMPPage, arguements));
                        break;
                    case "textentry":
                        _controls.Add(new Gumplings.TextEntry(this, currentGUMPPage, arguements, gumpLines));
                        break;
                    case "textentrylimited":
                        _controls.Add(new Gumplings.TextEntry(this, currentGUMPPage, arguements, gumpLines));
                        break;

                    case "checkbox":
                        _manager.AddMessage_Debug("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "group":
                        _manager.AddMessage_Debug("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmlgump":
                        _manager.AddMessage_Debug("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmlgumpcolor":
                        _manager.AddMessage_Debug("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmltok":
                        _manager.AddMessage_Debug("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "buttontileart":
                        _manager.AddMessage_Debug("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "tooltip":
                        _manager.AddMessage_Debug("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "radio":
                        _manager.AddMessage_Debug("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    default:
                        _manager.AddMessage_Debug("GUMP: Unknown piece '" + arguements[0] + "'.");
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
            int w = 0, h = 0;
            foreach (Control c in _controls)
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

            return changedDimensions;
        }

        protected void Quit()
        {
            ClientVars.EngineRunning = false;
        }
    }
}
