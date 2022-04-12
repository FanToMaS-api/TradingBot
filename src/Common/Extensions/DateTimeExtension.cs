using System;

namespace Common.Extensions
{
    /// <summary>
    ///     Класс расширяющий методы работы с DateTime
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        ///     Переводит миллисекунды из unix в DateTime
        /// </summary>
        public static DateTime FromUnixToDateTime(this long unixTimeStampMilliseconds)
        {
            var dateTime = new DateTime(1970, 1, 1, 3, 0, 0, 0);
            dateTime = dateTime.AddMilliseconds(unixTimeStampMilliseconds);

            return dateTime;
        }
    }
}
