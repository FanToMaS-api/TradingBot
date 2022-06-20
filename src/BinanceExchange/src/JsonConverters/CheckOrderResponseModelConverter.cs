using BinanceExchange.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.JsonConverters
{
    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    internal class CheckOrderResponseModelConverter : JsonConverter<CheckOrderResponseModel>
    {
        /// <inheritdoc />
        public override CheckOrderResponseModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var model = new CheckOrderResponseModel();
            model.SetProperties(ref reader);

            return model;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, CheckOrderResponseModel value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
