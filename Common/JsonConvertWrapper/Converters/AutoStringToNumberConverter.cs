using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.JsonConvertWrapper.Converters
{
    /// <summary>
    ///     Автоматически конвертирует строки в нужные типы
    /// </summary>
    /// <remarks> 
    ///     see https://stackoverflow.com/questions/59097784/system-text-json-deserialize-json-with-automatic-casting
    /// </remarks>
    public class AutoStringToNumberConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            // see https://stackoverflow.com/questions/1749966/c-sharp-how-to-determine-whether-a-type-is-a-number
            switch (Type.GetTypeCode(typeToConvert))
            {
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.String:
                case TypeCode.Boolean:
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Type.GetTypeCode(typeToConvert) switch
            {
                TypeCode.Int32 when reader.TokenType == JsonTokenType.Number => reader.GetInt32(),
                TypeCode.Int32 when reader.TokenType == JsonTokenType.String => int.Parse(reader.GetString()),
                TypeCode.Int64 when reader.TokenType == JsonTokenType.Number => reader.GetInt64(),
                TypeCode.Int64 when reader.TokenType == JsonTokenType.String => long.Parse(reader.GetString()),
                TypeCode.Double when reader.TokenType == JsonTokenType.Number => reader.GetDouble(),
                TypeCode.Double when reader.TokenType == JsonTokenType.String => double.Parse(reader.GetString()),
                TypeCode.Boolean => reader.GetBoolean(),
                TypeCode.String => reader.GetString(),
                _ => reader.GetString(),
            };
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var str = value.ToString();
            if (int.TryParse(str, out var i))
            {
                writer.WriteNumberValue(i);
            }
            else if (double.TryParse(str, out var d))
            {
                writer.WriteNumberValue(d);
            }
            else if (long.TryParse(str, out var l))
            {
                writer.WriteNumberValue(l);
            }
            else
            {
                throw new Exception($"Unable to parse {str} to number");
            }
        }
    }
}
