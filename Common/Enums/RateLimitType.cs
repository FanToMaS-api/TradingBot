namespace Common.Enums
{
    /// <summary>
    ///     Тип ограничения скорости
    /// </summary>
    public enum RateLimitType
    {
        /// <summary>
        ///     Запрос к sapi
        /// </summary>
        SAPI_REQUEST,

        /// <summary>
        ///     Запрос к api
        /// </summary>
        API_REQUEST,
    }
}
