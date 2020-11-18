﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ReportPortal.Shared.Extensibility
{
    public class ExtensionManager : IExtensionManager
    {
        private static Internal.Logging.ITraceLogger TraceLogger { get; } = Internal.Logging.TraceLogManager.Instance.GetLogger(typeof(ExtensionManager));

        private static Lazy<IExtensionManager> _instance = new Lazy<IExtensionManager>(() =>
            {
                var ext = new ExtensionManager();
                ext.Explore(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                ext.Explore(Environment.CurrentDirectory);
                return ext;
            });

        public static IExtensionManager Instance => _instance.Value;

        private List<string> _exploredPaths = new List<string>();

        private List<string> _exploredAssemblies = new List<string>();

        private static object _lockObj = new object();

        public void Explore(string path)
        {
            if (!_exploredPaths.Contains(path))
            {
                lock (_lockObj)
                {
                    if (!_exploredPaths.Contains(path))
                    {
                        var logFormatters = new List<ILogFormatter>();
                        var reportEventObservers = new List<IReportEventsObserver>();
                        var commandsListeners = new List<ICommandsListener>();

                        var currentDirectory = new DirectoryInfo(path);

                        TraceLogger.Info($"Exploring extensions in '{currentDirectory}' directory.");

                        foreach (var file in currentDirectory.GetFiles("*ReportPortal*.dll"))
                        {
                            TraceLogger.Verbose($"Found '{file.Name}' and loading it into current AppDomain.");
                            AppDomain.CurrentDomain.Load(Path.GetFileNameWithoutExtension(file.Name));
                        }

                        var iLogFormatterExtensionInterfaceType = typeof(ILogFormatter);
                        var iReportEventObserseExtensionInterfaceType = typeof(IReportEventsObserver);
                        var iCommandsListenerInterfaceType = typeof(ICommandsListener);

                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetName().Name.Contains("ReportPortal")))
                        {
                            if (!_exploredAssemblies.Contains(assembly.Location))
                            {
                                _exploredAssemblies.Add(assembly.Location);
                                TraceLogger.Verbose($"Exploring '{assembly.FullName}' assembly for extensions.");
                                try
                                {
                                    foreach (var type in assembly.GetTypes().Where(t => t.IsClass))
                                    {
                                        if (!type.IsAbstract && type.GetConstructors().Any(ctor => ctor.GetParameters().Length == 0))
                                        {
                                            if (iLogFormatterExtensionInterfaceType.IsAssignableFrom(type))
                                            {
                                                var extension = Activator.CreateInstance(type);
                                                logFormatters.Add((ILogFormatter)extension);
                                                TraceLogger.Info($"Registered '{type.FullName}' type as {nameof(ILogFormatter)} extension.");
                                            }

                                            if (iReportEventObserseExtensionInterfaceType.IsAssignableFrom(type))
                                            {
                                                var extension = Activator.CreateInstance(type);
                                                reportEventObservers.Add((IReportEventsObserver)extension);
                                                TraceLogger.Info($"Registered '{type.FullName}' type as {nameof(IReportEventsObserver)} extension.");
                                            }

                                            if (iCommandsListenerInterfaceType.IsAssignableFrom(type))
                                            {
                                                var extension = Activator.CreateInstance(type);
                                                commandsListeners.Add((ICommandsListener)extension);
                                                TraceLogger.Info($"Registered '{type.FullName}' type as {nameof(ICommandsListener)} extension.");
                                            }
                                        }
                                    }
                                }
                                catch (ReflectionTypeLoadException exp)
                                {
                                    TraceLogger.Warn($"Couldn't load '{assembly.GetName().Name}' assembly into domain. \n {exp}");
                                    foreach (var loaderException in exp.LoaderExceptions)
                                    {
                                        TraceLogger.Error(loaderException.ToString());
                                    }
                                }
                            }
                        }

                        logFormatters.OrderBy(ext => ext.Order).ToList().ForEach(lf => LogFormatters.Add(lf));
                        reportEventObservers.ToList().ForEach(reo => ReportEventObservers.Add(reo));
                        commandsListeners.ForEach(cl => CommandsListeners.Add(cl));

                        _exploredPaths.Add(path);
                    }
                }
            }
        }

        public IList<ILogFormatter> LogFormatters { get; } = new List<ILogFormatter>();

        public IList<IReportEventsObserver> ReportEventObservers { get; } = new List<IReportEventsObserver>();

        public IList<ICommandsListener> CommandsListeners { get; } = new List<ICommandsListener>();
    }
}
