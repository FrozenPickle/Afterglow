using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;
using log4net.Layout;
using System.IO;

namespace Afterglow.Core.Log
{
    public class AfterglowLogger : Afterglow.Core.Log.ILogger
    {
        private readonly log4net.ILog _logger;
        private PatternLayout _layout = new PatternLayout();
        private const string LOG_PATTERN = "%d [%t] %-5p %m%n";
        
        public string DefaultPattern
        {
            get { return LOG_PATTERN; }
        }

        public AfterglowLogger(string applicationDataFolder, string loggingFile, int loggingLevel = LoggingLevels.LOG_LEVEL_ERROR)
        {
            string loggingPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), applicationDataFolder);
            if (!System.IO.Directory.Exists(loggingPath))
            {
                System.IO.Directory.CreateDirectory(loggingPath);
            }

            loggingPath = System.IO.Path.Combine(loggingPath, Path.GetDirectoryName(loggingFile));
            if (!System.IO.Directory.Exists(loggingPath))
            {
                System.IO.Directory.CreateDirectory(loggingPath);
            }
            
            string logFilePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), applicationDataFolder, loggingFile);

            _layout.ConversionPattern = DefaultPattern;
            _layout.ActivateOptions();
            
            _logger = LogManager.GetLogger(typeof(AfterglowLogger));

            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            TraceAppender tracer = new TraceAppender();
            PatternLayout patternLayout = new PatternLayout();

            patternLayout.ConversionPattern = LOG_PATTERN;
            patternLayout.ActivateOptions();

            tracer.Layout = patternLayout;
            tracer.ActivateOptions();
            hierarchy.Root.AddAppender(tracer);

            RollingFileAppender roller = new RollingFileAppender();
            roller.Layout = patternLayout;
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.LockingModel = new log4net.Appender.FileAppender.MinimalLock();
            roller.MaxSizeRollBackups = 4;
            roller.MaximumFileSize = "100KB";
            roller.StaticLogFileName = true;
            roller.File = logFilePath;
            roller.Name = "RollingFileAppender";
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            LoggingLevel = loggingLevel;

            hierarchy.Configured = true;
        }

        public int LoggingLevel
        {
            set
            {
                Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

                switch (value)
                {
                    case LoggingLevels.LOG_LEVEL_DEBUG:
                        hierarchy.Root.Level = Level.Debug;
                        break;
                    case LoggingLevels.LOG_LEVEL_INFORMATION:
                        hierarchy.Root.Level = Level.Info;
                        break;
                    case LoggingLevels.LOG_LEVEL_WARNING:
                        hierarchy.Root.Level = Level.Warn;
                        break;
                    case LoggingLevels.LOG_LEVEL_ERROR:
                        hierarchy.Root.Level = Level.Error;
                        break;
                    case LoggingLevels.LOG_LEVEL_FATAL:
                        hierarchy.Root.Level = Level.Fatal;
                        break;
                    default:
                        hierarchy.Root.Level = Level.Error;
                        break;
                }
            }
        }

        public PatternLayout DefaultLayout
        {
            get { return _layout; }
        }

        public void AddAppender(IAppender appender)
        {
            Hierarchy hierarchy =
                (Hierarchy)LogManager.GetRepository();

            hierarchy.Root.AddAppender(appender);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(Exception ex, string message)
        {
            _logger.Debug(message, ex);
        }

        public void Debug(string format, params object[] args)
        {
            _logger.DebugFormat(format, args);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(Exception ex, string message)
        {
            _logger.Info(message, ex);
        }

        public void Info(string format, params object[] args)
        {
            _logger.InfoFormat(format, args);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Warn(Exception ex, string message)
        {
            _logger.Warn(message, ex);
        }

        public void Warn(string format, params object[] args)
        {
            _logger.WarnFormat(format, args);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(Exception ex, string message)
        {
            _logger.Error(message, ex);
        }

        public void Error(string format, params object[] args)
        {
            _logger.ErrorFormat(format, args);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(Exception ex, string message)
        {
            _logger.Fatal(message, ex);
        }

        public void Fatal(string format, params object[] args)
        {
            _logger.FatalFormat(format, args);
        }
    }
}
