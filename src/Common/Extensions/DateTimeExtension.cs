using System;

namespace Common.Extensions
{
    /// <summary>
    ///     Класс расширяющий методы работы с DateTime
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        ///     Переводит миллисекунды (utc) из unix в DateTime
        /// </summary>
        public static DateTime FromUnixToDateTime(this long unixTimeStampMilliseconds)
        {
            var dateTime = new DateTime(1970, 1, 1, 3, 0, 0, 0);
            dateTime = dateTime.AddMilliseconds(unixTimeStampMilliseconds);

            return dateTime;
        }

        /// <summary>
        ///     Переводит DateTime в миллисекунды unix (utc)
        /// </summary>
        public static long FromDateTimeToUnix(this DateTime dateTime)
        {
            var unix = (long)dateTime.Subtract(new DateTime(1970, 1, 1, 3, 0, 0, 0)).TotalMilliseconds;

            return unix;
        }
    }
}
