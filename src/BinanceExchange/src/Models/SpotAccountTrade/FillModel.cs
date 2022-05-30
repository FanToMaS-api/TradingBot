using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Содержит информацию о частях заполнения ордера
    /// </summary>
    public class FillModel : IEquatable<FillModel>
    {
        /// <summary>
        ///     Цена
        /// </summary>
        [JsonPropertyName("price")]
        public double Price { get; set; }

        /// <summary>
        ///     Кол-во
        /// </summary>
        [JsonPropertyName("qty")]
        public double Quantity { get; set; }

        /// <summary>
        ///     Коммиссия
        /// </summary>
        [JsonPropertyName("commission")]
        public double Commission { get; set; }

        /// <summary>
        ///     Актив комиссии
        /// </summary>
        [JsonPropertyName("commissionAsset")]
        public string CommissionAsset { get; set; }

        /// <summary>
        ///     Id сделки
        /// </summary>
        [JsonPropertyName("tradeId")]
        public long TradeId { get; set; }

        /// <summary>
        ///     Создает новую модель и добавляет в нужный массив пар
        /// </summary>
        internal static void CreateFillModel(ref Utf8JsonReader reader, FullOrderResponseModel result)
        {
            var workItem = new FillModel();
            var propertyName = "";
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                }

                switch (propertyName)
                {
                    case "price":
                        workItem.Price = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "qty":
                        workItem.Quantity = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "commission":
                        workItem.Commission = double.Parse(reader.GetString(), CultureInfo.InvariantCulture);
                        continue;
                    case "commissionAsset":
                        workItem.CommissionAsset = reader.GetString();
                        continue;
                    case "tradeId":
                        workItem.TradeId = reader.GetInt64();
                        continue;
                }
            }

            result.Fills.Add(workItem);
        }

        /// <inheritdoc />
        public bool Equals(FillModel other)
        {
            return Price == other.Price
                && Quantity == other.Quantity
                && CommissionAsset == other.CommissionAsset
                && TradeId == other.TradeId
                && Commission == other.Commission;
        }
    }
}
