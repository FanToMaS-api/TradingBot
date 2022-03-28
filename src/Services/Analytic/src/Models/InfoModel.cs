﻿using System.Collections.Generic;

namespace Analytic.Models
{
    /// <summary>
    ///     Модель совокупной информации об объекте торговли
    /// </summary>
    public class InfoModel
    {
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
        public double PrevPrice { get; internal set; }

        /// <summary>
        ///     Объем предложения
        /// </summary>
        public double BidVolume { get; internal set; }

        /// <summary>
        ///     Объем спроса
        /// </summary>
        public double AskVolume { get; internal set; }

        /// <summary>
        ///     Отклонения цены за последние таймфреймы в процентах
        /// </summary>
        public List<double> PricePercentDeviations { get; internal set; } = new();
    }
}
