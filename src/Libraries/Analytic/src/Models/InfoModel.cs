﻿using System;

namespace Analytic.Models
{
    /// <summary>
    ///     Модель совокупной информации об объекте торговли
    /// </summary>
    public class InfoModel
    {
        #region .ctors
        
        /// <inheritdoc cref="InfoModel"/>
        public InfoModel(string name)
        {
            TradeObjectName = name;
        }

        /// <inheritdoc cref="InfoModel"/>
        public InfoModel(string name, double lastPrice)
        {
            TradeObjectName = name;
            LastPrice = lastPrice;
        }

        #endregion

        #region Properties
        
        /// <summary>
        ///     Название объекта торговли
        /// </summary>
        public string TradeObjectName { get; internal set; }

        /// <summary>
        ///     Последняя цена
        /// </summary>
        public double LastPrice { get; internal set; }

        /// <summary>
        ///     Предпоследняя цена
        /// </summary>
        [Obsolete("Не используется")]
        public double PrevPrice { get; internal set; }

        /// <summary>
        ///     Объем спроса
        /// </summary>
        public double BidVolume { get; internal set; }

        /// <summary>
        ///     Объем предложения
        /// </summary>
        public double AskVolume { get; internal set; }

        /// <summary>
        ///     Последнее отклонение
        /// </summary>
        public double LastDeviation { get; internal set; }

        /// <summary>
        ///     Суммарное отклонение за какое-то кол-во таймфреймов
        /// </summary>
        public double DeviationsSum { get; internal set; }

        #endregion
    }
}
