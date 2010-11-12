using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using UltimaXNA.Entities;
using UltimaXNA.Extensions;
using UltimaXNA.Input;
using UltimaXNA.Input.Events;
using UltimaXNA.TileEngine;

namespace UltimaXNA.UILegacy
{
    public class UIManager : DrawableGameComponent, IUIManager
    {
        List<Control> _controls = null;
        Cursor _cursor = null;
        IInputState _input = null;

        ExtendedSpriteBatch _spriteBatch;
        public int Width { get { return _spriteBatch.GraphicsDevice.Viewport.Width; } }
        public int Height { get { return _spriteBatch.GraphicsDevice.Viewport.Height; } }
        public bool IsModalMsgBoxOpen { get { return (GetGump<MsgBox>(0) != null); } }

        ChatHandler _debugMessages = new ChatHandler();
        ChatHandler _chatMessages = new ChatHandler();

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

        // Keyboard-handling control 'announce' themselves when they are created. But only the first one per update
        // cycle is recognized.
        bool _keyboardHandlingControlAnnouncedThisRound = false;
        Control _keyboardFocusControl = null;
        public Control KeyboardFocusControl
        {
            get
            {
                if (IsModalMsgBoxOpen)
                    return null;
                if (_keyboardFocusControl == null)
                    return null;
                return _keyboardFocusControl;
            }
            set
            {
                _keyboardFocusControl = value;
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
        public Cursor Cursor { get { return _cursor; } }

        EventLogout OnRequestLogout;
        public void AddRequestLogoutNotifier(EventLogout methodGroup)
        {
            OnRequestLogout += methodGroup;
        }
        

        public UIManager(Game game)
            : base(game)
        {
            _spriteBatch = new ExtendedSpriteBatch(game);

            _controls = new List<Control>();
            _cursor = new Cursor(this);

            // Retrieve the needed services.
            _input = game.Services.GetService<IInputState>(true);
        }

        internal void RequestLogout()
        {
            if (OnRequestLogout != null)
                OnRequestLogout();
        }

        public MsgBox MsgBox(string msg, MsgBoxTypes type)
        {
            // pop up an error message, modal.
            MsgBox g = new MsgBox(msg, type);
            _controls.Add(g);
            return g;
        }

        public Gump ToggleGump_Local(Gump gump, int x, int y)
        {
            Control removeControl = null;
            foreach (Control c in _controls)
            {
                if (c.GetType() == gump.GetType())
                {
                    removeControl = c;
                    break; 
                }
            }

            if (removeControl != null)
                _controls.Remove(removeControl);
            else
            {
                _controls.Add(gump);
                gump.Position = new Point2D(x, y);
            }
            return gump;
        }

        public Gump AddGump_Server(Serial serial, Serial gumpID, string[] gumplings, string[] lines, int x, int y)
        {
            Gump g = new Gump(serial, gumpID, gumplings, lines);
            g.Position = new Point2D(x, y);
            g.IsServerGump = true;
            g.IsMovable = true;
            _controls.Add(g);
            return g;
        }

        public Gump AddGump_Local(Gump gump, int x, int y)
        {
            gump.Position = new Point2D(x, y);
            _controls.Add(gump);
            return gump;
        }

        public Gump GetGump(Serial serial)
        {
            foreach (Gump g in _controls)
            {
                if (g.Serial == serial)
                    return g;
            }
            return null;
        }

        public T GetGump<T>(Serial serial) where T : Gump
        {
            foreach (Gump g in _controls)
            {
                if (g.Serial == serial)
                    if (g.GetType() == typeof(T))
                        return (T)g;
            }
            return null;
        }

        public Gump AddContainerGump(Entity containerItem, int gumpID)
        {
            Gump g = new ClientsideGumps.ContainerGump(containerItem, gumpID);
            g.Position = new Point2D(64, 64);
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

            update_Chat(gameTime);

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

        void update_Chat(GameTime gameTime)
        {
            Gump g = this.GetGump<ClientsideGumps.ChatWindow>(0);
            if (g != null)
            {
                foreach (ChatLine c in _chatMessages.GetMessages())
                {
                    ((ClientsideGumps.ChatWindow)g).AddLine(c.Text);
                }
                _chatMessages.Clear();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (Control c in _controls)
            {
                if (c.IsInitialized)
                    c.Draw(_spriteBatch);
            }

            // Draw debug message
            if (ClientVars.DebugMessage != null)
                DEBUG_DrawText(new Point2D(5, 5), ClientVars.DebugMessage + Environment.NewLine + _DEBUG_TEXT(gameTime));

            // Draw the cursor
            _cursor.Draw(_spriteBatch, _input.MousePosition);

            _spriteBatch.Flush();

            base.Draw(gameTime);
        }

        public void Reset()
        {
            foreach (Control c in _controls)
            {
                c.Dispose();
            }
            _controls.Clear();
        }

        internal void DEBUG_DrawText(Point2D position, string text)
        {
            Texture2D t = Data.UniText.GetTexture(text);
            _spriteBatch.Draw2D(t, position, 0, false);
        }

        internal string _DEBUG_TEXT(GameTime gameTime)
        {
            _debugMessages.Update(gameTime, true);
            List<ChatLine> list = _debugMessages.GetMessages();

            string s = string.Empty;
            
            foreach (ChatLine c in list)
            {
                s += c.Text + Environment.NewLine;
            }

            return s;
        }

        public void AddMessage_Debug(string line)
        {
            _debugMessages.AddMessage(line);
        }

        public void AddMessage_Chat(string text)
        {
            _chatMessages.AddMessage(text);
        }

        public void AddMessage_Chat(string text, int hue, int font)
        {
            _chatMessages.AddMessage(text, hue, font);
        }

        internal void AnnounceNewKeyboardHandler(Control c)
        {
            // Pass null to CLEAR the keyboardhandlingcontrol.
            if (c == null)
            {
                _keyboardHandlingControlAnnouncedThisRound = false;
                _keyboardFocusControl = null;
            }
            else
            {
                if (c.HandlesKeyboardFocus)
                {
                    if (_keyboardHandlingControlAnnouncedThisRound == false)
                    {
                        _keyboardHandlingControlAnnouncedThisRound = true;
                        _keyboardFocusControl = c;
                    }
                }
            }
        }

        void updateInput()
        {
            _keyboardHandlingControlAnnouncedThisRound = false;

            Control[] focusedControls = null;
            List<Control> workingControls;
            if (IsModalMsgBoxOpen)
            {
                workingControls = new List<Control>();
                foreach (Control c in _controls)
                    if (c.GetType() == typeof(MsgBox))
                        workingControls.Add(c);
            }
            else
            {
                workingControls = _controls;
            }

            // Get the list of controls under the mouse cursor
            foreach (Control c in workingControls)
            {
                Control[] mouseOverControls = c.HitTest(_input.MousePosition);
                if (mouseOverControls != null)
                {
                    focusedControls = mouseOverControls;
                }
            }

            // MouseOver event.
            Control controlGivenMouseOver = null;
            if (focusedControls != null)
            {
                for (int iControl = 0; iControl < focusedControls.Length; iControl++)
                {
                    if (focusedControls[iControl].HandlesMouseInput)
                    {
                        // mouse over for the moused over control
                        focusedControls[iControl].MouseOver(_input.MousePosition);
                        controlGivenMouseOver = focusedControls[iControl];
                        if (MouseOverControl != null && controlGivenMouseOver != MouseOverControl)
                            MouseOverControl.MouseOut(_input.MousePosition);
                        break;
                    }
                }
            }

            // mouse over for any controls moused down on (have focus, in more common parlance)
            for (int iButton = 0; iButton < 5; iButton++)
            {
                if (_mouseDownControl[iButton] != null && _mouseDownControl[iButton] != controlGivenMouseOver)
                    _mouseDownControl[iButton].MouseOver(_input.MousePosition);
            }


            List<InputEventMouse> events = _input.GetMouseEvents();
            foreach (InputEventMouse e in events)
            {
                // MouseDown event.
                if (e.EventType == MouseEvent.Down)
                {
                    if (focusedControls != null)
                    {
                        for (int iControl = 0; iControl < focusedControls.Length; iControl++)
                        {
                            if (focusedControls[iControl].HandlesMouseInput)
                            {
                                focusedControls[iControl].MouseDown(_input.MousePosition, e.Button);
                                // if we're over a keyboard-handling control and press lmb, then give focus to the control.
                                if (focusedControls[iControl].HandlesKeyboardFocus)
                                    _keyboardFocusControl = focusedControls[iControl];
                                _mouseDownControl[(int)e.Button] = focusedControls[iControl];
                                break;
                            }
                        }
                    }
                }

                // MouseUp and MouseClick events
                if (e.EventType == MouseEvent.Up)
                {
                    if (Cursor.IsHolding && focusedControls != null)
                    {
                        if (e.Button == MouseButton.Left)
                        {
                            int x = (int)_input.MousePosition.X - Cursor.HoldingOffset.X - (focusedControls[0].X + focusedControls[0].Owner.X);
                            int y = (int)_input.MousePosition.Y - Cursor.HoldingOffset.Y - (focusedControls[0].Y + focusedControls[0].Owner.Y);
                            focusedControls[0].ItemDrop(Cursor.HoldingItem, x, y);
                        }
                    }

                    if (focusedControls != null)
                    {
                        if (_mouseDownControl[(int)e.Button] != null && focusedControls[0] == _mouseDownControl[(int)e.Button])
                        {
                            focusedControls[0].MouseClick(_input.MousePosition, e.Button);
                        }
                        focusedControls[0].MouseUp(_input.MousePosition, e.Button);
                        if (_mouseDownControl[(int)e.Button] != null && focusedControls[0] != _mouseDownControl[(int)e.Button])
                        {
                            _mouseDownControl[(int)e.Button].MouseUp(_input.MousePosition, e.Button);
                        }
                    }
                    else
                    {
                        if (_mouseDownControl[(int)e.Button] != null)
                        {
                            _mouseDownControl[(int)e.Button].MouseUp(_input.MousePosition, e.Button);
                        }
                    }

                    _mouseDownControl[(int)e.Button] = null;
                }
            }

            _mouseOverControls = focusedControls;

            if (KeyboardFocusControl != null)
            {
                if (_keyboardFocusControl.IsDisposed)
                {
                    _keyboardFocusControl = null;
                }
                else
                {
                    List<InputEventKeyboard> k_events = _input.GetKeyboardEvents();
                    foreach (InputEventKeyboard e in k_events)
                    {
                        if (e.EventType == KeyboardEvent.Press)
                            _keyboardFocusControl.KeyboardInput(e);
                    }
                }
            }
        }
    }
}
