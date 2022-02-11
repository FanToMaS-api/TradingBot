using ExchangeLibrary.Binance.Enums;
using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata
{
    /// <summary>
    ///     Модель индивидуального потока мини-тикера символа
    /// </summary>
    internal class MiniTickerStreamDto : MarketdataStreamDtoBase
    {
        /// <inheritdoc />
        public override MarketdataStreamType StreamType => MarketdataStreamType.IndividualSymbolMiniTickerStream;

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        [JsonProperty("с")]
        public double ClosePrice{ get; set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        [JsonProperty("o")]
        public double OpenPrice { get; set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        [JsonProperty("l")]
        public double MinPrice { get; set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        [JsonProperty("h")]
        public double MaxPrice { get; set; }

        /// <summary>
        ///     Объем базового актива, который купили тейкеры
        /// </summary>
        [JsonProperty("v")]
        public double BasePurchaseVolume { get; set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        [JsonProperty("q")]
        public double QuotePurchaseVolume { get; set; }
    }
}
