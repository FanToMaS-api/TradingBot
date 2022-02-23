using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Модель ответа на отправку нового ордера (содержит полную информацию)
    /// </summary>
    public class FullOrderResponseModel
    {
        /// <summary>
        ///     Пара
        /// </summary>
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }

        /// <summary>
        ///     Id ордера
        /// </summary>
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }

        /// <summary>
        ///     Id клиентского ордера
        /// </summary>
        [JsonPropertyName("clientOrderId")]
        public string ClientOrderId { get; set; }

        /// <summary>
        ///     Время исполнения транзакции
        /// </summary>
        [JsonPropertyName("transactTime")]
        public long TransactTimeUnix { get; set; }

        /// <summary>
        ///     Если не OCO значение будет -1 всегда
        /// </summary>
        [JsonPropertyName("orderListId")]
        public long OrderListId { get; set; }

        /// <summary>
        ///     Цена
        /// </summary>
        [JsonPropertyName("price")]
        public double Price { get; set; }

        /// <summary>
        ///     Запрошенное кол-во
        /// </summary>
        [JsonPropertyName("origQty")]
        public double OrigQty { get; set; }

        /// <summary>
        ///     Исполненное кол-во
        /// </summary>
        [JsonPropertyName("executedQty")]
        public double ExecutedQty { get; set; }

        /// <summary>
        ///     Кол-во совокупной котировки
        /// </summary>
        [JsonPropertyName("cummulativeQuoteQty")]
        public double СumulativeQuoteQty { get; set; }

        /// <summary>
        ///     Статус выполнения ордера
        /// </summary>
        public OrderStatusType Status { get; set; }

        /// <summary>
        ///     Время жизни ордера
        /// </summary>
        public TimeInForceType TimeInForce { get; set; }

        /// <summary>
        ///     Тип ордера
        /// </summary>
        public OrderType OrderType { get; set; }

        /// <summary>
        ///     Тип ордера (покупка, продажа)
        /// </summary>
        public OrderSideType OrderSide { get; set; }

        [JsonPropertyName("fills")]
        public List<FillModel> Fills { get; set; } = new();

        /// <inheritdoc />
        public void SetProperties(ref Utf8JsonReader reader)
        {
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    switch (propertyName)
                    {
                        case "symbol":
                            Symbol = reader.GetString();
                            continue;
                        case "orderId":
                            OrderId = reader.GetInt64();
                            continue;
                        case "clientOrderId":
                            ClientOrderId = reader.GetString();
                            continue;
                        case "transactTime":
                            TransactTimeUnix = reader.GetInt64();
                            continue;
                        case "orderListId":
                            OrderListId = reader.GetInt64();
                            continue;
                        case "price":
                            Price = double.Parse(reader.GetString());
                            continue;
                        case "origQty":
                            OrigQty = double.Parse(reader.GetString());
                            continue;
                        case "executedQty":
                            ExecutedQty = double.Parse(reader.GetString());
                            continue;
                        case "cummulativeQuoteQty":
                            СumulativeQuoteQty = double.Parse(reader.GetString());
                            continue;
                        case "status":
                            Status = reader.GetString().ConvertToOrderStatusType();
                            continue;
                        case "timeInForce":
                            TimeInForce = reader.GetString().ConvertToTimeInForceType();
                            continue;
                        case "type":
                            OrderType = reader.GetString().ConvertToOrderType();
                            continue;
                        case "side":
                            OrderSide = reader.GetString().ConvertToOrderSideType();
                            continue;
                    }
                }

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    FillModel.CreateFillModel(ref reader, this);
                }
            }
        }
    }

    /// <summary>
    ///     Содержит информацию о частях заполнения ордера
    /// </summary>
    public class FillModel
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
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

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

                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
            }

            result.Fills.Add(workItem);
        }
    }

    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    public class FullOrderResponseModelConverter : JsonConverter<FullOrderResponseModel>
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
