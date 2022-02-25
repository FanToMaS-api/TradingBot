using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
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
        ///    Лучшая объем спроса
        /// </summary>
        [JsonPropertyName("B")]
        public double BestBidQuantity { get; set; }

        /// <summary>
        ///    Лучшая цена предложения
        /// </summary>
        [JsonPropertyName("a")]
        public double BestAskPrice { get; set; }

        /// <summary>
        ///    Лучшая объем предложения
        /// </summary>
        [JsonPropertyName("A")]
        public double BestAskQuantity { get; set; }

        /// <inheritdoc />
        public void SetProperties(ref Utf8JsonReader reader, IHaveMyOwnJsonConverter result)
        {
            var temp = result as BookTickerStreamModel;
            string lastPropertyName = "";
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
                        temp.OrderBookUpdatedId = reader.GetInt64();
                        continue;
                    case "s":
                        temp.Symbol = reader.GetString();
                        continue;
                    case "b":
                        temp.BestBidPrice = double.Parse(reader.GetString());
                        continue;
                    case "B":
                        temp.BestBidQuantity = double.Parse(reader.GetString());
                        continue;
                    case "a":
                        temp.BestAskPrice = double.Parse(reader.GetString());
                        continue;
                    case "A":
                        temp.BestAskQuantity = double.Parse(reader.GetString());
                        continue;
                }
            }
        }
    }
}
