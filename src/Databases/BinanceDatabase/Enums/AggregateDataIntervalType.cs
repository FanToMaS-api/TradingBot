namespace BinanceDatabase.Enums
{
    /// <summary>
    ///     Тип интервала агрегированных данных
    /// </summary>
    public enum AggregateDataIntervalType
    {
        /// <summary>
        ///     Указывает, что агрегации не проводилось
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Одна минута
        /// </summary>
        OneMinute,

        /// <summary>
        ///     Пять минут
        /// </summary>
        FiveMinutes,

        /// <summary>
        ///     Пятнадцать минут
        /// </summary>
        FifteenMinutes,

        /// <summary>
        ///     1 час
        /// </summary>
        OneHour
    }
}
