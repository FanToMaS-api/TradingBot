using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Enums;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель книги ордеров
    /// </summary>
    internal class OrderBookModel : IMarketdataStreamModel, IHaveMyOwnJsonConverter
    {
        /// <inheritdoc />
        public MarketdataStreamType StreamType => MarketdataStreamType.PartialBookDepthStream;

        /// <summary>
        ///    Идентификатор последнего обновления 
        /// </summary>
        public long LastUpdateId { get; set; }

        /// <summary>
        ///     Ордера на покупку
        /// </summary>
        public List<PriceQtyPair> Bids { get; set; } = new();

        /// <summary>
        ///     Ордера на продажу
        /// </summary>
        public List<PriceQtyPair> Asks { get; set; } = new();

        /// <inheritdoc />
        public void SetProperties(ref Utf8JsonReader reader)
        {
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
                            LastUpdateId = reader.GetInt64();
                            continue;
                    }
                }

                if (reader.TokenType != JsonTokenType.String)
                {
                    continue;
                }

                PriceQtyPair.CreatePair(ref reader, this, lastPropertyName);
            }
        }
    }

    /// <summary>
    ///     Модель цены и объема ордера
    /// </summary>
    public class PriceQtyPair
    {
        /// <summary>
        ///     Цена
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        ///     Создает пару и добавляет в нужный массив пар
        /// </summary>
        internal static void CreatePair(ref Utf8JsonReader reader, OrderBookModel result, string lastPropertyName)
        {
            var workItem = new PriceQtyPair();
            workItem.Price = double.Parse(reader.GetString());
            reader.Read();
            workItem.Quantity = double.Parse(reader.GetString());
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

    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    internal class OrderBookModelConverter : JsonConverter<OrderBookModel>
    {
        /// <inheritdoc />
        public override OrderBookModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var orderBook = new OrderBookModel();
            orderBook.SetProperties(ref reader);

            return orderBook;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, OrderBookModel value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
