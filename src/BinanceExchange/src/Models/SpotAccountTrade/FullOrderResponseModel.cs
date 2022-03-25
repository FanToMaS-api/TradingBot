using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель ответа на отправку нового ордера (содержит полную информацию)
    /// </summary>
    internal class FullOrderResponseModel : OrderResponseModelBase
    {
        /// <summary>
        ///     Время исполнения транзакции
        /// </summary>
        public long TransactTimeUnix { get; set; }

        /// <summary>
        ///     Части заполнения ордера
        /// </summary>
        public List<FillModel> Fills { get; set; } = new();

        /// <summary>
        ///     Установить свойства
        /// </summary>
        public void SetProperties(ref Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    FillModel.CreateFillModel(ref reader, this);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "transactTime":
                            TransactTimeUnix = reader.GetInt64();
                            continue;
                        case "fills":
                            continue;
                        default:
                            {
                                SetProperty(propertyName, ref reader);
                                continue;
                            }
                    }
                }
            }
        }
    }

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
                        workItem.Price = double.Parse(reader.GetString());
                        continue;
                    case "qty":
                        workItem.Quantity = double.Parse(reader.GetString());
                        continue;
                    case "commission":
                        workItem.Commission = double.Parse(reader.GetString());
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

    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    internal class FullOrderResponseModelConverter : JsonConverter<FullOrderResponseModel>
    {
        /// <inheritdoc />
        public override FullOrderResponseModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var orderBook = new FullOrderResponseModel();
            orderBook.SetProperties(ref reader);

            return orderBook;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, FullOrderResponseModel value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
