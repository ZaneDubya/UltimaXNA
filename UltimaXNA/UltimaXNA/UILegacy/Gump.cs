using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.UILegacy.Gumplings;
using UltimaXNA.Graphics;

namespace UltimaXNA.UILegacy
{
    public class Gump : Control
    {
        Serial Serial;
        Serial GumpID;
        string[] _gumpPieces, _gumpLines;
        bool _mustRebuild = false;

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
            _manager.DebugMessage_Clear();
            base.Dispose();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Add any gump pieces that have been given to the gump...
            if (_gumpPieces != null)
            {
                buildGump(_gumpPieces, _gumpLines);
                _gumpPieces = null;
                _gumpLines = null;
                _mustRebuild = true;
            }

            // Update the gumplings...
            base.Update(gameTime);

            // Do we need to resize?
            if (_mustRebuild)
            {
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
        }

        RenderTarget2D _gumpTarget = null;
        Texture2D _gumpTexture = null;

        public override void Draw(ExtendedSpriteBatch spriteBatch)
        {
            if (_gumpTarget == null)
            {
                _gumpTarget = new RenderTarget2D(spriteBatch.GraphicsDevice, Width, Height, 1, SurfaceFormat.Color);
            }

            spriteBatch.End();
            spriteBatch.GraphicsDevice.SetRenderTarget(0, _gumpTarget);
            spriteBatch.GraphicsDevice.Clear(Color.TransparentBlack);
            
            spriteBatch.Begin();
            base.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(0, null);
            _gumpTexture = _gumpTarget.GetTexture();
            spriteBatch.Begin();
            spriteBatch.Draw(_gumpTexture, Position, Color.White);
        }

        public override void Activate(Control c)
        {
            int[] switchIDs = new int[0];
            Network.Pair<short, string>[] textEntries = new UltimaXNA.Network.Pair<short,string>[0];
            Client.UltimaClient.Send(new Network.Packets.Client.GumpMenuSelectPacket(
                this.Serial, this.GumpID, ((Button)c).ButtonID, switchIDs, textEntries));
            this.Dispose();
        }

        public override void ChangePage(Control c)
        {
            // For a gump, Page is the page that is drawing.
            ActivePage = ((Button)c).ButtonParameter;
            _mustRebuild = true;
        }

        public Control AddGumpling(Control c)
        {
            _controls.Add(c);
            _mustRebuild = true;
            return _controls[_controls.Count - 1];
        }

        private void buildGump(string[] gumpPieces, string[] gumpLines)
        {
            _manager.DebugMessage_Clear();
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
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "group":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmlgump":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmlgumpcolor":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "xmfhtmltok":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "buttontileart":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "tooltip":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "radio":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    default:
                        _manager.DebugMessage_AddLine("GUMP: Unknown piece '" + arguements[0] + "'.");
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
            foreach (Control c in _controls)
            {
                if (Width < c.X + c.Width)
                {
                    Width = c.X + c.Width;
                    changedDimensions = true;
                }
                if (Height < c.Y + c.Height)
                {
                    Height = c.Y + c.Height;
                    changedDimensions = true;
                }
            }
            return changedDimensions;
        }
    }
}
