using System;
using System.Diagnostics;
using System.Globalization;

namespace Orangebeard.Client.V3.ClientUtils.Logging
{
    internal class Logger : ILogger
    {
        private readonly TraceSource _traceSource;

        private readonly int _appDomainId;
        private readonly string _appDomainFriendlyName;

        public Logger(TraceSource traceSource)
        {
            _traceSource = traceSource;

            _appDomainId = AppDomain.CurrentDomain.Id;
            _appDomainFriendlyName = AppDomain.CurrentDomain.FriendlyName;
        }

        public void Info(string message)
        {
            Message(TraceEventType.Information, message);
        }

        public void Verbose(string message)
        {
            Message(TraceEventType.Verbose, message);
        }

        public void Error(string message)
        {
            Message(TraceEventType.Error, message);
        }

        public void Warn(string message)
        {
            Message(TraceEventType.Warning, message);
        }

        private void Message(TraceEventType eventType, string message)
        {
            var formattedMessage =
                $"{DateTime.Now.ToString("HH:mm:ss.fffffff", CultureInfo.InvariantCulture)} : {_appDomainId}-{_appDomainFriendlyName} : {message}";
            _traceSource.TraceEvent(eventType, 0, formattedMessage);
        }
    }
}