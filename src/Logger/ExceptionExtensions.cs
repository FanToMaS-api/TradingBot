using System;
using System.Linq;

namespace Logger
{
    /// <summary>
    ///     Расширяет работу с ошибками
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        ///     Возвращает весь трейс стека
        /// </summary>
        public static string GetFullMessage(this Exception exception)
        {
            var message = exception.Message;
            if (exception is AggregateException aggregateEx)
            {
                message += $"\n{string.Join("\n", aggregateEx.InnerExceptions.Select(_ => _.Message))}";
            }
            else if (exception.InnerException is not null)
            {
                message += $"\n{exception.InnerException.Message}";
            }

            return message;
        }
    }
}
