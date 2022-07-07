using Logger.Impl;
using System;
using System.Threading;

namespace Logger
{
    /// <summary>
    ///     Статический класс создания логгеров
    /// </summary>
    public static class LoggerManager
    {
        private static ILoggerDecorator Logger;

        /// <summary>
        ///     Создать базовый логгер
        /// </summary>
        public static ILoggerDecorator CreateDefaultLogger()
        {
            if (Logger is null)
            {
                Interlocked.CompareExchange(ref Logger, new BaseLoggerDecorator(), null);
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            }

            return Logger;
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Logger.Dispose();

            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;

            Logger.InfoAsync("Logger shutdown successfully").Wait();
        }
    }
}
