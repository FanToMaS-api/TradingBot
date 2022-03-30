using Common.JsonConvertWrapper;
using BinanceExchange.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель данных обновления лучшей цены или количества спроса или предложения
    ///     в режиме реального времени для указанного символа
    /// </summary>
    internal class BookTickerStreamModel : IMarketdataStreamModel, IHaveMyOwnJsonConverter
    {
        /// <summary>
        ///     Тип стрима с которого получаем данные
        /// </summary>
        public MarketdataStreamType StreamType => MarketdataStreamType.IndividualSymbolBookTickerStream;

        /// <summary>
        ///     Идентификатор обновления книги заказов
        /// </summary>
        [JsonPropertyName("u")]
        public long OrderBookUpdatedId { get; set; }

        /// <summary>
        ///     Имя пары
        /// </summary>
        [JsonPropertyName("s")]
        public string Symbol { get; set; }

        /// <summary>
        ///    Лучшая цена спроса
        /// </summary>
        [JsonPropertyName("b")]
        public double BestBidPrice { get; set; }

        /// <summary>
        ///    Лучший объем спроса
        /// </summary>
        [JsonPropertyName("B")]
        public double BestBidQuantity { get; set; }

        /// <summary>
        ///    Лучшая цена предложения
        /// </summary>
        [JsonPropertyName("a")]
        public double BestAskPrice { get; set; }

        /// <summary>
        ///    Лучший объем предложения
        /// </summary>
        [JsonPropertyName("A")]
        public double BestAskQuantity { get; set; }

        /// <inheritdoc />
        public void SetProperties(ref Utf8JsonReader reader)
        {
            var lastPropertyName = "";
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    lastPropertyName = reader.GetString();
                    reader.Read();
                }

                switch (lastPropertyName)
                {
                    case "u":
                        OrderBookUpdatedId = reader.GetInt64();
                        continue;
                    case "s":
                        Symbol = reader.GetString();
                        continue;
                    case "b":
                        BestBidPrice = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "B":
                        BestBidQuantity = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "a":
                        BestAskPrice = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "A":
                        BestAskQuantity = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                }
            }
        }
    }
}
