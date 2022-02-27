namespace ExchangeLibrary.Binance.Enums
{
    /// <summary>
    ///     Версии базовых конечных точек
    /// </summary>
    /// <remarks>
    ///     Используются, когда некоторые из них недоступны
    /// </remarks>
    internal enum ApiVersionType
    {
        /// <summary>
        ///     https://api.binance.com
        /// </summary>
        Default,

        /// <summary>
        ///     https://api1.binance.com
        /// </summary>
        ApiV1,

        // <summary>
        ///     https://api2.binance.com
        /// </summary>
        ApiV2,

        // <summary>
        ///     https://api3.binance.com
        /// </summary>
        ApiV3,
    }
}
