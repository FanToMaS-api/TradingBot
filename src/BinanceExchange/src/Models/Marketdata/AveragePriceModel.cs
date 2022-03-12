using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель средней цены тикера
    /// </summary>
    internal class AveragePriceModel
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
