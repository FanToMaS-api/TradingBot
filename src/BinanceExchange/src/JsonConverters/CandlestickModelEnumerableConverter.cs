using BinanceExchange.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.JsonConverters
{
    /// <summary>
    ///     Конвертирует данные в массив объектов
    /// </summary>
    internal class CandlestickModelEnumerableConverter : JsonConverter<IEnumerable<CandlestickModel>>
    {
        /// <inheritdoc />
        public override IEnumerable<CandlestickModel> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new List<CandlestickModel>();
            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.StartArray)
                {
                    continue;
                }

                var newCandleStick = CandlestickModel.Create(ref reader);
                result.Add(newCandleStick);
            }

            return result;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IEnumerable<CandlestickModel> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
