using Newtonsoft.Json;

namespace ExchangeLibrary.Binance.DTOs.Marketdata
{
    /// <summary>
    ///     Модель средней цены тикера
    /// </summary>
    public class AveragePriceDto
    {
        /// <summary>
        ///     Кол-во минут выборки данных средней цены
        /// </summary>
        [JsonProperty("mins")]
        public int Mins { get; set; }

        /// <summary>
        ///     Текущая средняя цена
        /// </summary>
        [JsonProperty("price")]
        public double AveragePrice { get; set; }
    }
}
