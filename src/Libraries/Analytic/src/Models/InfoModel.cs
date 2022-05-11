using Analytic.Filters;
using System.Collections.Generic;
using System.Linq;

namespace Analytic.Models
{
    /// <summary>
    ///     Модель совокупной информации об объекте торговли
    /// </summary>
    public class InfoModel
    {
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

        /// <summary>
        ///     Отклонения цены за последние таймфреймы в процентах
        /// </summary>
        public IQueryable<double> PricePercentDeviations { get; internal set; }

        #region Public methods

        /// <summary>
        ///     Вычисляет суммарное отклонение цен за указанное кол-во таймфреймов
        /// </summary>
        /// <param name="timeframeNumber"> Кол-во таймфреймов </param>
        /// <remarks>
        ///     Вызывается в <see cref="PriceDeviationFilter"/>
        /// </remarks>
        public void ComputeDeviationsSum(int timeframeNumber)
        {
            var deviationsSum = PricePercentDeviations.Take(timeframeNumber).Sum();
            DeviationsSum = deviationsSum;
        }

        #endregion
    }
}
