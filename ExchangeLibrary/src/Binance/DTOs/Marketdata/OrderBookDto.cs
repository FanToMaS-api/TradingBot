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

        /// <summary>
        ///     Список цен/объемов на покупку
        /// </summary>
        /// <remarks>
        ///     Содержит списки с 2мя элементами, где 1ый это Цена 2ой - кол-во
        /// </remarks>
        [JsonProperty("bids")]
        public List<List<double>> Bids { get; set; }

        /// <summary>
        ///     Список цен/объемов на продажу
        /// </summary>
        /// <remarks>
        ///     Содержит списки с 2мя элементами, где 1ый это Цена 2ой - кол-во
        /// </remarks>
        [JsonProperty("asks")]
        public List<List<double>> Asks { get; set; }
    }


}
