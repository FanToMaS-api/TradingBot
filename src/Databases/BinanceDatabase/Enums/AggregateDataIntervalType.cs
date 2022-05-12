namespace BinanceDatabase.Enums
{
    /// <summary>
    ///     Тип интервала агрегированных данных
    /// </summary>
    /// <remarks>
    ///     Важно соблюдать сортировку по росту интервалов 1s -> 1m -> 5m -> 1h -> ... 
    /// </remarks>
    public enum AggregateDataIntervalType
    {
        /// <summary>
        ///     Указывает, что агрегации не проводилось
        /// </summary>
        Default = 0,

        /// <summary>
        ///     Одна минута
        /// </summary>
        OneMinute = 1,

        /// <summary>
        ///     Пять минут
        /// </summary>
        FiveMinutes = 2,

        /// <summary>
        ///     Пятнадцать минут
        /// </summary>
        FifteenMinutes = 3,

        /// <summary>
        ///     1 час
        /// </summary>
        OneHour = 4
    }
}
