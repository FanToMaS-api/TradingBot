﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        [JsonPropertyName("lastUpdateId")]
        public long LastUpdateId { get; set; }

        /// <summary>
        ///     Список цен/объемов на покупку
        /// </summary>
        [JsonPropertyName("bids")]
        public List<PriceQtyPair> Bids { get; set; } = new();

        /// <summary>
        ///     Список цен/объемов на продажу
        /// </summary>
        [JsonPropertyName("asks")]
        public List<PriceQtyPair> Asks { get; set; } = new();
    }

    /// <summary>
    ///     Модель цены и объема монеты
    /// </summary>
    public class PriceQtyPair
    {
        /// <summary>
        ///     Цена монеты
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Qty { get; set; }
    }

    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    public class OrderBookDtoConverter : JsonConverter<OrderBookDto>
    {
        /// <inheritdoc />
        public override OrderBookDto Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new OrderBookDto();
            string lastPropertyName = "";

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    lastPropertyName = reader.GetString();
                    reader.Read();

                    switch (lastPropertyName)
                    {
                        case "lastUpdateId":
                            result.LastUpdateId = reader.GetInt64();
                            continue;
                    }
                }

                if (reader.TokenType != JsonTokenType.String)
                {
                    continue;
                }

                CreatePair(ref reader, result, lastPropertyName);
            }

            return result;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, OrderBookDto value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Создает пару и добавляет в нужный массив пар
        /// </summary>
        private void CreatePair(ref Utf8JsonReader reader, OrderBookDto result, string lastPropertyName)
        {
            var workItem = new PriceQtyPair();
            workItem.Price = double.Parse(reader.GetString());
            reader.Read();
            workItem.Qty = double.Parse(reader.GetString());
            switch (lastPropertyName)
            {
                case "bids":
                    result.Bids.Add(workItem);
                    break;
                case "asks":
                    result.Asks.Add(workItem);
                    break;
            }
        }
    }

}
