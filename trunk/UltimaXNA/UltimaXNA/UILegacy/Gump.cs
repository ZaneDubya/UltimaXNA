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
        Serial GumpID;
        string[] _gumpPieces, _gumpLines;

        public Gump(Serial serial, Serial gumpID)
            : base(serial, null)
        {
            _controls = new List<Control>();
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
            _manager.DebugMessage_Clear();
            base.Dispose();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (_gumpPieces != null)
            {
                buildGump(_gumpPieces, _gumpLines);
                _gumpPieces = null;
                _gumpLines = null;
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

            base.Update(gameTime);
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

        private void buildGump(string[] gumpPieces, string[] gumpLines)
        {
            _manager.DebugMessage_Clear();

            for (int i = 0; i < gumpPieces.Length; i++)
            {
                string[] arguements = gumpPieces[i].Split(' ');
                switch (arguements[0])
                {
                    case "checkertrans":
                        _controls.Add(new Gumplings.CheckerTrans(0, this, arguements));
                        break;
                    case "resizepic":
                        _controls.Add(new Gumplings.ResizePic(0, this, arguements));
                        break;
                    case "button":
                        _controls.Add(new Gumplings.Button(0, this, arguements));
                        break;
                    case "croppedtext":
                        _controls.Add(new Gumplings.CroppedText(0, this, arguements, gumpLines));
                        break;
                    case "htmlgump":
                        _controls.Add(new Gumplings.HtmlGump(0, this, arguements, gumpLines));
                        break;
                    case "gumppictiled":
                        _controls.Add(new Gumplings.GumpPicTiled(0, this, arguements));
                        break;
                    case "gumppic":
                        _controls.Add(new Gumplings.GumpPic(0, this, arguements));
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
                    case "tilepic":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "tilepichue":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "text":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "radio":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "textentry":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "textentrylimited":
                        _manager.DebugMessage_AddLine("GUMP: Unhandled '" + arguements[0] + "'.");
                        break;
                    case "page":
                        interpret_page(arguements);
                        break;
                    default:
                        _manager.DebugMessage_AddLine("GUMP: Unknown piece '" + arguements[0] + "'.");
                        break;
                }
            }
        }

        private void interpret_croppedtext(string[] arguements)
        {
            int m_X, m_Y, m_Width, m_Height, m_Hue, m_Parent;
            m_X = Int32.Parse(arguements[1]);
            m_Y = Int32.Parse(arguements[2]);
            m_Width = Int32.Parse(arguements[3]);
            m_Height = Int32.Parse(arguements[4]);
            m_Hue = Int32.Parse(arguements[5]);
            m_Parent = Int32.Parse(arguements[6]);
            // Controls.Add(new Label("lbl" + (Controls.Count + 1), new Vector2(m_X, m_Y), string.Empty, Color.TransparentBlack, Color.Black, m_Width, Label.Align.Left));
            // Controls["lbl" + Controls.Count].Text = _gumpText[m_Parent];
            checkResize();
        }

        private void interpret_page(string[] arguements)
        {
            if (arguements[1] != "0")
                throw new Exception("Unknown page #");
            return;
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
