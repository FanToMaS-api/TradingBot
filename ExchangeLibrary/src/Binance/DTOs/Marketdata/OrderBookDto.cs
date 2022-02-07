using Newtonsoft.Json;
using System.Collections.Generic;

namespace ExchangeLibrary.Binance.DTOs.Marketdata
{
    /// <summary>
    ///     Модель книги заказов
    /// </summary>
    public class OrderBookDto
    {
        /// <summary>
        ///    Идентификатор последнего обновления 
        /// </summary>
        [JsonProperty("lastUpdateId")]
        public long LastUpdateId { get; set; }

        [JsonProperty("bids")]
        public List<double> Bids { get; set; }

        [JsonProperty("asks")]
        public List<double> Asks { get; set; }
    }
}
