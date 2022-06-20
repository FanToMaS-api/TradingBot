using BinanceExchange.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.JsonConverters
{
    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    internal class ExchangeInfoModelConverter : JsonConverter<ExchangeInfoModel>
    {
        /// <inheritdoc />
        public override ExchangeInfoModel Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            var result = new ExchangeInfoModel();
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var lastPropertyName = reader.GetString();
                    reader.Read();

                    if (lastPropertyName == "symbols")
                    {
                        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                        {
                            var symbol = new SymbolInfoModel();
                            symbol.SetProperties(ref reader);

                            result.Symbols.Add(symbol);
                        }
                    }
                }
            }

            return result;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, ExchangeInfoModel value, JsonSerializerOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}
