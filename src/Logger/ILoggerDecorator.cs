using NLog;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Logger
{
    /// <summary>
    ///     Декоратор над <see cref="ILogger"/>
    /// </summary>
    public interface ILoggerDecorator : IDisposable
    {
        #region Properties

        /// <summary>
        ///     Базовый логгер
        /// </summary>
        ILoggerDecorator BaseLogger { get; }

        #endregion

        #region Public methods

        /// <inheritdoc cref="NLog.Logger.Warn"/>
        Task WarnAsync(string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="NLog.Logger.Warn"/>
        Task WarnAsync(Exception exception,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="NLog.Logger.Trace"/>
        Task TraceAsync(string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="NLog.Logger.Error"/>
        Task ErrorAsync(Exception exception,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="NLog.Logger.Error"/>
        Task ErrorAsync(string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="NLog.Logger.Info"/>
        Task InfoAsync(string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="NLog.Logger.Debug"/>
        Task DebugAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="NLog.Logger.Fatal"/>
        Task FatalAsync(
            Exception exception,
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default);

        /// <inheritdoc cref="NLog.Logger.Fatal"/>
        Task FatalAsync(
            string message = null,
            [CallerMemberName] string memberName = null,
            [CallerLineNumber] int? lineNumber = null,
            CancellationToken cancellationToken = default);

        #endregion
    }
}
