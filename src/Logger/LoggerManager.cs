using Logger.Impl;

namespace Logger
{
    /// <summary>
    ///     Статический класс создания логгеров
    /// </summary>
    public static class LoggerManager
    {
        /// <summary>
        ///     Создать простой логгер
        /// </summary>
        public static ILoggerDecorator CreateDefaultLogger() => new BaseLoggerDecorator();
    }
}
