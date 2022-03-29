using System.Collections.Generic;

namespace Analytic.Models
{
    /// <summary>
    ///     Модель совокупной информации об объекте торговли
    /// </summary>
    public class InfoModel
    {
        /// <inheritdoc cref="InfoModel"/>
        public InfoModel(string name, double lastPrice)
        {
            TradeObjectName = name;
            LastPrice = lastPrice;
        }

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
        ///     Последнее отклонение
        /// </summary>
        public double LastDeviation { get; internal set; }

        /// <summary>
        ///     Суммарное отклонение за 5 таймфреймов
        /// </summary>
        public double SumDeviations { get; internal set; }

        /// <summary>
        ///     Отклонения цены за последние таймфреймы в процентах
        /// </summary>
        public List<double> PricePercentDeviations { get; internal set; } = new();
    }
}
