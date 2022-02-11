using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata
{
    /// <summary>
    ///     Модель потока обновления информации о свече пары
    /// </summary>
    internal class CandleStickStreamDto : MarketdataStreamDtoBase
    {
        /// <inheritdoc />
        public override MarketdataStreamType StreamType => MarketdataStreamType.CandleStickStream;

        /// <summary>
        ///     Данные о свече
        /// </summary>
        [JsonProperty("k")]
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
        [JsonProperty("t")]
        public long KineStartTimeUnix { get; set; }

        /// <summary>
        ///     Время закрытия свечи
        /// </summary>
        [JsonProperty("T")]
        public long KineStopTimeUnix { get; set; }

        /// <summary>
        ///     Пара тикеров
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Интервал
        /// </summary>
        public CandleStickIntervalType Interval => _interval.ConvertToCandleStickIntervalType();

        /// <summary>
        ///     Интервал
        /// </summary>
        [JsonProperty("i")]
        private string _interval { get; set; }

        /// <summary>
        ///     Первое Id сделки
        /// </summary>
        [JsonProperty("f")]
        public long FirstTradeId { get; set; }

        /// <summary>
        ///     Последнее Id сделки
        /// </summary>
        [JsonProperty("L")]
        public long LastTradeId { get; set; }

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
        ///     Объем
        /// </summary>
        [JsonProperty("v")]
        public double Volume { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        [JsonProperty("c")]
        public double ClosePrice { get; set; }

        /// <summary>
        ///     Закрыта ли свеча
        /// </summary>
        [JsonProperty("x")]
        public bool IsKlineClosed { get; set; }

        /// <summary>
        ///     Объем котируемого актива
        /// </summary>
        [JsonProperty("q")]
        public double QuoteAssetVolume { get; set; }

        /// <summary>
        ///     Кол-во сделок
        /// </summary>
        [JsonProperty("n")]
        public int TradesNumber { get; set; }

        /// <summary>
        ///     Объем базового актива, который купили тейкеры
        /// </summary>
        [JsonProperty("V")] 
        public double BasePurchaseVolume { get; set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        [JsonProperty("Q")]
        public double QuotePurchaseVolume { get; set; }
    }
}
