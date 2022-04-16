using NLog;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Logger.Impl
{
    /// <summary>
    ///     Базовый класс декоратора
    /// </summary>
    internal class BaseLoggerDecorator : ILoggerDecorator
    {
        private readonly ILogger _logger = LogManager.LoadConfiguration("nlog.config").GetLogger("Logger");

        public ILoggerDecorator BaseLogger => null;

        #region Public methods

        /// <inheritdoc />
        public Task WarnAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            var eventInfo = new LogEventInfo(LogLevel.Warn, _logger.Name, message);
            WriteLog(eventInfo, memberName, lineNumber);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task WarnAsync(
            Exception exception,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            var eventInfo = new LogEventInfo(LogLevel.Warn, _logger.Name, message)
            {
                Exception = exception
            };
            WriteLog(eventInfo, memberName, lineNumber);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task TraceAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            var eventInfo = new LogEventInfo(LogLevel.Trace, _logger.Name, message);
            WriteLog(eventInfo, memberName, lineNumber);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ErrorAsync(
            Exception exception, 
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            var eventInfo = new LogEventInfo(LogLevel.Error, _logger.Name, message)
            {
                Exception = exception
            };
            WriteLog(eventInfo, memberName, lineNumber);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task ErrorAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            var eventInfo = new LogEventInfo(LogLevel.Error, _logger.Name, message);
            WriteLog(eventInfo, memberName, lineNumber);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task InfoAsync(
            string message = null, 
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            var eventInfo = new LogEventInfo(LogLevel.Info, _logger.Name, message);
            WriteLog(eventInfo, memberName, lineNumber);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task DebugAsync(
            string message = null, 
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            var eventInfo = new LogEventInfo(LogLevel.Debug, _logger.Name, message);
            WriteLog(eventInfo, memberName, lineNumber);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task FatalAsync(
            Exception exception, 
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            var eventInfo = new LogEventInfo(LogLevel.Fatal, _logger.Name, message)
            {
                Exception = exception
            };
            WriteLog(eventInfo, memberName, lineNumber);

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task FatalAsync(
            string message = null, 
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            var eventInfo = new LogEventInfo(LogLevel.Fatal, _logger.Name, message);
            WriteLog(eventInfo, memberName, lineNumber);

            return Task.CompletedTask;
        }

        #endregion

        #region Private methods

        /// <inheritdoc cref="NLog.Logger.Log"/>
        private void WriteLog(LogEventInfo eventInfo, string memberName, int? lineNumber)
        {
            eventInfo.Properties["CallerMemberName"] = memberName;
            eventInfo.Properties["CallerLineNumber"] = lineNumber;

            _logger.Log(eventInfo);
        }

        #endregion

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            LogManager.Shutdown();
        }

        #endregion
    }
}
