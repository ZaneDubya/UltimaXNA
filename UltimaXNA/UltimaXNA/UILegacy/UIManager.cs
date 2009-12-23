using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Extensions;
using UltimaXNA.Graphics;
using UltimaXNA.Input;
using UltimaXNA.TileEngine;

namespace UltimaXNA.UILegacy
{
    public class UIManager : DrawableGameComponent, IUIManager
    {
        ExtendedSpriteBatch _spriteBatch;
        List<Control> _controls = null;
        Cursor _cursor = null;
        IInputService _input = null;

        SpriteBatch3D _spriteBatch3D;

        Control[] _mouseOverControls = null; // the controls that the mouse is over, 0 index is the frontmost control, last index is the backmost control (always the owner gump).
        Control[] _mouseDownControl = new Control[5]; // the control that the mouse was over when the button was clicked. 5 buttons
        public Control MouseOverControl
        {
            get
            {
                if (_mouseOverControls == null)
                    return null;
                else
                    return _mouseOverControls[0];
            }
        }

        public bool IsMouseOverUI
        {
            get
            {
                if (MouseOverControl == null)
                    return false;
                else
                    return true;
            }
        }
        public Cursor Cursor
        {
            get
            {
                return _cursor;
            }
        }

        public UIManager(Game game)
            : base(game)
        {
            _spriteBatch = new ExtendedSpriteBatch(game.GraphicsDevice);
            _spriteBatch.Effect = game.Content.Load<Effect>("Shaders\\Gumps");

            _spriteBatch3D = new SpriteBatch3D(game);

            _controls = new List<Control>();
            _cursor = new Cursor(this);

            // Retrieve the needed services.
            _input = game.Services.GetService<IInputService>(true);

            _controls.Add(new Clientside.DebugGump(0x1));
        }

        public Gump AddGump(Serial serial, Serial gumpID, string[] gumplings, string[] lines, int x, int y)
        {
            Gump g = new Gump(serial, gumpID, gumplings, lines);
            g.Position = new Vector2(x, y);
            _controls.Add(g);
            return g;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Control c in _controls)
            {
                if (!c.IsInitialized)
                    c.Initialize(this);
                c.Update(gameTime);

            }

            updateInput();

            List<Control> disposedControls = new List<Control>();
            foreach (Control c in _controls)
            {
                if (c.IsDisposed)
                    disposedControls.Add(c);
            }
            foreach (Control c in disposedControls)
            {
                _controls.Remove(c);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Effect.Parameters["HueTexture"].SetValue(UltimaXNA.Data.HuesXNA.HueTexture);
            _spriteBatch.Begin();

            z = 10000000;

            foreach (Control c in _controls)
            {
                c.Draw(_spriteBatch);
            }

            // Draw debug message
            if (GameState.DebugMessage != null)
                DEBUG_DrawText(new Vector2(5, 5), GameState.DebugMessage + Environment.NewLine + _DEBUG_TEXT);
            // version message
            Version v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime d = new DateTime(v.Build * TimeSpan.TicksPerDay).AddYears(1999).AddDays(-1);
            string versionString = string.Format("UltimaXNA PreAlpha v{0}.{1}", v.Major, v.Minor) + Environment.NewLine +
                "Compiled: " + String.Format("{0:MMMM d, yyyy}", d);
            DEBUG_DrawText(new Vector2(630, 5), versionString);

            _cursor.Draw(_spriteBatch, _input.CurrentMousePosition);

            _spriteBatch.End();

            _spriteBatch3D.FlushOld(false);

            base.Draw(gameTime);
        }

        int z;
        internal void DEBUG_DrawText(Vector2 position, string text)
        {
            Texture2D t = Data.UniText.GetTextTexture(text, 1, false);
            Draw(t, position, 1152);
        }
        internal void Draw(Texture2D texture, Vector2 position, int hue)
        {
            _spriteBatch3D.DrawSimple(texture, new Vector3(position.X, position.Y, z), new Vector2(hue, 0));
            z += 1000;
        }

        string _DEBUG_TEXT = string.Empty;
        public void DebugMessage_AddLine(string line)
        {
            _DEBUG_TEXT += line + Environment.NewLine;
        }
        public void DebugMessage_Clear()
        {
            _DEBUG_TEXT = string.Empty;
        }

        void updateInput()
        {
            Control[] focusedControls = null;

            foreach (Control c in _controls)
            {
                Control[] mouseOverControls = c.HitTest(_input.CurrentMousePosition);
                if (mouseOverControls != null)
                {
                    focusedControls = mouseOverControls;
                }
            }

            _mouseOverControls = focusedControls;

            for (int iButton = 0; iButton < 5; iButton++)
            {
                if (_mouseOverControls != null)
                {
                    // MouseDown event.
                    if (_input.IsMouseButtonPress((MouseButtons)iButton))
                    {
                        for (int iControl = 0; iControl < _mouseOverControls.Length; iControl++)
                        {
                            if (_mouseOverControls[iControl].HandlesInput)
                            {
                                _mouseOverControls[iControl].MouseDown(_input.CurrentMousePosition, iButton);
                                _mouseDownControl[iButton] = _mouseOverControls[iControl];
                                break;
                            }
                        }
                    }
                }


                // MouseUp and MouseClick events
                if (_input.IsMouseButtonRelease((MouseButtons)iButton))
                {
                    if (_mouseDownControl[iButton] != null)
                    {
                        _mouseDownControl[iButton].MouseUp(_input.CurrentMousePosition, iButton);
                        if (_mouseOverControls != null)
                        {
                            if (_mouseOverControls[0] == _mouseDownControl[iButton])
                            {
                                _mouseDownControl[iButton].MouseClick(_input.CurrentMousePosition, iButton);
                            }
                        }
                    }
                }

                if (_input.IsMouseButtonUp((MouseButtons)iButton))
                {
                    _mouseDownControl[iButton] = null;
                }
            }

            // MouseOver event.
            if (_mouseOverControls != null)
            {
                for (int iControl = 0; iControl < _mouseOverControls.Length; iControl++)
                {
                    if (_mouseOverControls[iControl].HandlesInput)
                    {
                        _mouseOverControls[iControl].MouseOver(_input.CurrentMousePosition);
                        break;
                    }
                }
            }
        }

        string[] splitGumpPieces(string gumpData)
        {
            List<string> i = new List<string>(); ;
            bool isData = true;
            int dataIndex = 0;
            while (isData)
            {
                if (gumpData.Substring(dataIndex) == "\0")
                {
                    isData = false;
                }
                else
                {
                    int begin = gumpData.IndexOf("{ ", dataIndex);
                    int end = gumpData.IndexOf(" }", dataIndex + 1);
                    if ((begin != -1) && (end != -1))
                    {
                        string sub = gumpData.Substring(begin + 2, end - begin - 2);
                        // iConstruct = iConstruct.Substring(0, iBeginReplace) + iArgs[i] + iConstruct.Substring(iEndReplace + 1, iConstruct.Length - iEndReplace - 1);
                        i.Add(sub);
                        dataIndex += (end - begin) + 2;
                    }
                    else
                    {
                        isData = false;
                    }
                }
            }

            return i.ToArray();
        }
    }
}
