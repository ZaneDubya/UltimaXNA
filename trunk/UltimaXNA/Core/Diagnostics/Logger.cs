/***************************************************************************
 *   Logger.cs
 *   Part of UltimaXNA: http://code.google.com/p/ultimaxna
 *   
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/
#region usings
using System;
#endregion

namespace UltimaXNA.Diagnostics
{
    /// <summary>
    /// The Logger class is a simple wrapper for log4net.
    /// </summary>
    public class Logger
    {
        public static event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        /// <summary>
        /// Logs debug information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public static void Debug(object obj)
        {
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Debug, obj.ToString()));
            Console.WriteLine(obj.ToString()); // _log.Debug(obj.ToString());
        }

        /// <summary>
        /// Logs debug information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public static void Debug(string message)
        {
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Debug, message));
            Console.WriteLine(message); // _log.Debug(message);
        }

        /// <summary>
        /// Logs debug information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public static void Debug(string message, params object[] objects)
        {
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Debug, message, objects));
            Console.WriteLine(string.Format(message, objects)); // _log.DebugFormat(message, objects);
        }

        /// <summary>
        /// Logs information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public static void Info(object obj)
        {
            // Utility.PushColor(ConsoleColor.White);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Info, obj.ToString()));
            Console.WriteLine(obj.ToString()); // _log.Info(obj.ToString());
            // Utility.PopColor();
        }

        /// <summary>
        /// Logs information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public static void Info(string message)
        {
            // Utility.PushColor(ConsoleColor.White);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Info, message));
            Console.WriteLine(message); // _log.Info(message);
            // Utility.PopColor();
        }

        /// <summary>
        /// Logs information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public static void Info(string message, params object[] objects)
        {
            // Utility.PushColor(ConsoleColor.White);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Info, message, objects));
            Console.WriteLine(string.Format(message, objects)); // _log.InfoFormat(message, objects);
            // Utility.PopColor();
        }

        /// <summary>
        /// Logs warning information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public static void Warn(object obj)
        {
            // Utility.PushColor(ConsoleColor.Yellow);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Warn, obj.ToString()));
            // Console.WriteLine(obj.ToString()); // _log.Warn(obj.ToString());
            // Utility.PopColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(obj.ToString());
            Console.ResetColor();
        }

        /// <summary>
        /// Logs warning information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public static void Warn(string message)
        {
            // Utility.PushColor(ConsoleColor.Yellow);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Warn, message));
            // _log.Warn(message);
            // Utility.PopColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Logs warning information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public static void Warn(string message, params object[] objects)
        {
            // Utility.PushColor(ConsoleColor.Yellow);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Warn, message, objects));
            // _log.WarnFormat(message, objects);
            // Utility.PopColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(string.Format(message, objects));
            Console.ResetColor();
        }

        /// <summary>
        /// Logs error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public static void Error(object obj)
        {
            // Utility.PushColor(ConsoleColor.Red);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, obj.ToString()));
            // _log.Error(obj.ToString());
            // Utility.PopColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(obj.ToString());
            Console.ResetColor();
        }

        /// <summary>
        /// Logs error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public static void Error(string message)
        {
            // Utility.PushColor(ConsoleColor.Red);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, message));
            // _log.Error(message);
            // Utility.PopColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message); // _log.Fatal(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Logs error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public static void Error(string message, params object[] objects)
        {
            // Utility.PushColor(ConsoleColor.Red);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, message, objects));
            // _log.ErrorFormat(message, objects);
            // Utility.PopColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format(message, objects)); // _log.Fatal(message);
            Console.ResetColor();
        }

        /// <summary>
        /// Logs fatal error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public static void Fatal(object obj)
        {
            Fatal(obj.ToString());
        }

        /// <summary>
        /// Logs fatal error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public static void Fatal(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            /*Console.WriteLine("Press any key to quit.");
            Console.ReadKey(true);
            UltimaVars.EngineRunning = false;*/
            Console.ResetColor();
        }

        /// <summary>
        /// Logs fatal error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public static void Fatal(string message, params object[] objects)
        {
            Fatal(string.Format(message, objects));
        }

        /// <summary>
        /// Raises the StatusUpdate event.
        /// </summary>
        /// <param name="sender">The object making the call</param>
        /// <param name="e">A StatusUpdateEventArgs that contains the event UltimaData.</param>
        protected static void OnStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(sender, e);
        }
    }
}
