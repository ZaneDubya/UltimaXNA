/***************************************************************************
 *   UIManager.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using UltimaXNA.Entity;
using UltimaXNA.Core.Extensions;
using UltimaXNA.Interface.Graphics;
using UltimaXNA.Interface.Input;
using UltimaXNA.UILegacy;

namespace UltimaXNA
{
    public class UserInterface
    {
        static SpriteBatchUI m_SpriteBatch;
        static ChatHandler m_ChatHandler;

        public static int Width { get { return m_SpriteBatch.GraphicsDevice.Viewport.Width; } }
        public static int Height { get { return m_SpriteBatch.GraphicsDevice.Viewport.Height; } }

        static Cursor m_Cusor = null;
        public static Cursor Cursor { get { return m_Cusor; } }

        // All open controls:
        static List<Control> m_Controls = null;
        static List<Control> m_DisposedControls = null;
        // List of controls that the Cursor is over, with the control at index 0 being the frontmost control:
        static Control[] m_MouseOverControl = null;
        // Controls that the Cursor was over when a mouse button was clicked. We allow for five buttons:
        static Control[] m_MouseDownControl = new Control[5];
        /// <summary>
        /// Returns the control directly under the Cursor.
        /// </summary>
        public static Control MouseOverControl
        {
            get
            {
                if (m_MouseOverControl == null)
                    return null;
                else
                    return m_MouseOverControl[0];
            }
        }
        /// <summary>
        /// Returns True if the Cursor is over the UserInterface.
        /// </summary>
        public static bool IsMouseOverUI
        {
            get
            {
                if (MouseOverControl == null)
                    return false;
                else
                    return true;
            }
        }

        // Keyboard-handling control 'announce' themselves when they are created. But only the first one per update
        // cycle is recognized.
        static bool _keyboardHandlingControlAnnouncedThisRound = false;
        static Control _keyboardFocusControl = null;
        public static Control KeyboardFocusControl
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

        public static bool IsModalMsgBoxOpen { get { return (GetGump<MsgBox>(0) != null); } }

        public static void Initialize(Game game)
        {
            m_SpriteBatch = new SpriteBatchUI(game);
            m_Controls = new List<Control>();
            m_DisposedControls = new List<Control>();
            m_Cusor = new Cursor();
            m_ChatHandler = new ChatHandler();
        }

        public static MsgBox MsgBox(string msg, MsgBoxTypes type)
        {
            // pop up an error message, modal.
            MsgBox g = new MsgBox(msg, type);
            m_Controls.Add(g);
            return g;
        }

        public static Gump ToggleLocalGump(Gump gump, int x, int y)
        {
            Control removeControl = null;
            foreach (Control c in m_Controls)
            {
                if (c.GetType() == gump.GetType())
                {
                    removeControl = c;
                    break; 
                }
            }

            if (removeControl != null)
                m_Controls.Remove(removeControl);
            else
            {
                m_Controls.Add(gump);
                gump.Position = new Point2D(x, y);
            }
            return gump;
        }

        public static Gump AddGump_Server(Serial serial, Serial gumpID, string[] gumplings, string[] lines, int x, int y)
        {
            Gump g = new Gump(serial, gumpID, gumplings, lines);
            g.Position = new Point2D(x, y);
            g.IsServerGump = true;
            g.IsMovable = true;
            m_Controls.Add(g);
            return g;
        }

        public static Gump AddGump_Local(Gump gump, int x, int y)
        {
            gump.Position = new Point2D(x, y);
            m_Controls.Add(gump);
            return gump;
        }

        public static Gump GetGump(Serial serial)
        {
            foreach (Gump g in m_Controls)
            {
                if (g.Serial == serial)
                    return g;
            }
            return null;
        }

        public static T GetGump<T>(Serial serial) where T : Gump
        {
            foreach (Gump g in m_Controls)
            {
                if (g.Serial == serial)
                    if (g.GetType() == typeof(T))
                        return (T)g;
            }
            return null;
        }

        public static Gump AddContainerGump(BaseEntity containerItem, int gumpID)
        {
            foreach (Gump g in m_Controls)
            {
                if (g is UILegacy.ClientsideGumps.ContainerGump)
                    if (((UILegacy.ClientsideGumps.ContainerGump)g).ContainerSerial == containerItem.Serial)
                        g.Dispose();
            }
            Gump gump = new UILegacy.ClientsideGumps.ContainerGump(containerItem, gumpID);
            gump.Position = new Point2D(64, 64);
            m_Controls.Add(gump);
            return gump;
        }

        public static void Update(GameTime gameTime)
        {
            foreach (Control c in m_Controls)
            {
                if (!c.IsInitialized)
                    c.Initialize();
                c.Update(gameTime);
            }

            foreach (Control c in m_Controls)
                if (c.IsDisposed)
                    m_DisposedControls.Add(c);

            foreach (Control c in m_DisposedControls)
                m_Controls.Remove(c);
            m_DisposedControls.Clear();

            update_Chat(gameTime);

            updateInput();
        }

        static void update_Chat(GameTime gameTime)
        {
            Gump g = GetGump<UILegacy.ClientsideGumps.ChatWindow>(0);
            if (g != null)
            {
                foreach (ChatLine c in m_ChatHandler.GetMessages())
                {
                    ((UILegacy.ClientsideGumps.ChatWindow)g).AddLine(c.Text);
                }
                m_ChatHandler.Clear();
            }
        }

        public static void Draw(GameTime gameTime)
        {
            m_SpriteBatch.Prepare();

            foreach (Control c in m_Controls)
            {
                if (c.IsInitialized)
                    c.Draw(m_SpriteBatch);
            }

            // Draw the cursor
            m_Cusor.Draw(m_SpriteBatch, InputState.MousePosition);

            m_SpriteBatch.Flush();
        }

        public static void Reset()
        {
            foreach (Control c in m_Controls)
                c.Dispose();
        }

        public static void AddMessage_Chat(string text)
        {
            m_ChatHandler.AddMessage(text);
        }

        public static void AddMessage_Chat(string text, int hue, int font)
        {
            m_ChatHandler.AddMessage(text, hue, font);
        }

        static internal void AnnounceNewKeyboardHandler(Control c)
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

        static void updateInput()
        {
            _keyboardHandlingControlAnnouncedThisRound = false;

            Control[] focusedControls = null;
            List<Control> workingControls;
            if (IsModalMsgBoxOpen)
            {
                workingControls = new List<Control>();
                foreach (Control c in m_Controls)
                    if (c.GetType() == typeof(MsgBox))
                        workingControls.Add(c);
            }
            else
            {
                workingControls = m_Controls;
            }

            // Get the list of controls under the mouse cursor
            foreach (Control c in workingControls)
            {
                Control[] mouseOverControls = c.HitTest(InputState.MousePosition, false);
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
                        focusedControls[iControl].MouseOver(InputState.MousePosition);
                        controlGivenMouseOver = focusedControls[iControl];
                        if (MouseOverControl != null && controlGivenMouseOver != MouseOverControl)
                            MouseOverControl.MouseOut(InputState.MousePosition);
                        break;
                    }
                }
            }

            // mouse over for any controls with mouse focus
            for (int iButton = 0; iButton < 5; iButton++)
            {
                if (m_MouseDownControl[iButton] != null && m_MouseDownControl[iButton] != controlGivenMouseOver)
                    m_MouseDownControl[iButton].MouseOver(InputState.MousePosition);
            }


            List<InputEventM> events = InputState.GetMouseEvents();
            foreach (InputEventM e in events)
            {
                if (focusedControls != null)
                    e.Handled = true;

                // MouseDown event.
                if (e.EventType == MouseEvent.Down)
                {
                    if (focusedControls != null)
                    {
                        for (int iControl = 0; iControl < focusedControls.Length; iControl++)
                        {
                            if (focusedControls[iControl].HandlesMouseInput)
                            {
                                focusedControls[iControl].MouseDown(InputState.MousePosition, e.Button);
                                // if we're over a keyboard-handling control and press lmb, then give focus to the control.
                                if (focusedControls[iControl].HandlesKeyboardFocus)
                                    _keyboardFocusControl = focusedControls[iControl];
                                m_MouseDownControl[(int)e.Button] = focusedControls[iControl];
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
                            int x = (int)InputState.MousePosition.X - Cursor.HoldingOffset.X - (focusedControls[0].X + focusedControls[0].Owner.X);
                            int y = (int)InputState.MousePosition.Y - Cursor.HoldingOffset.Y - (focusedControls[0].Y + focusedControls[0].Owner.Y);
                            focusedControls[0].ItemDrop(Cursor.HoldingItem, x, y);
                        }
                    }

                    if (focusedControls != null)
                    {
                        if (m_MouseDownControl[(int)e.Button] != null && focusedControls[0] == m_MouseDownControl[(int)e.Button])
                        {
                            focusedControls[0].MouseClick(InputState.MousePosition, e.Button);
                        }
                        focusedControls[0].MouseUp(InputState.MousePosition, e.Button);
                        if (m_MouseDownControl[(int)e.Button] != null && focusedControls[0] != m_MouseDownControl[(int)e.Button])
                        {
                            m_MouseDownControl[(int)e.Button].MouseUp(InputState.MousePosition, e.Button);
                        }
                    }
                    else
                    {
                        if (m_MouseDownControl[(int)e.Button] != null)
                        {
                            m_MouseDownControl[(int)e.Button].MouseUp(InputState.MousePosition, e.Button);
                        }
                    }

                    m_MouseDownControl[(int)e.Button] = null;
                }
            }

            m_MouseOverControl = focusedControls;

            if (KeyboardFocusControl != null)
            {
                if (_keyboardFocusControl.IsDisposed)
                {
                    _keyboardFocusControl = null;
                }
                else
                {
                    List<InputEventKB> k_events = InputState.GetKeyboardEvents();
                    foreach (InputEventKB e in k_events)
                    {
                        if (e.EventType == KeyboardEvent.Press)
                            _keyboardFocusControl.KeyboardInput(e);
                    }
                }
            }
        }
    }
}
