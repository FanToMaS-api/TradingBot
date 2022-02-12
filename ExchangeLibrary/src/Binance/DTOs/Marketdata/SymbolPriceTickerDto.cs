using System.Text.Json.Serialization;

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
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Цена
        /// </summary>
        [JsonPropertyName("price")]
        public double Price { get; set; }
    }
}
