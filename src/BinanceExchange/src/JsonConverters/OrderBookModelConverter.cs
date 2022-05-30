using BinanceExchange.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.JsonConverters
{
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
