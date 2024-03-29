﻿using Common.JsonConvertWrapper;
using System.Text.Json;

namespace BinanceExchange.Models
{
    /// <summary>
    ///     Модель свечи для пары
    /// </summary>
    internal class CandlestickModel
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
        public double BasePurchaseVolume { get; set; }

        /// <summary>
        ///     Объем актива по котировке тейкера на покупку
        /// </summary>
        public double QuotePurchaseVolume { get; set; }

        /// <summary>
        ///     Устанавливает св-ва для <see cref="CandlestickModel"/>
        /// </summary>
        /// <param name="reader"> Reader с указателем на начало массива с данными о свече </param>
        internal static CandlestickModel Create(ref Utf8JsonReader reader)
        {
            var result = new CandlestickModel();

            reader.Read();
            result.OpenTimeUnix = reader.ReadLongAndNext();
            result.OpenPrice = reader.ReadDoubleAndNext();
            result.MaxPrice = reader.ReadDoubleAndNext();
            result.MinPrice = reader.ReadDoubleAndNext();
            result.ClosePrice = reader.ReadDoubleAndNext();
            result.Volume = reader.ReadDoubleAndNext();
            result.CloseTimeUnix = reader.ReadLongAndNext();
            result.QuoteAssetVolume = reader.ReadDoubleAndNext();
            result.TradesNumber = reader.ReadIntAndNext();
            result.BasePurchaseVolume = reader.ReadDoubleAndNext();
            result.QuotePurchaseVolume = reader.ReadDoubleAndNext();

            return result;
        }
    }
}
