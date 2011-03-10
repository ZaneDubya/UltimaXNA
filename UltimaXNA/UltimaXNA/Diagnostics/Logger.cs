/***************************************************************************
 *   Logger.cs
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
// using log4net;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Configuration;
#endregion

namespace UltimaXNA.Diagnostics
{
    /// <summary>
    /// The Logger class is a simple wrapper for log4net.
    /// </summary>
    public class Logger : ILoggingService
    {
        static bool Initialized = false;

        // log4net.ILog _log;

        protected string LoggerType;

        public event EventHandler<StatusUpdateEventArgs> StatusUpdate;

        public Logger(Type type)
            : this(type.ToString())
        {
        }

        public Logger(string loggerType)
        {
            LoggerType = loggerType;

            if (!Initialized) 
            {
                // string path = "log4net.config";

                // log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(path));
                Initialized = true;
            }

            // _log = log4net.LogManager.GetLogger(LoggerType);
        }

        /// <summary>
        /// Logs debug information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public virtual void Debug(object obj)
        {
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Debug, obj.ToString()));
            Console.WriteLine(obj.ToString()); // _log.Debug(obj.ToString());
        }

        /// <summary>
        /// Logs debug information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public virtual void Debug(string message)
        {
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Debug, message));
            Console.WriteLine(message); // _log.Debug(message);
        }

        /// <summary>
        /// Logs debug information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public virtual void Debug(string message, params object[] objects)
        {
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Debug, message, objects));
            Console.WriteLine(string.Format(message, objects)); // _log.DebugFormat(message, objects);
        }

        /// <summary>
        /// Logs information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public virtual void Info(object obj)
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
        public virtual void Info(string message)
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
        public virtual void Info(string message, params object[] objects)
        {
            // Utility.PushColor(ConsoleColor.White);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Info, message, objects));
            Console.WriteLine(string.Format(message, objects)); // _log.InfoFormat(message, objects);
            // Utility.PopColor();
        }

        /// <summary>
        /// Logs warning information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public virtual void Warn(object obj)
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
        public virtual void Warn(string message)
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
        public virtual void Warn(string message, params object[] objects)
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
        public virtual void Error(object obj)
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
        public virtual void Error(string message)
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
        public virtual void Error(string message, params object[] objects)
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
        public virtual void Fatal(object obj)
        {
            // Utility.PushColor(ConsoleColor.Red);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, obj.ToString()));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(obj.ToString()); // _log.Fatal(message);
            Console.ResetColor();
            // Utility.PopColor();
        }

        /// <summary>
        /// Logs fatal error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public virtual void Fatal(string message)
        {
            // Utility.PushColor(ConsoleColor.Red);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, message));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message); // _log.Fatal(message);
            Console.ResetColor();
            // Utility.PopColor();
        }

        /// <summary>
        /// Logs fatal error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public virtual void Fatal(string message, params object[] objects)
        {
            // Utility.PushColor(ConsoleColor.Red);
            // OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, message, objects));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(string.Format(message, objects)); // _log.FatalFormat(message, objects);
            Console.ResetColor();
            // Utility.PopColor();
        }

        /// <summary>
        /// Raises the StatusUpdate event.
        /// </summary>
        /// <param name="sender">The object making the call</param>
        /// <param name="e">A StatusUpdateEventArgs that contains the event data.</param>
        protected virtual void OnStatusUpdate(object sender, StatusUpdateEventArgs e)
        {
            if (StatusUpdate != null)
                StatusUpdate(sender, e);
        }
    }
}
