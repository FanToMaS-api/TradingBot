using Common.JsonConvertWrapper;
using BinanceExchange.Enums;
using BinanceExchange.Enums.Helper;
using System.Collections.Generic;
using System.Text.Json;

namespace BinanceExchange.Models
{
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
            while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var lastPropertyName = reader.GetString();
                    reader.Read();

                    if (lastPropertyName == "filters")
                    {
                        reader.SkipToSection("permissions");
                    }

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
}
