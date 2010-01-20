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
        Diagnostics.Logger _logService;
        Input.InputState _inputState;
        SceneManagement.SceneManager _sceneService;
        TileEngine.World _worldService;
        UILegacy.UIManager _LegacyUIService;

        public Engine()
        {
            setupGraphicsDeviceManager();
        }

        protected override void Initialize()
        {
            this.Content.RootDirectory = "Content";
            
            // Load all the services we need.
            _logService = new Diagnostics.Logger("UXNA");
            Services.AddService<Diagnostics.ILoggingService>(_logService);

            _inputState = new Input.InputState(this);
            Services.AddService<Input.IInputService>(_inputState);

            _worldService = new TileEngine.World(this);
            Services.AddService<TileEngine.IWorld>(_worldService);

            _LegacyUIService = new UltimaXNA.UILegacy.UIManager(this);
            Services.AddService<UILegacy.IUIManager>(_LegacyUIService);

            _sceneService = new SceneManagement.SceneManager(this);
            Services.AddService<SceneManagement.ISceneService>(_sceneService);

            InvokeInitializers();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _sceneService.CurrentScene = new SceneManagement.LoginScene(this);
            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            // WavePlayer opens a thread for each sound.
            WavePlayer.Player.EndEverything();
            base.OnExiting(sender, args);
        }

        protected override void Update(GameTime gameTime)
        {
            SpriteBatch3D.Z = 0;
            if (!ClientVars.EngineRunning)
                Exit();
            ClientVars.IsMinimized = isMinimized();

            base.Update(gameTime);

            if (ClientVars.EngineRunning)
            {
                _inputState.Update(gameTime);

                Client.UltimaClient.Update(gameTime);
                GameState.Update(gameTime);
                Entities.EntitiesCollection.Update(gameTime);
                _sceneService.Update(gameTime);
                GameState.UpdateAfter(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            _sceneService.Draw(gameTime);
            base.Draw(gameTime);
        }

        bool isMinimized()
        {
            //Get out top level form via the handle.
            System.Windows.Forms.Control MainForm = System.Windows.Forms.Form.FromHandle(Window.Handle);
            //If we are minimized don't waste time trying to draw, and avoid crash on resume.
            if (((System.Windows.Forms.Form)MainForm).WindowState == System.Windows.Forms.FormWindowState.Minimized)
                return true;
            return false;
        }

        // Some settings to designate a screen size and fps limit.
        void setupGraphicsDeviceManager()
        {
            GraphicsDeviceManager graphicsDeviceManager = new GraphicsDeviceManager(this);
            graphicsDeviceManager.PreferredBackBufferWidth = 800;
            graphicsDeviceManager.PreferredBackBufferHeight = 600;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;
            graphicsDeviceManager.PreparingDeviceSettings += OnPreparingDeviceSettings;
            this.IsFixedTimeStep = false;
            graphicsDeviceManager.ApplyChanges();
        }
        static void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
            if (_passedPtr != -1)
                e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = new IntPtr(_passedPtr);
        }

        /// <summary>
        /// Invokes "public static void Initialize(Game game)" for each type that contains this function
        /// </summary>
        void InvokeInitializers()
        {
            // First initialize some of the local data classes.
            Data.AnimationsXNA.Initialize(GraphicsDevice);
            Data.Art.Initialize(GraphicsDevice);
            Data.ASCIIText.Initialize(GraphicsDevice);
            Data.UniText.Initialize(GraphicsDevice);
            Data.Gumps.Initialize(GraphicsDevice);
            Data.HuesXNA.Initialize(GraphicsDevice);
            Data.Texmaps.Initialize(GraphicsDevice);
            Data.StringList.LoadStringList("enu");
            Data.Skills.Initialize();

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

        static int _passedPtr = -1;

        #region EntryPoint
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0)
               _passedPtr = Int32.Parse(args[0]);

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