using BinanceExchange.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.JsonConverters
{
    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    internal class CandlestickModelConverter : JsonConverter<CandlestickModel>
    {
        /// <inheritdoc />
        public override CandlestickModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return CandlestickModel.Create(ref reader);
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, CandlestickModel value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
