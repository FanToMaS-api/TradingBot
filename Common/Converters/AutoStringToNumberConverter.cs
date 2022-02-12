using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Converters
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
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc />
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                return (typeToConvert == typeof(int) && int.TryParse(s, out var i))
                    ? i
                    : (typeToConvert == typeof(long) && long.TryParse(s, out var l))
                    ? l
                    : (typeToConvert == typeof(double) && double.TryParse(s, out var d))
                    ? d
                    : throw new Exception($"Unable to parse {s} to number");
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                return typeToConvert == typeof(int)
                    ? reader.GetInt32()
                    : typeToConvert == typeof(long)
                    ? reader.GetInt64()
                    : throw new Exception($"Unable to parse to number");
            }

            using (var document = JsonDocument.ParseValue(ref reader))
            {
                throw new Exception($"Unable to parse {document.RootElement} to number");
            }
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
