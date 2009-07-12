using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UltimaXNA.Diagnostics
{
    /// <summary>
    /// Defines functions to use for logging
    /// </summary>
    public interface ILoggingService : IStatusNotifier
    {
        /// <summary>
        /// Logs debug information 
        /// </summary>
        /// <param name="obj">The object to be logged</param>
        void Debug(object obj);
        /// <summary>
        /// Log general information 
        /// </summary>
        /// <param name="obj">The object to be logged</param>
        void Info(object obj);
        /// <summary>
        /// Log warning information 
        /// </summary>
        /// <param name="obj">The object to be logged</param>
        void Warn(object obj);
        /// <summary>
        /// Log error information 
        /// </summary>
        /// <param name="obj">The object to be logged</param>
        void Error(object obj);
        /// <summary>
        /// Log fatal information 
        /// </summary>
        /// <param name="obj">The object to be logged</param>
        void Fatal(object obj);

        /// <summary>
        /// Log debug information 
        /// </summary>
        /// <param name="message">The message to be logged</param>
        void Debug(string message);

        /// <summary>
        /// Log general information 
        /// </summary>
        /// <param name="message">The message to be logged</param>
        void Info(string message);

        /// <summary>
        /// Log warning information 
        /// </summary>
        /// <param name="message">The message to be logged</param>
        void Warn(string message);

        /// <summary>
        /// Log error information 
        /// </summary>
        /// <param name="message">The message to be logged</param>
        void Error(string message);

        /// <summary>
        /// Log fatal information 
        /// </summary>
        /// <param name="message">The message to be logged</param>
        void Fatal(string message);
        
        /// <summary>
        /// Log debug information 
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An object array containing zero or more objects to format</param>
        void Debug(string message, params object[] objects);

        /// <summary>
        /// Log general information 
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An object array containing zero or more objects to format</param>
        void Info(string message, params object[] objects);

        /// <summary>
        /// Log warning information 
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An object array containing zero or more objects to format</param>
        void Warn(string message, params object[] objects);

        /// <summary>
        /// Log error information 
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An object array containing zero or more objects to format</param>
        void Error(string message, params object[] objects);

        /// <summary>
        /// Log fatal information 
        /// </summary>
        /// <param name="message">A composite format string</param>
        /// <param name="objects">An object array containing zero or more objects to format</param>
        void Fatal(string message, params object[] objects);
    }
}
