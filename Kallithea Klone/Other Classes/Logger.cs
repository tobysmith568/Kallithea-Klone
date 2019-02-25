using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using System;
using System.IO;

namespace Kallithea_Klone.Other_Classes
{
    public class Logger
    {
        //  Variables
        //  =========

        private readonly ILog log;

        private const string DEBUG_FORMAT = "[{0}] [DEBUG] {1}";
        private const string INFO_FORMAT = "[{0}] [INFO] {1}";
        private const string REPO_FORMAT = "[{0}] [REPO] [{1}] [{2}] [{3}]";
        private const string WARN_FORMAT = "[{0}] [WARN] {1}";
        private const string ERROR_FORMAT = "[{0}] [ERROR] {1}";
        private const string FATAL_FORMAT = "[{0}] [FATAL] {1}";
        private const string DATETIME_FORMAT = "dd/MM/yyyy HH:mm:ss";

        //  Constructors
        //  ============

        static Logger()
        {
            log4net.Util.LogLog.InternalDebugging = true;
            PatternLayout p = new PatternLayout()
            {
                ConversionPattern = "%message%newline"
            };
            p.ActivateOptions();
            RollingFileAppender a = new RollingFileAppender()
            {
                Layout = p,
                File = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Kallithea Klone", "Output.log"),
                RollingStyle = RollingFileAppender.RollingMode.Date,
                MaxSizeRollBackups = 5,
                ImmediateFlush = true,
                PreserveLogFileNameExtension = true,
                StaticLogFileName = true,
            };
            a.ActivateOptions();
            BasicConfigurator.Configure(a);
        }

        public Logger(Type type)
        {
            log = LogManager.GetLogger(type);
        }

        //  Methods
        //  =======

        public void Debug(string message)
        {
            log.Debug(string.Format(DEBUG_FORMAT, DateTime.Now.ToString(DATETIME_FORMAT), message));
        }

        public void Info(string message)
        {
            log.Info(string.Format(INFO_FORMAT, DateTime.Now.ToString(DATETIME_FORMAT), message));
        }

        public void Repo(string action, string repo, string message)
        {
            log.Info(string.Format(REPO_FORMAT, DateTime.Now.ToString(DATETIME_FORMAT), action, repo, message));
        }

        public void Warn(string message)
        {
            log.Warn(string.Format(WARN_FORMAT, DateTime.Now.ToString(DATETIME_FORMAT), message));
        }

        public void Error(string message)
        {
            log.Error(string.Format(ERROR_FORMAT, DateTime.Now.ToString(DATETIME_FORMAT), message));
        }

        public void Fatal(string message)
        {
            log.Fatal(string.Format(FATAL_FORMAT, DateTime.Now.ToString(DATETIME_FORMAT), message));
        }
    }
}