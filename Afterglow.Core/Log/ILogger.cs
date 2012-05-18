using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Core.Log
{
    public enum Level
    {
        All,
        Debug,
        Information,
        Warning,
        Error,
        Fatal,
        Off
    }

    /// <summary>
    /// Logging interface
    /// </summary>
    public interface ILogger
    {
        /* Log a message object */
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);

        /* Log a message object and exception */
        void Debug(Exception ex, string message);
        void Info(Exception ex, string message);
        void Warn(Exception ex, string message);
        void Error(Exception ex, string message);
        void Fatal(Exception ex, string message);

        /* Log a message using a format string */
        void Debug(string format, params object[] args);
        void Info(string format, params object[] args);
        void Warn(string format, params object[] args);
        void Error(string format, params object[] args);
        void Fatal(string format, params object[] args);
    }
}
