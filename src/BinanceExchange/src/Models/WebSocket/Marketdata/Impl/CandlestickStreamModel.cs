using BinanceExchange.Enums;
using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель потока обновления информации о свече пары
    /// </summary>
    internal class CandlestickStreamModel : MarketdataStreamModelBase, IMarketdataStreamModel
    {
        /// <inheritdoc />
        public MarketdataStreamType StreamType => MarketdataStreamType.CandlestickStream;

        /// <summary>
        ///     Данные о свече
        /// </summary>
        [JsonPropertyName("k")]
        public KlineModel Kline { get; set; }
    }
}
