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
    public class UserInterface
    {
        private static Dictionary<string, Window> _UIWindows;
        private static SpriteBatch _spriteBatch;
        private static FormCollection _formCollection;
        private static bool _DrawForms = false;
        private static GraphicsDeviceManager _graphics;

        static bool DEBUG_drawUI = true;

        static UserInterface()
        {
            
        }

        public static void Initialize(Game game) 
        {
            /*
            _graphics = game.Services.GetService(typeof(IGraphicsDeviceService)) as GraphicsDeviceManager;
            _formCollection = new FormCollection(game.Window, game.Services, ref _graphics, @"..\..\res\");
            _spriteBatch = new SpriteBatch(_graphics.GraphicsDevice);
            UIHelper.SetObjects(_graphics.GraphicsDevice, _formCollection);
            _UIWindows = new Dictionary<string, Window>();
            _DrawForms = true;
            FormCollection.Cursor.HasShadow = false;
             */
        }

        protected static void UnloadContent()
        {
            /*
            if (_DrawForms)
            {
                //Dispose of the form collection
                _formCollection.Dispose();
            }
             */
        }

        public static void Update(GameTime gameTime)
        {
            /*
            UIHelper.Update();

            DEBUG_drawUI = false;

            if (DEBUG_drawUI)
            {
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
            }*/
        }

        public static void Draw(GameTime gameTime)
        {
            /*
            if (DEBUG_drawUI)
            {
                //Draw the form collection
                if (_DrawForms)
                {
                    _formCollection.Draw();
                }
            }*/
        }

        private static void drawText(SpriteBatch spriteBatch, string text, int font, int hue, int x, int y)
        {
            Texture2D texture = UIHelper.TextTexture(text, font, hue);
            spriteBatch.Draw(texture, new Rectangle(x, y, texture.Width, texture.Height), Color.White);
        }

        public void Dispose()
        {

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