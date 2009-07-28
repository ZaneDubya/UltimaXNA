/***************************************************************************
 *   ScreenManager.cs
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
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UltimaXNA.Diagnostics;
using UltimaXNA;
#endregion

namespace UltimaXNA.SceneManagement
{
    public static class SceneManager
    {
        static IScene _currentScene;
        static ILoggingService _loggingService;
        static bool _isTransitioning = false;

        public static bool IsTransitioning
        {
            get { return _isTransitioning; }
        }

        private static Game _game;
        public static Game Game { get { return _game; } }

        public static IScene CurrentScene
        {
            get { return _currentScene; }
            set
            {
                if (_isTransitioning)
                    return;

                _isTransitioning = true;

                if (_currentScene != null)
                {
                    _loggingService.Debug("Starting scene transition from {0} to {1}", _currentScene.GetType().Name, value.GetType().Name);
                    _currentScene.SceneState = SceneState.TransitioningOff;

                    _currentScene.TransitionComplete += new TransitionCompleteHandler(delegate()
                    {
                        _loggingService.Debug("Scene transition complete.");
                        _loggingService.Debug("Disposing {0}.", _currentScene.GetType().Name);

                        _currentScene.Dispose();
                        _currentScene = value;

                        if (!_currentScene.IsInitialized)
                        {
                            _loggingService.Debug("Initializing {0}.", _currentScene.GetType().Name);
                            _currentScene.Intitialize();
                        }

                        _isTransitioning = false;
                    });
                }
                else
                {
                    _loggingService.Debug("Starting scene {0}", value.GetType().Name);
                    _currentScene = value;

                    if (!_currentScene.IsInitialized)
                    {
                        _loggingService.Debug("Initializing {0}.", _currentScene.GetType().Name);
                        _currentScene.Intitialize();
                    }

                    _isTransitioning = false;
                }
            }
        }

        static SceneManager()
        {
        }

        public static void Initialize(Game game)
        {
            _game = game;
            _loggingService = _game.Services.GetService<ILoggingService>(true);
        }

        public static void Update(GameTime gameTime)
        {
            IScene current = _currentScene;

            if (_currentScene != null)
                _currentScene.Update(gameTime);

            //This is just incase a scene changes in the middle of updating.
            if (current != _currentScene)
                _currentScene.Update(gameTime);
        }

        public static void Draw(GameTime gameTime)
        {
            if (_currentScene != null)
            {
                _currentScene.OnBeforeDraw(gameTime);
                _currentScene.Draw(gameTime);
                _currentScene.OnAfterDraw(gameTime);
            }
        }
    }
}
