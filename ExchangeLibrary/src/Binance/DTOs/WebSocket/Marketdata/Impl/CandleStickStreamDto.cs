using ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata.Impl
{
    /// <summary>
    ///     Модель потока обновления информации о свече пары
    /// </summary>
    internal class CandleStickStreamDto : MarketdataStreamDtoBase, IMarketdataStreamDto
    {
        /// <inheritdoc />
        public MarketdataStreamType StreamType => MarketdataStreamType.CandleStickStream;

        /// <summary>
        ///     Данные о свече
        /// </summary>
        [JsonPropertyName("k")]
        public KlineModelDto Kline { get; set; }
    }

    /// <summary>
    ///     Модель данных о свече
    /// </summary>
    internal class KlineModelDto
    {
        /// <summary>
        ///     Время открытия свечи
        /// </summary>
        [JsonPropertyName("t")]
        public long KineStartTimeUnix { get; set; }

        /// <summary>
        ///     Время закрытия свечи
        /// </summary>
        [JsonPropertyName("T")]
        public long KineStopTimeUnix { get; set; }

        /// <summary>
        ///     Пара тикеров
        /// </summary>
        [JsonPropertyName("s")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Интервал
        /// </summary>
        public CandleStickIntervalType Interval => _interval.ConvertToCandleStickIntervalType();

        /// <summary>
        ///     Интервал
        /// </summary>
        [JsonPropertyName("i")]
        private string _interval { get; set; }

        /// <summary>
        ///     Первое Id сделки
        /// </summary>
        [JsonPropertyName("f")]
        public long FirstTradeId { get; set; }

        /// <summary>
        ///     Последнее Id сделки
        /// </summary>
        [JsonPropertyName("L")]
        public long LastTradeId { get; set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        [JsonPropertyName("o")]
        public double OpenPrice { get; set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        [JsonPropertyName("l")]
        public double MinPrice { get; set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        [JsonPropertyName("h")]
        public double MaxPrice { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        [JsonPropertyName("v")]
        public double Volume { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        [JsonPropertyName("c")]
        public double ClosePrice { get; set; }

        /// <summary>
        ///     Закрыта ли свеча
        /// </summary>
        [JsonPropertyName("x")]
        public bool IsKlineClosed { get; set; }

        /// <summary>
        ///     Объем котируемого актива
        /// </summary>
        [JsonPropertyName("q")]
        public double QuoteAssetVolume { get; set; }

        /// <summary>
        ///     Кол-во сделок
        /// </summary>
        [JsonPropertyName("n")]
        public int TradesNumber { get; set; }

        /// <summary>
        ///     Объем базового актива, который купили тейкеры
        /// </summary>
        [JsonPropertyName("V")]
        public double BasePurchaseVolume { get; set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        [JsonPropertyName("Q")]
        public double QuotePurchaseVolume { get; set; }
    }
}
