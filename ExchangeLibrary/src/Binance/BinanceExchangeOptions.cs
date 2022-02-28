﻿namespace ExchangeLibrary.Binance
{
    /// <summary>
    ///     Содержит настройки Binance Exchange
    /// </summary>
    public class BinanceExchangeOptions : OptionsBase
    {
        /// <inheritdoc />
        public override string Name => "Binance";

        /// <summary>
        ///     Ключ подключения к Binance
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        ///     Секретный ключ подключения к Binance
        /// </summary>
        public string SecretKey { get; set; }
    }
}