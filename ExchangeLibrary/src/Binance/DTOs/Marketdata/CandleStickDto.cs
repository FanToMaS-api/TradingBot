using System;
using System.Linq;
using System.Text.Json.Serialization;

namespace ExchangeLibrary.Binance.DTOs.Marketdata
{
    /// <summary>
    ///     Модель свечи для монеты
    /// </summary>
    public class CandleStickDto
    {
        /// <summary>
        ///     Время открытия
        /// </summary>
        public long OpenTimeUnix { get; set; }

        /// <summary>
        ///     Цена открытия
        /// </summary>
        public double OpenPrice { get; set; }

        /// <summary>
        ///     Минимальная цена
        /// </summary>
        public double MinPrice { get; set; }

        /// <summary>
        ///     Максимальная цена
        /// </summary>
        public double MaxPrice { get; set; }

        /// <summary>
        ///     Объем
        /// </summary>
        public double Volume { get; set; }

        /// <summary>
        ///     Цена закрытия
        /// </summary>
        public double ClosePrice { get; set; }

        /// <summary>
        ///     Время закрытия
        /// </summary>
        public long CloseTimeUnix { get; set; }

        /// <summary>
        ///     Объем котируемого актива
        /// </summary>
        public double QuoteAssetVolume { get; set; }

        /// <summary>
        ///     Кол-во сделок
        /// </summary>
        public int TradesNumber { get; set; }

        /// <summary>
        ///     Объем базового актива, который купили тейкеры
        /// </summary>
        public double BasePurchaseVolume{ get; set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        public double QuotePurchaseVolume { get; set; }

        /// <summary>
        ///     Игнорировать
        /// </summary>
        public string Ignore { get; set; }
    }

    /// <summary>
    ///     Нормально конвертирует полученные данные
    /// </summary>
    public class CandleStickDtoConverter : JsonConverter<CandleStickDto>
    {
        public override bool CanWrite => false;

        public override CandleStickDto ReadJson(
            JsonReader reader,
            Type objectType,
            CandleStickDto existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            var result = new CandleStickDto();

            // Load JObject from stream
            var jArray = JArray.Load(reader);
            if (jArray is null)
            {
                return result;
            }
            
            // TODO: возможно стоит подумать над улучшением
            var collection = jArray.Values().ToList();
            for (var i = 0; i < collection.Count; i++)
            {
                result.OpenTimeUnix = (long)collection[i++];
                result.OpenPrice = (double)collection[i++];
                result.MaxPrice = (double)collection[i++];
                result.MinPrice = (double)collection[i++];
                result.ClosePrice = (double)collection[i++];
                result.Volume = (double)collection[i++];
                result.CloseTimeUnix = (long)collection[i++];
                result.QuoteAssetVolume = (double)collection[i++];
                result.TradesNumber = (int)collection[i++];
                result.BasePurchaseVolume = (double)collection[i++];
                result.QuotePurchaseVolume = (double)collection[i++];
                result.Ignore = (string)collection[i];
            }

            return result;
        }

        public override void WriteJson(JsonWriter writer, CandleStickDto value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
