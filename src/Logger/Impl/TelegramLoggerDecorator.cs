using Logger.Configuration;
using NLog;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Builder;
using Telegram.Client;

namespace Logger.Impl
{
    /// <summary>
    ///     Логгер с возможностью отправки сообщений в телеграмм
    /// </summary>
    internal class TelegramLoggerDecorator : ILoggerDecorator
    {
        #region Fields

        private readonly ITelegramClient _client;
        private readonly LogLevel _minLogLevel;
        private readonly TelegramLoggerConfiguration _loggerConfiguration;

        #endregion

        #region .ctor

        /// <inheritdoc cref="TelegramLoggerDecorator"/>
        public TelegramLoggerDecorator(
            ILoggerDecorator baseLogger,
            ITelegramClient client,
            LogLevel minLogLevel,
            TelegramLoggerConfiguration loggerConfiguration)
        {
            BaseLogger = baseLogger;
            _client = client;
            _minLogLevel = minLogLevel;
            _loggerConfiguration = loggerConfiguration;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public ILoggerDecorator BaseLogger { get; }

        #endregion

        #region Public methods

        /// <inheritdoc />
        public async Task WarnAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            await BaseLogger?.WarnAsync(message, memberName, lineNumber, cancellationToken);
            if (_minLogLevel <= LogLevel.Warn)
            {
                await WriteLogAsync(message, memberName, lineNumber, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task WarnAsync(
            Exception exception,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            await BaseLogger?.WarnAsync(exception, message, memberName, lineNumber, cancellationToken);
            if (_minLogLevel <= LogLevel.Warn)
            {
                message += $" {exception.Message}";
                await WriteLogAsync(message, memberName, lineNumber, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task TraceAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            await BaseLogger?.TraceAsync(message, memberName, lineNumber, cancellationToken);
            if (_minLogLevel <= LogLevel.Trace)
            {
                await WriteLogAsync(message, memberName, lineNumber, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task ErrorAsync(
            Exception exception,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            await BaseLogger?.ErrorAsync(exception, message, memberName, lineNumber, cancellationToken);
            if (_minLogLevel <= LogLevel.Error)
            {
                message += $" {exception.Message}";
                await WriteLogAsync(message, memberName, lineNumber, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task ErrorAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            await BaseLogger?.ErrorAsync(message, memberName, lineNumber, cancellationToken);
            if (_minLogLevel <= LogLevel.Error)
            {
                await WriteLogAsync(message, memberName, lineNumber, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task InfoAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            await BaseLogger?.InfoAsync(message, memberName, lineNumber, cancellationToken);
            if (_minLogLevel <= LogLevel.Info)
            {
                await WriteLogAsync(message, memberName, lineNumber, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task DebugAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            await BaseLogger?.DebugAsync(message, memberName, lineNumber, cancellationToken);
            if (_minLogLevel <= LogLevel.Debug)
            {
                await WriteLogAsync(message, memberName, lineNumber, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task FatalAsync(
            Exception exception,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            await BaseLogger?.FatalAsync(exception, message, memberName, lineNumber, cancellationToken);
            if (_minLogLevel <= LogLevel.Fatal)
            {
                message += $" {exception.Message}";
                await WriteLogAsync(message, memberName, lineNumber, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task FatalAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default)
        {
            BaseLogger?.FatalAsync(message, memberName, lineNumber, cancellationToken);
            if (_minLogLevel <= LogLevel.Fatal)
            {
                await WriteLogAsync(message, memberName, lineNumber, cancellationToken);
            }
        }

        #endregion

        #region Private methods

        /// <inheritdoc cref="NLog.Logger.Log"/>
        private async Task WriteLogAsync(string message, string memberName, int? lineNumber, CancellationToken cancellationToken)
        {
            var builder = new TelegramMessageBuilder();
            builder.SetChatId(long.Parse(_loggerConfiguration.ChatId));
            builder.SetMessageText($"Caller: {memberName}\nLine: {lineNumber}\n{message}");
            var messageModel = builder.GetResult();
            try
            {
                await _client.SendMessageAsync(messageModel, cancellationToken);
            }
            catch (Exception exception)
            {
                await BaseLogger?.ErrorAsync(
                    exception,
                    "Failed to send log \n\"{message}\"\n to telegram",
                    cancellationToken: cancellationToken);
            }
        }

        #endregion

        #region Implementation IDisposable

        /// <inheritdoc />
        public void Dispose() => BaseLogger?.Dispose();

        #endregion
    }
}
