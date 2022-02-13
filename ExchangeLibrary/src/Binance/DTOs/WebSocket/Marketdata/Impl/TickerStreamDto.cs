﻿using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.DTOs.WebSocket.Marketdata.Impl
{
    /// <summary>
    ///     Модель статистики бегущего окна за 24 часа для одного символа
    /// </summary>
    public class TickerStreamDto : MarketdataStreamDtoBase, IMarketdataStreamDto, IHaveMyOwnJsonConverter
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
        public void SetProperties(ref Utf8JsonReader reader, IHaveMyOwnJsonConverter result)
        {
            var temp = result as TickerStreamDto;
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
                        temp.Symbol = reader.GetString();
                        continue;
                    case "E":
                        temp.EventTimeUnix = reader.GetInt64();
                        continue;
                    case "p":
                        temp.Price = double.Parse(reader.GetString());
                        continue;
                    case "P":
                        temp.PricePercentChange = double.Parse(reader.GetString());
                        continue;
                    case "w":
                        temp.WeightedAveragePrice = double.Parse(reader.GetString());
                        continue;
                    case "x":
                        temp.FirstPrice = double.Parse(reader.GetString());
                        continue;
                    case "c":
                        temp.LastPrice = double.Parse(reader.GetString());
                        continue;
                    case "Q":
                        temp.LastQuantity = double.Parse(reader.GetString());
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
                    case "o":
                        temp.OpenPrice = double.Parse(reader.GetString());
                        continue;
                    case "l":
                        temp.MinPrice = double.Parse(reader.GetString());
                        continue;
                    case "h":
                        temp.MaxPrice = double.Parse(reader.GetString());
                        continue;
                    case "v":
                        temp.AllBaseVolume = double.Parse(reader.GetString());
                        continue;
                    case "q":
                        temp.AllQuoteVolume = double.Parse(reader.GetString());
                        continue;
                    case "O":
                        temp.StatisticOpenTimeUnix = reader.GetInt64();
                        continue;
                    case "C":
                        temp.StatisticCloseTimeUnix = reader.GetInt64();
                        continue;
                    case "F":
                        temp.FirstTradeId = reader.GetInt64();
                        continue;
                    case "L":
                        temp.LastTradeId = reader.GetInt64();
                        continue;
                    case "n":
                        temp.TradeNumber = reader.GetInt64();
                        continue;
                }
            }
        }
    }
}
