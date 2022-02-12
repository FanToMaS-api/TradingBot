using System.Text.Json.Serialization;

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
        [JsonPropertyName("mins")]
        public int Mins { get; set; }

        /// <summary>
        ///     Текущая средняя цена
        /// </summary>
        [JsonPropertyName("price")]
        public double AveragePrice { get; set; }
    }
}
