using BinanceExchange.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.JsonConverters
{
    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    internal class CancelOrderResponseModelConverter : JsonConverter<CancelOrderResponseModel>
    {
        /// <inheritdoc />
        public override CancelOrderResponseModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var model = new CancelOrderResponseModel();
            model.SetProperties(ref reader);

            return model;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, CancelOrderResponseModel value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
