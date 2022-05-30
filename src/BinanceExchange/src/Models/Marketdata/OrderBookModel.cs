using Common.JsonConvertWrapper;
using BinanceExchange.Enums;
using System.Collections.Generic;
using System.Text.Json;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель книги ордеров
    /// </summary>
    internal class OrderBookModel : IMarketdataStreamModel, IHaveMyOwnJsonConverter
    {
        /// <inheritdoc />
        public MarketdataStreamType StreamType => MarketdataStreamType.PartialBookDepthStream;

        /// <summary>
        ///    Идентификатор последнего обновления 
        /// </summary>
        public long LastUpdateId { get; set; }

        /// <summary>
        ///     Ордера на покупку
        /// </summary>
        public List<PriceQtyPair> Bids { get; set; } = new();

        /// <summary>
        ///     Ордера на продажу
        /// </summary>
        public List<PriceQtyPair> Asks { get; set; } = new();

        /// <inheritdoc />
        public void SetProperties(ref Utf8JsonReader reader)
        {
            string lastPropertyName = "";
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    lastPropertyName = reader.GetString();
                    reader.Read();

                    switch (lastPropertyName)
                    {
                        case "lastUpdateId":
                            LastUpdateId = reader.GetInt64();
                            continue;
                    }
                }

                if (reader.TokenType != JsonTokenType.String)
                {
                    continue;
                }

                PriceQtyPair.CreatePair(ref reader, this, lastPropertyName);
            }
        }
    }
}
