using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель индивидуального потока мини-тикера символа
    /// </summary>
    internal class MiniTickerStreamModel : MarketdataStreamModelBase, IMarketdataStreamModel, IHaveMyOwnJsonConverter
    {
        /// <inheritdoc />
        public MarketdataStreamType StreamType => MarketdataStreamType.IndividualSymbolMiniTickerStream;

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        [JsonPropertyName("c")]
        public double ClosePrice { get; set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        [JsonPropertyName("o")]
        public double OpenPrice { get; set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        [JsonPropertyName("l")]
        public double MinPrice { get; set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        [JsonPropertyName("h")]
        public double MaxPrice { get; set; }

        /// <summary>
        ///     Объем базового актива, который купили тейкеры
        /// </summary>
        [JsonPropertyName("v")]
        public double BasePurchaseVolume { get; set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        [JsonPropertyName("q")]
        public double QuotePurchaseVolume { get; set; }

        /// <inheritdoc />
        public void SetProperties(ref Utf8JsonReader reader, IHaveMyOwnJsonConverter temp)
        {
            var result = temp as MiniTickerStreamModel;
            string lastPropertyName = "";
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    lastPropertyName = reader.GetString();
                    reader.Read();
                }

                switch (lastPropertyName)
                {
                    case "s":
                        result.Symbol = reader.GetString();
                        continue;
                    case "E":
                        result.EventTimeUnix = reader.GetInt64();
                        continue;
                    case "c":
                        result.ClosePrice = double.Parse(reader.GetString());
                        continue;
                    case "o":
                        result.OpenPrice = double.Parse(reader.GetString());
                        continue;
                    case "l":
                        result.MinPrice = double.Parse(reader.GetString());
                        continue;
                    case "h":
                        result.MaxPrice = double.Parse(reader.GetString());
                        continue;
                    case "v":
                        result.BasePurchaseVolume = double.Parse(reader.GetString());
                        continue;
                    case "q":
                        result.QuotePurchaseVolume = double.Parse(reader.GetString());
                        continue;
                }
            }
        }
    }
}
