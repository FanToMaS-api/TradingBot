using Analytic.Filters.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Analytic.AnalyticUnits.Profiles.ML.Models.ArgumentsCreators
{
    /// <summary>
    ///     Вычисляет аргументы для прогноза
    /// </summary>
    internal class ArgumentsCreator
    {
        #region Fields

        private const int _maxNumberPricesToForecast = 150; // максимальное кол-во данных для прогноза
        private const int _minNumberPricesToForecast = 10; // минимальное кол-во данных для прогноза
        private const int _denominator = 25; // делитель кол-ва данных, участвующих в предсказании цены
                                             // для прогноза только некоторого кол-ва цен

        private readonly IEnumerable<IObjectForMachineLearning> _data;
        private readonly int _dataLength;
        private readonly AggregateDataIntervalType _aggregateDataInterval;

        #endregion
        
        #region .ctor

        /// <inheritdoc cref="ArgumentsCreator"/>
        /// <remarks> 
        ///     Данные должны быть строго одного интервала агрегирования
        /// </remarks>
        internal ArgumentsCreator(IEnumerable<IObjectForMachineLearning> data)
        {
            _data = data;
            data.TryGetNonEnumeratedCount(out var dataLength);
            _dataLength = dataLength;
            _aggregateDataInterval = data.First().AggregateDataInterval;
        }

        #endregion
        
        #region Public methods

        /// <summary>
        ///     Получить число спрогнозируемых данных
        /// </summary>
        internal int GetNumberPricesToForecast()
        {
            var numberPricesToForecast = _dataLength / _denominator;
            return numberPricesToForecast is > _minNumberPricesToForecast and < _maxNumberPricesToForecast
                ? numberPricesToForecast
                : _maxNumberPricesToForecast;
        }

        /// <summary>
        ///     Получить длину интервала разделения данных
        /// </summary>
        internal int GetSeriesLength() =>
            _aggregateDataInterval switch
            {
                AggregateDataIntervalType.Default => 100,
                AggregateDataIntervalType.OneMinute => 60,
                AggregateDataIntervalType.FiveMinutes => 12,
                AggregateDataIntervalType.FifteenMinutes => 5,
                AggregateDataIntervalType.OneHour => 5,
                _ => throw new Exception($"Unknown type of {nameof(AggregateDataIntervalType)} = '{_aggregateDataInterval}'")
            };

        /// <summary>
        ///     Получить размер окна для анализа каждого из образцов
        /// </summary>
        internal int GetWindowSize() =>
            _aggregateDataInterval switch
            {
                AggregateDataIntervalType.Default => 60,
                AggregateDataIntervalType.OneMinute => 15,
                AggregateDataIntervalType.FiveMinutes => 12,
                AggregateDataIntervalType.FifteenMinutes => 7,
                AggregateDataIntervalType.OneHour => 5,
                _ => throw new Exception($"Unknown type of {nameof(AggregateDataIntervalType)} = '{_aggregateDataInterval}'")
            };

        #endregion
    }
}
