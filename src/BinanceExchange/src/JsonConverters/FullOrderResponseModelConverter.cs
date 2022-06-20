using BinanceExchange.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.JsonConverters
{
    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    internal class FullOrderResponseModelConverter : JsonConverter<FullOrderResponseModel>
    {
        /// <inheritdoc />
        public override FullOrderResponseModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var orderBook = new FullOrderResponseModel();
            orderBook.SetProperties(ref reader);

            return orderBook;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, FullOrderResponseModel value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
