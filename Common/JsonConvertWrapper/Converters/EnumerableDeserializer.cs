using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.JsonConvertWrapper.Converters
{
    /// <summary>
    ///     Конвертирует данные в массив объектов
    /// </summary>
    public class EnumerableDeserializer<T> : JsonConverter<IEnumerable<T>>
        where T : IHaveMyOwnJsonConverter, new()
    {
        /// <inheritdoc />
        public override IEnumerable<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new List<T>();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    T obj = new();
                    obj.SetProperties(ref reader, obj);

                    result.Add(obj);
                }
            }

            return result;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, IEnumerable<T> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}

