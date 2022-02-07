using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExchangeLibrary.Binance.DTOs.Marketdata
{
    /// <summary>
    ///     Модель книги заказов
    /// </summary>
    public class OrderBookDto
    {
        /// <summary>
        ///    Идентификатор последнего обновления 
        /// </summary>
        public long LastUpdateId { get; set; }

        /// <summary>
        ///     Список цен/объемов на покупку
        /// </summary>
        public List<PriceQtyPair> Bids { get; set; }

        /// <summary>
        ///     Список цен/объемов на продажу
        /// </summary>
        public List<PriceQtyPair> Asks { get; set; }
    }

    /// <summary>
    ///     Модель цены и объема монеты
    /// </summary>
    public class PriceQtyPair
    {
        /// <summary>
        ///     Цена монеты
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Qty { get; set; }
    }

    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    public class OrderBookDtoConverter : JsonConverter<OrderBookDto>
    {
        public override bool CanWrite => false;

        public override OrderBookDto ReadJson(
            JsonReader reader,
            Type objectType,
            OrderBookDto existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            // Load JObject from stream
            var jObject = JObject.Load(reader);
            var result = new OrderBookDto();
            if (jObject["lastUpdateId"] is not null)
            {
                result.LastUpdateId = (long)jObject.GetValue("lastUpdateId");
            }

            if (jObject["bids"] is not null)
            {
                result.Bids = CreatePairList(jObject.GetValue("bids"));
            }

            if (jObject["asks"] is not null)
            {
                result.Asks = CreatePairList(jObject.GetValue("asks"));
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, OrderBookDto value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private List<PriceQtyPair> CreatePairList(JToken token)
        {
            if (token is null)
            {
                return new();
            }

            var result = new List<PriceQtyPair>();
            var collection = token.Values().ToList();
            for (var i = 0; i < collection.Count; i++)
            {
                result.Add(new PriceQtyPair 
                { 
                    Price = (double)collection[i++], 
                    Qty = (double)collection[i] 
                });
            }

            return result;
        }
    }

}
