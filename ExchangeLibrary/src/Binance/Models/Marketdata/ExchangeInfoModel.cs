using Common.JsonConvertWrapper;
using ExchangeLibrary.Binance.Enums;
using ExchangeLibrary.Binance.Enums.Helper;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.Models
{
    /// <summary>
    ///     Информация о правилах текущей торговли парами
    /// </summary>
    internal class ExchangeInfoModel
    {
        /// <summary>
        ///     Правила торговли для символа
        /// </summary>
        public List<SymbolInfoModel> Symbols { get; set; } = new();
    }

    /// <summary>
    ///     Содержит инфу о правилах торговли парой
    /// </summary>
    internal class SymbolInfoModel
    {
        /// <summary>
        ///     Пара
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        ///     Статус пары
        /// </summary>
        public SymbolStatusType Status { get; set; }

        /// <summary>
        ///     Базовая валюта
        /// </summary>
        public string BaseAsset { get; set; }

        /// <summary>
        ///     Требуемое количество символов базовой валюты после запятой при создании ордера (для цены и количества)
        /// </summary>
        public int BaseAssetPrecision { get; set; }

        /// <summary>
        ///     Квотируемая валюта
        /// </summary>
        public string QuoteAsset { get; set; }

        /// <summary>
        ///     Требуемое количество символов квотируемой валюты после запятой при создании ордера (для цены и количества)
        /// </summary>
        public int QuotePrecision { get; set; }

        /// <summary>
        ///     Допустимые виды ордеров по паре
        /// </summary>
        public OrderType[] OrderTypes { get; set; }

        /// <summary>
        ///     Разрешено ли создание айсбергов
        /// </summary>
        public bool IsIcebergAllowed { get; set; }

        /// <summary>
        ///     Разрешено ли создание OCO-ордеров
        /// </summary>
        public bool IsOcoAllowed { get; set; }

        /// <summary>
        ///     Устанавливает занчения св-в
        /// </summary>
        public void SetProperties(ref Utf8JsonReader reader)
        {
            string lastPropertyName = "";
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    lastPropertyName = reader.GetString();
                    reader.Read();

                    switch (lastPropertyName)
                    {
                        case "symbol":
                            Symbol = reader.GetString();
                            continue;
                        case "status":
                            Status = reader.GetString().ConvertToSymbolStatusType();
                            continue;
                        case "baseAsset":
                            BaseAsset = reader.GetString();
                            continue;
                        case "baseAssetPrecision":
                            BaseAssetPrecision = reader.GetInt32();
                            continue;
                        case "quoteAsset":
                            QuoteAsset = reader.GetString();
                            continue;
                        case "quotePrecision":
                            QuotePrecision = reader.GetInt32();
                            continue;
                        case "orderTypes":
                            {
                                OrderTypes = GetOrderTypes(ref reader);
                                continue;
                            }
                        case "icebergAllowed":
                            IsIcebergAllowed = reader.GetBoolean();
                            continue;
                        case "ocoAllowed":
                            IsOcoAllowed = reader.GetBoolean();
                            continue;
                    }
                }
            }

            OrderType[] GetOrderTypes(ref Utf8JsonReader reader)
            {
                var result = new List<OrderType>();
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    var type = reader.GetString();
                    result.Add(type.ConvertToOrderType());
                }

                return result.ToArray();
            }
        }
    }

    #region ExchangeInfoModelConverter

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

    #endregion
}
