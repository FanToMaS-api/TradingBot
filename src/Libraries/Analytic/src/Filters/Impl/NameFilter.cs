using Analytic.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

        /// <summary>
        ///     Названия торговых объектов участвующих в анализе
        /// </summary>
        public string[] TradeObjectNamesToAnalyze { get; }

        /// <inheritdoc />
        public FilterType Type => FilterType.NameFilter;

        /// <inheritdoc />
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> CheckConditionsAsync(IServiceScopeFactory _, InfoModel model, CancellationToken __) =>
            TradeObjectNamesToAnalyze.Any(_ => model.TradeObjectName.Contains(_, StringComparison.InvariantCultureIgnoreCase));
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
