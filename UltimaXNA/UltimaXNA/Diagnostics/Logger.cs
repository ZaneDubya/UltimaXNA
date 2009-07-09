using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using l4 = log4net;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Configuration;

namespace UltimaXNA.Diagnostics
{
    /// <summary>
    /// The Logger class is a simple wrapper for log4net.
    /// </summary>
    public class Logger : ILoggable
    {
        static bool Initialized = false;

        log4net.ILog _log;

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
                string path = "log4net.config";

                log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(path));
                Initialized = true;
            }

            _log = log4net.LogManager.GetLogger(LoggerType);
        }

        /// <summary>
        /// Logs debug information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public virtual void Debug(object obj)
        {
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Debug, obj.ToString()));
            _log.Debug(obj.ToString());
        }

        /// <summary>
        /// Logs debug information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public virtual void Debug(string message)
        {
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Debug, message));
            _log.Debug(message);
        }

        /// <summary>
        /// Logs debug information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public virtual void Debug(string message, params object[] objects)
        {
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Debug, message, objects));
            _log.DebugFormat(message, objects);
        }

        /// <summary>
        /// Logs information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public virtual void Info(object obj)
        {
            Utility.PushColor(ConsoleColor.White);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Info, obj.ToString()));
            _log.Info(obj.ToString());
            Utility.PopColor();
        }

        /// <summary>
        /// Logs information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public virtual void Info(string message)
        {
            Utility.PushColor(ConsoleColor.White);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Info, message));
            _log.Info(message);
            Utility.PopColor();
        }

        /// <summary>
        /// Logs information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public virtual void Info(string message, params object[] objects)
        {
            Utility.PushColor(ConsoleColor.White);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Info, message, objects));
            _log.InfoFormat(message, objects);
            Utility.PopColor();
        }

        /// <summary>
        /// Logs warning information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public virtual void Warn(object obj)
        {
            Utility.PushColor(ConsoleColor.Yellow);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Warn, obj.ToString()));
            _log.Warn(obj.ToString());
            Utility.PopColor();
        }

        /// <summary>
        /// Logs warning information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public virtual void Warn(string message)
        {
            Utility.PushColor(ConsoleColor.Yellow);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Warn, message));
            _log.Warn(message);
            Utility.PopColor();
        }

        /// <summary>
        /// Logs warning information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public virtual void Warn(string message, params object[] objects)
        {
            Utility.PushColor(ConsoleColor.Yellow);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Warn, message, objects));
            _log.WarnFormat(message, objects);
            Utility.PopColor();
        }

        /// <summary>
        /// Logs error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public virtual void Error(object obj)
        {
            Utility.PushColor(ConsoleColor.Red);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, obj.ToString()));
            _log.Error(obj.ToString());
            Utility.PopColor();
        }

        /// <summary>
        /// Logs error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public virtual void Error(string message)
        {
            Utility.PushColor(ConsoleColor.Red);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, message));
            _log.Error(message);
            Utility.PopColor();
        }

        /// <summary>
        /// Logs error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public virtual void Error(string message, params object[] objects)
        {
            Utility.PushColor(ConsoleColor.Red);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, message, objects));
            _log.ErrorFormat(message, objects);
            Utility.PopColor();
        }

        /// <summary>
        /// Logs fatal error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        public virtual void Fatal(object obj)
        {
            Utility.PushColor(ConsoleColor.Red);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, obj.ToString()));
            _log.Fatal(obj.ToString());
            Utility.PopColor();
        }

        /// <summary>
        /// Logs fatal error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">message string</param>
        public virtual void Fatal(string message)
        {
            Utility.PushColor(ConsoleColor.Red);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, message));
            _log.Fatal(message);
            Utility.PopColor();
        }

        /// <summary>
        /// Logs fatal error information to the objects Logger.  This will also raise the StatusUpdate event
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An System.Object array containing zero or more objects to format</param>
        public virtual void Fatal(string message, params object[] objects)
        {
            Utility.PushColor(ConsoleColor.Red);
            OnStatusUpdate(this, new StatusUpdateEventArgs(StatusLevel.Error, message, objects));
            _log.FatalFormat(message, objects);
            Utility.PopColor();
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
