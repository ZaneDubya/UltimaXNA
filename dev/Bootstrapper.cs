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
    sealed class Bootstrapper
    {
        // === Main ===================================================================================================
        [STAThread]
        static void Main(string[] args)
        {
            new Bootstrapper(args).Initialize();
        }

        readonly

                // === Instance ===============================================================================================
                GeneralExceptionHandler m_ExecptionHandler;

        Bootstrapper(string[] args)
        {
            m_ExecptionHandler = new GeneralExceptionHandler();
        }

        void Initialize()
        {
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

        void StartEngine()
        {
            SetExceptionHandlers();
            using (UltimaGame engine = new UltimaGame())
            {
                Resolutions.SetWindowSize(engine.Window);
                engine.Run();
            }
            ClearExceptionHandlers();
        }

        void ConfigureTraceListeners()
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

        void SetExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        void ClearExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        }

        void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            m_ExecptionHandler.OnError((Exception) e.ExceptionObject);
        }

        void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            m_ExecptionHandler.OnError(e.Exception);
        }
    }
}