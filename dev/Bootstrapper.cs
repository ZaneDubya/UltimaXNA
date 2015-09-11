/***************************************************************************
 *   Bootstrapper.cs
 *   Copyright (c) 2015 UltimaXNA Development Team
 * 
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region Usings
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UltimaXNA.Core;
using UltimaXNA.Core.Diagnostics;
using UltimaXNA.Core.Diagnostics.Listeners;
using UltimaXNA.Core.Diagnostics.Tracing;
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

        private bool m_IsInitialized;

        public string BaseApplicationPath
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
        }

        public Bootstrapper(string[] args)
        {
            GeneralExceptionHandler.Instance = new GeneralExceptionHandler();
        }

        public void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }

            m_IsInitialized = true;

            ConfigureTraceListeners();

            if (Settings.Debug.IsConsoleEnabled && !ConsoleManager.HasConsole)
            {
                ConsoleManager.Show();
            }

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

        private void StartEngine()
        {
            SetExceptionHandlers();

            using (UltimaGame engine = new UltimaGame())
            {
                Resolutions.SetScreenSize(engine.Window);
                engine.Run();
            }

            ClearExceptionHandlers();
        }

        private void ConfigureTraceListeners()
        {
            if (Debugger.IsAttached)
            {
                Tracer.RegisterListener(new DebugOutputEventListener());
            }

            Tracer.RegisterListener(new FileLogEventListener("debug.log"));

            if (Settings.Debug.IsConsoleEnabled)
            {
                Tracer.RegisterListener(new ConsoleOutputEventListener());
            }

            Tracer.RegisterListener(new MsgBoxOnCriticalListener());
        }

        private void SetExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private void ClearExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
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