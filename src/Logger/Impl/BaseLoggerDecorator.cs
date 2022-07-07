using NLog;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Logger.Impl
{
    /// <summary>
    ///     Базовый логгер
    /// </summary>
    internal class BaseLoggerDecorator : ILoggerDecorator
    {
        #region Fields

        private readonly ILogger _logger;

        #endregion

        #region .ctor

        /// <inheritdoc cref="BaseLoggerDecorator"/>
        public BaseLoggerDecorator()
        {
            using var loggerFactory = LogManager.LoadConfiguration("nlog.config");
            _logger = loggerFactory.GetLogger("Logger");
        }

        #endregion

        #region Implementation of ILoggerDecorator

        /// <inheritdoc />
        public ILoggerDecorator BaseLogger => null;

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

        #region Implementation of IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            LogManager.Shutdown();
        }

        #endregion
    }
}
