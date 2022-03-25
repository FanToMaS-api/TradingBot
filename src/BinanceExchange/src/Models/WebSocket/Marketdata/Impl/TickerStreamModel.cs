using Common.JsonConvertWrapper;
using BinanceExchange.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель статистики бегущего окна за 24 часа для одного символа
    /// </summary>
    internal class TickerStreamModel : MarketdataStreamModelBase, IMarketdataStreamModel, IHaveMyOwnJsonConverter
    {
        /// <inheritdoc />
        public MarketdataStreamType StreamType => MarketdataStreamType.IndividualSymbolTickerStream;

        /// <summary>
        ///    Цена
        /// </summary>
        [JsonPropertyName("p")]
        public double Price { get; set; }

        /// <summary>
        ///    Изменение цены в процентах
        /// </summary>
        [JsonPropertyName("P")]
        public double PricePercentChange { get; set; }

        /// <summary>
        ///    Средневзвешенная цена
        /// </summary>
        [JsonPropertyName("w")]
        public double WeightedAveragePrice { get; set; }

        /// <summary>
        ///    Цена самой первой сделки до 24-х часового скользящего окна
        /// </summary>
        [JsonPropertyName("x")]
        public double FirstPrice { get; set; }

        /// <summary>
        ///    Последняя цена
        /// </summary>
        [JsonPropertyName("c")]
        public double LastPrice { get; set; }

        /// <summary>
        ///    Последнее кол-во
        /// </summary>
        [JsonPropertyName("Q")]
        public double LastQuantity { get; set; }

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

        /// <summary>
        ///    Цена открытия
        /// </summary>
        [JsonPropertyName("o")]
        public double OpenPrice { get; set; }

        /// <summary>
        ///    Максимальная цена
        /// </summary>
        [JsonPropertyName("h")]
        public double MaxPrice { get; set; }

        /// <summary>
        ///    Минимальная цена
        /// </summary>
        [JsonPropertyName("l")]
        public double MinPrice { get; set; }

        /// <summary>
        ///    Общий торгуемый объем базовых активов
        /// </summary>
        [JsonPropertyName("v")]
        public double AllBaseVolume { get; set; }

        /// <summary>
        ///    Общий торгуемый объем котировочного актива
        /// </summary>
        [JsonPropertyName("q")]
        public double AllQuoteVolume { get; set; }

        /// <summary>
        ///    Время открытия статистики
        /// </summary>
        [JsonPropertyName("O")]
        public long StatisticOpenTimeUnix { get; set; }

        /// <summary>
        ///    Время закрытия статистики
        /// </summary>
        [JsonPropertyName("C")]
        public long StatisticCloseTimeUnix { get; set; }

        /// <summary>
        ///    Id первой сделки
        /// </summary>
        [JsonPropertyName("F")]
        public long FirstTradeId { get; set; }

        /// <summary>
        ///    Id последней сделки
        /// </summary>
        [JsonPropertyName("L")]
        public long LastTradeId { get; set; }

        /// <summary>
        ///    Число сделок
        /// </summary>
        [JsonPropertyName("n")]
        public long TradeNumber { get; set; }

        /// <inheritdoc />
        public void SetProperties(ref Utf8JsonReader reader)
        {
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
                    case "s":
                        Symbol = reader.GetString();
                        continue;
                    case "E":
                        EventTimeUnix = reader.GetInt64();
                        continue;
                    case "p":
                        Price = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "P":
                        PricePercentChange = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "w":
                        WeightedAveragePrice = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "x":
                        FirstPrice = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "c":
                        LastPrice = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "Q":
                        LastQuantity = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
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
                    case "o":
                        OpenPrice = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "l":
                        MinPrice = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "h":
                        MaxPrice = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "v":
                        AllBaseVolume = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "q":
                        AllQuoteVolume = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "O":
                        StatisticOpenTimeUnix = reader.GetInt64();
                        continue;
                    case "C":
                        StatisticCloseTimeUnix = reader.GetInt64();
                        continue;
                    case "F":
                        FirstTradeId = reader.GetInt64();
                        continue;
                    case "L":
                        LastTradeId = reader.GetInt64();
                        continue;
                    case "n":
                        TradeNumber = reader.GetInt64();
                        continue;
                }
            }
        }
    }
}
