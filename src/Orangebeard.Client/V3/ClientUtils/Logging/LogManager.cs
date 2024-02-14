using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Orangebeard.Client.V3.ClientUtils.Logging
{
    /// <summary>
    /// Class to manage all internal loggers.
    /// </summary>
    public class LogManager
    {
        static LogManager()
        {
        }

        private static readonly Lazy<LogManager> LogManagerInstance = new Lazy<LogManager>(() => new LogManager());

        /// <summary>
        /// Returns single instance of <see cref="LogManager"/>
        /// </summary>
        public static LogManager Instance => LogManagerInstance.Value;

        private string _baseDir = Environment.CurrentDirectory;


        /// <summary>
        /// Fluently sets BaseDir.
        /// </summary>
        /// <param name="baseDir"></param>
        /// <returns></returns>
        public LogManager WithBaseDir(string baseDir)
        {
            _baseDir = baseDir;

            return this;
        }

        private static readonly object LockObj = new object();

        private static readonly Dictionary<Type, ILogger> _loggers = new Dictionary<Type, ILogger>();

        /// <summary>
        /// Gets or creates new logger for requested type.
        /// </summary>
        /// <param name="type">Type where logger should be registered for</param>
        /// <returns><see cref="ILogger"/> instance for logging internal messages</returns>
        private ILogger GetLogger(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            
            lock (LockObj)
            {
                if (_loggers.TryGetValue(type, out var logger)) return logger;

                var envTraceLevelValue = Environment.GetEnvironmentVariable("Orangebeard_TraceLevel");

                if (!Enum.TryParse(envTraceLevelValue, ignoreCase: true, out SourceLevels traceLevel))
                {
                    traceLevel = SourceLevels.Error;
                }

                var traceSource = new TraceSource(type.Name)
                {
                    Switch = new SourceSwitch("Orangebeard_TraceSwitch", traceLevel.ToString())
                };

                var logFileName = $"{type.Assembly.GetName().Name}.{Process.GetCurrentProcess().Id}.log";

                logFileName = Path.Combine(_baseDir, logFileName);

                var traceListener = new DefaultTraceListener
                {
                    Filter = new SourceFilter(traceSource.Name),
                    LogFileName = logFileName
                };

                traceSource.Listeners.Add(traceListener);

                _loggers[type] = new Logger(traceSource);
            }

            return _loggers[type];
        }

        /// <summary>
        /// Gets or creates new logger for requested type.
        /// </summary>
        /// <typeparam name="T">Type where logger should be registered for</typeparam>
        /// <returns><see cref="ILogger"/> instance for logging internal messages</returns>
        public ILogger GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }
    }
}