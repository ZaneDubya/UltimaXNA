/***************************************************************************
 *   BaseScene.cs
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
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UltimaXNA;
using UltimaXNA.Client;
using UltimaXNA.Diagnostics;
using UltimaXNA.Input;
using UltimaXNA.GUI;
using Microsoft.Xna.Framework.Content;
#endregion


namespace UltimaXNA.SceneManagement
{
    public abstract class BaseScene : IScene, IDisposable
    {
        public virtual TimeSpan TransitionOnLength { get { return TimeSpan.FromSeconds(0.5); } }
        public virtual TimeSpan TransitionOffLength { get { return TimeSpan.FromSeconds(0.5); } }

        protected SpriteBatch SpriteBatch;

        Game _game;

        public bool IsInitialized { get; set; }
        ISceneService _sceneService;
        IUltimaClient _networkService;
        ILoggingService _loggingService;
        IInputService _inputService;
        IGUI _guiService;

        Texture2D _transitionTexture;
        ContentManager _content;
        SceneState _sceneState;
        Color _clearColor;
        float _transitionAlpha;
        float _elapsed;

        public Game Game
        {
            get { return _game; }
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return _game.GraphicsDevice; }
        }

        public Color ClearColor
        {
            get { return _clearColor; }
            set { _clearColor = value; }
        }

        public ContentManager Content
        {
            get { return _content; }
            set { _content = value; }
        }
        
        public SceneState SceneState
        {
            get { return _sceneState; }
            set
            {
                _sceneState = value;
                _elapsed = 0;
            }
        }

        public ISceneService SceneManager
        {
            get { return _sceneService; }
        }

        public IUltimaClient Network
        {
            get { return _networkService; }
        }

        public ILoggingService Log
        {
            get { return _loggingService; }
            set { _loggingService = value; }
        }

        public IInputService Input
        {
            get { return _inputService; }
            set { _inputService = value; }
        }

        public IGUI GUI
        {
            get { return _guiService; }
        }

        public event TransitionCompleteHandler TransitionComplete;

        public BaseScene(Game game)
        {
            _game = game;
            _clearColor = Color.Black;
            _content = new ContentManager(_game.Services);
            _content.RootDirectory = "Content";
            _sceneState = SceneState.TransitioningOn;

            _sceneService = game.Services.GetService<ISceneService>(true);
            _networkService = game.Services.GetService<IUltimaClient>(true);
            _loggingService = game.Services.GetService<ILoggingService>(true);
            _inputService = game.Services.GetService<IInputService>(true);
            _guiService = game.Services.GetService<IGUI>(true);
        }

        public virtual void Intitialize()
        {
            SpriteBatch = new SpriteBatch(Game.GraphicsDevice);

            Color[] data = new Color[] { Color.Black };
            _transitionTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            _transitionTexture.SetData<Color>(data);
        }

        public virtual void Update(GameTime gameTime)
        {
            _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            switch (_sceneState)
            {
                case SceneState.TransitioningOn:
                    {
                        _transitionAlpha = 1 - (_elapsed / (float)TransitionOffLength.TotalSeconds);

                        if (_elapsed >= (float)TransitionOnLength.TotalSeconds)
                        {
                            _elapsed = 0;
                            _sceneState = SceneState.Active;
                        }

                        break;
                    }
                case SceneState.TransitioningOff:
                    {
                        _transitionAlpha = _elapsed / (float)TransitionOnLength.TotalSeconds;

                        if (_elapsed >= (float)TransitionOffLength.TotalSeconds)
                        {
                            _elapsed = 0;
                            _sceneState = SceneState.None;

                            if (TransitionComplete != null)
                                TransitionComplete();
                        }

                        break;
                    }
            }
        }

        public virtual void OnBeforeDraw(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_clearColor);
        }

        public virtual void OnAfterDraw(GameTime gameTime)
        {
            PresentationParameters pp = Game.GraphicsDevice.PresentationParameters;
            Color color = new Color(new Vector4(1, 1, 1, _transitionAlpha));

            SpriteBatch.Begin();
            SpriteBatch.Draw(_transitionTexture,
                new Rectangle(0,0,pp.BackBufferWidth, pp.BackBufferHeight), color);
            SpriteBatch.End();
        }

        public virtual void Dispose()
        {
            _content.Dispose();
        }
    }
}
