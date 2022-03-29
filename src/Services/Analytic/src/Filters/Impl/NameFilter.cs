using Analytic.Models;
using System;
using System.Linq;

namespace Analytic.Filters
{
    /// <summary>
    ///     Фильтр наименования объекта торговли
    /// </summary>
    public class NameFilter : IFilter
    {
        /// <summary>
        ///     Фильтр наименования объекта торговли
        /// </summary>
        /// <param name="filterName"> Название фильтра </param>
        /// <param name="tradeObjectNamesToAnalyze"> Названия или части названий объектов торговли необходимые в работе </param>
        public NameFilter(string filterName, string[] tradeObjectNamesToAnalyze)
        {
            FilterName = filterName;
            TradeObjectNamesToAnalyze = tradeObjectNamesToAnalyze;
        }

        /// <inheritdoc />
        public string FilterName { get; }

        /// <inheritdoc />
        public string TargetTradeObjectName { get; }

        /// <summary>
        ///     Названия торговых объектов участвующих в анализе
        /// </summary>
        public string[] TradeObjectNamesToAnalyze { get; }

        /// <inheritdoc />
        public FilterType Type => FilterType.NameFilter;

        /// <inheritdoc />
        public bool CheckConditions(InfoModel model) =>
            TradeObjectNamesToAnalyze.Any(_ => model.TradeObjectName.Contains(_, StringComparison.InvariantCultureIgnoreCase));
    }
}
