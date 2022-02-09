using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.Marketdata
{
    /// <summary>
    ///     Модель текущей цены пары
    /// </summary>
    public class SymbolPriceTickerDto
    {
        /// <summary>
        ///     Название пары
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Цена
        /// </summary>
        [JsonProperty("price")]
        public double Price { get; set; }
    }
}
