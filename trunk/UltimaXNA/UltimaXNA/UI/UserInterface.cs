/***************************************************************************
 *   UI.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   begin                : May 31, 2009
 *   email                : poplicola@ultimaxna.com
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Entities;
using xWinFormsLib;
#endregion

namespace UltimaXNA.UI
{
    public class UserInterface : IUserInterface
    {
        private static Dictionary<string, Window> _UIWindows;
        private static SpriteBatch _spriteBatch;
        private static FormCollection _formCollection;
        private static bool _DrawForms = false;
        private static GraphicsDeviceManager _graphics;

        private static int _mouseCursorIndex = int.MinValue; // set so it is always properly initialized to zero in Initialize();
        private const int _mouseCursorHolding = int.MaxValue;
        private const int _mouseCursorTargetting = int.MaxValue - 1;
        public static int MouseCursor
        {
            get { return _mouseCursorIndex; }
            set
            {
                // Only change the mouse cursor when you need to.
                if (_mouseCursorIndex != value || value == 0)
                {
                    _mouseCursorIndex = value;
                    switch (_mouseCursorIndex)
                    {
                        case 0:
                            // movement. Angle the cursor appropriately.
                            Vector2 cursorCenter;
                            int cursorTextureID;
                            Texture2D cursorTexture;
                            if (GameState.InWorld)
                            {
                                switch (GameState.CursorDirection)
                                {
                                    case Direction.North:
                                        cursorCenter = new Vector2(29, 1);
                                        cursorTextureID = 8299;
                                        break;
                                    case Direction.Right:
                                        cursorCenter = new Vector2(41, 9);
                                        cursorTextureID = 8300;
                                        break;
                                    case Direction.East:
                                        cursorCenter = new Vector2(36, 24);
                                        cursorTextureID = 8301;
                                        break;
                                    case Direction.Down:
                                        cursorCenter = new Vector2(14, 33);
                                        cursorTextureID = 8302;
                                        break;
                                    case Direction.South:
                                        cursorCenter = new Vector2(4, 28);
                                        cursorTextureID = 8303;
                                        break;
                                    case Direction.Left:
                                        cursorCenter = new Vector2(2, 10);
                                        cursorTextureID = 8304;
                                        break;
                                    case Direction.West:
                                        cursorCenter = new Vector2(1, 1);
                                        cursorTextureID = 8305;
                                        break;
                                    case Direction.Up:
                                        cursorCenter = new Vector2(4, 2);
                                        cursorTextureID = 8298;
                                        break;
                                    default:
                                        cursorCenter = new Vector2(2, 10);
                                        cursorTextureID = 8309;
                                        break;
                                }

                                // Hue the cursor if in warmode.
                                if (GameState.WarMode)
                                    cursorTexture = Data.Art.GetStaticTexture(cursorTextureID - 23);
                                else
                                    cursorTexture = Data.Art.GetStaticTexture(cursorTextureID);
                            }
                            else
                            {
                                // not in world. Display a default cursor.
                                cursorCenter = new Vector2(1, 1);
                                cursorTextureID = 8305;
                                cursorTexture = Data.Art.GetStaticTexture(cursorTextureID);
                            }
                            FormCollection.Cursor.Center = cursorCenter;
                            FormCollection.Cursor.Texture = cursorTexture;
                            FormCollection.Cursor.SourceRect = new Rectangle(1, 1, FormCollection.Cursor.Texture.Width - 2, FormCollection.Cursor.Texture.Height - 2);
                            break;
                        case _mouseCursorTargetting:
                            // target
                            FormCollection.Cursor.Center = new Vector2(13, 13);
                            FormCollection.Cursor.Texture = Data.Art.GetStaticTexture(8310);
                            FormCollection.Cursor.SourceRect = new Rectangle(1, 1, 46, 34);
                            break;
                        case _mouseCursorHolding:
                            // holding something.
                            FormCollection.Cursor.Center = new Vector2(0, 0);
                            FormCollection.Cursor.Texture = UIHelper.ItemIcon(((Item)UIHelper.MouseHoldingItem));
                            FormCollection.Cursor.SourceRect = new Rectangle(0, 0, 64, 64);
                            break;
                        default:
                            // unknown cursor type. Raise an exception!
                            FormCollection.Cursor.Texture = Data.Art.GetStaticTexture(1);
                            FormCollection.Cursor.SourceRect = new Rectangle(0, 0, 44, 44);
                            break;
                    }
                }
            }
        }
        private static bool _TargettingCursor = false;
        public static bool TargettingCursor
        {
            get
            {
                return _TargettingCursor;
            }
            set
            {
                // Only change it if we have to...
                if (_TargettingCursor != value)
                {
                    _TargettingCursor = value;
                    if (_TargettingCursor == true)
                    {
                        // If we're carrying something in the mouse cursor...
                        if (MouseCursor == _mouseCursorHolding)
                        {
                            // drop it!
                            UIHelper.MouseHoldingItem = null;
                        }
                        MouseCursor = _mouseCursorTargetting;
                    }
                    else
                    {
                        // Reset the mouse cursor to default.
                        MouseCursor = 0;
                    }
                }
            }
        }

        static bool DEBUG_drawUI = true;

        static UserInterface()
        {
            
        }

        public static void Initialize(Game game) 
        {
            _graphics = game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
            _formCollection = new FormCollection(game.Window, game.Services, ref _graphics, @"..\..\res\");
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            UIHelper.SetObjects(_graphics.GraphicsDevice, _formCollection);
            _UIWindows = new Dictionary<string, Window>();
            _DrawForms = true;
            MouseCursor = 0;
            FormCollection.Cursor.HasShadow = false;
        }

        protected static void UnloadContent()
        {
            if (_DrawForms)
            {
                //Dispose of the form collection
                _formCollection.Dispose();
            }
        }

        public static void Update(GameTime gameTime)
        {
            UIHelper.Update();

            DEBUG_drawUI = true;

            if (DEBUG_drawUI)
            {
                // Fix for issue 1. http://code.google.com/p/ultimaxna/issues/detail?id=1 --ZDW 6/17/09
                if ((MouseCursor == _mouseCursorHolding) && (UIHelper.MouseHoldingItem == null))
                {
                    MouseCursor = 0;
                }
                else if (UIHelper.MouseHoldingItem != null)
                {
                    MouseCursor = _mouseCursorHolding;
                }
                else if (TargettingCursor)
                {
                    MouseCursor = _mouseCursorTargetting;
                }
                else
                {
                    // refresh the mouse cursor.
                    MouseCursor = 0;
                }

                // First update our collection of windows.
                updateWindows();
                if (_DrawForms)
                {
                    //Update the form collection
                    _formCollection.Update(gameTime);
                    if (_formCollection["msgbox"] != null)
                        _formCollection["msgbox"].Focus();
                    //Render the form collection (required before drawing)
                    _formCollection.Render();
                }
            }
        }

        public static void Draw(GameTime gameTime)
        {
            if (DEBUG_drawUI)
            {
                //Draw the form collection
                if (_DrawForms)
                {
                    _formCollection.Draw();
                }
            }

            
        }

        private static void drawText(SpriteBatch spriteBatch, string text, int font, int hue, int x, int y)
        {
            Texture2D texture = UIHelper.TextTexture(text, font, hue);
            spriteBatch.Draw(texture, new Rectangle(x, y, texture.Width, texture.Height), Color.White);
        }

        public void Dispose()
        {

        }

        public static bool IsMouseOverUI(Vector2 nPosition)
        {
            foreach (Form f in FormCollection.Forms)
            {
                if (!f.IsDisposed)
                    if (f.IsMouseOver())
                        return true;
            }
            return false;
        }

        public static Window Window(string nWindowName)
        {
            try { return _UIWindows[nWindowName]; }
            catch
            {
                // This window is not open.
                return null;
            }
        }

        public static void CloseWindow(string nWindowName)
        {
            Window w = _UIWindows[nWindowName];
            if (w != null)
                w.Close();
        }

        public static Window AddWindow(string windowName, Window window)
        {
            if (_UIWindows.ContainsKey(windowName))
            {
                if (_UIWindows[windowName].IsClosed)
                {
                    _UIWindows.Remove(windowName);
                }
                else
                {
                    _UIWindows[windowName].Close();
                    _UIWindows.Remove(windowName);
                }
            }
            _UIWindows.Add(windowName, window);
            return _UIWindows[windowName];
        }

        public static void Reset()
        {
            lock (_formCollection)
            {
                foreach (KeyValuePair<string, Window> kvp in _UIWindows)
                {
                    if (!kvp.Key.Contains("Error"))
                        CloseWindow(kvp.Key);
                }
            }
        }

        public static void PaperDoll_Open(Entity nMobileObject)
        {
            lock (_formCollection)
            {
                string iContainerKey = "PaperDoll:" + nMobileObject.Serial;
                if (_UIWindows.ContainsKey(iContainerKey))
                {
                    // focus the window
                }
                else
                {
                    _UIWindows.Add(iContainerKey, new Window_PaperDoll(nMobileObject, _formCollection));
                }
            }
        }

        public static void Container_Open(Entity nContainerObject, int nGump)
        {
            lock (_formCollection)
            {
                string iContainerKey = "Container:" + nContainerObject.Serial;
                if (_UIWindows.ContainsKey(iContainerKey))
                {
                    // focus the window
                }
                else
                {
                    _UIWindows.Add(iContainerKey, new Window_Container(nContainerObject, _formCollection));
                }
            }
        }

        public static void Merchant_Open(Entity nContainerObject, int nGump)
        {
            lock (_formCollection)
            {
                string iContainerKey = "Merchant:" + nContainerObject.Serial;
                if (_UIWindows.ContainsKey(iContainerKey))
                {
                    // focus the window
                }
                else
                {
                    _UIWindows.Add(iContainerKey, new Window_Merchant(nContainerObject, _formCollection));
                }
            }
        }

        public static void ErrorPopup_Modal(string nText)
        {
            lock (_formCollection)
            {
                if (_UIWindows.ContainsKey("ErrorModal"))
                    _UIWindows.Remove("ErrorModal");
                _UIWindows.Add("ErrorModal", new ErrorModal(_formCollection, nText));
            }
            }


        private static void updateWindows()
        {
            lock (_formCollection)
            {
                bool iMustUpdateWindowList = false;

                foreach (KeyValuePair<string, Window> w in _UIWindows)
                {
                    if (!(w.Value.IsClosed))
                        w.Value.Update();
                    else
                        iMustUpdateWindowList = true;
                }

                if (iMustUpdateWindowList)
                {
                    Dictionary<string, Window> iUIWindows = new Dictionary<string, Window>();
                    foreach (KeyValuePair<string, Window> w in _UIWindows)
                    {
                        if (!w.Value.IsClosed)
                        {
                            iUIWindows.Add(w.Key, w.Value);
                        }
                    }
                    _UIWindows = iUIWindows;
                }
            }
        }
    }
}