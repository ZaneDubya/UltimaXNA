/***************************************************************************
 *   Engine.cs
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
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UltimaXNA.Extensions;
#endregion

namespace UltimaXNA
{
    public class Engine : Game
    {
        private Diagnostics.Logger _logService;
        private Input.InputState _inputState;
        private Graphics.UI.UIManager _uiService;
        private EventSystem.EventEngine _eventService;
        private SceneManagement.SceneManager _sceneService;
        private TileEngine.World _worldService;

        public Engine()
        {
            m_SetupGraphicsDeviceManager();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            // WavePlayer opens a thread for each sound.
            WavePlayer.Player.EndEverything();
            base.OnExiting(sender, args);
        }

        protected override void Initialize()
        {
            this.Content.RootDirectory = "Content";
            InvokeInitializers();
            
            // Load all the services we need.
            _logService = new Diagnostics.Logger("UXNA");
            Services.AddService<Diagnostics.ILoggingService>(_logService);

            _inputState = new Input.InputState(this);
            Services.AddService<Input.IInputService>(_inputState);

            _worldService = new TileEngine.World(this);
            Services.AddService<TileEngine.IWorld>(_worldService);

            _eventService = new EventSystem.EventEngine(this);
            Services.AddService<EventSystem.IEventService>(_eventService);

            _uiService = new Graphics.UI.UIManager(this);
            Services.AddService<Graphics.UI.IUIService>(_uiService);

			ParticleEngine.ParticleEngine.Initialize(this, System.IO.Path.Combine(this.Content.RootDirectory, "pfx"));

            _sceneService = new SceneManagement.SceneManager(this);
            Services.AddService<SceneManagement.ISceneService>(_sceneService);

            _sceneService.CurrentScene = new SceneManagement.LoginScene(this);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _inputState.Update(gameTime);

            Client.UltimaClient.Update(gameTime);
            GameState.Update(gameTime);
            Entities.EntitiesCollection.Update(gameTime);
            
            UI.UserInterface.Update(gameTime);
            _sceneService.Update(gameTime);
			ParticleEngine.ParticleEngine.Update(gameTime);

            GameState.UpdateAfter(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _sceneService.Draw(gameTime);
            // ParticleEngine.ParticleEngine.Draw(gameTime);
			UI.UserInterface.Draw(gameTime);
			
            base.Draw(gameTime);
        }

        // Some settings to designate a screen size and fps limit.
        private void m_SetupGraphicsDeviceManager()
        {
            GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.PreferredBackBufferWidth = 800;
            graphicsDeviceManager.PreferredBackBufferHeight = 600;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            graphicsDeviceManager.PreparingDeviceSettings += OnPreparingDeviceSettings;
            this.IsFixedTimeStep = false;
            graphicsDeviceManager.ApplyChanges();
        }
        private static void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        /// <summary>
        /// Invokes "public static void Initialize(Game game)" for each type that contains this function
        /// </summary>
        private void InvokeInitializers()
        {
            // First initialize some of the local data classes.
            Data.AnimationsXNA.Initialize(GraphicsDevice);
            Data.Art.Initialize(GraphicsDevice);
            Data.ASCIIText.Initialize(GraphicsDevice);
            Data.Gumps.Initialize(GraphicsDevice);
            Data.HuesXNA.Initialize(GraphicsDevice);
            Data.Texmaps.Initialize(GraphicsDevice);
            Data.StringList.LoadStringList("enu");

            //Get all loaded assemblies
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<MethodInfo> invoke = new List<MethodInfo>();

            for (int a = 0; a < assemblies.Length; ++a)
            {
                //Get all types within that assembly
                Type[] types = assemblies[a].GetTypes();

                for (int i = 0; i < types.Length; ++i)
                {
                    //Find the method "public static void Intialize"
                    MethodInfo m = types[i].GetMethod("Initialize", BindingFlags.Static | BindingFlags.Public);

                    if (m != null)
                    {
                        //A valid methodinfo will contain 1 paramager of type Game.
                        bool valid = ((from arg in m.GetParameters()
                                       where arg.ParameterType == typeof(Game)
                                       && m.GetParameters().Length == 1
                                       select arg).Count() > 0);

                        //If its valid, add it to the invokation list.
                        if (valid)
                        {
                            invoke.Add(m);
                        }
                    }
                }
            }

            //Sort the invocation list by call order
            invoke.Sort(new CallPriorityComparer());

            //Invoke each function
            for (int i = 0; i < invoke.Count; i++)
            {
                invoke[i].Invoke(null, new object[] { this });
            }
        }

        #region EntryPoint
        [STAThread]
        private static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            using (Engine engine = new Engine())
            {
                engine.Run();
            }
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Diagnostics.Logger log = new Diagnostics.Logger(typeof(Engine));
            log.Fatal(e.ExceptionObject);
        }

        #endregion
    }
}