using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Afterglow.Log
{
    internal class Log4NetProxy : Afterglow.Core.Log.ILogger
    {
        public Log4NetProxy(log4net.ILog logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            _logger = logger;
        }

        private readonly log4net.ILog _logger;

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Debug(Exception ex, string message)
        {
            _logger.Debug(message, ex);
        }

        public void Info(Exception ex, string message)
        {
            _logger.Info(message, ex);
        }

        public void Warn(Exception ex, string message)
        {
            _logger.Warn(message, ex);
        }

        public void Error(Exception ex, string message)
        {
            _logger.Error(message, ex);
        }

        public void Fatal(Exception ex, string message)
        {
            _logger.Fatal(message, ex);
        }

        public void Debug(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);
        }

        public void Info(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void Warn(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public void Error(string format, params object[] args)
        {
            _logger.ErrorFormat(format, args);
        }

        public void Fatal(string format, params object[] args)
        {
            _logger.FatalFormat(format, args);
        }
    }
}
