#region Usings
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UltimaXNA.Configuration;
using UltimaXNA.Core;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Tracing;
using UltimaXNA.Core.Diagnostics.Tracing.Listeners;
using UltimaXNA.Core.Input.Windows;
using UltimaXNA.Core.Patterns.IoC;
using UltimaXNA.Ultima.IO;
using UltimaXNA.Windows.Diagnostics.Tracing.Listeners;
#endregion

namespace UltimaXNA
{
    internal sealed class Bootstrapper
    {
        [STAThread]
        private static void Main(string[] args)
        {
            new Bootstrapper(args).Initialize();
        }

        private bool m_isInitialized;

        public Bootstrapper(string[] args)
        {
            GeneralExceptionHandler.Instance = new GeneralExceptionHandler();
        }
        
        public string BaseApplicationPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        private void ConfigureContainer(IContainer container)
        {
            container.RegisterModule<CoreModule>();
        }

        private void ConfigureTraceListeners()
        {
            if (Settings.Debug.IsConsoleEnabled)
            {
                Tracer.RegisterListener(new ConsoleOutputEventListener());
            }

            if (Debugger.IsAttached)
            {
                Tracer.RegisterListener(new DebugOutputEventListener());
            }

            Tracer.RegisterListener(new FileLogEventListener("debug.log"));
        }

        private IContainer m_container;

        private void Configure()
        {
            Container rootContainer = new Container();

            m_container = rootContainer.CreateChildContainer();

            ConfigureContainer(rootContainer);
            ConfigurePlugins(m_container);
        }

        private void ConfigurePlugins(IContainer container)
        {
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(BaseApplicationPath, "plugins"));

            if (!directory.Exists)
            {
                return;
            }

            FileInfo[] assemblies = directory.GetFiles("*.dll");

            foreach (FileInfo file in assemblies)
            {
                try
                {
                    Tracer.Info("Loading plugin {0}.", file.Name);

                    Assembly assembly = Assembly.LoadFile(file.FullName);
                    IEnumerable<Type> modules = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof (IModule)));

                    foreach (Type module in modules)
                    {
                        Tracer.Info("Activating module {0}.", module.FullName);

                        IModule instance = (IModule)Activator.CreateInstance(module);

                        instance.Load(container);
                    }
                }
                catch (Exception e)
                {
                    Tracer.Warn(e, "An error occurred while trying to load plugin. [{0}]", file.FullName);
                }
            }
        }

        public void Initialize()
        {
            if (m_isInitialized)
            {
                return;
            }

            m_isInitialized = true;

            if (!Settings.IsSettingsFileCreated)
            {
                Settings.Debug.IsConsoleEnabled = false;
                Settings.Debug.ShowDataRead = false;
                Settings.Debug.ShowDataReadBreakdown = false;
                Settings.Debug.ShowFps = false;
                Settings.Debug.ShowUIOutlines = false;
                Settings.Game.AlwaysRun = false;
                Settings.Game.AutoSelectLastCharacter = false;
                Settings.Game.IsFixedTimeStep = false;
                Settings.Game.IsVSyncEnabled = false;
                Settings.Game.LastCharacterName = "Jeff";
                Settings.Game.Mouse.InteractionButton = MouseButton.Left;
                Settings.Game.Mouse.MovementButton = MouseButton.Right;
                Settings.Game.Mouse.IsEnabled = true;
                Settings.Game.Resolution = new Resolution(800, 600);
                Settings.Server.ServerAddress = Settings.Server.ServerAddress;
                Settings.Server.ServerPort = Settings.Server.ServerPort;
                Settings.Server.UserName = "";
                Settings.UltimaOnline.DataDirectory = FileManager.DataPath;
            }

            if (Settings.Debug.IsConsoleEnabled && !ConsoleManager.HasConsole)
            {
                ConsoleManager.Show();
            }

            ConfigureTraceListeners();

            try
            {
                StartEngine();
            }
            finally
            {
                if (ConsoleManager.HasConsole)
                {
                    ConsoleManager.Hide();
                }
            }
        }

        private void Prepare()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private void StartEngine()
        {
            Prepare();
            Configure();

            using (UltimaEngine engine = new UltimaEngine(m_container))
            {
                engine.Run();
            }
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GeneralExceptionHandler.Instance.OnError((Exception) e.ExceptionObject);
        }

        private void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            GeneralExceptionHandler.Instance.OnError(e.Exception);
        }
    }
}