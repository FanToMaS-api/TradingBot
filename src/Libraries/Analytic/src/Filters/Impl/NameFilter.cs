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
        #region .ctor
        
        /// <summary>
        ///     Фильтр наименования объекта торговли
        /// </summary>
        /// <param name="filterName"> Название фильтра </param>
        /// <param name="tradeObjectNamesToAnalyze"> Названия объектов торговли, которые смогут пройти фильтр </param>
        internal NameFilter(string filterName, string[] tradeObjectNamesToAnalyze)
        {
            Name = filterName;
            TradeObjectNamesToAnalyze = tradeObjectNamesToAnalyze;
        }

        #endregion

        #region Implemented properties of IFilter

        /// <inheritdoc />
        public string Name { get; internal set; }

        /// <inheritdoc />
        public FilterType Type => FilterType.NameFilter;

        #endregion

        #region Implemented public methods of IFilter

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        /// <inheritdoc />
        public async Task<bool> CheckConditionsAsync(IServiceScopeFactory _, InfoModel model, CancellationToken __)
        {
            return TradeObjectNamesToAnalyze.Any(_ => model.TradeObjectName.Contains(_, StringComparison.InvariantCultureIgnoreCase));
        }

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        #endregion

        #region Properties

        /// <summary>
        ///     Названия торговых объектов участвующих в анализе
        /// </summary>
        public string[] TradeObjectNamesToAnalyze { get; }

        #endregion
    }
}
